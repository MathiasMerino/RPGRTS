using System.Collections;
using System.Linq;
using System.IO;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;
namespace Aethernia.Tests.Editor
{
    public sealed class ProgressionIntegrationTests
    {
        [UnityTest]public IEnumerator RetryIsExplicitAndGroupUiHasOneCommandSet()=>Run();
        private static IEnumerator Run()
        {
            EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;flow.SlotPath="Logs/E18/integration-"+System.Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;flow.EncounterSeed=42;
            Assert.That(flow.StartNew("Controlled E18 retry fixture","warrior","human"),Is.True);flow.CompleteIntro();yield return null;
            // Controlled failure state only; not counted as a played victory or native journey.
            flow.Session.State.Health=0;flow.Session.Paused=true;flow.RetryFirstCombat();
            Assert.That(flow.Session.State.Health,Is.EqualTo(flow.Session.HeroProfile.Health));Assert.That(d.Encounter.Battle,Is.Not.Null);Assert.That(flow.Session.State.Experience,Is.Zero);
            d.Encounter.Clear();flow.Session.SetMode(PrototypeMode.Exploration);var s=flow.Session.State;s.Facts.Add("rpg-victory");flow.Session.AwardForFact("rpg-victory");s.Health-=3;var health=s.Health;flow.Session.SelectClass("warrior");Assert.That(s.Health,Is.EqualTo(health),"class selection cannot heal");
            s.Demo.Chapter=DemoChapter.Settlement;s.Facts.Add("demo-camp-secured");flow.Session.AwardForFact("demo-camp-secured");Assert.That(d.Hero.Warp(new Vector3(9,0,-10)),Is.True);
            var party=Object.FindAnyObjectByType<PartyWorld>();foreach(var role in party.Roles.Roles)Assert.That(party.Recruit(role.Id),Is.True);yield return null;
            var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();Assert.That(dungeon.Travel(true),Is.True);yield return null;Assert.That(d.Encounter.BeginGroup(new Vector3(78,0,0),42),Is.True);yield return null;yield return null;
            var data=new SerializedObject(flow.ExistingHud);var bindings=data.FindProperty("bindings");
            for(var i=0;i<bindings.arraySize;i++){var binding=bindings.GetArrayElementAtIndex(i);var id=binding.FindPropertyRelative("Id").stringValue;var button=(UnityEngine.UI.Button)binding.FindPropertyRelative("Button").objectReferenceValue;if(new[]{"attack","guard","recover","end","approach"}.Contains(id))Assert.That(button.gameObject.activeInHierarchy,Is.False,"group legacy action hidden "+id);if(id=="technique")Assert.That(button.interactable,Is.EqualTo(!d.Encounter.Group.Finished&&d.Encounter.Group.Current.Hero&&d.Encounter.Group.ActionAvailable&&!d.Hero.IsMoving));}
            var panel=Object.FindAnyObjectByType<ExpeditionPanel>();var combat=panel.Controls["target-0"].transform.parent;Assert.That(string.Join(" ",combat.GetComponentsInChildren<TMPro.TMP_Text>().Select(t=>t.text)),Does.Not.Contain("enemy-"));
            Directory.CreateDirectory("Logs/E18/Captures");yield return DemoCapture.Save("Logs/E18/Captures/Group-1280.png",1280,720);yield return DemoCapture.Save("Logs/E18/Captures/Group-1920.png",1920,1080);
            d.Encounter.Clear();flow.Session.SetMode(PrototypeMode.Exploration);Assert.That(flow.Session.LastProgressionNotice,Is.Not.Empty);Assert.That(d.Save(flow.SlotPath),Is.True);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.LastProgressionNotice,Is.Empty,"no stale XP notice after load");LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
