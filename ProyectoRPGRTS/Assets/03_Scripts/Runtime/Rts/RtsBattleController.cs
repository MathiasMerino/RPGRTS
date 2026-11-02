using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed class RtsBattleController : MonoBehaviour
    {
        private sealed class Soldier
        {
            public ExplorationUnit Unit;
            public int Team;
            public int Health = 10;
            public float AttackAt;
            public float OrderedUntil;
            public string Role="swordsman";
            public string RecruitId;
            public float Range=1.5f;
            public int Damage=2,Maximum=10;
            public bool Hero,Guarding,MoveOrder;
            public Soldier OrderedTarget;
            public GameObject Visual;
            public HumanoidAnimationDriver Animator;
        }
        [SerializeField] private GameSession session;
        [SerializeField] private GameObject template;
        [SerializeField] private Material hostile;
        [SerializeField] private RtsDemoRules demoRules;
        [SerializeField] private CharacterVisualCatalog visuals;
        [SerializeField] private CombatFeedbackPresenter feedback;
        private ExplorationUnit sharedHero;
        private bool heroRecoveryUsed;
        private int sequence;
        public bool IncludesHero=>sharedHero!=null;
        public RtsDemoRules DemoRules=>demoRules;
        public GameObject UnitTemplate=>template;
        public string ContextId { get; private set; }="patrol";
        public event Action<bool> Resolved;
        public event Action<CombatFeedback> ActionResolved;
        private readonly List<Soldier> soldiers = new List<Soldier>();
        private float nextTick;
        public bool Active { get; private set; }
        public int StartingAllyHealth { get; private set; }
        public int Allies => soldiers.Count(s => s.Team == 0 && Health(s) > 0);
        public int Enemies => soldiers.Count(s => s.Team == 1 && Health(s) > 0);
        public int SelectedAllies => soldiers.Count(s => s.Team == 0 && Health(s) > 0 && s.Unit != null && s.Unit.IsSelected);
        public bool Won => !Active && Allies > 0 && Enemies == 0 && soldiers.Count > 0 && (!IncludesHero||session.State.Health>0);
        public void Configure(GameSession game, GameObject pawn, Material material) { session = game; template = pawn; hostile = material; }
        public void ConfigureDemo(RtsDemoRules rules,CharacterVisualCatalog models,CombatFeedbackPresenter effects){demoRules=rules;visuals=models;feedback=effects;}
        private int Health(Soldier s)=>s.Hero?session.State.Health:s.Health;
        private int Maximum(Soldier s)=>s.Hero?session.Rules.Find(session.State.ClassId).Health:s.Maximum;
        private void SetHealth(Soldier s,int value)
        {if(s.Hero)session.State.Health=value;else{s.Health=value;if(!string.IsNullOrEmpty(s.RecruitId)){var record=session.State.Army.Find(x=>x.Id==s.RecruitId);if(record!=null)record.Health=value;}}}
        public void CopyStatus(List<RtsUnitStatus> buffer){buffer.Clear();foreach(var s in soldiers)buffer.Add(new RtsUnitStatus(s.Unit,s.Hero?session.Rules.Find(session.State.ClassId).Title:demoRules?.Find(s.Role)?.Title??s.Role,s.Team,Health(s),Maximum(s),s.Hero));}
        public void Clear()
        {
            foreach (var soldier in soldiers)
                if (soldier.Unit != null)
                {
                    if(soldier.Hero){soldier.Unit.Stop();var p=soldier.Unit.GetComponent<CharacterCombatAnimationPresenter>();if(p!=null)p.ExternallyControlled=false;soldier.Animator?.SetGuarding(false);}
                    else{soldier.Unit.gameObject.SetActive(false);Destroy(soldier.Unit.gameObject);}
                }
            soldiers.Clear();sharedHero=null; Active = false;
        }
        public bool Begin(bool escalated = false)
        {
            if (Active) return false;
            // Validate the existing world before creating any participant or changing mode.
            var positions = new Vector3[12];
            var path = new NavMeshPath();
            for (var team = 0; team < 2; team++) for (var i = 0; i < 6; i++)
            {
                var requested = new Vector3(team == 0 ? -12 : -6, 0, 11 + i * 0.9f);
                if (!NavMesh.SamplePosition(requested, out var hit, 0.35f, NavMesh.AllAreas)) return RejectDeployment();
                positions[team * 6 + i] = hit.position;
                foreach (var blocker in Physics.OverlapCapsule(hit.position + Vector3.up * 0.35f, hit.position + Vector3.up * 1.45f, 0.29f, ~0, QueryTriggerInteraction.Ignore))
                    if (blocker.GetComponentInParent<ExplorationUnit>() == null) return RejectDeployment();
                if (!NavMesh.CalculatePath(positions[0], hit.position, NavMesh.AllAreas, path) || path.status != NavMeshPathStatus.PathComplete) return RejectDeployment();
            }
            Clear();
            StartingAllyHealth = escalated ? 14 : 10; nextTick = 0;
            for (var team = 0; team < 2; team++) for (var i = 0; i < 6; i++)
            {
                var unit = Instantiate(template, positions[team * 6 + i], Quaternion.identity).GetComponent<ExplorationUnit>();
                unit.name = (team == 0 ? "A" : "B") + " Soldier " + i;
                unit.Agent.speed = 2.5f;
                unit.Select(team == 0);
                if (team == 1) foreach (var renderer in unit.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = hostile;
                soldiers.Add(new Soldier { Unit = unit, Team = team, Health = team == 0 && escalated ? 14 : 10,Maximum=team==0&&escalated?14:10 });
            }
            Active = true; session.SetMode(PrototypeMode.Rts);
            session.Say(escalated ? "Escalada experimental: la victoria RPG da +4 salud a cada aliado." : "RTS 6 contra 6. Clic derecho para ordenar; las unidades atacan al acercarse.");
            return true;
        }
        public bool BeginWithHero(ExplorationUnit hero)
        {return BeginHeroForce(hero,session.State.Army,false,6);}
        public bool BeginExpedition(ExplorationUnit hero,IReadOnlyList<string> ids,int defenders)
        {
            if(ids==null||ids.Count==0||ids.Count>6||ids.Distinct().Count()!=ids.Count||ids.Any(id=>session.State.Army.All(r=>r.Id!=id))||defenders<1||defenders>6)return false;
            return BeginHeroForce(hero,session.State.Army.Where(r=>ids.Contains(r.Id)).ToList(),true,defenders);
        }
        private bool BeginHeroForce(ExplorationUnit hero,IReadOnlyList<RecruitRecord> roster,bool expedition,int defenders)
        {
            if(hero==null||demoRules==null||Active||session.State.Health<=0)return false;
            var reinforcements=new List<Vector3>();
            for(var i=0;i<roster.Count;i++)
            {
                if(!NavMesh.SamplePosition(new Vector3(-10.5f,0,11+i*.9f),out var point,.35f,NavMesh.AllAreas))return RejectDeployment();
                foreach(var block in Physics.OverlapCapsule(point.position+Vector3.up*.35f,point.position+Vector3.up*1.45f,.29f,~0,QueryTriggerInteraction.Ignore))if(block.GetComponentInParent<ExplorationUnit>()==null)return RejectDeployment();
                reinforcements.Add(point.position);
            }
            var requested=new Vector3(-13.3f,0,13.2f);
            if(!NavMesh.SamplePosition(requested,out var at, .5f,NavMesh.AllAreas))return RejectDeployment();
            foreach(var block in Physics.OverlapCapsule(at.position+Vector3.up*.35f,at.position+Vector3.up*1.45f,.29f,~0,QueryTriggerInteraction.Ignore))if(block.GetComponentInParent<ExplorationUnit>()==null)return RejectDeployment();
            if(!Begin())return false;
            ContextId=expedition?"conquest-outpost":"patrol";
            if(expedition)
            {
                var enemyIndex=0;foreach(var s in soldiers.ToArray())if(s.Team==0||enemyIndex++>=defenders){s.Unit.gameObject.SetActive(false);Destroy(s.Unit.gameObject);soldiers.Remove(s);}
            }
            var index=0;
            foreach(var s in soldiers)
            {
                var role=demoRules.Roles[index++%demoRules.Roles.Length];s.Role=role.Id;s.Range=role.Range;s.Damage=role.Damage;s.Maximum=role.Health;s.Health=role.Health;s.Unit.Agent.speed=role.Speed;
                BuildVisual(s,role);
            }
            for(var i=0;i<roster.Count;i++)
            {
                var record=roster[i];var role=demoRules.Find(record.Role);if(role==null)continue;
                var unit=Instantiate(template,reinforcements[i],Quaternion.identity).GetComponent<ExplorationUnit>();unit.name="Recluta "+role.Title;unit.Agent.speed=role.Speed;unit.Select(true);
                var s=new Soldier{Unit=unit,Team=0,Health=record.Health,Maximum=role.Health,Damage=role.Damage,Range=role.Range,Role=role.Id,RecruitId=record.Id};BuildVisual(s,role);soldiers.Add(s);
            }
            hero.Stop();if(!hero.Warp(at.position)){Clear();session.SetMode(PrototypeMode.Exploration);return false;}
            sharedHero=hero;heroRecoveryUsed=false;sequence=0;hero.Select(true);
            var profile=session.Rules.Find(session.State.ClassId);var actor=hero.GetComponent<CharacterVisualPresenter>();
            var h=new Soldier{Unit=hero,Team=0,Hero=true,Role="hero",Range=profile.Range,Animator=actor.Model==null?null:actor.Model.GetComponent<HumanoidAnimationDriver>()};soldiers.Add(h);
            hero.GetComponent<CharacterCombatAnimationPresenter>().ExternallyControlled=true;
            session.Say((expedition?"Conquista del puesto.":"La patrulla necesita tu mando.")+" Arrastra para seleccionar; clic derecho para mover o atacar. Tu héroe conserva vida y equipo.");return true;
        }
        private void BuildVisual(Soldier s,RtsRoleProfile role)
        {
            var model=SupportVisualFactory.Attach(s.Unit,role,visuals,feedback==null?null:feedback.SharedEffectMaterial);if(model==null)return;s.Visual=model.gameObject;s.Animator=s.Visual.GetComponent<HumanoidAnimationDriver>();
        }
        private bool RejectDeployment()
        { session.Say("Zona de despliegue RTS bloqueada. Se conserva el mundo; prueba en una partida con esa zona libre."); return false; }
        public void SelectAll() { foreach (var s in soldiers) if (s.Unit != null && s.Team == 0 && Health(s) > 0) s.Unit.Select(true); }
        public void SelectArea(Camera camera,Rect area,bool additive)
        {
            if(!Active)return;foreach(var s in soldiers)if(s.Team==0&&Health(s)>0){var p=camera.WorldToScreenPoint(s.Unit.transform.position+Vector3.up*.9f);s.Unit.Select((additive&&s.Unit.IsSelected)||(p.z>0&&area.Contains(p)));}
        }
        public bool SelectUnit(Collider collider,bool additive)
        {
            if(!Active||collider==null)return false;var selected=collider.GetComponentInParent<ExplorationUnit>();var found=false;
            foreach(var s in soldiers)if(s.Team==0&&Health(s)>0){var match=s.Unit==selected;found|=match;s.Unit.Select(match||(additive&&s.Unit.IsSelected));}return found;
        }
        public void RememberGroup(List<ExplorationUnit> group){group.Clear();foreach(var s in soldiers)if(s.Team==0&&Health(s)>0&&s.Unit.IsSelected)group.Add(s.Unit);}
        public void SelectGroup(List<ExplorationUnit> group,bool additive){foreach(var s in soldiers)if(s.Team==0&&s.Unit!=null)s.Unit.Select(Health(s)>0&&(group.Contains(s.Unit)||(additive&&s.Unit.IsSelected)));}
        public bool Command(Vector3 point, Collider collider, bool primary)
        {
            if (!Active) return false;
            if (primary)
            {
                return SelectUnit(collider,false);
            }
            var accepted = false; var index = 0;
            var explicitTarget=collider==null?null:soldiers.Find(s=>s.Team==1&&Health(s)>0&&collider.transform.IsChildOf(s.Unit.transform));
            foreach (var s in soldiers)
                if (s.Team == 0 && Health(s) > 0 && s.Unit.IsSelected)
                {s.OrderedTarget=explicitTarget;s.MoveOrder=IncludesHero&&explicitTarget==null; accepted |= s.Unit.TryMoveTo(point + new Vector3((index % 3 - 1) * 0.8f, 0, index++ / 3 * 0.8f)); s.OrderedUntil = Time.time + 3; }
            return accepted;
        }
        private void Update()
        {
            if (!Active || session.Paused || Time.time < nextTick) return;
            nextTick = Time.time + 0.2f;
            foreach (var s in soldiers)
            {
                if (Health(s) <= 0) continue;
                if(IncludesHero&&s.MoveOrder&&s.Unit.IsMoving&&Time.time<s.OrderedUntil)continue;s.MoveOrder=false;
                Soldier target = s.OrderedTarget!=null&&Health(s.OrderedTarget)>0?s.OrderedTarget:null;var ordered=target!=null;var nearest=target==null?float.MaxValue:Vector3.Distance(s.Unit.transform.position,target.Unit.transform.position);
                foreach (var other in soldiers)
                    if (!ordered&&Health(other) > 0 && other.Team != s.Team)
                    { var d = Vector3.Distance(s.Unit.transform.position, other.Unit.transform.position); if (d < nearest) { nearest = d; target = other; } }
                if (target == null) continue;
                if (nearest <= s.Range && (!IncludesHero||ClearLine(s,target)))
                {
                    if(IncludesHero)s.Unit.Stop();
                    if(!s.Guarding&&Time.time>=s.AttackAt)Strike(s,target);
                }
                else if ((nearest > s.Range || (IncludesHero&&!ClearLine(s,target))) && Time.time >= s.OrderedUntil) s.Unit.TryMoveTo(target.Unit.transform.position);
            }
            if (Allies == 0 || Enemies == 0)
            {
                Active = false;
                session.State.Army.RemoveAll(s=>s.Health<=0);
                session.State.ArmySelection.RemoveAll(id=>session.State.Army.All(s=>s.Id!=id));
                foreach (var soldier in soldiers) if (soldier.Unit != null && soldier.Health > 0) soldier.Unit.Stop();
                if (Won && !session.HasFact("rts-victory")) session.State.Facts.Add("rts-victory");
                session.Say(Won ? "Victoria RTS registrada en la partida." : "Derrota RTS; puedes reiniciar la prueba.");
                Resolved?.Invoke(Won);
            }
        }
        private static bool ClearLine(Soldier source,Soldier target)
        {
            return !Physics.Linecast(source.Unit.transform.position+Vector3.up*.9f,target.Unit.transform.position+Vector3.up*.9f,out var hit,~0,QueryTriggerInteraction.Ignore)||hit.transform.IsChildOf(target.Unit.transform);
        }
        private Soldier Nearest(Soldier source)
        {Soldier best=null;var distance=float.MaxValue;foreach(var s in soldiers)if(s.Team!=source.Team&&Health(s)>0){var d=Vector3.Distance(source.Unit.transform.position,s.Unit.transform.position);if(d<distance){distance=d;best=s;}}return best;}
        private void Strike(Soldier source,Soldier target)
        {
            var before=Health(target);var damage=source.Hero?HeroActionRules.AttackDamage(session.Rules.Find(session.State.ClassId),HeroActionRules.WeaponBonus(session.State)):source.Damage;
            var guarded=target.Guarding;var actual=Mathf.Min(before,HeroActionRules.GuardedDamage(damage,guarded));SetHealth(target,before-actual);source.AttackAt=Time.time+1;
            if(IncludesHero){source.Unit.Stop();source.Guarding=false;source.Animator?.SetGuarding(false);source.Animator?.TryPlay(CharacterMotion.Attack);target.Animator?.TryPlay(before-actual<=0?CharacterMotion.Defeat:CharacterMotion.Hit);Emit(source,new CombatResolution(++sequence,source.Team==0?CombatSide.Player:CombatSide.Enemy,CombatActionKind.Attack,hit:true,damage:actual,before:before,after:before-actual,guarded:guarded),target.Unit.transform.position);}
            target.Guarding=false;target.Animator?.SetGuarding(false);
            if(Health(target)<=0)
            {
                if(target.Visual!=null){var model=target.Visual;model.transform.SetParent(null,true);if(target.Animator!=null){var a=target.Animator.Rig.Animator;target.Animator.enabled=false;a.enabled=true;a.SetBool("Defeated",true);}model.AddComponent<CombatTransient>().Configure(1.4f);target.Visual=null;}
                if(!target.Hero)target.Unit.gameObject.SetActive(false);else session.Say("Tu héroe ha caído; la patrulla aún combate.");
            }
        }
        private void Emit(Soldier source,CombatResolution result,Vector3 target)
        {var classId=source.Hero?session.State.ClassId:source.Role=="archer"?"archer":source.Role=="brawler"?"monk":"warrior";var data=new CombatFeedback(-10,result,classId,source.Unit.transform.position,target);feedback?.PresentRealtime(data);ActionResolved?.Invoke(data);}
        public bool HeroAction(string action)
        {
            var hero=soldiers.Find(s=>s.Hero);if(!Active||hero==null||!hero.Unit.IsSelected||Health(hero)<=0||Time.time<hero.AttackAt)return false;
            if(action=="attack")
            {var target=hero.OrderedTarget!=null&&Health(hero.OrderedTarget)>0?hero.OrderedTarget:Nearest(hero);if(target==null||Vector3.Distance(hero.Unit.transform.position,target.Unit.transform.position)>hero.Range||!ClearLine(hero,target))return false;Strike(hero,target);return true;}
            if(action=="guard")
            {hero.Guarding=true;hero.AttackAt=Time.time+1;hero.Animator?.SetGuarding(true);Emit(hero,new CombatResolution(++sequence,CombatSide.Player,CombatActionKind.Guard),hero.Unit.transform.position);return true;}
            if(action!="recover"||heroRecoveryUsed)return false;var recovered=HeroActionRules.Recovery(Health(hero),Maximum(hero));if(recovered<=0)return false;
            var before=Health(hero);SetHealth(hero,before+recovered);heroRecoveryUsed=true;hero.AttackAt=Time.time+1;Emit(hero,new CombatResolution(++sequence,CombatSide.Player,CombatActionKind.Recover,healing:recovered,before:before,after:before+recovered),hero.Unit.transform.position);return true;
        }
    }
}
