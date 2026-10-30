using System.IO;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Aethernia.Tests.Editor
{
    public class VerticalModelTests
    {
        private PrototypeRules rules;
        [SetUp] public void Setup() { rules = AssetDatabase.LoadAssetAtPath<PrototypeRules>("Assets/Settings/Prototype/ApprovedRules.asset"); Assert.That(rules, Is.Not.Null); }
        [Test]
        public void InventoryEnforcesCapacityOwnershipAndConsumption()
        {
            var s = new PrototypeState();
            Assert.That(WorldOperations.AddItem(s, "unknown", 1), Is.False);
            Assert.That(WorldOperations.AddItem(s, "ration", 7), Is.False);
            Assert.That(WorldOperations.Equip(s, "training-sword"), Is.False);
            Assert.That(WorldOperations.AddItem(s, "training-sword", 1), Is.True);
            Assert.That(WorldOperations.Equip(s, "training-sword"), Is.True);
            s.Health = 10;
            Assert.That(WorldOperations.UseRation(s, 24), Is.True);
            Assert.That(s.Health, Is.EqualTo(14));
            Assert.That(s.Items.Find(i => i.Id == "ration").Count, Is.EqualTo(1));
        }
        [Test]
        public void EconomyConservesCostsAndProducesOnlyWithFood()
        {
            var s = new PrototypeState();
            Assert.That(WorldOperations.Trade(s), Is.True); Assert.That(s.Gold, Is.EqualTo(45)); Assert.That(s.Wood, Is.EqualTo(22));
            Assert.That(WorldOperations.BuildWorkshop(s, new Vector3(12, 0, -10)), Is.True);
            Assert.That(s.Gold, Is.EqualTo(35)); Assert.That(s.Wood, Is.EqualTo(17));
            WorldOperations.Advance(s, 20); Assert.That(s.Food, Is.EqualTo(8)); Assert.That(s.Wood, Is.EqualTo(19));
            s.Food = 0; WorldOperations.Advance(s, 20); Assert.That(s.Wood, Is.EqualTo(19));
            s.Gold = 0; Assert.That(WorldOperations.Trade(s), Is.False);
            Assert.That(WorldOperations.BuildWorkshop(s, Vector3.zero), Is.False);
        }
        [Test]
        public void TurnRulesLimitActionsMovementAndFinishDeterministically()
        {
            var battle = new TurnBattle(rules.Find("warrior"), 24, 1, 1, 42);
            Assert.That(battle.Attack(20), Is.False);
            Assert.That(battle.Move(5), Is.False); Assert.That(battle.Move(3), Is.True);
            Assert.That(battle.Move(2), Is.False);
            Assert.That(battle.Guard(), Is.True); Assert.That(battle.Attack(1), Is.False);
            battle.EndTurn(true); Assert.That(battle.MovementRemaining, Is.EqualTo(4));
            for (var i = 0; i < 30 && !battle.Finished; i++) { battle.Attack(1); if (!battle.Finished) battle.EndTurn(true); }
            Assert.That(battle.Finished, Is.True); Assert.That(battle.HeroHealth, Is.GreaterThanOrEqualTo(0)); Assert.That(battle.EnemyHealth, Is.GreaterThanOrEqualTo(0));
        }
        [Test]
        public void WinningInitiativeDoesNotGuaranteeAnEnemyHit()
        {
            var battle = new TurnBattle(rules.Find("warrior"), 24, 0, 1, 21);
            Assert.That(battle.EnemyInitiative, Is.GreaterThan(battle.HeroInitiative));
            Assert.That(battle.HeroHealth, Is.EqualTo(24), "The enemy's opening attack misses in this seeded encounter.");
            Assert.That(battle.ActionAvailable, Is.True);
        }
        [Test]
        public void SaveRoundtripRejectsCorruptionAndUnknownVersions()
        {
            var path = Path.GetFullPath("Logs/model-save.json"); var s = new PrototypeState();
            WorldOperations.CompleteQuest(s); WorldOperations.AddItem(s, "cloth", 1); WorldOperations.Equip(s, "cloth");
            Assert.That(PrototypeSaveStore.Save(path, s, rules), Is.True);
            Assert.That(PrototypeSaveStore.Load(path, rules, out var restored), Is.True);
            Assert.That(restored.ReputationA, Is.EqualTo(10)); Assert.That(restored.Armour, Is.EqualTo("cloth"));
            Assert.That(WorldOperations.CompleteQuest(restored), Is.False);
            s.Version = 99; Assert.That(PrototypeSaveStore.Save(path, s, rules), Is.False);
            s.Version = 1; s.Buildings.Add(new BuildingRecord("workshop", new Vector3(0, float.NaN, 0)));
            Assert.That(PrototypeSaveStore.Valid(s, rules), Is.False); s.Buildings.Clear();
            s.Gold = int.MaxValue; Assert.That(PrototypeSaveStore.Valid(s, rules), Is.False);
            File.WriteAllText(path, "{broken"); Assert.That(PrototypeSaveStore.Load(path, rules, out _), Is.False);
            File.WriteAllText(path, "{\"Version\":99,\"ClassId\":\"warrior\"}"); Assert.That(PrototypeSaveStore.Load(path, rules, out _), Is.False);
        }
    }
}
