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
        public System.Func<bool> ModalOpen;
        private Camera view;
        public ExplorationUnit FollowTarget;
        private Vector3 smoothedFocus,followVelocity,lastHero;
        private float smoothedSize,manualUntil;
        private PrototypeMode previousMode;
        private bool smoothInitialized;
        private bool Ux=>InputSession!=null&&InputSession.Rules.UxExperience;
        private void LateUpdate()
        {
            if(!Ux)return;
            var mode=InputSession.Mode;
            if(mode!=previousMode)
            {
                previousMode=mode;minimumSize=mode==PrototypeMode.Exploration?4.5f:6;maximumSize=mode==PrototypeMode.Exploration?11:18;
                size=mode==PrototypeMode.Exploration?6.2f:mode==PrototypeMode.TurnCombat?7:mode==PrototypeMode.Construction?10:mode==PrototypeMode.Rts?8.5f:13;
                manualUntil=0;
            }
            if(FollowTarget!=null&&mode==PrototypeMode.Exploration&&!InputSession.Paused&&Time.unscaledTime>manualUntil)
            {
                var point=FollowTarget.transform.position;var velocity=Time.unscaledDeltaTime>0?(point-lastHero)/Time.unscaledDeltaTime:Vector3.zero;
                focus=point+Vector3.ClampMagnitude(velocity*.12f,.5f);focus.y=0;
            }
            if(FollowTarget!=null)lastHero=FollowTarget.transform.position;
            focus.x=Mathf.Clamp(focus.x,horizontalLimits.x,horizontalLimits.y);focus.z=Mathf.Clamp(focus.z,verticalLimits.x,verticalLimits.y);
            if(!smoothInitialized||Vector3.Distance(smoothedFocus,focus)>30){smoothedFocus=focus;smoothedSize=size;smoothInitialized=true;followVelocity=Vector3.zero;}
            smoothedFocus=Vector3.SmoothDamp(smoothedFocus,focus,ref followVelocity,.18f,100,Time.unscaledDeltaTime);
            smoothedSize=Mathf.Lerp(smoothedSize,size,1-Mathf.Exp(-10*Time.unscaledDeltaTime));
            ApplyPose(smoothedFocus,smoothedSize);
        }
        public Vector3 Focus => focus;
        public float Size => size;
        public float PanSpeed => panSpeed;
        public GameSession InputSession { get; set; }
        public bool ReserveWASDForHero { get; set; }
        public void SetZoom(float value){if(float.IsNaN(value)||float.IsInfinity(value))return;size=Mathf.Clamp(value,minimumSize,maximumSize);Refresh();}
        public void SetPanSpeed(float value) { if(!float.IsNaN(value)&&!float.IsInfinity(value))panSpeed=Mathf.Clamp(value,4,24); }

        private void Awake() { view = GetComponent<Camera>(); Refresh(); }
        public void Configure(Vector3 centre) { focus = centre; view = GetComponent<Camera>(); Refresh(); }
        public void FocusOn(Vector3 point) { focus = new Vector3(point.x, 0, point.z); Refresh(); }
        public void SetRegion(Vector3 centre)
        { horizontalLimits = new Vector2(centre.x - 18, centre.x + 18); verticalLimits = new Vector2(centre.z - 18, centre.z + 18); if(Ux&&centre==Vector3.zero){horizontalLimits=new Vector2(-54,18);verticalLimits=new Vector2(-18,18);} FocusOn(centre); }

        private void Update()
        {
            if(InputSession!=null&&InputSession.Paused)return;
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            var pan = Vector2.zero;
            if (keyboard != null)
            {
                var direct=ReserveWASDForHero&&InputSession!=null&&InputSession.Mode==PrototypeMode.Exploration;
                pan.x = ((!direct&&keyboard.dKey.isPressed) || keyboard.rightArrowKey.isPressed ? 1 : 0) -
                        ((!direct&&keyboard.aKey.isPressed) || keyboard.leftArrowKey.isPressed ? 1 : 0);
                pan.y = ((!direct&&keyboard.wKey.isPressed) || keyboard.upArrowKey.isPressed ? 1 : 0) -
                        ((!direct&&keyboard.sKey.isPressed) || keyboard.downArrowKey.isPressed ? 1 : 0);
            }
            if (mouse != null && mouse.middleButton.isPressed) pan -= mouse.delta.ReadValue() * 0.04f;
            var blocked = UiInputGuard.BlocksWorldInput;
            var wheel = mouse == null ? 0 : mouse.scroll.ReadValue().y;
            ApplyInput(pan, wheel == 0 ? 0 : Mathf.Sign(wheel), Time.unscaledDeltaTime, blocked);
        }

        public void ApplyInput(Vector2 pan, float zoom, float deltaTime, bool blockedByUi)
        {
            if (blockedByUi||(ModalOpen?.Invoke()??false)) return;
            var right = new Vector3(0.7071068f, 0, -0.7071068f);
            var forward = new Vector3(0.7071068f, 0, 0.7071068f);
            pan = Vector2.ClampMagnitude(pan, 1);
            if(pan.sqrMagnitude>.001f)manualUntil=Time.unscaledTime+2;
            focus += (right * pan.x + forward * pan.y) * (panSpeed * Mathf.Max(0, deltaTime));
            focus.x = Mathf.Clamp(focus.x, horizontalLimits.x, horizontalLimits.y);
            focus.z = Mathf.Clamp(focus.z, verticalLimits.x, verticalLimits.y);
            size = Mathf.Clamp(size - zoom, minimumSize, maximumSize);
            Refresh();
        }

        private void Refresh()
        {
            if (view == null) view = GetComponent<Camera>();
            if(Ux)return;
            ApplyPose(focus,size);
        }
        private void ApplyPose(Vector3 point,float zoom)
        {
            if(view==null)view=GetComponent<Camera>();
            view.orthographic = true;
            view.orthographicSize = zoom;
            transform.rotation = Quaternion.Euler(35.26439f, 45, 0);
            transform.position = point - transform.forward * 45;
        }
    }
}
