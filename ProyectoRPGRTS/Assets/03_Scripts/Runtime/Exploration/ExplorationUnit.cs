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
        public GameObject SelectionIndicator => selectionIndicator;
        public bool IsSelected { get; private set; }
        public float LastPathLength { get; private set; }
        public bool DirectControl { get; private set; }
        private Vector3 directVelocity;
        public Vector3 PresentationVelocity=>DirectControl?directVelocity:Agent.velocity;
        public NavMeshAgent Agent => agent != null ? agent : (agent = GetComponent<NavMeshAgent>());
        public bool IsMoving => Agent.enabled && Agent.isOnNavMesh && (DirectControl?directVelocity.sqrMagnitude>.0025f:
            (Agent.pathPending || (Agent.hasPath && Agent.remainingDistance > Agent.stoppingDistance + 0.02f)));
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
            DirectControl=false;directVelocity=Vector3.zero;
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
        public bool TryStepTowards(Vector3 point, float maximumDistance, float stoppingDistance, bool animated = false)
        {
            if (!CalculatePath(point)) return false;
            // Both presentations use a destination along the baked path in metres.
            var remaining = Mathf.Min(maximumDistance, Mathf.Max(0, LastPathLength - stoppingDistance));
            if (remaining <= 0) return false;
            var corners = path.corners;
            for (var i = 1; i < corners.Length; i++)
            {
                var length = Vector3.Distance(corners[i - 1], corners[i]);
                if (remaining <= length)
                {
                    var destination = Vector3.MoveTowards(corners[i - 1], corners[i], remaining);
                    if(animated)return TryMoveTo(destination,maximumDistance);
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
        { DirectControl=false;directVelocity=Vector3.zero;if (Agent.enabled && Agent.isOnNavMesh) Agent.ResetPath(); }
        public bool MoveDirect(Vector3 displacement,float seconds)
        {
            if(!Agent.enabled||!Agent.isOnNavMesh||seconds<=0||float.IsNaN(seconds)||float.IsInfinity(seconds))return false;
            if(float.IsNaN(displacement.sqrMagnitude)||float.IsInfinity(displacement.sqrMagnitude))return false;
            if(Agent.hasPath)Agent.ResetPath();DirectControl=true;directVelocity=displacement/seconds;Agent.Move(displacement);
            if(displacement.sqrMagnitude>.000001f)transform.rotation=Quaternion.LookRotation(displacement.normalized);
            return displacement.sqrMagnitude>.000001f;
        }
        public void EndDirectControl(){if(DirectControl)Stop();}
    }
}
