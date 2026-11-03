using System;
using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class UxExperienceTests
    {
        [Test]public void ScenePreservesE19AndUsesPublicDependencies()
        {
            EditorSceneManager.OpenScene(UxExperienceBuilder.ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();Assert.That(flow,Is.Not.Null);Assert.That(flow.Director,Is.Not.Null);Assert.That(flow.CameraRig,Is.Not.Null);Assert.That(flow.GetComponent<UxExperienceView>().Flow,Is.SameAs(flow));
            var rules=AssetDatabase.LoadAssetAtPath<PrototypeRules>(UxExperienceBuilder.Root+"/PrototypeRules.asset");
            var previous=AssetDatabase.LoadAssetAtPath<PrototypeRules>(CivilizationSceneBuilder.RulesPath);
            Assert.That(rules.UxExperience,Is.True);Assert.That(previous.UxExperience,Is.False);
            Assert.That(rules.Civilizations,Is.SameAs(previous.Civilizations));Assert.That(rules.Progression,Is.SameAs(previous.Progression));
            Assert.That(AssetDatabase.GetDependencies(UxExperienceBuilder.ScenePath,true).Any(p=>p.Contains("Meshy")||p.Contains("LocalOnly")),Is.False);
        }
        [Test]public void SaveRejectsInvalidPreparationAndReadsLegacyWithoutStartingTutorial()
        {
            var rules=AssetDatabase.LoadAssetAtPath<PrototypeRules>(UxExperienceBuilder.Root+"/PrototypeRules.asset");
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",rules,out var old),Is.True);
            Assert.That(old.Demo.UxRevision,Is.Zero);Assert.That(old.Demo.Chapter,Is.GreaterThanOrEqualTo(DemoChapter.Settlement));
            old.Demo.UxFormation=new[]{new Vector3(float.NaN,0,0)};Assert.That(PrototypeSaveStore.Valid(old,rules),Is.False);
        }
        [UnityTest]public IEnumerator WarriorFullRoute()=>Route("warrior","human");
        [UnityTest]public IEnumerator MonkFullRoute()=>Route("monk","elf");
        [UnityTest]public IEnumerator MageFullRoute()=>Route("mage","dwarf");
        private static IEnumerator Route(string cls,string race)
        {
            EditorSceneManager.OpenScene(UxExperienceBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            yield return RunningRoute(cls,race);
            yield return new ExitPlayMode();
        }
        private static IEnumerator RunningRoute(string cls,string race)
        {
            var flow=Object.FindAnyObjectByType<DemoFlow>();Assert.That(flow,Is.Not.Null,"Missing flow in "+UnityEngine.SceneManagement.SceneManager.GetActiveScene().path+"; objects="+Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).Length);Assert.That(flow.Director,Is.Not.Null,"Director binding");Directory.CreateDirectory("Logs/UX01/Captures");var slot="Logs/UX01/route-"+cls+"-"+Guid.NewGuid().ToString("N")+".json";flow.SlotPath=slot;flow.Director.SavePathOverride=slot;
            yield return UxExperienceRoute.Run(flow,slot,cls,race,name=>Capture(flow,cls,name));
            Assert.That(PrototypeSaveStore.Load(slot,flow.Session.Rules,out var before),Is.True);Assert.That(flow.Continue(),Is.True);flow.Pause();
            var after=flow.Session.State;Assert.That(after.RaceId,Is.EqualTo(race));Assert.That(after.ClassId,Is.EqualTo(cls));Assert.That(after.Health,Is.EqualTo(before.Health));Assert.That(after.Gold,Is.EqualTo(before.Gold));Assert.That(after.Army.Single().Id,Is.EqualTo(before.Army.Single().Id));Assert.That(after.Demo.Chapter,Is.EqualTo(DemoChapter.FreeExploration));Assert.That(LostCargoQuest.Stage(after),Is.EqualTo(CargoQuestStage.Completed));
            flow.Resume();LogAssert.NoUnexpectedReceived();
        }
        private static IEnumerator Capture(DemoFlow flow,string cls,string name)
        {
            foreach(var width in cls=="warrior"&&(name=="04-SafeVillage"||name=="11-FirstRpg"||name=="18-Positioned"||name=="21-Building")?new[]{1280,1920,2560}:new[]{1280})
                yield return DemoCapture.Save("Logs/UX01/Captures/"+cls+"-"+name+"-"+width+".png",width,width*9/16,null,(camera,w,h)=>
                {
                    var view=flow.ExistingHud.CompactView;
                    Assert.That(view.Resources.gameObject.activeInHierarchy,Is.EqualTo(flow.Session.Mode==PrototypeMode.Construction||(flow.Session.Mode==PrototypeMode.Exploration&&flow.Session.State.Demo.Chapter>=DemoChapter.Settlement)));
                    if(flow.Session.Mode==PrototypeMode.Exploration&&!view.WindowOpen)Assert.That(view.Hint.gameObject.activeInHierarchy,Is.False);
                });
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
