using System;
using UnityEngine;
namespace Aethernia
{
    public enum ItemKind { Weapon,Armour,Accessory,Consumable,Material,CollectiveResource,Quest }
    [Serializable]public sealed class ItemDefinition
    {
        public string Id,Title,ClassId="",VisualId="";
        public ItemKind Kind;
        public int Damage,Defense,MaximumHealth,SellGold;
        public ItemDefinition(string id,string title,ItemKind kind,int damage=0,int defense=0,int hp=0,string owner="",string visual="",int sell=1)
        {Id=id;Title=title;Kind=kind;Damage=damage;Defense=defense;MaximumHealth=hp;ClassId=owner;VisualId=visual;SellGold=sell;}
    }
    [CreateAssetMenu(menuName="Aethernia/Item Catalog")]
    public sealed class ItemCatalog:ScriptableObject
    {
        public ItemDefinition[] Entries=Defaults();
        private static readonly ItemDefinition[] standard=Defaults();
        private static ItemDefinition[] Defaults()=>new[]{
            new ItemDefinition("training-sword","Espada de entrenamiento",ItemKind.Weapon,1,visual:"training-sword"),
            new ItemDefinition("cloth","Túnica de tela",ItemKind.Armour,defense:1,visual:"cloth"),
            new ItemDefinition("ration","Ración",ItemKind.Consumable),
            new ItemDefinition("tempered-blade","Hoja templada",ItemKind.Weapon,2,owner:"warrior",visual:"training-sword",sell:4),
            new ItemDefinition("weighted-wraps","Vendas lastradas",ItemKind.Weapon,2,owner:"monk",sell:4),
            new ItemDefinition("runic-focus","Foco rúnico",ItemKind.Weapon,2,owner:"mage",sell:4),
            new ItemDefinition("reinforced-vest","Chaleco reforzado",ItemKind.Armour,defense:2,visual:"cloth",sell:3),
            new ItemDefinition("pilgrim-charm","Talismán del peregrino",ItemKind.Accessory,hp:2,sell:3),
            new ItemDefinition("forge-ore","Mineral de forja",ItemKind.Material,sell:2)};
        public static ItemDefinition Find(string id,ItemCatalog catalog=null)=>Array.Find(catalog==null?standard:catalog.Entries,e=>e.Id==id);
        public static bool Known(string id)=>Array.Exists(standard,e=>e.Id==id);
        public static string Visual(string id)=>Find(id)?.VisualId??(id??"");
    }
}
