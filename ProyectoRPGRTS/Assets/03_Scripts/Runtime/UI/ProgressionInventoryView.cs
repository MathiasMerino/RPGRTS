using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Aethernia
{
    public sealed class ProgressionInventoryView:MonoBehaviour
    {
        public DemoFlow Flow;
        private GameObject panel;private TMP_Text heading,detail;private DemoSettlementPanel settlement;
        private readonly Dictionary<string,Button> controls=new Dictionary<string,Button>();
        private readonly Dictionary<string,Button> slots=new Dictionary<string,Button>();
        private string selected="ration";
        public IReadOnlyDictionary<string,Button> Controls=>controls;
        public void Select(string id){if(ItemCatalog.Find(id,Flow.Session.Rules.Progression.Items)!=null)selected=id;}
        private ItemDefinition Selected=>ItemCatalog.Find(selected,Flow.Session.Rules.Progression.Items);
        private bool AtCamp=>Flow.IsPlaying&&Flow.Session.Mode==PrototypeMode.Exploration&&!Flow.Session.Paused&&Vector3.Distance(Flow.Director.Hero.transform.position,new Vector3(9,0,-10))<9;
        private void Start()
        {
            settlement=GetComponent<DemoSettlementPanel>();var ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);var root=DemoWidgets.Rect(Flow.ExistingHud.CompactView.Window.transform,"Progression inventory",32,80,836,348);panel=root.gameObject;ui.Surface(root,new Color(.04f,.045f,.045f,1));
            heading=ui.Text(root,"Inventory capacity","",8,0,486,24,18);heading.color=ui.Gold;var i=0;
            foreach(var item in Flow.Session.Rules.Progression.Items.Entries)
            {var id=item.Id;var button=ui.Button(root,"Select "+id,item.Title,8+(i%3)*164,30+(i/3)*70,158,64,()=>Select(id));button.GetComponentInChildren<TMP_Text>().fontSize=14;slots.Add(id,button);i++;}
            detail=ui.Text(root,"Selected item effects","",520,8,304,198,16);
            Add("equip","Equipar",520,220,148,()=>Flow.Session.EquipItem(selected));Add("use","Consumir",676,220,148,()=>Flow.Director.Execute("ration"));Add("sell","Vender",520,266,148,()=>settlement.SellItem(selected));Add("unequip","Retirar",676,266,148,()=>Flow.Session.UnequipItem(Selected.Kind));
            Add("forge","Forjar arma",8,244,118,()=>settlement.Forge());Add("vest","Chaleco",130,244,118,()=>settlement.BuyVest());Add("trade","Madera +2",252,244,118,()=>{if(AtCamp)Flow.Director.Execute("trade");});Add("wood-sale","Madera -5",374,244,118,()=>settlement.SellWood());Add("loot","Recoger botín pendiente",8,292,486,()=>Flow.Session.ClaimLoot());panel.SetActive(false);
            void Add(string id,string title,float x,float y,float width,UnityEngine.Events.UnityAction action){var b=ui.Button(root,id,title,x,y,width,38,action);b.GetComponentInChildren<TMP_Text>().fontSize=14;controls.Add(id,b);}
        }
        private void LateUpdate()
        {
            if(panel==null)return;var s=Flow.Session.State;var view=Flow.ExistingHud.CompactView;var visible=Flow.IsPlaying&&Flow.Session.Mode==PrototypeMode.Exploration&&view.WindowOpen&&Flow.ExistingHud.ActivePage==ExplorationPage.Inventory;panel.SetActive(visible);if(!visible)return;view.Inventory.gameObject.SetActive(false);
            var catalog=Flow.Session.Rules.Progression.Items;var used=s.Items.Sum(i=>i.Count);heading.text="BOLSA "+used+" / 8 · equipo, consumibles y materiales";
            foreach(var slot in slots){var item=ItemCatalog.Find(slot.Key,catalog);var count=s.Items.Find(i=>i.Id==slot.Key)?.Count??0;var label=slot.Value.GetComponentInChildren<TMP_Text>();label.text=item.Title+"\n×"+count+(slot.Key==selected?" · ELEGIDO":"");label.color=slot.Key==selected?Flow.Front.Presentation.Gold:Flow.Front.Presentation.Text;}
            var chosen=Selected;var owned=EquipmentOperations.Owns(s,selected);var equipped=s.Weapon==selected||s.Armour==selected||s.Accessory==selected;
            var description=new StringBuilder(chosen.Title+"\n"+KindTitle(chosen.Kind)+"\n");if(chosen.Damage>0)description.AppendLine("Daño +"+chosen.Damage);if(chosen.Defense>0)description.AppendLine("Defensa RPG +"+chosen.Defense+(chosen.Defense>=2?" · RTS mitiga1":""));if(chosen.MaximumHealth>0)description.AppendLine("PV máximos +"+chosen.MaximumHealth+" · no cura");if(chosen.Kind==ItemKind.Consumable)description.AppendLine("Restaura hasta4PV; consume1");if(chosen.Kind==ItemKind.Material)description.AppendLine("Forja: taller2 + mineral1 +8oro");if(chosen.ClassId!="")description.AppendLine("Clase: "+Flow.Session.Rules.Find(chosen.ClassId).Title);description.AppendLine(equipped?"EQUIPADO":owned?"Disponible":"Sin unidades");description.AppendLine("Venta: "+chosen.SellGold+"oro · en campamento");description.Append("Chaleco: taller,8oro/4madera\nMadera: compra2por5oro; vende5por5oro");detail.text=description.ToString();
            controls["equip"].interactable=owned&&(chosen.Kind==ItemKind.Weapon||chosen.Kind==ItemKind.Armour||chosen.Kind==ItemKind.Accessory)&&(chosen.ClassId==""||chosen.ClassId==s.ClassId);controls["unequip"].interactable=equipped;controls["use"].interactable=selected=="ration"&&owned&&s.Health<Flow.Session.HeroProfile.Health;controls["sell"].interactable=AtCamp&&owned&&!equipped;
            controls["forge"].interactable=AtCamp&&s.Buildings.Any(b=>b.Id=="workshop"&&b.Level==2)&&s.Gold>=8&&EquipmentOperations.Owns(s,"forge-ore");controls["vest"].interactable=AtCamp&&SettlementOperations.Has(s,"workshop")&&s.Gold>=8&&s.Wood>=4&&used<8;controls["trade"].interactable=AtCamp&&s.Gold>=5;controls["wood-sale"].interactable=AtCamp&&s.Wood>=5&&s.Gold<=999995;
            var pending=s.Facts.Contains(EquipmentOperations.LootPending);controls["loot"].interactable=pending&&used<=6;controls["loot"].GetComponentInChildren<TMP_Text>().text=pending?(used<=6?"Recoger mineral y talismán · 2 espacios":"Botín pendiente · libera 2 espacios") :s.Facts.Contains(EquipmentOperations.LootClaimed)?"Botín de las ruinas ya recibido":"Ruinas: mineral y talismán de primera finalización";
        }
        private static string KindTitle(ItemKind kind)=>kind==ItemKind.Weapon?"Arma":kind==ItemKind.Armour?"Armadura":kind==ItemKind.Accessory?"Accesorio":kind==ItemKind.Consumable?"Consumible":kind==ItemKind.Material?"Material de fabricación":kind==ItemKind.Quest?"Objeto de misión":"Recurso colectivo";
    }
}
