using UnityEngine;

namespace Aethernia
{
    public sealed partial class TurnEncounter
    {
        public bool CanUseCoreCommands => !session.Paused&&!session.WorldUiOpen&&
            (InGroup?GroupReady():PlayerCanAct);
        public bool HasCoreTarget => CoreTarget(out _,out _,out _);
        public void ClearCoreTarget(){TargetSelected=false;groupTarget=null;}

        private bool CoreTarget(out ExplorationUnit source,out ExplorationUnit target,out TacticalCombatant member,string candidate=null)
        {
            member=null;
            source=InGroup?GroupUnit(Group.Current.Id):hero;
            var targetId=candidate??groupTarget;
            target=InGroup?GroupUnit(targetId):enemy;
            if(source==null||target==null)return false;
            if(!InGroup)return (TargetSelected||candidate=="solo")&&Battle!=null&&Battle.EnemyHealth>0;
            foreach(var actor in Group.Members)
                if(actor.Id==targetId&&actor.Hostile&&actor.Health>0){member=actor;return true;}
            return false;
        }

        public string CoreTargetStatus(bool technique,string candidate=null)
        {
            if(!CoreTarget(out var source,out var target,out _,candidate))return "Selecciona un rival en iniciativa o en el terreno.";
            var profile=InGroup?Group.Current.Profile:session.HeroProfile;
            var range=technique?profile.Combat?.TechniqueRange??0:profile.Range;
            if(!HasClearAttackLine(source,target))return "Línea bloqueada. Cambia de posición.";
            var distance=Vector3.Distance(source.transform.position,target.transform.position);
            return distance>range?$"Fuera de alcance · {distance:0.#} / {range:0.#} m":null;
        }

        public string CoreTargetSummary
        {
            get
            {
                if(!CoreTarget(out var source,out var target,out var actor))return "";
                var distance=Vector3.Distance(source.transform.position,target.transform.position);
                var flank=actor!=null&&FlankBonus(Group.Current,actor)>0?" · Flanqueo +2":"";
                return (actor?.Title??"Merodeador")+" · "+distance.ToString("0.#")+" m"+flank;
            }
        }

        private int FlankBonus(TacticalCombatant source,TacticalCombatant target)
        {
            if(!session.Rules.CoreRefine||source.Profile.Range>2.2f)return 0;
            var victim=GroupUnit(target.Id).transform.position;
            var strike=GroupUnit(source.Id).transform.position-victim;
            foreach(var ally in Group.Members)
            {
                if(ally==source||ally.Hostile!=source.Hostile||ally.Health<=0)continue;
                var support=GroupUnit(ally.Id).transform.position-victim;
                if(HasFlank(strike,support)&&HasClearAttackLine(GroupUnit(ally.Id),GroupUnit(target.Id)))return 2;
            }
            return 0;
        }

        public static bool HasFlank(Vector3 source,Vector3 support)
        {
            if(Mathf.Abs(source.y)>.7f||Mathf.Abs(support.y)>.7f)return false;
            source.y=support.y=0;
            return source.sqrMagnitude>.04f&&source.sqrMagnitude<=4.84f&&support.sqrMagnitude>.04f&&support.sqrMagnitude<=4.84f&&
                Vector3.Dot(source.normalized,support.normalized)<=-.5f;
        }
    }
}
