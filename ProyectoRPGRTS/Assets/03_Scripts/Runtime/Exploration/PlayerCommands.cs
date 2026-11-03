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
        public bool NearbyInteraction { get; set; }
        public bool ExternalRtsPointer { get; set; }
        private readonly Collider[] nearby=new Collider[24];
        private float nextNearby;
        public InteractionPoint CurrentInteraction { get; private set; }
        public InteractionPoint RefreshNearby()
        {
            if(session==null||hero==null||session.Mode!=PrototypeMode.Exploration||session.Paused||session.WorldUiOpen){CurrentInteraction=null;return null;}
            var count=Physics.OverlapSphereNonAlloc(hero.transform.position,2.2f,nearby,~0,QueryTriggerInteraction.Collide);
            InteractionPoint best=null;var score=float.MaxValue;
            for(var i=0;i<count;i++)
            {var candidate=nearby[i].GetComponentInParent<InteractionPoint>();if(candidate==null||!candidate.isActiveAndEnabled||!candidate.Available(session))continue;
             var d=Vector3.Distance(hero.transform.position,candidate.transform.position);if(d>candidate.Reach)continue;var rank=d-(candidate==CurrentInteraction ? .25f : 0);if(rank<score){best=candidate;score=rank;}}
            CurrentInteraction=best;return best;
        }
        public void Configure(Camera camera, ExplorationUnit unit, GameSession game)
        { view = camera; hero = unit; session = game; }
        private void Update()
        {
            if(session.Paused||session.WorldUiOpen||session.Mode==PrototypeMode.MainMenu||session.Mode==PrototypeMode.CharacterCreation||session.Mode==PrototypeMode.Intro)return;
            if(ExternalRtsPointer&&session.Mode==PrototypeMode.Rts)return;
            if(session.Rules.UxExperience&&Time.unscaledTime>=nextNearby){nextNearby=Time.unscaledTime+.1f;RefreshNearby();}
            var mouse = Mouse.current;
            if (mouse == null || view == null) return;
            var blocked = UiInputGuard.BlocksWorldInput;
            if (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)
                IssuePointer(mouse.position.ReadValue(), mouse.leftButton.wasPressedThisFrame, blocked || UiInputGuard.HasUiAt(mouse.position.ReadValue()));
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && !blocked) Interact();
        }
        public bool IssuePointer(Vector2 screenPoint, bool primary, bool blockedByUi)
        {
            if (blockedByUi || !Physics.Raycast(view.ScreenPointToRay(screenPoint), out var hit, 150)) return false;
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
            if(session.Paused||session.Mode!=PrototypeMode.Exploration)return false;
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
