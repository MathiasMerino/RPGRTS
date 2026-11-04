using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed class DungeonExpedition : MonoBehaviour
    {
        public DemoFlow Flow;public PartyWorld Party;public ExpeditionRules Rules;
        public Vector3 SurfaceReturn=new Vector3(4,0,-11);
        private string encounterFact;
        private Coroutine rest;
        public bool Resting=>rest!=null;
        public bool Inside=>Flow.Director.Hero.transform.position.x>65;
        public int TestSeed { get; set; }
        private void OnEnable(){if(Flow!=null){Flow.Session.AdditionalInteraction+=Interact;Flow.Director.Encounter.GroupResolved+=Resolved;Flow.Started+=Loaded;}}
        private void OnDisable(){if(Flow!=null){Flow.Session.AdditionalInteraction-=Interact;Flow.Director.Encounter.GroupResolved-=Resolved;Flow.Started-=Loaded;}if(rest!=null)StopCoroutine(rest);rest=null;}
        private void Loaded(){if(Inside){Flow.CameraRig.SetRegion(new Vector3(80,0,0));Flow.CameraRig.FocusOn(Flow.Director.Hero.transform.position);}Party.Reconcile();}
        private bool? Interact(string id,string title)
        {
            if(id==FrontierContracts.Watch)
            {
                var session=Flow.Session;
                if(!session.Rules.LivingWorld||!Inside||!FrontierContracts.WatchAvailable(session.State)||Flow.Director.IsBusy)return false;
                return BeginEncounter(id,new Vector3(78,0,0),true);
            }
            if(!id.StartsWith("umbral-"))return null;
            if(!ExpeditionOperations.Ready(Flow.Session.State)){Flow.Session.Say("Asegura primero el campamento de la demo.");return false;}
            if(id=="umbral-entry")return Travel(true);
            if(id=="umbral-exit")return Travel(false);
            if(id=="umbral-objective")
            {var ok=ExpeditionOperations.RewardDungeon(Flow.Session.State,Rules);Flow.Session.Say(ok?"Ruinas completadas: +30oro, +15madera, +6alimento para tu asentamiento.":"Completa ambos encuentros; la recompensa se entrega una sola vez.");if(ok){Flow.Session.AwardForFact("umbral-reward");EquipmentOperations.RegisterDungeonLoot(Flow.Session.State);Flow.Director.Save(Flow.SlotPath);}return ok;}
            if(id=="umbral-cache")
            {if(Flow.Session.HasFact(id))return false;Flow.Session.State.Gold=Math.Min(1000000,Flow.Session.State.Gold+5);Flow.Session.State.Facts.Add(id);Flow.Session.Say("Alijo opcional:5oro, registrado una sola vez.");return true;}
            if(id=="umbral-battle-1"||id=="umbral-battle-2")
            {
                if(!Inside||Flow.Session.HasFact(id)||Flow.Director.IsBusy)return false;
                var centre=id.EndsWith("1")?new Vector3(78,0,0):new Vector3(86,0,0);
                return BeginEncounter(id,centre,false);
            }
            return false;
        }
        private bool BeginEncounter(string id,Vector3 centre,bool loneSentry)
        {
            if(!Flow.Director.Encounter.BeginGroup(centre,TestSeed,loneSentry))return false;
            encounterFact=id;
            var hero=Flow.Director.Hero.transform.position;
            var enemy=Flow.Director.Encounter.GroupUnit("enemy-0");
            if(Flow.Session.Rules.IntegratedAlpha&&enemy!=null)Flow.CameraRig.FrameDungeonFight(hero,enemy.transform.position);
            else Flow.CameraRig.FocusOn(hero);
            Flow.ExistingHud.ClosePanels();
            return true;
        }
        public bool Travel(bool enter)
        {
            if(Flow.Director.IsBusy||Flow.Session.Mode!=PrototypeMode.Exploration||Flow.Session.State.Health<=0||rest!=null)return false;
            var destination=enter?new Vector3(71,0,-4):SurfaceReturn;
            if(!NavMesh.SamplePosition(destination,out var at,.5f,NavMesh.AllAreas))return false;
            // Validate formation slots before moving any member.
            var count=0;foreach(var c in Flow.Session.State.Companions)if(c.InParty&&c.Health>0){if(!PartyWorld.TrySlot(at.position,count++,out _))return false;}
            Flow.Director.Encounter.Clear();Flow.Director.Hero.Stop();if(!Flow.Director.Hero.Warp(at.position))return false;Party.Regroup(at.position);
            Flow.CameraRig.SetRegion(enter?new Vector3(80,0,0):Vector3.zero);Flow.CameraRig.SetZoom(Flow.Session.Rules.WorldExploration?5.4f:9);Flow.CameraRig.FocusOn(at.position);
            Flow.Session.Say(enter?"Las Ruinas del Umbral · escenario provisional. Dos guardianías, alijo opcional, objetivo y salida.":"Regreso al campamento. El grupo conserva salud, identidad y recursos.");Flow.Director.Save(Flow.SlotPath);return true;
        }
        private void Resolved(bool won)
        {
            Flow.Front.Audio?.Play(won?FeedbackSound.Victory:FeedbackSound.Defeat,Vector3.zero,true);
            if(won)
            {if(!Flow.Session.HasFact(encounterFact)){Flow.Session.State.Facts.Add(encounterFact);Flow.Session.AwardForFact(encounterFact);TroopProgression.AwardCompanions(Flow.Session.State,encounterFact,Flow.Director.Encounter.Group.Members);}Flow.Director.Execute("explore");Flow.CameraRig.FocusOn(Flow.Director.Hero.transform.position);Party.Reconcile();Flow.Session.Say("Encuentro despejado. Continúa al objetivo o vuelve a descansar.");Flow.Director.Save(Flow.SlotPath);}
            else Flow.Session.Say("El héroe cayó. Retirada segura disponible; nadie del grupo se borra permanentemente.");
        }
        public bool Retreat()
        {
            if(!Flow.Director.Encounter.InGroup||Flow.Director.Encounter.GroupActive||Flow.Director.Encounter.Group.Won)return false;
            Flow.Director.Execute("explore");Flow.Director.Hero.Warp(new Vector3(9,0,-10));Flow.CameraRig.SetRegion(Vector3.zero);Flow.CameraRig.FocusOn(Flow.Director.Hero.transform.position);Party.Regroup(Flow.Director.Hero.transform.position);Flow.Director.Save(Flow.SlotPath);return true;
        }
        public bool Rest()
        {if(!Party.AtCamp||Flow.Director.IsBusy||rest!=null)return false;Flow.Director.Hero.Stop();rest=StartCoroutine(RestSequence());return true;}
        private IEnumerator RestSequence()
        {
            var restingState=Flow.Session.State;var start=restingState.WorldSeconds;var origin=Flow.Director.Hero.transform.position;Flow.Session.Say(restingState.ProgressionRevision==1?"Descanso10s: hasta4PV para héroe, grupo y ejército. Caminar lo cancela.":"Descanso10segundos en campamento: recupera hasta4PV por miembro. Caminar lo cancela.");
            while(Flow.Session.State.WorldSeconds-start<Rules.RestSeconds)
            {if(!Flow.IsPlaying||Flow.Session.State!=restingState||Flow.Session.Mode!=PrototypeMode.Exploration||Vector3.Distance(origin,Flow.Director.Hero.transform.position)>.6f){rest=null;yield break;}yield return null;}
            if(Flow.Session.State!=restingState){rest=null;yield break;}
            var state=Flow.Session.State;state.Health+=HeroActionRules.Recovery(state.Health,Flow.Session.HeroProfile.Health);
            if(state.Health>0)Flow.Director.Hero.GetComponentInChildren<HumanoidAnimationDriver>()?.SetDefeated(false);
            foreach(var c in state.Companions)c.Health+=HeroActionRules.Recovery(c.Health,TroopProgression.Profile(c,Party.Roles.Find(c.Role)).Health);
            if(state.ProgressionRevision==1)foreach(var soldier in state.Army)soldier.Health+=HeroActionRules.Recovery(soldier.Health,TroopProgression.Profile(soldier,Party.Roles.Find(soldier.Role)).Health);
            rest=null;Party.Reconcile();Flow.Session.Say(state.ProgressionRevision==1?"Descanso completado: héroe, grupo y ejército recuperan salud.":"Descanso completado. Salud recuperada con el tiempo de juego; no hay muerte permanente de compañeros.");Flow.Director.Save(Flow.SlotPath);
        }
    }
}
