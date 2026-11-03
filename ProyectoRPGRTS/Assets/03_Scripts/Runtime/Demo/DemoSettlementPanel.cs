using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    public sealed class DemoSettlementPanel : MonoBehaviour
    {
        public DemoFlow Flow;
        private RectTransform panel;
        private Button manage,activate,upgrade;
        private TMP_Text selectedLabel,resources;
        private readonly List<Button> recruits=new List<Button>();
        private int selected=-1,lastCount;
        private SettlementRules Rules=>Flow.Session.Settlement;
        private BuildingRecord Selected=>selected>=0&&selected<Flow.Session.State.Buildings.Count?Flow.Session.State.Buildings[selected]:null;
        public bool SelectBuilding(int index){if(index<0||index>=Flow.Session.State.Buildings.Count)return false;selected=index;return true;}
        private DemoWidgets ui;
        private void Start()
        {
            ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);var root=Flow.ExistingHud.transform;
            manage=Button(root,"Manage camp","Administrar campamento",1000,235,264,Open);
            panel=DemoWidgets.Rect(root,"Settlement management",456,98,570,496);ui.Surface(panel,new Color(.035f,.043f,.046f,.97f));
            ui.Text(panel,"Camp title","CAMPAMENTO · CONSTRUCCIÓN",12,10,546,32,23).color=ui.Gold;
            resources=ui.Text(panel,"Resources","",12,42,546,28,17);
            var row=0;foreach(var profile in Rules.Blueprints)
            {
                var id=profile.Id;Button(panel,"Build "+id,profile.Title+" · "+profile.Gold+" oro / "+profile.Wood+" madera",12,76+row++*44,546,()=>SelectBlueprint(id));
            }
            selectedLabel=ui.Text(panel,"Selected building","",12,304,546,32,17);
            Button(panel,"Next building","Siguiente",12,341,126,()=>{if(Flow.Session.State.Buildings.Count>0)selected=(selected+1)%Flow.Session.State.Buildings.Count;});
            activate=Button(panel,"Toggle production","Activar",150,341,132,()=>ToggleProduction());
            upgrade=Button(panel,"Upgrade building","Mejorar",294,341,126,()=>Improve());
            Button(panel,"Explore","Explorar",432,341,126,()=>Flow.Director.Execute("explore"));
            row=0;foreach(var role in Flow.Director.Rts.DemoRules.Roles)
            {var id=role.Id;recruits.Add(Button(panel,"Recruit "+id,role.Title+(Rules.Units!=null?" "+Rules.GoldCost(id)+"o/2a":""),12+row++*184,396,170,()=>Recruit(id)));}
            ui.Text(panel,"Recruitment rules",(Rules.Units==null?"Reclutar: 5 oro + 2 alimento":"Costes por rol indicados arriba; requiere cuartel")+"\nMejorar nivel 2: 10 oro + 5 madera · producción/almacén",12,447,546,45,16);
        }
        private Button Button(Transform parent,string name,string caption,float x,float y,float width,Action action)
        {var b=ui.Button(parent,name,caption,x,y,width,38,()=>{Flow.Front.Audio?.Play(FeedbackSound.Select,Vector3.zero,true);action();});return b;}
        public void Open()
        {
            if(Flow.Session.State.Demo==null||Flow.Session.State.Demo.Chapter<DemoChapter.Settlement)return;
            Flow.ExistingHud.ClosePanels();Flow.Director.Execute("construct");
            if(!SettlementOperations.Has(Flow.Session.State,"townhall"))Flow.Director.Construction.SelectBlueprint("townhall");
        }
        public bool SelectBlueprint(string id)
        {Open();var accepted=Flow.Director.Construction.SelectBlueprint(id);if(accepted)Flow.Session.Say("Seleccionado: "+Rules.Find(id).Title+". Clic derecho en suelo libre, huella 3 × 3 m.");return accepted;}
        public bool ToggleProduction()
        {var b=Selected;var success=SettlementOperations.SetProduction(Flow.Session.State,Rules,b,b!=null&&b.ProductionPaused);Flow.Session.Say(success?(b.ProductionPaused?"Producción pausada.":"Instalación activa: produce en el siguiente ciclo de 10 s."):"Selecciona una instalación productiva.");return success;}
        public bool Improve()
        {var result=SettlementOperations.Upgrade(Flow.Session.State,Rules,Selected);Flow.Session.Say(result?"Nivel 2: mayor rendimiento/capacidad, misma huella.":"No se puede mejorar: comprueba instalación, nivel y recursos.");return result;}
        public bool Recruit(string role)
        {var result=SettlementOperations.Recruit(Flow.Session.State,Rules,role);Flow.Session.Say(result?"Recluta añadido al ejército persistente. Visita al capitán para desplegarlo.":"Reclutamiento rechazado: requiere cuartel, recursos y espacio (6).");return result;}
        private bool WorkbenchReady()=>Flow.IsPlaying&&Flow.Session.Mode==PrototypeMode.Exploration&&!Flow.Session.Paused&&!Flow.Director.IsBusy&&Vector3.Distance(Flow.Director.Hero.transform.position,new Vector3(9,0,-10))<9;
        public bool Forge(){var ok=WorkbenchReady()&&EquipmentOperations.Forge(Flow.Session.State);Flow.Session.Say(ok?"Arma de clase forjada: equípala desde Inventario.":"Forja: requiere campamento, taller2,8oro y mineral1.");return ok;}
        public bool BuyVest(){var ok=WorkbenchReady()&&EquipmentOperations.BuyVest(Flow.Session.State);Flow.Session.Say(ok?"Chaleco adquirido; puedes equiparlo.":"Chaleco: campamento, taller,8oro/4madera y espacio.");return ok;}
        public bool SellItem(string id){var ok=WorkbenchReady()&&EquipmentOperations.Sell(Flow.Session.State,id,Flow.Session.Rules);Flow.Session.Say(ok?"Objeto vendido. Recursos registrados.":"Venta: campamento y pieza sin equipar.");return ok;}
        public bool SellWood(){var ok=WorkbenchReady()&&EquipmentOperations.SellWood(Flow.Session.State);Flow.Session.Say(ok?"Vendiste5madera por5oro. Ingresos del asentamiento disponibles para mejoras.":"Venta de madera: campamento y al menos5madera.");return ok;}
        private void LateUpdate()
        {
            if(panel==null)return;var state=Flow.Session.State;var available=Flow.IsPlaying&&state.Demo!=null&&state.Demo.Chapter>=DemoChapter.Settlement&&!Flow.Session.Paused;
            var modal=Flow.Front.Presentation!=null&&(Flow.ExistingHud.CompactView.WindowOpen||Flow.ExistingHud.CompactView.HelpOpen);
            manage.gameObject.SetActive(available&&!modal&&Flow.Session.Mode==PrototypeMode.Exploration);panel.gameObject.SetActive(available&&!modal&&Flow.Session.Mode==PrototypeMode.Construction);if(!panel.gameObject.activeSelf)return;
            if(lastCount!=state.Buildings.Count){lastCount=state.Buildings.Count;selected=lastCount-1;}
            resources.text="Oro "+state.Gold+" · Madera "+state.Wood+" · Alimento "+state.Food+" · Cap. "+SettlementOperations.Capacity(state,Rules)+" · Ejército "+state.Army.Count+"/6";
            var b=Selected;selectedLabel.text=b==null?"Ningún edificio seleccionado.":Rules.Find(b.Id).Title+" · nivel "+b.Level+(b.ProductionPaused?" · producción pausada":" · activo");
            activate.interactable=b!=null&&(Rules.Find(b.Id).FoodPerCycle>0||Rules.Find(b.Id).WoodPerCycle>0);upgrade.interactable=b!=null&&Rules.Find(b.Id).Upgradeable&&b.Level==1&&state.Gold>=Rules.UpgradeGold&&state.Wood>=Rules.UpgradeWood;
            activate.GetComponentInChildren<TMP_Text>().text=b!=null&&!b.ProductionPaused?"Pausar":"Activar";
            for(var i=0;i<recruits.Count;i++){var role=Flow.Director.Rts.DemoRules.Roles[i].Id;recruits[i].interactable=SettlementOperations.Has(state,"barracks")&&state.Gold>=Rules.GoldCost(role)&&state.Food>=Rules.FoodCost(role)&&state.Army.Count<Rules.MaximumRecruits;}
        }
    }
}
