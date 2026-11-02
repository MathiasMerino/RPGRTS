using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Aethernia
{
    public sealed partial class DemoFrontEnd
    {
        public VisualPresentationProfile Presentation;
        private readonly Dictionary<string,ClassCardView> classCards=new Dictionary<string,ClassCardView>();
        private void PolishTitle(Transform p,string name,string title,float x,float y,float width,float height,float size)
        {var text=ui.Text(p,name,title,x,y,width,height,size);text.color=Presentation.Gold;text.fontStyle=FontStyles.Bold;text.characterSpacing=1;}
        private void BuildPolishedMain()
        {
            var main=Panel(DemoScreen.Main,"Main menu");
            ui.Surface(DemoWidgets.Rect(main,"Menu leather",48,44,654,652),new Color(.06f,.07f,.07f,.97f));
            PolishTitle(main,"Title","CRÓNICAS\nDE AETHERNIA",80,78,590,134,48);
            ui.Text(main,"Subtitle",Flow.Content.Subtitle,82,226,560,56,21);
            Add(main,"new","Comenzar una aventura",82,306,556,Flow.CreateCharacter);ui.Primary(Controls["new"]);
            Add(main,"continue","Continuar",82,364,556,()=>Flow.Continue());
            Add(main,"load","Cargar partida",82,422,556,()=>Flow.Continue());
            Add(main,"options","Configuración",82,480,270,ShowOptions);Add(main,"credits","Créditos",368,480,270,()=>Show(DemoScreen.Credits));
            Add(main,"quit","Salir del juego",82,538,556,Quit);
            ui.Text(main,"Demo label","EXPLORA · REÚNE TU COMPAÑÍA · ELIGE TU DESTINO",82,606,556,36,16).color=Presentation.Gold;
            var art=Presentation.Find("monk");var feature=Instantiate(Presentation.CardPrefab,main,false);var r=(RectTransform)feature.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(776,-86);r.sizeDelta=new Vector2(368,462);
            var card=feature.GetComponent<ClassCardView>();card.Bind(art,()=>{});card.Select(false);card.Button.enabled=false;card.GetComponent<Image>().raycastTarget=false;card.Selection.text="ILUSTRACIÓN PROVISIONAL";
            ui.Text(main,"World invitation","Una frontera de aldeas, ruinas y caminos por descubrir.",764,568,440,64,22).color=Presentation.Text;
        }
        private void BuildPolishedCharacter()
        {
            var p=Panel(DemoScreen.Character,"Character creation");
            PolishTitle(p,"Heading","ELIGE TU CAMINO",56,26,1168,46,32);
            ui.Text(p,"Selection introduction","Tres disciplinas, una misma aventura. Elige la clase que guiará tus primeros pasos.",56,78,1168,30,18);
            var i=0;foreach(var profile in Flow.Session.Rules.Classes)
            {
                var id=profile.Id;var node=Instantiate(Presentation.CardPrefab,p,false);node.name="Class "+id;var rect=(RectTransform)node.transform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(56+i++*400,-120);rect.sizeDelta=new Vector2(368,350);
                var card=node.GetComponent<ClassCardView>();card.Bind(Presentation.Find(id),()=>ChooseClass(id));classCards.Add(id,card);Controls.Add("class-"+id,card.Button);
            }
            ui.Text(p,"Name label","NOMBRE DEL VIAJERO",56,480,500,24,16).color=Presentation.Gold;NameInput=ui.Input(p,"Name","Viajero",56,510,500);
            classText=ui.Text(p,"Class description","",592,485,632,74,18);
            Add(p,"lineage","Cambiar linaje",56,568,220,()=>{lineageIndex=(lineageIndex+1)%Flow.Content.Lineages.Length;RefreshCharacter();});
            lineageText=ui.Text(p,"Lineage description","",298,568,926,54,16);
            ui.Text(p,"Racial visual notice",Flow.Content.VisualNotice,56,626,1168,24,14);
            Add(p,"character-back","Volver",56,664,220,Flow.ShowMain);Add(p,"begin","Comenzar aventura",892,664,332,()=>Flow.StartNew(NameInput.text,ChosenClass,ChosenLineage));ui.Primary(Controls["begin"]);RefreshPolishedCharacter();
        }
        private void RefreshPolishedCharacter()
        {
            var profile=Flow.Session.Rules.Find(ChosenClass);foreach(var pair in classCards)pair.Value.Select(pair.Key==ChosenClass);
            classText.text=profile.Title+" · "+profile.Health+" PV · Alcance "+profile.Range.ToString("0.#")+" m\n"+Flow.Content.ClassDescription(ChosenClass);
            var lineage=Flow.Content.Lineages[lineageIndex];lineageText.text=lineage.Title+" · "+lineage.Description;
        }
    }
}
