using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    public sealed class DemoRtsOverlay : MonoBehaviour
    {
        public DemoFlow Flow;
        private sealed class Bar {public RectTransform Root;public Image Fill;public TMP_Text Label;}
        private readonly List<RtsUnitStatus> status=new List<RtsUnitStatus>(24);
        private readonly Dictionary<ExplorationUnit,Bar> bars=new Dictionary<ExplorationUnit,Bar>();
        private RectTransform panel,drag;
        private TMP_Text text;
        private Button attack,guard,recover;
        private DemoWidgets ui;
        private Canvas canvas;
        public int LiveBars=>bars.Count;
        private void ClearBars(){foreach(var bar in bars.Values)if(bar.Root!=null)Destroy(bar.Root.gameObject);bars.Clear();}
        private void OnDisable(){ClearBars();}
        private void Start()
        {
            ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);canvas=Flow.ExistingHud.GetComponent<Canvas>();
            panel=DemoWidgets.Rect(canvas.transform,"Hero RTS commands",456,98,570,124);ui.Surface(panel,new Color(.035f,.043f,.046f,.96f));text=ui.Text(panel,"Group context","",12,6,546,56,18);
            attack=ui.Button(panel,"Hero attack","Atacar",12,72,170,42,()=>Order("attack"));guard=ui.Button(panel,"Hero guard","Guardia",196,72,170,42,()=>Order("guard"));recover=ui.Button(panel,"Hero recover","Recuperar",380,72,170,42,()=>Order("recover"));
            drag=DemoWidgets.Rect(canvas.transform,"Selection rectangle",0,0,0,0);DemoWidgets.Fill(drag,new Color(.70f,.63f,.36f,.18f),false);
        }
        private void Order(string action){if(!Flow.Director.Rts.HeroAction(action))Flow.Session.Say("Selecciona al h茅roe y espera su intervalo; comprueba alcance y recursos.");}
        private void LateUpdate()
        {
            if(panel==null)return;var rts=Flow.Director.Rts;var show=Flow.IsPlaying&&rts.IncludesHero&&Flow.Session.Mode==PrototypeMode.Rts;panel.gameObject.SetActive(show);
            foreach(var b in bars.Values)b.Root.gameObject.SetActive(false);
            if(!show){ClearBars();drag.gameObject.SetActive(false);return;}
            rts.CopyStatus(status);var selectedHero=false;var occupied=new List<Rect>();
            foreach(var unit in status)
            {
                if(unit.Unit==null||unit.Health<=0)continue;
                if(unit.Hero&&unit.Unit.IsSelected)selectedHero=true;
                if(!bars.TryGetValue(unit.Unit,out var bar))
                {
                    var barWidth=unit.Hero?80:44;var rect=DemoWidgets.Rect(canvas.transform,"RTS health",0,0,barWidth,32);rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.pivot=new Vector2(.5f,0);
                    DemoWidgets.Fill(DemoWidgets.Rect(rect,"Track",0,23,barWidth,6),new Color(.06f,.06f,.05f),false);
                    var fill=DemoWidgets.Fill(DemoWidgets.Rect(rect,"Health",0,23,barWidth,6),unit.Team==0?new Color(.43f,.65f,.32f):new Color(.70f,.27f,.20f),false);fill.sprite=Flow.Front.Theme.ButtonSprite;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;
                    bar=new Bar{Root=rect,Fill=fill,Label=ui.Text(rect,"Unit name","",0,0,150,22,15)};bars.Add(unit.Unit,bar);
                }
                var projected=Camera.main.WorldToScreenPoint(unit.Unit.transform.position+Vector3.up*2.05f);if(projected.z<=0)continue;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,projected,canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera,out var local);if(Flow.Front.Presentation!=null){var width=unit.Hero?140:48;var candidate=new Rect(local.x-width*.5f,local.y,width,30);for(var n=0;n<12&&occupied.Exists(r=>r.Overlaps(candidate));n++){local.y+=12;candidate.y=local.y;}occupied.Add(candidate);bar.Root.sizeDelta=new Vector2(width,32);bar.Label.rectTransform.sizeDelta=new Vector2(width,22);}
                bar.Root.anchoredPosition=local;bar.Root.gameObject.SetActive(true);
                bar.Fill.fillAmount=(float)unit.Health/unit.Maximum;var detailed=unit.Hero||(rts.SelectedAllies==1&&unit.Unit.IsSelected);bar.Label.text=(unit.Hero?"H脡ROE 路 ":unit.Team==0?"A ":"B ")+(detailed?unit.Role+" ":"")+unit.Health;
                if(Flow.Front.Presentation!=null){bar.Label.rectTransform.sizeDelta=new Vector2(140,22);bar.Label.fontSize=14;bar.Label.enabled=detailed;bar.Label.text=unit.Hero?"H蒖OE "+unit.Health+"/"+unit.Maximum:unit.Role+" "+unit.Health;}
                bar.Label.color=unit.Unit.IsSelected?ui.Gold:ui.Ink;
            }
            text.text=(rts.ContextId=="conquest-outpost"?"CONQUISTA":"PATRULLA")+" 路 "+rts.Allies+" aliados / "+rts.Enemies+" rivales 路 "+rts.SelectedAllies+" seleccionados\nCtrl+1/2/3: guardar grupo 路 1/2/3: seleccionar";
            attack.interactable=guard.interactable=recover.interactable=rts.Active&&selectedHero&&!Flow.Session.Paused;
            var input=Flow.GetComponent<DemoRtsInput>();drag.gameObject.SetActive(input!=null&&input.Dragging);
            if(input!=null&&input.Dragging)
            {
                var rect=canvas.transform as RectTransform;RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,input.Selection.min,canvas.worldCamera,out var min);RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,input.Selection.max,canvas.worldCamera,out var max);
                drag.anchorMin=drag.anchorMax=new Vector2(.5f,.5f);drag.pivot=Vector2.zero;drag.anchoredPosition=min;drag.sizeDelta=max-min;
            }
        }
    }
}
