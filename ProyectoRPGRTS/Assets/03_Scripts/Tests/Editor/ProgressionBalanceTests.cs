using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
namespace Aethernia.Tests.Editor
{
    public sealed class ProgressionBalanceTests
    {
        [Test]public void EnemyRecoveryConsumesNormalActionAndIsOncePerActor()
        {
            var heroHp=24;var enemyHp=6;var g=new GroupTurnSequence(new[]{new TacticalCombatant{Id="hero",Hero=true,Profile=new ClassProfile("warrior","Hero",24,6,2,3),ReadHealth=()=>heroHp,WriteHealth=x=>heroHp=x},new TacticalCombatant{Id="enemy",Hostile=true,Profile=new ClassProfile("warrior","Enemy",16,3,2,3),ReadHealth=()=>enemyHp,WriteHealth=x=>enemyHp=x}},42);while(!g.Current.Hostile)g.EndTurn();
            Assert.That(g.TryEnemyRecovery(false),Is.False);Assert.That(g.TryEnemyRecovery(true),Is.True);Assert.That(enemyHp,Is.EqualTo(10));Assert.That(g.ActionAvailable,Is.False);Assert.That(g.TryEnemyRecovery(true),Is.False);g.EndTurn();g.EndTurn();enemyHp=5;Assert.That(g.TryEnemyRecovery(true),Is.False);Assert.That(g.ActionAvailable,Is.True);
        }
        [TestCase("warrior"),TestCase("monk"),TestCase("mage")]
        public void ControlledSeedMatrixDistinguishesPreparedPartyFromSoloRisk(string id)
        {
            var rules=AssetDatabase.LoadAssetAtPath<PrototypeRules>(ProgressionSceneBuilder.RulesPath);var rows=new List<string>{"class,party,seed,win,rounds,heroHealth"};var prepared=0;var soloLoss=0;
            foreach(var party in new[]{false,true})for(var seed=1;seed<=120;seed++)
            {
                var s=new PrototypeState{ClassId=id,Health=rules.Find(id).Health};ProgressionOperations.Activate(s,rules.Progression);foreach(var fact in new[]{"rpg-victory","demo-camp-secured","lost-cargo-completed"}){s.Facts.Add(fact);ProgressionOperations.GrantForFact(s,rules.Progression,fact);}ProgressionOperations.Learn(s,rules.Progression,id+"-discipline");var hp=ProgressionOperations.ResolveProfile(s,rules).Health;
                var members=new List<TacticalCombatant>{new TacticalCombatant{Id="hero",Hero=true,Profile=ProgressionOperations.ResolveProfile(s,rules),Weapon=1,Armour=1,ReadHealth=()=>hp,WriteHealth=x=>hp=x}};
                if(party)foreach(var role in rules.Progression.Units.Roles){var health=role.Health;members.Add(new TacticalCombatant{Id=role.Id,Profile=ExpeditionRules.RoleProfile(role),ReadHealth=()=>health,WriteHealth=x=>health=x});}
                foreach(var name in new[]{"swordsman","archer"}){var role=rules.Progression.Units.Find(name);var p=ExpeditionRules.RoleProfile(role);p.Health+=rules.Progression.DungeonHealthBonus;p.Damage+=rules.Progression.DungeonDamageBonus;var health=p.Health;members.Add(new TacticalCombatant{Id="enemy-"+name,Hostile=true,Profile=p,ReadHealth=()=>health,WriteHealth=x=>health=x});}
                var g=new GroupTurnSequence(members,seed);for(var turn=0;turn<160&&!g.Finished;turn++){var current=g.Current;var target=g.Members.First(a=>a.Hostile!=current.Hostile&&a.Health>0);if(!g.TryEnemyRecovery(true)){if(current.Health*2<=current.Profile.Health&&!current.Recovered)g.Recover();else if(!current.Hero||!g.UseTechnique(target,1,true))g.Attack(target,1,true);}if(!g.Finished)g.EndTurn();}Assert.That(g.Finished,Is.True,"bounded simulation");if(party&&g.Won)prepared++;if(!party&&!g.Won)soloLoss++;rows.Add(id+","+party+","+seed+","+g.Won+","+g.Round+","+hp);
            }
            Directory.CreateDirectory("Logs/E18");File.WriteAllLines("Logs/E18/balance-"+id+".csv",rows);Assert.That(prepared,Is.GreaterThanOrEqualTo(100),"prepared party can progress in this geometry-free model");Assert.That(soloLoss,Is.GreaterThan(0),"enemies can defeat an unaccompanied hero; simulation is not native gameplay QA");
        }
    }
}
