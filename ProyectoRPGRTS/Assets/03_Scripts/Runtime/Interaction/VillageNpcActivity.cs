using UnityEngine;

namespace Aethernia
{
    // Actividad local provisional, sin simulación social ni estado de campaña adicional.
    public sealed class VillageNpcActivity : MonoBehaviour
    {
        public DemoFlow Flow;
        public ExplorationUnit Actor;
        public Vector3[] Route = System.Array.Empty<Vector3>();
        private int next;
        private float wait;
        private Quaternion restingRotation;
        private bool talking;
        private void Start() { restingRotation = transform.rotation; Actor.Agent.avoidancePriority = 75; }
        private void Update()
        {
            if (!Flow.IsPlaying || Flow.Session.Paused || Flow.Session.Mode != PrototypeMode.Exploration) { Actor.Stop(); return; }
            var near = Vector3.Distance(transform.position, Flow.Director.Hero.transform.position) < 3;
            if (near || Flow.Session.WorldUiOpen)
            {
                Actor.Stop(); talking = true;
                var direction = Flow.Director.Hero.transform.position - transform.position; direction.y = 0;
                if (direction.sqrMagnitude > .01f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.deltaTime);
                return;
            }
            if (talking) { transform.rotation = restingRotation; talking = false; wait = Time.time + 1; }
            if (Route.Length == 0 || Actor.IsMoving || Time.time < wait) return;
            Actor.TryMoveTo(Route[next++ % Route.Length]); wait = Time.time + 3;
        }
    }
}
