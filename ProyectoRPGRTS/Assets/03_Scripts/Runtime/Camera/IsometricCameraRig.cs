using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Aethernia
{
    [RequireComponent(typeof(Camera))]
    public sealed class IsometricCameraRig : MonoBehaviour
    {
        [SerializeField] private Vector3 focus;
        [SerializeField] private float size = 12;
        [SerializeField] private float minimumSize = 7;
        [SerializeField] private float maximumSize = 25;
        [SerializeField] private float panSpeed = 12;
        [SerializeField] private Vector2 horizontalLimits = new Vector2(-18, 18);
        [SerializeField] private Vector2 verticalLimits = new Vector2(-18, 18);
        private Camera view;
        public Vector3 Focus => focus;
        public float Size => size;

        private void Awake() { view = GetComponent<Camera>(); Refresh(); }
        public void Configure(Vector3 centre) { focus = centre; view = GetComponent<Camera>(); Refresh(); }
        public void FocusOn(Vector3 point) { focus = new Vector3(point.x, 0, point.z); Refresh(); }
        public void SetRegion(Vector3 centre)
        { horizontalLimits = new Vector2(centre.x - 18, centre.x + 18); verticalLimits = new Vector2(centre.z - 18, centre.z + 18); FocusOn(centre); }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            var pan = Vector2.zero;
            if (keyboard != null)
            {
                pan.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0) -
                        (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
                pan.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0) -
                        (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
            }
            if (mouse != null && mouse.middleButton.isPressed) pan -= mouse.delta.ReadValue() * 0.04f;
            var blocked = UiInputGuard.BlocksWorldInput;
            ApplyInput(pan, mouse == null ? 0 : Mathf.Sign(mouse.scroll.ReadValue().y), Time.unscaledDeltaTime, blocked);
        }

        public void ApplyInput(Vector2 pan, float zoom, float deltaTime, bool blockedByUi)
        {
            if (blockedByUi) return;
            var right = new Vector3(0.7071068f, 0, -0.7071068f);
            var forward = new Vector3(0.7071068f, 0, 0.7071068f);
            pan = Vector2.ClampMagnitude(pan, 1);
            focus += (right * pan.x + forward * pan.y) * (panSpeed * Mathf.Max(0, deltaTime));
            focus.x = Mathf.Clamp(focus.x, horizontalLimits.x, horizontalLimits.y);
            focus.z = Mathf.Clamp(focus.z, verticalLimits.x, verticalLimits.y);
            size = Mathf.Clamp(size - zoom, minimumSize, maximumSize);
            Refresh();
        }

        private void Refresh()
        {
            if (view == null) view = GetComponent<Camera>();
            view.orthographic = true;
            view.orthographicSize = size;
            transform.rotation = Quaternion.Euler(35.26439f, 45, 0);
            transform.position = focus - transform.forward * 45;
        }
    }
}
