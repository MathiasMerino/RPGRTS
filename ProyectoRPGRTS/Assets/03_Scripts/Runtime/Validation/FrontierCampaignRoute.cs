using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    internal static class FrontierCampaignRoute
    {
        private static void Check(bool valid,string message)
        {
            if(!valid)throw new InvalidOperationException("E21: "+message);
        }

        public static IEnumerator ResolveRoad(DemoFlow flow,Func<string,IEnumerator> capture)
        {
            var world=flow.GetComponent<WorldExploration>();
            var state=flow.Session.State;
            Check(FrontierChapter.Step(state)==FrontierStep.Trail&&!flow.Director.IsBusy,"cargo opens route investigation");
            Check(!flow.BeginPatrol(),"patrol waits for the route decision");
            yield return WorldExplorationRoute.Use(flow,FrontierChapter.Sign,capture,"C01-Trail");
            Check(FrontierChapter.Step(state)==FrontierStep.Trail,"reading dialogue alone grants no clue");
            world.ConfirmButton.onClick.Invoke();
            Check(FrontierChapter.Step(state)==FrontierStep.Supply,"recorded clue opens the choice");
            yield return WorldExplorationRoute.Use(flow,FrontierChapter.Merchant,capture,"C02-Merchant");
            Check(world.AlternativeButton.gameObject.activeInHierarchy,"two choices in the same live dialogue");
            yield return capture("C03-Choice");
            var food=state.Food;
            var gold=state.Gold;
            var wood=state.Wood;
            var reputation=state.ReputationA;
            var repair=state.ClassId=="mage";
            (repair?world.AlternativeButton:world.ConfirmButton).onClick.Invoke();
            Check(FrontierChapter.Decision(state)==(repair?RoadChoice.Repair:RoadChoice.Report),"selected result persists");
            Check(repair?state.Gold==gold-5&&state.Wood==wood+6&&state.ReputationA==reputation:
                state.Food==food+3&&state.ReputationA==reputation+5&&state.Gold==gold,"distinct resource and reputation consequences");
            Check(FrontierChapter.Step(state)==FrontierStep.Patrol,"real decision opens the patrol");
            Check(!FrontierChapter.ChooseRoad(state,repair?RoadChoice.Report:RoadChoice.Repair),"choice is exclusive and one-time");
            flow.ExistingHud.SelectPage(ExplorationPage.Overview);
            yield return null;
            yield return capture("C04-Journal");
            flow.ExistingHud.ClosePanels();
        }

        public static IEnumerator PrepareParty(DemoFlow flow,PartyWorld party,DungeonExpedition dungeon)
        {
            foreach(var role in new[]{"archer","brawler"})
                Check(party.Recruit(role),"paid support for the validation route "+role);
            yield return WorldExplorationRoute.MoveTo(flow,new Vector3(9,0,-10));
            for(var i=0;i<8&&flow.Session.State.Health<flow.Session.HeroProfile.Health;i++)
            {
                Check(dungeon.Rest(),"camp rest");
                while(dungeon.Resting)yield return null;
            }
        }

        public static IEnumerator ExploreRuins(DemoFlow flow,DungeonExpedition dungeon,Func<string,IEnumerator> capture)
        {
            var session=flow.Session;
            dungeon.TestSeed=42;
            var start=session.State.Gold;
            for(var i=1;i<=2;i++)
            {
                yield return WorldExplorationRoute.Use(flow,"umbral-battle-"+i,capture,"C05-Guardian-"+i);
                Check(flow.Director.Encounter.GroupActive,"real guardian encounter "+i);
                yield return capture("C06-Group-"+i);
                yield return ExpeditionSmokeProbe.FightGroup(flow);
                Check(session.HasFact("umbral-battle-"+i),"guardian defeat recorded "+i);
            }
            yield return WorldExplorationRoute.Use(flow,"umbral-objective",capture,"C07-Relic");
            Check(FrontierChapter.Step(session.State)==FrontierStep.Debrief&&session.State.Gold==start+dungeon.Rules.DungeonGold,"one real expedition reward");
            var point=UnityEngine.Object.FindObjectsByType<InteractionPoint>().Single(p=>p.FactId=="umbral-objective");
            Check(!point.TryInteract(flow.Director.Hero,session)&&session.State.Gold==start+dungeon.Rules.DungeonGold,"relic reward cannot repeat");
        }

        public static IEnumerator Debrief(DemoFlow flow,Func<string,IEnumerator> capture)
        {
            var world=flow.GetComponent<WorldExploration>();
            yield return WorldExplorationRoute.Use(flow,FrontierChapter.Liaison,capture,"C08-Debrief");
            Check(FrontierChapter.Step(flow.Session.State)==FrontierStep.Debrief,"reaching Tarek does not finish the objective");
            world.ConfirmButton.onClick.Invoke();
            Check(FrontierChapter.Step(flow.Session.State)==FrontierStep.Conflict,"the delivered dossier unlocks the conflict");
        }

        public static IEnumerator Finish(DemoFlow flow,Func<string,IEnumerator> capture)
        {
            var world=flow.GetComponent<WorldExploration>();
            Check(FrontierChapter.Step(flow.Session.State)==FrontierStep.Return,"real territorial victory completes the conflict");
            yield return WorldExplorationRoute.Use(flow,FrontierChapter.Liaison,capture,"C09-Return");
            world.ConfirmButton.onClick.Invoke();
            Check(FrontierChapter.Step(flow.Session.State)==FrontierStep.Free,"chapter completes through a valid conversation");
            flow.ExistingHud.SelectPage(ExplorationPage.Overview);
            yield return null;
            yield return capture("C10-NewPaths");
            flow.ExistingHud.ClosePanels();
        }
    }
}
