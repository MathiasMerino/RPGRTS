using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aethernia
{
    public enum PrototypeMode { Exploration, TurnCombat, Construction, Rts, MainMenu, CharacterCreation, Intro }
    public sealed class GameSession : MonoBehaviour
    {
        public event Action Changed;
        public event Action<string> Message;
        [SerializeField] private PrototypeRules rules;
        [SerializeField] private SettlementRules settlementRules;
        public SettlementRules Settlement=>settlementRules;
        public void ConfigureSettlement(SettlementRules value){settlementRules=value;}
        private bool ownsRules;
        private int displayedSecond;
        public PrototypeState State { get; private set; } = new PrototypeState();
        public PrototypeRules Rules => rules;
        private PrototypeState profiledState;private ClassProfile cachedProfile;private int profiledLevel,profiledImpact,profiledDiscipline;private string profiledClass,profiledAccessory,profiledRace;private int profiledIdentity;
        public ClassProfile HeroProfile
        {
            get
            {
                if(rules.Progression==null||State.ProgressionRevision==0)return rules.Find(State.ClassId);
                var impact=ProgressionOperations.Rank(State,"impact");var discipline=ProgressionOperations.Rank(State,"discipline");
                if(profiledIdentity!=State.IdentityRevision||profiledRace!=State.RaceId||profiledState!=State||profiledLevel!=State.HeroLevel||profiledClass!=State.ClassId||profiledImpact!=impact||profiledDiscipline!=discipline||profiledAccessory!=State.Accessory||cachedProfile==null)
                {profiledIdentity=State.IdentityRevision;profiledRace=State.RaceId;profiledState=State;profiledLevel=State.HeroLevel;profiledClass=State.ClassId;profiledImpact=impact;profiledDiscipline=discipline;profiledAccessory=State.Accessory;cachedProfile=ProgressionOperations.ResolveProfile(State,rules);}return cachedProfile;
            }
        }
        public bool EquipItem(string id)
        {if(Mode!=PrototypeMode.Exploration||Paused||!EquipmentOperations.Equip(State,id,rules))return false;cachedProfile=null;Say("Equipado: "+WorldOperations.ItemTitle(id));return true;}
        public bool UnequipItem(ItemKind kind)
        {if(Mode!=PrototypeMode.Exploration||Paused||!EquipmentOperations.Unequip(State,kind,rules))return false;cachedProfile=null;Say("Pieza retirada; bonificaciones recalculadas.");return true;}
        public bool ClaimLoot()
        {if(Mode!=PrototypeMode.Exploration||Paused||!EquipmentOperations.ClaimDungeonLoot(State,rules.Progression))return false;Say("Botín recibido: mineral y talismán. Úsalo en equipo o forja.");return true;}
        public string LastProgressionNotice {get;private set;}="";
        public int AwardForFact(string fact)
        {
            var level=State.HeroLevel;var amount=ProgressionOperations.GrantForFact(State,rules.Progression,fact);
            if(amount>0){LastProgressionNotice="+"+amount+" XP"+(State.HeroLevel>level?" · NIVEL "+State.HeroLevel+" · mejora disponible":"");Changed?.Invoke();}return amount;
        }
        public bool LearnSkill(string id)
        {
            if(Mode!=PrototypeMode.Exploration||Paused||!ProgressionOperations.Learn(State,rules.Progression,id))return false;
            Say("Mejora aprendida. Consulta la ficha para ver su efecto.");return true;
        }
        public Func<Vector3, Collider, bool, bool> WorldCommand;
        public Func<string, bool> TravelRequested;
        public Func<string, string, bool?> AdditionalInteraction;
        public PrototypeMode Mode { get; private set; }
        public bool Paused { get; set; }
        private bool worldUiOpen;
        public bool WorldUiOpen
        {
            get=>worldUiOpen;
            set
            {
                if(worldUiOpen==value)return;
                worldUiOpen=value;
                if(rules!=null&&rules.CoreRefine)Changed?.Invoke();
            }
        }
        public string ModeTitle => Mode == PrototypeMode.Exploration ? "Exploración" : Mode == PrototypeMode.TurnCombat ? "Combate por turnos" : Mode == PrototypeMode.Construction ? "Construcción" : Mode==PrototypeMode.Rts?"Guerra RTS":Mode==PrototypeMode.MainMenu?"Menú principal":Mode==PrototypeMode.CharacterCreation?"Crear personaje":"Prólogo";
        public string LastMessage { get; private set; } = "Selecciona el personaje. Clic derecho: caminar. WASD: cámara. Rueda: zoom.";
        private void Awake()
        {
            if (rules == null) { rules = ScriptableObject.CreateInstance<PrototypeRules>(); ownsRules = true; }
        }
        private void OnDestroy() { if (ownsRules && rules != null) Destroy(rules); }
        public void Configure(PrototypeRules value) { rules = value; }
        private void Update()
        {
            if (Paused || Mode == PrototypeMode.TurnCombat || Mode == PrototypeMode.Rts || Mode == PrototypeMode.MainMenu || Mode == PrototypeMode.CharacterCreation || Mode == PrototypeMode.Intro) return;
            WorldOperations.Advance(State, Time.deltaTime,settlementRules);
            if ((int)State.WorldSeconds != displayedSecond) { displayedSecond = (int)State.WorldSeconds; Changed?.Invoke(); }
        }
        public void SetMode(PrototypeMode mode) { Mode = mode; Say("Modo: " + ModeTitle); }
        public void SelectClass(string id)
        {
            var profile = rules.Find(id);
            if (profile == null || Mode == PrototypeMode.TurnCombat || Mode == PrototypeMode.Rts) return;
            if(rules.Progression!=null&&State.ProgressionRevision==1&&(id==State.ClassId||State.Experience>0)){Say("La clase de esta aventura se conserva; crea otra partida para cambiarla.");return;}
            State.ClassId = id; State.Health = profile.Health; Say("Clase provisional: " + profile.Title);
        }
        public void Rename(string name)
        {
            if(string.IsNullOrWhiteSpace(name))return;
            var next=name.Trim().Substring(0,Math.Min(32,name.Trim().Length));
            if(State.Name==next)return;State.Name=next;Changed?.Invoke();
        }
        public void Say(string message) { LastMessage = message; Message?.Invoke(message); Changed?.Invoke(); }
        public bool CompleteInteraction(string id, string title)
        {
            if (Mode != PrototypeMode.Exploration) { Say("Vuelve a Explorar para interactuar con objetos del mundo."); return false; }
            if(AdditionalInteraction!=null)foreach(Func<string,string,bool?> handler in AdditionalInteraction.GetInvocationList())
            {var extra=handler(id,title);if(extra.HasValue)return extra.Value;}
            if (id == "portal-dungeon" || id == "portal-surface") return TravelRequested != null && TravelRequested(id);
            if (id == "faction-b") { if(rules.Civilizations!=null){var relation=CivilizationOperations.Relation(State,id,rules.Civilizations);Say("Guarnición de Forja · cultura "+rules.Civilizations.Culture("stonehall").Title+" · "+(relation==FactionRelation.Ally?"aliada":relation==FactionRelation.Hostile?"hostil":"neutral")+". Tu raza no determina esta relación.");}else Say("Facción B: relación " + State.ReputationB + ". Las consecuencias se conservan en la partida."); return true; }
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
        public void ReplaceState(PrototypeState state) { State = state;LastProgressionNotice="";ProgressionOperations.Activate(State,rules.Progression);EquipmentOperations.RegisterDungeonLoot(State);cachedProfile=null; Mode = PrototypeMode.Exploration; Say("Partida cargada; hechos y recursos restaurados."); }
    }
}
