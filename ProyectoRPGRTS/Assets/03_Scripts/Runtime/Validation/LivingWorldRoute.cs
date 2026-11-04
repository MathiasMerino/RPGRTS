using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    internal static class LivingWorldRoute
    {
        private static void Check(bool valid, string step)
        {
            if (!valid) throw new InvalidOperationException("E22: " + step);
        }

        public static IEnumerator Resolve(DemoFlow flow, Func<string, IEnumerator> capture)
        {
            var game = flow.Session;
            var state = game.State;
            var world = flow.GetComponent<WorldExploration>();
            if (state.ClassId == "warrior")
            {
                yield return TacticalWarfareRoute.FromCamp(flow, capture);
                yield return FrontierCampaignRoute.Finish(flow, capture);
            }
            else
            {
                yield return TacticalWarfareRoute.PrepareArmy(flow);
                if (state.ClassId == "mage")
                {
                    var panel = UnityEngine.Object.FindAnyObjectByType<ExpeditionPanel>();
                    var gold = state.Gold;
                    var food = state.Food;
                    var catalog = game.Rules.Civilizations;
                    panel.Open();
                    panel.Controls["culture-tab"].onClick.Invoke();
                    yield return null;
                    Check(panel.Controls["culture-treaty"].interactable, "opción de pacto accesible");
                    panel.Controls["culture-treaty"].onClick.Invoke();
                    Check(state.Gold == gold - catalog.TreatyGold && state.Food >= food - catalog.TreatyFood &&
                        state.Facts.Contains(CivilizationOperations.Pact) && CivilizationOperations.Relation(state, "faction-b") == FactionRelation.Ally,
                        "pacto pagado con efecto diplomático");
                    yield return capture("L01-Treaty");
                    panel.Close();
                    yield return FrontierCampaignRoute.Finish(flow, capture);
                }
                else
                {
                    yield return WorldExplorationRoute.Use(flow, FrontierChapter.Liaison, capture, "L01-Independent");
                    world.ConfirmButton.onClick.Invoke();
                    Check(state.Facts.Contains(FrontierChapter.Independent) && FrontierChapter.Step(state) == FrontierStep.Free,
                        "cierre independiente por Tarek");
                }
                Check(!flow.Director.Rts.Active && state.Territories.Single(t => t.Id == "outpost").Owner == "faction-b",
                    "el cierre conserva el propietario sin conquista");
            }

            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Board, capture, "L02-Board");
            yield return capture("L03-ContractCatalog");
            for (var i = 0; i < FrontierContracts.Count; i++)
            {
                var contract = FrontierContracts.At(i);
                Check(FrontierContracts.Status(state, contract) == ContractStatus.Available, "consultar no acepta " + contract.Id);
                var gold = state.Gold;
                world.ConfirmButton.onClick.Invoke();
                Check(FrontierContracts.Status(state, contract) == ContractStatus.Accepted && state.Gold == gold, "aceptación sin pago " + contract.Id);
                if (i + 1 < FrontierContracts.Count) world.AlternativeButton.onClick.Invoke();
            }
            world.Close();
            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Cairn, capture, "L04-Cairn");
            world.ConfirmButton.onClick.Invoke();
            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Materials, capture, "L05-Reserve");
            world.ConfirmButton.onClick.Invoke();

            var dungeon = UnityEngine.Object.FindAnyObjectByType<DungeonExpedition>();
            var party = UnityEngine.Object.FindAnyObjectByType<PartyWorld>();
            yield return WorldExplorationRoute.MoveTo(flow, new Vector3(9, 0, -10));
            var companions = state.ClassId == "warrior" ? 0 : state.ClassId == "mage" ? 1 : 2;
            for (var i = companions; i < state.Companions.Count; i++)
                Check(party.SetParty(state.Companions[i].Id, false), "grupo opcional de " + companions);
            for (var attempt = 0; attempt < 8 && state.Health < game.HeroProfile.Health; attempt++)
            {
                Check(dungeon.Rest(), "descanso antes de la última guardia");
                while (dungeon.Resting) yield return null;
            }
            yield return WorldExplorationRoute.Use(flow, "umbral-entry", capture, "L06-OptionalEntry");
            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Watch, capture, "L07-OptionalEncounter");
            var group = flow.Director.Encounter.Group;
            Check(group != null && group.Members.Count(m => !m.Hostile) == companions + 1 &&
                group.Members.Count(m => m.Hostile) == 1, "centinela y participantes reales");
            if(game.Rules.IntegratedAlpha)
            {
                var controls=UnityEngine.Object.FindAnyObjectByType<ExpeditionPanel>().Controls;
                Check(!controls["target-1"].gameObject.activeSelf&&controls["target-0"].GetComponentInChildren<TMPro.TMP_Text>().text=="Centinela",
                    "panel de centinela muestra un solo rival real");
            }
            yield return capture("L08-Sentry");
            yield return ExpeditionSmokeProbe.FightGroup(flow);
            Check(state.Facts.Contains(FrontierContracts.Watch), "victoria registrada por el combate");
            yield return WorldExplorationRoute.Use(flow, "umbral-exit", capture, "L09-Return");

            yield return WorldExplorationRoute.Use(flow, FrontierContracts.Board, capture, "L10-BoardReturn");
            for (var i = 0; i < FrontierContracts.Count; i++)
            {
                var contract = FrontierContracts.At(i);
                var gold = state.Gold;
                Check(FrontierContracts.Status(state, contract) == ContractStatus.Ready, "objetivo cumplido " + contract.Id);
                world.ConfirmButton.onClick.Invoke();
                Check(state.Gold == gold + contract.Gold && FrontierContracts.Status(state, contract) == ContractStatus.Completed &&
                    !world.ConfirmButton.gameObject.activeSelf, "cobro único " + contract.Id);
                if (i + 1 < FrontierContracts.Count) world.AlternativeButton.onClick.Invoke();
            }
            yield return capture("L11-Delivered");
            world.Close();

            yield return WorldExplorationRoute.Use(flow, FrontierChapter.Merchant, capture, "L12-Merchant");
            world.AlternativeButton.onClick.Invoke();
            var price = state.Gold;
            var rations = state.Items.Find(i => i.Id == "ration")?.Count ?? 0;
            world.ConfirmButton.onClick.Invoke();
            Check(state.Gold == price - WorldOperations.RationPrice && state.Items.Find(i => i.Id == "ration")?.Count == rations + 1,
                "compra de objeto con inventario real");
            yield return capture("L13-BuyRation");
            world.AlternativeButton.onClick.Invoke();
            price = state.Gold;
            world.ConfirmButton.onClick.Invoke();
            Check(state.Gold == price + ItemCatalog.Find("ration", game.Rules.Progression.Items).SellGold &&
                (state.Items.Find(i => i.Id == "ration")?.Count ?? 0) == rations, "venta retira objeto y paga oro");
            yield return capture("L14-SellRation");
            world.Close();
            if(game.Rules.IntegratedAlpha)
            {
                flow.ExistingHud.SelectPage(ExplorationPage.Inventory);
                yield return null;
                yield return capture("A01-Inventory");
                flow.ExistingHud.SelectPage(ExplorationPage.Character);
                yield return null;
                yield return capture("A02-Progression");
                flow.ExistingHud.ClosePanels();
            }

            yield return WorldExplorationRoute.MoveTo(flow, new Vector3(9, 0, -10));
            var camp = flow.GetComponent<DemoSettlementPanel>();
            camp.Open();
            yield return null;
            var rules = game.Settlement;
            var farm = state.Buildings.FindIndex(b => b.Id == "farm");
            var beforeGold = state.Gold;
            var beforeWood = state.Wood;
            Check(camp.SelectBuilding(farm) && camp.Improve() && state.Buildings[farm].Level == 2 &&
                state.Gold == beforeGold - rules.UpgradeGold && state.Wood == beforeWood - rules.UpgradeWood,
                "mejora agrícola real y pagada");
            var beforeFood = state.Food;
            var cycle = state.ProductionCycle;
            var deadline = Time.realtimeSinceStartup + 25;
            while (state.ProductionCycle == cycle && Time.realtimeSinceStartup < deadline) yield return null;
            Check(state.Food >= beforeFood + rules.Find("farm").FoodPerCycle * 2, "producción de nivel 2 en el reloj real");
            yield return capture("L15-UpgradedFarm");
            Check(flow.Director.Execute("explore"), "regreso a exploración después de mejorar");
        }
    }
}
