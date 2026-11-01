using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    public enum DemoScreen { None, Main, Character, Intro, Options, Credits, Retry }
    public sealed class DemoFrontEnd : MonoBehaviour
    {
        public DemoFlow Flow;
        public TMP_FontAsset Font;
        public CharacterVisualCatalog Catalog;
        public MedievalUiTheme Theme;
        public CombatAudioPlayer Audio;
        public DemoScreen Screen { get; private set; }
        public readonly Dictionary<string,Button> Controls=new Dictionary<string,Button>();
        public DemoPreferences Preferences { get; private set; }
        public string ChosenClass { get; private set; }="warrior";
        public string ChosenLineage=>Flow.Content.Lineages[lineageIndex].Id;
        public TMP_InputField NameInput { get; private set; }
        private readonly Dictionary<DemoScreen,GameObject> panels=new Dictionary<DemoScreen,GameObject>();
        private Canvas canvas;
        private CanvasGroup fade;
        private Coroutine transition;
        private DemoWidgets ui;
        private TMP_Text notice,classText,lineageText,introText,optionsDisplay;
        private Image portrait;
        private int lineageIndex,introPage,displayIndex;
        private readonly Vector2Int[] resolutions={new Vector2Int(1280,720),new Vector2Int(1920,1080),new Vector2Int(2560,1440),new Vector2Int(3840,2160)};
        private void Awake()
        {
            ui=new DemoWidgets(Font,Theme.ButtonSprite);Preferences=DemoPreferences.Load();Preferences.Apply(Audio,Flow.CameraRig,false);
            Build();
        }
        private RectTransform Panel(DemoScreen id,string name)
        {
            var r=DemoWidgets.Stretch(transform,name);panels[id]=r.gameObject;
            DemoWidgets.Fill(DemoWidgets.Stretch(r,"Atmosphere"),new Color(.025f,.035f,.04f,.74f));
            return r;
        }
        private Button Add(Transform p,string id,string text,float x,float y,float w,System.Action action)
        {
            var b=ui.Button(p,id,text,x,y,w,46,()=>{Audio?.Play(FeedbackSound.Select,Vector3.zero,true);action();});Controls.Add(id,b);return b;
        }
        private void Build()
        {
            canvas=gameObject.GetComponent<Canvas>();if(canvas==null)canvas=gameObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=30;
            var scaler=gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=.5f;gameObject.AddComponent<GraphicRaycaster>();fade=gameObject.AddComponent<CanvasGroup>();
            var main=Panel(DemoScreen.Main,"Main menu");
            var slab=DemoWidgets.Rect(main,"Bronze frame",40,42,516,633);DemoWidgets.Fill(slab,new Color(.57f,.43f,.24f,.8f));
            DemoWidgets.Fill(DemoWidgets.Rect(slab,"Stone inset",2,2,512,629),new Color(.055f,.064f,.067f,.97f));
            var title=ui.Text(main,"Title","CRÓNICAS\nDE AETHERNIA",72,75,454,130,43);title.color=ui.Gold;title.fontStyle=FontStyles.Bold;title.characterSpacing=2;
            ui.Text(main,"Subtitle",Flow.Content.Subtitle,74,213,450,56,20);
            Add(main,"new","Nueva partida",72,290,452,Flow.CreateCharacter);
            Add(main,"continue","Continuar",72,347,452,()=>Flow.Continue());
            Add(main,"load","Cargar partida",72,404,452,()=>Flow.Continue());
            Add(main,"options","Opciones",72,461,218,ShowOptions);Add(main,"credits","Créditos",306,461,218,()=>Show(DemoScreen.Credits));
            Add(main,"quit","Salir",72,518,452,Quit);
            ui.Text(main,"Demo label","PRIMERA DEMO · RPG / RTS / ASENTAMIENTO",74,591,442,44,17).color=ui.Gold;
            notice=ui.Text(transform,"Status","",590,580,630,94,20);
            BuildCharacter();BuildIntro();BuildOptions();
            var credits=Panel(DemoScreen.Credits,"Credits");ui.Text(credits,"Credits text",Flow.Content.Credits,145,65,990,510,23);Add(credits,"credits-back","Volver",145,600,250,ShowMain);
            var retry=Panel(DemoScreen.Retry,"Defeat");ui.Text(retry,"Defeat title","La aventura continúa",210,175,860,65,36).color=ui.Gold;
            ui.Text(retry,"Defeat explanation","Has perdido este encuentro. Puedes iniciar un nuevo intento con la salud de tu clase restaurada; los dados y las reglas no cambian.",210,265,840,126,23);
            Add(retry,"retry","Reintentar encuentro",210,433,430,()=>{Flow.Resume();Flow.RetryCurrentEncounter();});
            Show(DemoScreen.Main);
        }
        private void BuildCharacter()
        {
            var p=Panel(DemoScreen.Character,"Character creation");ui.Text(p,"Heading","ELIGE TU CAMINO",60,40,850,64,36).color=ui.Gold;
            ui.Text(p,"Name label","Nombre",60,123,300,30);NameInput=ui.Input(p,"Name","Viajero",60,160,540);
            var x=60f;foreach(var profile in Flow.Session.Rules.Classes){var id=profile.Id;Add(p,"class-"+id,profile.Title,x,230,170,()=>ChooseClass(id));x+=184;}
            portrait=DemoWidgets.Rect(p,"Original 2D portrait",830,125,250,330).gameObject.AddComponent<Image>();portrait.preserveAspect=true;portrait.raycastTarget=false;
            ui.Text(p,"Portrait caption","Retrato 2D original · modelo Generic existente",767,470,430,50,17);
            classText=ui.Text(p,"Class description","",60,301,610,120,23);
            Add(p,"lineage","Cambiar linaje",60,438,235,()=>{lineageIndex=(lineageIndex+1)%Flow.Content.Lineages.Length;RefreshCharacter();});
            lineageText=ui.Text(p,"Lineage description","",315,438,390,95,19);
            ui.Text(p,"Racial visual notice",Flow.Content.VisualNotice,60,546,1120,55,17);
            Add(p,"character-back","Volver",60,630,230,Flow.ShowMain);
            Add(p,"begin","Comenzar aventura",830,630,350,()=>Flow.StartNew(NameInput.text,ChosenClass,ChosenLineage));RefreshCharacter();
        }
        public void ChooseClass(string id){if(Flow.Session.Rules.Find(id)==null)return;ChosenClass=id;RefreshCharacter();}
        private void RefreshCharacter()
        {
            if(classText==null)return;var profile=Flow.Session.Rules.Find(ChosenClass);classText.text=profile.Title+"\n"+Flow.Content.ClassDescription(ChosenClass)+"\n"+profile.Health+" PV · Alcance "+profile.Range.ToString("0.#")+" m";
            var entry=Catalog.FindEntry(ChosenClass);portrait.sprite=entry==null?null:entry.PortraitSprite;portrait.enabled=portrait.sprite!=null;
            var lineage=Flow.Content.Lineages[lineageIndex];lineageText.text=lineage.Title+" · provisional\n"+lineage.Description;
        }
        private void BuildIntro()
        {
            var p=Panel(DemoScreen.Intro,"Prologue");introText=ui.Text(p,"Narrative","",115,125,1050,350,29);
            ui.Text(p,"Provisional narrative",Flow.Content.NarrativeNotice,115,535,1050,48,18).color=ui.Gold;
            Add(p,"skip","Omitir prólogo",115,617,300,Flow.CompleteIntro);
            Add(p,"next","Continuar",865,617,300,()=>{if(++introPage>=Flow.Content.Intro.Length)Flow.CompleteIntro();else ShowIntro(introPage);});
        }
        private void BuildOptions()
        {
            var p=Panel(DemoScreen.Options,"Options");ui.Text(p,"Heading","OPCIONES",190,48,900,60,36).color=ui.Gold;
            ui.Text(p,"Master label","Volumen general",190,151,330,44,22);ui.Slider(p,"Master",Preferences.Master,550,151,530,v=>{Preferences.Master=v;ApplyPreferences(false);});
            ui.Text(p,"Effects label","Efectos e interfaz",190,220,330,44,22);ui.Slider(p,"Effects",Preferences.Effects,550,220,530,v=>{Preferences.Effects=v;ApplyPreferences(false);});
            ui.Text(p,"Camera label","Velocidad de cámara",190,289,330,44,22);ui.Slider(p,"Camera",(Preferences.CameraSpeed-4)/20,550,289,530,v=>{Preferences.CameraSpeed=4+20*v;ApplyPreferences(false);});
            optionsDisplay=ui.Text(p,"Display settings","",190,370,900,90,22);
            Add(p,"resolution","Cambiar resolución",190,483,270,()=>{displayIndex=(displayIndex+1)%resolutions.Length;Preferences.Width=resolutions[displayIndex].x;Preferences.Height=resolutions[displayIndex].y;RefreshOptions();});
            Add(p,"fullscreen","Pantalla / ventana",480,483,280,()=>{Preferences.Fullscreen=!Preferences.Fullscreen;RefreshOptions();});
            Add(p,"quality","Calidad gráfica",780,483,300,()=>{Preferences.Quality=(Preferences.Quality+1)%Mathf.Max(1,QualitySettings.names.Length);RefreshOptions();});
            ui.Text(p,"Audio scope","Sin pista musical: no se ofrece un control de música sin uso. P: pausar durante el juego.",190,558,900,52,17);
            Add(p,"apply","Aplicar y volver",190,635,890,()=>{ApplyPreferences(true);if(Flow.IsPlaying)Flow.Resume();else ShowMain();});RefreshOptions();
        }
        public void ApplyPreferences(bool display){Preferences.Apply(Audio,Flow.CameraRig,display);Preferences.Save();}
        private void RefreshOptions(){optionsDisplay.text=Preferences.Width+" × "+Preferences.Height+" · "+(Preferences.Fullscreen?"Pantalla completa":"Ventana")+"\nCalidad: "+QualitySettings.names[Preferences.Quality]+(Application.isEditor?" · Resolución aplicada en el ejecutable":"");}
        public void ShowMain(){Show(DemoScreen.Main);var valid=Flow.HasSave();Controls["continue"].interactable=valid;Controls["load"].interactable=valid;Notify(valid?"Partida de demo disponible. Nueva partida creará un respaldo antes de reemplazarla.":"No hay una partida de demo válida. Puedes comenzar una aventura.");}
        public void ShowCreation(){Show(DemoScreen.Character);RefreshCharacter();}
        public void ShowIntro(int introPageIndex){introPage=introPageIndex;Show(DemoScreen.Intro);introText.text=Flow.Content.Intro[Mathf.Clamp(introPage,0,Flow.Content.Intro.Length-1)];}
        public void ShowOptions(){Show(DemoScreen.Options);RefreshOptions();}
        public void ShowRetry()=>Show(DemoScreen.Retry);
        public void Notify(string value){notice.text=value;notice.transform.SetAsLastSibling();}
        public void Hide(){Screen=DemoScreen.None;canvas.enabled=false;fade.blocksRaycasts=false;notice.text="";}
        private void Show(DemoScreen next)
        {
            Screen=next;canvas.enabled=true;fade.blocksRaycasts=true;notice.text="";
            foreach(var p in panels)p.Value.SetActive(p.Key==next);
            notice.transform.SetAsLastSibling();if(transition!=null)StopCoroutine(transition);transition=StartCoroutine(Fade());
        }
        private IEnumerator Fade(){fade.alpha=0;while(fade.alpha<1){fade.alpha=Mathf.Min(1,fade.alpha+Time.unscaledDeltaTime*5);yield return null;}transition=null;}
        public void Quit(){if(Application.isEditor){Notify("Salir cierra el ejecutable Windows. En el editor se conserva la sesión.");return;}Application.Quit();}
    }
}
