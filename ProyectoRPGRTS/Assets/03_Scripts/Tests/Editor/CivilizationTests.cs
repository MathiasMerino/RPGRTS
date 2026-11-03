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
    public sealed class CivilizationTests
    {
        private PrototypeRules rules;private SettlementRules settlement;private ExpeditionRules expedition;private CivilizationCatalog catalog;
        [SetUp]public void Setup(){rules=AssetDatabase.LoadAssetAtPath<PrototypeRules>(CivilizationSceneBuilder.RulesPath);catalog=rules.Civilizations;settlement=AssetDatabase.LoadAssetAtPath<SettlementRules>(CivilizationSceneBuilder.Root+"/SettlementRules.asset");expedition=AssetDatabase.LoadAssetAtPath<ExpeditionRules>(CivilizationSceneBuilder.Root+"/ExpeditionRules.asset");}
        private PrototypeState Fixture(string race="elf",string cls="mage")
        {var s=new PrototypeState{RaceId=race,ClassId=cls,IdentityRevision=1,CultureId="stonehall",Gold=150,Wood=80,Food=80,Demo=new DemoProgress{Chapter=DemoChapter.Settlement}};ProgressionOperations.Activate(s,rules.Progression);s.Health=ProgressionOperations.ResolveProfile(s,rules).Health;s.Facts.Add("demo-camp-secured");ExpeditionOperations.InitializeTerritories(s,expedition);return s;}
        [TestCase("human","warrior"),TestCase("human","monk"),TestCase("human","mage"),TestCase("elf","warrior"),TestCase("elf","monk"),TestCase("elf","mage"),TestCase("dwarf","warrior"),TestCase("dwarf","monk"),TestCase("dwarf","mage"),TestCase("orc","warrior"),TestCase("orc","monk"),TestCase("orc","mage")]
        public void TwelveCombinationsHaveBoundedSharedTraitsAndStableSave(string race,string cls)
        {
            var s=Fixture(race,cls);Assert.That(catalog.Allows(race,cls),Is.True);var p=ProgressionOperations.ResolveProfile(s,rules);var baseProfile=rules.Find(cls);var r=catalog.Race(race);Assert.That(p.Health,Is.EqualTo(baseProfile.Health+r.MaximumHealth));Assert.That(p.Speed,Is.EqualTo(baseProfile.Speed+r.Speed).Within(.001));Assert.That(p.Combat.GuardReduction,Is.EqualTo(1+r.Guard));
            s.Facts.Add("rpg-victory");ProgressionOperations.GrantForFact(s,rules.Progression,"rpg-victory");p=ProgressionOperations.ResolveProfile(s,rules);Assert.That(p.Combat.TechniqueUnlocked,Is.True);Assert.That(p.Combat.TechniqueDamage,Is.EqualTo(rules.Progression.Find(cls).TechniqueDamage+r.TechniqueDamage));
            var hp=s.Health;var max=p.Health;var path="Logs/E19/"+race+"-"+cls+".json";for(var i=0;i<3;i++){Assert.That(PrototypeSaveStore.Save(path,s,rules),Is.True,PrototypeSaveStore.LastWriteFailure);Assert.That(PrototypeSaveStore.Load(path,rules,out s),Is.True);Assert.That(s.RaceId,Is.EqualTo(race));Assert.That(s.CultureId,Is.EqualTo("stonehall"));Assert.That(s.Health,Is.EqualTo(hp));Assert.That(ProgressionOperations.ResolveProfile(s,rules).Health,Is.EqualTo(max));}Assert.That(baseProfile.Health,Is.EqualTo(rules.Find(cls).Health));
        }
        private void Infrastructure(PrototypeState s)
        {Assert.That(SettlementOperations.Build(s,settlement,"townhall",Vector3.zero),Is.True);Assert.That(SettlementOperations.Build(s,settlement,"barracks",Vector3.right*4),Is.True);Assert.That(SettlementOperations.Build(s,settlement,"workshop",Vector3.left*4),Is.True);Assert.That(CivilizationOperations.ChooseCulture(s,catalog,"frontier"),Is.True);Assert.That(SettlementOperations.Build(s,settlement,"warehouse",Vector3.forward*4),Is.True);}
        [Test]public void CulturalBuildingsProducePayAndRetainCultureWithoutDuplication()
        {
            var s=Fixture();Infrastructure(s);Assert.That(s.Buildings.Single(b=>b.Id=="workshop").CultureId,Is.EqualTo("stonehall"));Assert.That(s.Buildings.Single(b=>b.Id=="warehouse").CultureId,Is.EqualTo("frontier"));Assert.That(s.Gold,Is.EqualTo(103));Assert.That(s.Wood,Is.EqualTo(54));Assert.That(SettlementOperations.Capacity(s,settlement),Is.EqualTo(320));var workshop=s.Buildings.Single(b=>b.Id=="workshop");SettlementOperations.SetProduction(s,settlement,workshop,true);var wood=s.Wood;var food=s.Food;SettlementOperations.Produce(s,settlement,1);Assert.That(s.Wood,Is.EqualTo(wood+2));Assert.That(s.Food,Is.EqualTo(food-2));SettlementOperations.Upgrade(s,settlement,workshop);wood=s.Wood;food=s.Food;SettlementOperations.Produce(s,settlement,1);Assert.That(s.Wood,Is.EqualTo(wood+4));Assert.That(s.Food,Is.EqualTo(food-4));Assert.That(PrototypeSaveStore.Valid(s,rules),Is.True);
        }
        [Test]public void TwoPilotsNeedRealAccessAndResourcesAndRespectSeparateArmyLimit()
        {
            var s=Fixture("orc","mage");Infrastructure(s);Assert.That(CivilizationOperations.Recruit(s,settlement,"stonehall-striker"),Is.False);var before=JsonUtility.ToJson(s);Assert.That(CivilizationOperations.Recruit(s,settlement,"sylvan-bow"),Is.False);Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before));Assert.That(ExpeditionOperations.ClaimNeutral(s,expedition),Is.True);Assert.That(ExpeditionOperations.Conquer(s,"outpost",false),Is.False);Assert.That(ExpeditionOperations.Conquer(s,"outpost",true),Is.True);
            foreach(var id in new[]{"frontier-guard","stonehall-striker"}){var p=catalog.Unit(id).Profile;var gold=s.Gold;var food=s.Food;Assert.That(SettlementOperations.Recruit(s,settlement,id),Is.True);var r=s.Army.Last();Assert.That(r.Health,Is.EqualTo(p.Health));Assert.That(s.Gold,Is.EqualTo(gold-p.RecruitGold));Assert.That(s.Food,Is.EqualTo(food-p.RecruitFood));Assert.That(r.CultureId,Is.EqualTo(catalog.Unit(id).CultureId));}
            foreach(var role in rules.Progression.Units.Roles)Assert.That(ExpeditionOperations.Recruit(s,expedition,rules.Progression.Units,role.Id),Is.True);Assert.That(s.Companions.Count,Is.EqualTo(3));Assert.That(s.Companions.Select(c=>c.RaceId).Distinct().Count(),Is.EqualTo(3));Assert.That(ExpeditionOperations.Recruit(s,expedition,rules.Progression.Units,"frontier-guard"),Is.False);while(s.Army.Count<6)Assert.That(SettlementOperations.Recruit(s,settlement,"frontier-guard"),Is.True);before=JsonUtility.ToJson(s);Assert.That(SettlementOperations.Recruit(s,settlement,"frontier-guard"),Is.False);Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before));Assert.That(PrototypeSaveStore.Valid(s,rules),Is.True);
        }
        [Test]public void PactChangesAccessWithoutChangingOwnerRaceOrGeneratingResources()
        {
            var s=Fixture("dwarf","monk");Infrastructure(s);Assert.That(CivilizationOperations.Relation(s,"faction-b"),Is.EqualTo(FactionRelation.Hostile));Assert.That(CivilizationOperations.Relation(s,""),Is.EqualTo(FactionRelation.Neutral));var gold=s.Gold;var food=s.Food;Assert.That(CivilizationOperations.Treaty(s,catalog),Is.True);Assert.That(s.Gold,Is.EqualTo(gold-catalog.TreatyGold));Assert.That(s.Food,Is.EqualTo(food-catalog.TreatyFood));Assert.That(CivilizationOperations.Relation(s,"faction-b"),Is.EqualTo(FactionRelation.Ally));Assert.That(s.Territories.Single(t=>t.Id=="outpost").Owner,Is.EqualTo("faction-b"));Assert.That(CivilizationOperations.Recruit(s,settlement,"stonehall-striker"),Is.True);Assert.That(CivilizationOperations.Treaty(s,catalog),Is.False);Assert.That(ExpeditionOperations.Conquer(s,"outpost",true,catalog),Is.False,"Allied ownership cannot be conquered");Assert.That(s.RaceId,Is.EqualTo("dwarf"));Assert.That(s.FactionId,Is.EqualTo("player-company"));
        }
        [Test]public void LegacyIdentityIsPreservedWithoutInventedCultureOrNewBonus()
        {
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",rules,out var s),Is.True);var hp=s.Health;var race=s.RaceId;Assert.That(s.IdentityRevision,Is.Zero);s.CultureId="legacy-unmapped-culture";ProgressionOperations.Activate(s,rules.Progression);var path="Logs/E19/legacy.json";Assert.That(PrototypeSaveStore.Save(path,s,rules),Is.True,PrototypeSaveStore.LastWriteFailure);Assert.That(PrototypeSaveStore.Load(path,rules,out s),Is.True);Assert.That(s.CultureId,Is.EqualTo("legacy-unmapped-culture"));Assert.That(s.RaceId,Is.EqualTo(race));Assert.That(s.Health,Is.EqualTo(hp));Assert.That(s.Companions.All(c=>string.IsNullOrEmpty(c.RaceId)),Is.True);s.IdentityRevision=1;Assert.That(PrototypeSaveStore.Valid(s,rules),Is.False,"Unknown explicit new identities rejected; original save remains intact");
        }
        [Test]public void CatalogHasOnlyTwoEnabledPilotsAndUsesPublicSharedArt()
        {Assert.That(catalog.Races.Select(r=>r.Id).Distinct().Count(),Is.EqualTo(4));Assert.That(catalog.Cultures.Length,Is.EqualTo(4));Assert.That(catalog.Units.Length,Is.EqualTo(12));Assert.That(catalog.Units.Count(u=>u.Enabled),Is.EqualTo(2));foreach(var u in catalog.Units){Assert.That(u.Profile.RecruitGold,Is.GreaterThan(0));Assert.That(u.Profile.Speed,Is.GreaterThan(0));Assert.That(catalog.Race(u.RaceId),Is.Not.Null);}Assert.That(AssetDatabase.GetDependencies(CivilizationSceneBuilder.ScenePath,true).Any(p=>p.Contains("MonkMeshy")||p.Contains("LocalOnly")),Is.False);Assert.That(rules.Progression.Units.Roles.Length,Is.EqualTo(3),"Legacy patrol role set preserved");}
        [UnityTest]public IEnumerator RealCreationAllTwelveCombinationsAndContinue()=>Creation();
        private IEnumerator Creation()
        {
            EditorSceneManager.OpenScene(CivilizationSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;var flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E19/creation-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;Directory.CreateDirectory("Logs/E19/Captures");
            foreach(var race in flow.Session.Rules.Civilizations.Races)foreach(var cls in flow.Session.Rules.Classes)
            {flow.ShowMain();flow.Front.Controls["new"].onClick.Invoke();flow.Front.Controls["race-"+race.Id].onClick.Invoke();flow.Front.Controls["class-"+cls.Id].onClick.Invoke();flow.Front.NameInput.text=race.Title+" "+cls.Title;yield return null;yield return null;Canvas.ForceUpdateCanvases();yield return DemoCapture.Save("Logs/E19/Captures/Creation-"+race.Id+"-"+cls.Id+".png",1280,720);flow.Front.Controls["begin"].onClick.Invoke();Assert.That(flow.Session.State.RaceId,Is.EqualTo(race.Id));Assert.That(flow.Session.State.ClassId,Is.EqualTo(cls.Id));Assert.That(flow.Session.State.IdentityRevision,Is.EqualTo(1));var max=flow.Session.HeroProfile.Health;Assert.That(flow.Session.State.Health,Is.EqualTo(max));Assert.That(flow.Director.Hero.Agent.speed,Is.EqualTo(flow.Session.HeroProfile.Speed).Within(.01));var model=flow.Director.Hero.GetComponent<CharacterVisualPresenter>().Model;var scale=model.transform.localScale;Assert.That(flow.Director.Save(flow.SlotPath),Is.True);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.HeroProfile.Health,Is.EqualTo(max));Assert.That(model.transform.localScale,Is.EqualTo(scale));}
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }

        [UnityTest]public IEnumerator TwoPaidPilotsUseActualRtsDamageAndExistingUi()=>PilotsPlay();
        private IEnumerator PilotsPlay()
        {
            EditorSceneManager.OpenScene(CivilizationSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;var s=Fixture("elf","monk");Infrastructure(s);RequirePact(s);Assert.That(CivilizationOperations.Recruit(s,settlement,"frontier-guard"),Is.True);Assert.That(CivilizationOperations.Recruit(s,settlement,"stonehall-striker"),Is.True);
            var points=new[]{new Vector3(12,0,-8),new Vector3(15,0,-13),new Vector3(8,0,-13),new Vector3(11.5f,0,-14)};for(var i=0;i<4;i++)s.Buildings[i].Position=points[i];s.HeroPosition=new Vector3(9,0,-10);s.Demo.IntroSeen=true;
            flow.SlotPath="Logs/E19/pilot-play-"+Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;Assert.That(PrototypeSaveStore.Save(flow.SlotPath,s,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;
            var panel=Object.FindAnyObjectByType<ExpeditionPanel>();panel.Open();panel.Controls["culture-tab"].onClick.Invoke();yield return null;
            foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080)})yield return DemoCapture.Save("Logs/E19/Captures/Culture-"+size.x+".png",size.x,size.y);panel.Close();
            s=flow.Session.State;var rts=d.Rts;Assert.That(rts.BeginExpedition(d.Hero,s.Army.Select(r=>r.Id).ToArray(),2),Is.True);var units=new System.Collections.Generic.List<RtsUnitStatus>();rts.CopyStatus(units);var hits=new System.Collections.Generic.List<RtsTraceSample>();Action<RtsTraceSample> listener=hits.Add;rts.Diagnostic+=listener;
            var pilots=units.Where(u=>u.Team==0&&!u.Hero).ToArray();Assert.That(pilots.Length,Is.EqualTo(2));var foe=units.First(u=>u.Team==1).Unit;
            for(var i=0;i<2;i++)Assert.That(rts.SelectUnit(pilots[i].Unit.GetComponentInChildren<Collider>(),i>0),Is.True);Assert.That(rts.Command(foe.transform.position,foe.GetComponentInChildren<Collider>(),false),Is.True);rts.SelectUnit(d.Hero.GetComponentInChildren<Collider>(),false);Assert.That(rts.HeroAction("guard"),Is.True);
            var limit=Time.realtimeSinceStartup+45;while(rts.Active&&Time.realtimeSinceStartup<limit)yield return null;rts.Diagnostic-=listener;Assert.That(rts.Active,Is.False);Assert.That(rts.LastResolvedVictory,Is.True);
            foreach(var id in new[]{"frontier-guard","stonehall-striker"}){var p=catalog.Unit(id).Profile;var index=units.FindIndex(u=>u.Role==p.Title);Assert.That(index,Is.GreaterThanOrEqualTo(0));Assert.That(units[index].Maximum,Is.EqualTo(p.Health));Assert.That(units[index].Unit.Agent.speed,Is.EqualTo(p.Speed).Within(.01));Assert.That(hits.Any(h=>h.Kind=="strike"&&h.Actor==index&&h.Value==p.Damage),Is.True,"Actual configured strike damage: "+id);}
            Assert.That(flow.Session.State.RaceId,Is.EqualTo("elf"));Assert.That(flow.Session.State.ClassId,Is.EqualTo("monk"));d.Execute("explore");Assert.That(d.Hero.Warp(new Vector3(9,0,-10)),Is.True);var camp=flow.GetComponent<DemoSettlementPanel>();camp.Open();Assert.That(flow.Session.Mode,Is.EqualTo(PrototypeMode.Construction));Assert.That(camp.Recruit("archer"),Is.True,"Existing military button must work from Construction at camp");d.Execute("explore");Assert.That(d.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private void RequirePact(PrototypeState s){Assert.That(CivilizationOperations.Treaty(s,catalog),Is.True);}

        [Test]public void SeededBalanceSimulationRecordsTwelveCombinationsAtThreeLevels()
        {
            Directory.CreateDirectory("Logs/E19");var csv=new System.Text.StringBuilder("race,class,level,seeds,wins,meanRemainingHP,meanRounds,kind\n");
            foreach(var race in catalog.Races)foreach(var cls in rules.Classes)foreach(var level in new[]{1,2,4})
            {var s=Fixture(race.Id,cls.Id);s.HeroLevel=level;var profile=ProgressionOperations.ResolveProfile(s,rules);var wins=0;var health=0;var rounds=0;
                for(var seed=1;seed<=100;seed++)
                {var battle=new TurnBattle(profile,profile.Health,1,1,seed);while(!battle.Finished&&battle.Round<40){if(battle.HeroHealth<profile.Health/2&&!battle.RecoveryUsed)battle.Recover();else if(battle.Round%4==0)battle.Guard();else if(!battle.UseTechnique(1))battle.Attack(1);battle.EndTurn(true);}Assert.That(battle.Finished,Is.True);wins+=battle.Won?1:0;health+=battle.HeroHealth;rounds+=battle.Round;}
                csv.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,"{0},{1},{2},100,{3},{4:F2},{5:F2},pure_rules_not_native_or_human\n",race.Id,cls.Id,level,wins,health/100f,rounds/100f);
            }
            File.WriteAllText("Logs/E19/BALANCE.csv",csv.ToString());
        }

        [Test]public void ConfigurablePoliticsAndYieldAffectActualTransactions()
        {
            var config=Object.Instantiate(catalog);var storage=Object.Instantiate(settlement);storage.Civilizations=config;
            try{var s=Fixture();config.InitialRelationB=FactionRelation.Neutral;Assert.That(CivilizationOperations.Relation(s,"faction-b",config),Is.EqualTo(FactionRelation.Neutral));Assert.That(ExpeditionOperations.Conquer(s,"outpost",true,config),Is.False);config.InitialRelationB=FactionRelation.Hostile;config.ConquestReputationCost=2;config.TerritoryWoodPerCycle=3;Assert.That(ExpeditionOperations.Conquer(s,"outpost",true,config),Is.True);Assert.That(s.ReputationB,Is.EqualTo(-2));var wood=s.Wood;ExpeditionOperations.ProduceTerritories(s,storage,1);Assert.That(s.Wood,Is.EqualTo(wood+3));}finally{Object.DestroyImmediate(config);Object.DestroyImmediate(storage);}
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
