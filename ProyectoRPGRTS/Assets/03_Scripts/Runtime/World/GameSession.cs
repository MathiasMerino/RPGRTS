using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aethernia
{
    public enum PrototypeMode { Exploration, TurnCombat, Construction, Rts }
    public sealed class GameSession : MonoBehaviour
    {
        public event Action Changed;
        public event Action<string> Message;
        [SerializeField] private PrototypeRules rules;
        private bool ownsRules;
        private int displayedSecond;
        public PrototypeState State { get; private set; } = new PrototypeState();
        public PrototypeRules Rules => rules;
        public Func<Vector3, Collider, bool, bool> WorldCommand;
        public Func<string, bool> TravelRequested;
        public Func<string, string, bool?> AdditionalInteraction;
        public PrototypeMode Mode { get; private set; }
        public string ModeTitle => Mode == PrototypeMode.Exploration ? "Exploración" : Mode == PrototypeMode.TurnCombat ? "Combate por turnos" : Mode == PrototypeMode.Construction ? "Construcción" : "Guerra RTS";
        public string LastMessage { get; private set; } = "Selecciona el personaje. Clic derecho: caminar. WASD: cámara. Rueda: zoom.";
        private void Awake()
        {
            if (rules == null) { rules = ScriptableObject.CreateInstance<PrototypeRules>(); ownsRules = true; }
        }
        private void OnDestroy() { if (ownsRules && rules != null) Destroy(rules); }
        public void Configure(PrototypeRules value) { rules = value; }
        private void Update()
        {
            if (Mode == PrototypeMode.TurnCombat || Mode == PrototypeMode.Rts) return;
            WorldOperations.Advance(State, Time.deltaTime);
            if ((int)State.WorldSeconds != displayedSecond) { displayedSecond = (int)State.WorldSeconds; Changed?.Invoke(); }
        }
        public void SetMode(PrototypeMode mode) { Mode = mode; Say("Modo: " + ModeTitle); }
        public void SelectClass(string id)
        {
            var profile = rules.Find(id);
            if (profile == null || Mode == PrototypeMode.TurnCombat || Mode == PrototypeMode.Rts) return;
            State.ClassId = id; State.Health = profile.Health; Say("Clase provisional: " + profile.Title);
        }
        public void Rename(string name)
        { if (!string.IsNullOrWhiteSpace(name)) { State.Name = name.Trim().Substring(0, Math.Min(32, name.Trim().Length)); Changed?.Invoke(); } }
        public void Say(string message) { LastMessage = message; Message?.Invoke(message); Changed?.Invoke(); }
        public bool CompleteInteraction(string id, string title)
        {
            if (Mode != PrototypeMode.Exploration) { Say("Vuelve a Explorar para interactuar con objetos del mundo."); return false; }
            var extra=AdditionalInteraction?.Invoke(id,title);if(extra.HasValue)return extra.Value;
            if (id == "portal-dungeon" || id == "portal-surface") return TravelRequested != null && TravelRequested(id);
            if (id == "faction-b") { Say("Facción B: relación " + State.ReputationB + ". Las consecuencias se conservan en la partida."); return true; }
            if (id == "market") { var traded = WorldOperations.Trade(State); Say(traded ? "Comercio: 5 oro por 2 madera." : "Oro insuficiente."); return traded; }
            if (State.Facts.Contains(id)) { Say("Ya has activado " + title + "."); return false; }
            if (id == "dungeon-objective") { WorldOperations.CompleteQuest(State); Say("Objetivo completado. Facción A +10; Facción B -5. Consecuencia persistente."); return true; }
            if (id != "reference-chest") { Say("Este objeto no tiene una interacción de prototipo configurada."); return false; }
            if (!WorldOperations.CanAdd(State, 2)) { Say("Inventario lleno: libera dos espacios antes de recoger."); return false; }
            WorldOperations.AddItem(State, "training-sword", 1); WorldOperations.AddItem(State, "cloth", 1);
            State.Facts.Add(id); State.Gold += 10; State.Wood += 4;
            Say(title + ": interacción completada.");
            return true;
        }
        public bool HasFact(string id) => State.Facts.Contains(id);
        public bool RouteWorldClick(Vector3 point, Collider collider, bool primary) => WorldCommand != null && WorldCommand(point, collider, primary);
        public void ReplaceState(PrototypeState state) { State = state; Mode = PrototypeMode.Exploration; Say("Partida cargada; hechos y recursos restaurados."); }
    }
}
