using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed class PartyWorld : MonoBehaviour
    {
        public DemoFlow Flow;
        public ExpeditionRules Rules;
        public CombatFeedbackPresenter Feedback;
        private readonly Dictionary<string,ExplorationUnit> units=new Dictionary<string,ExplorationUnit>();
        private PrototypeState observed;private float nextFollow;
        public IReadOnlyDictionary<string,ExplorationUnit> Units=>units;
        public RtsDemoRules Roles=>Flow.Director.Rts.DemoRules;
        public bool AtCamp=>Flow.IsPlaying&&Flow.Session.Mode==PrototypeMode.Exploration&&Vector3.Distance(Flow.Director.Hero.transform.position,new Vector3(9,0,-10))<9;
        private void OnEnable(){if(Flow!=null)Flow.Started+=Reconcile;}
        private void OnDisable(){if(Flow!=null)Flow.Started-=Reconcile;Clear();}
        private void Clear(){foreach(var u in units.Values)if(u!=null){u.gameObject.SetActive(false);Destroy(u.gameObject);}units.Clear();}
        public bool Recruit(string role)
        {var ok=AtCamp&&ExpeditionOperations.Recruit(Flow.Session.State,Rules,Roles,role);Flow.Session.Say(ok?"Compañero incorporado al grupo.":"No puedes incorporar: campamento, coste5oro/2alimento, rol único y máximo3.");if(ok)Reconcile();return ok;}
        public bool SetParty(string id,bool active)
        {var ok=AtCamp&&ExpeditionOperations.SetParty(Flow.Session.State,Rules,id,active);if(ok)Reconcile();return ok;}
        public void Reconcile()
        {
            var state=Flow.Session.State;if(observed!=state){Clear();observed=state;}
            foreach(var id in units.Keys.ToArray())if(!state.Companions.Any(c=>c.Id==id&&c.InParty&&c.Health>0)){Destroy(units[id].gameObject);units.Remove(id);}
            var i=0;foreach(var c in state.Companions.Where(x=>x.InParty&&x.Health>0))
            {
                if(!units.ContainsKey(c.Id)&&TrySlot(Flow.Director.Hero.transform.position,i,out var at))
                {var role=Roles.Find(c.Role);var unit=Instantiate(Flow.Director.Rts.UnitTemplate,at,Quaternion.identity).GetComponent<ExplorationUnit>();unit.name=role.Title+" · "+c.Id.Substring(0,4);unit.Agent.speed=role.Speed;unit.Agent.avoidancePriority=80+i*5;SupportVisualFactory.Attach(unit,role,Flow.Front.Catalog,Feedback.SharedEffectMaterial);units.Add(c.Id,unit);}
                i++;
            }
        }
        public static bool TrySlot(Vector3 anchor,int index,out Vector3 position)
        {
            for(var attempt=0;attempt<8;attempt++)
            {
                var angle=(index*110+attempt*45)*Mathf.Deg2Rad;var requested=anchor+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*(1.25f+attempt*.15f);
                if(!NavMesh.SamplePosition(requested,out var hit,.5f,NavMesh.AllAreas))continue;
                if(Physics.OverlapCapsule(hit.position+Vector3.up*.35f,hit.position+Vector3.up*1.45f,.3f,~0,QueryTriggerInteraction.Ignore).Any(c=>c.GetComponentInParent<ExplorationUnit>()==null))continue;
                position=hit.position;return true;
            }
            position=default;return false;
        }
        public bool Regroup(Vector3 anchor)
        {
            var active=Flow.Session.State.Companions.Where(c=>c.InParty&&c.Health>0).ToArray();var positions=new List<Vector3>();
            for(var i=0;i<active.Length;i++){if(!TrySlot(anchor,i,out var p))return false;positions.Add(p);}
            Reconcile();for(var i=0;i<active.Length;i++)if(units.TryGetValue(active[i].Id,out var u)){u.gameObject.SetActive(true);u.Stop();if(!u.Warp(positions[i]))return false;}return true;
        }
        private void Update()
        {
            if(Flow==null||!Flow.IsPlaying){if(units.Count>0)Clear();return;}
            if(Flow.Session.State!=observed)Reconcile();
            var show=Flow.Session.Mode!=PrototypeMode.Rts;foreach(var u in units.Values)if(u!=null&&u.gameObject.activeSelf!=show)u.gameObject.SetActive(show);
            if(!show||Flow.Session.Paused||Flow.Session.Mode!=PrototypeMode.Exploration||Time.time<nextFollow)return;nextFollow=Time.time+.25f;Reconcile();
            var i=0;foreach(var u in units.Values)
            {
                var distance=Vector3.Distance(u.transform.position,Flow.Director.Hero.transform.position);
                if(distance>25){Regroup(Flow.Director.Hero.transform.position);break;}
                if(distance<1.2f){u.Stop();i++;continue;}
                var hero=Flow.Director.Hero.transform;var rank=i++;var desired=hero.position-hero.forward*(1.6f+rank*.5f)+hero.right*((rank%2==0?1:-1)*.8f);
                if(NavMesh.SamplePosition(desired,out var behind,.8f,NavMesh.AllAreas))u.TryMoveTo(behind.position);
            }
        }
    }
}
