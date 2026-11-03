using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    // Presentación contextual sobre las ventanas existentes. No posee estado de partida.
    [DefaultExecutionOrder(500)]
    public sealed class UxExperienceView : MonoBehaviour
    {
        public DemoFlow Flow;
        public LostCargoController Cargo;
        public PlayerCommands Commands;
        private TMP_Text objective,prompt,marker,notice;
        private string lastNotice;private float noticeUntil;
        private RectTransform noticeRect,pages,back,interaction;
        private RectTransform objectives,prompts,markerRect;
        private RectTransform header,dock;
        private MedievalHudView hud;
        private bool guidance=true;
        private float nextRefresh;
        public string ObjectiveText=>objective==null?"":objective.text;
        public string PromptText=>prompt==null?"":prompt.text;
        public Vector3 ObjectivePosition { get; private set; }
        private void Start()
        {
            hud=Flow.ExistingHud.CompactView;
            var canvas=Flow.ExistingHud.transform;
            var ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);
            objectives=DemoWidgets.Rect(canvas,"UX Current objective",16,88,360,76);
            ui.Surface(objectives,new Color(.03f,.035f,.03f,.8f)).raycastTarget=false;
            objective=ui.Text(objectives,"Objective","",12,8,336,60,17);
            prompts=DemoWidgets.Rect(canvas,"UX Nearby interaction",0,0,440,42);
            prompts.anchorMin=prompts.anchorMax=new Vector2(.5f,0);prompts.pivot=new Vector2(.5f,0);prompts.anchoredPosition=new Vector2(0,18);
            ui.Surface(prompts,new Color(.03f,.035f,.03f,.9f)).raycastTarget=false;
            prompt=ui.Text(prompts,"Prompt","",12,8,416,28,18);prompt.alignment=TextAlignmentOptions.Center;
            noticeRect=DemoWidgets.Rect(canvas,"UX Notice",0,0,740,34);noticeRect.anchorMin=noticeRect.anchorMax=noticeRect.pivot=new Vector2(.5f,0);noticeRect.anchoredPosition=new Vector2(0,72);
            notice=ui.Text(noticeRect,"Notice","",0,0,740,34,17);notice.alignment=TextAlignmentOptions.Center;
            markerRect=DemoWidgets.Rect(canvas,"UX Objective marker",0,0,100,44);markerRect.anchorMin=markerRect.anchorMax=new Vector2(.5f,.5f);markerRect.pivot=new Vector2(.5f,.5f);
            marker=ui.Text(markerRect,"Destination","◆",0,0,100,44,21);marker.alignment=TextAlignmentOptions.Center;marker.color=ui.Gold;
            var toggle=ui.Button(canvas,"UX Toggle guidance","Guía",0,0,76,32,()=>guidance=!guidance);
            var tr=(RectTransform)toggle.transform;tr.anchorMin=tr.anchorMax=new Vector2(1,1);tr.anchoredPosition=new Vector2(-272,-20);
            header=canvas.Find("Title Bar") as RectTransform;dock=canvas.Find("C02 Action Dock") as RectTransform;
            if(header!=null)
            {
                foreach(var text in header.GetComponentsInChildren<Text>(true))if(text!=hud.HeroName&&text!=hud.HealthLabel&&text!=hud.Resources&&text.GetComponentInParent<Button>()==null)text.gameObject.SetActive(false);
                var image=header.GetComponent<Image>();if(image!=null)image.enabled=false;
            }
            if(dock!=null)
            {
                pages=dock.Find("Exploration Pages") as RectTransform;back=dock.Find("Return to Exploration") as RectTransform;
                interaction=Flow.ExistingHud.GetComponent<ExplorationHud>().ActionButton.transform as RectTransform;
                var image=dock.GetComponent<Image>();if(image!=null)image.enabled=false;
                var caption=dock.Find("Navigation Caption");if(caption!=null)caption.gameObject.SetActive(false);
            }
            hud.Portrait.transform.parent.gameObject.SetActive(false);
            foreach(var label in Object.FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include))
                if(label.GetComponentInParent<Canvas>()==null)label.gameObject.SetActive(false);
            foreach(var point in Object.FindObjectsByType<InteractionPoint>(FindObjectsInactive.Include))foreach(var label in point.GetComponentsInChildren<Canvas>(true))if(label.renderMode==RenderMode.WorldSpace)label.gameObject.SetActive(false);
            PlaceMenu("help","Ayuda",-136,88);PlaceMenu("page-session","Menú",-16,108);
        }
        private void PlaceMenu(string id,string caption,float x,float width)
        {
            var button=Flow.ExistingHud.FindControl(id);if(button==null)return;
            button.transform.SetParent(Flow.ExistingHud.transform,false);Place((RectTransform)button.transform,new Vector2(1,1),new Vector2(x,-20),new Vector2(width,36));
            var source=button.GetComponentInChildren<Text>(true);if(source!=null){source.gameObject.SetActive(true);source.text=caption;}
            var label=button.GetComponentInChildren<TMP_Text>(true);if(label!=null){label.transform.SetParent(button.transform,false);label.gameObject.SetActive(true);label.rectTransform.anchorMin=Vector2.zero;label.rectTransform.anchorMax=Vector2.one;label.rectTransform.offsetMin=new Vector2(6,4);label.rectTransform.offsetMax=new Vector2(-6,-4);label.text=caption;}
        }
        private static void Place(RectTransform r,Vector2 anchor,Vector2 position,Vector2 size)
        {r.anchorMin=r.anchorMax=r.pivot=anchor;r.anchoredPosition=position;r.sizeDelta=size;}
        private void LateUpdate()
        {
            if(hud==null)return;
            var session=Flow.Session;var visible=Flow.IsPlaying&&!session.Paused&&!hud.WindowOpen&&!hud.HelpOpen&&!session.WorldUiOpen;
            var mode=session.Mode;var exploration=mode==PrototypeMode.Exploration;
            // Existing renderers refresh these rectangles; this is the single opt-in final layout.
            if(header!=null){Place(header,new Vector2(0,1),new Vector2(16,-12),new Vector2(1248,64));}
            Place(hud.HeroName.rectTransform,new Vector2(0,1),new Vector2(0,-4),new Vector2(350,26));
            var track=(RectTransform)hud.HealthFill.transform.parent;Place(track,new Vector2(0,1),new Vector2(0,-36),new Vector2(184,10));
            Place(hud.HealthLabel.rectTransform,new Vector2(0,1),new Vector2(194,-28),new Vector2(112,26));
            hud.Resources.transform.parent.gameObject.SetActive(mode==PrototypeMode.Construction||(exploration&&session.State.Demo!=null&&session.State.Demo.Chapter>=DemoChapter.Settlement));
            if(dock!=null)Place(dock,new Vector2(0,0),new Vector2(16,10),new Vector2(1248,58));
            if(pages!=null)Place(pages,new Vector2(0,0),Vector2.zero,new Vector2(276,40));
            if(back!=null){Place(back,new Vector2(0,0),Vector2.zero,new Vector2(180,38));back.gameObject.SetActive(mode==PrototypeMode.Construction);}
            if(interaction!=null)interaction.gameObject.SetActive(false);
            if(session.LastMessage!=lastNotice){lastNotice=session.LastMessage;notice.text=lastNotice;noticeUntil=Time.unscaledTime+4;}
            noticeRect.gameObject.SetActive(visible&&Time.unscaledTime<noticeUntil);
            hud.Context.gameObject.SetActive(mode==PrototypeMode.Construction);
            hud.Hint.gameObject.SetActive(false);
            if(!hud.WindowOpen)
            {Place(hud.ActionGrid,new Vector2(.5f,0),new Vector2(0,14),new Vector2(810,40));hud.ActionLayout.cellSize=new Vector2(126,38);}
            hud.Feedback.transform.parent.gameObject.SetActive(false); // Objective/help owns one bounded notice surface.
            prompts.gameObject.SetActive(visible&&exploration);
            objectives.gameObject.SetActive(visible&&guidance);
            markerRect.gameObject.SetActive(visible&&guidance&&(exploration||(mode==PrototypeMode.Rts&&Flow.Director.Rts.Preparing&&Flow.Director.Rts.PreparationStep<3)));
            if(Time.unscaledTime>=nextRefresh)
            {
                nextRefresh=Time.unscaledTime+.12f;
                var point=Commands.RefreshNearby();
                prompt.text=point==null?"WASD · caminar     Rueda · acercar":"E · "+Verb(point)+" — "+point.DisplayName;
                SetObjective();
            }
            if(markerRect.gameObject.activeSelf)
            {
                var camera=Flow.CameraRig.GetComponent<Camera>();var screen=camera.WorldToScreenPoint(ObjectivePosition+Vector3.up*2.1f);
                var canvas=(Canvas)Flow.ExistingHud.GetComponent<Canvas>();var cr=(RectTransform)canvas.transform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(cr,screen,canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera,out var local);
                local.x=Mathf.Clamp(local.x,-cr.rect.width*.5f+48,cr.rect.width*.5f-48);local.y=Mathf.Clamp(local.y,-cr.rect.height*.5f+100,cr.rect.height*.5f-190);
                markerRect.anchoredPosition=local;marker.text="+ "+Vector3.Distance(Flow.Director.Hero.transform.position,ObjectivePosition).ToString("0")+" m";
            }
        }
        private static string Verb(InteractionPoint point)=>point.FactId==LostCargoQuest.NpcId?"Hablar":point.FactId==LostCargoQuest.CargoId?"Recuperar":point.FactId=="demo-patrol"?"Preparar patrulla":point.FactId.StartsWith("portal-")?"Viajar":"Examinar";
        private void SetObjective()
        {
            var state=Flow.Session.State;var mode=Flow.Session.Mode;var quest=LostCargoQuest.Stage(state);
            ObjectivePosition=quest==CargoQuestStage.Accepted?Cargo.Cargo.transform.position:Cargo.Npc.transform.position;
            if(mode==PrototypeMode.TurnCombat)
            {
                var battle=Flow.Director.Encounter.Battle;
                objective.text=battle==null?"Selecciona tu objetivo.":battle.Phase==TurnPhase.Enemy?"El rival actúa. Observa el resultado.":!battle.ActionAvailable?"Acción realizada · Fin de turno.":!Flow.Director.Encounter.TargetSelected?"Selecciona al rival con clic izquierdo.":"Acércate si hace falta; Atacar o Guardia.\nUna acción por turno.";
            }
            else if(mode==PrototypeMode.Rts)
            {
                var war=Flow.Director.Rts;ObjectivePosition=war.PreparationTarget;
                objective.text=!war.Preparing?"Combate RTS · Selección y clic derecho.\nSelecciona al héroe para sus acciones.":war.PreparationStep==0?"Preparación segura · Selecciona UN aliado.":war.PreparationStep==1?"Shift o arrastre: selecciona varios.\nClic derecho al primer hito; espera llegada.":war.PreparationStep==2?"Reagrupa varias unidades en el segundo hito.\nLas órdenes requieren llegada real.":"Rivales al este · Coloca tus tropas.\nClic derecho sobre un rival inicia el ataque.";
                
            }
            else if(mode==PrototypeMode.Construction)objective.text="Construye en terreno libre. Clic derecho.\nRecursos y costes visibles; Explorar cancela.";
            else if(quest!=CargoQuestStage.Completed&&(state.Demo==null||state.Demo.Chapter<DemoChapter.Settlement))objective.text="EL CARGAMENTO PERDIDO\n"+LostCargoQuest.Objective(state);
            else if(state.Demo!=null&&state.Demo.Chapter<DemoChapter.Settlement)
            {ObjectivePosition=Flow.Patrol.transform.position;objective.text="LA PATRULLA\nHabla con el capitán y prepara las tropas.";}
            else
            {ObjectivePosition=new Vector3(9,0,-10);objective.text="CAMPAMENTO\nMenú → Construir: centro, taller y producción.\nDespués: compañeros, mazmorra y culturas.";if(state.Demo!=null&&state.Demo.Chapter==DemoChapter.FreeExploration)markerRect.gameObject.SetActive(false);}
        }
    }
}
