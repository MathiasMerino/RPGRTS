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
    public sealed class ProgressionTests
    {
        private PrototypeRules rules;private ProgressionRules progression;
        [SetUp]public void Setup(){rules=ScriptableObject.CreateInstance<PrototypeRules>();progression=ScriptableObject.CreateInstance<ProgressionRules>();rules.Progression=progression;}
        [TearDown]public void Dispose(){Object.DestroyImmediate(rules);Object.DestroyImmediate(progression);}
        private PrototypeState New(string id="warrior"){var s=new PrototypeState{ClassId=id,Health=rules.Find(id).Health};ProgressionOperations.Activate(s,progression);return s;}
        private void Earn(PrototypeState s,string fact){s.Facts.Add(fact);Assert.That(ProgressionOperations.GrantForFact(s,progression,fact),Is.GreaterThanOrEqualTo(0));}
        [TestCase("warrior"),TestCase("monk"),TestCase("mage")]
        public void RealFactsGrantOnceAndCurveIsBounded(string id)
        {
            var s=New(id);Assert.That(ProgressionOperations.GrantForFact(s,progression,"rpg-victory"),Is.Zero,"No event/fact, no XP");
            var expected=new[]{12,20,28,38,48,60,60};for(var i=0;i<progression.Rewards.Length;i++){var fact=progression.Rewards[i].Fact;Earn(s,fact);Assert.That(s.Experience,Is.EqualTo(expected[i]));Assert.That(ProgressionOperations.GrantForFact(s,progression,fact),Is.Zero);Assert.That(ProgressionOperations.Valid(s,rules),Is.True);}
            Assert.That(s.HeroLevel,Is.EqualTo(4));Assert.That(ProgressionOperations.Activate(s,progression),Is.False);Assert.That(s.Experience,Is.EqualTo(60));
        }
        [Test]public void DerivedAttributesEquipmentAndSkillPointsNeverAccumulate()
        {
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                var s=New(id);var original=rules.Find(id);var hp=original.Health;Assert.That(ProgressionOperations.ResolveProfile(s,rules).Health,Is.EqualTo(hp));Earn(s,"rpg-victory");
                Assert.That(ProgressionOperations.AvailablePoints(s),Is.EqualTo(1));Assert.That(ProgressionOperations.Learn(s,progression,id+"-discipline"),Is.True);Assert.That(ProgressionOperations.Learn(s,progression,id+"-discipline"),Is.False);Assert.That(ProgressionOperations.Learn(s,progression,"other-impact"),Is.False);
                var profile=ProgressionOperations.ResolveProfile(s,rules);Assert.That(profile.Health,Is.EqualTo(hp+progression.Find(id).HealthPerLevel+2));Assert.That(profile.Combat.Attributes[progression.Find(id).Primary],Is.EqualTo(14));Assert.That(profile.Combat.Accuracy,Is.EqualTo(2));Assert.That(s.Health,Is.EqualTo(hp),"No free heal");
                Earn(s,"demo-camp-secured");Earn(s,"lost-cargo-completed");Earn(s,"umbral-battle-1");profile=ProgressionOperations.ResolveProfile(s,rules);Assert.That(profile.Combat.Attributes[progression.Find(id).Primary],Is.EqualTo(16));Assert.That(profile.Combat.Accuracy,Is.EqualTo(3));Assert.That(profile.Damage,Is.EqualTo(original.Damage+1));
                Assert.That(ProgressionOperations.ResolveProfile(s,rules).Health,Is.EqualTo(profile.Health));Assert.That(original.Health,Is.EqualTo(hp));Assert.That(ProgressionOperations.Valid(s,rules),Is.True);
            }
        }
        [Test]public void LegacyCreditsPersistAndMalformedProgressionIsRejected()
        {
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",rules,out var old),Is.True);Assert.That(old.ProgressionRevision,Is.Zero);var hp=old.Health;ProgressionOperations.Activate(old,progression);var xp=progression.Rewards.Where(r=>old.Facts.Contains(r.Fact)).Sum(r=>r.Amount);Assert.That(old.Experience,Is.EqualTo(Math.Min(60,xp)));Assert.That(old.Health,Is.EqualTo(hp));
            var path="Logs/E18/save-"+Guid.NewGuid().ToString("N")+".json";Assert.That(PrototypeSaveStore.Save(path,old,rules),Is.True,PrototypeSaveStore.LastWriteFailure);
            for(var i=0;i<3;i++){Assert.That(PrototypeSaveStore.Load(path,rules,out old),Is.True);Assert.That(ProgressionOperations.Activate(old,progression),Is.False);Assert.That(old.Experience,Is.EqualTo(Math.Min(60,xp)));Assert.That(PrototypeSaveStore.Save(path,old,rules),Is.True);}
            old.Experience++;Assert.That(PrototypeSaveStore.Valid(old,rules),Is.False);old.Experience--;old.Skills.Add(null);Assert.That(PrototypeSaveStore.Valid(old,rules),Is.False);old.Skills.Clear();old.Skills.Add(new SkillRank("mage-impact",1));Assert.That(PrototypeSaveStore.Valid(old,rules),Is.False);
        }
        [Test]public void SceneRetainsPhysicsAndNoPrivateDependencies()
        {
            EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);var signature=VisualPolishTests.PhysicsSignature();EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);Assert.That(VisualPolishTests.PhysicsSignature(),Is.EqualTo(signature));Assert.That(Object.FindAnyObjectByType<GameSession>().Rules.Progression,Is.Not.Null);Assert.That(AssetDatabase.GetDependencies(ProgressionSceneBuilder.ScenePath,true).Any(p=>p.Contains("MonkMeshy")||p.Contains("LocalOnly")),Is.False);
        }
        [UnityTest]public IEnumerator FirstRealRpgLevelsThreeClassesAndSurvivesContinue()=>Play();
        private static IEnumerator Play()
        {
            EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;flow.SlotPath="Logs/E18/play-"+Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;flow.EncounterSeed=42;Directory.CreateDirectory("Logs/E18/Captures");
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                flow.ShowMain();Assert.That(flow.StartNew("Aprendiz",id,"human"),Is.True);Assert.That(flow.Session.State.Experience,Is.Zero);flow.CompleteIntro();yield return null;d.Encounter.Approach();yield return ExpeditionIntegrationTests.Arrive(d.Hero);
                for(var turn=0;turn<30&&flow.Session.State.Demo.Chapter==DemoChapter.FirstCombat;turn++)
                {var b=d.Encounter.Battle;if(b.HeroHealth<8&&!b.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();if(flow.Session.State.Demo.Chapter!=DemoChapter.FirstCombat)break;Assert.That(d.Execute("end"),Is.True);var limit=Time.realtimeSinceStartup+5;while(d.Encounter.Battle!=null&&d.Encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<limit)yield return null;}
                Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.RoadToRts));Assert.That(flow.Session.State.HeroLevel,Is.EqualTo(2));Assert.That(flow.Session.State.Experience,Is.EqualTo(12));var health=flow.Session.State.Health;Assert.That(flow.Session.HeroProfile.Health,Is.GreaterThan(flow.Session.Rules.Find(id).Health));Assert.That(flow.Session.LearnSkill(id+"-discipline"),Is.True);Assert.That(flow.Session.State.Health,Is.EqualTo(health));
                var stats=flow.Session.HeroProfile.Health;Assert.That(d.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.State.HeroLevel,Is.EqualTo(2));Assert.That(flow.Session.HeroProfile.Health,Is.EqualTo(stats));Assert.That(flow.Session.State.Health,Is.EqualTo(health));Assert.That(flow.Session.State.Experience,Is.EqualTo(12));
                yield return DemoCapture.Save("Logs/E18/Captures/BC-"+id+".png",1280,720);
            }
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
