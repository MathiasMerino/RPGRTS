using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    [DefaultExecutionOrder(600)]
    public sealed class DemoRtsOverlay : MonoBehaviour
    {
        public DemoFlow Flow;
        private sealed class Bar {public RectTransform Root;public Image Fill;public TMP_Text Label;}
        private readonly List<RtsUnitStatus> status=new List<RtsUnitStatus>(24);
        private readonly Dictionary<ExplorationUnit,Bar> bars=new Dictionary<ExplorationUnit,Bar>();
        private RectTransform panel,drag;
        private TMP_Text text;
        private TMP_Text orderText;
        private TMP_Text formationLabel, stanceLabel, defendLabel, destinationMarker;
        private Button engage, retreat;
        private Button[] tacticalButtons;
        private RectTransform tacticalPanel;
        private DemoRtsInput input;
        private Button attack,guard,recover,technique;
        private DemoWidgets ui;
        private Canvas canvas;
        private readonly List<Rect> occupiedRects=new List<Rect>(24);
        public int LiveBars=>bars.Count;
        private void ClearBars(){foreach(var bar in bars.Values)if(bar.Root!=null)Destroy(bar.Root.gameObject);bars.Clear();}
        private void OnDisable(){ClearBars();}
        private void Start()
        {
            ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);canvas=Flow.ExistingHud.GetComponent<Canvas>();
            panel=DemoWidgets.Rect(canvas.transform,"Hero RTS commands",456,98,570,124);ui.Surface(panel,new Color(.035f,.043f,.046f,.96f));text=ui.Text(panel,"Group context","",12,6,546,56,18);
            attack=ui.Button(panel,"Hero attack","Atacar",12,72,170,42,()=>Order("attack"));guard=ui.Button(panel,"Hero guard","Guardia",196,72,170,42,()=>Order("guard"));recover=ui.Button(panel,"Hero recover","Recuperar",380,72,170,42,()=>Order("recover"));
            if(Flow.Front.AlphaLayout)
            {
                panel.anchoredPosition=new Vector2(920,-98);panel.sizeDelta=new Vector2(344,168);text.rectTransform.sizeDelta=new Vector2(320,96);text.fontSize=17;
                var buttons=new[]{attack,guard,recover};for(var i=0;i<buttons.Length;i++){var rect=buttons[i].GetComponent<RectTransform>();rect.anchoredPosition=new Vector2(12+i*108,-112);rect.sizeDelta=new Vector2(104,42);buttons[i].GetComponentInChildren<TMP_Text>().fontSize=16;}
            }
            if(Flow.Session.Rules.Progression!=null)
            {panel.sizeDelta=new Vector2(344,222);technique=ui.Button(panel,"Hero technique","Técnica",12,164,320,42,()=>Order("technique"));}
            if(Flow.UxEnabled)
            {
                panel.anchorMin=panel.anchorMax=panel.pivot=new Vector2(.5f,0);panel.anchoredPosition=new Vector2(90,12);panel.sizeDelta=new Vector2(584,84);
                text.rectTransform.anchoredPosition=new Vector2(12,-4);text.rectTransform.sizeDelta=new Vector2(560,30);text.fontSize=16;
                var buttons=new[]{attack,guard,recover,technique};for(var i=0;i<buttons.Length;i++)if(buttons[i]!=null){var rect=(RectTransform)buttons[i].transform;rect.anchoredPosition=new Vector2(12+i*142,-40);rect.sizeDelta=new Vector2(134,36);buttons[i].GetComponentInChildren<TMP_Text>().fontSize=15;}
            }
            input=Flow.GetComponent<DemoRtsInput>();
            if(Flow.Session.Rules.TacticalWarfare)CreateTacticalPanel();
            drag=DemoWidgets.Rect(canvas.transform,"Selection rectangle",0,0,0,0);DemoWidgets.Fill(drag,new Color(.70f,.63f,.36f,.18f),false);
            if(Flow.Session.Rules.WorldExploration){orderText=ui.Text(canvas.transform,"RTS accepted order","",900,90,360,42,16);orderText.alignment=TextAlignmentOptions.TopRight;orderText.gameObject.SetActive(false);}
        }
        private void CreateTacticalPanel()
        {
            var rts=Flow.Director.Rts;
            tacticalPanel=DemoWidgets.Rect(panel,"Tactical commands",0,82,584,73);
            ui.Surface(tacticalPanel,new Color(.035f,.043f,.046f,.98f));
            var buttons=new List<Button>();
            Button Add(string label,int column,int row,UnityEngine.Events.UnityAction action)
            {
                var button=ui.Button(tacticalPanel,label,label,12+column*94,4+row*33,90,30,action);
                var caption=button.GetComponentInChildren<TMP_Text>();
                caption.fontSize=14;
                caption.rectTransform.anchorMin=Vector2.zero;
                caption.rectTransform.anchorMax=Vector2.one;
                caption.rectTransform.offsetMin=new Vector2(5,3);
                caption.rectTransform.offsetMax=new Vector2(-5,-3);
                buttons.Add(button);
                return button;
            }
            formationLabel=Add("Línea",0,0,rts.CycleFormation).GetComponentInChildren<TMP_Text>();
            stanceLabel=Add("Defensiva",1,0,rts.CycleStance).GetComponentInChildren<TMP_Text>();
            Add("Detener",2,0,()=>rts.StopTactical(false));
            Add("Mantener",3,0,()=>rts.StopTactical(true));
            Add("Reagrupar",4,0,()=>rts.RegroupTactical());
            defendLabel=Add("Defender",5,0,()=>input.ArmDefend()).GetComponentInChildren<TMP_Text>();
            Add("Centrar · F",0,1,()=>rts.FocusSelection(Flow.CameraRig));
            engage=Add("Iniciar",1,1,()=>rts.StartTacticalBattle());
            retreat=Add("Retirada",1,1,()=>rts.RetreatTactical());
            tacticalButtons=buttons.ToArray();
            ui.Text(tacticalPanel,"Tactical hints","Alt + clic derecho: avanzar atacando   ·   Ctrl + 1–3: grupo",202,42,370,20,13);
            destinationMarker=ui.Text(canvas.transform,"Order location","+",0,0,28,28,24);
            destinationMarker.rectTransform.anchorMin=destinationMarker.rectTransform.anchorMax=new Vector2(.5f,.5f);
            destinationMarker.rectTransform.pivot=new Vector2(.5f,.5f);
            destinationMarker.alignment=TextAlignmentOptions.Center;
            destinationMarker.color=ui.Gold;
            destinationMarker.gameObject.SetActive(false);
            tacticalPanel.gameObject.SetActive(false);
        }
        private void UpdateTactical(RtsBattleController rts)
        {
            tacticalPanel.gameObject.SetActive(rts.TacticalBattle);
            panel.sizeDelta=new Vector2(584,rts.TacticalBattle?158:84);
            destinationMarker.gameObject.SetActive(false);
            if(!rts.TacticalBattle)return;
            formationLabel.text=rts.Formation==RtsFormation.Line?"Línea":rts.Formation==RtsFormation.Column?"Columna":"Compacta";
            stanceLabel.text=rts.SelectedStance switch
            {
                RtsStance.Passive=>"Pasiva", RtsStance.Defensive=>"Defensiva", RtsStance.Aggressive=>"Agresiva", _=>"Postura"
            };
            defendLabel.text=input.DefendArmed?"Marca suelo":"Defender";
            var enabled=rts.Active&&!Flow.Session.Paused&&!Flow.Session.WorldUiOpen;
            foreach(var button in tacticalButtons)button.interactable=enabled&&rts.SelectedAllies>0;
            engage.gameObject.SetActive(rts.TacticalDeployment);
            engage.interactable=enabled&&rts.TacticalReady;
            retreat.gameObject.SetActive(!rts.TacticalDeployment);
            retreat.interactable=enabled;
            text.text=(rts.TacticalDeployment?"DESPLIEGUE SEGURO":"PASO FRONTERIZO")+"   ·   "+rts.SelectedAllies+" seleccionados   ·   "+rts.Allies+" aliados / "+rts.Enemies+" rivales";
            if(Time.unscaledTime-rts.LastOrderAt>2)return;
            var projected=Camera.main.WorldToScreenPoint(rts.LastOrderPosition+Vector3.up*.15f);
            if(projected.z<=0)return;
            var canvasRect=(RectTransform)canvas.transform;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,projected,canvas.worldCamera,out var local)&&canvasRect.rect.Contains(local))
            {
                destinationMarker.rectTransform.anchoredPosition=local;
                destinationMarker.gameObject.SetActive(true);
            }
        }
        private void Order(string action){if(!Flow.Director.Rts.HeroAction(action))Flow.Session.Say("Selecciona al héroe y espera su intervalo; comprueba alcance y recursos.");}
        private void LateUpdate()
        {
            if(panel==null)return;var rts=Flow.Director.Rts;var show=Flow.IsPlaying&&rts.IncludesHero&&Flow.Session.Mode==PrototypeMode.Rts&&!Flow.ExistingHud.CompactView.HelpOpen;panel.gameObject.SetActive(show);
            foreach(var b in bars.Values)b.Root.gameObject.SetActive(false);
            if(!show){ClearBars();drag.gameObject.SetActive(false);if(orderText!=null)orderText.gameObject.SetActive(false);if(destinationMarker!=null)destinationMarker.gameObject.SetActive(false);return;}
            if(orderText!=null){orderText.gameObject.SetActive(!rts.TacticalBattle&&Time.unscaledTime-rts.LastOrderAt<2.5f);orderText.text="Orden: "+rts.LastOrderText;}
            rts.CopyStatus(status);var selectedHero=false;occupiedRects.Clear();var occupied=Flow.Front.Presentation==null?null:occupiedRects;
            foreach(var unit in status)
            {
                if(unit.Unit==null||unit.Health<=0)continue;
                if(unit.Hero&&unit.Unit.IsSelected)selectedHero=true;
                if(!bars.TryGetValue(unit.Unit,out var bar))
                {
                    var compact=Flow.Session.Rules.WorldExploration;var barWidth=unit.Hero?80:compact?34:44;var rect=DemoWidgets.Rect(canvas.transform,"RTS health",0,0,barWidth,compact?22:32);rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.pivot=new Vector2(.5f,0);if(Flow.Front.Presentation!=null)rect.SetAsFirstSibling();
                    DemoWidgets.Fill(DemoWidgets.Rect(rect,"Track",0,compact?16:23,barWidth,compact?5:6),new Color(.06f,.06f,.05f),false);
                    var fill=DemoWidgets.Fill(DemoWidgets.Rect(rect,"Health",0,compact?16:23,barWidth,compact?5:6),unit.Team==0?new Color(.43f,.65f,.32f):new Color(.70f,.27f,.20f),false);fill.sprite=Flow.Front.Presentation==null?Flow.Front.Theme.ButtonSprite:null;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;
                    bar=new Bar{Root=rect,Fill=fill,Label=ui.Text(rect,"Unit name","",0,0,150,compact?16:22,compact?13:15)};bars.Add(unit.Unit,bar);
                }
                var projected=Camera.main.WorldToScreenPoint(unit.Unit.transform.position+Vector3.up*2.05f);if(projected.z<=0)continue;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,projected,canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera,out var local);if(Flow.Front.Presentation!=null){var width=unit.Hero?140:48;var candidate=new Rect(local.x-width*.5f,local.y,width,30);if(!Flow.Session.Rules.WorldExploration){for(var n=0;n<12&&occupied.Exists(r=>r.Overlaps(candidate));n++){local.y+=12;candidate.y=local.y;}occupied.Add(candidate);}bar.Root.sizeDelta=new Vector2(Flow.Session.Rules.WorldExploration?(unit.Hero?80:34):width,Flow.Session.Rules.WorldExploration?22:32);bar.Label.rectTransform.sizeDelta=new Vector2(140,Flow.Session.Rules.WorldExploration?16:22);}
                bar.Root.anchoredPosition=local;bar.Root.gameObject.SetActive(true);
                bar.Fill.fillAmount=(float)unit.Health/unit.Maximum;var detailed=unit.Hero||(rts.SelectedAllies==1&&unit.Unit.IsSelected);bar.Label.text=(unit.Hero?"HÉROE · ":unit.Team==0?"A ":"B ")+(detailed?unit.Role+" ":"")+unit.Health;
                if(Flow.Front.Presentation!=null){bar.Label.fontSize=14;bar.Label.enabled=detailed;bar.Label.text=unit.Hero?"HÉROE "+unit.Health+"/"+unit.Maximum:unit.Role+" "+unit.Health;}
                bar.Label.color=unit.Unit.IsSelected?ui.Gold:ui.Ink;
            }
            text.text=(rts.ContextId=="conquest-outpost"?"CONQUISTA":"PATRULLA")+" · "+rts.Allies+" aliados / "+rts.Enemies+" rivales · "+rts.SelectedAllies+" seleccionados\nCtrl+1/2/3: guardar grupo · 1/2/3: seleccionar";
            if(Flow.UxEnabled)text.text=(rts.Preparing?"PREPARACIÓN SEGURA":"BATALLA")+" · "+rts.SelectedAllies+" seleccionados · "+rts.Allies+" aliados";
            if(tacticalPanel!=null)UpdateTactical(rts);
            attack.interactable=guard.interactable=recover.interactable=!rts.Preparing&&!rts.TacticalDeployment&&rts.Active&&selectedHero&&!Flow.Session.Paused;
            if(technique!=null)
            {
                var skill=Flow.Session.HeroProfile.Combat;technique.interactable=!rts.Preparing&&!rts.TacticalDeployment&&rts.Active&&selectedHero&&!Flow.Session.Paused&&skill!=null&&skill.TechniqueUnlocked&&rts.TechniqueFocus>=skill.TechniqueCost;
                var title=Flow.UxEnabled?(Flow.Session.State.ClassId=="warrior"?"Golpe":Flow.Session.State.ClassId=="mage"?"Proyectil":"Ki"):skill?.TechniqueTitle;
                technique.GetComponentInChildren<TMP_Text>().text=skill==null||!skill.TechniqueUnlocked?"Técnica · Nv2":title+" · "+rts.TechniqueFocus;
            }
            drag.gameObject.SetActive(input!=null&&input.Dragging);
            if(input!=null&&input.Dragging)
            {
                var rect=canvas.transform as RectTransform;RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,input.Selection.min,canvas.worldCamera,out var min);RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,input.Selection.max,canvas.worldCamera,out var max);
                drag.anchorMin=drag.anchorMax=new Vector2(.5f,.5f);drag.pivot=Vector2.zero;drag.anchoredPosition=min;drag.sizeDelta=max-min;
            }
        }
    }
}
