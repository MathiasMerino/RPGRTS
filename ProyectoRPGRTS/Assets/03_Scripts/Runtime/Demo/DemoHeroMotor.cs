using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Aethernia
{
    // Control directo opt-in de la demo. Reutiliza agente, escala, clase y colisión; no controla unidades RTS.
    public sealed class DemoHeroMotor : MonoBehaviour
    {
        public DemoFlow Flow;
        public int UpdateTicks { get; private set; }
        public Vector2 LastReadInput { get; private set; }
        public bool LastBlocked { get; private set; }
        private readonly RaycastHit[] hits=new RaycastHit[24];
        private ExplorationUnit Hero=>Flow.Director.Hero;
        private void Start(){var input=Hero.GetComponent<PlayerCommands>();if(input==null)input=Object.FindAnyObjectByType<PlayerCommands>();if(input!=null)input.NearbyInteraction=!Flow.LegacyProbe;}
        public bool CanControl=>Flow.IsPlaying&&!Flow.LegacyProbe&&!Flow.Session.Paused&&Flow.Session.Mode==PrototypeMode.Exploration&&Flow.Session.State.Health>0;
        private void Update()
        {
            UpdateTicks++;
            Flow.CameraRig.ReserveWASDForHero=!Flow.LegacyProbe;
            var keys=Keyboard.current;var input=Vector2.zero;
            if(keys!=null)input=new Vector2((keys.dKey.isPressed?1:0)-(keys.aKey.isPressed?1:0),(keys.wKey.isPressed?1:0)-(keys.sKey.isPressed?1:0));
            var ui=Flow.ExistingHud.CompactView;
            LastReadInput=input;LastBlocked=UiInputGuard.IsTyping||ui.WindowOpen||ui.HelpOpen||Flow.Session.WorldUiOpen;ApplyInput(input,Time.deltaTime,LastBlocked);
        }
        private void LateUpdate(){if(CanControl&&Hero.IsMoving)Flow.CameraRig.FocusOn(Hero.transform.position);}
        public static Vector3 CameraRelative(Vector2 input,Transform camera)
        {
            input=Vector2.ClampMagnitude(input,1);var right=camera.right;right.y=0;var forward=camera.forward;forward.y=0;
            return right.normalized*input.x+forward.normalized*input.y;
        }
        public bool ApplyInput(Vector2 input,float seconds,bool blocked)
        {
            if(!CanControl||blocked||input.sqrMagnitude<.001f||seconds<=0||float.IsNaN(input.sqrMagnitude)||float.IsInfinity(input.sqrMagnitude)||float.IsNaN(seconds)||float.IsInfinity(seconds))
            {Hero.EndDirectControl();return false;}
            var direction=CameraRelative(input,Flow.CameraRig.transform);var speed=Flow.Session.Rules.Find(Flow.Session.State.ClassId).Speed;
            var displacement=direction*speed*Mathf.Min(seconds,.1f);var start=Hero.Agent.nextPosition;
            if(NavMesh.Raycast(start,start+displacement,out var obstacle,Hero.Agent.areaMask))
            {var planar=obstacle.position-start;planar.y=0;displacement=Vector3.ClampMagnitude(displacement,Mathf.Max(0,planar.magnitude-.025f));}
            var distance=displacement.magnitude;
            if(distance>.0001f)
            {
                var count=Physics.CapsuleCastNonAlloc(start+Vector3.up*.31f,start+Vector3.up*1.49f,.28f,displacement.normalized,hits,distance+.025f,~0,QueryTriggerInteraction.Ignore);
                for(var i=0;i<count;i++)
                {
                    if(hits[i].transform.IsChildOf(Hero.transform)||hits[i].collider.bounds.max.y<=start.y+.08f)continue;
                    distance=Mathf.Min(distance,Mathf.Max(0,hits[i].distance-.025f));
                }
                displacement=Vector3.ClampMagnitude(displacement,distance);
            }
            return Hero.MoveDirect(displacement,seconds);
        }
        private void OnDisable(){if(Flow!=null&&Flow.Director!=null)Hero.EndDirectControl();}
    }
}
