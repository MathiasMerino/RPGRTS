using System.Collections;
using System.IO;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Aethernia.Tests.Editor
{
    public class PhaseTwoValidationTests
    {
        [SetUp] public void Open()
        { if (!EditorApplication.isPlaying) EditorSceneManager.OpenScene("Assets/01_Scenes/PlayablePrototype.unity"); }

        [Test] public void SceneHeroDefersNativeAgentUntilSurfaceHasLoaded()
        {
            var hero = Object.FindAnyObjectByType<PlayerCommands>().Hero;
            Assert.That(hero.Agent.enabled, Is.False, "Only the scene instance defers enabling until Start; spawn prefabs stay unchanged.");
        }

        [UnityTest]
        public IEnumerator EnteringManagementStopsThePreviousHeroOrder()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>();
            Assert.That(d.Hero.TryMoveTo(new Vector3(-5, 0, -7)), Is.True);
            Assert.That(d.Execute("construct"), Is.True);
            Assert.That(d.Hero.Agent.hasPath, Is.False, "Construction must not retain an exploration order.");
            d.Execute("explore");
            Assert.That(d.Hero.TryMoveTo(new Vector3(-5, 0, -7)), Is.True);
            Assert.That(d.Execute("rts"), Is.True);
            Assert.That(d.Hero.Agent.hasPath, Is.False, "The hero must remain outside the RTS simulation.");
            var before = d.Hero.transform.position;
            yield return null; yield return null;
            Assert.That(Vector3.Distance(d.Hero.transform.position, before), Is.LessThan(0.02f));
            Assert.That(d.Execute("explore"), Is.False);
            Assert.That(d.Save(Path.GetFullPath("Logs/busy-save.json")), Is.False);
            Assert.That(d.Load(Path.GetFullPath("Logs/busy-save.json")), Is.False);
            Assert.That(d.Session.Mode, Is.EqualTo(PrototypeMode.Rts));
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();
            var modeButton=System.Array.Find(hud.GetComponentsInChildren<Button>(true),b=>b.name=="Combate RPG");
            Assert.That(modeButton.interactable, Is.False);Assert.That(modeButton.gameObject.activeInHierarchy, Is.False);
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator FinishedRtsDoesNotLeakParticipantsIntoExplorationOrLoad()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>();
            d.Session.State.Facts.Add("rpg-victory");
            Assert.That(d.Execute("escalate"), Is.True);
            var deadline = Time.realtimeSinceStartup + 45;
            while (d.Rts.Active && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(d.Rts.Won, Is.True);
            foreach (var unit in Object.FindObjectsByType<ExplorationUnit>()) if (unit != d.Hero) Assert.That(unit.Agent.hasPath, Is.False, "Survivors stop when combat ends.");
            var path = Path.GetFullPath("Logs/phase2-world.json");
            Assert.That(d.Execute("explore"), Is.True);
            Assert.That(d.Save(path), Is.True);
            Assert.That(Object.FindObjectsByType<ExplorationUnit>().Length, Is.EqualTo(1), "RTS pawns must be retired immediately.");
            Assert.That(d.Load(path), Is.True);
            Assert.That(d.Session.HasFact("rts-victory"), Is.True);
            Assert.That(d.Encounter.Battle, Is.Null);
            Assert.That(Object.FindObjectsByType<ExplorationUnit>().Length, Is.EqualTo(1));
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator RtsRejectsBlockedDeploymentWithoutEnteringMode()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>();
            Assert.That(d.Construction.Place(new Vector3(-12, 0, 11)), Is.True);
            var deadline = Time.realtimeSinceStartup + 1;
            while (Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(d.Execute("rts"), Is.False, "A persistent workshop must not leave an active, unwinnable battle with invalid agents.");
            Assert.That(d.Rts.Active, Is.False);
            Assert.That(d.Session.Mode, Is.EqualTo(PrototypeMode.Exploration));
            Assert.That(d.Session.State.Buildings.Count, Is.EqualTo(1));
            Assert.That(d.Session.LastMessage, Does.Contain("despliegue"));
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator TacticalAttacksRespectWallsAndTurnCannotResetInTransit()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>();
            d.Session.SelectClass("mage"); Assert.That(d.Encounter.Begin(42), Is.True);
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Phase2 temporary attack blocker";
            wall.transform.position = (d.Hero.transform.position + d.Encounter.EnemyPosition) * 0.5f + Vector3.up;
            wall.transform.localScale = new Vector3(2, 2, 0.3f); Physics.SyncTransforms();
            var hp = d.Encounter.Battle.EnemyHealth;
            d.Encounter.Attack();
            Assert.That(d.Encounter.Battle.EnemyHealth, Is.EqualTo(hp));
            Assert.That(d.Encounter.Battle.ActionAvailable, Is.True, "Occluded attacks must not consume an action.");
            Object.Destroy(wall); yield return null;
            Assert.That(d.Encounter.Move(new Vector3(-5, 0, -7)), Is.True);
            var movement = d.Encounter.Battle.MovementRemaining;
            d.Encounter.EndTurn();
            Assert.That(d.Encounter.Battle.MovementRemaining, Is.EqualTo(movement), "Wait for movement before ending the turn.");
            var deadline = Time.realtimeSinceStartup + 5;
            while (d.Hero.IsMoving && Time.realtimeSinceStartup < deadline) yield return null;
            d.Encounter.EndTurn();
            Assert.That(d.Encounter.Battle.MovementRemaining, Is.EqualTo(4));
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator EnemyTurnFollowsACompletePathWithinThreeMetres()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>();
            Assert.That(d.Encounter.Begin(42), Is.True);
            var enemy = GameObject.Find("RPG Enemy").GetComponent<ExplorationUnit>();
            Assert.That(d.Hero.Warp(new Vector3(-2, 0, 8)), Is.True);
            Assert.That(enemy.Warp(new Vector3(-2, 0, 3.3f)), Is.True);
            var before = enemy.transform.position;
            d.Encounter.EndTurn();
            var after = enemy.transform.position;
            var path = new NavMeshPath();
            Assert.That(NavMesh.CalculatePath(before, after, NavMesh.AllAreas, path), Is.True);
            Assert.That(path.status, Is.EqualTo(NavMeshPathStatus.PathComplete));
            var length = 0f;
            for (var i = 1; i < path.corners.Length; i++) length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            Assert.That(length, Is.GreaterThan(0.1f), "Enemy must advance around the obstacle, not stall.");
            Assert.That(length, Is.LessThanOrEqualTo(3.05f), "A direct Warp must not cross the obstacle or exceed the turn path budget.");
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator InvalidLoadReportsFailureWithoutChangingWorldOrPosition()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>();
            var state = d.Session.State; var before = d.Hero.transform.position;
            var path = Path.GetFullPath("Logs/phase2-invalid.json");
            File.WriteAllText(path, "{broken");
            d.Session.Say("Previous message");
            Assert.That(d.Load(path), Is.False);
            Assert.That(d.Session.LastMessage, Does.Contain("cargar"));
            Assert.That(d.Session.State, Is.SameAs(state)); Assert.That(d.Hero.transform.position, Is.EqualTo(before));
            var candidate = new PrototypeState { HeroPosition = new Vector3(200, 0, 200) };
            Assert.That(PrototypeSaveStore.Save(path, candidate, d.Session.Rules), Is.True);
            Assert.That(d.Load(path), Is.False);
            Assert.That(d.Session.State, Is.SameAs(state)); Assert.That(d.Hero.transform.position, Is.EqualTo(before));
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator ExplorationActionsAreModeGuardedAndHudKeepsSpecificFeedback()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>(); var s = d.Session;
            var gold = s.State.Gold;
            d.Execute("construct");
            Assert.That(s.CompleteInteraction("market", "Market"), Is.False);
            Assert.That(s.State.Gold, Is.EqualTo(gold));
            d.Execute("explore");
            Assert.That(s.CompleteInteraction("unconfigured-node", "Unknown"), Is.False);
            Assert.That(s.State.Gold, Is.EqualTo(gold));
            var chest = GameObject.Find("Interaction Chest");
            var commands = Object.FindAnyObjectByType<PlayerCommands>();
            var screen = Camera.main.WorldToScreenPoint(chest.transform.position);
            Assert.That(commands.IssuePointer(screen, true, false), Is.True);
            Assert.That(d.Hero.Warp(new Vector3(-5, 0, -12)), Is.True); yield return null;
            Object.FindAnyObjectByType<ExplorationHud>().ActionButton.onClick.Invoke();
            Assert.That(s.LastMessage, Does.StartWith("Acércate"), "The HUD must preserve the object's reason for rejection.");
            Assert.That(d.Hero.Warp(new Vector3(-3.4f, 0, -5)), Is.True); yield return null;
            Assert.That(commands.Interact(), Is.True);
            Assert.That(d.Execute("sword"), Is.True); Assert.That(d.Execute("armour"), Is.True);
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();hud.SelectPage(ExplorationPage.Character);
            Assert.That(hud.CompactView.Equipment.text, Does.Contain("espada").And.Contain("túnica"));
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        [TestCase("overflow")]
        [TestCase("clock-behind")]
        [TestCase("clock-ahead")]
        public void InvalidInventoryAndProductionClockAreRejected(string scenario)
        {
            var rules = AssetDatabase.LoadAssetAtPath<PrototypeRules>("Assets/Settings/Prototype/ApprovedRules.asset");
            var s = new PrototypeState();
            if (scenario == "overflow") { s.Items.Clear(); s.Items.Add(new ItemStack("ration", int.MaxValue)); s.Items.Add(new ItemStack("cloth", 1)); }
            if (scenario == "clock-behind") { s.WorldSeconds = 120; s.ProductionCycle = 0; }
            if (scenario == "clock-ahead") { s.WorldSeconds = 0; s.ProductionCycle = 1; }
            Assert.That(PrototypeSaveStore.Valid(s, rules), Is.False);
            var path = Path.GetFullPath("Logs/phase2-corrupt-" + scenario + ".json");
            File.WriteAllText(path, JsonUtility.ToJson(s));
            Assert.That(PrototypeSaveStore.Load(path, rules, out var loaded), Is.False);
            Assert.That(loaded, Is.Null);
        }

        [UnityTearDown] public IEnumerator Cleanup() { if (EditorApplication.isPlaying) yield return new ExitPlayMode(); }
    }
}
