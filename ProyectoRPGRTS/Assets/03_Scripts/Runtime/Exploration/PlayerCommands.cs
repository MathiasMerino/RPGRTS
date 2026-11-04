using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Aethernia
{
    public sealed class PlayerCommands : MonoBehaviour
    {
        [SerializeField] private Camera view;
        [SerializeField] private ExplorationUnit hero;
        [SerializeField] private GameSession session;
        public InteractionPoint Target { get; private set; }
        public ExplorationUnit Hero => hero;
        public System.Func<bool> ModalOpen;
        public bool NearbyInteraction { get; set; }
        public bool ExternalRtsPointer { get; set; }
        public int InteractKeyPresses { get; private set; }
        public bool LastInteractBlocked { get; private set; }
        private readonly Collider[] nearby=new Collider[24];
        private float nextNearby;
        public InteractionPoint CurrentInteraction { get; private set; }
        public InteractionPoint RefreshNearby()
        {
            if(session==null||hero==null||session.Mode!=PrototypeMode.Exploration||session.Paused||session.WorldUiOpen||(ModalOpen?.Invoke()??false)){CurrentInteraction=null;return null;}
            var count=Physics.OverlapSphereNonAlloc(hero.transform.position,2.2f,nearby,~0,QueryTriggerInteraction.Collide);
            InteractionPoint best=null;var score=float.MaxValue;
            for(var i=0;i<count;i++)
            {var candidate=nearby[i].GetComponentInParent<InteractionPoint>();if(candidate==null||!candidate.isActiveAndEnabled||!candidate.Available(session))continue;
             var d=Vector3.Distance(hero.transform.position,candidate.transform.position);if(d>candidate.Reach)continue;
             if(session.Rules.WorldExploration&&!HasInteractionSight(candidate))continue;
             var direction=candidate.transform.position-hero.transform.position;direction.y=0;
             var focus=session.Rules.WorldExploration?Vector3.Dot(hero.transform.forward,direction.normalized)*.18f:0;
             var rank=d-(candidate==CurrentInteraction ? .25f : 0)-focus-(candidate==Target ? .1f : 0);if(rank<score){best=candidate;score=rank;}}
            CurrentInteraction=best;return best;
        }
        public void Configure(Camera camera, ExplorationUnit unit, GameSession game)
        { view = camera; hero = unit; session = game; }
        private readonly RaycastHit[] sightHits=new RaycastHit[32];
        public bool HasInteractionSight(InteractionPoint point)
        {
            if(point==null||hero==null)return false;
            var start=hero.transform.position+Vector3.up;var end=point.transform.position+Vector3.up;
            var direction=end-start;var distance=direction.magnitude;if(distance<.01f)return true;
            var count=Physics.RaycastNonAlloc(start,direction/distance,sightHits,distance,~0,QueryTriggerInteraction.Ignore);
            if(count==sightHits.Length)return false; // Una consulta saturada no autoriza atravesar geometría no examinada.
            for(var i=0;i<count;i++)
            {var t=sightHits[i].transform;if(t.IsChildOf(hero.transform)||t.IsChildOf(point.transform)||point.transform.IsChildOf(t))continue;return false;}
            return true;
        }
        private void Update()
        {
            if((ModalOpen?.Invoke()??false)||session.Paused||session.WorldUiOpen||session.Mode==PrototypeMode.MainMenu||session.Mode==PrototypeMode.CharacterCreation||session.Mode==PrototypeMode.Intro)return;
            if(ExternalRtsPointer&&session.Mode==PrototypeMode.Rts)return;
            if(session.Rules.UxExperience&&Time.unscaledTime>=nextNearby){nextNearby=Time.unscaledTime+.1f;RefreshNearby();}
            var mouse = Mouse.current;
            var blocked = UiInputGuard.BlocksWorldInput;
            if (mouse!=null&&view!=null&&(mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame))
                IssuePointer(mouse.position.ReadValue(), mouse.leftButton.wasPressedThisFrame, blocked || UiInputGuard.HasUiAt(mouse.position.ReadValue()));
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame){InteractKeyPresses++;LastInteractBlocked=blocked;if(!blocked)Interact();}
        }
        public bool IssuePointer(Vector2 screenPoint, bool primary, bool blockedByUi)
        {
            if (blockedByUi || (ModalOpen?.Invoke()??false) || !Physics.Raycast(view.ScreenPointToRay(screenPoint), out var hit, 150)) return false;
            if (session.Mode != PrototypeMode.Exploration) return session.RouteWorldClick(hit.point, hit.collider, primary);
            if (primary)
            {
                var unit = hit.collider.GetComponentInParent<ExplorationUnit>();
                if (unit != null) { hero.Select(true); session.Say("Personaje seleccionado. Clic derecho para caminar."); return true; }
                Target = hit.collider.GetComponentInParent<InteractionPoint>();
                if (Target != null) { session.Say(Target.DisplayName + ": pulsa E o Interactuar."); return true; }
                hero.Select(false);
                return false;
            }
            if (!hero.IsSelected) { session.Say("Selecciona primero tu personaje con clic izquierdo."); return false; }
            var accepted = hero.TryMoveTo(hit.point);
            if (!accepted) session.Say("No hay un camino accesible hasta ese punto.");
            return accepted;
        }
        public bool Interact()
        {
            if(session.Paused||session.WorldUiOpen||(ModalOpen?.Invoke()??false)||session.Mode!=PrototypeMode.Exploration)return false;
            if(session.Rules.UxExperience){var selected=RefreshNearby();return selected!=null&&selected.TryInteract(hero,session);}
            if(Target!=null&&(!NearbyInteraction||Vector3.Distance(Target.transform.position,hero.transform.position)<=2.2f))return Target.TryInteract(hero,session);
            if(!NearbyInteraction)return false;
            var count=Physics.OverlapSphereNonAlloc(hero.transform.position,2.2f,nearby,~0,QueryTriggerInteraction.Collide);InteractionPoint closest=null;var distance=float.MaxValue;
            for(var i=0;i<count;i++)
            {var point=nearby[i].GetComponentInParent<InteractionPoint>();if(point==null)continue;var d=Vector3.Distance(point.transform.position,hero.transform.position);if(d<=2.2f&&d<distance){closest=point;distance=d;}}
            if(closest==null)return false;Target=closest;return Target.TryInteract(hero,session);
        }
    }
}
