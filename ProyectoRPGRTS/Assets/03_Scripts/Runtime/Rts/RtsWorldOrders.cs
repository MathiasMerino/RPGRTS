using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed partial class RtsBattleController
    {
        public string LastOrderText { get; private set; } = "";
        public Vector3 LastOrderPosition { get; private set; }
        public float LastOrderAt { get; private set; } = -100;
        private Vector3 AttackPosition(Soldier actor, Soldier target)
        {
            var direction=actor.Unit.transform.position-target.Unit.transform.position;direction.y=0;
            if(direction.sqrMagnitude<.01f)direction=Vector3.left;
            return target.Unit.transform.position+direction.normalized*Mathf.Max(.65f,actor.Range*.8f);
        }
        private bool WorldCommand(Vector3 point,Collider collider)
        {
            if(session.Paused||session.WorldUiOpen||float.IsNaN(point.sqrMagnitude)||float.IsInfinity(point.sqrMagnitude))return false;
            var selected=soldiers.Where(s=>s.Team==0&&Health(s)>0&&s.Unit.IsSelected).ToArray();
            if(selected.Length==0){session.Say("Selecciona aliados antes de dar una orden.");return false;}
            var target=collider==null?null:soldiers.Find(s=>s.Team==1&&Health(s)>0&&s.Unit.gameObject.activeInHierarchy&&collider.transform.IsChildOf(s.Unit.transform));
            if(collider!=null&&collider.GetComponentInParent<ExplorationUnit>()!=null&&target==null)return false;
            var destinations=new Vector3[selected.Length];
            var path=new NavMeshPath();
            for(var i=0;i<selected.Length;i++)
            {
                var requested=target!=null?AttackPosition(selected[i],target):point+new Vector3((i%3-1)*1.2f,0,(i/3)*1.2f);
                if(!NavMesh.SamplePosition(requested,out var hit,.6f,NavMesh.AllAreas)||!NavMesh.CalculatePath(selected[i].Unit.transform.position,hit.position,NavMesh.AllAreas,path)||path.status!=NavMeshPathStatus.PathComplete)
                {session.Say("Destino de formación inaccesible; orden conservada.");return false;}
                destinations[i]=hit.position;
            }
            if(Preparing&&target!=null&&!EngagePrepared())return false;
            for(var i=0;i<selected.Length;i++)
            {
                var s=selected[i];s.OrderedTarget=target;s.MoveOrder=target==null;s.OrderedUntil=target==null?Time.time+3:Time.time;
                s.Unit.TryMoveTo(destinations[i]);Trace("command",s,target);
            }
            LastOrderText=(target==null?"Mover formación":"Atacar objetivo")+" · "+selected.Length;
            LastOrderPosition=point;LastOrderAt=Time.unscaledTime;session.Say(LastOrderText);return true;
        }
    }
}
