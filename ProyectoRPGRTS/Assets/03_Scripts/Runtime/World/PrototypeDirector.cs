using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aethernia
{
    public sealed class PrototypeDirector : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private ExplorationUnit hero;
        [SerializeField] private IsometricCameraRig cameraRig;
        [SerializeField] private WorldConstruction construction;
        [SerializeField] private TurnEncounter encounter;
        [SerializeField] private RtsBattleController rts;
        public GameSession Session => session;
        public TurnEncounter Encounter => encounter;
        public WorldConstruction Construction => construction;
        public RtsBattleController Rts => rts;
        public ExplorationUnit Hero => hero;
        public string SavePath => Path.Combine(Application.persistentDataPath, "AetherniaPrototype", "save-v1.json");
        public void Configure(GameSession game, ExplorationUnit actor, IsometricCameraRig camera, WorldConstruction builder, TurnEncounter turns, RtsBattleController war)
        { session = game; hero = actor; cameraRig = camera; construction = builder; encounter = turns; rts = war; }
        private void OnEnable()
        {
            if (session == null) return;
            session.WorldCommand = Route; session.TravelRequested = Travel;
            session.Changed += RefreshHero;
        }
        private void OnDisable()
        {
            if (session == null) return;
            session.WorldCommand = null; session.TravelRequested = null; session.Changed -= RefreshHero;
        }
        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.f5Key.wasPressedThisFrame) Save(SavePath);
            if (Keyboard.current.f9Key.wasPressedThisFrame) Load(SavePath);
        }
        private void RefreshHero()
        { if (hero != null && session.Rules != null) hero.Agent.speed = session.Rules.Find(session.State.ClassId).Speed; }
        public bool IsBusy => rts.Active || (encounter.Battle != null && !encounter.Battle.Finished);
        private void ClearEncounters(bool preserveAcceptedStrike = false) { encounter.Clear(preserveAcceptedStrike); rts.Clear(); }
        public bool Execute(string id)
        {
            if (id == "attack" && session.Mode == PrototypeMode.TurnCombat) { encounter.Attack(); return true; }
            if (id == "guard" && session.Mode == PrototypeMode.TurnCombat) { encounter.Guard(); return true; }
            if (id == "recover" && session.Mode == PrototypeMode.TurnCombat) { encounter.Recover(); return true; }
            if (id == "end" && session.Mode == PrototypeMode.TurnCombat) { encounter.EndTurn(); return true; }
            if (id == "approach" && session.Mode == PrototypeMode.TurnCombat) { encounter.Approach(); return true; }
            if (id == "select-all" && rts.Active) { rts.SelectAll(); return true; }
            if (id == "charge" && rts.Active) { rts.SelectAll(); return rts.Command(new Vector3(-7, 0, 13), null, false); }
            if (IsBusy) { session.Say("Termina el encuentro activo antes de cambiar de actividad."); return false; }
            if (id == "combat") { rts.Clear(); return encounter.Begin(); }
            if (id == "rts" || id == "escalate")
            {
                if (id == "escalate" && !session.HasFact("rpg-victory")) { session.Say("Primero gana un encuentro RPG para probar la escalada."); return false; }
                if (!rts.Begin(id == "escalate")) return false;
                encounter.Clear(); hero.Stop();
                cameraRig.SetRegion(Vector3.zero); cameraRig.FocusOn(new Vector3(-9, 0, 13));
                return true;
            }
            if (id == "explore") { ClearEncounters(encounter.Battle != null && encounter.Battle.Won); hero.Stop(); session.SetMode(PrototypeMode.Exploration); cameraRig.SetRegion(hero.transform.position.x > 30 ? new Vector3(50, 0, 0) : Vector3.zero); return true; }
            if (id == "construct") { ClearEncounters(); hero.Stop(); session.SetMode(PrototypeMode.Construction); session.Say("Clic derecho sobre suelo libre: taller 3×3 m; coste 10 oro y 5 madera."); return true; }
            if (id == "save") return Save(SavePath);
            if (id == "load") return Load(SavePath);
            if (id.StartsWith("class-")) { session.SelectClass(id.Substring(6)); return true; }
            bool result;
            if (id == "trade") result = WorldOperations.Trade(session.State);
            else if (id == "sword") result = WorldOperations.Equip(session.State, "training-sword");
            else if (id == "armour") result = WorldOperations.Equip(session.State, "cloth");
            else if (id == "ration") result = WorldOperations.UseRation(session.State, session.Rules.Find(session.State.ClassId).Health);
            else return false;
            var success = id == "trade" ? "Intercambio: 5 oro por 2 madera." : id == "sword" ? "Espada equipada." : id == "armour" ? "Túnica equipada." : "Ración consumida: recuperas hasta 4 puntos.";
            session.Say(result ? success : "No puedes realizar esa acción con los recursos actuales."); return result;
        }
        private bool Route(Vector3 point, Collider collider, bool primary)
        {
            if (session.Mode == PrototypeMode.Construction) return !primary && construction.Place(point);
            if (session.Mode == PrototypeMode.TurnCombat) return !primary && encounter.Move(point);
            return session.Mode == PrototypeMode.Rts && rts.Command(point, collider, primary);
        }
        public bool Travel(string id)
        {
            if (IsBusy || session.Mode != PrototypeMode.Exploration || (id != "portal-dungeon" && id != "portal-surface")) return false;
            var dungeon = id == "portal-dungeon";
            var destination = dungeon ? new Vector3(47, 0, -2) : new Vector3(-9, 0, 5);
            if (!hero.Warp(destination)) return false;
            cameraRig.SetRegion(dungeon ? new Vector3(50, 0, 0) : Vector3.zero);
            session.Say(dungeon ? "Mazmorra de prueba: activa el hito. No hay trama definitiva." : "Asentamiento de prueba. Facción A y Facción B comparten el estado de mundo."); return true;
        }
        public bool Save(string path)
        {
            if (IsBusy || session.Mode == PrototypeMode.TurnCombat || session.Mode == PrototypeMode.Rts)
                return Reject("No puedes guardar durante el modo de combate. Termina el encuentro y vuelve a Explorar.");
            session.State.HeroPosition = hero.transform.position;
            var result = PrototypeSaveStore.Save(path, session.State, session.Rules);
            session.Say(result ? "Partida guardada." : "No se pudo guardar; la partida actual se conserva."); return result;
        }
        public bool Load(string path)
        {
            if (IsBusy) return Reject("No puedes cargar durante un encuentro activo.");
            if (!PrototypeSaveStore.Load(path, session.Rules, out var loaded))
                return Reject("No se pudo cargar: archivo ausente, ilegible o datos incompatibles. La partida actual se conserva.");
            if (!construction.CanRestore(loaded.Buildings))
                return Reject("No se pudo cargar: una construcción guardada se solapa con el escenario. La partida actual se conserva.");
            foreach (var building in loaded.Buildings)
                if (Mathf.Abs(loaded.HeroPosition.x - building.Position.x) < 1.9f && Mathf.Abs(loaded.HeroPosition.z - building.Position.z) < 1.9f)
                    return Reject("No se pudo cargar: el personaje está dentro de un edificio guardado.");
            if (!hero.Warp(loaded.HeroPosition)) return Reject("No se pudo cargar: posición del personaje fuera de terreno navegable. La partida actual se conserva.");
            ClearEncounters(); hero.Stop();
            session.ReplaceState(loaded); construction.Restore();
            cameraRig.SetRegion(loaded.HeroPosition.x > 30 ? new Vector3(50, 0, 0) : Vector3.zero);
            return true;
        }
        private bool Reject(string reason) { session.Say(reason); return false; }
    }
}
