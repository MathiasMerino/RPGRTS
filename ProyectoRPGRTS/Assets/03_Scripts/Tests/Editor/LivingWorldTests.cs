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
    public sealed class LivingWorldTests
    {
        [Test]
        public void ClaimsRequireAcceptanceAndEvidenceAndCannotPayTwice()
        {
            var state = new PrototypeState();
            var survey = FrontierContracts.At(0);
            var watch = FrontierContracts.At(2);
            Assert.That(FrontierContracts.Status(state, watch), Is.EqualTo(ContractStatus.Locked));
            Assert.That(FrontierContracts.Accept(state, watch), Is.False);
            Assert.That(FrontierContracts.Claim(state, survey), Is.False);
            Assert.That(FrontierContracts.Accept(state, survey), Is.True);
            Assert.That(FrontierContracts.Accept(state, survey), Is.False);
            Assert.That(FrontierContracts.Claim(state, survey), Is.False);
            state.Facts.Add(FrontierContracts.Cairn);
            var gold = state.Gold;
            Assert.That(FrontierContracts.Status(state, survey), Is.EqualTo(ContractStatus.Ready));
            Assert.That(FrontierContracts.Claim(state, survey), Is.True);
            Assert.That(FrontierContracts.Claim(state, survey), Is.False);
            Assert.That(state.Gold, Is.EqualTo(gold + survey.Gold));
            state.Facts.Remove(survey.Accepted);
            Assert.That(FrontierContracts.Valid(state), Is.False);
        }

        [Test]
        public void TradeRejectsInsufficientGoldFullInventoryAndExcessResources()
        {
            var rules = AssetDatabase.LoadAssetAtPath<PrototypeRules>("Assets/20_E22/LivingRules.asset");
            var state = new PrototypeState { Gold = 2, Wood = 999999 };
            Assert.That(WorldOperations.BuyRation(state), Is.False);
            state.Gold = 10;
            Assert.That(WorldOperations.Trade(state), Is.False);
            state.Items.Single().Count = 8;
            Assert.That(WorldOperations.BuyRation(state), Is.False);
            Assert.That(state.Gold, Is.EqualTo(10));
            Assert.That(EquipmentOperations.Sell(state, "ration", rules), Is.True);
            Assert.That(WorldOperations.BuyRation(state), Is.True);
            Assert.That(state.Items.Single().Count, Is.EqualTo(8));
            Assert.That(state.Gold, Is.EqualTo(8));
        }

        [UnityTest]
        public IEnumerator BoardAndMerchantPersistTransactionsThroughLiveScene()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.LivingScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return VerifyActivities();
        }

        private static IEnumerator VerifyActivities()
        {
            var flow = Object.FindAnyObjectByType<DemoFlow>();
            var world = flow.GetComponent<WorldExploration>();
            var original = "../Docs/Validacion/UX02/PartidasFinales/human-warrior.json";
            flow.SlotPath = "Logs/E22/activities-" + Guid.NewGuid().ToString("N") + ".json";
            Assert.That(flow.Continue(original), Is.True);
            var game = flow.Session;
            var contract = FrontierContracts.At(0);
            Assert.That(game.Rules.LivingWorld, Is.True);
            Assert.That(FrontierContracts.Status(game.State, contract), Is.EqualTo(ContractStatus.Available));
            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Board, NoCapture, "Board");
            world.ConfirmButton.onClick.Invoke();
            Assert.That(FrontierContracts.Status(game.State, contract), Is.EqualTo(ContractStatus.Accepted));
            world.Close();
            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Cairn, NoCapture, "Cairn");
            world.ConfirmButton.onClick.Invoke();
            Assert.That(FrontierContracts.Status(game.State, contract), Is.EqualTo(ContractStatus.Ready));
            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Board, NoCapture, "Return");
            var beforeGold = game.State.Gold;
            var beforeXp = game.State.Experience;
            world.ConfirmButton.onClick.Invoke();
            Assert.That(game.State.Gold, Is.EqualTo(beforeGold + contract.Gold));
            Assert.That(game.State.Experience, Is.EqualTo(Math.Min(game.Rules.Progression.MaximumExperience, beforeXp + contract.Experience)));
            Assert.That(game.State.Facts, Does.Contain(ProgressionOperations.LedgerPrefix + contract.Completed));
            world.Close();
            yield return WorldExplorationRoute.Use(flow, FrontierChapter.Merchant, NoCapture, "Merchant");
            world.AlternativeButton.onClick.Invoke();
            world.ConfirmButton.onClick.Invoke();
            Assert.That(game.State.Gold, Is.EqualTo(beforeGold + contract.Gold - WorldOperations.RationPrice));
            world.Close();
            Assert.That(flow.ReturnToTitle() && flow.Continue(), Is.True);
            Assert.That(game.State.Gold, Is.EqualTo(beforeGold + contract.Gold - WorldOperations.RationPrice));
            Assert.That(FrontierContracts.Status(game.State, contract), Is.EqualTo(ContractStatus.Completed));
            Assert.That(FrontierContracts.Claim(game.State, contract), Is.False);
        }

        private static IEnumerator NoCapture(string _) { yield break; }

        [UnityTearDown]
        public IEnumerator Exit()
        {
            if (EditorApplication.isPlaying) yield return new ExitPlayMode();
        }
    }
}
