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
        private void Awake() { agent = GetComponent<NavMeshAgent>(); path = new NavMeshPath(); }
        public void Configure(GameObject indicator) { selectionIndicator = indicator; Select(false); }
        public void Select(bool value)
        {
            IsSelected = value;
            if (selectionIndicator != null) selectionIndicator.SetActive(value);
        }
        public bool TryMoveTo(Vector3 point, float maximumDistance = float.PositiveInfinity)
        {
            if (!isActiveAndEnabled || !Agent.isOnNavMesh) return false;
            if (!NavMesh.SamplePosition(point, out var hit, 0.6f, Agent.areaMask)) return false;
            if (path == null) path = new NavMeshPath();
            if (!Agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete) return false;
            LastPathLength = 0;
            var corners = path.corners;
            for (var i = 1; i < corners.Length; i++) LastPathLength += Vector3.Distance(corners[i - 1], corners[i]);
            if (LastPathLength > maximumDistance + 0.01f) return false;
            return Agent.SetPath(path);
        }
        public bool Warp(Vector3 point)
        {
            return NavMesh.SamplePosition(point, out var hit, 1, Agent.areaMask) && Agent.Warp(hit.position);
        }
    }
}
