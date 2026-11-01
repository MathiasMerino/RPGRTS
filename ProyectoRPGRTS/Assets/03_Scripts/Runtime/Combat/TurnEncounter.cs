using System;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public enum EncounterCue { Reset, Attack, Guard, GuardEnded, Hit, Defeat, Completed }
    public readonly struct EncounterPresentation
    {
        public readonly EncounterCue Cue;
        public readonly Vector3 Target;
        public EncounterPresentation(EncounterCue cue, Vector3 target) { Cue = cue; Target = target; }
    }
    public sealed class TurnEncounter : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private ExplorationUnit hero;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Material enemyMaterial;
        private ExplorationUnit enemy;
        private bool resolutionSent;
        public event Action<bool> Resolved;
        public event Action<EncounterPresentation> Presentation;
        private void Show(EncounterCue cue) { Presentation?.Invoke(new EncounterPresentation(cue, EnemyPosition)); }
        public TurnBattle Battle { get; private set; }
        public Vector3 EnemyPosition => enemy != null ? enemy.transform.position : hero.transform.position;
        public void Configure(GameSession game, ExplorationUnit actor, GameObject template, Material material)
        { session = game; hero = actor; enemyPrefab = template; enemyMaterial = material; }
        public void Clear(bool preserveAcceptedStrike = false)
        {
            if (enemy != null) { enemy.gameObject.SetActive(false); Destroy(enemy.gameObject); }
            enemy = null; Battle = null;
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
            if (!NavMesh.SamplePosition(requested, out var hit, 5, NavMesh.AllAreas)) return false;
            enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity).GetComponent<ExplorationUnit>();
            enemy.name = "RPG Enemy";
            foreach (var renderer in enemy.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = enemyMaterial;
            Battle = new TurnBattle(session.Rules.Find(session.State.ClassId), session.State.Health,
                session.State.Weapon == "training-sword" ? 1 : 0, session.State.Armour == "cloth" ? 1 : 0, seed == 0 ? Environment.TickCount : seed,
                Vector3.Distance(hero.transform.position, EnemyPosition) <= 1.9f && HasClearAttackLine(enemy, hero));
            session.SetMode(PrototypeMode.TurnCombat);
            if (Battle.HeroHealth < session.State.Health) Show(Battle.HeroHealth <= 0 ? EncounterCue.Defeat : EncounterCue.Hit);
            Sync(); return true;
        }
        private bool Ready()
        {
            if (Battle == null || Battle.Finished) return false;
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
            if (!Ready()) return;
            if (!HasClearAttackLine(hero, enemy)) { session.Say("El objetivo está bloqueado por un obstáculo. Busca una línea de ataque libre."); return; }
            if (Battle.Attack(Vector3.Distance(hero.transform.position, EnemyPosition))) Show(EncounterCue.Attack);
            Sync();
        }
        public void Guard() { if (Ready()) { if (Battle.Guard()) Show(EncounterCue.Guard); Sync(); } }
        public void Recover() { if (Ready()) { Battle.Recover(); Sync(); } }
        public bool Move(Vector3 point)
        {
            if (!Ready() || !hero.TryMoveTo(point, Battle.MovementRemaining)) return false;
            var accepted = Battle.Move(hero.LastPathLength); Sync(); return accepted;
        }
        public void Approach()
        {
            var delta = EnemyPosition - hero.transform.position; delta.y = 0;
            Move(hero.transform.position + delta.normalized * Mathf.Min(Mathf.Max(0, delta.magnitude - 1.2f), 3.8f));
            Sync();
        }
        public void EndTurn()
        {
            if (!Ready()) return;
            var delta = hero.transform.position - EnemyPosition; delta.y = 0;
            if (delta.magnitude > 1.8f || !HasClearAttackLine(enemy, hero)) enemy.TryStepTowards(hero.transform.position, 3, 1.3f);
            var before = Battle.HeroHealth;
            Battle.EndTurn(Vector3.Distance(hero.transform.position, EnemyPosition) <= 1.9f && HasClearAttackLine(enemy, hero));
            Show(EncounterCue.GuardEnded);
            if (Battle.HeroHealth < before) Show(Battle.HeroHealth <= 0 ? EncounterCue.Defeat : EncounterCue.Hit);
            Sync();
        }
        private void Sync()
        {
            session.State.Health = Battle.HeroHealth;
            if (Battle.Won && !session.HasFact("rpg-victory")) session.State.Facts.Add("rpg-victory");
            if (Battle.Finished) { hero.Stop(); if (enemy != null) enemy.gameObject.SetActive(false); }
            session.Say(Battle.Finished ? (Battle.Won ? "Victoria RPG. Resultado registrado; puedes explorar o probar la escalada RTS." : "Derrota RPG. Vuelve a Explorar y elige una clase para reiniciar.") : Battle.Message);
            if(Battle.Finished&&!resolutionSent){resolutionSent=true;Resolved?.Invoke(Battle.Won);}
        }
    }
}
