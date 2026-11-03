using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Aethernia
{
    // Recorrido explícito compartido por QA de editor y player. Sólo usa acciones públicas reales.
    public static class UxExperienceRoute
    {
        public static void Require(bool ok,string step){if(!ok)throw new InvalidOperationException("UX01: "+step);}
        public static IEnumerator Wait(float seconds){var end=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<end)yield return null;}
        public static IEnumerator Arrive(ExplorationUnit unit)
        {yield return null;var end=Time.realtimeSinceStartup+40;while(unit.IsMoving&&Time.realtimeSinceStartup<end)yield return null;Require(!unit.IsMoving,"real navigation arrival");}
        public static IEnumerator Run(DemoFlow flow,string slot,string cls,string race,Func<string,IEnumerator> capture)
        {
            var d=flow.Director;var session=flow.Session;var hero=d.Hero;var cargo=UnityEngine.Object.FindAnyObjectByType<LostCargoController>();var questView=cargo.GetComponent<LostCargoView>();var ux=flow.GetComponent<UxExperienceView>();var input=UnityEngine.Object.FindAnyObjectByType<PlayerCommands>();
            Require(flow.UxEnabled&&flow.Front.Screen==DemoScreen.Main,"main menu first");yield return capture("01-Menu");
            flow.Front.Controls["new"].onClick.Invoke();flow.Front.Controls["race-"+race].onClick.Invoke();flow.Front.Controls["class-"+cls].onClick.Invoke();flow.Front.NameInput.text="Viajero "+cls;yield return capture("02-Creation");
            flow.Front.Controls["begin"].onClick.Invoke();yield return capture("03-Intro");flow.Front.Controls["skip"].onClick.Invoke();yield return Wait(.7f);
            Require(session.Mode==PrototypeMode.Exploration&&!d.IsBusy&&d.Encounter.Battle==null,"safe exploration without automatic RPG");
            Require(session.State.Demo.UxRevision==1&&session.State.RaceId==race&&session.State.ClassId==cls,"same race and class");
            Require(flow.CameraRig.GetComponent<Camera>().orthographicSize<7,"closer exploration camera");
            var model=hero.GetComponent<CharacterVisualPresenter>().Model;
            yield return capture("04-SafeVillage");yield return capture("05-CloserHero");
            var keyboard=InputSystem.AddDevice<Keyboard>();var background=InputSystem.settings.backgroundBehavior;
            try
            {
                InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;EventSystem.current.SetSelectedGameObject(null);
                var before=hero.transform.position;InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));yield return Wait(.45f);InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;
                Require(Vector3.Distance(before,hero.transform.position)>.2f,"WASD actual displacement; distance="+Vector3.Distance(before,hero.transform.position)+" read="+flow.GetComponent<DemoHeroMotor>().LastReadInput+" blocked="+flow.GetComponent<DemoHeroMotor>().LastBlocked+" can="+flow.GetComponent<DemoHeroMotor>().CanControl+" start="+before+" end="+hero.transform.position);yield return capture("06-Wasd");
                Require(hero.TryMoveTo(cargo.Npc.transform.position+Vector3.back*1.1f),"NPC reachable");yield return Arrive(hero);yield return Wait(.4f);
                Require(input.RefreshNearby()==cargo.Npc,"best nearby interaction is resident");yield return capture("07-NpcPrompt");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
                Require(questView.DialogueOpen,"actual E opens resident dialogue");yield return capture("08-Dialogue");questView.AcceptButton.onClick.Invoke();
                Require(LostCargoQuest.Stage(session.State)==CargoQuestStage.Accepted,"quest accepted via UI");yield return Wait(.3f);yield return capture("09-Objective");
                Require(ux.ObjectivePosition==cargo.Cargo.transform.position,"objective points to cargo instead of spawn");
                flow.ExistingHud.CompactView.ToggleHelp();yield return capture("10-Help");flow.ExistingHud.CompactView.Close();
                Require(hero.TryMoveTo(cargo.Cargo.transform.position+Vector3.back*1.6f),"cargo reachable");yield return Arrive(hero);cargo.EncounterSeed=42;
                Require(input.RefreshNearby()==cargo.Cargo,"cargo prompt only after acceptance");InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            }
            finally{InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=background;}
            Require(session.Mode==PrototypeMode.TurnCombat&&cargo.CargoEncounter,"quest triggers real RPG");
            Require(d.Encounter.TrySelect(d.Encounter.Enemy.GetComponentInChildren<Collider>()),"actual target selection");d.Encounter.Approach();yield return Arrive(hero);yield return capture("11-FirstRpg");
            for(var turn=0;turn<35&&cargo.CargoEncounter;turn++)
            {
                var battle=d.Encounter.Battle;Require(battle!=null,"RPG state");
                if(battle.Phase==TurnPhase.Enemy){yield return Wait(.2f);turn--;continue;}
                if(battle.HeroHealth<8&&!battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                if(turn==0)yield return capture("12-CompactRpg");
                if(cargo.CargoEncounter&&d.Encounter.Battle!=null&&!d.Encounter.Battle.Finished){Require(d.Execute("end"),"legal end turn");yield return Wait(.3f);}
            }
            Require(LostCargoQuest.Stage(session.State)==CargoQuestStage.Recovered&&session.Mode==PrototypeMode.Exploration,"real RPG victory returns to exploration");yield return capture("13-ExploreAgain");
            Require(hero.TryMoveTo(cargo.Npc.transform.position+Vector3.back*1.1f),"return to resident");yield return Arrive(hero);Require(input.Interact(),"resident reward dialogue");questView.ClaimButton.onClick.Invoke();questView.CloseDialogue();
            Require(LostCargoQuest.Stage(session.State)==CargoQuestStage.Completed&&!cargo.Claim(),"unique reward");yield return capture("14-QuestReward");
            if(session.State.Health<session.HeroProfile.Health)d.Execute("ration");
            flow.ExistingHud.SelectPage(ExplorationPage.Character);yield return null;var progression=flow.GetComponent<ProgressionPanelView>();if(progression.Discipline.interactable)progression.Discipline.onClick.Invoke();flow.ExistingHud.ClosePanels();
            Require(hero.TryMoveTo(flow.Patrol.transform.position+Vector3.back*1.1f),"connected path to western camp");yield return Arrive(hero);yield return capture("15-FriendlyCamp");
            var hp=session.State.Health;Require(input.RefreshNearby()==flow.Patrol&&input.Interact(),"captain E interaction");var war=d.Rts;Require(war.Preparing&&war.IncludesHero&&session.State.Health==hp,"safe RTS preparation same health");
            var units=new List<RtsUnitStatus>();war.CopyStatus(units);Require(units.All(u=>u.Team==0),"hostiles not revealed before orders");var ally=units.First(u=>!u.Hero).Unit;
            Require(war.SelectUnit(ally.GetComponentInChildren<Collider>(),false),"single selection");yield return Wait(.3f);Require(war.PreparationStep==1,"single selection tutorial checkpoint");yield return capture("16-Selection");
            war.SelectAll();Require(war.SelectedAllies==units.Count,"multiple selection");var target=war.PreparationTarget;Require(war.Command(target,null,false),"first move order");yield return Wait(.4f);yield return capture("17-Move");
            var deadline=Time.realtimeSinceStartup+35;while(war.PreparationStep<2&&Time.realtimeSinceStartup<deadline)yield return null;Require(war.PreparationStep==2,"actual first waypoint arrival");
            Require(session.State.Health==hp&&war.Preparing,"no damage while preparing");
            Require(d.Save(slot),"preparation checkpoint saved");Require(PrototypeSaveStore.Load(slot,session.Rules,out var checkpoint)&&checkpoint.Demo.UxFormation.Length==units.Count,"actual troop positions persisted");
            target=war.PreparationTarget;Require(war.Command(target,null,false),"regroup move order");deadline=Time.realtimeSinceStartup+35;while(war.PreparationStep<3&&Time.realtimeSinceStartup<deadline)yield return null;
            Require(war.PreparationStep==3&&war.Preparing,"real regroup before hostility");yield return capture("18-Positioned");war.CopyStatus(units);var enemy=units.First(u=>u.Team==1).Unit;
            Require(war.Command(enemy.transform.position,enemy.GetComponentInChildren<Collider>(),false)&&!war.Preparing,"explicit player attack starts hostility");yield return Wait(.5f);yield return capture("19-Attack");
            deadline=Time.realtimeSinceStartup+100;while(war.Active&&Time.realtimeSinceStartup<deadline){if(session.State.Health<session.HeroProfile.Health)war.HeroAction("recover");war.HeroAction("technique");yield return null;}
            Require(war.LastResolvedVictory==true&&session.State.Demo.Chapter==DemoChapter.Settlement,"real RTS victory");Require(hero.GetComponent<CharacterVisualPresenter>().Model==model,"same hero model across modes");yield return capture("20-Victory");
            var camp=flow.GetComponent<DemoSettlementPanel>();Require(camp.SelectBlueprint("townhall")&&d.Construction.Place(new Vector3(12,0,-8)),"paid camp center");
            Require(camp.SelectBlueprint("workshop")&&d.Construction.Place(new Vector3(8,0,-13)),"paid workshop");yield return null;Require(camp.SelectBuilding(1)&&camp.ToggleProduction(),"real workshop production");yield return capture("21-Building");
            deadline=Time.realtimeSinceStartup+16;while(session.State.Demo.Chapter!=DemoChapter.FreeExploration&&Time.realtimeSinceStartup<deadline)yield return null;Require(session.State.Demo.Produced>0,"actual production");
            Require(camp.SelectBlueprint("barracks")&&d.Construction.Place(new Vector3(15,0,-13)),"paid barracks");Require(camp.Recruit("archer"),"paid military recruit");d.Execute("explore");yield return Wait(.6f);yield return capture("22-Camp");
            Require(d.Save(slot),"final save");File.WriteAllText(slot+".identity.json",JsonUtility.ToJson(session.State,true));Require(flow.ReturnToTitle(),"save and title");
        }
    }
}
