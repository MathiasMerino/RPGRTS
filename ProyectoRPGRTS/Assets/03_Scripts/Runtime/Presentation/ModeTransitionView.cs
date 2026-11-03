using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Aethernia
{
    // Decoration on the existing HUD: never owns modes, input, camera or saved state.
    public sealed class ModeTransitionView : MonoBehaviour
    {
        public DemoFlow Flow;
        private CanvasGroup toast;
        private TMP_Text caption;
        private Image veil;
        private PrototypeMode lastMode;
        private bool lastDungeon,ready;
        private float fadeUntil,toastUntil;private string lastProgression="";
        public string Announcement=>caption==null?"":caption.text;
        public float VeilAlpha=>veil==null?0:veil.color.a;
        private void OnEnable()
        {if(Flow==null)return;Flow.Started+=Started;Flow.Session.Changed+=Changed;Flow.Director.Encounter.Resolved+=RpgResolved;Flow.Director.Rts.Resolved+=RtsResolved;}
        private void OnDisable()
        {
            if(Flow==null)return;Flow.Started-=Started;Flow.Session.Changed-=Changed;Flow.Director.Encounter.Resolved-=RpgResolved;Flow.Director.Rts.Resolved-=RtsResolved;
            if(toast!=null)toast.alpha=0;if(veil!=null)veil.color=Color.clear;
        }
        private void Start()
        {
            var ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);var root=Flow.ExistingHud.transform;
            var sheet=DemoWidgets.Stretch(root,"Mode transition veil");sheet.SetAsFirstSibling();veil=DemoWidgets.Fill(sheet,Color.clear,false);
            var panel=DemoWidgets.Rect(root,"Mode announcement",16,342,350,70);var surface=ui.Surface(panel,new Color(.04f,.04f,.035f,.96f));surface.raycastTarget=false;
            toast=panel.gameObject.AddComponent<CanvasGroup>();toast.blocksRaycasts=false;toast.interactable=false;toast.alpha=0;
            caption=ui.Text(panel,"Transition caption","",12,10,326,50,18);caption.color=ui.Gold;ready=true;Remember();
        }
        private bool InDungeon=>Flow.Director.Hero.transform.position.x>65;
        private void Remember(){lastMode=Flow.Session.Mode;lastDungeon=InDungeon;}
        private void Started(){if(!ready)return;Remember();Show(InDungeon?"LAS RUINAS DEL UMBRAL":"EL CAMINO CONTINÚA",true);}
        private void Changed()
        {
            if(!ready||!Flow.IsPlaying)return;
            if(lastMode==Flow.Session.Mode)return;lastMode=Flow.Session.Mode;
            switch(lastMode)
            {
                case PrototypeMode.TurnCombat:Show("COMBATE · observa el turno y el alcance",true);break;
                case PrototypeMode.Rts:Show("BATALLA · selecciona y da órdenes",true);break;
                case PrototypeMode.Construction:Show("CAMPAMENTO · construye sobre suelo libre",false);break;
                case PrototypeMode.Exploration:Show("EXPLORACIÓN · retomas el camino",true);break;
            }
        }
        private void RpgResolved(bool won){if(ready)Show(won?"VICTORIA · el camino queda abierto":"DERROTA · puedes volver a intentarlo",true);}
        private void RtsResolved(bool won){if(ready)Show(won?"VICTORIA · batalla terminada":"DERROTA · revisa tus órdenes",true);}
        private void Show(string text,bool fade){caption.text=text;toastUntil=Time.unscaledTime+2.4f;if(fade)fadeUntil=Time.unscaledTime+.18f;}
        private void LateUpdate()
        {
            if(!ready)return;
            if(Flow.IsPlaying&&lastDungeon!=InDungeon){lastDungeon=InDungeon;Show(lastDungeon?"LAS RUINAS DEL UMBRAL":"REGRESO AL CAMPAMENTO",true);}
            var visible=Flow.IsPlaying&&Flow.Front.Screen==DemoScreen.None;var modal=Flow.ExistingHud.CompactView.WindowOpen||Flow.ExistingHud.CompactView.HelpOpen;
            if(visible&&Flow.Session.LastProgressionNotice!=lastProgression){lastProgression=Flow.Session.LastProgressionNotice;if(!string.IsNullOrEmpty(lastProgression))Show(lastProgression,false);}
            toast.alpha=visible&&!modal&&!Flow.UxEnabled?Mathf.Clamp01((toastUntil-Time.unscaledTime)*3):0;
            veil.color=new Color(0,0,0,visible?Mathf.Clamp01((fadeUntil-Time.unscaledTime)/.18f)*.18f:0);
        }
    }
}
