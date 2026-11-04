using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class TacticalWarfareTests
    {
        [TestCase(RtsFormation.Line)]
        [TestCase(RtsFormation.Column)]
        [TestCase(RtsFormation.Compact)]
        public void SlotsRemainDistinctAndCentredForSmallArmies(RtsFormation shape)
        {
            for (var count = 5; count <= 10; count++)
            {
                var slots = Enumerable.Range(0, count).Select(i => RtsBattleController.FormationOffset(shape, i, count)).ToArray();
                Assert.That(slots.Distinct().Count(), Is.EqualTo(count));
                Assert.That(slots.Max(p => p.x) + slots.Min(p => p.x), Is.EqualTo(0).Within(.01));
                Assert.That(slots.Max(p => p.z) + slots.Min(p => p.z), Is.EqualTo(0).Within(.01));
            }
        }

        [Test]
        public void CandidateKeepsUx02NavigationAndPublicSystems()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.ScenePath);
            var source = Object.FindAnyObjectByType<DemoFlow>().Session.Rules;
            Assert.That(source.TacticalWarfare, Is.False);
            EditorSceneManager.OpenScene(WorldExplorationBuilder.TacticalScenePath);
            var flow = Object.FindAnyObjectByType<DemoFlow>();
            Assert.That(flow.Session.Rules.TacticalWarfare && flow.Session.Rules.WorldExploration, Is.True);
            Assert.That(flow.Session.Rules.Civilizations, Is.SameAs(source.Civilizations));
            Assert.That(flow.Session.Rules.Progression, Is.SameAs(source.Progression));
            Assert.That(Object.FindAnyObjectByType<TerritoryCampaign>(), Is.Not.Null);
            Assert.That(AssetDatabase.GetDependencies(WorldExplorationBuilder.TacticalScenePath, true)
                .Any(path => path.Contains("LocalOnly") || path.Contains("Meshy")), Is.False);
        }

        [UnityTest]
        public IEnumerator LegacySaveDeploysPaidArmyAndConquersWithSameHero()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.TacticalScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            var flow = LoadLegacy("dwarf-mage");
            var territory = Object.FindAnyObjectByType<TerritoryCampaign>();
            yield return TacticalWarfareRoute.PrepareArmy(flow);
            var state = flow.Session.State;
            foreach (var record in state.Army) Assert.That(territory.SelectSoldier(record.Id, true), Is.True);
            yield return StartPilot(flow, territory);
            yield return TacticalWarfareRoute.Fight(flow, Capture);
            Assert.That(flow.ReturnToTitle() && flow.Continue(), Is.True);
            Assert.That(flow.Session.State.Territories.Single(t => t.Id == "outpost").Owner, Is.EqualTo(state.FactionId));
            Assert.That(flow.Session.State.Army.Select(r => r.Id), Is.EqualTo(state.Army.Select(r => r.Id)));
            Assert.That(flow.Director.Rts.Active, Is.False);
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator RetreatKeepsTerritoryAndSavePreservesOriginalOwnership()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.TacticalScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            var flow = LoadLegacy("human-warrior");
            var state = flow.Session.State;
            var territory = Object.FindAnyObjectByType<TerritoryCampaign>();
            foreach (var record in state.Army) Assert.That(territory.SelectSoldier(record.Id, true), Is.True);
            yield return StartPilot(flow, territory);
            var rts = flow.Director.Rts;
            Assert.That(rts.RetreatTactical(), Is.False);
            Assert.That(flow.Director.Save(flow.SlotPath), Is.False, "save during an unresolved expedition");
            rts.SelectAll();
            Assert.That(rts.Command(new Vector3(-39, 0, 2), null, false), Is.True);
            var deadline = Time.realtimeSinceStartup + 30;
            while (!rts.TacticalReady && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(rts.StartTacticalBattle(), Is.True);
            Assert.That(rts.RetreatTactical(), Is.True);
            Assert.That(rts.LastResolvedVictory, Is.False);
            Assert.That(state.Territories.Single(t => t.Id == "outpost").Owner, Is.EqualTo("faction-b"));
            Assert.That(state.Facts, Does.Not.Contain("umbral-outpost-conquered"));
            Assert.That(flow.Session.Mode, Is.EqualTo(PrototypeMode.Exploration));
            Assert.That(flow.ReturnToTitle() && flow.Continue(), Is.True);
            Assert.That(flow.Session.State.Territories.Single(t => t.Id == "outpost").Owner, Is.EqualTo("faction-b"));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator EnemyFireDefeatsWoundedHeroWithoutTransferringTheOutpost()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.TacticalScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            var flow = LoadLegacy("human-warrior");
            var state = flow.Session.State;
            var territory = Object.FindAnyObjectByType<TerritoryCampaign>();
            state.Health = 1; // Fixture explícita de derrota, sin alterar los defensores ni el daño.
            Assert.That(territory.SelectSoldier(state.Army[0].Id, true), Is.True);
            yield return StartPilot(flow, territory);

            var rts = flow.Director.Rts;
            var hero = flow.Director.Hero;
            Assert.That(rts.SelectUnit(hero.GetComponentInChildren<Collider>(), false), Is.True);
            rts.CycleStance();
            rts.CycleStance();
            Assert.That(rts.SelectedStance, Is.EqualTo(RtsStance.Passive));
            Assert.That(rts.Command(new Vector3(-34, 0, 2), null, false), Is.True);
            yield return UxExperienceRoute.Arrive(hero);
            yield return UxExperienceRoute.Wait(.25f);
            Assert.That(state.Health, Is.EqualTo(1));
            Assert.That(rts.StartTacticalBattle(), Is.True);
            var deadline = Time.realtimeSinceStartup + 35;
            while (rts.Active && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(state.Health, Is.Zero, "los defensores golpean al héroe");
            Assert.That(rts.LastResolvedVictory, Is.False);
            Assert.That(state.Territories.Single(t => t.Id == "outpost").Owner, Is.EqualTo("faction-b"));
            Assert.That(state.Facts, Does.Not.Contain("umbral-outpost-conquered"));
            Assert.That(flow.Session.Mode, Is.EqualTo(PrototypeMode.Exploration));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator ArenaCoverBlocksSightAndThreePassagesAreNavigable()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.TacticalScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            foreach (var z in new[] { -7f, 2f, 12f })
            {
                var path = new NavMeshPath();
                Assert.That(NavMesh.CalculatePath(new Vector3(-35, 0, z), new Vector3(-28, 0, z),
                    NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete, Is.True, "passage at z=" + z);
            }
            Assert.That(Physics.Linecast(new Vector3(-33, .9f, -4.5f), new Vector3(-30, .9f, -4.5f),
                out var hit, ~0, QueryTriggerInteraction.Ignore), Is.True);
            Assert.That(hit.collider.name, Does.Contain("StoneWall"));
        }

        private static DemoFlow LoadLegacy(string identity)
        {
            var flow = Object.FindAnyObjectByType<DemoFlow>();
            Directory.CreateDirectory("Logs/E20");
            flow.SlotPath = "Logs/E20/test-" + Guid.NewGuid().ToString("N") + ".json";
            flow.Director.SavePathOverride = flow.SlotPath;
            Assert.That(flow.Continue("../Docs/Validacion/UX02/PartidasFinales/" + identity + ".json"), Is.True);
            return flow;
        }

        private static IEnumerator StartPilot(DemoFlow flow, TerritoryCampaign territory)
        {
            foreach (var id in new[] { "crossing", "outpost" })
            {
                Assert.That(territory.March(id), Is.True);
                yield return UxExperienceRoute.Arrive(flow.Director.Hero);
                var point = Object.FindObjectsByType<InteractionPoint>().Single(p => p.FactId == "territory-" + id);
                Assert.That(point.TryInteract(flow.Director.Hero, flow.Session), Is.True, id);
            }
            Assert.That(flow.Director.Rts.TacticalDeployment, Is.True);
        }

        private static IEnumerator Capture(string name)
        {
            Directory.CreateDirectory("Logs/E20/Captures");
            yield return DemoCapture.Save("Logs/E20/Captures/" + name + ".png", 1280, 720);
        }

        [UnityTearDown]
        public IEnumerator Exit()
        {
            if (EditorApplication.isPlaying) yield return new ExitPlayMode();
        }
    }
}
