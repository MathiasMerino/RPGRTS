using System;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed class TurnEncounter : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private ExplorationUnit hero;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Material enemyMaterial;
        private ExplorationUnit enemy;
        public TurnBattle Battle { get; private set; }
        public Vector3 EnemyPosition => enemy != null ? enemy.transform.position : hero.transform.position;
        public void Configure(GameSession game, ExplorationUnit actor, GameObject template, Material material)
        { session = game; hero = actor; enemyPrefab = template; enemyMaterial = material; }
        public void Clear()
        {
            if (enemy != null) { enemy.gameObject.SetActive(false); Destroy(enemy.gameObject); }
            enemy = null; Battle = null;
        }
        public bool Begin(int seed = 0)
        {
            if (Battle != null && !Battle.Finished) return false;
            if (session.State.Health <= 0) { session.Say("Elige una clase para reiniciar la prueba del personaje."); return false; }
            hero.Stop(); Clear();
            var requested = hero.transform.position + Vector3.forward * 3;
            if (!NavMesh.SamplePosition(requested, out var hit, 5, NavMesh.AllAreas)) return false;
            enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity).GetComponent<ExplorationUnit>();
            enemy.name = "RPG Enemy";
            foreach (var renderer in enemy.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = enemyMaterial;
            Battle = new TurnBattle(session.Rules.Find(session.State.ClassId), session.State.Health,
                session.State.Weapon == "training-sword" ? 1 : 0, session.State.Armour == "cloth" ? 1 : 0, seed == 0 ? Environment.TickCount : seed,
                Vector3.Distance(hero.transform.position, EnemyPosition) <= 1.9f && HasClearAttackLine(enemy, hero));
            session.SetMode(PrototypeMode.TurnCombat); Sync(); return true;
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
            Battle.Attack(Vector3.Distance(hero.transform.position, EnemyPosition)); Sync();
        }
        public void Guard() { if (Ready()) { Battle.Guard(); Sync(); } }
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
            Battle.EndTurn(Vector3.Distance(hero.transform.position, EnemyPosition) <= 1.9f && HasClearAttackLine(enemy, hero)); Sync();
        }
        private void Sync()
        {
            session.State.Health = Battle.HeroHealth;
            if (Battle.Won && !session.HasFact("rpg-victory")) session.State.Facts.Add("rpg-victory");
            if (Battle.Finished) { hero.Stop(); if (enemy != null) enemy.gameObject.SetActive(false); }
            session.Say(Battle.Finished ? (Battle.Won ? "Victoria RPG. Resultado registrado; puedes explorar o probar la escalada RTS." : "Derrota RPG. Vuelve a Explorar y elige una clase para reiniciar.") : Battle.Message);
        }
    }
}
