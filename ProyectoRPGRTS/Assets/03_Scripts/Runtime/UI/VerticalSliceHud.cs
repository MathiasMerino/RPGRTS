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
        [NonSerialized] public Text Label;
    }
    public enum ExplorationPage { Overview, Character, Inventory, Session }
    public sealed class VerticalSliceHud : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private PrototypeDirector director;
        [SerializeField] private Text stats;
        [SerializeField] private Text detail;
        [SerializeField] private HudBinding[] bindings = Array.Empty<HudBinding>();
        [SerializeField] private InputField nameInput;
        [SerializeField] private MedievalUiTheme theme;
        [SerializeField] private Text controls;
        [SerializeField] private GameObject characterPanel;
        [SerializeField] private GameObject explorationPages;
        [SerializeField] private RectTransform actionGrid;
        [SerializeField] private ClassReferenceCard classReference;
        [SerializeField] private MedievalHudView compactView;
        public MedievalHudView CompactView => compactView;
        public Button FindControl(string id){foreach(var binding in bindings)if(binding.Id==id)return binding.Button;return null;}
        public void ConfigureCompact(MedievalHudView view,HudBinding[] extra)
        { compactView=view;var all=new HudBinding[bindings.Length+extra.Length];Array.Copy(bindings,all,bindings.Length);Array.Copy(extra,0,all,bindings.Length,extra.Length);bindings=all; }
        private float refreshAt;
        private PrototypeMode lastMode;
        private bool layoutApplied;
        private bool refreshing,refreshPending;
        private UnityEngine.InputSystem.InputAction closeAction;
        public ExplorationPage ActivePage { get; private set; }
        public void ConfigureReference(ClassReferenceCard card) { classReference=card; }
        public void Configure(GameSession game, PrototypeDirector control, Text resources, Text info, HudBinding[] actions, InputField input)
        { session = game; director = control; stats = resources; detail = info; bindings = actions; nameInput = input; }
        public void ConfigureContext(MedievalUiTheme style, Text help, GameObject character, GameObject pages, RectTransform grid, HudBinding[] pageButtons)
        {
            theme = style; controls = help; characterPanel = character; explorationPages = pages; actionGrid = grid;
            var all = new HudBinding[bindings.Length + pageButtons.Length];
            Array.Copy(bindings, all, bindings.Length); Array.Copy(pageButtons, 0, all, bindings.Length, pageButtons.Length); bindings = all;
        }
        public void SelectPage(ExplorationPage page)
        { if (session.Mode == PrototypeMode.Exploration) { ActivePage = page; if(compactView!=null) compactView.Open(); Refresh(); } }
        public void ClosePanels() { if(compactView!=null) compactView.Close(); Refresh(); }
        private void Dispatch(string id)
        {
            if(compactView!=null)
            {
                if(id=="close") {ClosePanels();return;}
                if(id=="help") {compactView.ToggleHelp();Refresh();return;}
                if(id=="back") {director.Execute("explore");return;}
                if(id.StartsWith("item-")) {compactView.Inventory.Select(id.Substring(5));Refresh();return;}
                if(id==PageId()&&compactView.WindowOpen) {ClosePanels();return;}
            }
            if (id == "page-overview") SelectPage(ExplorationPage.Overview);
            else if (id == "page-character") SelectPage(ExplorationPage.Character);
            else if (id == "page-inventory") SelectPage(ExplorationPage.Inventory);
            else if (id == "page-session") SelectPage(ExplorationPage.Session);
            else director.Execute(id.StartsWith("mode-") ? id.Substring(5) : id);
        }
        private void OnEnable()
        {
            if (session == null) return;
            if(compactView!=null)
            {
                closeAction=new UnityEngine.InputSystem.InputAction("CloseWindows",UnityEngine.InputSystem.InputActionType.Button,"<Keyboard>/escape");
                closeAction.performed+=ClosePerformed;closeAction.Enable();
            }
            session.Changed += Refresh;
            foreach (var binding in bindings)
            {
                var id = binding.Id;
                binding.Label = binding.Button.GetComponentInChildren<Text>(true);
                if (theme != null) StyleText(binding.Label, compactView!=null&&binding.Id.StartsWith("page-")?15:theme.ButtonFontSize);
                binding.Listener = () => Dispatch(id);
                binding.Button.onClick.AddListener(binding.Listener);
            }
            if (nameInput != null) nameInput.onEndEdit.AddListener(session.Rename);
            if (theme != null)
            {
                if(compactView==null) StyleText(detail, theme.BodyFontSize); StyleText(controls, theme.BodyFontSize);
                StyleText(stats, theme.BodyFontSize + 1);
                if (nameInput != null) StyleText(nameInput.textComponent, theme.BodyFontSize + 1);
            }
            Refresh();
        }
        private void OnDisable()
        {
            if(closeAction!=null){closeAction.performed-=ClosePerformed;closeAction.Dispose();closeAction=null;}
            if (session == null) return;
            session.Changed -= Refresh;
            foreach (var binding in bindings) if (binding.Button != null && binding.Listener != null) binding.Button.onClick.RemoveListener(binding.Listener);
            if (nameInput != null) nameInput.onEndEdit.RemoveListener(session.Rename);
        }
        private void Update()
        {
            if (refreshPending || Time.unscaledTime > refreshAt) { refreshPending=false;refreshAt = Time.unscaledTime + 0.5f; Refresh(); }
        }
        private void ClosePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context) { ClosePanels(); }
        private void Refresh()
        {
            if(!isActiveAndEnabled)return;
            if(refreshing){refreshPending=true;return;}
            refreshing=true;try{RefreshPresentation();}finally{refreshing=false;}
        }
        private void RefreshPresentation()
        {
            if (session == null || session.Rules == null) return;
            if(compactView!=null) { RefreshCompact();return; }
            if (!layoutApplied || lastMode != session.Mode)
            {
                ActivePage = ExplorationPage.Overview; lastMode = session.Mode; layoutApplied = true;
                if (theme != null && actionGrid != null)
                {
                    var top = theme.Padding + (session.Mode == PrototypeMode.Exploration ? theme.ActionButtonSize.y * 2 + theme.Spacing * 3 : 0);
                    detail.rectTransform.anchoredPosition = new Vector2(0, -top);
                    actionGrid.anchoredPosition = new Vector2(0, -top - detail.rectTransform.sizeDelta.y - theme.Spacing * 2);
                }
            }
            var exploring = session.Mode == PrototypeMode.Exploration;
            if (explorationPages != null) explorationPages.SetActive(exploring);
            if (characterPanel != null) characterPanel.SetActive(exploring && ActivePage == ExplorationPage.Character);
            var s = session.State; var profile = session.HeroProfile;
            if(classReference!=null) classReference.Refresh(exploring && ActivePage==ExplorationPage.Character,profile);
            if (nameInput != null && !nameInput.isFocused && nameInput.text != s.Name) nameInput.SetTextWithoutNotify(s.Name);
            stats.text = s.Name + " · " + profile.Title + "  |  Salud " + s.Health + "/" + profile.Health + "  |  Oro " + s.Gold + "  Madera " + s.Wood + "  Alimentos " + s.Food + "  |  Tiempo " + (int)s.WorldSeconds + " s";
            var equipment = "Equipo: " + (s.Weapon == "" ? "sin arma" : "espada") + " / " + (s.Armour == "" ? "sin armadura" : "túnica");
            detail.text = Heading("RESUMEN") + "\nFacción A: " + s.ReputationA + " · B: " + s.ReputationB + "\n" + equipment + "\nTalleres: " + s.Buildings.Count + "\nObjetivo: " + (session.HasFact("dungeon-objective") ? "completado" : "pendiente");
            if (exploring && ActivePage == ExplorationPage.Character)
                detail.text = Heading("PERSONAJE") + "\n" + profile.Title + " · Salud " + s.Health + "/" + profile.Health + "\nNombre: campo de la izquierda.\nCambiar clase restaura salud.\nRegla provisional del ensayo.";
            if (exploring && ActivePage == ExplorationPage.Inventory)
                detail.text = Heading("INVENTARIO") + "\n" + string.Join(", ", s.Items.ConvertAll(i => WorldOperations.ItemTitle(i.Id) + " ×" + i.Count)) + "\n" + equipment + "\nComercio: 5 oro → 2 madera.";
            if (exploring && ActivePage == ExplorationPage.Session)
                detail.text = Heading("PARTIDA") + "\nGuardado manual.\nF5: guardar · F9: cargar\nPersonaje, equipo, mundo y reloj.\nDisponible fuera de combate.";
            if (session.Mode == PrototypeMode.Construction)
                detail.text = Heading("CONSTRUCCIÓN") + "\nTaller: 10 oro + 5 madera\nHuella: 3 × 3 metros\nClic der.: colocar en suelo libre.\n10 s: 1 alimento → 1 madera.";
            if (session.Mode == PrototypeMode.TurnCombat && director.Encounter.Battle != null)
                detail.text = Heading("COMBATE RPG") + "\nIniciativa: " + director.Encounter.Battle.HeroInitiative + " / " + director.Encounter.Battle.EnemyInitiative + "\nObjetivo: rival · Salud " + director.Encounter.Battle.EnemyHealth + "\nMovimiento: " + director.Encounter.Battle.MovementRemaining.ToString("0.0") + " m\nAcción: " + (director.Encounter.Battle.Finished ? "encuentro finalizado" : director.Encounter.Battle.ActionAvailable ? "disponible" : "usada");
            if (session.Mode == PrototypeMode.Rts) detail.text = Heading("GUERRA RTS") + "\nAliados: " + director.Rts.Allies + " · Rivales: " + director.Rts.Enemies + "\nClic izquierdo: selección\nClic derecho: orden de grupo\nAtaque automático cercano.";
            if (controls != null) controls.text = Help(session.Mode);
            foreach (var binding in bindings)
            {
                if (binding.Id.StartsWith("mode-")) { binding.Button.interactable = !director.IsBusy; continue; }
                if (binding.Id.StartsWith("page-"))
                {
                    var selected = binding.Id == PageId();
                    binding.Button.gameObject.SetActive(exploring);
                    if (binding.Label != null) { binding.Label.fontStyle = selected ? FontStyle.Bold : FontStyle.Normal; if (theme != null) binding.Label.color = selected ? theme.Gold : theme.Text; }
                    continue;
                }
                var combatAction = binding.Id == "attack" || binding.Id == "guard" || binding.Id == "recover" || binding.Id == "end" || binding.Id == "approach";
                var warAction = binding.Id == "select-all" || binding.Id == "charge";
                var visible = session.Mode == PrototypeMode.TurnCombat ? combatAction || binding.Id == "escalate" :
                    session.Mode == PrototypeMode.Rts ? warAction : exploring && VisibleOnPage(binding.Id);
                binding.Button.gameObject.SetActive(visible);
                binding.Button.interactable = combatAction ? director.Encounter.Battle != null && !director.Encounter.Battle.Finished && !director.Hero.IsMoving :
                    warAction ? director.Rts.Active : binding.Id == "escalate" ? !director.IsBusy && session.HasFact("rpg-victory") : !director.IsBusy;
            }
        }
        private string Heading(string title) => "<b><size=" + (theme != null ? theme.HeadingFontSize : 20) + ">" + title + "</size></b>";
        private void RefreshCompact()
        {
            if(!layoutApplied||lastMode!=session.Mode)
            {ActivePage=ExplorationPage.Overview;lastMode=session.Mode;layoutApplied=true;compactView.Close();}
            var exploring=session.Mode==PrototypeMode.Exploration;
            if(nameInput!=null&&!nameInput.isFocused&&nameInput.text!=session.State.Name)nameInput.SetTextWithoutNotify(session.State.Name);
            compactView.Render(session,director,ActivePage);
            if(controls!=null)controls.text=Help(session.Mode);
            foreach(var b in bindings)
            {
                var id=b.Id;var visible=true;var ready=true;
                if(id.StartsWith("mode-")) {visible=exploring&&compactView.WindowOpen&&ActivePage==ExplorationPage.Session;ready=!director.IsBusy;}
                else if(id=="back") {visible=!exploring;ready=!director.IsBusy;}
                else if(id=="help") { }
                else if(id=="close") visible=exploring&&compactView.WindowOpen;
                else if(id.StartsWith("page-"))
                {
                    visible=exploring||id=="page-session";ready=exploring;
                    if(b.Label!=null) {var selected=compactView.WindowOpen&&id==PageId();b.Label.fontStyle=selected?FontStyle.Bold:FontStyle.Normal;b.Label.color=selected?theme.Gold:theme.Text;}
                }
                else if(id.StartsWith("item-")) {visible=exploring&&compactView.WindowOpen&&ActivePage==ExplorationPage.Inventory;ready=compactView.Inventory.Count(session.State,id.Substring(5))>0;}
                else
                {
                    var action=id=="attack"||id=="guard"||id=="recover"||id=="end"||id=="approach"||id=="technique";
                    var war=id=="select-all"||id=="charge";
                    visible=session.Mode==PrototypeMode.TurnCombat?action||id=="escalate":session.Mode==PrototypeMode.Rts?war:exploring&&compactView.ActionVisible(id,ActivePage);
                    ready=action?director.Encounter.PlayerCanAct:
                        war?director.Rts.Active:id=="escalate"?!director.IsBusy&&session.HasFact("rpg-victory"):!director.IsBusy;
                    if(action&&id!="end"&&id!="approach"&&director.Encounter.Battle!=null)ready&=director.Encounter.Battle.ActionAvailable;
                    if(id=="recover"&&director.Encounter.Battle!=null)ready&=!director.Encounter.Battle.RecoveryUsed&&session.State.Health<session.HeroProfile.Health;
                    if(id=="ration") ready&=compactView.Inventory.Count(session.State,"ration")>0&&session.State.Health<session.HeroProfile.Health;
                    if(id=="sword"||id=="armour")ready&=compactView.Inventory.Count(session.State,id=="sword"?"training-sword":"cloth")>0;
                    if(id=="trade")ready&=session.State.Gold>=5;
                    if(id.StartsWith("class-")&&b.Label!=null)
                    {
                        var chosen=id=="class-"+session.State.ClassId;
                        b.Label.text=(chosen?"• ":"")+session.Rules.Find(id.Substring(6)).Title;
                        b.Label.fontStyle=chosen?FontStyle.Bold:FontStyle.Normal;
                    }
                }
                if(session.Rules.Progression!=null&&(id.StartsWith("class-")||id.StartsWith("item-")||id=="sword"||id=="armour"||id=="ration"||id=="trade"))visible=false;
                if(session.Rules.Progression!=null&&director.Encounter.InGroup&&(id=="attack"||id=="guard"||id=="recover"||id=="end"||id=="approach"))visible=false;
                if(id=="technique")
                {
                    var skill=session.HeroProfile.Combat;if(director.Encounter.InGroup){var g=director.Encounter.Group;ready=!g.Finished&&g.Current.Hero&&g.ActionAvailable&&!director.Hero.IsMoving;}ready=ready&&skill!=null&&skill.TechniqueUnlocked&&director.Encounter.TechniqueFocus>=skill.TechniqueCost;
                    if(b.Label!=null)b.Label.text=skill==null||!skill.TechniqueUnlocked?"Técnica · Nv2":"Técnica · "+director.Encounter.TechniqueFocus;
                }
                if(session.Rules.UxExperience&&(id=="mode-combat"||id=="mode-rts"||id=="escalate"||id=="charge"))visible=false;
                b.Button.gameObject.SetActive(visible);b.Button.interactable=ready;
            }
        }
        private void StyleText(Text text, int size)
        { if (text != null) { text.font = theme.Font; text.fontSize = size; text.color = theme.Text; } }
        private string PageId() => ActivePage == ExplorationPage.Character ? "page-character" : ActivePage == ExplorationPage.Inventory ? "page-inventory" : ActivePage == ExplorationPage.Session ? "page-session" : "page-overview";
        private bool VisibleOnPage(string id)
        {
            if (ActivePage == ExplorationPage.Character) return id.StartsWith("class-");
            if (ActivePage == ExplorationPage.Inventory) return id == "sword" || id == "armour" || id == "ration" || id == "trade";
            return ActivePage == ExplorationPage.Session && (id == "save" || id == "load");
        }
        private static string Help(PrototypeMode mode)
        {
            if (mode == PrototypeMode.TurnCombat) return "COMBATE RPG\nClic der.: mover (hasta 4 m)\nEspera hasta llegar.\nAtaca con línea libre.\nUna acción por turno.\nFin de turno: actúa el rival.\nWASD / rueda: cámara";
            if (mode == PrototypeMode.Construction) return "CONSTRUCCIÓN\nTaller: 10 oro / 5 madera\nHuella: 3 × 3 metros\nClic der.: colocar\nSuelo libre, sin solapes.\n10 s: alimento → madera\nExplorar: salir de construcción";
            if (mode == PrototypeMode.Rts) return "GUERRA RTS\nClic izq.: elegir aliado\nTodo el grupo: seleccionar\nClic der.: ordenar\nAvanzar: orden de grupo\nAtaque automático cercano\nExplorar al terminar";
            return "EXPLORACIÓN\nClic izq.: seleccionar\nClic der.: caminar\nE: activar objeto cercano\nWASD / flechas: cámara\nRueda: zoom\nPortal gris: mazmorra\nCofre: equipo inicial";
        }
    }
}
