using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Aethernia
{
    [Serializable] public sealed class HudBinding
    {
        public string Id;
        public Button Button;
        [NonSerialized] public UnityAction Listener;
    }
    public sealed class VerticalSliceHud : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private PrototypeDirector director;
        [SerializeField] private Text stats;
        [SerializeField] private Text detail;
        [SerializeField] private HudBinding[] bindings = Array.Empty<HudBinding>();
        [SerializeField] private InputField nameInput;
        private float refreshAt;
        public void Configure(GameSession game, PrototypeDirector control, Text resources, Text info, HudBinding[] actions, InputField input)
        { session = game; director = control; stats = resources; detail = info; bindings = actions; nameInput = input; }
        private void OnEnable()
        {
            if (session == null) return;
            session.Changed += Refresh;
            foreach (var binding in bindings)
            {
                var id = binding.Id;
                binding.Listener = () => director.Execute(id.StartsWith("mode-") ? id.Substring(5) : id);
                binding.Button.onClick.AddListener(binding.Listener);
            }
            if (nameInput != null) nameInput.onEndEdit.AddListener(session.Rename);
            Refresh();
        }
        private void OnDisable()
        {
            if (session == null) return;
            session.Changed -= Refresh;
            foreach (var binding in bindings) if (binding.Button != null && binding.Listener != null) binding.Button.onClick.RemoveListener(binding.Listener);
            if (nameInput != null) nameInput.onEndEdit.RemoveListener(session.Rename);
        }
        private void Update() { if (Time.unscaledTime > refreshAt) { refreshAt = Time.unscaledTime + 0.5f; Refresh(); } }
        private void Refresh()
        {
            if (session == null || session.Rules == null) return;
            var s = session.State; var profile = session.Rules.Find(s.ClassId);
            if (nameInput != null && !nameInput.isFocused && nameInput.text != s.Name) nameInput.SetTextWithoutNotify(s.Name);
            stats.text = s.Name + " · " + profile.Title + "  |  Salud " + s.Health + "/" + profile.Health + "  |  Oro " + s.Gold + "  Madera " + s.Wood + "  Alimentos " + s.Food + "  |  Tiempo " + (int)s.WorldSeconds + " s";
            detail.text = session.ModeTitle + " · Talleres " + s.Buildings.Count + "\nFacción A: " + s.ReputationA + " · B: " + s.ReputationB + "\nInventario: " + string.Join(", ", s.Items.ConvertAll(i => WorldOperations.ItemTitle(i.Id) + " ×" + i.Count)) +
                "\nEquipo: " + (s.Weapon == "" ? "sin arma" : "espada") + " / " + (s.Armour == "" ? "sin armadura" : "túnica");
            if (session.Mode == PrototypeMode.TurnCombat && director.Encounter.Battle != null)
                detail.text = "COMBATE POR TURNOS\nEnemigo: " + director.Encounter.Battle.EnemyHealth + "\nMovimiento: " + director.Encounter.Battle.MovementRemaining.ToString("0.0") + " m\nAcción: " + (director.Encounter.Battle.Finished ? "encuentro finalizado" : director.Encounter.Battle.ActionAvailable ? "disponible" : "usada");
            if (session.Mode == PrototypeMode.Rts) detail.text = "GUERRA RTS\nAliados: " + director.Rts.Allies + " · Rivales: " + director.Rts.Enemies + "\nClic izquierdo: selección\nClic derecho: orden de grupo";
            foreach (var binding in bindings)
            {
                if (binding.Id.StartsWith("mode-")) { binding.Button.interactable = !director.IsBusy; continue; }
                var combatAction = binding.Id == "attack" || binding.Id == "guard" || binding.Id == "recover" || binding.Id == "end" || binding.Id == "approach";
                var warAction = binding.Id == "select-all" || binding.Id == "charge";
                var visible = session.Mode == PrototypeMode.TurnCombat ? combatAction || binding.Id == "escalate" :
                    session.Mode == PrototypeMode.Rts ? warAction : !combatAction && !warAction && binding.Id != "escalate";
                binding.Button.gameObject.SetActive(visible);
                binding.Button.interactable = combatAction ? director.Encounter.Battle != null && !director.Encounter.Battle.Finished && !director.Hero.IsMoving :
                    warAction ? director.Rts.Active : binding.Id == "escalate" ? !director.IsBusy && session.HasFact("rpg-victory") : !director.IsBusy;
            }
        }
    }
}
