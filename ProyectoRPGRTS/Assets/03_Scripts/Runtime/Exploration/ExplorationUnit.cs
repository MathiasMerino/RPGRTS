using UnityEngine;
using UnityEngine.AI;

namespace Aethernia
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class ExplorationUnit : MonoBehaviour
    {
        [SerializeField] private GameObject selectionIndicator;
        private NavMeshAgent agent;
        private NavMeshPath path;
        public bool IsSelected { get; private set; }
        public float LastPathLength { get; private set; }
        public NavMeshAgent Agent => agent != null ? agent : (agent = GetComponent<NavMeshAgent>());
        public bool IsMoving => Agent.enabled && Agent.isOnNavMesh &&
            (Agent.pathPending || (Agent.hasPath && Agent.remainingDistance > Agent.stoppingDistance + 0.02f));
        private void Awake() { agent = GetComponent<NavMeshAgent>(); path = new NavMeshPath(); }
        // The scene hero is serialized disabled: native agents can enable before scene surfaces.
        // Runtime spawned prefabs already enable against a loaded surface.
        private void Start() { if (!Agent.enabled) Agent.enabled = true; }
        public void Configure(GameObject indicator) { selectionIndicator = indicator; Select(false); }
        public void Select(bool value)
        {
            IsSelected = value;
            if (selectionIndicator != null) selectionIndicator.SetActive(value);
        }
        public bool TryMoveTo(Vector3 point, float maximumDistance = float.PositiveInfinity)
        {
            if (!CalculatePath(point) || LastPathLength > maximumDistance + 0.01f) return false;
            return Agent.SetPath(path);
        }
        private bool CalculatePath(Vector3 point)
        {
            if (!isActiveAndEnabled || !Agent.isOnNavMesh) return false;
            if (!NavMesh.SamplePosition(point, out var hit, 0.6f, Agent.areaMask)) return false;
            if (path == null) path = new NavMeshPath();
            if (!Agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete) return false;
            LastPathLength = 0;
            var corners = path.corners;
            for (var i = 1; i < corners.Length; i++) LastPathLength += Vector3.Distance(corners[i - 1], corners[i]);
            return true;
        }
        public bool TryStepTowards(Vector3 point, float maximumDistance, float stoppingDistance)
        {
            if (!CalculatePath(point)) return false;
            // Enemy turns remain instantaneous, but travel follows the baked path in metres.
            var remaining = Mathf.Min(maximumDistance, Mathf.Max(0, LastPathLength - stoppingDistance));
            if (remaining <= 0) return false;
            var corners = path.corners;
            for (var i = 1; i < corners.Length; i++)
            {
                var length = Vector3.Distance(corners[i - 1], corners[i]);
                if (remaining <= length)
                {
                    var destination = Vector3.MoveTowards(corners[i - 1], corners[i], remaining);
                    Stop(); return Agent.Warp(destination);
                }
                remaining -= length;
            }
            return false;
        }
        public bool Warp(Vector3 point)
        {
            return NavMesh.SamplePosition(point, out var hit, 1, Agent.areaMask) && Agent.Warp(hit.position);
        }
        public void Stop()
        { if (Agent.enabled && Agent.isOnNavMesh) Agent.ResetPath(); }
    }
}
