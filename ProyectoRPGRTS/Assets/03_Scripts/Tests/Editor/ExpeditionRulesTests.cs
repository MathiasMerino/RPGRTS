using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Aethernia.Tests.Editor
{
    public sealed class ExpeditionRulesTests
    {
        private ExpeditionRules rules;private RtsDemoRules roles;private PrototypeRules heroRules;private SettlementRules settlement;
        [SetUp]public void Setup(){rules=ScriptableObject.CreateInstance<ExpeditionRules>();roles=ScriptableObject.CreateInstance<RtsDemoRules>();heroRules=ScriptableObject.CreateInstance<PrototypeRules>();settlement=ScriptableObject.CreateInstance<SettlementRules>();}
        [TearDown]public void End(){UnityEngine.Object.DestroyImmediate(rules);UnityEngine.Object.DestroyImmediate(roles);UnityEngine.Object.DestroyImmediate(heroRules);UnityEngine.Object.DestroyImmediate(settlement);}
        public static PrototypeState Ready(string classId="warrior")
        {return new PrototypeState{ClassId=classId,Health=classId=="mage"?16:classId=="monk"?20:24,Gold=40,Wood=20,Food=20,Demo=new DemoProgress{Chapter=DemoChapter.FreeExploration,IntroSeen=true},HeroPosition=new Vector3(9,0,-10),Facts=new List<string>{"demo-starter-kit","demo-camp-secured"}};}
        [Test]public void RecruitmentHasRealCostsUniqueIdsAndThreeSeparatePartySlots()
        {
            var s=Ready();foreach(var r in roles.Roles)Assert.That(ExpeditionOperations.Recruit(s,rules,roles,r.Id),Is.True);
            Assert.That(s.Gold,Is.EqualTo(25));Assert.That(s.Food,Is.EqualTo(14));Assert.That(s.Companions.Select(c=>c.Id).Distinct().Count(),Is.EqualTo(3));Assert.That(ExpeditionOperations.Recruit(s,rules,roles,"archer"),Is.False);
            var c=s.Companions[0];c.Health=3;Assert.That(ExpeditionOperations.SetParty(s,rules,c.Id,false),Is.True);Assert.That(s.Companions.Count,Is.EqualTo(3));Assert.That(c.Health,Is.EqualTo(3));Assert.That(ExpeditionOperations.SetParty(s,rules,c.Id,true),Is.True);
            c.Health=0;ExpeditionOperations.SetParty(s,rules,c.Id,false);Assert.That(ExpeditionOperations.SetParty(s,rules,c.Id,true),Is.False);Assert.That(PrototypeSaveStore.Valid(s,heroRules),Is.True);
        }
        [Test]public void InvalidOrUnaffordableRecruitmentDoesNotCharge()
        {var s=Ready();s.Gold=4;Assert.That(ExpeditionOperations.Recruit(s,rules,roles,"archer"),Is.False);Assert.That(s.Gold,Is.EqualTo(4));s.Gold=40;Assert.That(ExpeditionOperations.Recruit(s,rules,roles,"dragon"),Is.False);s.Demo=null;Assert.That(ExpeditionOperations.Recruit(s,rules,roles,"archer"),Is.False);Assert.That(s.Companions,Is.Empty);}
        [Test]public void DungeonRewardRequiresBothWinsAndCannotRepeatAcrossLoad()
        {
            var s=Ready();Assert.That(ExpeditionOperations.RewardDungeon(s,rules),Is.False);s.Facts.Add("umbral-battle-1");Assert.That(ExpeditionOperations.RewardDungeon(s,rules),Is.False);s.Facts.Add("umbral-battle-2");Assert.That(ExpeditionOperations.RewardDungeon(s,rules),Is.True);
            var path="Logs/E14/reward-"+Guid.NewGuid().ToString("N")+".json";Assert.That(PrototypeSaveStore.Save(path,s,heroRules),Is.True);Assert.That(PrototypeSaveStore.Load(path,heroRules,out s),Is.True);Assert.That(ExpeditionOperations.RewardDungeon(s,rules),Is.False);Assert.That(s.Gold,Is.EqualTo(70));Assert.That(s.Wood,Is.EqualTo(35));
        }
        [Test]public void OwnershipOnlyChangesOnWinAndOnlineBonusDoesNotDuplicate()
        {
            var s=Ready();s.Wood=0;ExpeditionOperations.InitializeTerritories(s,rules);ExpeditionOperations.InitializeTerritories(s,rules);Assert.That(s.Territories.Count,Is.EqualTo(3));Assert.That(s.Territories.Single(t=>t.Id=="camp").Secured,Is.True);Assert.That(ExpeditionOperations.Conquer(s,"outpost",false),Is.False);Assert.That(s.Territories.Single(t=>t.Id=="outpost").Owner,Is.EqualTo("faction-b"));
            Assert.That(ExpeditionOperations.ClaimNeutral(s,rules),Is.True);Assert.That(ExpeditionOperations.Conquer(s,"outpost",true),Is.True);Assert.That(ExpeditionOperations.Conquer(s,"outpost",true),Is.False);
            WorldOperations.Advance(s,10,settlement);Assert.That(s.Wood,Is.EqualTo(2));var path="Logs/E14/territory-"+Guid.NewGuid().ToString("N")+".json";Assert.That(PrototypeSaveStore.Save(path,s,heroRules),Is.True);Assert.That(PrototypeSaveStore.Load(path,heroRules,out s),Is.True);Assert.That(s.Wood,Is.EqualTo(2));WorldOperations.Advance(s,10,settlement);Assert.That(s.Wood,Is.EqualTo(4));
        }
        [Test]public void CultureFactionAndRaceAreIndependentAndArmyIsNotCappedAtThree()
        {
            var s=Ready();s.RaceId="elf";s.CultureId="culture-provisional";s.ReputationA=10;s.ReputationB=-5;for(var i=0;i<6;i++){var c=new RecruitRecord{Id=Guid.NewGuid().ToString("N"),Role="swordsman"};s.Army.Add(c);s.ArmySelection.Add(c.Id);}foreach(var r in roles.Roles)ExpeditionOperations.Recruit(s,rules,roles,r.Id);
            Assert.That(PrototypeSaveStore.Valid(s,heroRules),Is.True);Assert.That(s.Companions.Count,Is.EqualTo(3));Assert.That(s.Army.Count,Is.EqualTo(6));Assert.That(ExpeditionOperations.Relation(s,"faction-a"),Is.EqualTo(10));Assert.That(ExpeditionOperations.Relation(s,"faction-b"),Is.EqualTo(-5));Assert.That(s.FactionId,Is.EqualTo("player-company"));
            s.ArmySelection.Add(s.ArmySelection[0]);Assert.That(PrototypeSaveStore.Valid(s,heroRules),Is.False);
        }
        [Test]public void CompanionKnockoutIdsAndArmySelectionPersistWithoutMultiplication()
        {
            var s=Ready();ExpeditionOperations.Recruit(s,rules,roles,"archer");s.Companions[0].Health=0;var id=s.Companions[0].Id;var path="Logs/E14/party-"+Guid.NewGuid().ToString("N")+".json";
            Assert.That(PrototypeSaveStore.Save(path,s,heroRules),Is.True);Assert.That(PrototypeSaveStore.Load(path,heroRules,out s),Is.True);Assert.That(s.Companions.Single().Id,Is.EqualTo(id));Assert.That(s.Companions[0].Health,Is.EqualTo(0));Assert.That(s.Companions[0].InParty,Is.True);
            var legacy=JsonUtility.ToJson(new PrototypeState());File.WriteAllText(path,legacy);Assert.That(PrototypeSaveStore.Load(path,heroRules,out s),Is.True);Assert.That(s.Companions,Is.Empty);Assert.That(s.Territories,Is.Empty);Assert.That(s.Demo,Is.Null);
        }
        private sealed class Health{public int Value;}
        private static TacticalCombatant Actor(string id,ClassProfile profile,bool hostile=false,bool hero=false)
        {var h=new Health{Value=profile.Health};return new TacticalCombatant{Id=id,Title=id,Hero=hero,Hostile=hostile,Profile=profile,ReadHealth=()=>h.Value,WriteHealth=v=>h.Value=v};}
        [Test]public void GroupInitiativeActionsMovementAndKnockoutsAreAuthoritative()
        {
            var hero=Actor("hero",heroRules.Find("warrior"),hero:true);var friend=Actor("friend",ExpeditionRules.RoleProfile(roles.Find("archer")));var foe=Actor("foe",ExpeditionRules.RoleProfile(roles.Find("swordsman")),true);var battle=TurnBattle.CreateGroup(new[]{hero,friend,foe},42);var g=battle.Group;
            var seen=new HashSet<string>();for(var i=0;i<3;i++){seen.Add(g.Current.Id);Assert.That(g.Move(1),Is.True);Assert.That(g.Move(4),Is.False);var target=g.Current.Hostile?hero:foe;Assert.That(g.Attack(target,100,true),Is.False);Assert.That(g.Attack(target,0,false),Is.False);Assert.That(g.Guard(),Is.True);Assert.That(g.Attack(target,0,true),Is.False);g.EndTurn();}Assert.That(seen.Count,Is.EqualTo(3));Assert.That(g.Round,Is.EqualTo(2));
            friend.Health=0;for(var i=0;i<50&&!g.Finished;i++){var target=g.Current.Hostile?hero:foe;g.Attack(target,0,true);if(!g.Finished)g.EndTurn();}Assert.That(g.Finished,Is.True);Assert.That(friend.Health,Is.EqualTo(0));Assert.That(g.Won,Is.EqualTo(hero.Health>0));
        }
        [Test]public void TerritorialStorageCapIsRespectedAndDefeatGivesNoBonus()
        {var s=Ready();s.Wood=200;ExpeditionOperations.InitializeTerritories(s,rules);ExpeditionOperations.Conquer(s,"outpost",false);WorldOperations.Advance(s,10,settlement);Assert.That(s.Wood,Is.EqualTo(200));ExpeditionOperations.ClaimNeutral(s,rules);WorldOperations.Advance(s,10,settlement);Assert.That(s.Wood,Is.EqualTo(200));}
    }
}
