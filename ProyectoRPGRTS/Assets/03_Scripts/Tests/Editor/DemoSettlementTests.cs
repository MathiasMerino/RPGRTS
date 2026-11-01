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
    public sealed class DemoSettlementTests
    {
        private static PrototypeState Fixture()=>new PrototypeState{Gold=200,Wood=100,Food=20,Demo=new DemoProgress{Chapter=DemoChapter.Settlement},Facts=new System.Collections.Generic.List<string>{"demo-camp-secured"},HeroPosition=new Vector3(9,0,-10)};
        [Test]public void CostsRequirementsAndInvalidPointsAreTransactional()
        {
            var rules=ScriptableObject.CreateInstance<SettlementRules>();var state=Fixture();
            try
            {
                Assert.That(SettlementOperations.Build(state,rules,"workshop",Vector3.zero),Is.False);Assert.That(state.Gold,Is.EqualTo(200));
                Assert.That(SettlementOperations.Build(state,rules,"townhall",new Vector3(float.NaN,0,0)),Is.False);
                Assert.That(SettlementOperations.Build(state,rules,"townhall",new Vector3(12,0,-8)),Is.True);Assert.That(state.Gold,Is.EqualTo(185));Assert.That(state.Wood,Is.EqualTo(92));
                Assert.That(SettlementOperations.Build(state,rules,"townhall",Vector3.zero),Is.False);state.Gold=0;Assert.That(SettlementOperations.Build(state,rules,"farm",Vector3.zero),Is.False);Assert.That(state.Wood,Is.EqualTo(92));
            }finally{Object.DestroyImmediate(rules);}
        }
        [Test]public void ProductionNeedsActivationAndCyclesAreConsumedOnce()
        {
            var rules=ScriptableObject.CreateInstance<SettlementRules>();var state=Fixture();
            try
            {
                SettlementOperations.Build(state,rules,"townhall",new Vector3(12,0,-8));SettlementOperations.Build(state,rules,"workshop",new Vector3(8,0,-13));var workshop=state.Buildings[1];var wood=state.Wood;
                WorldOperations.Advance(state,20,rules);Assert.That(state.Wood,Is.EqualTo(wood));Assert.That(SettlementOperations.SetProduction(state,rules,workshop,true),Is.True);
                WorldOperations.Advance(state,10,rules);Assert.That(state.Wood,Is.EqualTo(wood+1));Assert.That(state.Food,Is.EqualTo(19));Assert.That(state.Demo.Produced,Is.EqualTo(1));
                WorldOperations.Advance(state,0,rules);Assert.That(state.Wood,Is.EqualTo(wood+1));SettlementOperations.SetProduction(state,rules,workshop,false);WorldOperations.Advance(state,10,rules);Assert.That(state.Wood,Is.EqualTo(wood+1));
            }finally{Object.DestroyImmediate(rules);}
        }
        [Test]public void StorageUpgradeAndProductionAreIndependentOfFramePartition()
        {
            var rules=ScriptableObject.CreateInstance<SettlementRules>();var state=Fixture();
            try
            {
                SettlementOperations.Build(state,rules,"townhall",Vector3.zero);SettlementOperations.Build(state,rules,"warehouse",new Vector3(4,0,0));Assert.That(SettlementOperations.Capacity(state,rules),Is.EqualTo(300));
                Assert.That(SettlementOperations.Upgrade(state,rules,state.Buildings[1]),Is.True);Assert.That(SettlementOperations.Capacity(state,rules),Is.EqualTo(400));Assert.That(SettlementOperations.Upgrade(state,rules,state.Buildings[1]),Is.False);
                SettlementOperations.Build(state,rules,"workshop",new Vector3(8,0,0));SettlementOperations.Build(state,rules,"farm",new Vector3(12,0,0));
                foreach(var b in state.Buildings)SettlementOperations.SetProduction(state,rules,b,true);
                var clone=JsonUtility.FromJson<PrototypeState>(JsonUtility.ToJson(state));WorldOperations.Advance(state,30,rules);for(var i=0;i<3;i++)WorldOperations.Advance(clone,10,rules);
                Assert.That(clone.Wood,Is.EqualTo(state.Wood));Assert.That(clone.Food,Is.EqualTo(state.Food));Assert.That(clone.Demo.Produced,Is.EqualTo(state.Demo.Produced));
                state.Food=state.Wood=400;WorldOperations.Advance(state,100,rules);Assert.That(state.Wood,Is.LessThanOrEqualTo(400));Assert.That(state.Food,Is.LessThanOrEqualTo(400));
            }finally{Object.DestroyImmediate(rules);}
        }
        [Test]public void RecruitmentRequiresPaymentBarracksAndFiniteSlots()
        {
            var rules=ScriptableObject.CreateInstance<SettlementRules>();var state=Fixture();
            try
            {
                Assert.That(SettlementOperations.Recruit(state,rules,"archer"),Is.False);SettlementOperations.Build(state,rules,"townhall",Vector3.zero);SettlementOperations.Build(state,rules,"barracks",new Vector3(4,0,0));
                var gold=state.Gold;var food=state.Food;for(var i=0;i<6;i++)Assert.That(SettlementOperations.Recruit(state,rules,"archer"),Is.True);
                Assert.That(state.Gold,Is.EqualTo(gold-30));Assert.That(state.Food,Is.EqualTo(food-12));Assert.That(state.Army.Select(x=>x.Id).Distinct().Count(),Is.EqualTo(6));Assert.That(SettlementOperations.Recruit(state,rules,"archer"),Is.False);Assert.That(SettlementOperations.Recruit(state,rules,"unknown"),Is.False);
            }finally{Object.DestroyImmediate(rules);}
        }
        [Test]public void SaveLoadKeepsProductionRosterAndLegacyWorkshopWithoutOfflineGain()
        {
            var rules=ScriptableObject.CreateInstance<SettlementRules>();var proto=ScriptableObject.CreateInstance<PrototypeRules>();var state=Fixture();Directory.CreateDirectory("Logs/E13");var path="Logs/E13/schema-"+System.Guid.NewGuid().ToString("N")+".json";
            try
            {
                SettlementOperations.Build(state,rules,"townhall",new Vector3(12,0,-8));SettlementOperations.Build(state,rules,"workshop",new Vector3(8,0,-13));SettlementOperations.Build(state,rules,"barracks",new Vector3(15,0,-13));SettlementOperations.Recruit(state,rules,"brawler");SettlementOperations.SetProduction(state,rules,state.Buildings[1],true);WorldOperations.Advance(state,20,rules);
                Assert.That(PrototypeSaveStore.Save(path,state,proto),Is.True);Assert.That(PrototypeSaveStore.Load(path,proto,out var saved),Is.True);Assert.That(saved.Wood,Is.EqualTo(state.Wood));Assert.That(saved.WorldSeconds,Is.EqualTo(state.WorldSeconds));Assert.That(saved.Army[0].Id,Is.EqualTo(state.Army[0].Id));Assert.That(saved.Buildings[1].ProductionPaused,Is.False);
                var old=new PrototypeState();old.Buildings.Add(new BuildingRecord("workshop",new Vector3(12,0,-10)));var json=JsonUtility.ToJson(old).Replace("\"Level\":1,","").Replace("\"ProductionPaused\":false,","");File.WriteAllText(path,json);Assert.That(PrototypeSaveStore.Load(path,proto,out saved),Is.True);Assert.That(saved.Buildings[0].Level,Is.EqualTo(1));Assert.That(saved.Buildings[0].ProductionPaused,Is.False);
            }finally{Object.DestroyImmediate(rules);Object.DestroyImmediate(proto);}
        }
        [UnityTest]public IEnumerator ActualBuildingsProductionRecruitDeploymentAndContinue()=>Economy();
        private static IEnumerator Economy()
        {
            EditorSceneManager.OpenScene(FirstDemoBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            Directory.CreateDirectory("Logs/E13/Captures");var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;flow.SlotPath="Logs/E13/camp-"+System.Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;
            // Fixture explícita para aislar economía. No se declara recorrido narrativo end-to-end.
            Assert.That(PrototypeSaveStore.Save(flow.SlotPath,Fixture(),flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);var panel=flow.GetComponent<DemoSettlementPanel>();
            var ids=new[]{"townhall","workshop","barracks","farm","warehouse"};var points=new[]{new Vector3(12,0,-8),new Vector3(8,0,-13),new Vector3(15,0,-13),new Vector3(11.5f,0,-14),new Vector3(15.5f,0,-9)};
            for(var i=0;i<ids.Length;i++){Assert.That(panel.SelectBlueprint(ids[i]),Is.True);Assert.That(d.Construction.Place(points[i]),Is.True,"Colocación real "+ids[i]);}
            var gold=flow.Session.State.Gold;Assert.That(d.Construction.Place(points[0]),Is.False);Assert.That(flow.Session.State.Gold,Is.EqualTo(gold));yield return null;yield return null;
            Assert.That(panel.SelectBuilding(1),Is.True);Assert.That(panel.ToggleProduction(),Is.True);yield return DemoCapture.Save("Logs/E13/Captures/Camp-Construction-Fixture.png",1280,720);
            var limit=Time.realtimeSinceStartup+13;while(flow.Session.State.Demo.Produced==0&&Time.realtimeSinceStartup<limit)yield return null;
            Assert.That(flow.Session.State.Demo.Produced,Is.GreaterThan(0));limit=Time.realtimeSinceStartup+1;while(flow.Session.State.Demo.Chapter==DemoChapter.Settlement&&Time.realtimeSinceStartup<limit)yield return null;
            Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.FreeExploration));
            Assert.That(panel.Recruit("archer"),Is.True);Assert.That(flow.Session.State.Army.Count,Is.EqualTo(1));Assert.That(panel.SelectBuilding(4),Is.True);Assert.That(panel.Improve(),Is.True);Assert.That(SettlementOperations.Capacity(flow.Session.State,flow.Session.Settlement),Is.EqualTo(400));
            Assert.That(d.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);var wood=flow.Session.State.Wood;flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.State.Wood,Is.EqualTo(wood));Assert.That(flow.Session.State.Buildings.Count,Is.EqualTo(5));Assert.That(flow.Session.State.Army.Count,Is.EqualTo(1));
            Assert.That(flow.BeginPatrol(),Is.True);var statuses=new System.Collections.Generic.List<RtsUnitStatus>();d.Rts.CopyStatus(statuses);Assert.That(statuses.Count,Is.EqualTo(14));Assert.That(statuses.Count(s=>s.Team==0),Is.EqualTo(8));
            var beforeTraining=flow.Session.State.Gold;d.Rts.SelectAll();Assert.That(d.Rts.Command(new Vector3(-7,0,13),null,false),Is.True);limit=Time.realtimeSinceStartup+45;while(d.Rts.Active&&Time.realtimeSinceStartup<limit)yield return null;
            Assert.That(d.Rts.Active,Is.False);Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.FreeExploration));Assert.That(flow.Session.State.Gold,Is.EqualTo(beforeTraining),"Entrenamiento no fabrica recompensas");yield return null;yield return null;
            Assert.That(flow.GetComponent<DemoRtsOverlay>().LiveBars,Is.EqualTo(0),"Liberar widgets/referencias al terminar");
            Assert.That(d.Save(flow.SlotPath),Is.True);yield return DemoCapture.Save("Logs/E13/Captures/Camp-Free-Fixture.png",1280,720);
            File.WriteAllText("Logs/E13/economy.txt","PASS explicit economy fixture: five actual 3x3 buildings, collision rejects cost, production activation/online cycle/free exploration, paid archer/warehouse upgrade, v1 restore without duplication, actual reinforcement deployment/battle/no duplicate reward, RTS UI cleanup. Narrative E2E still verified separately.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
