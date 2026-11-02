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
    // Solo se instala con un argumento explícito. No prepara recursos ni altera salud/dados.
    public sealed class ExpeditionSmokeProbe : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install()
        {
            var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"-aetherniaExpeditionSmoke")<0)return;
            var go=new GameObject("Explicit expedition validation");go.AddComponent<ExpeditionSmokeProbe>();
            if(Array.IndexOf(args,"-demoRecord")>=0)go.AddComponent<DemoRecordingProbe>();
        }
        private DemoFlow flow;private PartyWorld party;private DungeonExpedition dungeon;private TerritoryCampaign territory;private ExpeditionPanel panel;
        private string slot;private bool resume;private PrototypeDirector D=>flow.Director;
        private PrototypeState S=>flow.Session.State;
        private IEnumerator Start()
        {
            Application.runInBackground=true;var args=Environment.GetCommandLineArgs();slot=args[Array.IndexOf(args,"-aetherniaExpeditionSmoke")+1];resume=Array.IndexOf(args,"-demoResume")>=0;
            var stack=new Stack<IEnumerator>();stack.Push(Run());var started=Time.realtimeSinceStartup;
            while(stack.Count>0)
            {
                object step;try{var top=stack.Peek();if(!top.MoveNext()){stack.Pop();continue;}step=top.Current;if(step is IEnumerator nested){stack.Push(nested);continue;}}
                catch(Exception e){File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"FAIL: "+e);Debug.LogError(e);GetComponent<DemoRecordingProbe>()?.Finish();Application.Quit(2);yield break;}
                yield return step;
            }
            GetComponent<DemoRecordingProbe>()?.Finish();
            File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"PASS: "+(resume?"nuevo proceso conserva héroe, grupo, reserva, ejército, territorios, recursos y recompensas sin duplicar":"partida E13 real -> reclutamiento pagado -> seguimiento/WASD -> dos victorias RPG -> recompensa única -> granja/reclutas pagados -> selección/marcha -> conquista RTS real -> producción online -> guardado")+"; class="+S.ClassId+"; seconds="+(Time.realtimeSinceStartup-started).ToString("F2"));
            ClickFront("quit");
        }
        private IEnumerator Run()
        {
            yield return null;yield return null;flow=FindAnyObjectByType<DemoFlow>();party=FindAnyObjectByType<PartyWorld>();dungeon=FindAnyObjectByType<DungeonExpedition>();territory=FindAnyObjectByType<TerritoryCampaign>();panel=FindAnyObjectByType<ExpeditionPanel>();
            Require(flow!=null&&party!=null&&flow.Front.Screen==DemoScreen.Main,"main and expedition scene");Require(PrototypeSaveStore.Load(slot,flow.Session.Rules,out var before),"real E13/checkpoint input");
            ClickFront("continue");if(resume)flow.Pause();yield return null;Require(flow.IsPlaying&&flow.Front.Screen==DemoScreen.None&&S.Demo.IntroSeen,"continue bypasses intro");
            if(resume)
            {
                flow.Pause();Require(JsonUtility.ToJson(S)==JsonUtility.ToJson(before),"exact serialized state after process restart");
                Require(Vector3.Distance(D.Hero.transform.position,before.HeroPosition)<.1f,"hero position");flow.Resume();yield return null;
                Require(party.Units.Count==S.Companions.Count(c=>c.InParty&&c.Health>0),"no duplicate party instances");
                var gold=S.Gold;Require(!ExpeditionOperations.RewardDungeon(S,dungeon.Rules)&&S.Gold==gold,"reward cannot repeat");yield return Capture("Continued");
                Require(flow.ReturnToTitle(),"save and title");yield break;
            }
            Require(S.Companions.Count==0&&S.Army.Count==1&&S.Buildings.Count==3&&!S.Facts.Contains("umbral-reward"),"unaltered actual E13 baseline");
            dungeon.TestSeed=42;
            // El jugador puede pausar el taller para reservar alimento para compañeros.
            var settlement=flow.GetComponent<DemoSettlementPanel>();Require(settlement.SelectBuilding(1)&&settlement.ToggleProduction(),"pause existing workshop through camp action");
            yield return Move(new Vector3(-3.4f,0,-5));Require(Find("reference-chest").TryInteract(D.Hero,flow.Session),"actual village chest funds");
            yield return Move(new Vector3(9,0,-10));panel.Open();yield return null;
            foreach(var role in party.Roles.Roles){Click("companion-"+role.Id);yield return null;}
            Require(S.Companions.Count==3&&party.Units.Count==3,"three paid unique companions");yield return Capture("Recruitment");
            var first=S.Companions[0].Id;Click("toggle-party-0");yield return null;Require(!S.Companions[0].InParty&&party.Units.Count==2,"reserve keeps identity");Click("toggle-party-0");yield return null;Require(S.Companions[0].Id==first&&party.Units.Count==3,"rejoin same companion");panel.Close();
            yield return HealAtCamp();yield return Wasd();yield return Capture("Following");
            yield return Move(new Vector3(4,0,-12));yield return Capture("Entry");Require(Find("umbral-entry").TryInteract(D.Hero,flow.Session),"dungeon entry");yield return null;yield return Capture("Interior");
            Require(dungeon.Inside&&party.Units.Count==3,"same party transition");
            yield return Move(new Vector3(76,0,-2));yield return Wait(.6f);Require(Find("umbral-battle-1").TryInteract(D.Hero,flow.Session),"first actual encounter");yield return Capture("GroupCombat");yield return Fight();
            yield return Move(new Vector3(84,0,-2));yield return Wait(.6f);Require(Find("umbral-battle-2").TryInteract(D.Hero,flow.Session),"second actual encounter");yield return Fight();
            yield return Move(new Vector3(89,0,3));var goldBefore=S.Gold;Require(Find("umbral-objective").TryInteract(D.Hero,flow.Session)&&S.Gold==goldBefore+30,"real objective reward");Require(!Find("umbral-objective").TryInteract(D.Hero,flow.Session),"unique reward");yield return Capture("Reward");
            yield return Move(new Vector3(71,0,-4));Require(Find("umbral-exit").TryInteract(D.Hero,flow.Session),"exit");yield return Move(new Vector3(9,0,-10));yield return HealAtCamp();
            Require(settlement.SelectBlueprint("farm")&&D.Construction.Place(new Vector3(11.5f,0,-14)),"farm paid with expedition resources");Require(settlement.SelectBuilding(S.Buildings.Count-1)&&settlement.ToggleProduction(),"activate farm");
            foreach(var role in party.Roles.Roles)Require(settlement.Recruit(role.Id),"paid military recruit "+role.Id);D.Execute("explore");yield return null;
            panel.Open();Click("army-tab");yield return null;for(var i=0;i<S.Army.Count;i++){Click("army-"+i);yield return null;}
            Require(S.ArmySelection.Count==4&&S.Companions.Count==3,"separate persistent rosters");yield return Capture("Army");Click("territory-tab");yield return null;yield return Capture("TerritoriesBefore");panel.Close();
            Require(territory.March("crossing"),"actual march neutral");yield return Arrive(D.Hero);Require(Find("territory-crossing").TryInteract(D.Hero,flow.Session),"claim neutral on arrival");
            Require(territory.March("outpost"),"actual march enemy");yield return Arrive(D.Hero);Require(Find("territory-outpost").TryInteract(D.Hero,flow.Session),"real conquest start");
            var rts=D.Rts;var units=new List<RtsUnitStatus>();rts.CopyStatus(units);Require(rts.ContextId=="conquest-outpost"&&units.Count==8&&units.Single(u=>u.Hero).Unit==D.Hero,"same hero + four paid soldiers vs three defenders");
            rts.SelectArea(Camera.main,new Rect(0,0,Screen.width,Screen.height),false);Require(rts.SelectedAllies==5,"native multiple selection");yield return Capture("Selection");
            var foe=units.First(u=>u.Team==1).Unit;Require(rts.Command(foe.transform.position,foe.GetComponentInChildren<Collider>(),false),"attack order");yield return Wait(.5f);yield return Capture("RtsBattle");
            var deadline=Time.realtimeSinceStartup+100;while(rts.Active&&Time.realtimeSinceStartup<deadline){if(S.Health<flow.Session.Rules.Find(S.ClassId).Health)rts.HeroAction("recover");yield return null;}
            Require(!rts.Active&&S.Territories.Single(t=>t.Id=="outpost").Owner==S.FactionId&&S.Health>0,"real conquest victory");
            Require(S.Companions.Count==3&&S.Facts.Contains("umbral-outpost-conquered"),"companions not consumed by RTS");yield return Capture("Conquest");
            var wood=S.Wood;var cycle=S.ProductionCycle;deadline=Time.realtimeSinceStartup+12;while(S.ProductionCycle==cycle&&Time.realtimeSinceStartup<deadline)yield return null;Require(S.Wood==wood+2,"two territorial online bonuses, paused workshop");
            panel.Open();Click("territory-tab");yield return null;yield return Capture("ControlledResources");panel.Close();
            // Deja una reserva real para comprobar también esa composición al reiniciar.
            yield return Move(new Vector3(9,0,-10));Require(party.SetParty(first,false),"reserve before restart");Require(D.Save(slot),"atomic final save");
            File.WriteAllText(slot+".identity.json",JsonUtility.ToJson(S,true));Require(flow.ReturnToTitle(),"save/title");yield return Capture("Saved");
        }
        private IEnumerator HealAtCamp()
        {
            var attempts=0;while((S.Health<flow.Session.Rules.Find(S.ClassId).Health||S.Companions.Any(c=>c.Health<party.Roles.Find(c.Role).Health))&&attempts++<8)
            {Require(dungeon.Rest(),"legal camp rest");while(dungeon.Resting)yield return null;}
            Require(S.Health==flow.Session.Rules.Find(S.ClassId).Health,"rest completed with world time");
        }
        private IEnumerator Wasd()
        {
            var keyboard=InputSystem.AddDevice<Keyboard>();var previous=InputSystem.settings.backgroundBehavior;
            try{InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;EventSystem.current.SetSelectedGameObject(null);var p=D.Hero.transform.position;InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));yield return Wait(.4f);InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;Require(Vector3.Distance(p,D.Hero.transform.position)>.25f,"actual WASD");}
            finally{InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=previous;}
        }
        private IEnumerator Fight()
        {
            var e=D.Encounter;var clock=S.WorldSeconds;var until=Time.realtimeSinceStartup+120;
            while(e.GroupActive&&Time.realtimeSinceStartup<until)
            {
                var g=e.Group;if(g.Current.Hostile){yield return null;continue;}var u=e.GroupUnit(g.Current.Id);if(u.IsMoving){yield return null;continue;}
                var target=g.Members.Where(a=>a.Hostile&&a.Health>0).OrderBy(a=>Vector3.Distance(u.transform.position,e.GroupUnit(a.Id).transform.position)).First();e.SelectGroupTarget(target.Id);
                if(g.ActionAvailable){if(g.Current.Health<g.Current.Profile.Health*.5f&&!g.Current.Recovered)e.GroupRecover();else{e.GroupApproach();yield return Arrive(u);if(!e.GroupAttack()&&g.ActionAvailable)e.GroupGuard();}}
                if(e.GroupActive)e.GroupEnd();yield return null;
            }
            Require(!e.GroupActive&&flow.Session.Mode==PrototypeMode.Exploration,"actual group win");Require(Math.Abs(S.WorldSeconds-clock)<.2,"strategic clock paused in combat");
        }
        private IEnumerator Move(Vector3 position){Require(D.Hero.TryMoveTo(position),"path to "+position);yield return Arrive(D.Hero);}
        private static IEnumerator Arrive(ExplorationUnit u){yield return null;var end=Time.realtimeSinceStartup+25;while(u.IsMoving&&Time.realtimeSinceStartup<end)yield return null;Require(!u.IsMoving,"navigation arrival "+u.name+" at "+u.transform.position);}
        private static IEnumerator Wait(float seconds){var end=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<end)yield return null;}
        private static InteractionPoint Find(string id)=>FindObjectsByType<InteractionPoint>().Single(p=>p.FactId==id);
        private IEnumerator Capture(string name)=>DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(slot),"Captures",Path.GetFileNameWithoutExtension(slot)+"-"+name+".png"));
        private void Click(string id){var b=panel.Controls[id];Require(b.gameObject.activeInHierarchy&&b.interactable,"expedition button "+id);b.onClick.Invoke();}
        private void ClickFront(string id){var b=flow.Front.Controls[id];Require(b.gameObject.activeInHierarchy&&b.interactable,"menu button "+id);b.onClick.Invoke();}
        private static void Require(bool value,string message){if(!value)throw new InvalidOperationException("Expedition smoke: "+message);}
    }
}
