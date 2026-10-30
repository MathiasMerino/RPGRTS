using UnityEngine;

namespace Aethernia
{
    // Reversible visual cutaway. Uses cached renderer bounds, never physics or materials.
    public sealed class RoofCutaway : MonoBehaviour
    {
        [SerializeField] private Camera view;
        [SerializeField] private Transform target;
        [SerializeField] private Renderer[] roof = System.Array.Empty<Renderer>();
        [SerializeField] private float targetHeight = 1.55f;
        private bool[] original;
        public bool IsCutAway { get; private set; }
        public void Configure(Camera camera, Transform actor, Renderer[] renderers)
        { Restore(); view = camera; target = actor; roof = renderers; original = null; }
        private void OnEnable() { CaptureOriginal(); }
        private void CaptureOriginal()
        {
            if (original != null && original.Length == roof.Length) return;
            original = new bool[roof.Length];
            for (var i = 0; i < roof.Length; i++) original[i] = roof[i] != null && roof[i].forceRenderingOff;
        }
        private void LateUpdate()
        {
            CaptureOriginal(); var occluded = false;
            if (view != null && target != null && target.gameObject.activeInHierarchy)
            {
                var point = target.position + Vector3.up * targetHeight;
                // Orthographic rays must start at the projected pixel, not at the camera centre.
                var forward = view.transform.forward;
                var depth = Vector3.Dot(point - view.transform.position, forward);
                if (depth > 0)
                {
                    var ray = view.orthographic
                        ? new Ray(point - forward * depth, forward)
                        : new Ray(view.transform.position, point - view.transform.position);
                    var distance = Vector3.Dot(point - ray.origin, ray.direction);
                    foreach (var renderer in roof)
                        if (renderer != null && renderer.enabled && renderer.bounds.IntersectRay(ray, out var hit) && hit < distance - .05f)
                        { occluded = true; break; }
                }
            }
            IsCutAway = occluded;
            for (var i = 0; i < roof.Length; i++) if (roof[i] != null) roof[i].forceRenderingOff = original[i] || occluded;
        }
        private void Restore()
        {
            if (original != null)
                for (var i = 0; i < Mathf.Min(roof.Length, original.Length); i++) if (roof[i] != null) roof[i].forceRenderingOff = original[i];
            IsCutAway = false;
        }
        private void OnDisable() { Restore(); original = null; }
    }
}
