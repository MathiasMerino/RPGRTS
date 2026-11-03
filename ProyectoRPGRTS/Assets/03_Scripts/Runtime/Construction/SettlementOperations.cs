using System;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    // Transacciones puras; geometría y presentación permanecen en WorldConstruction/UI.
    public static class SettlementOperations
    {
        public static bool Has(PrototypeState state,string id)=>state.Buildings.Any(b=>b.Id==id);
        public static int Capacity(PrototypeState state,SettlementRules rules)=>rules.BaseStorage+state.Buildings.Sum(b=>((rules.Find(b.Id)?.Storage??0)+(b.Id==rules.Civilizations?.Culture(b.CultureId)?.Building?rules.Civilizations.Culture(b.CultureId).StorageBonus:0))*b.Level);
        public static bool Build(PrototypeState state,SettlementRules rules,string id,Vector3 point)
        {
            var b=rules.Find(id);
            if(float.IsNaN(point.sqrMagnitude)||float.IsInfinity(point.sqrMagnitude)||Mathf.Abs(point.x)>16||Mathf.Abs(point.z)>16||Mathf.Abs(point.y)>.2f)return false;
            if(state.Demo==null||state.Demo.Chapter<DemoChapter.Settlement||!state.Facts.Contains("demo-camp-secured")||b==null||state.Buildings.Count>=32)return false;
            if((b.RequiresCenter&&!Has(state,"townhall"))||(id=="townhall"&&Has(state,"townhall"))||state.Gold<b.Gold||state.Wood<b.Wood)return false;
            state.Gold-=b.Gold;state.Wood-=b.Wood;state.Buildings.Add(new BuildingRecord(id,point){CultureId=rules.Civilizations?.Culture(state.CultureId)?.Pilot==true?state.CultureId:"",ProductionPaused=b.FoodPerCycle>0||b.WoodPerCycle>0});return true;
        }
        public static bool SetProduction(PrototypeState state,SettlementRules rules,BuildingRecord building,bool enabled)
        {
            if(building==null||!state.Buildings.Contains(building))return false;var profile=rules.Find(building.Id);
            if(profile==null||(profile.FoodPerCycle==0&&profile.WoodPerCycle==0))return false;building.ProductionPaused=!enabled;return true;
        }
        public static bool Upgrade(PrototypeState state,SettlementRules rules,BuildingRecord building)
        {
            if(building==null||!state.Buildings.Contains(building)||rules.Find(building.Id)?.Upgradeable!=true||building.Level!=1||state.Gold<rules.UpgradeGold||state.Wood<rules.UpgradeWood)return false;
            state.Gold-=rules.UpgradeGold;state.Wood-=rules.UpgradeWood;building.Level=2;return true;
        }
        public static bool Recruit(PrototypeState state,SettlementRules rules,string role)
        {
            if(rules.Civilizations?.MilitaryRole(role)!=null)return CivilizationOperations.Recruit(state,rules,role);
            if(!SettlementRules.KnownRole(role)||!Has(state,"barracks")||state.Army==null||state.Army.Count>=rules.MaximumRecruits||state.Gold<rules.GoldCost(role)||state.Food<rules.FoodCost(role))return false;
            state.Gold-=rules.GoldCost(role);state.Food-=rules.FoodCost(role);state.Army.Add(new RecruitRecord{Id=Guid.NewGuid().ToString("N"),Role=role,Health=rules.Units?.Find(role)?.Health??10});CivilizationOperations.IdentifyCompanion(state.Army[state.Army.Count-1],rules.Civilizations);return true;
        }
        public static void Produce(PrototypeState state,SettlementRules rules,int cycles)
        {
            var capacity=Capacity(state,rules);
            for(var cycle=0;cycle<cycles;cycle++)
            {
                // Orden estable: alimento y después madera, independiente del orden de colocación/carga.
                foreach(var building in state.Buildings)
                {
                    var p=rules.Find(building.Id);if(p==null||building.ProductionPaused||p.FoodPerCycle<=0)continue;
                    var amount=Math.Min(p.FoodPerCycle*building.Level,Math.Max(0,capacity-state.Food));state.Food+=amount;Count(state,amount);
                }
                foreach(var building in state.Buildings)
                {
                    var p=rules.Find(building.Id);if(p==null||building.ProductionPaused||p.WoodPerCycle<=0)continue;
                    var extra=building.Id==rules.Civilizations?.Culture(building.CultureId)?.Building?rules.Civilizations.Culture(building.CultureId).WoodBonus:0;
                    var amount=Math.Min((p.WoodPerCycle+extra)*building.Level,Math.Max(0,capacity-state.Wood));
                    if(p.FoodConsumed>0)amount=Math.Min(amount,state.Food/p.FoodConsumed);
                    state.Food-=amount*p.FoodConsumed;state.Wood+=amount;Count(state,amount);
                }
            }
        }
        private static void Count(PrototypeState state,int amount){if(state.Demo!=null)state.Demo.Produced=Math.Min(1000000,state.Demo.Produced+amount);}
    }
}
