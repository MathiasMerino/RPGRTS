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
        public void Configure(Camera camera, ExplorationUnit unit, GameSession game)
        { view = camera; hero = unit; session = game; }
        private void Update()
        {
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
        public bool Interact() => session.Mode == PrototypeMode.Exploration && Target != null && Target.TryInteract(hero, session);
    }
}
