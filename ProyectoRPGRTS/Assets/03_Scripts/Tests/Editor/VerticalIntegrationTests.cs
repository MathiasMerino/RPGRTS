using System.Collections;
using System.IO;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Aethernia.Tests.Editor
{
    public class VerticalIntegrationTests
    {
        [SetUp] public void Open() { if (!EditorApplication.isPlaying) EditorSceneManager.OpenScene("Assets/01_Scenes/PlayablePrototype.unity"); }
        [Test] public void FinalSceneHasNoMissingSerializedReferences() { EnvironmentValidationTests.AssertSceneReferences(UnityEngine.SceneManagement.SceneManager.GetActiveScene()); }
        [UnityTest]
        public IEnumerator ClassesConstructionDungeonAndSaveShareOneState()
        {
            yield return new EnterPlayMode(); yield return null; yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>(); var s = d.Session;
            foreach (var profile in s.Rules.Classes)
            { s.SelectClass(profile.Id); Assert.That(s.State.Health, Is.EqualTo(profile.Health)); Assert.That(d.Hero.Agent.speed, Is.EqualTo(profile.Speed)); }
            Assert.That(s.Rules.Classes.Length, Is.EqualTo(3));
            s.Rename("Validador"); Assert.That(s.State.Name, Is.EqualTo("Validador"));
            Assert.That(d.Hero.Warp(new Vector3(-3.4f, 0, -5)), Is.True);
            yield return null;
            Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(d.Hero, s), Is.True);
            Assert.That(d.Execute("sword"), Is.True); Assert.That(d.Execute("armour"), Is.True);
            Assert.That(d.Execute("trade"), Is.True); d.Execute("construct");
            Assert.That(d.Construction.Place(new Vector3(12, 0, -10)), Is.True);
            var carvingDeadline = Time.realtimeSinceStartup + 1;
            while (Time.realtimeSinceStartup < carvingDeadline) yield return null;
            Assert.That(d.Hero.TryMoveTo(new Vector3(12, 0, -10)), Is.False, "The newly carved workshop must block navigation.");
            var gold = s.State.Gold;
            Assert.That(d.Construction.Place(new Vector3(12, 0, -10)), Is.False); Assert.That(s.State.Gold, Is.EqualTo(gold));
            Assert.That(d.Construction.Place(new Vector3(-2, 0, 5)), Is.False);
            var food = s.State.Food; WorldOperations.Advance(s.State, 10); Assert.That(s.State.Food, Is.EqualTo(food - 1));
            d.Execute("explore");
            var path = Path.GetFullPath("Logs/vertical-save.json"); Assert.That(d.Save(path), Is.True);
            gold = s.State.Gold; s.State.Gold = 999; Assert.That(d.Load(path), Is.True); Assert.That(s.State.Gold, Is.EqualTo(gold));
            Assert.That(s.State.Buildings.Count, Is.EqualTo(1)); Assert.That(s.State.Weapon, Is.EqualTo("training-sword"));
            Assert.That(d.Hero.Warp(new Vector3(-9, 0, 5)), Is.True); yield return null;
            Assert.That(GameObject.Find("Dungeon Entrance").GetComponent<InteractionPoint>().TryInteract(d.Hero, s), Is.True);
            Assert.That(d.Hero.transform.position.x, Is.GreaterThan(40));
            Assert.That(d.Hero.Warp(new Vector3(50.5f, 0, 1)), Is.True); yield return null;
            var objective = GameObject.Find("Dungeon Objective").GetComponent<InteractionPoint>();
            Assert.That(objective.TryInteract(d.Hero, s), Is.True); Assert.That(objective.TryInteract(d.Hero, s), Is.False);
            Assert.That(s.State.ReputationA, Is.EqualTo(10)); Assert.That(s.State.ReputationB, Is.EqualTo(-5));
            yield return SceneCapture.Save("Logs/Dungeon-preview.png");
            Assert.That(d.Save(path), Is.True, "Save after dungeon: " + PrototypeSaveStore.LastWriteFailure); s.State.ReputationA = 0;
            Assert.That(d.Load(path), Is.True); Assert.That(s.State.ReputationA, Is.EqualTo(10)); Assert.That(s.HasFact("dungeon-objective"), Is.True);
            File.WriteAllText(path, "invalid"); gold = s.State.Gold;
            Assert.That(d.Load(path), Is.False); Assert.That(s.State.Gold, Is.EqualTo(gold));
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }
        [UnityTest]
        public IEnumerator RpgVictoryEscalatesToRtsWhileCalendarIsPaused()
        {
            yield return new EnterPlayMode(); yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>(); var s = d.Session;
            s.SelectClass("mage"); Assert.That(d.Encounter.Begin(42), Is.True); yield return null;
            var clock = s.State.WorldSeconds;
            yield return SceneCapture.Save("Logs/Rpg-preview.png");
            Assert.That(d.Hero.Warp(d.Encounter.EnemyPosition + Vector3.right * 1.3f), Is.True);
            for (var i = 0; i < 20 && !d.Encounter.Battle.Finished; i++)
            { d.Encounter.Attack(); if (!d.Encounter.Battle.Finished) d.Encounter.EndTurn(); yield return null; }
            Assert.That(d.Encounter.Battle.Won, Is.True); Assert.That(s.HasFact("rpg-victory"), Is.True);
            Assert.That(s.State.WorldSeconds, Is.EqualTo(clock).Within(0.001f));
            Assert.That(d.Execute("escalate"), Is.True); Assert.That(d.Rts.StartingAllyHealth, Is.EqualTo(14));
            Assert.That(d.Rts.Allies, Is.EqualTo(6)); Assert.That(d.Rts.Enemies, Is.EqualTo(6));
            yield return SceneCapture.Save("Logs/Rts-preview.png");
            var frames = Time.frameCount; var started = Time.realtimeSinceStartup;
            var deadline = Time.realtimeSinceStartup + 45;
            while (d.Rts.Active && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(d.Rts.Active, Is.False, "RTS must conclude, not stall in navigation.");
            Assert.That(d.Rts.Won, Is.True); Assert.That(s.HasFact("rts-victory"), Is.True);
            Assert.That(s.State.WorldSeconds, Is.EqualTo(clock).Within(0.001f));
            File.WriteAllText("Logs/Vertical-performance.txt", "RTS editor GPU batch: seconds=" + (Time.realtimeSinceStartup - started).ToString("0.00") + "; frames=" + (Time.frameCount - frames) + "; allocatedBytes=" + UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong());
            d.Execute("explore"); Assert.That(s.Mode, Is.EqualTo(PrototypeMode.Exploration));
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }
        [UnityTearDown] public IEnumerator Cleanup() { if (EditorApplication.isPlaying) yield return new ExitPlayMode(); }
    }
}
