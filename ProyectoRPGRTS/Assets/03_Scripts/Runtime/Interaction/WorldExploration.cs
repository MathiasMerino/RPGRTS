using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Aethernia
{
    // Contenido del mundo conectado a los controladores existentes; Facts pertenece a la partida.
    [DefaultExecutionOrder(550)]
    public sealed partial class WorldExploration : MonoBehaviour
    {
        public DemoFlow Flow;
        public WorldInteractable[] Interactions = Array.Empty<WorldInteractable>();
        public Vector3[] Discoveries = Array.Empty<Vector3>();
        public string[] DiscoveryNames = Array.Empty<string>();
        public Transform[] RoutePoints = Array.Empty<Transform>();
        public bool DialogueOpen => dialogue != null && dialogue.gameObject.activeSelf;
        public string DialogueText => body == null ? "" : body.text;
        public Button ConfirmButton { get; private set; }
        public Button CloseButton { get; private set; }
        public Button AlternativeButton { get; private set; }
        private readonly Dictionary<string,InteractionPoint> landmarks = new Dictionary<string,InteractionPoint>();
        private Action alternative;
        private RectTransform dialogue, indicator;
        private TMP_Text body, title, focusLabel, neutralPortrait;
        private Image portrait;
        private Action confirm;
        private InteractionPoint speaker;
        private float nextScan;
        private PlayerCommands commands;
        private PrototypeState observedState;
        private void OnEnable()
        { if (Flow != null) { Flow.Session.AdditionalInteraction += Interact; Flow.Started += Loaded; } }
        private void OnDisable()
        { if (Flow != null) { Flow.Session.AdditionalInteraction -= Interact; Flow.Started -= Loaded; Close(); } }
        private void Loaded() { Close(); nextScan = 0; observedState=Flow.Session.State; }
        private void Start()
        {
            commands = FindAnyObjectByType<PlayerCommands>();
            var ui = new DemoWidgets(Flow.Front.Font, Flow.Front.Theme.ButtonSprite, Flow.Front.Presentation);
            dialogue = DemoWidgets.Rect(Flow.ExistingHud.transform, "World conversation", 0, 0, 650, 288);
            dialogue.anchorMin = dialogue.anchorMax = dialogue.pivot = new Vector2(.5f, .5f);
            dialogue.anchoredPosition = Vector2.zero; ui.Surface(dialogue, new Color(.06f, .06f, .045f, .98f));
            portrait = DemoWidgets.Fill(DemoWidgets.Rect(dialogue, "Provisional portrait", 18, 18, 82, 110), Color.white);
            portrait.preserveAspect = true;
            if(Flow.Session.Rules.CoreRefine)
                neutralPortrait=ui.Text(dialogue,"Neutral identity","",18,18,82,110,28);
            if(neutralPortrait!=null)neutralPortrait.alignment=TextAlignmentOptions.Center;
            title = ui.Text(dialogue, "Speaker", "", 120, 18, 510, 36, 23);
            body = ui.Text(dialogue, "Dialogue", "", 120, 62, 510, 152, 18);
            ConfirmButton = ui.Button(dialogue, "World confirm", "Continuar", 120, 230, 300, 40, Confirm);
            CloseButton = ui.Button(dialogue, "World close", "Cerrar · Esc", 440, 230, 190, 40, Close);
            AlternativeButton = ui.Button(dialogue, "World alternative", "", 288, 230, 166, 40, () => ConfirmChoice(alternative));
            AlternativeButton.gameObject.SetActive(false);
            if(Flow.Session.Rules.FrontierCampaign)
                foreach(var point in FindObjectsByType<InteractionPoint>(FindObjectsInactive.Include))
                    landmarks.TryAdd(point.FactId,point);
            dialogue.gameObject.SetActive(false);
            indicator = DemoWidgets.Rect(Flow.ExistingHud.transform, "Nearby world focus", 0, 0, 42, 32);
            indicator.anchorMin = indicator.anchorMax = indicator.pivot = new Vector2(.5f, .5f);
            focusLabel = ui.Text(indicator, "Focus", "[E]", 0, 0, 42, 32, 18);
            focusLabel.alignment = TextAlignmentOptions.Center; focusLabel.color = ui.Gold;
            indicator.gameObject.SetActive(false);
            var quest=FindAnyObjectByType<LostCargoView>();
            var qr=(RectTransform)quest.Dialogue.transform;qr.anchorMin=qr.anchorMax=qr.pivot=new Vector2(.5f,.5f);qr.anchoredPosition=Vector2.zero;qr.sizeDelta=new Vector2(650,300);
            var picture=DemoWidgets.Fill(DemoWidgets.Rect(qr,"Resident provisional portrait",24,76,88,120),Color.white);picture.preserveAspect=true;picture.raycastTarget=false;picture.sprite=Flow.Front.Catalog.FindEntry("monk")?.PortraitSprite;
            if(Flow.Session.Rules.CoreRefine){picture.enabled=false;ui.Text(qr,"Mara identity","M",24,76,88,120,28).alignment=TextAlignmentOptions.Center;}
            var source=quest.DialogueText.rectTransform;source.anchoredPosition=new Vector2(132,-76);source.sizeDelta=new Vector2(490,136);
            var bridge=quest.Dialogue.GetComponentsInChildren<LegacySdfPresenter>(true).FirstOrDefault(b=>b.Source==quest.DialogueText);
            if(bridge!=null)
            {
                var rect=bridge.Target.rectTransform;
                if(Flow.Session.Rules.CoreRefine&&rect.parent==source)
                {
                    rect.anchorMin=Vector2.zero;
                    rect.anchorMax=Vector2.one;
                    rect.offsetMin=rect.offsetMax=Vector2.zero;
                }
                else
                {
                    rect.anchorMin=source.anchorMin;rect.anchorMax=source.anchorMax;rect.pivot=source.pivot;
                    rect.anchoredPosition=source.anchoredPosition;rect.sizeDelta=source.sizeDelta;
                }
            }
        }
        private void Update()
        {
            if(observedState!=Flow.Session.State){observedState=Flow.Session.State;Close();nextScan=0;}
            if (DialogueOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) Close();
            if (!Flow.IsPlaying || Flow.Session.Paused || Flow.Session.Mode != PrototypeMode.Exploration) { if (DialogueOpen) Close(); return; }
            if (DialogueOpen || Flow.Session.WorldUiOpen || Time.unscaledTime < nextScan) return;
            nextScan = Time.unscaledTime + .25f;
            for (var i = 0; i < Mathf.Min(Discoveries.Length, DiscoveryNames.Length); i++)
            {
                var fact = "ux02-discovery-" + i;
                if (!Flow.Session.HasFact(fact) && Vector3.Distance(Flow.Director.Hero.transform.position, Discoveries[i]) < 5)
                { Flow.Session.State.Facts.Add(fact); Flow.Session.Say("Lugar descubierto: " + DiscoveryNames[i] + "."); }
            }
        }
        private void LateUpdate()
        {
            if (indicator == null || commands == null) return;
            if(DialogueOpen&&(Flow.ExistingHud.CompactView.WindowOpen||Flow.ExistingHud.CompactView.HelpOpen))Close();
            var point = commands.CurrentInteraction;
            var visible = Flow.IsPlaying && Flow.Session.Mode == PrototypeMode.Exploration && !Flow.Session.Paused && !Flow.Session.WorldUiOpen &&
                !(commands.ModalOpen?.Invoke() ?? false) && point != null;
            indicator.gameObject.SetActive(visible);
            if (!visible) return;
            var camera = Flow.CameraRig.GetComponent<Camera>(); var screen = camera.WorldToScreenPoint(point.transform.position + Vector3.up * 1.8f);
            var canvas = Flow.ExistingHud.GetComponent<Canvas>(); var rect = (RectTransform)canvas.transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screen, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out var local);
            indicator.anchoredPosition = local; indicator.gameObject.SetActive(screen.z > 0 && Mathf.Abs(local.x) < rect.rect.width * .5f - 24 && local.y < rect.rect.height * .5f - 90 && local.y > -rect.rect.height * .5f + 90);
        }
        public void Close()
        {
            if (dialogue != null) dialogue.gameObject.SetActive(false);
            if (Flow != null && speaker != null) Flow.Session.WorldUiOpen = false;
            speaker = null; confirm = null; alternative = null;
        }
        public void Confirm() => ConfirmChoice(confirm);
        private void ConfirmChoice(Action action)
        {
            if (!DialogueOpen || speaker == null) return;
            var s = Flow.Session;
            if (s.Mode != PrototypeMode.Exploration || s.Paused || Vector3.Distance(speaker.transform.position, Flow.Director.Hero.transform.position) > speaker.Reach || !commands.HasInteractionSight(speaker))
            { Close(); s.Say("La conversación terminó: acércate de nuevo."); return; }
            Close(); action?.Invoke();
        }
        public bool TalkCaptain()
        {
            var state=Flow.Session.State;
            if(Flow.Session.Rules.FrontierCampaign&&state.Demo?.Chapter<DemoChapter.Settlement)
            {
                var step=FrontierChapter.Step(state);
                if(step!=FrontierStep.Patrol)
                    return Show(Flow.Patrol,"Darien · capitán",FrontierChapter.Objective(state),null,null,"warrior");
                return Show(Flow.Patrol,"Darien · capitán",
                    "El cruce vuelve a recibir víveres. Necesito abrir paso al campamento para que alguien pueda investigar las ruinas. Despliega primero la patrulla y decide cuándo atacar.",
                    "Preparar patrulla",()=>Flow.BeginPatrol(),"warrior");
            }
            if(Flow.Session.Rules.TacticalWarfare&&Flow.Session.State.Demo?.Chapter>=DemoChapter.FreeExploration)
                return Show(Flow.Patrol,"Capitán de patrulla",
                    "Defensa del paso fronterizo. Recluta en el cuartel y elige el despliegue en Ejército. Asegura el Paso del bosque y marcha al Puesto del Umbral. Allí podrás reconocer el terreno antes de ordenar la ofensiva.",
                    "Practicar patrulla",()=>Flow.BeginPatrol(),"warrior");
            return Show(Flow.Patrol, "Capitán de patrulla", DemoFlow.CanVisitPatrol(Flow.Session)
                ? "El puesto oriental tiene arqueros e infantería. Antes del ataque, selecciona y mueve nuestra patrulla. El enemigo sólo reaccionará cuando confirmes una orden de ataque."
                : "Primero devuelve las provisiones al habitante. Puedes reconocer el sendero y las ruinas mientras tanto.",
                DemoFlow.CanVisitPatrol(Flow.Session) ? "Preparar patrulla" : null, () => Flow.BeginPatrol(), "warrior");
        }
        private bool Show(InteractionPoint point, string name, string message, string action, Action callback, string classId, string alternativeText=null, Action alternativeCallback=null)
        {
            if (DialogueOpen || Flow.Session.WorldUiOpen || dialogue == null) return false;
            Flow.ExistingHud.ClosePanels(); Flow.Director.Hero.Stop(); speaker = point; confirm = callback;
            title.text = name; body.text = message; ConfirmButton.gameObject.SetActive(action != null);
            ConfirmButton.GetComponentInChildren<TMP_Text>().text = action ?? "Continuar";
            alternative=alternativeCallback;
            AlternativeButton.gameObject.SetActive(alternative!=null);
            AlternativeButton.GetComponentInChildren<TMP_Text>().text=alternativeText??"";
            ((RectTransform)ConfirmButton.transform).sizeDelta=new Vector2(alternative!=null?166:300,40);
            ((RectTransform)CloseButton.transform).anchoredPosition=new Vector2(alternative!=null?460:440,-230);
            ((RectTransform)CloseButton.transform).sizeDelta=new Vector2(alternative!=null?170:190,40);
            var entry = Flow.Front.Catalog.Entries.FirstOrDefault(e => e.ClassId == classId);
            portrait.sprite = entry?.PortraitSprite; portrait.enabled = portrait.sprite != null&&!Flow.Session.Rules.CoreRefine;
            if(neutralPortrait!=null)neutralPortrait.text=name.Substring(0,1);
            Flow.Session.WorldUiOpen = true; dialogue.gameObject.SetActive(true); dialogue.SetAsLastSibling();
            return true;
        }
        private bool? Interact(string id, string name)
        {
            var item = Interactions.FirstOrDefault(i => i != null && i.GetComponent<InteractionPoint>().FactId == id);
            if (item == null) return null;
            var s = Flow.Session; var point = item.GetComponent<InteractionPoint>();
            if (DialogueOpen || s.WorldUiOpen || s.Paused || item.Consumed(s)) return false;
            if (s.Rules.LivingWorld && TryLivingDialogue(point, out var activity)) return activity;
            if(s.Rules.FrontierCampaign&&TryCampaignDialogue(point,out var handled))return handled;
            switch (item.Kind)
            {
                case WorldInteractionKind.Merchant:
                    return Show(point, "Mercader del almacén", "Tengo madera para reparar el campamento. Intercambio 2 de madera por 5 de oro. El coste se descuenta de tu bolsa.", "Comprar madera · 5 oro", () => s.CompleteInteraction("market", "Mercado"), "monk");
                case WorldInteractionKind.Recruiter:
                    var party=FindAnyObjectByType<PartyWorld>();
                    return Show(point, "Enlace de reclutamiento", "Los compañeros viajan contigo a las ruinas. La tropa militar se recluta desde el campamento con cuartel; ambos sistemas conservan salud, raza y cultura.", "Compañero · "+party.Rules.GoldCost("swordsman")+" oro / "+party.Rules.FoodCost("swordsman")+" alimento", () => party.Recruit("swordsman"), "warrior");
                case WorldInteractionKind.Guard:
                    return Show(point, "Guardia del sendero", item.Description, null, null, "warrior");
                case WorldInteractionKind.Chest:
                case WorldInteractionKind.Materials:
                    s.State.Facts.Add(id);
                    if (item.Kind == WorldInteractionKind.Chest) s.State.Gold = Math.Min(1000000, s.State.Gold + 8);
                    else s.State.Wood = Math.Min(1000000, s.State.Wood + 3);
                    s.Say(item.Kind == WorldInteractionKind.Chest ? "Alijo encontrado: +8 oro. Recompensa única." : "Ramas secas recogidas: +3 madera. Recurso único."); return true;
                case WorldInteractionKind.Door:
                    return Show(point, name, item.Description, "Pedir provisiones · una vez", () =>
                    { if (s.HasFact(id)) { s.Say("Ya recibiste las provisiones del almacén."); return; } s.State.Facts.Add(id); s.State.Food = Math.Min(1000000, s.State.Food + 2); s.Say("Almacén abierto: +2 alimento. Entrega única registrada."); }, "monk");
                case WorldInteractionKind.Store:
                    return Show(point, name, "Recursos compartidos con construcción y ejército: " + s.State.Gold + " oro, " + s.State.Wood + " madera, " + s.State.Food + " alimento. El centro y el taller habilitan la producción existente.", null, null, "monk");
                default: return Show(point, name, item.Description, null, null, "mage");
            }
        }
        public Vector3 CampaignTarget(FrontierStep step, bool inside, Vector3 fallback)
        {
            var state=Flow.Session.State;
            string id;
            switch(step)
            {
                case FrontierStep.Trail: id=FrontierChapter.Sign; break;
                case FrontierStep.Supply: id=FrontierChapter.Merchant; break;
                case FrontierStep.Patrol: id="demo-patrol"; break;
                case FrontierStep.Ruins:
                    id=!inside?"umbral-entry":!state.Facts.Contains("umbral-battle-1")?"umbral-battle-1":
                        !state.Facts.Contains("umbral-battle-2")?"umbral-battle-2":"umbral-objective";
                    break;
                case FrontierStep.Debrief: id=inside?"umbral-exit":FrontierChapter.Liaison; break;
                case FrontierStep.Return: id=FrontierChapter.Liaison; break;
                case FrontierStep.Conflict:
                    id=state.Territories.Exists(t=>t.Id=="crossing"&&t.Owner==state.FactionId)?
                        "territory-outpost":"territory-crossing";
                    break;
                default: return fallback;
            }
            return landmarks.TryGetValue(id,out var point)?point.transform.position:fallback;
        }

        private bool TryCampaignDialogue(InteractionPoint point,out bool handled)
        {
            var game=Flow.Session;
            var state=game.State;
            handled=false;
            switch(point.FactId)
            {
                case FrontierChapter.Sign:
                    var read=state.Facts.Contains(FrontierChapter.Clue);
                    var canRead=!read&&(state.Facts.Contains(LostCargoQuest.Completed)||ExpeditionOperations.Ready(state));
                    handled=Show(point,"Señal del cruce",
                        "Oeste: patrulla. Sur: ruinas. Hay rodadas recientes junto a un símbolo tallado. Elia conoce el camino y a quienes lo usan.",
                        canRead?"Anotar pista":null,
                        ()=>ApplyCampaign(()=>FrontierChapter.ReadClue(game.State),"Pista anotada. Habla con Elia."),"mage");
                    return true;
                case FrontierChapter.Merchant:
                    var choice=FrontierChapter.Decision(state);
                    if(state.Facts.Contains(FrontierChapter.Clue)&&choice==RoadChoice.None)
                    {
                        handled=Show(point,"Elia · mercader",
                            "Reconozco el símbolo. Lo usan en los caminos del Umbral. Si llevas el informe a la guardia, te doy 3 víveres. Si financias el arreglo del paso, 5 de oro liberan 6 de madera para tus obras.",
                            "Entregar informe",()=>ApplyCampaign(()=>FrontierChapter.ChooseRoad(game.State,RoadChoice.Report),"Informe entregado. +3 alimento y +5 reputación con la Liga."),
                            "monk","Reparar · 5 oro",()=>ApplyCampaign(()=>FrontierChapter.ChooseRoad(game.State,RoadChoice.Repair),"Reparación financiada. −5 oro y +6 madera."));
                        return true;
                    }
                    var memory=choice==RoadChoice.Report?"La guardia agradece tu informe. ":choice==RoadChoice.Repair?"Tus monedas ayudaron a reparar el paso. ":
                        state.Facts.Contains(LostCargoQuest.Completed)?"Mara ya recibió el cargamento. ":"";
                    if (game.Rules.LivingWorld)
                    {
                        handled = ShowMarket(point, 0, memory);
                        return true;
                    }
                    handled=Show(point,"Elia · mercader",memory+"Cambio 2 de madera por 5 de oro.", "Comprar madera · 5 oro",
                        ()=>game.CompleteInteraction("market","Mercado"),"monk");
                    return true;
                case "ux02-guard":
                    handled=Show(point,"Nilo · guardia",
                        game.Rules.LivingWorld&&state.Facts.Contains(FrontierContracts.At(2).Completed)?"El último centinela ha caído. Ahora podemos vigilar el camino desde fuera.":
                        game.Rules.LivingWorld&&FrontierContracts.WatchAvailable(state)?"El último centinela sigue dentro de las ruinas. Tú decides si bajas con compañía.":
                        FrontierChapter.Decision(state)==RoadChoice.Report?"Tu informe nos permitió orientar la patrulla. Mantén los ojos en las laderas.":
                        FrontierChapter.Decision(state)==RoadChoice.Repair?"El paso reparado nos permite mover suministros de nuevo.":
                        "Alguien ha movido la carga por el cruce. Lee la señal y pregunta a Elia cuando hayas devuelto el arcón.",
                        null,null,"warrior");
                    return true;
                case FrontierChapter.Liaison:
                    var party=FindAnyObjectByType<PartyWorld>();
                    var canDebrief=state.Facts.Contains("umbral-reward")&&!state.Facts.Contains(FrontierChapter.Dossier);
                    var canFinish=state.Facts.Contains(FrontierChapter.Dossier)&&!state.Facts.Contains(FrontierChapter.Finished);
                    var briefing=canDebrief?"Ese sello vincula las marcas de la ruta con el puesto. Déjame registrar lo que encontraste.":
                        canFinish&&FrontierChapter.ConflictResolved(state)?"El resultado del puesto ya consta. Puedes cerrar este capítulo y elegir tu siguiente trabajo.":
                        canFinish?"Puedes asegurar el Paso y marchar al Puesto, pactar en Culturas o cerrar el capítulo por tu cuenta. Las expediciones seguirán abiertas.":
                        state.Facts.Contains(FrontierChapter.Finished)?"El camino vuelve a estar abierto. Puedo ayudarte a reunir un grupo.":"Las ruinas guardan pruebas de lo ocurrido. Si quieres compañía, puedo recomendarte a un espadachín.";
                    var recruit="Reclutar · "+party.Rules.GoldCost("swordsman")+" oro";
                    if(canDebrief||canFinish)
                        handled=Show(point,"Tarek · enlace",briefing,canDebrief?"Entregar hallazgo":"Cerrar capítulo",
                            ()=>ApplyCampaign(()=>canDebrief?FrontierChapter.Debrief(game.State):FrontierChapter.Finish(game.State),
                                canDebrief?"Hallazgo registrado. Decide qué hacer con el puesto.":"Capítulo cerrado. Nuevos caminos disponibles."),
                            "warrior",recruit,()=>party.Recruit("swordsman"));
                    else handled=Show(point,"Tarek · enlace",briefing+" Coste adicional: "+party.Rules.FoodCost("swordsman")+" alimento.",
                        recruit,()=>party.Recruit("swordsman"),"warrior");
                    return true;
                default: return false;
            }
        }

        private void ApplyCampaign(Func<bool> transaction,string message)
        {
            if(!transaction()){Flow.Session.Say("Comprueba los requisitos y tus recursos. La decisión se conserva.");return;}
            if(Flow.Director.Save(Flow.SlotPath))Flow.Session.Say(message);
        }
    }
}
