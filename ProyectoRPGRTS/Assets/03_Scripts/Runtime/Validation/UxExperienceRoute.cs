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
        public static IEnumerator Run(DemoFlow flow,string slot,string cls,string race,Func<string,IEnumerator> capture,bool nativeInput=true,bool allowPatrolRetry=true)
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
                var before=hero.transform.position;
                if(nativeInput){InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));yield return Wait(.45f);InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;}
                else {for(var frame=0;frame<12;frame++){flow.GetComponent<DemoHeroMotor>().ApplyInput(Vector2.up,.02f,false);yield return null;}}
                Require(Vector3.Distance(before,hero.transform.position)>.2f,"WASD actual displacement; distance="+Vector3.Distance(before,hero.transform.position)+" read="+flow.GetComponent<DemoHeroMotor>().LastReadInput+" blocked="+flow.GetComponent<DemoHeroMotor>().LastBlocked+" can="+flow.GetComponent<DemoHeroMotor>().CanControl+" start="+before+" end="+hero.transform.position);yield return capture("06-Wasd");
                Require(hero.TryMoveTo(cargo.Npc.transform.position+Vector3.back*1.1f),"NPC reachable");yield return Arrive(hero);yield return Wait(.4f);
                Require(input.RefreshNearby()==cargo.Npc,"best nearby interaction is resident");yield return capture("07-NpcPrompt");
                if(nativeInput){InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;}else{Require(input.Interact(),"existing interactor API");yield return null;}
                Require(questView.DialogueOpen,"actual E opens resident dialogue; presses="+input.InteractKeyPresses+" blocked="+input.LastInteractBlocked+" mouse="+(Mouse.current!=null)+" modal="+(input.ModalOpen?.Invoke()??false));yield return capture("08-Dialogue");questView.AcceptButton.onClick.Invoke();
                Require(LostCargoQuest.Stage(session.State)==CargoQuestStage.Accepted,"quest accepted via UI");yield return Wait(.3f);yield return capture("09-Objective");
                Require(ux.ObjectivePosition==cargo.Cargo.transform.position,"objective points to cargo instead of spawn");
                flow.ExistingHud.CompactView.ToggleHelp();yield return capture("10-Help");flow.ExistingHud.CompactView.Close();
                Require(hero.TryMoveTo(cargo.Cargo.transform.position+Vector3.back*1.6f),"cargo reachable");yield return Arrive(hero);cargo.EncounterSeed=42;
                Require(input.RefreshNearby()==cargo.Cargo,"cargo prompt only after acceptance");if(nativeInput){InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;}else{Require(input.Interact(),"existing interactor API");yield return null;}
            }
            finally{InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=background;}
            Require(session.Mode==PrototypeMode.TurnCombat&&cargo.CargoEncounter,"quest triggers real RPG");
            Require(d.Encounter.TrySelect(d.Encounter.Enemy.GetComponentInChildren<Collider>()),"actual target selection");if(!session.Rules.FrontierCampaign||cls!="mage")d.Encounter.Approach();yield return Arrive(hero);yield return capture("11-FirstRpg");
            var withdrawals=0;
            var duelDeadline=Time.realtimeSinceStartup+110;
            for(var turn=0;turn<35&&cargo.CargoEncounter;turn++)
            {
                var battle=d.Encounter.Battle;Require(battle!=null,"RPG state");
                if(battle.Phase==TurnPhase.Enemy)
                {
                    if(session.Rules.CoreRefine)Require(Time.realtimeSinceStartup<duelDeadline,"CORE enemy turn settles");
                    yield return Wait(.2f);turn--;continue;
                }
                if(session.Rules.CoreRefine)
                {
                    var tactical=flow.GetComponent<CoreTacticalHud>();
                    Require(tactical.Visible&&tactical.VisibleActors==2&&!flow.ExistingHud.FindControl("attack").gameObject.activeInHierarchy,"CORE solo uses one combat HUD");
                    if(cls!="mage"&&d.Encounter.CoreTargetStatus(false)!=null)
                    {
                        d.Encounter.Approach();
                        yield return Arrive(hero);
                    }
                }
                if(battle.HeroHealth<8&&!battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                if(turn==0)yield return capture("12-CompactRpg");
                if(session.Rules.LivingWorld&&cls=="mage"&&cargo.CargoEncounter&&battle.MovementRemaining>.5f)
                {
                    var away=hero.transform.position-d.Encounter.Enemy.transform.position;
                    away.y=0;
                    var distance=Mathf.Min(3.5f,battle.MovementRemaining-.2f);
                    foreach(var angle in new[]{0,35,-35,70,-70})
                    {
                        if(!d.Encounter.Move(hero.transform.position+Quaternion.Euler(0,angle,0)*away.normalized*distance))continue;
                        withdrawals++;
                        yield return Arrive(hero);
                        break;
                    }
                }
                if(cargo.CargoEncounter&&d.Encounter.Battle!=null&&!d.Encounter.Battle.Finished){Require(d.Execute("end"),"legal end turn");yield return Wait(.3f);}
            }
            Require(LostCargoQuest.Stage(session.State)==CargoQuestStage.Recovered&&session.Mode==PrototypeMode.Exploration,"real RPG victory returns to exploration");yield return capture("13-ExploreAgain");
            if(session.Rules.LivingWorld&&cls=="mage")
                File.WriteAllText(slot+".mage-positioning.txt","seed=42 withdrawals="+withdrawals+" hp="+session.State.Health+"/"+session.HeroProfile.Health);
            Require(hero.TryMoveTo(cargo.Npc.transform.position+Vector3.back*1.1f),"return to resident");yield return Arrive(hero);Require(input.Interact(),"resident reward dialogue");questView.ClaimButton.onClick.Invoke();questView.CloseDialogue();
            Require(LostCargoQuest.Stage(session.State)==CargoQuestStage.Completed&&!cargo.Claim(),"unique reward");yield return capture("14-QuestReward");
            while(session.State.Health<session.HeroProfile.Health&&session.State.Items.Any(i=>i.Id=="ration"&&i.Count>0))Require(d.Execute("ration"),"consume actual available ration before patrol");
            flow.ExistingHud.SelectPage(ExplorationPage.Character);yield return null;var progression=flow.GetComponent<ProgressionPanelView>();if(progression.Discipline.interactable)progression.Discipline.onClick.Invoke();flow.ExistingHud.ClosePanels();
            Require(hero.TryMoveTo(flow.Patrol.transform.position+Vector3.back*1.1f),"connected path to western camp");yield return Arrive(hero);yield return capture("15-FriendlyCamp");
            var hp=session.State.Health;Require(input.RefreshNearby()==flow.Patrol&&input.Interact(),"captain E interaction");
            var world=flow.GetComponent<WorldExploration>();if(world!=null&&world.DialogueOpen){yield return capture("15B-CaptainDialogue");world.ConfirmButton.onClick.Invoke();}
            var war=d.Rts;Require(war.Preparing&&war.IncludesHero&&session.State.Health==hp,"safe RTS preparation same health");
            var units=new List<RtsUnitStatus>();war.CopyStatus(units);Require(units.All(u=>u.Team==0),"hostiles not revealed before orders");var ally=units.First(u=>!u.Hero).Unit;
            Require(war.SelectUnit(ally.GetComponentInChildren<Collider>(),false),"single selection");yield return Wait(.3f);Require(war.PreparationStep==1,"single selection tutorial checkpoint");yield return capture("16-Selection");
            war.SelectAll();Require(war.SelectedAllies==units.Count,"multiple selection");var target=war.PreparationTarget;Require(war.Command(target,null,false),"first move order");yield return Wait(.4f);yield return capture("17-Move");
            if(session.Rules.CoreRefine)
                Require(!flow.GetComponent<CoreTacticalHud>().Visible&&flow.ExistingHud.FindControl("select-all").gameObject.activeInHierarchy,"CORE RTS keeps separate controls");
            var deadline=Time.realtimeSinceStartup+35;while(war.PreparationStep<2&&Time.realtimeSinceStartup<deadline)yield return null;Require(war.PreparationStep==2,"actual first waypoint arrival");
            Require(session.State.Health==hp&&war.Preparing,"no damage while preparing");
            Require(d.Save(slot),"preparation checkpoint saved");Require(PrototypeSaveStore.Load(slot,session.Rules,out var checkpoint)&&checkpoint.Demo.UxFormation.Length==units.Count,"actual troop positions persisted");
            Require(flow.ReturnToTitle()&&flow.Continue(),"actual preparation title/Continue restore");Require(war.Preparing&&war.PreparationStep==2&&session.State.Health==hp,"preparation mode and health restored");
            war.CopyStatus(units);Require(units.Count==checkpoint.Demo.UxFormation.Length,"no duplicated troops on restore");for(var i=0;i<units.Count;i++)Require(Vector3.Distance(units[i].Unit.transform.position,checkpoint.Demo.UxFormation[i])<.15f,"actual saved formation restored");
            target=war.PreparationTarget;Require(war.Command(target,null,false),"regroup move order");deadline=Time.realtimeSinceStartup+35;while(war.PreparationStep<3&&Time.realtimeSinceStartup<deadline)yield return null;
            Require(war.PreparationStep==3&&war.Preparing,"real regroup before hostility");
            var proof=new System.Text.StringBuilder();
            foreach(var unit in units){proof.AppendLine(unit.Role+" actor="+unit.Unit.transform.position);foreach(var renderer in unit.Unit.GetComponentsInChildren<Renderer>())
                {proof.AppendLine(renderer.name+" bounds="+renderer.bounds+" enabled="+renderer.enabled);if(renderer is SkinnedMeshRenderer skin){var baked=new Mesh();skin.BakeMesh(baked);var low=float.MaxValue;var high=float.MinValue;foreach(var v in baked.vertices){var y=skin.transform.TransformPoint(v).y;low=Mathf.Min(low,y);high=Mathf.Max(high,y);}proof.AppendLine("BAKED_Y "+low+".."+high);UnityEngine.Object.Destroy(baked);}}}
            File.WriteAllText(slot+".preparation.details",proof.ToString());yield return Wait(.3f);yield return capture("18-Positioned");war.CopyStatus(units);var enemy=units.First(u=>u.Team==1&&u.Role=="Arquero").Unit;
            Require(war.Command(enemy.transform.position,enemy.GetComponentInChildren<Collider>(),false)&&!war.Preparing,"explicit player attack starts hostility");yield return Wait(.5f);yield return capture("19-Attack");
            var attempts=new List<string>();
            var maximumAttempts=allowPatrolRetry?2:1;
            for(var attempt=0;attempt<maximumAttempts;attempt++)
            {
                var guarded=false;deadline=Time.realtimeSinceStartup+120;while(war.Active&&Time.realtimeSinceStartup<deadline)
            {if(!guarded)guarded=war.HeroAction("guard");if(session.State.Health<=session.HeroProfile.Health-4)war.HeroAction("recover");war.HeroAction("technique");war.HeroAction("attack");
             war.CopyStatus(units);if(enemy==null||!enemy.gameObject.activeSelf){enemy=units.Where(u=>u.Team==1&&u.Health>0).OrderBy(u=>u.Role=="Arquero"?0:1).Select(u=>u.Unit).FirstOrDefault();if(enemy!=null)war.Command(enemy.transform.position,enemy.GetComponentInChildren<Collider>(),false);}yield return null;}
                attempts.Add("attempt="+(attempt+1)+" victory="+war.LastResolvedVictory+" hp="+session.State.Health+" allies="+war.Allies+" enemies="+war.Enemies);
                if(war.LastResolvedVictory==true)break;
                Require(!war.Active,"battle timed out, no automatic retry");
                if(attempt+1<maximumAttempts)
                {
                    yield return capture("19B-ActualDefeat");Require(flow.Front.Screen==DemoScreen.Retry&&flow.Front.Controls["retry"].interactable,"explicit defeat retry screen");
                    flow.Front.Controls["retry"].onClick.Invoke();yield return Wait(.4f);Require(war.Preparing&&session.State.Health==session.HeroProfile.Health,"existing paid-resource state retained; retry restores class health by existing rule");
                    war.SelectAll();war.CopyStatus(units);enemy=units.First(u=>u.Team==1&&u.Role=="Arquero").Unit;Require(war.Command(enemy.transform.position,enemy.GetComponentInChildren<Collider>(),false),"player starts retry attack");
                }
            }
            File.WriteAllText(slot+".rts-attempts.details",string.Join("\n",attempts));
            Require(war.LastResolvedVictory==true&&session.State.Demo.Chapter==DemoChapter.Settlement,"real RTS victory; active="+war.Active+" outcome="+war.LastResolvedVictory+" hp="+session.State.Health+" allies="+war.Allies+" enemies="+war.Enemies);Require(hero.GetComponent<CharacterVisualPresenter>().Model==model,"same hero model across modes");yield return capture("20-Victory");
            if(session.Rules.CoreRefine)
            {
                flow.ExistingHud.FindControl("page-session").onClick.Invoke();
                var construct=flow.ExistingHud.FindControl("mode-construct");
                Require(construct.gameObject.activeInHierarchy&&construct.interactable,"CORE Construir is available in Menú");
                construct.onClick.Invoke();
                Require(session.Mode==PrototypeMode.Construction,"CORE construction via actual menu");
            }
            var camp=flow.GetComponent<DemoSettlementPanel>();Require(camp.SelectBlueprint("townhall")&&d.Construction.Place(new Vector3(12,0,-8)),"paid camp center");
            Require(camp.SelectBlueprint("workshop")&&d.Construction.Place(new Vector3(8,0,-13)),"paid workshop");yield return null;Require(camp.SelectBuilding(1)&&camp.ToggleProduction(),"real workshop production");yield return capture("21-Building");
            deadline=Time.realtimeSinceStartup+16;while(session.State.Demo.Chapter!=DemoChapter.FreeExploration&&Time.realtimeSinceStartup<deadline)yield return null;Require(session.State.Demo.Produced>0,"actual production");
            Require(camp.SelectBlueprint("barracks")&&d.Construction.Place(new Vector3(15,0,-13)),"paid barracks");Require(camp.Recruit("archer"),"paid military recruit");d.Execute("explore");yield return Wait(.6f);yield return capture("22-Camp");
            var previousZoom=flow.CameraRig.Size;flow.CameraRig.ApplyInput(Vector2.right,0,.1f,false);flow.CameraRig.FocusOn(Vector3.zero);flow.CameraRig.SetZoom(11);yield return Wait(.4f);yield return capture("24-WorldOverview");flow.CameraRig.SetZoom(previousZoom);flow.CameraRig.FocusOn(hero.transform.position);
            Require(d.Save(slot),"final save");File.WriteAllText(slot+".identity.json",JsonUtility.ToJson(session.State,true));Require(flow.ReturnToTitle(),"save and title");
        }
    }
}
