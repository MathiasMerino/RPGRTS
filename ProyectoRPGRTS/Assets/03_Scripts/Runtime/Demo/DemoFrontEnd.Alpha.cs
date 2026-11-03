using UnityEngine;
using UnityEngine.UI;
namespace Aethernia
{
    public sealed partial class DemoFrontEnd
    {
        public bool AlphaLayout;public GameObject AlphaClassOption;
        private Image alphaFigure;private RuntimeClassPreview alphaPreview;
        private RuntimeClassPreview Preview(Transform parent,string id,float x,float y,float w,float h)
        {
            var mount=DemoWidgets.Rect(parent,"Game model preview",x,y,w,h);ui.Surface(mount,new Color(.04f,.05f,.06f));
            var fallback=DemoWidgets.Rect(mount,"Fallback illustration",8,8,w-16,h-16).gameObject.AddComponent<Image>();fallback.sprite=Presentation.Find(id).Figure;fallback.preserveAspect=true;fallback.raycastTarget=false;
            var output=DemoWidgets.Rect(mount,"Live model",4,4,w-8,h-8).gameObject.AddComponent<RawImage>();output.raycastTarget=false;
            var preview=mount.gameObject.AddComponent<RuntimeClassPreview>();preview.Configure(Catalog,id,output,fallback);return preview;
        }
        private void BuildAlphaMain()
        {
            var main=Panel(DemoScreen.Main,"Main menu");ui.Surface(DemoWidgets.Rect(main,"Menu leather",48,44,654,652),new Color(.06f,.07f,.07f,.97f));
            PolishTitle(main,"Title","CRÓNICAS\nDE AETHERNIA",80,78,590,134,48);ui.Text(main,"Subtitle",Flow.Content.Subtitle,82,226,560,56,21);
            Add(main,"new","Comenzar una aventura",82,306,556,Flow.CreateCharacter);ui.Primary(Controls["new"]);Add(main,"continue","Continuar",82,364,556,()=>Flow.Continue());Add(main,"load","Cargar partida",82,422,556,()=>Flow.Continue());Add(main,"options","Configuración",82,480,270,ShowOptions);Add(main,"credits","Créditos",368,480,270,()=>Show(DemoScreen.Credits));Add(main,"quit","Salir del juego",82,538,556,Quit);
            ui.Text(main,"Development label",Flow.Session.Rules.Civilizations!=null?"E19 · CULTURAS PILOTO · ARTE PROVISIONAL":"E17 · VERSIÓN EN DESARROLLO · ARTE PROVISIONAL",82,606,556,36,16).color=Presentation.Gold;
            var portrait=DemoWidgets.Rect(main,"Monk portrait",766,64,70,82).gameObject.AddComponent<Image>();portrait.sprite=Presentation.Find("monk").Portrait;portrait.preserveAspect=true;portrait.raycastTarget=false;
            PolishTitle(main,"Featured class","MONJE",858,70,350,38,30);ui.Text(main,"Featured identity","Disciplina · temple · combate marcial",858,114,350,30,17);
            Preview(main,"monk",764,158,440,406);ui.Text(main,"Preview notice","Modelo del juego · representación provisional",764,578,440,30,16).color=Presentation.Gold;ui.Text(main,"World invitation","Una frontera de aldeas, ruinas y caminos por descubrir.",764,624,440,62,21);
        }
        private void BuildAlphaCharacter()
        {
            var p=Panel(DemoScreen.Character,"Character creation");PolishTitle(p,"Heading","ELIGE TU CAMINO",56,26,1168,46,32);ui.Text(p,"Introduction",Flow.Session.Rules.Civilizations!=null?"Cuatro linajes, tres disciplinas. Elige raza y clase.":"Tres disciplinas, una misma aventura. Elige tu clase.",56,80,1168,30,19);
            var i=0;foreach(var profile in Flow.Session.Rules.Classes)
            {
                var id=profile.Id;var node=Instantiate(AlphaClassOption,p,false);node.name="Class "+id;var r=(RectTransform)node.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(56,-(138+i++*130));r.sizeDelta=new Vector2(300,112);
                var card=node.GetComponent<ClassCardView>();card.Bind(Presentation.Find(id),()=>{Audio?.Play(FeedbackSound.Select,Vector3.zero,true);ChooseClass(id);});classCards.Add(id,card);Controls.Add("class-"+id,card.Button);
            }
            var buttons=new System.Collections.Generic.List<Button>();foreach(var item in classCards.Values)buttons.Add(item.Button);for(var n=0;n<buttons.Count;n++){var nav=buttons[n].navigation;nav.mode=Navigation.Mode.Explicit;nav.selectOnUp=buttons[(n+buttons.Count-1)%buttons.Count];nav.selectOnDown=buttons[(n+1)%buttons.Count];buttons[n].navigation=nav;}
            ui.Text(p,"Illustration label","ILUSTRACIÓN DE CLASE",390,116,300,22,14).color=Presentation.Gold;var paper=DemoWidgets.Rect(p,"Concept mount",390,142,300,368);ui.Surface(paper,Color.white,true);alphaFigure=DemoWidgets.Rect(paper,"Selected concept",8,8,284,352).gameObject.AddComponent<Image>();alphaFigure.preserveAspect=true;alphaFigure.raycastTarget=false;
            ui.Text(p,"Model label","VISTA PREVIA DEL PERSONAJE",740,116,484,22,14).color=Presentation.Gold;alphaPreview=Preview(p,ChosenClass,740,142,484,368);ui.Text(p,"Model notice","Modelo provisional · animación de reposo",740,514,484,20,14);
            ui.Text(p,"Name label","NOMBRE DEL VIAJERO",56,526,300,22,15).color=Presentation.Gold;NameInput=ui.Input(p,"Name","Viajero",56,552,300);classText=ui.Text(p,"Class description","",390,538,834,60,18);
            if(Flow.Session.Rules.Civilizations!=null)
            {var n=0;foreach(var race in Flow.Content.Lineages){var id=race.Id;Add(p,"race-"+id,race.Title,56+n++*296,602,280,()=>ChooseRace(id));}lineageText=ui.Text(p,"Lineage description","",56,644,1168,18,15);}
            else {Add(p,"lineage","Cambiar linaje",56,602,204,()=>{lineageIndex=(lineageIndex+1)%Flow.Content.Lineages.Length;RefreshCharacter();});lineageText=ui.Text(p,"Lineage description","",280,606,944,38,15);ui.Text(p,"Racial visual notice",Flow.Content.VisualNotice,56,646,1168,18,14);}
            Add(p,"character-back","Volver",56,666,220,Flow.ShowMain);Add(p,"begin","Comenzar aventura",892,666,332,()=>Flow.StartNew(NameInput.text,ChosenClass,ChosenLineage));ui.Primary(Controls["begin"]);RefreshAlphaCharacter();
        }
        private void RefreshAlphaCharacter(){RefreshPolishedCharacter();alphaFigure.sprite=Presentation.Find(ChosenClass).Figure;alphaPreview.ShowClass(ChosenClass);if(Flow.Session.Rules.Civilizations!=null){var race=Flow.Session.Rules.Civilizations.Race(ChosenLineage);RaceVisualVariant.Apply(alphaPreview.Model?.transform,Flow.Session.Rules.Civilizations,ChosenLineage);var profile=Flow.Session.Rules.Find(ChosenClass);classText.text=profile.Title+" · "+(profile.Health+race.MaximumHealth)+" PV · "+(profile.Speed+race.Speed).ToString("0.#")+"m/s · alcance "+profile.Range+"m\n"+Flow.Content.ClassDescription(ChosenClass);lineageText.text=race.Title+" · "+race.Trait+" · Todas las clases / cultura independiente";}}
    }
}
