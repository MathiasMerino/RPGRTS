using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    public sealed class ExplorationHud : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private PlayerCommands commands;
        [SerializeField] private Text log;
        [SerializeField] private Button interact;
        public Button ActionButton => interact;
        public void Configure(GameSession game, PlayerCommands input, Text message, Button action)
        { session = game; commands = input; log = message; interact = action; }
        private void OnEnable()
        {
            if (session == null || interact == null) return;
            session.Changed += Refresh;
            interact.onClick.AddListener(Interact);
            Refresh();
        }
        private void OnDisable()
        {
            if (session != null) session.Changed -= Refresh;
            if (interact != null) interact.onClick.RemoveListener(Interact);
        }
        private void Interact()
        {
            if (commands.Target == null) session.Say("Selecciona un objeto cercano del escenario.");
            else commands.Interact();
        }
        private void Refresh()
        {
            if (log != null) log.text = session.LastMessage;
            if (interact != null) interact.interactable = session.Mode == PrototypeMode.Exploration;
        }
    }
}
