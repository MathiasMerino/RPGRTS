using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Aethernia
{
    public static class WorldExplorationRoute
    {
        private static void Check(bool result,string name)=>UxExperienceRoute.Require(result,"UX02 "+name);
        public static IEnumerator MoveTo(DemoFlow flow,Vector3 destination)
        {
            Check(flow.Director.Hero.TryMoveTo(destination),"walkable target "+destination);
            yield return UxExperienceRoute.Arrive(flow.Director.Hero);
            Check(Vector3.Distance(flow.Director.Hero.transform.position,destination)<.7f,"actual arrival "+destination);
        }
        public static IEnumerator Use(DemoFlow flow,string id,Func<string,IEnumerator> capture,string frame)
        {
            var point=UnityEngine.Object.FindObjectsByType<InteractionPoint>().Single(p=>p.FactId==id);
            yield return MoveTo(flow,point.transform.position+Vector3.back*1.1f);
            var commands=UnityEngine.Object.FindAnyObjectByType<PlayerCommands>();
            Check(commands.RefreshNearby()==point,"focused E target "+id);
            yield return UxExperienceRoute.Wait(.2f);yield return capture(frame+"-Prompt");
            Check(commands.Interact(),"real interaction "+id);yield return null;
        }
        public static IEnumerator Explore(DemoFlow flow,Func<string,IEnumerator> capture)
        {
            var s=flow.Session;var world=flow.GetComponent<WorldExploration>();var beforeGold=s.State.Gold;
            var activities=UnityEngine.Object.FindObjectsByType<VillageNpcActivity>();Check(activities.All(a=>a.Actor.Agent.isOnNavMesh),"all village agents start on real navigation");
            var guard=activities.Single(a=>a.Route.Length>1);var start=guard.Actor.transform.position;var until=Time.time+6;var hardLimit=Time.realtimeSinceStartup+30;
            while(Vector3.Distance(start,guard.Actor.transform.position)<.2f&&Time.time<until&&Time.realtimeSinceStartup<hardLimit)yield return null;
            Check(Vector3.Distance(start,guard.Actor.transform.position)>=.2f,"guard actually patrols existing route");
            var animation=guard.Actor.GetComponentInChildren<HumanoidAnimationDriver>();
            Check(animation!=null&&animation.CanAnimate&&animation.IsMoving&&animation.Rig.Animator.GetInteger("Locomotion")==1,"guard uses existing Walk driven by live movement");
            foreach(var waypoint in world.RoutePoints.Take(5))
            {yield return MoveTo(flow,waypoint.position);yield return UxExperienceRoute.Wait(.3f);if(waypoint==world.RoutePoints[1])yield return capture("W01-ForestFork");}
            Check(!flow.Director.IsBusy,"walking does not start encounters");
            yield return Use(flow,"ux02-crossroads",capture,"W02-Sign");Check(world.DialogueOpen&&s.WorldUiOpen,"sign information modal");
            var pos=flow.Director.Hero.transform.position;var size=flow.CameraRig.Size;Check(!flow.GetComponent<DemoHeroMotor>().ApplyInput(Vector2.up,.1f,false),"world dialogue blocks direct motor");flow.CameraRig.ApplyInput(Vector2.right,1,.1f,false);Check(flow.CameraRig.Size==size&&!UnityEngine.Object.FindAnyObjectByType<PlayerCommands>().Interact(),"modal blocks camera and repeated E");Check(flow.Director.Hero.transform.position==pos,"hero stationary in dialogue");yield return capture("W03-ReadableSign");world.CloseButton.onClick.Invoke();
            yield return Use(flow,"ux02-cache",capture,"W04-SideCache");Check(s.State.Gold==beforeGold+8&&s.HasFact("ux02-cache"),"real unique gold reward");
            var cache=world.Interactions.Single(i=>i.Kind==WorldInteractionKind.Chest).GetComponent<InteractionPoint>();Check(!cache.TryInteract(flow.Director.Hero,s)&&s.State.Gold==beforeGold+8,"no duplicated chest reward");
            var wood=s.State.Wood;yield return Use(flow,"ux02-branches",capture,"W05-Materials");Check(s.State.Wood>=wood+3&&s.HasFact("ux02-branches"),"shared construction resource");
            yield return MoveTo(flow,new Vector3(-16,0,-17.2f));yield return UxExperienceRoute.Wait(.3f);yield return capture("W06-RuinsEntrance");
            var entry=UnityEngine.Object.FindObjectsByType<InteractionPoint>().Single(p=>p.FactId=="umbral-entry");Check(!entry.TryInteract(flow.Director.Hero,s),"dungeon progression respected before camp");Check(!flow.Director.IsBusy,"ruins discovery does not auto fight");
            Check(s.HasFact("ux02-discovery-0")&&s.HasFact("ux02-discovery-1"),"locations discovered once");
        }
        public static IEnumerator CampAndRuins(DemoFlow flow,Func<string,IEnumerator> capture)
        {
            var s=flow.Session;var world=flow.GetComponent<WorldExploration>();
            var campaign=s.Rules.FrontierCampaign;
            if(campaign)
            {
                var settlement=flow.GetComponent<DemoSettlementPanel>();
                var workshop=s.State.Buildings.FindIndex(b=>b.Id=="workshop");
                if(workshop>=0&&!s.State.Buildings[workshop].ProductionPaused)
                    Check(settlement.SelectBuilding(workshop)&&settlement.ToggleProduction(),"pause workshop before recruiting");
            }
            yield return Use(flow,"ux02-door",capture,"W07-StoreDoor");Check(world.DialogueOpen,"door request dialogue");var food=s.State.Food;world.ConfirmButton.onClick.Invoke();Check(s.State.Food==food+2,"store delivery real food");
            yield return Use(flow,"ux02-merchant",capture,"W08-Merchant");yield return capture("W09-TradeDialogue");var gold=s.State.Gold;var wood=s.State.Wood;world.ConfirmButton.onClick.Invoke();
            Check(s.State.Gold==gold-5&&s.State.Wood>=wood+2,"real existing trade cost");
            if(s.Rules.LivingWorld)world.Close();
            yield return Use(flow,"ux02-store",capture,"W10-SharedStorage");Check(world.DialogueOpen,"storage reports actual world resources");world.Close();
            yield return Use(flow,"ux02-recruiter",capture,"W11-Recruiter");var party=UnityEngine.Object.FindAnyObjectByType<PartyWorld>();gold=s.State.Gold;
            world.ConfirmButton.onClick.Invoke();Check(s.State.Companions.Count==1&&s.State.Gold==gold-party.Rules.GoldCost("swordsman"),"real paid companion recruitment");
            var companion=s.State.Companions.Single();Check(companion.InParty&&party.Units.ContainsKey(companion.Id),"recruit joins real formation by existing rule");
            Check(party.SetParty(companion.Id,false)&&!companion.InParty&&party.SetParty(companion.Id,true)&&companion.InParty,"existing companion reserve and formation toggles");yield return capture("W12-CompanionCamp");
            var dungeon=UnityEngine.Object.FindAnyObjectByType<DungeonExpedition>();
            if(campaign)yield return FrontierCampaignRoute.PrepareParty(flow,party,dungeon);
            yield return Use(flow,"umbral-entry",capture,"W13-RuinsAccess");
            Check(dungeon.Inside&&s.Mode==PrototypeMode.Exploration&&!flow.Director.IsBusy,"existing dungeon entrance without auto battle");yield return UxExperienceRoute.Wait(.5f);yield return capture("W14-InsideRuins");
            if(campaign)yield return FrontierCampaignRoute.ExploreRuins(flow,dungeon,capture);
            var before=s.State.Companions.Single(c=>c.Id==companion.Id).Health;
            yield return Use(flow,"umbral-exit",capture,"W15-RuinsExit");Check(!dungeon.Inside&&Vector3.Distance(flow.Director.Hero.transform.position,dungeon.SurfaceReturn)<.5f,"return to actual forest entrance");Check(s.State.Companions.Single(c=>c.Id==companion.Id).Health==before,"companion state preserved across travel");yield return capture("W16-ReturnedToTrail");
            if(campaign)yield return FrontierCampaignRoute.Debrief(flow,capture);
            Check(flow.Director.Save(flow.SlotPath),"save world discoveries and rewards");
            var facts=s.State.Facts.Where(f=>f.StartsWith("ux02-")).OrderBy(f=>f).ToArray();
            Check(flow.ReturnToTitle()&&flow.Continue(),"actual title Continue");flow.Pause();
            Check(facts.SequenceEqual(s.State.Facts.Where(f=>f.StartsWith("ux02-")).OrderBy(f=>f)),"world facts persisted");flow.Resume();
        }
        public static IEnumerator Run(DemoFlow flow,string slot,string cls,string race,Func<string,IEnumerator> capture,bool nativeInput=true,bool allowPatrolRetry=true)
        {
            Check(flow.Session.Rules.WorldExploration,"opt-in world scene");
            yield return UxExperienceRoute.Run(flow,slot,cls,race,name=>Hook(flow,name,capture,nativeInput),nativeInput,allowPatrolRetry);
        }
        private static IEnumerator Hook(DemoFlow flow,string name,Func<string,IEnumerator> capture,bool nativeInput)
        {
            yield return capture(name);
            if(name=="04-SafeVillage")yield return Explore(flow,capture);
            if(name=="14-QuestReward"&&flow.Session.Rules.FrontierCampaign)
                yield return FrontierCampaignRoute.ResolveRoad(flow,capture);
            if(nativeInput&&name=="16-Selection")yield return MouseOrders(flow,capture,false);
            if(nativeInput&&name=="18-Positioned")yield return MouseOrders(flow,capture,true);
            if(name=="22-Camp")
            {
                yield return CampAndRuins(flow,capture);
                if(flow.Session.Rules.LivingWorld)
                {
                    yield return LivingWorldRoute.Resolve(flow,capture);
                    yield break;
                }
                if(flow.Session.Rules.TacticalWarfare)yield return TacticalWarfareRoute.FromCamp(flow,capture);
                if(flow.Session.Rules.FrontierCampaign)yield return FrontierCampaignRoute.Finish(flow,capture);
            }
        }
        private static IEnumerator MouseFrame(Mouse mouse,Vector2 point,ushort buttons)
        {InputSystem.QueueStateEvent(mouse,new MouseState{position=point,buttons=buttons});yield return null;yield return null;}
        // Eventos de dispositivo sintético recorren Update/raycast/arrastre reales; no son prueba humana.
        private static IEnumerator MouseOrders(DemoFlow flow,Func<string,IEnumerator> capture,bool attack)
        {
            var camera=flow.CameraRig.GetComponent<Camera>();var last=camera.transform.position;var stable=0;var end=Time.realtimeSinceStartup+6;
            while(stable<3&&Time.realtimeSinceStartup<end){yield return null;stable=Vector3.Distance(camera.transform.position,last)<.015f?stable+1:0;last=camera.transform.position;}
            Check(stable>=3,"camera settled before projecting synthetic pointer");
            var mouse=InputSystem.AddDevice<Mouse>();var keyboard=InputSystem.AddDevice<Keyboard>();var old=InputSystem.settings.backgroundBehavior;
            try
            {
                InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
                var war=flow.Director.Rts;var units=new System.Collections.Generic.List<RtsUnitStatus>();war.CopyStatus(units);
                var allies=units.Where(u=>u.Team==0&&u.Health>0).ToArray();
                var a=(Vector2)camera.WorldToScreenPoint(allies[0].Unit.transform.position+Vector3.up*.9f);
                yield return MouseFrame(mouse,a,1);yield return MouseFrame(mouse,a,0);Check(war.SelectedAllies==1,"native mouse single selection");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.LeftShift));yield return null;
                var b=(Vector2)camera.WorldToScreenPoint(allies[1].Unit.transform.position+Vector3.up*.9f);
                yield return MouseFrame(mouse,b,1);yield return MouseFrame(mouse,b,0);Check(war.SelectedAllies==2,"native Shift additive selection");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
                if(!attack)
                {
                    var blockedAt=(Vector2)camera.WorldToScreenPoint(war.PreparationTarget+Vector3.back*3.5f);
                    if(UiInputGuard.HasUiAt(blockedAt)){var stamp=war.LastOrderAt;yield return MouseFrame(mouse,blockedAt,2);yield return MouseFrame(mouse,blockedAt,0);Check(war.LastOrderAt==stamp&&war.Preparing,"native UI panel correctly blocks world order");}
                    var before=allies[0].Unit.transform.position;var target=war.PreparationTarget+Vector3.forward*4;var at=(Vector2)camera.WorldToScreenPoint(target);
                    Check(!UiInputGuard.HasUiAt(at),"synthetic move target is visible ground outside UI");
                    yield return MouseFrame(mouse,at,2);yield return MouseFrame(mouse,at,0);end=Time.realtimeSinceStartup+4;
                    while(Vector3.Distance(allies[0].Unit.transform.position,before)<=.15f&&Time.realtimeSinceStartup<end)yield return null;
                    var hit=Physics.Raycast(camera.ScreenPointToRay(at),out var pointerHit,150)?pointerHit.collider.name+" "+pointerHit.point:"none";
                    Check(Vector3.Distance(allies[0].Unit.transform.position,before)>.15f&&war.Preparing,"native right mouse actual movement without hostility; moved="+Vector3.Distance(allies[0].Unit.transform.position,before)+" lastOrder="+war.LastOrderText+" selected="+war.SelectedAllies+" pointer="+at+" hit="+hit+" ui="+UiInputGuard.HasUiAt(at));yield return capture("W17-MouseMove");
                }
                else
                {
                    var pixels=allies.Select(u=>(Vector2)camera.WorldToScreenPoint(u.Unit.transform.position+Vector3.up*.9f)).ToArray();
                    var min=new Vector2(pixels.Min(v=>v.x)-20,pixels.Min(v=>v.y)-20);var max=new Vector2(pixels.Max(v=>v.x)+20,pixels.Max(v=>v.y)+20);
                    yield return MouseFrame(mouse,min,1);yield return MouseFrame(mouse,max,1);yield return MouseFrame(mouse,max,0);
                    Check(war.SelectedAllies==allies.Length,"native rectangle multiple selection");yield return capture("W18-MouseRectangle");
                    var enemy=units.First(u=>u.Team==1&&u.Health>0&&u.Role=="Arquero");var at=(Vector2)camera.WorldToScreenPoint(enemy.Unit.transform.position+Vector3.up*.9f);
                    yield return MouseFrame(mouse,at,2);yield return MouseFrame(mouse,at,0);Check(!war.Preparing&&war.LastOrderText.Contains("Atacar"),"native right mouse attack engages real battle");yield return capture("W19-MouseAttack");
                }
            }
            finally{InputSystem.RemoveDevice(mouse);InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=old;}
        }
    }
}
