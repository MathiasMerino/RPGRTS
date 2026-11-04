using System;
using System.Collections;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class FrontierCampaignTests
    {
        [Test]
        public void CandidateReusesTacticalAndCivilizationSystems()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.TacticalScenePath);
            var prior=Object.FindAnyObjectByType<DemoFlow>().Session.Rules;
            EditorSceneManager.OpenScene(WorldExplorationBuilder.CampaignScenePath);
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            Assert.That(flow.Session.Rules.FrontierCampaign&&flow.Session.Rules.TacticalWarfare,Is.True);
            Assert.That(prior.FrontierCampaign,Is.False);
            Assert.That(flow.Session.Rules.Civilizations,Is.SameAs(prior.Civilizations));
            Assert.That(flow.Session.Rules.Progression,Is.SameAs(prior.Progression));
            Assert.That(Object.FindObjectsByType<DungeonExpedition>().Length,Is.EqualTo(1));
            Assert.That(Object.FindObjectsByType<TerritoryCampaign>().Length,Is.EqualTo(1));
            Assert.That(AssetDatabase.GetDependencies(WorldExplorationBuilder.CampaignScenePath,true)
                .Any(path=>path.Contains("LocalOnly")||path.Contains("Meshy")),Is.False);
        }

        [Test]
        public void RoadTransactionsRequireEvidenceAndApplyOneDifferentResult()
        {
            var report = new PrototypeState { Gold = 10, Food = 6, Wood = 4, ReputationA = -3 };
            var repair = new PrototypeState { Gold = 4, Food = 6, Wood = 4, ReputationA = -3 };
            Assert.That(FrontierChapter.Step(report),Is.EqualTo(FrontierStep.Arrival));
            Assert.That(FrontierChapter.ReadClue(report),Is.False);
            foreach(var state in new[]{report,repair})
            {
                Assert.That(LostCargoQuest.Accept(state)&&LostCargoQuest.Secure(state)&&LostCargoQuest.Claim(state,out _),Is.True);
                Assert.That(FrontierChapter.ChooseRoad(state,RoadChoice.Report),Is.False);
                Assert.That(FrontierChapter.ReadClue(state),Is.True);
                Assert.That(FrontierChapter.ReadClue(state),Is.False);
                Assert.That(FrontierChapter.Step(state),Is.EqualTo(FrontierStep.Supply));
            }
            repair.Gold=4; // Prueba específica de fondos insuficientes tras investigar.
            Assert.That(FrontierChapter.ChooseRoad(repair,RoadChoice.Repair),Is.False);
            Assert.That(repair.Wood,Is.EqualTo(4));
            Assert.That(FrontierChapter.ChooseRoad(report,RoadChoice.Report),Is.True);
            Assert.That(report.Food,Is.EqualTo(9));
            Assert.That(report.ReputationA,Is.EqualTo(2));
            Assert.That(CivilizationOperations.Relation(report,"faction-a"),Is.EqualTo(FactionRelation.Ally));
            repair.Gold=9;
            Assert.That(FrontierChapter.ChooseRoad(repair,RoadChoice.Repair),Is.True);
            Assert.That(repair.Gold,Is.EqualTo(4));
            Assert.That(repair.Wood,Is.EqualTo(10));
            Assert.That(repair.Food,Is.EqualTo(6));
            Assert.That(CivilizationOperations.Relation(repair,"faction-a"),Is.EqualTo(FactionRelation.Hostile));
            Assert.That(FrontierChapter.ChooseRoad(repair,RoadChoice.Report),Is.False);
            Assert.That(FrontierChapter.ChooseRoad(report,RoadChoice.Repair),Is.False);
            Assert.That(FrontierChapter.Valid(report)&&FrontierChapter.Valid(repair),Is.True);
            repair.Facts.Add(FrontierChapter.Report);
            Assert.That(FrontierChapter.Valid(repair),Is.False,"mutually exclusive saved choices");
        }

        [UnityTest]
        public IEnumerator LegacySlotCanCloseChapterWithoutConquestAndKeepTheWorld()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.CampaignScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            yield return VerifyIndependentPath();
        }

        private static IEnumerator VerifyIndependentPath()
        {
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            var world=flow.GetComponent<WorldExploration>();
            flow.SlotPath="Logs/E21/independent-"+Guid.NewGuid().ToString("N")+".json";
            flow.Director.SavePathOverride=flow.SlotPath;
            Assert.That(flow.Continue("../Docs/Validacion/UX02/PartidasFinales/human-warrior.json"),Is.True);
            var state=flow.Session.State;
            Assert.That(FrontierChapter.Step(state),Is.EqualTo(FrontierStep.Ruins));
            Assert.That(FrontierChapter.Decision(state),Is.EqualTo(RoadChoice.None),"legacy save is not given an invented choice");
            Assert.That(FrontierChapter.Finish(state),Is.False);
            var territory=Object.FindAnyObjectByType<TerritoryCampaign>();
            // Estado de prueba tras despejar ambos encuentros. La ruta Windows verifica el combate completo.
            state.Facts.Add("umbral-battle-1");
            state.Facts.Add("umbral-battle-2");
            Assert.That(ExpeditionOperations.RewardDungeon(state,territory.Rules),Is.True);
            flow.Session.AwardForFact("umbral-reward");
            yield return WorldExplorationRoute.Use(flow,FrontierChapter.Liaison,NoCapture,"Debrief");
            Assert.That(FrontierChapter.Step(state),Is.EqualTo(FrontierStep.Debrief));
            world.ConfirmButton.onClick.Invoke();
            Assert.That(FrontierChapter.Step(state),Is.EqualTo(FrontierStep.Conflict));
            yield return WorldExplorationRoute.Use(flow,FrontierChapter.Liaison,NoCapture,"Independent");
            world.ConfirmButton.onClick.Invoke();
            Assert.That(FrontierChapter.Step(state),Is.EqualTo(FrontierStep.Free));
            Assert.That(state.Facts,Does.Contain(FrontierChapter.Independent));
            Assert.That(state.Territories.Single(t=>t.Id=="outpost").Owner,Is.EqualTo("faction-b"));
            Assert.That(flow.ReturnToTitle()&&flow.Continue(),Is.True);
            Assert.That(FrontierChapter.Step(flow.Session.State),Is.EqualTo(FrontierStep.Free));
            Assert.That(flow.Session.State.Facts.Count(f=>f==FrontierChapter.Finished),Is.EqualTo(1));
            Assert.That(flow.Session.State.Territories.Single(t=>t.Id=="outpost").Owner,Is.EqualTo("faction-b"));
            LogAssert.NoUnexpectedReceived();
        }

        private static IEnumerator NoCapture(string _) { yield break; }

        [UnityTearDown]
        public IEnumerator Exit()
        {
            if(EditorApplication.isPlaying)yield return new ExitPlayMode();
        }
    }
}
