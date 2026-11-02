using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed partial class TurnEncounter
    {
        [SerializeField] private PartyWorld partyWorld;
        [SerializeField] private CombatFeedbackPresenter groupFeedback;
        private TurnBattle groupBattle;private Coroutine groupRoutine,groupMovement;private bool groupReported;
        private readonly Dictionary<string,ExplorationUnit> groupUnits=new Dictionary<string,ExplorationUnit>();
        private readonly List<ExplorationUnit> groupEnemies=new List<ExplorationUnit>();
        private string groupTarget;
        public bool InGroup=>groupBattle!=null;
        public bool GroupConfigured=>partyWorld!=null;
        public bool GroupActive=>Group!=null&&!Group.Finished;
        public GroupTurnSequence Group=>groupBattle?.Group;
        public string GroupTarget=>groupTarget;
        public event Action<bool> GroupResolved;
        public void ConfigureGroup(PartyWorld party,CombatFeedbackPresenter feedback){partyWorld=party;groupFeedback=feedback;}
        private void ClearGroup()
        {
            if(groupRoutine!=null){StopCoroutine(groupRoutine);groupRoutine=null;}
            if(groupMovement!=null){StopCoroutine(groupMovement);groupMovement=null;}
            foreach(var u in groupUnits.Values)if(u!=null){var a=u.GetComponentInChildren<HumanoidAnimationDriver>();a?.SetGuarding(false);}
            foreach(var u in groupEnemies)if(u!=null){u.gameObject.SetActive(false);Destroy(u.gameObject);}groupEnemies.Clear();groupUnits.Clear();groupBattle=null;
            if(hero!=null){hero.Agent.avoidancePriority=50;var p=hero.GetComponent<CharacterCombatAnimationPresenter>();if(p!=null)p.ExternallyControlled=false;}
        }
        public ExplorationUnit GroupUnit(string id)=>id!=null&&groupUnits.TryGetValue(id,out var u)?u:null;
        public bool BeginGroup(Vector3 centre,int seed=0)
        {
            if(partyWorld==null||session.State.Health<=0||(Battle!=null&&!Battle.Finished)||GroupActive||session.Mode!=PrototypeMode.Exploration)return false;
            var places=new List<Vector3>();
            for(var i=0;i<2;i++)
            {
                var desired=centre+new Vector3(i*2-1,0,1.5f);
                if(!NavMesh.SamplePosition(desired,out var at,.6f,NavMesh.AllAreas)||Physics.OverlapCapsule(at.position+Vector3.up*.35f,at.position+Vector3.up*1.45f,.29f,~0,QueryTriggerInteraction.Ignore).Any(c=>c.GetComponentInParent<ExplorationUnit>()==null))return false;places.Add(at.position);
            }
            Clear();partyWorld.Reconcile();hero.Stop();groupUnits.Add("hero",hero);
            var members=new List<TacticalCombatant>{new TacticalCombatant{Id="hero",Title=session.State.Name,Hero=true,Profile=session.HeroProfile,ReadHealth=()=>session.State.Health,WriteHealth=value=>session.State.Health=value,Weapon=HeroActionRules.WeaponBonus(session.State),Armour=session.State.Armour=="cloth"?1:0}};
            foreach(var c in session.State.Companions.Where(x=>x.InParty&&x.Health>0))
            {
                if(!partyWorld.Units.TryGetValue(c.Id,out var unit)||Vector3.Distance(unit.transform.position,hero.transform.position)>12)continue;
                unit.Stop();groupUnits.Add(c.Id,unit);members.Add(new TacticalCombatant{Id=c.Id,Title=partyWorld.Roles.Find(c.Role).Title,Profile=ExpeditionRules.RoleProfile(partyWorld.Roles.Find(c.Role)),ReadHealth=()=>c.Health,WriteHealth=value=>c.Health=value});
            }
            for(var i=0;i<2;i++)
            {
                var role=partyWorld.Roles.Find(i==0?"swordsman":"archer");var record=new RecruitRecord{Health=role.Health};var unit=Instantiate(enemyPrefab,places[i],Quaternion.identity).GetComponent<ExplorationUnit>();unit.name="Guardián del Umbral "+i;
                unit.Agent.speed=role.Speed;SupportVisualFactory.Attach(unit,role,partyWorld.Flow.Front.Catalog,groupFeedback.SharedEffectMaterial);groupEnemies.Add(unit);var id="enemy-"+i;groupUnits.Add(id,unit);
                members.Add(new TacticalCombatant{Id=id,Title=unit.name,Hostile=true,Profile=ExpeditionRules.RoleProfile(role),ReadHealth=()=>record.Health,WriteHealth=value=>record.Health=value});
            }
            groupTarget="enemy-0";groupReported=false;groupBattle=TurnBattle.CreateGroup(members,seed==0?Environment.TickCount:seed);hero.GetComponent<CharacterCombatAnimationPresenter>().ExternallyControlled=true;
            session.SetMode(PrototypeMode.TurnCombat);GroupRefresh();return true;
        }
        private bool GroupReady()=>GroupActive&&!Group.Current.Hostile&&!session.Paused&&!GroupUnit(Group.Current.Id).IsMoving;
        public bool GroupSelect(Collider hit)
        {if(!GroupActive||hit==null)return false;var target=Group.Members.FirstOrDefault(a=>a.Hostile&&a.Health>0&&hit.transform.IsChildOf(GroupUnit(a.Id).transform));if(target==null)return false;groupTarget=target.Id;return true;}
        public void SelectGroupTarget(string id){if(Group!=null&&Group.Members.Any(a=>a.Id==id&&a.Hostile&&a.Health>0))groupTarget=id;}
        public bool GroupAttack()
        {
            if(!GroupReady())return false;var source=GroupUnit(Group.Current.Id);var target=Group.Members.FirstOrDefault(x=>x.Id==groupTarget&&x.Health>0);
            if(target==null)return false;var victim=GroupUnit(target.Id);
            if(!Group.Attack(target,Vector3.Distance(source.transform.position,victim.transform.position),HasClearAttackLine(source,victim)))return false;
            GroupPresent(Group.Current,target);GroupRefresh();return true;
        }
        public bool GroupGuard(){if(!GroupReady()||!Group.Guard())return false;GroupPresent(Group.Current,Group.Current);GroupRefresh();return true;}
        public bool GroupRecover(){if(!GroupReady()||!Group.Recover())return false;GroupPresent(Group.Current,Group.Current);GroupRefresh();return true;}
        public bool GroupMove(Vector3 at)
        {
            if(!GroupReady()||Group.MovementRemaining<.05f)return false;
            var u=GroupUnit(Group.Current.Id);if(!u.TryMoveTo(at,Group.MovementRemaining))return false;
            if(!Group.Move(u.LastPathLength)){u.Stop();return false;}
            groupMovement=StartCoroutine(WatchGroupMovement(u));return true;
        }
        private IEnumerator WatchGroupMovement(ExplorationUnit unit)
        {
            yield return null;var position=unit.transform.position;var progressAt=Time.time;var started=Time.time;
            while(GroupActive&&unit.IsMoving)
            {
                if(Vector3.Distance(position,unit.transform.position)>.08f){position=unit.transform.position;progressAt=Time.time;}
                if(Time.time-progressAt>1.5f||Time.time-started>8){unit.Stop();session.Say("Paso bloqueado. Elige otra posición o termina el turno.");break;}
                yield return null;
            }
            groupMovement=null;
        }
        public void GroupApproach()
        {
            if(!GroupReady())return;var u=GroupUnit(Group.Current.Id);var target=GroupUnit(groupTarget);if(target==null)return;
            var desired=target.transform.position-u.transform.position;desired.y=0;
            if(desired.magnitude<=Group.Current.Profile.Range&&HasClearAttackLine(u,target))return;
            GroupMove(u.transform.position+desired.normalized*Mathf.Min(Group.MovementRemaining,Mathf.Max(0,desired.magnitude-Group.Current.Profile.Range*.8f)));
        }
        public bool GroupEnd()
        {
            if(!GroupActive||Group.Current.Hostile||session.Paused)return false;
            GroupUnit(Group.Current.Id).Stop();if(groupMovement!=null){StopCoroutine(groupMovement);groupMovement=null;}
            if(!Group.EndTurn())return false;GroupRefresh();return true;
        }
        private void GroupPresent(TacticalCombatant source,TacticalCombatant target)
        {
            var a=GroupUnit(source.Id);var b=GroupUnit(target.Id);var driver=a.GetComponentInChildren<HumanoidAnimationDriver>();var victim=b.GetComponentInChildren<HumanoidAnimationDriver>();var result=Group.LastResolution;
            if(result.Kind==CombatActionKind.Attack){driver?.TryPlay(CharacterMotion.Attack);if(result.Hit)victim?.TryPlay(target.Health<=0?CharacterMotion.Defeat:CharacterMotion.Hit);}
            if(result.Kind==CombatActionKind.Guard)driver?.SetGuarding(true);
            groupFeedback.PresentRealtime(new CombatFeedback(-20,result,source.Profile.Id,a.transform.position,b.transform.position));
        }
        private void GroupRefresh()
        {
            if(Group==null)return;
            foreach(var pair in groupUnits){pair.Value.Agent.avoidancePriority=pair.Key==Group.Current.Id?10:80;if(pair.Key!=Group.Current.Id)pair.Value.Stop();}
            foreach(var a in Group.Members)
            {var u=GroupUnit(a.Id);var driver=u.GetComponentInChildren<HumanoidAnimationDriver>();driver?.SetGuarding(a.Guarding);if(a.Health<=0){u.Stop();driver?.SetDefeated(true);foreach(var c in u.GetComponentsInChildren<Collider>())if(!a.Hero)c.enabled=false;}}
            if(Group.Finished)
            {
                foreach(var u in groupUnits.Values)u.Stop();session.Say(Group.Won?"Victoria de la expedición; objetivo despejado.":"Derrota de la expedición. Los compañeros no desaparecen; puedes retirarte al campamento.");
                if(!groupReported){groupReported=true;GroupResolved?.Invoke(Group.Won);}return;
            }
            session.Say("Ronda "+Group.Round+" · turno de "+Group.Current.Title);
            if(Group.Current.Hostile&&groupRoutine==null)groupRoutine=StartCoroutine(GroupEnemyTurn());
        }
        private IEnumerator GroupEnemyTurn()
        {
            yield return new WaitForSeconds(.55f);
            if(!GroupActive){groupRoutine=null;yield break;}
            var source=Group.Current;var u=GroupUnit(source.Id);var target=Group.Members.Where(a=>a.Hostile!=source.Hostile&&a.Health>0).OrderBy(a=>Vector3.Distance(u.transform.position,GroupUnit(a.Id).transform.position)).First();var v=GroupUnit(target.Id);
            if(Vector3.Distance(u.transform.position,v.transform.position)>source.Profile.Range||!HasClearAttackLine(u,v))
            {
                var delta=v.transform.position-u.transform.position;delta.y=0;
                var desired=u.transform.position+delta.normalized*Mathf.Min(2.8f,Mathf.Max(0,delta.magnitude-source.Profile.Range*.8f));
                if(u.TryMoveTo(desired,3)){var until=Time.time+4;while(u.IsMoving&&Time.time<until)yield return null;u.Stop();}
            }
            if(Group.Attack(target,Vector3.Distance(u.transform.position,v.transform.position),HasClearAttackLine(u,v)))GroupPresent(source,target);
            yield return new WaitForSeconds(.45f);groupRoutine=null;
            if(GroupActive)Group.EndTurn();GroupRefresh();
        }
    }
}
