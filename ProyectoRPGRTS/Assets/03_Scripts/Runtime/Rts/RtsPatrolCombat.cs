using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    public sealed partial class RtsBattleController
    {
        private static readonly float[] PatrolApproachAngles = { 0, 35, -35, 70, -70 };

        private void OrderPatrolAttack(Soldier soldier, Soldier target)
        {
            soldier.Unit.Stop();
            soldier.OrderedTarget = target;
            soldier.MoveOrder = false;
            soldier.Guarding = false;
            soldier.Animator?.SetGuarding(false);
            soldier.RepathAt = 0;
        }

        private void TickPatrolSoldier(Soldier soldier)
        {
            if (soldier.MoveOrder && soldier.Unit.IsMoving &&
                (session.Rules.UxExperience || Time.time < soldier.OrderedUntil)) return;
            soldier.MoveOrder = false;

            var position = soldier.Unit.transform.position;
            var target = IsLive(soldier.OrderedTarget) ? soldier.OrderedTarget : null;
            // Una orden conserva su objetivo de marcha. Un defensor visible a distancia
            // de ataque se puede combatir mientras ese objetivo esté obstruido.
            var contact = target != null && InRange(soldier, target, soldier.Range)
                ? target : LocalTarget(soldier, soldier.Range, position, soldier.Range);
            if (contact != null)
            {
                if (soldier.Unit.IsMoving) soldier.Unit.Stop();
                if (!soldier.Guarding && Time.time >= soldier.AttackAt) Strike(soldier, contact);
                return;
            }

            target ??= Nearest(soldier);
            if (target == null || (session.Rules.UxExperience && soldier.OrderedTarget != target &&
                (target.Unit.transform.position - position).sqrMagnitude > 64f)) return;
            if (Time.time < soldier.RepathAt) return;
            soldier.RepathAt = Time.time + .55f;

            var centre = target.Unit.transform.position;
            var offset = AttackPosition(soldier, target) - centre;
            var agent = soldier.Unit.Agent;
            foreach (var angle in PatrolApproachAngles)
            {
                var point = centre + Quaternion.AngleAxis(angle, Vector3.up) * offset;
                if (!NavMesh.SamplePosition(point, out var hit, .45f, agent.areaMask) ||
                    !TacticalLine(soldier, target, hit.position)) continue;
                if (soldier.Unit.IsMoving && (agent.destination - hit.position).sqrMagnitude < .16f) return;
                if (soldier.Unit.TryMoveTo(hit.position)) return;
            }
        }
    }
}
