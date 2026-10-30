using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        }
        [SerializeField] private GameSession session;
        [SerializeField] private GameObject template;
        [SerializeField] private Material hostile;
        private readonly List<Soldier> soldiers = new List<Soldier>();
        private float nextTick;
        public bool Active { get; private set; }
        public int StartingAllyHealth { get; private set; }
        public int Allies => soldiers.Count(s => s.Team == 0 && s.Health > 0);
        public int Enemies => soldiers.Count(s => s.Team == 1 && s.Health > 0);
        public bool Won => !Active && Allies > 0 && Enemies == 0 && soldiers.Count > 0;
        public void Configure(GameSession game, GameObject pawn, Material material) { session = game; template = pawn; hostile = material; }
        public void Begin(bool escalated = false)
        {
            StartingAllyHealth = escalated ? 14 : 10;
            foreach (var soldier in soldiers) if (soldier.Unit != null) Destroy(soldier.Unit.gameObject);
            soldiers.Clear();
            for (var team = 0; team < 2; team++) for (var i = 0; i < 6; i++)
            {
                var unit = Instantiate(template, new Vector3(team == 0 ? -12 : -6, 0, 11 + i * 0.9f), Quaternion.identity).GetComponent<ExplorationUnit>();
                unit.name = (team == 0 ? "A" : "B") + " Soldier " + i;
                unit.Agent.speed = 2.5f;
                unit.Select(team == 0);
                if (team == 1) foreach (var renderer in unit.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = hostile;
                soldiers.Add(new Soldier { Unit = unit, Team = team, Health = team == 0 && escalated ? 14 : 10 });
            }
            Active = true; session.SetMode(PrototypeMode.Rts);
            session.Say(escalated ? "Escalada experimental: la victoria RPG da +4 salud a cada aliado." : "RTS 6 contra 6. Clic derecho para ordenar; las unidades atacan al acercarse.");
        }
        public void SelectAll() { foreach (var s in soldiers) if (s.Unit != null && s.Team == 0 && s.Health > 0) s.Unit.Select(true); }
        public bool Command(Vector3 point, Collider collider, bool primary)
        {
            if (!Active) return false;
            if (primary)
            {
                var selected = collider.GetComponentInParent<ExplorationUnit>();
                foreach (var s in soldiers) if (s.Team == 0 && s.Unit != null) s.Unit.Select(s.Unit == selected);
                return selected != null;
            }
            var accepted = false; var index = 0;
            foreach (var s in soldiers)
                if (s.Team == 0 && s.Health > 0 && s.Unit.IsSelected)
                { accepted |= s.Unit.TryMoveTo(point + new Vector3((index % 3 - 1) * 0.8f, 0, index++ / 3 * 0.8f)); s.OrderedUntil = Time.time + 3; }
            return accepted;
        }
        private void Update()
        {
            if (!Active || Time.time < nextTick) return;
            nextTick = Time.time + 0.2f;
            foreach (var s in soldiers)
            {
                if (s.Health <= 0) continue;
                Soldier target = null; var nearest = float.MaxValue;
                foreach (var other in soldiers)
                    if (other.Health > 0 && other.Team != s.Team)
                    { var d = Vector3.Distance(s.Unit.transform.position, other.Unit.transform.position); if (d < nearest) { nearest = d; target = other; } }
                if (target == null) continue;
                if (nearest <= 1.5f && Time.time >= s.AttackAt)
                {
                    target.Health -= 2; s.AttackAt = Time.time + 1;
                    if (target.Health <= 0) target.Unit.gameObject.SetActive(false);
                }
                else if (nearest > 1.5f && Time.time >= s.OrderedUntil) s.Unit.TryMoveTo(target.Unit.transform.position);
            }
            if (Allies == 0 || Enemies == 0)
            {
                Active = false;
                if (Allies > 0 && !session.HasFact("rts-victory")) session.State.Facts.Add("rts-victory");
                session.Say(Allies > 0 ? "Victoria RTS; consecuencia territorial registrada." : "Derrota RTS; puedes reiniciar la prueba.");
            }
        }
    }
}
