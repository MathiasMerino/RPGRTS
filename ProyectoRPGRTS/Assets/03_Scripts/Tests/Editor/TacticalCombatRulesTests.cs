using System;
using NUnit.Framework;
using UnityEngine;

namespace Aethernia.Tests.Editor
{
    public class TacticalCombatRulesTests
    {
        [TestCase(true,true,true,true,true,false,EnemyIntent.Attack)]
        [TestCase(true,true,true,false,true,true,EnemyIntent.Approach)]
        [TestCase(true,true,true,true,false,true,EnemyIntent.Approach)]
        [TestCase(true,true,true,false,false,false,EnemyIntent.Wait)]
        [TestCase(false,true,true,true,true,true,EnemyIntent.Wait)]
        [TestCase(true,false,true,true,true,true,EnemyIntent.Wait)]
        [TestCase(true,true,false,true,true,true,EnemyIntent.Wait)]
        public void MeleeSelectsOnlyPermittedIntent(bool turn,bool alive,bool target,bool reach,bool line,bool path,EnemyIntent expected)
        {Assert.That(MeleeEnemyPolicy.Decide(new EnemySituation(turn,alive,target,reach,line,path)),Is.EqualTo(expected));}
        [TestCase("warrior")][TestCase("monk")][TestCase("mage")]
        public void DiceAndDamagePreserveApprovedRulesAndRejectDuplicates(string id)
        {
            var rules=ScriptableObject.CreateInstance<PrototypeRules>();
            try
            {
                var profile=rules.Find(id);var random=new System.Random(42);var hi=random.Next(1,21)+(id=="monk"?2:0);var ei=random.Next(1,21);var health=profile.Health;
                if(ei>hi&&random.Next(1,21)+2>=13)health-=3;
                var battle=new TurnBattle(profile,profile.Health,1,1,42,true);
                Assert.That(battle.HeroInitiative,Is.EqualTo(hi));Assert.That(battle.EnemyInitiative,Is.EqualTo(ei));Assert.That(battle.HeroHealth,Is.EqualTo(health));
                Assert.That(battle.Attack(profile.Range+.2f),Is.False);Assert.That(battle.Attack(float.NaN),Is.False);Assert.That(battle.Attack(-1),Is.False);
                var roll=random.Next(1,21);Assert.That(battle.Attack(profile.Range),Is.True);var result=battle.LastHeroResolution;
                Assert.That(result.Roll,Is.EqualTo(roll));Assert.That(result.Hit,Is.EqualTo(roll+2>=12));Assert.That(result.Damage,Is.EqualTo(roll+2>=12?profile.Damage+1:0));
                Assert.That(battle.Attack(0),Is.False);Assert.That(battle.Guard(),Is.False);Assert.That(battle.EnemyHealth,Is.EqualTo(18-result.Damage));
                Assert.That(battle.BeginEnemyTurn(),Is.True);Assert.That(battle.BeginEnemyTurn(),Is.False);Assert.That(battle.Attack(0),Is.False);Assert.That(battle.Move(1),Is.False);
                var enemyRoll=random.Next(1,21);Assert.That(battle.ResolveEnemyAction(true),Is.True);var after=battle.HeroHealth;
                Assert.That(battle.LastEnemyResolution.Roll,Is.EqualTo(enemyRoll));Assert.That(after,Is.EqualTo(health-(enemyRoll+2>=13?3:0)));
                Assert.That(battle.ResolveEnemyAction(true),Is.False);Assert.That(battle.HeroHealth,Is.EqualTo(after));Assert.That(battle.FinishEnemyTurn(),Is.True);Assert.That(battle.FinishEnemyTurn(),Is.False);Assert.That(battle.Round,Is.EqualTo(2));
            }
            finally{UnityEngine.Object.DestroyImmediate(rules);}
        }
        [Test]public void GuardRecoveryAndWaitUseExistingResourcesOnce()
        {
            var rules=ScriptableObject.CreateInstance<PrototypeRules>();
            try
            {
                var battle=new TurnBattle(rules.Find("warrior"),12,0,0,42,false);
                Assert.That(battle.Recover(),Is.True);Assert.That(battle.LastHeroResolution.Healing,Is.EqualTo(4));Assert.That(battle.Recover(),Is.False);
                battle.EndTurn(false);Assert.That(battle.LastEnemyResolution.Kind,Is.EqualTo(CombatActionKind.Wait));Assert.That(battle.HeroHealth,Is.EqualTo(16));Assert.That(battle.Recover(),Is.False);
                Assert.That(battle.Move(4),Is.True);Assert.That(battle.Move(.1f),Is.False);Assert.That(battle.Move(float.PositiveInfinity),Is.False);
                Assert.That(battle.Guard(),Is.True);battle.EndTurn(true);if(battle.LastEnemyResolution.Hit)Assert.That(battle.LastEnemyResolution.Damage,Is.EqualTo(2));
                Assert.That(battle.Guarding,Is.False);Assert.That(battle.MovementRemaining,Is.EqualTo(4));
            }
            finally{UnityEngine.Object.DestroyImmediate(rules);}
        }
        [Test]public void SuccessfulDamageAndGuardAreAppliedExactlyOnce()
        {
            var rules=ScriptableObject.CreateInstance<PrototypeRules>();
            try
            {
                var battle=new TurnBattle(rules.Find("warrior"),24,1,0,1,false);var hit=false;
                for(var i=0;i<20&&!hit;i++)
                {
                    var before=battle.EnemyHealth;Assert.That(battle.Attack(1),Is.True);
                    if(battle.LastHeroResolution.Hit){hit=true;Assert.That(battle.LastHeroResolution.Damage,Is.EqualTo(7));Assert.That(battle.EnemyHealth,Is.EqualTo(before-7));Assert.That(battle.Attack(1),Is.False);Assert.That(battle.EnemyHealth,Is.EqualTo(before-7));}
                    battle.EndTurn(false);
                }
                Assert.That(hit,Is.True);var guarded=new TurnBattle(rules.Find("warrior"),24,0,0,42,false);hit=false;
                for(var i=0;i<20&&!hit;i++){Assert.That(guarded.Guard(),Is.True);guarded.EndTurn(true);hit=guarded.LastEnemyResolution.Hit;}
                Assert.That(hit,Is.True);Assert.That(guarded.LastEnemyResolution.Guarded,Is.True);Assert.That(guarded.LastEnemyResolution.Damage,Is.EqualTo(2));
            }
            finally{UnityEngine.Object.DestroyImmediate(rules);}
        }
    }
}
