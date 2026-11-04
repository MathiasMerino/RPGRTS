using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public enum EncounterCue { Reset, Attack, Guard, GuardEnded, Hit, Defeat, Completed, Recover }
    public readonly struct EncounterPresentation
    {
        public readonly EncounterCue Cue;
        public readonly Vector3 Target;
        public readonly CombatFeedback Feedback;
        public EncounterPresentation(EncounterCue cue, Vector3 target, CombatFeedback feedback=default) { Cue = cue; Target = target; Feedback=feedback; }
    }
    public sealed partial class TurnEncounter : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private ExplorationUnit hero;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Material enemyMaterial;
        [SerializeField] private bool stagedCommands;
        [SerializeField] private GameObject enemyVisualPrefab;
        private ExplorationUnit enemy;
        private bool resolutionSent;
        private Coroutine enemyRoutine;
        private int encounterId;
        private int publishedSequence;
        public event Action<bool> Resolved;
        public event Action<EncounterPresentation> Presentation;
        public event Action<CombatFeedback> ActionResolved;
        public event Action<Vector3,Vector3> EnemyMoved;
        public event Action<ExplorationUnit,GameObject> EnemySpawned;
        public event Action EnemyPreparingAttack;
        public EnemyIntent LastEnemyIntent { get; private set; }
        public bool TargetSelected { get; private set; }
        public bool PlayerCanAct => Battle!=null&&!Battle.Finished&&Battle.Phase==TurnPhase.Player&&!hero.IsMoving;
        public bool UsesStagedCommands => stagedCommands;
        public ExplorationUnit Enemy => enemy;
        private CombatFeedback Feedback(CombatResolution result)
        {var from=result.Actor==CombatSide.Player?hero.transform.position:EnemyPosition;var to=result.Actor==CombatSide.Player?EnemyPosition:hero.transform.position;return new CombatFeedback(encounterId,result,session.State.ClassId,from,to);}
        private void Publish(CombatResolution result)
        {if(!result.Accepted||result.Sequence<=publishedSequence)return;publishedSequence=result.Sequence;ActionResolved?.Invoke(Feedback(result));}
        private void Show(EncounterCue cue,CombatResolution result=default) { Presentation?.Invoke(new EncounterPresentation(cue, EnemyPosition,Feedback(result))); }
        public TurnBattle Battle { get; private set; }
        public Vector3 EnemyPosition => enemy != null ? enemy.transform.position : hero.transform.position;
        public void Configure(GameSession game, ExplorationUnit actor, GameObject template, Material material)
        { session = game; hero = actor; enemyPrefab = template; enemyMaterial = material; }
        public void ConfigureTactics(bool staged,GameObject visual){stagedCommands=staged;enemyVisualPrefab=visual;}
        public void Clear(bool preserveAcceptedStrike = false)
        {
            ClearGroup();
            if(enemyRoutine!=null){StopCoroutine(enemyRoutine);enemyRoutine=null;}
            if (enemy != null) { enemy.gameObject.SetActive(false); Destroy(enemy.gameObject); }
            enemy = null; Battle = null;TargetSelected=false;
            Show(preserveAcceptedStrike ? EncounterCue.Completed : EncounterCue.Reset);
        }
        public bool Begin(int seed = 0)
        { return BeginAt(hero.transform.position+Vector3.forward*3,seed); }
        public bool BeginAt(Vector3 requested,int seed=0)
        {
            if (Battle != null && !Battle.Finished) return false;
            if (session.State.Health <= 0) { session.Say("Elige una clase para reiniciar la prueba del personaje."); return false; }
            hero.Stop(); Clear();
            resolutionSent=false;
            encounterId++;publishedSequence=0;
            if (!NavMesh.SamplePosition(requested, out var hit, 5, NavMesh.AllAreas)) return false;
            enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity).GetComponent<ExplorationUnit>();
            enemy.name = "RPG Enemy";
            foreach (var renderer in enemy.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = enemyMaterial;
            TargetSelected=!session.Rules.UxExperience;EnemySpawned?.Invoke(enemy,enemyVisualPrefab);
            Battle = new TurnBattle(session.HeroProfile, session.State.Health,
                HeroActionRules.WeaponBonus(session.State,session.Rules.Progression?.Items), EquipmentOperations.ArmourBonus(session.State,session.Rules.Progression?.Items), seed == 0 ? Environment.TickCount : seed,
                Vector3.Distance(hero.transform.position, EnemyPosition) <= 1.9f && HasClearAttackLine(enemy, hero));
            session.SetMode(PrototypeMode.TurnCombat);
            Publish(Battle.LastEnemyResolution);
            if (Battle.HeroHealth < session.State.Health) Show(Battle.HeroHealth <= 0 ? EncounterCue.Defeat : EncounterCue.Hit,Battle.LastEnemyResolution);
            Sync(); return true;
        }
        private bool Ready()
        {
            if (Battle == null || Battle.Finished) return false;
            if (Battle.Phase!=TurnPhase.Player){session.Say("Espera: está actuando el enemigo.");return false;}
            if (!hero.IsMoving) return true;
            session.Say("Espera a que el personaje termine su movimiento."); return false;
        }
        private static bool HasClearAttackLine(ExplorationUnit source, ExplorationUnit target)
        {
            var start = source.transform.position + Vector3.up * 0.9f;
            var end = target.transform.position + Vector3.up * 0.9f;
            return !Physics.Linecast(start, end, out var hit, ~0, QueryTriggerInteraction.Ignore) || hit.transform.IsChildOf(target.transform);
        }
        public void Attack()
        {
            if(InGroup){GroupAttack();return;}
            if (!Ready()) return;
            if (!HasClearAttackLine(hero, enemy)) { session.Say("El objetivo está bloqueado por un obstáculo. Busca una línea de ataque libre."); return; }
            if(!TargetSelected){session.Say("Selecciona un objetivo válido.");return;}
            if (Battle.Attack(Vector3.Distance(hero.transform.position, EnemyPosition))) {Show(EncounterCue.Attack,Battle.LastHeroResolution);Publish(Battle.LastHeroResolution);}
            Sync();
        }
        public void Guard() { if(InGroup){GroupGuard();return;}if (Ready()) { if (Battle.Guard()) {Show(EncounterCue.Guard,Battle.LastHeroResolution);Publish(Battle.LastHeroResolution);} Sync(); } }
        public void Recover() { if(InGroup){GroupRecover();return;}if (Ready()) { if(Battle.Recover()){Show(EncounterCue.Recover,Battle.LastHeroResolution);Publish(Battle.LastHeroResolution);} Sync(); } }
        public bool TrySelect(Collider target)
        {
            if(InGroup)return GroupSelect(target);
            if(Battle==null||Battle.Finished||enemy==null||target==null||!target.transform.IsChildOf(enemy.transform))return false;
            TargetSelected=true;session.Say("Objetivo seleccionado: merodeador · "+Battle.EnemyHealth+" / 18 PV.");return true;
        }
        public bool Move(Vector3 point)
        {
            if(InGroup)return GroupMove(point);
            if (!Ready() || !hero.TryMoveTo(point, Battle.MovementRemaining)) return false;
            var accepted = Battle.Move(hero.LastPathLength); Sync(); return accepted;
        }
        public void Approach()
        {
            if(InGroup){GroupApproach();return;}
            var delta = EnemyPosition - hero.transform.position; delta.y = 0;
            Move(hero.transform.position + delta.normalized * Mathf.Min(Mathf.Max(0, delta.magnitude - 1.2f), 3.8f));
            Sync();
        }
        public void EndTurn()
        {
            if (!Ready()) return;
            if(!Battle.BeginEnemyTurn())return;
            DecideAndMoveEnemy();ResolveEnemy();
        }
        public bool RequestEndTurn()
        {
            if(InGroup)return GroupEnd();
            if(!stagedCommands){var ready=Ready();if(ready)EndTurn();return ready;}
            if(!Ready()||!Battle.BeginEnemyTurn())return false;
            session.Say("Turno del enemigo: evalúa posición y alcance.");enemyRoutine=StartCoroutine(EnemyTurnSequence());return true;
        }
        private IEnumerator EnemyTurnSequence()
        {
            var current=Battle;
            yield return new WaitForSeconds(.65f);
            if(Battle!=current||current==null)yield break;
            DecideAndMoveEnemy();session.Say(LastEnemyIntent==EnemyIntent.Approach?"El enemigo se aproxima por un camino accesible.":LastEnemyIntent==EnemyIntent.Attack?"El enemigo prepara su ataque.":"El enemigo no dispone de un ataque legal; espera.");
            yield return new WaitForSeconds(.35f);
            if(Battle!=current)yield break;
            if(Vector3.Distance(hero.transform.position,EnemyPosition)<=1.9f&&HasClearAttackLine(enemy,hero))EnemyPreparingAttack?.Invoke();
            yield return new WaitForSeconds(.45f);
            if(Battle==current)ResolveEnemy();enemyRoutine=null;
        }
        private void DecideAndMoveEnemy()
        {
            var distance=Vector3.Distance(hero.transform.position,EnemyPosition);var line=HasClearAttackLine(enemy,hero);
            var path=new NavMeshPath();var canApproach=(distance>1.8f||!line)&&enemy.Agent.CalculatePath(hero.transform.position,path)&&path.status==NavMeshPathStatus.PathComplete;
            LastEnemyIntent=MeleeEnemyPolicy.Decide(new EnemySituation(Battle.Phase==TurnPhase.Enemy,Battle.EnemyHealth>0,Battle.HeroHealth>0,distance<=1.8f,line,canApproach));
            if(LastEnemyIntent==EnemyIntent.Approach)
            {var before=EnemyPosition;if(enemy.TryStepTowards(hero.transform.position,3,1.3f))EnemyMoved?.Invoke(before,EnemyPosition);else LastEnemyIntent=EnemyIntent.Wait;}
        }
        private void ResolveEnemy()
        {
            var before=Battle.HeroHealth;
            if(!Battle.ResolveEnemyAction(Vector3.Distance(hero.transform.position,EnemyPosition)<=1.9f&&HasClearAttackLine(enemy,hero)))return;
            Publish(Battle.LastEnemyResolution);Battle.FinishEnemyTurn();
            Show(EncounterCue.GuardEnded);
            if (Battle.HeroHealth < before) Show(Battle.HeroHealth <= 0 ? EncounterCue.Defeat : EncounterCue.Hit,Battle.LastEnemyResolution);
            Sync();
        }
        private void OnDisable()
        {
            if(enemyRoutine!=null){StopCoroutine(enemyRoutine);enemyRoutine=null;}
            // Interrumpir una presentación no puede dejar una fase enemiga esperando un clip.
            if(Battle!=null&&Battle.Phase==TurnPhase.Enemy)
            {
                if(session!=null&&hero!=null&&enemy!=null&&enemy.Agent.enabled&&enemy.Agent.isOnNavMesh){DecideAndMoveEnemy();ResolveEnemy();}
                else{Battle.ResolveEnemyAction(false);Battle.FinishEnemyTurn();}
            }
        }
        private void Sync()
        {
            session.State.Health = Battle.HeroHealth;
            if (Battle.Won && !session.HasFact("rpg-victory")){session.State.Facts.Add("rpg-victory");session.AwardForFact("rpg-victory");}
            if (Battle.Finished) { hero.Stop(); if (enemy != null) enemy.gameObject.SetActive(false); }
            session.Say(Battle.Finished ? (Battle.Won ? "Victoria RPG. Resultado registrado; puedes explorar o probar la escalada RTS." : "Derrota RPG. Vuelve a Explorar y elige una clase para reiniciar.") : Battle.Message);
            if(Battle.Finished&&!resolutionSent){resolutionSent=true;Resolved?.Invoke(Battle.Won);}
        }
    }
}
