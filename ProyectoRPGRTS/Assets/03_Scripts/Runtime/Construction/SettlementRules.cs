using System;
using UnityEngine;

namespace Aethernia
{
    [Serializable] public sealed class SettlementBlueprint
    {
        public string Id,Title;
        public int Gold,Wood,FoodPerCycle,WoodPerCycle,FoodConsumed,Storage;
        public bool RequiresCenter=true,Upgradeable;
        public SettlementBlueprint(string id,string title,int gold,int wood){Id=id;Title=title;Gold=gold;Wood=wood;}
    }
    [CreateAssetMenu(menuName="Aethernia/Settlement Demo Rules")]
    public sealed class SettlementRules : ScriptableObject
    {
        public SettlementBlueprint[] Blueprints={
            new SettlementBlueprint("townhall","Centro del campamento",15,8){RequiresCenter=false},
            new SettlementBlueprint("workshop","Taller de recolección",10,5){WoodPerCycle=1,FoodConsumed=1,Upgradeable=true},
            new SettlementBlueprint("farm","Granja",8,5){FoodPerCycle=2,Upgradeable=true},
            new SettlementBlueprint("barracks","Cuartel",12,8),
            new SettlementBlueprint("warehouse","Almacén",10,5){Storage=100,Upgradeable=true}
        };
        public RtsDemoRules Units;
        public CivilizationCatalog Civilizations;
        public int GoldCost(string role)=>Units?.Find(role)?.RecruitGold>0?Units.Find(role).RecruitGold:RecruitGold;
        public int FoodCost(string role)=>Units?.Find(role)?.RecruitFood>0?Units.Find(role).RecruitFood:RecruitFood;
        public int BaseStorage=200,RecruitGold=5,RecruitFood=2,MaximumRecruits=6,UpgradeGold=10,UpgradeWood=5;
        public SettlementBlueprint Find(string id)=>Array.Find(Blueprints,b=>b.Id==id);
        public static bool KnownBuilding(string id)=>id=="workshop"||id=="townhall"||id=="farm"||id=="barracks"||id=="warehouse";
        public static bool KnownRole(string id)=>id=="swordsman"||id=="brawler"||id=="archer";
    }
    [Serializable] public sealed class BuildingPrefabBinding {public string Id,CultureId="";public GameObject Prefab;}
}
