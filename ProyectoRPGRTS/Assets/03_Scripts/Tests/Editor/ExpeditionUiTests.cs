using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class ExpeditionUiTests
    {
        [UnityEngine.TestTools.UnityTest,Timeout(600000)]public IEnumerator WindowsAndCombatFitAndReceiveInput()=>Run();
        private static IEnumerator Run()
        {
            EditorSceneManager.OpenScene(ExpeditionBuilder.ScenePath);yield return new UnityEngine.TestTools.EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var panel=Object.FindAnyObjectByType<ExpeditionPanel>();var party=Object.FindAnyObjectByType<PartyWorld>();var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();
            // Fixture de presentación: seis soldados permiten comprobar la segunda fila completa.
            var state=ExpeditionRulesTests.Ready();for(var i=0;i<6;i++)state.Army.Add(new RecruitRecord{Id=Guid.NewGuid().ToString("N"),Role=i%2==0?"archer":"swordsman",Health=10});
            flow.SlotPath="Logs/E14/ui-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;
            Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;
            foreach(var role in party.Roles.Roles)Assert.That(party.Recruit(role.Id),Is.True);panel.Open();yield return null;
            flow.ExistingHud.CompactView.Open();yield return null;Assert.That(flow.Session.WorldUiOpen,Is.False);Assert.That(panel.Controls["close"].gameObject.activeInHierarchy,Is.False);Assert.That(flow.ExistingHud.CompactView.WindowOpen,Is.True);
            flow.ExistingHud.ClosePanels();panel.Open();yield return null;flow.ExistingHud.CompactView.ToggleHelp();yield return null;Assert.That(flow.Session.WorldUiOpen,Is.False);Assert.That(flow.ExistingHud.CompactView.HelpOpen,Is.True);flow.ExistingHud.ClosePanels();panel.Open();yield return null;
            Directory.CreateDirectory("Logs/E14/UI");
            foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1024,768)})
                foreach(var tab in new[]{"party-tab","army-tab","territory-tab"})
                {panel.Controls[tab].onClick.Invoke();yield return DemoCapture.Save("Logs/E14/UI/"+tab+"-"+size.x+".png",size.x,size.y,validate:Validate);}
            panel.Close();Assert.That(dungeon.Travel(true),Is.True);Assert.That(flow.Director.Hero.TryMoveTo(new Vector3(76,0,-2)),Is.True);yield return ExpeditionIntegrationTests.Arrive(flow.Director.Hero);
            dungeon.TestSeed=42;Assert.That(ExpeditionIntegrationTests.Find("umbral-battle-1").TryInteract(flow.Director.Hero,flow.Session),Is.True);
            flow.ExistingHud.CompactView.ToggleHelp();yield return null;Assert.That(panel.Controls["group-attack"].gameObject.activeInHierarchy,Is.False);flow.ExistingHud.ClosePanels();yield return null;Assert.That(panel.Controls["group-attack"].gameObject.activeInHierarchy,Is.True);
            foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1024,768)})yield return DemoCapture.Save("Logs/E14/UI/group-"+size.x+".png",size.x,size.y,validate:Validate);
            UnityEngine.TestTools.LogAssert.NoUnexpectedReceived();yield return new UnityEngine.TestTools.ExitPlayMode();
        }
        private static void Validate(Camera camera,int width,int height)
        {
            var panel=Object.FindAnyObjectByType<ExpeditionPanel>();var root=panel.Controls["open"].transform.parent;
            Assert.That(root.GetSiblingIndex(),Is.EqualTo(root.parent.childCount-1),"Window must render over the existing HUD");
            foreach(var pair in panel.Controls)
            {
                var b=pair.Value;if(!b.gameObject.activeInHierarchy)continue;var points=new Vector3[4];b.GetComponent<RectTransform>().GetWorldCorners(points);
                var min=RectTransformUtility.WorldToScreenPoint(camera,points[0]);var max=RectTransformUtility.WorldToScreenPoint(camera,points[2]);
                Assert.That(min.x,Is.GreaterThanOrEqualTo(-1),pair.Key);Assert.That(min.y,Is.GreaterThanOrEqualTo(-1),pair.Key);Assert.That(max.x,Is.LessThanOrEqualTo(width+1),pair.Key);Assert.That(max.y,Is.LessThanOrEqualTo(height+1),pair.Key);
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=(min+max)*.5f},hits);
                Assert.That(hits.Count,Is.GreaterThan(0),pair.Key);Assert.That(hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>(),Is.SameAs(b),"Button obstructed: "+pair.Key);
            }
            foreach(var text in root.GetComponentsInChildren<TMP_Text>()){text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,"Clipped text: "+text.name+" / "+text.text);}
            if(panel.Flow.Director.Encounter.GroupActive)
            {
                var hero=camera.WorldToScreenPoint(panel.Flow.Director.Hero.transform.position+Vector3.up*.9f);var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=hero},hits);
                Assert.That(hits,Is.Empty,"Default combat framing must keep the hero clear of the HUD");
            }
            foreach(var button in panel.Flow.ExistingHud.GetComponentsInChildren<UnityEngine.UI.Button>())
            {
                var points=new Vector3[4];button.GetComponent<RectTransform>().GetWorldCorners(points);var min=RectTransformUtility.WorldToScreenPoint(camera,points[0]);var max=RectTransformUtility.WorldToScreenPoint(camera,points[2]);
                Assert.That(min.x,Is.GreaterThanOrEqualTo(-1),button.name);Assert.That(min.y,Is.GreaterThanOrEqualTo(-1),button.name);Assert.That(max.x,Is.LessThanOrEqualTo(width+1),"Existing HUD clipped: "+button.name);Assert.That(max.y,Is.LessThanOrEqualTo(height+1),button.name);
            }
        }
        [UnityEngine.TestTools.UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();}
    }
}
