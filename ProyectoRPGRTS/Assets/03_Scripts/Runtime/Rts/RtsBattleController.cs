using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed partial class RtsBattleController : MonoBehaviour
    {
        private sealed class Soldier
        {
            public ExplorationUnit Unit;
            public int Team;
            public int Health = 10;
            public float AttackAt,SkillAt;
            public float OrderedUntil;
            public string Role="swordsman";
            public string RecruitId;
            public float Range=1.5f;
            public int Damage=2,Maximum=10;
            public bool Hero,Guarding,MoveOrder;
            public Soldier OrderedTarget;
            public TacticalOrder Order;
            public RtsStance Stance = RtsStance.Defensive;
            public Vector3 Post;
            public Vector3 Destination;
            public float RepathAt;
            public bool Deployed;
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
        private int techniqueFocus;
        public int TechniqueFocus=>IncludesHero?techniqueFocus:0;
        private int sequence;
        public bool IncludesHero=>sharedHero!=null;
        private bool PatrolCombat=>IncludesHero&&ContextId=="patrol";
        public RtsDemoRules DemoRules=>demoRules;
        public GameObject UnitTemplate=>template;
        public string ContextId { get; private set; }="patrol";
        public event Action<bool> Resolved;
        public event Action<CombatFeedback> ActionResolved;
        // Sin suscriptores no se construyen muestras ni se guardan archivos.
        public event Action<RtsTraceSample> Diagnostic;
        private int recoveryTraceFrame=-1;
        private void Trace(string kind,Soldier actor=null,Soldier target=null,int value=0)
        {
            var listener=Diagnostic;if(listener==null)return;
            if(kind=="recover-cooldown"){if(recoveryTraceFrame==Time.frameCount)return;recoveryTraceFrame=Time.frameCount;}
            listener(new RtsTraceSample(kind,Time.frameCount,Time.time,Time.deltaTime,actor==null?-1:soldiers.IndexOf(actor),actor!=null&&actor.Hero,actor==null?0:Health(actor),actor?.AttackAt??0,actor!=null&&actor.Unit.IsSelected,heroRecoveryUsed,target==null?-1:soldiers.IndexOf(target),target==null?0:Health(target),value,actor==null?Vector3.zero:actor.Unit.transform.position,Allies,Enemies));
        }
        private readonly List<Soldier> soldiers = new List<Soldier>();
        private float nextTick;
        public bool Active { get; private set; }
        // Sobrevive a Clear() invocado por la transición; se reinicia al aceptar otra batalla.
        public bool? LastResolvedVictory { get; private set; }
        public int StartingAllyHealth { get; private set; }
        public int Allies => soldiers.Count(s => s.Team == 0 && Health(s) > 0);
        public int Enemies => soldiers.Count(s => s.Team == 1 && Health(s) > 0);
        public int SelectedAllies => soldiers.Count(s => s.Team == 0 && Health(s) > 0 && s.Unit != null && s.Unit.IsSelected);
        public bool Won => !Active && Allies > 0 && Enemies == 0 && soldiers.Count > 0 && (!IncludesHero||session.State.Health>0);
        public void Configure(GameSession game, GameObject pawn, Material material) { session = game; template = pawn; hostile = material; }
        public void ConfigureDemo(RtsDemoRules rules,CharacterVisualCatalog models,CombatFeedbackPresenter effects){demoRules=rules;visuals=models;feedback=effects;}
        private int Health(Soldier s)=>s.Hero?session.State.Health:s.Health;
        private int Maximum(Soldier s)=>s.Hero?session.HeroProfile.Health:s.Maximum;
        private void SetHealth(Soldier s,int value)
        {if(s.Hero)session.State.Health=value;else{s.Health=value;if(!string.IsNullOrEmpty(s.RecruitId)){var record=session.State.Army.Find(x=>x.Id==s.RecruitId);if(record!=null)record.Health=value;}}}
        public void CopyStatus(List<RtsUnitStatus> buffer){buffer.Clear();foreach(var s in soldiers)if(!Preparing||s.Team==0||s.Unit.gameObject.activeSelf)buffer.Add(new RtsUnitStatus(s.Unit,s.Hero?session.HeroProfile.Title:demoRules?.Find(s.Role)?.Title??s.Role,s.Team,Health(s),Maximum(s),s.Hero));}
        public void Clear()
        {
            Trace("clear");recoveryTraceFrame=-1;
            foreach (var soldier in soldiers)
                if (soldier.Unit != null)
                {
                    if(soldier.Hero){soldier.Unit.Stop();var p=soldier.Unit.GetComponent<CharacterCombatAnimationPresenter>();if(p!=null)p.ExternallyControlled=false;soldier.Animator?.SetGuarding(false);}
                    else{soldier.Unit.gameObject.SetActive(false);Destroy(soldier.Unit.gameObject);}
                }
            soldiers.Clear();sharedHero=null; Active = false;Preparing=false;ResetTactics();
        }
        public bool Begin(bool escalated = false)
        {
            if (Active) return false;
            // Validate the existing world before creating any participant or changing mode.
            var positions = new Vector3[12];
            var path = new NavMeshPath();
            for (var team = 0; team < 2; team++) for (var i = 0; i < 6; i++)
            {
                var requested = ArenaOffset+new Vector3(team == 0 ? -12 : (session.Rules.UxExperience?6:-6), 0, 11 + i * 0.9f);
                if (!NavMesh.SamplePosition(requested, out var hit, 0.35f, NavMesh.AllAreas)) return RejectDeployment();
                positions[team * 6 + i] = hit.position;
                foreach (var blocker in Physics.OverlapCapsule(hit.position + Vector3.up * 0.35f, hit.position + Vector3.up * 1.45f, 0.29f, ~0, QueryTriggerInteraction.Ignore))
                    if (blocker.GetComponentInParent<ExplorationUnit>() == null) return RejectDeployment();
                if (!NavMesh.CalculatePath(positions[0], hit.position, NavMesh.AllAreas, path) || path.status != NavMeshPathStatus.PathComplete) return RejectDeployment();
            }
            Clear();
            StartingAllyHealth = escalated ? 14 : 10; nextTick = 0;LastResolvedVictory=null;
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
                if(!NavMesh.SamplePosition(ArenaOffset+new Vector3(-10.5f,0,11+i*.9f),out var point,.35f,NavMesh.AllAreas))return RejectDeployment();
                foreach(var block in Physics.OverlapCapsule(point.position+Vector3.up*.35f,point.position+Vector3.up*1.45f,.29f,~0,QueryTriggerInteraction.Ignore))if(block.GetComponentInParent<ExplorationUnit>()==null)return RejectDeployment();
                reinforcements.Add(point.position);
            }
            var requested=ArenaOffset+new Vector3(-13.3f,0,13.2f);
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
                var formation=expedition?demoRules.Civilizations?.ConquestFormation:null;var role=formation!=null&&formation.Length>0?demoRules.Find(formation[index%formation.Length]):demoRules.Roles[index%demoRules.Roles.Length];index++;if(role==null){Clear();session.SetMode(PrototypeMode.Exploration);return false;}s.Role=role.Id;s.Range=role.Range;s.Damage=role.Damage;s.Maximum=role.Health;s.Health=role.Health;s.Unit.Agent.speed=role.Speed;
                BuildVisual(s,role);
            }
            for(var i=0;i<roster.Count;i++)
            {
                var record=roster[i];var role=demoRules.Find(record.Role);if(role==null)continue;
                var unit=Instantiate(template,reinforcements[i],Quaternion.identity).GetComponent<ExplorationUnit>();unit.name="Recluta "+role.Title;unit.Agent.speed=role.Speed;unit.Select(true);
                var s=new Soldier{Unit=unit,Team=0,Health=record.Health,Maximum=TroopProgression.Profile(record,role).Health,Damage=TroopProgression.Profile(record,role).Damage,Range=role.Range,Role=role.Id,RecruitId=record.Id};BuildVisual(s,role);soldiers.Add(s);
            }
            hero.Stop();if(!hero.Warp(at.position)){Clear();session.SetMode(PrototypeMode.Exploration);return false;}
            sharedHero=hero;heroRecoveryUsed=false;sequence=0;hero.Select(true);
            var profile=session.HeroProfile;techniqueFocus=profile.Combat?.FocusCapacity??0;var actor=hero.GetComponent<CharacterVisualPresenter>();
            var h=new Soldier{Unit=hero,Team=0,Hero=true,Role="hero",Range=profile.Range,Animator=actor.Model==null?null:actor.Model.GetComponent<HumanoidAnimationDriver>()};soldiers.Add(h);
            hero.GetComponent<CharacterCombatAnimationPresenter>().ExternallyControlled=true;
            if(TacticalBattle)OpenTacticalDeployment();
            Trace("begin",h);
            if(TacticalBattle)
            {
                session.Say("Defensa del paso fronterizo · despliega tropas y confirma la ofensiva.");
                return true;
            }
            session.Say((expedition?"Conquista del puesto.":"La patrulla necesita tu mando.")+" Arrastra para seleccionar; clic derecho para mover o atacar. Tu héroe conserva vida y equipo.");return true;
        }
        private void BuildVisual(Soldier s,RtsRoleProfile role)
        {
            var model=SupportVisualFactory.Attach(s.Unit,role,visuals,feedback==null?null:feedback.SharedEffectMaterial,demoRules.Civilizations,string.IsNullOrEmpty(s.RecruitId)?null:session.State.Army.Find(r=>r.Id==s.RecruitId)?.RaceId);if(model==null)return;s.Visual=model.gameObject;s.Animator=s.Visual.GetComponent<HumanoidAnimationDriver>();
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
            if(!Active||(!TacticalBattle&&collider==null))return false;var selected=collider==null?null:collider.GetComponentInParent<ExplorationUnit>();var found=false;
            foreach(var s in soldiers)if(s.Team==0&&Health(s)>0){var match=s.Unit==selected;found|=match;s.Unit.Select(match||(additive&&s.Unit.IsSelected));}return found;
        }
        public void RememberGroup(List<ExplorationUnit> group){group.Clear();foreach(var s in soldiers)if(s.Team==0&&Health(s)>0&&s.Unit.IsSelected)group.Add(s.Unit);}
        public void SelectGroup(List<ExplorationUnit> group,bool additive){group.RemoveAll(unit=>unit==null||!soldiers.Exists(s=>s.Unit==unit&&s.Team==0&&Health(s)>0));foreach(var s in soldiers)if(s.Team==0&&s.Unit!=null)s.Unit.Select(Health(s)>0&&(group.Contains(s.Unit)||(additive&&s.Unit.IsSelected)));}
        public bool Command(Vector3 point, Collider collider, bool primary)
        {
            if (!Active) return false;
            if(TacticalBattle&&!primary)return TacticalCommand(point,collider,false);
            if(session.Rules.WorldExploration&&!primary)return WorldCommand(point,collider);
            if (primary)
            {
                return SelectUnit(collider,false);
            }
            var accepted = false; var index = 0;
            var explicitTarget=collider==null?null:soldiers.Find(s=>s.Team==1&&Health(s)>0&&collider.transform.IsChildOf(s.Unit.transform));
            if(Preparing&&explicitTarget!=null&&!EngagePrepared())return false;
            foreach (var s in soldiers)
                if (s.Team == 0 && Health(s) > 0 && s.Unit.IsSelected)
                {
                    if (PatrolCombat && explicitTarget != null)
                    {
                        OrderPatrolAttack(s, explicitTarget);
                        accepted = true;
                    }
                    else
                    {
                        s.OrderedTarget = explicitTarget;
                        s.MoveOrder = IncludesHero && explicitTarget == null;
                        accepted |= s.Unit.TryMoveTo(point + new Vector3((index % 3 - 1) * .8f, 0, index++ / 3 * .8f));
                        s.OrderedUntil = Time.time + 3;
                    }
                    Trace("command", s, explicitTarget);
                }
            return accepted;
        }
        private void Update()
        {
            if (!Active || session.Paused || Time.time < nextTick) return;
            if(Preparing){TickPreparation();return;}
            if(TacticalBattle){TickTactics();return;}
            nextTick = Time.time + 0.2f;
            if(Diagnostic!=null)Trace("tick-start",soldiers.Find(s=>s.Hero));
            foreach (var s in soldiers)
            {
                if (Health(s) <= 0) continue;
                if (PatrolCombat)
                {
                    TickPatrolSoldier(s);
                    continue;
                }
                if(IncludesHero&&s.MoveOrder&&s.Unit.IsMoving&&(session.Rules.UxExperience||Time.time<s.OrderedUntil))continue;s.MoveOrder=false;
                Soldier target = s.OrderedTarget!=null&&Health(s.OrderedTarget)>0?s.OrderedTarget:null;var ordered=target!=null;var nearest=target==null?float.MaxValue:Vector3.Distance(s.Unit.transform.position,target.Unit.transform.position);
                foreach (var other in soldiers)
                    if (!ordered&&Health(other) > 0 && other.Team != s.Team)
                    { var d = Vector3.Distance(s.Unit.transform.position, other.Unit.transform.position); if (d < nearest) { nearest = d; target = other; } }
                if (target == null||(session.Rules.UxExperience&&!ordered&&nearest>8)) continue;
                if (nearest <= s.Range && (!IncludesHero||ClearLine(s,target)))
                {
                    if(IncludesHero)s.Unit.Stop();
                    if(!s.Guarding&&Time.time>=s.AttackAt)Strike(s,target);
                }
                else if ((nearest > s.Range || (IncludesHero&&!ClearLine(s,target))) && Time.time >= s.OrderedUntil) s.Unit.TryMoveTo(session.Rules.WorldExploration?AttackPosition(s,target):target.Unit.transform.position);
            }
            if (Allies == 0 || Enemies == 0) FinishBattle();
        }
        private void FinishBattle()
        {
            Active = false;
            session.State.Army.RemoveAll(s=>s.Health<=0);
            session.State.ArmySelection.RemoveAll(id=>session.State.Army.All(s=>s.Id!=id));
            foreach (var soldier in soldiers) if (soldier.Unit != null && soldier.Health > 0) soldier.Unit.Stop();
            var victory=Won;LastResolvedVictory=victory;
            if (victory && !session.HasFact("rts-victory")) session.State.Facts.Add("rts-victory");
            session.Say(victory ? "Victoria RTS registrada en la partida." : "Derrota RTS; puedes reiniciar la prueba.");
            Trace(victory?"victory":"defeat",soldiers.Find(s=>s.Hero));
            Resolved?.Invoke(victory);
        }
        private bool ClearLine(Soldier source,Soldier target)
        {
            if(TacticalBattle||PatrolCombat)return TacticalLine(source,target);
            return !Physics.Linecast(source.Unit.transform.position+Vector3.up*.9f,target.Unit.transform.position+Vector3.up*.9f,out var hit,~0,QueryTriggerInteraction.Ignore)||hit.transform.IsChildOf(target.Unit.transform);
        }
        private Soldier Nearest(Soldier source)
        {Soldier best=null;var distance=float.MaxValue;foreach(var s in soldiers)if(s.Team!=source.Team&&Health(s)>0){var d=Vector3.Distance(source.Unit.transform.position,s.Unit.transform.position);if(d<distance){distance=d;best=s;}}return best;}
        private void Strike(Soldier source,Soldier target,int techniqueBonus=0)
        {
            var before=Health(target);var damage=source.Hero?HeroActionRules.AttackDamage(session.HeroProfile,HeroActionRules.WeaponBonus(session.State,session.Rules.Progression?.Items)):source.Damage;damage+=techniqueBonus;
            if(target.Hero&&session.State.ProgressionRevision==1&&EquipmentOperations.ArmourBonus(session.State,session.Rules.Progression?.Items)>=2)damage=Math.Max(1,damage-1);
            var guarded=target.Guarding;var actual=Mathf.Min(before,HeroActionRules.GuardedDamage(damage,guarded,target.Hero?HeroActionRules.GuardReduction(session.HeroProfile):1));SetHealth(target,before-actual);source.AttackAt=Time.time+1;Trace("strike",source,target,actual);
            if(IncludesHero){source.Unit.Stop();source.Guarding=false;source.Animator?.SetGuarding(false);source.Animator?.TryPlay(CharacterMotion.Attack);target.Animator?.TryPlay(before-actual<=0?CharacterMotion.Defeat:CharacterMotion.Hit);Emit(source,new CombatResolution(++sequence,source.Team==0?CombatSide.Player:CombatSide.Enemy,CombatActionKind.Attack,hit:true,damage:actual,before:before,after:before-actual,guarded:guarded),target.Unit.transform.position);}
            target.Guarding=false;target.Animator?.SetGuarding(false);
            if(Health(target)<=0)
            {
                if(target.Visual!=null){var model=target.Visual;model.transform.SetParent(null,true);if(target.Animator!=null){var a=target.Animator.Rig.Animator;target.Animator.enabled=false;a.enabled=true;a.SetBool("Defeated",true);}model.AddComponent<CombatTransient>().Configure(1.4f);target.Visual=null;}
                if(!target.Hero){target.Unit.Select(false);target.Unit.gameObject.SetActive(false);}else{target.Unit.Stop();target.Unit.Select(false);session.Say(TacticalBattle?"Tu héroe ha caído.":"Tu héroe ha caído; la patrulla aún combate.");}
            }
        }
        private void Emit(Soldier source,CombatResolution result,Vector3 target)
        {var classId=source.Hero?session.State.ClassId:source.Role=="archer"?"archer":source.Role=="brawler"?"monk":"warrior";var data=new CombatFeedback(-10,result,classId,source.Unit.transform.position,target);feedback?.PresentRealtime(data);ActionResolved?.Invoke(data);}
        public bool HeroAction(string action)
        {
            if(Preparing||(TacticalBattle&&(TacticalDeployment||session.Paused||session.WorldUiOpen)))return false;
            var hero=soldiers.Find(s=>s.Hero);
            if(!Active||hero==null||!hero.Unit.IsSelected||Health(hero)<=0){if(action=="recover")Trace("recover-unavailable",hero);return false;}
            if(Time.time<hero.AttackAt&&!(session.Rules.LivingWorld&&action=="technique")){if(action=="recover")Trace("recover-cooldown",hero);return false;}
            if(action=="technique")
            {
                var skill=session.HeroProfile.Combat;var target=HeroTarget(hero,skill?.TechniqueRange??0,session.Rules.LivingWorld);
                if(skill==null||!skill.TechniqueUnlocked||techniqueFocus<skill.TechniqueCost||Time.time<hero.SkillAt||target==null||Vector3.Distance(hero.Unit.transform.position,target.Unit.transform.position)>skill.TechniqueRange||!ClearLine(hero,target))return false;
                techniqueFocus-=skill.TechniqueCost;hero.SkillAt=Time.time+skill.TechniqueCooldown;Strike(hero,target,skill.TechniqueDamage+(session.State.ClassId=="monk"?2:0));session.Say(skill.TechniqueTitle+" · foco "+techniqueFocus);return true;
            }
            if(action=="attack")
            {var target=HeroTarget(hero,hero.Range);if(target==null||Vector3.Distance(hero.Unit.transform.position,target.Unit.transform.position)>hero.Range||!ClearLine(hero,target))return false;Strike(hero,target);return true;}
            if(action=="guard")
            {if(TacticalBattle)SetPost(hero,TacticalOrder.Hold);hero.Guarding=true;hero.AttackAt=Time.time+1;hero.Animator?.SetGuarding(true);Emit(hero,new CombatResolution(++sequence,CombatSide.Player,CombatActionKind.Guard),hero.Unit.transform.position);return true;}
            if(action!="recover"||heroRecoveryUsed)return false;var recovered=HeroActionRules.Recovery(Health(hero),Maximum(hero));if(recovered<=0)return false;
            var before=Health(hero);SetHealth(hero,before+recovered);heroRecoveryUsed=true;hero.AttackAt=Time.time+1;Trace("recover-accepted",hero,value:recovered);Emit(hero,new CombatResolution(++sequence,CombatSide.Player,CombatActionKind.Recover,healing:recovered,before:before,after:before+recovered),hero.Unit.transform.position);return true;
        }
    }
}
