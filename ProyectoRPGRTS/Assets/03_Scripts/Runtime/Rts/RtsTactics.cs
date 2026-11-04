using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public enum RtsFormation { Line, Column, Compact }
    public enum RtsStance { Passive, Defensive, Aggressive }
    internal enum TacticalOrder { Idle, Stopped, Move, Attack, Advance, Hold, Defend }

    public sealed partial class RtsBattleController
    {
        private const float SlotSpacing = 1.45f;
        private const float ArrivalTolerance = .65f;
        private const float DefenderLeash = 9f;
        private readonly List<Soldier> commandGroup = new List<Soldier>(12);
        private readonly RaycastHit[] sightHits = new RaycastHit[32];
        private NavMeshPath tacticalPath;

        public bool TacticalBattle => session != null && session.Rules.TacticalWarfare &&
            IncludesHero && ContextId == "conquest-outpost";
        public bool TacticalDeployment { get; private set; }
        public bool TacticalReady { get; private set; }
        public RtsFormation Formation { get; private set; }
        public RtsStance? SelectedStance
        {
            get
            {
                RtsStance? stance = null;
                foreach (var s in soldiers)
                {
                    if (s.Team != 0 || !IsLive(s) || !s.Unit.IsSelected) continue;
                    if (stance.HasValue && stance.Value != s.Stance) return null;
                    stance = s.Stance;
                }
                return stance;
            }
        }

        private bool CanOrder => Active && TacticalBattle && !session.Paused && !session.WorldUiOpen;

        private void ResetTactics()
        {
            TacticalDeployment = TacticalReady = false;
            Formation = RtsFormation.Line;
            commandGroup.Clear();
            LastOrderText = "";
            LastOrderAt = -100;
        }

        private void OpenTacticalDeployment()
        {
            tacticalPath ??= new NavMeshPath();
            TacticalDeployment = true;
            foreach (var s in soldiers)
            {
                s.Post = s.Unit.transform.position;
                s.Unit.Select(s.Hero);
            }
        }

        public void CycleFormation()
        {
            if (CanOrder) Formation = (RtsFormation)(((int)Formation + 1) % 3);
        }

        public void CycleStance()
        {
            if (!CanOrder) return;
            var next = (RtsStance)(((int)(SelectedStance ?? RtsStance.Aggressive) + 1) % 3);
            foreach (var s in soldiers)
                if (s.Team == 0 && IsLive(s) && s.Unit.IsSelected) s.Stance = next;
        }

        public bool FocusSelection(IsometricCameraRig rig)
        {
            if (!CanOrder || !CollectSelected()) return false;
            rig.FocusOn(GroupCentre());
            return true;
        }

        public bool StopTactical(bool hold)
        {
            if (!CanOrder || !CollectSelected()) return false;
            foreach (var s in commandGroup) SetPost(s, hold ? TacticalOrder.Hold : TacticalOrder.Stopped);
            return ReportOrder(hold ? "Mantener posición" : "Detener", GroupCentre());
        }

        public bool RegroupTactical()
        {
            if (!CanOrder || !CollectSelected()) return false;
            return SendFormation(GroupCentre(), TacticalOrder.Move, "Reagrupar");
        }

        public bool DefendTactical(Vector3 point)
        {
            if (!CanOrder || !CollectSelected()) return false;
            return SendFormation(point, TacticalOrder.Defend, "Defender posición");
        }

        public bool TacticalCommand(Vector3 point, Collider collider, bool advance)
        {
            if (!CanOrder || !CollectSelected()) return false;
            if (!Finite(point)) return false;
            var unit = collider == null ? null : collider.GetComponentInParent<ExplorationUnit>();
            if (unit == null)
            {
                if (advance && TacticalDeployment) return RejectTactical("Confirma la ofensiva para usar el avance ofensivo.");
                return SendFormation(point, advance ? TacticalOrder.Advance : TacticalOrder.Move,
                    advance ? "Avance ofensivo" : "Mover formación");
            }

            var target = soldiers.Find(s => s.Team == 1 && s.Unit == unit && IsLive(s));
            if (target == null) return false;
            if (TacticalDeployment && !StartTacticalBattle()) return false;
            foreach (var s in commandGroup)
            {
                SetPost(s, TacticalOrder.Attack);
                s.OrderedTarget = target;
            }
            return ReportOrder("Atacar objetivo", target.Unit.transform.position);
        }

        public bool StartTacticalBattle()
        {
            if (!CanOrder || !TacticalDeployment) return false;
            if (!TacticalReady) return RejectTactical("Despliega al menos una unidad y espera su llegada.");
            TacticalDeployment = false;
            session.Say("Ofensiva confirmada. Los defensores protegen el paso.");
            Trace("tactical-engage");
            return true;
        }

        public bool RetreatTactical()
        {
            if (!CanOrder || TacticalDeployment) return false;
            Trace("tactical-retreat");
            FinishBattle();
            return true;
        }

        private bool CollectSelected()
        {
            commandGroup.Clear();
            foreach (var s in soldiers)
                if (s.Team == 0 && IsLive(s) && s.Unit.IsSelected) commandGroup.Add(s);
            if (commandGroup.Count > 0) return true;
            return RejectTactical("Selecciona aliados antes de dar una orden.");
        }

        private Vector3 GroupCentre()
        {
            var centre = Vector3.zero;
            foreach (var s in commandGroup) centre += s.Unit.transform.position;
            return centre / commandGroup.Count;
        }

        internal static Vector3 FormationOffset(RtsFormation shape, int index, int count)
        {
            var columns = shape == RtsFormation.Line ? Mathf.Min(5, count) :
                shape == RtsFormation.Compact ? Mathf.CeilToInt(Mathf.Sqrt(count)) : 1;
            var row = index / columns;
            var width = Mathf.Min(columns, count - row * columns);
            var rows = Mathf.CeilToInt((float)count / columns);
            return new Vector3((index % columns - (width - 1) * .5f) * SlotSpacing, 0,
                ((rows - 1) * .5f - row) * SlotSpacing);
        }

        private bool SendFormation(Vector3 point, TacticalOrder order, string label)
        {
            if (!Finite(point)) return false;
            var forward = point - GroupCentre();
            forward.y = 0;
            var facing = Quaternion.LookRotation(forward.sqrMagnitude > .04f ? forward : Vector3.right);
            // Las unidades de corto alcance lideran, incluido el héroe según su alcance real.
            var arranged = commandGroup.ToArray();
            System.Array.Sort(arranged, (a, b) =>
            {
                var role = (a.Range > 2.5f).CompareTo(b.Range > 2.5f);
                return role != 0 ? role : soldiers.IndexOf(a).CompareTo(soldiers.IndexOf(b));
            });
            var mixedRange = Formation != RtsFormation.Column && arranged[0].Range <= 2.5f &&
                arranged[arranged.Length - 1].Range > 2.5f;
            var slots = new Vector3[arranged.Length];
            var paths = new NavMeshPath[arranged.Length];
            var previous = new NavMeshPath[arranged.Length];
            for (var i = 0; i < arranged.Length; i++)
            {
                var agent = arranged[i].Unit.Agent;
                var offset = FormationOffset(Formation, i, arranged.Length);
                if (mixedRange) offset.z += arranged[i].Range <= 2.5f ? .5f : -.5f;
                var requested = point + facing * offset;
                var path = new NavMeshPath();
                if (!agent.isOnNavMesh || !NavMesh.SamplePosition(requested, out var hit, .45f, agent.areaMask) ||
                    !agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete)
                    return RejectTactical("Formación sin ruta completa. Prueba otra posición o columna.");
                for (var j = 0; j < i; j++)
                    if ((slots[j] - hit.position).sqrMagnitude < 1f)
                        return RejectTactical("Los destinos de formación se solapan.");
                slots[i] = hit.position;
                paths[i] = path;
                if (agent.hasPath) previous[i] = agent.path;
            }

            for (var i = 0; i < arranged.Length; i++)
            {
                if (arranged[i].Unit.Agent.SetPath(paths[i])) continue;
                for (var j = 0; j <= i; j++)
                {
                    var agent = arranged[j].Unit.Agent;
                    if (previous[j] == null) agent.ResetPath();
                    else agent.SetPath(previous[j]);
                }
                return RejectTactical("La orden no se pudo aplicar.");
            }
            for (var i = 0; i < arranged.Length; i++)
            {
                var s = arranged[i];
                SetTacticalState(s, order);
                s.Destination = slots[i];
                s.Deployed = (s.Unit.transform.position - slots[i]).sqrMagnitude > 1f;
                Trace("command", s);
            }
            return ReportOrder(label, point);
        }

        private void SetPost(Soldier s, TacticalOrder order)
        {
            s.Unit.Stop();
            SetTacticalState(s, order);
        }

        private static void SetTacticalState(Soldier s, TacticalOrder order)
        {
            s.Order = order;
            s.OrderedTarget = null;
            s.Guarding = false;
            s.Animator?.SetGuarding(false);
            s.Post = s.Unit.transform.position;
            s.RepathAt = 0;
            s.Deployed = false;
        }

        private bool ReportOrder(string text, Vector3 point)
        {
            LastOrderText = text + " · " + commandGroup.Count;
            LastOrderPosition = point;
            LastOrderAt = Time.unscaledTime;
            session.Say(LastOrderText);
            return true;
        }

        private bool RejectTactical(string reason)
        {
            session.Say(reason);
            return false;
        }

        private static bool Finite(Vector3 point) => !float.IsNaN(point.sqrMagnitude) && !float.IsInfinity(point.sqrMagnitude);
        private bool IsLive(Soldier s) => s != null && s.Unit != null && s.Unit.isActiveAndEnabled && Health(s) > 0;

        private void TickTactics()
        {
            nextTick = Time.time + .2f;
            foreach (var s in soldiers)
            {
                if (!IsLive(s)) continue;
                if (TacticalDeployment)
                {
                    if (s.Team == 0 && s.Deployed && Arrived(s))
                    {
                        TacticalReady = true;
                        s.Deployed = false;
                    }
                    continue;
                }
                TickTacticalSoldier(s);
                if (session.State.Health <= 0) break;
            }
            if (!TacticalDeployment && (session.State.Health <= 0 || Allies == 0 || Enemies == 0)) FinishBattle();
        }

        private static bool Arrived(Soldier s) => !s.Unit.IsMoving &&
            (s.Unit.transform.position - s.Destination).sqrMagnitude <= ArrivalTolerance * ArrivalTolerance;

        private void TickTacticalSoldier(Soldier s)
        {
            if (s.Order == TacticalOrder.Stopped) return;
            if (s.Order == TacticalOrder.Move || s.Order == TacticalOrder.Defend)
            {
                if (s.Unit.IsMoving) return;
                if (Arrived(s)) s.Post = s.Destination;
                s.Order = s.Order == TacticalOrder.Defend ? TacticalOrder.Hold : TacticalOrder.Idle;
            }
            if (s.Order == TacticalOrder.Attack && !IsLive(s.OrderedTarget))
                SetPost(s, TacticalOrder.Idle);

            var leash = s.Team == 1 ? DefenderLeash : s.Stance == RtsStance.Aggressive ? 8f : 4f;
            var anchor = s.Order == TacticalOrder.Advance ? s.Destination : s.Post;
            var target = s.OrderedTarget;
            if (target == null && s.Stance != RtsStance.Passive)
                target = LocalTarget(s, s.Order == TacticalOrder.Hold ? s.Range : Mathf.Max(s.Range, leash),
                    anchor, s.Order == TacticalOrder.Advance ? 8f : leash);
            if (target != null)
            {
                if (InRange(s, target, s.Range))
                {
                    if (s.Unit.IsMoving) s.Unit.Stop();
                    if (!s.Guarding && Time.time >= s.AttackAt) Strike(s, target);
                    return;
                }
                if (s.Order != TacticalOrder.Hold && Time.time >= s.RepathAt)
                {
                    var attackAt = AttackPosition(s, target);
                    var allowed = s.Order == TacticalOrder.Attack ||
                        (attackAt - anchor).sqrMagnitude <= (s.Order == TacticalOrder.Advance ? 64f : leash * leash);
                    if (allowed) Repath(s, attackAt);
                    return;
                }
            }

            if (target != null && s.Order != TacticalOrder.Hold) return;
            if (s.Order == TacticalOrder.Advance)
            {
                if (Arrived(s)) SetPost(s, TacticalOrder.Idle);
                else if (!s.Unit.IsMoving && Time.time >= s.RepathAt) Repath(s, s.Destination);
            }
            else if (s.Order == TacticalOrder.Idle && (s.Unit.transform.position - s.Post).sqrMagnitude > .75f &&
                     Time.time >= s.RepathAt && !s.Unit.IsMoving)
                Repath(s, s.Post);
        }

        private void Repath(Soldier s, Vector3 point)
        {
            s.RepathAt = Time.time + .55f;
            var agent = s.Unit.Agent;
            if (!agent.isOnNavMesh || !NavMesh.SamplePosition(point, out var hit, .55f, agent.areaMask) ||
                !agent.CalculatePath(hit.position, tacticalPath) || tacticalPath.status != NavMeshPathStatus.PathComplete) return;
            if (!agent.hasPath || (agent.destination - hit.position).sqrMagnitude > .16f)
                agent.SetPath(tacticalPath);
        }

        private bool InRange(Soldier source, Soldier target, float range) =>
            (target.Unit.transform.position - source.Unit.transform.position).sqrMagnitude <= range * range &&
            ClearLine(source, target);

        private Soldier LocalTarget(Soldier source, float range, Vector3 anchor, float leash)
        {
            Soldier result = null;
            var best = range * range;
            foreach (var other in soldiers)
            {
                if (other.Team == source.Team || !IsLive(other)) continue;
                var point = other.Unit.transform.position;
                if ((point - anchor).sqrMagnitude > leash * leash) continue;
                var distance = (point - source.Unit.transform.position).sqrMagnitude;
                if (distance > best || !TacticalLine(source, other)) continue;
                result = other;
                best = distance;
            }
            return result;
        }

        private bool TacticalLine(Soldier source, Soldier target)
        {
            var from = source.Unit.transform.position + Vector3.up * .9f;
            var delta = target.Unit.transform.position + Vector3.up * .9f - from;
            var length = delta.magnitude;
            if (length < .01f) return true;
            var count = Physics.RaycastNonAlloc(from, delta / length, sightHits, length, ~0, QueryTriggerInteraction.Ignore);
            if (count == sightHits.Length) return false;
            for (var i = 0; i < count; i++)
            {
                var hit = sightHits[i].collider.GetComponentInParent<ExplorationUnit>();
                if (hit == source.Unit || hit == target.Unit) continue;
                if (hit != null && SameTeam(hit, source.Team)) continue;
                return false;
            }
            return true;
        }

        private bool SameTeam(ExplorationUnit unit, int team)
        {
            foreach (var soldier in soldiers)
                if (soldier.Unit == unit) return soldier.Team == team;
            return false;
        }

        private Soldier HeroTarget(Soldier hero, float range)
        {
            if (hero.OrderedTarget != null && Health(hero.OrderedTarget) > 0) return hero.OrderedTarget;
            if (!TacticalBattle) return Nearest(hero);
            return LocalTarget(hero, range, hero.Unit.transform.position, range);
        }
    }
}
