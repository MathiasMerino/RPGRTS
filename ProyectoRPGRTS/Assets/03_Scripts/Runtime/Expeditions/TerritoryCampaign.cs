using System;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    public sealed class TerritoryCampaign : MonoBehaviour
    {
        public DemoFlow Flow;public PartyWorld Party;public ExpeditionRules Rules;
        private PrototypeState observed;private string pending;private Vector3 returnPoint;
        public string LastResult{get;private set;}="";
        private void OnEnable(){if(Flow!=null){Flow.Session.AdditionalInteraction+=Interact;Flow.Director.Rts.Resolved+=Resolved;}}
        private void OnDisable(){if(Flow!=null){Flow.Session.AdditionalInteraction-=Interact;Flow.Director.Rts.Resolved-=Resolved;}}
        private void Update()
        {
            if(!Flow.IsPlaying)return;
            if(Flow.Session.State!=observed){observed=Flow.Session.State;pending=null;}
            ExpeditionOperations.InitializeTerritories(observed,Rules);
        }
        public bool SelectSoldier(string id,bool selected)
        {
            if(Flow.Director.IsBusy||Flow.Session.State.Army.All(r=>r.Id!=id))return false;var list=Flow.Session.State.ArmySelection;
            if(selected){if(!list.Contains(id))list.Add(id);}else list.Remove(id);return true;
        }
        public bool March(string territory)
        {
            if(Flow.Director.IsBusy||Flow.Session.Mode!=PrototypeMode.Exploration||Flow.Session.State.Health<=0)return false;
            var p=Rules.Find(territory);if(p==null||p.Id=="camp")return false;
            return Flow.Director.Hero.TryMoveTo(p.Position+Vector3.forward*1.5f);
        }
        private bool? Interact(string id,string title)
        {
            if(id=="territory-crossing")
            {var ok=ExpeditionOperations.ClaimNeutral(Flow.Session.State,Rules);Flow.Session.Say(ok?"Paso neutral asegurado: +1madera/ciclo10s online, limitado por almacén.":"El paso ya tiene propietario.");if(ok)Flow.Director.Save(Flow.SlotPath);return ok;}
            if(id!="territory-outpost")return null;
            return BeginConquest();
        }
        public bool BeginConquest()
        {
            var s=Flow.Session.State;ExpeditionOperations.InitializeTerritories(s,Rules);var t=s.Territories.Find(x=>x.Id=="outpost");var access=s.Territories.Find(x=>x.Id=="crossing");
            if(!Flow.IsPlaying||Flow.Director.IsBusy||Flow.Session.Mode!=PrototypeMode.Exploration||t==null||t.Owner==s.FactionId||access?.Owner!=s.FactionId||Vector3.Distance(Flow.Director.Hero.transform.position,Rules.Find("outpost").Position)>2.2f)
            {Flow.Session.Say("Asegura el paso neutral y acércate al puesto enemigo.");return false;}
            if(s.ArmySelection.Count==0){Flow.Session.Say("Prepara al menos un recluta militar en la pestaña Ejército.");return false;}
            returnPoint=Flow.Director.Hero.transform.position;Flow.Director.Encounter.Clear();
            if(!Flow.Director.Rts.BeginExpedition(Flow.Director.Hero,s.ArmySelection,Rules.OutpostDefenders))return false;
            pending="outpost";LastResult="Batalla en curso. El propietario aún no cambia.";Flow.CameraRig.SetRegion(Vector3.zero);Flow.CameraRig.SetZoom(8);Flow.CameraRig.FocusOn(new Vector3(-9,0,13));return true;
        }
        private void Resolved(bool won)
        {
            if(pending==null||Flow.Director.Rts.ContextId!="conquest-outpost")return;var id=pending;pending=null;
            var changed=ExpeditionOperations.Conquer(Flow.Session.State,id,won);
            LastResult=changed?"Victoria: puesto conquistado. +1madera/ciclo online, propietario persistente.":"Derrota: el puesto conserva su propietario. Bajas RTS registradas; compañeros RPG no desplegados.";
            Flow.Director.Execute("explore");Flow.Director.Hero.Warp(won?returnPoint:new Vector3(9,0,-10));Party.Regroup(Flow.Director.Hero.transform.position);
            Flow.CameraRig.SetRegion(Vector3.zero);Flow.CameraRig.SetZoom(11);Flow.CameraRig.FocusOn(Flow.Director.Hero.transform.position);Flow.Session.Say(LastResult);Flow.Director.Save(Flow.SlotPath);
        }
    }
}
