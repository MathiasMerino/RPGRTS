using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class VisualPolishTests
    {
        [Test]public void VisualScenePreservesPhysicsAndOriginalRules()
        {
            EditorSceneManager.OpenScene(ExpeditionBuilder.ScenePath);var original=PhysicsSignature();var originalRules=Object.FindAnyObjectByType<GameSession>().Rules;
            EditorSceneManager.OpenScene(VisualPolishBuilder.ScenePath);Assert.That(PhysicsSignature(),Is.EqualTo(original));Assert.That(Object.FindAnyObjectByType<GameSession>().Rules,Is.SameAs(originalRules));
            Assert.That(GameObject.Find("E16 visual environment · no physics").GetComponentsInChildren<Collider>(true),Is.Empty);
        }
        private static string[] PhysicsSignature()=>Object.FindObjectsByType<Collider>(FindObjectsInactive.Include).Select(c=>c.GetType().Name+"|"+c.name+"|"+c.transform.position.ToString("F4")+"|"+c.bounds.size.ToString("F4")+"|"+c.enabled+"|"+c.isTrigger).OrderBy(s=>s,StringComparer.Ordinal).ToArray();
        [Test]public void SharedArtAndLibraryAreReplaceableAndComplete()
        {
            var p=AssetDatabase.LoadAssetAtPath<VisualPresentationProfile>(VisualPolishBuilder.Root+"/Presentation.asset");Assert.That(p.Classes.Length,Is.EqualTo(3));Assert.That(p.CardPrefab,Is.Not.Null);Assert.That(p.ButtonPrefab,Is.Not.Null);
            Assert.That(Directory.GetFiles(VisualPolishBuilder.Root+"/Prefabs","*.prefab").Length,Is.EqualTo(9));
            foreach(var art in p.Classes){Assert.That(art.Figure,Is.Not.Null);Assert.That(art.Portrait,Is.Not.Null);Assert.That(AssetDatabase.GetAssetPath(art.Figure.texture),Does.Contain("ProductionProvisional/Source"));Assert.That(art.Figure.rect.height,Is.GreaterThan(art.Figure.rect.width));}
        }
        [UnityTest,Timeout(600000)]public IEnumerator MenusClassesAndPanelsFitAndRetainCommands()=>Presentation();
        private static IEnumerator Presentation()
        {
            EditorSceneManager.OpenScene(VisualPolishBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E16/test-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;flow.Front.Preferences.StorageKey="Aethernia.Tests.E16."+Guid.NewGuid().ToString("N");flow.ShowMain();
            Directory.CreateDirectory("Logs/E16/Captures");yield return TacticalCombatIntegrationTests.Wait(.3f);
            foreach(var size in Sizes())yield return Capture("Menu",size,true);
            FirstDemoTests.Click(flow.Front,"new");yield return TacticalCombatIntegrationTests.Wait(.3f);
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                FirstDemoTests.Click(flow.Front,"class-"+id);Assert.That(flow.Front.ChosenClass,Is.EqualTo(id));Assert.That(flow.Front.GetComponentsInChildren<ClassCardView>().Count(c=>c.Outline.enabled),Is.EqualTo(1));
                foreach(var card in flow.Front.GetComponentsInChildren<ClassCardView>()){Assert.That(card.Figure.sprite,Is.Not.Null);Assert.That(card.Figure.preserveAspect,Is.True);}
                foreach(var size in Sizes())yield return Capture("Class-"+id,size,true);
            }
            FirstDemoTests.Click(flow.Front,"character-back");FirstDemoTests.Click(flow.Front,"options");yield return TacticalCombatIntegrationTests.Wait(.3f);foreach(var size in Sizes())yield return Capture("Options",size,true);FirstDemoTests.Click(flow.Front,"apply");
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",flow.Session.Rules,out var state),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;yield return null;
            foreach(var page in new[]{ExplorationPage.Character,ExplorationPage.Inventory,ExplorationPage.Session})
            {flow.ExistingHud.SelectPage(page);yield return null;foreach(var size in Sizes())yield return Capture("Hud-"+page,size,false);flow.ExistingHud.ClosePanels();Assert.That(flow.ExistingHud.CompactView.WindowOpen,Is.False);}
            Assert.That(flow.Director.Save(flow.SlotPath),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;yield return Capture("Village",new Vector2Int(1280,720),false);
            var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();Assert.That(dungeon.Travel(true),Is.True);yield return TacticalCombatIntegrationTests.Wait(1.5f);Assert.That(Object.FindAnyObjectByType<RegionalAtmosphere>().Dungeon,Is.True);Assert.That(Object.FindAnyObjectByType<RegionalAtmosphere>().Sun.intensity,Is.LessThan(.75));yield return Capture("Dungeon",new Vector2Int(1280,720),false);
            Assert.That(flow.Director.Hero.TryMoveTo(new Vector3(76,0,-2)),Is.True);yield return ExpeditionIntegrationTests.Arrive(flow.Director.Hero);dungeon.TestSeed=42;Assert.That(ExpeditionIntegrationTests.Find("umbral-battle-1").TryInteract(flow.Director.Hero,flow.Session),Is.True);
            foreach(var size in Sizes())yield return DemoCapture.Save("Logs/E16/Captures/Group-"+size.x+".png",size.x,size.y,validate:ValidateGroup);
            PlayerPrefs.DeleteKey(flow.Front.Preferences.StorageKey);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static Vector2Int[] Sizes()=>new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080),new Vector2Int(1024,768)};
        private static IEnumerator Capture(string name,Vector2Int size,bool front)
        {yield return DemoCapture.Save("Logs/E16/Captures/"+name+"-"+size.x+".png",size.x,size.y,validate:front?ValidateFront:ValidateHud);}
        private static void ValidateFront(Camera camera,int width,int height)
        {
            var front=Object.FindAnyObjectByType<DemoFrontEnd>();
            foreach(var b in front.Controls.Values)if(b.gameObject.activeInHierarchy)CheckButton(b,camera,width,height,true);
            foreach(var text in front.GetComponentsInChildren<TMP_Text>()){text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,text.name+": "+text.text+" rect="+text.rectTransform.rect+" font="+text.fontSize);}
        }
        private static void ValidateHud(Camera camera,int width,int height)
        {
            var flow=Object.FindAnyObjectByType<DemoFlow>();foreach(var button in flow.ExistingHud.GetComponentsInChildren<Button>())CheckButton(button,camera,width,height,false);
            foreach(var bridge in flow.ExistingHud.GetComponentsInChildren<LegacySdfPresenter>())Assert.That(bridge.Source.enabled,Is.False,"No doble dibujo de fuente legado");
            foreach(var text in flow.ExistingHud.GetComponentsInChildren<TMP_Text>()){text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,text.name+": "+text.text+" rect="+text.rectTransform.rect+" font="+text.fontSize);}
        }
        private static void ValidateGroup(Camera camera,int width,int height)
        {
            var panel=Object.FindAnyObjectByType<ExpeditionPanel>();foreach(var b in panel.Controls.Values)if(b.gameObject.activeInHierarchy)CheckButton(b,camera,width,height,true);
            var root=panel.Controls["open"].transform.parent;foreach(var t in root.GetComponentsInChildren<TMP_Text>()){t.ForceMeshUpdate();Assert.That(t.isTextOverflowing,Is.False,"Group label: "+t.text+" rect="+t.rectTransform.rect);}
        }
        private static void CheckButton(Button b,Camera camera,int width,int height,bool raycast)
        {
            var corners=new Vector3[4];b.GetComponent<RectTransform>().GetWorldCorners(corners);var min=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);var max=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
            Assert.That(min.x,Is.GreaterThanOrEqualTo(-1),b.name);Assert.That(min.y,Is.GreaterThanOrEqualTo(-1),b.name);Assert.That(max.x,Is.LessThanOrEqualTo(width+1),b.name);Assert.That(max.y,Is.LessThanOrEqualTo(height+1),b.name);
            if(!raycast)return;var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=(min+max)*.5f},hits);Assert.That(hits.Count,Is.GreaterThan(0),b.name);Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(b),b.name);
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
