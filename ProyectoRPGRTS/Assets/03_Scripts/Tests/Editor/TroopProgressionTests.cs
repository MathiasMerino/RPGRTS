using System;
using System.Linq;
using System.Collections;
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
    public sealed class TroopProgressionTests
    {
        private PrototypeRules rules;private SettlementRules settlement;private ExpeditionRules expedition;private RtsDemoRules roles;
        [SetUp]public void Setup(){rules=Object.Instantiate(AssetDatabase.LoadAssetAtPath<PrototypeRules>(ProgressionSceneBuilder.RulesPath));settlement=AssetDatabase.LoadAssetAtPath<SettlementRules>(ProgressionSceneBuilder.Root+"/SettlementRules.asset");expedition=AssetDatabase.LoadAssetAtPath<ExpeditionRules>(ProgressionSceneBuilder.Root+"/ExpeditionRules.asset");roles=rules.Progression.Units;}
        [TearDown]public void Cleanup(){Object.DestroyImmediate(rules);}
        private PrototypeState State(){var s=new PrototypeState{Gold=100,Food=30,Wood=30,Demo=new DemoProgress{Chapter=DemoChapter.Settlement}};ProgressionOperations.Activate(s,rules.Progression);s.Facts.Add("demo-camp-secured");s.Buildings.Add(new BuildingRecord("barracks",Vector3.zero));s.Buildings.Add(new BuildingRecord("workshop",new Vector3(4,0,0)){Level=2});return s;}
        [Test]public void RolesHaveDistinctCostsHealthDamageRangeAndSpeed()
        {
            var s=State();foreach(var role in roles.Roles){var gold=s.Gold;Assert.That(SettlementOperations.Recruit(s,settlement,role.Id),Is.True);Assert.That(gold-s.Gold,Is.EqualTo(role.RecruitGold));Assert.That(s.Army.Last().Health,Is.EqualTo(role.Health));}
            Assert.That(roles.Find("swordsman").Health,Is.GreaterThan(roles.Find("archer").Health));Assert.That(roles.Find("brawler").Damage,Is.GreaterThan(roles.Find("swordsman").Damage));Assert.That(roles.Find("archer").Range,Is.GreaterThan(roles.Find("brawler").Range));Assert.That(roles.Find("brawler").Speed,Is.GreaterThan(roles.Find("archer").Speed));
            for(var i=0;i<3;i++)Assert.That(SettlementOperations.Recruit(s,settlement,"brawler"),Is.True);Assert.That(s.Army.Count,Is.EqualTo(6));Assert.That(SettlementOperations.Recruit(s,settlement,"archer"),Is.False);foreach(var role in roles.Roles)Assert.That(ExpeditionOperations.Recruit(s,expedition,roles,role.Id),Is.True);Assert.That(s.Companions.Count,Is.EqualTo(3));Assert.That(ExpeditionOperations.Recruit(s,expedition,roles,"archer"),Is.False);
        }
        [Test]public void CompanionExperienceRequiresParticipationAndNeverReplaysOrHeals()
        {
            var s=State();foreach(var role in roles.Roles)ExpeditionOperations.Recruit(s,expedition,roles,role.Id);var c=s.Companions[0];c.Health=4;var members=new[]{new TacticalCombatant{Id=c.Id}};
            Assert.That(TroopProgression.AwardCompanions(s,"umbral-battle-1",members),Is.Zero);foreach(var fact in new[]{"umbral-battle-1","umbral-battle-2"}){s.Facts.Add(fact);Assert.That(TroopProgression.AwardCompanions(s,fact,members),Is.EqualTo(1));Assert.That(TroopProgression.AwardCompanions(s,fact,members),Is.Zero);}Assert.That(c.Level,Is.EqualTo(2));Assert.That(c.Experience,Is.EqualTo(8));Assert.That(c.Health,Is.EqualTo(4));Assert.That(s.Companions[1].Experience,Is.Zero);Assert.That(TroopProgression.Profile(c,roles.Find(c.Role)).Damage,Is.EqualTo(3));Assert.That(PrototypeSaveStore.Valid(s,rules),Is.True);c.Experience=4;Assert.That(PrototypeSaveStore.Valid(s,rules),Is.False);
        }
        [Test]public void ArmyTrainingPreservesIdentityCostsAndSaveWithoutFreeHealing()
        {
            var s=State();SettlementOperations.Recruit(s,settlement,"swordsman");var r=s.Army[0];var id=r.Id;var gold=s.Gold;var wood=s.Wood;r.Health=7;Assert.That(TroopProgression.Train(s,r),Is.True);Assert.That(r.Id,Is.EqualTo(id));Assert.That(r.Health,Is.EqualTo(7));Assert.That(s.Gold,Is.EqualTo(gold-8));Assert.That(s.Wood,Is.EqualTo(wood-3));Assert.That(TroopProgression.Profile(r,roles.Find(r.Role)).Health,Is.EqualTo(14));Assert.That(TroopProgression.Train(s,r),Is.False);
            var path="Logs/E18/troops-"+Guid.NewGuid().ToString("N")+".json";Assert.That(PrototypeSaveStore.Save(path,s,rules),Is.True,PrototypeSaveStore.LastWriteFailure);Assert.That(PrototypeSaveStore.Load(path,rules,out var loaded),Is.True);Assert.That(loaded.Army[0].Level,Is.EqualTo(2));Assert.That(loaded.Army[0].Id,Is.EqualTo(id));Assert.That(loaded.Army[0].Health,Is.EqualTo(7));
        }
        [Test]public void TrainingAndRecruitmentRejectMissingRequirementsAtomically()
        {var s=State();s.Gold=0;var before=JsonUtility.ToJson(s);Assert.That(SettlementOperations.Recruit(s,settlement,"archer"),Is.False);Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before));s.Gold=100;SettlementOperations.Recruit(s,settlement,"brawler");s.Buildings.Find(b=>b.Id=="workshop").Level=1;before=JsonUtility.ToJson(s);Assert.That(TroopProgression.Train(s,s.Army[0]),Is.False);Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before));}
        [UnityTest]public IEnumerator RealArmyUiTrainingAndOldSaveMigration()=>ArmyUi();
        private static IEnumerator ArmyUi()
        {
            EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);
            yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();Assert.That(flow,Is.Not.Null);
            flow.SlotPath="Logs/E18/army-ui-"+Guid.NewGuid().ToString("N")+".json";
            flow.Director.SavePathOverride=flow.SlotPath;Directory.CreateDirectory("Logs/E18/Captures");
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E18/EntradaE17/warrior.json",flow.Session.Rules,out var old),Is.True);
            Assert.That(PrototypeSaveStore.Save(flow.SlotPath,old,flow.Session.Rules),Is.True);
            Assert.That(flow.Continue(),Is.True);yield return null;yield return null;
            var s=flow.Session.State;
            var camp=Object.FindAnyObjectByType<DemoSettlementPanel>();Assert.That(camp,Is.Not.Null);
            
            camp.Open();Assert.That(camp.SelectBuilding(s.Buildings.FindIndex(b=>b.Id=="workshop")),Is.True);
            Assert.That(camp.Improve(),Is.True);
            flow.Director.Execute("explore");
            Assert.That(camp.SellWood(),Is.True);Assert.That(camp.SellWood(),Is.True);
            var cycle=s.ProductionCycle;var deadline=Time.realtimeSinceStartup+15;
            while(s.ProductionCycle==cycle&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(s.Wood,Is.GreaterThanOrEqualTo(3),"real territorial production funds the remaining training wood");
            var node=GameObject.Find("E14 E15 Expedition integration");Assert.That(node,Is.Not.Null,"integration scene object");
            var panel=node.GetComponent<ExpeditionPanel>();
            Assert.That(panel,Is.Not.Null,"expedition component exists");
            
            panel.Open();yield return null;
            Assert.That(panel.Controls.ContainsKey("army-tab"),Is.True,"panel Start initialized");
            panel.Controls["army-tab"].onClick.Invoke();yield return null;yield return null;
            Assert.That(s.Army.Count,Is.GreaterThan(0));var health=s.Army[0].Health;
            Assert.That(panel.Controls["train-0"].interactable,Is.True,"training resources G"+s.Gold+" W"+s.Wood+" atCamp="+Object.FindAnyObjectByType<PartyWorld>().AtCamp);
            panel.Controls["train-0"].onClick.Invoke();
            Assert.That(s.Army[0].Level,Is.EqualTo(2));Assert.That(s.Army[0].Health,Is.EqualTo(health));
            foreach(var width in new[]{1280,1920}){var currentPanel=panel;yield return DemoCapture.Save("Logs/E18/Captures/Army-"+width+".png",width,width*9/16,null,(c,w,h)=>{foreach(var t in currentPanel.Controls["close"].transform.parent.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();Assert.That(t.isTextOverflowing,Is.False,t.name+": "+t.text);}});}
            panel.Close();Assert.That(flow.Director.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.State.Army[0].Level,Is.EqualTo(2));LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
