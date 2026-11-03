using System;
using System.Linq;
namespace Aethernia
{
    public static class EquipmentOperations
    {
        public const string LootPending="e18-loot-pending",LootClaimed="e18-loot-claimed";
        public static bool Owns(PrototypeState state,string id)=>state.Items.Any(i=>i.Id==id&&i.Count>0);
        public static int ArmourBonus(PrototypeState state,ItemCatalog catalog=null)=>ItemCatalog.Find(state.Armour,catalog)?.Defense??0;
        public static bool Equip(PrototypeState state,string id,PrototypeRules rules)
        {
            var item=ItemCatalog.Find(id,rules.Progression?.Items);if(item==null||!Owns(state,id)||(!string.IsNullOrEmpty(item.ClassId)&&item.ClassId!=state.ClassId))return false;
            if(item.Kind==ItemKind.Weapon)state.Weapon=id;else if(item.Kind==ItemKind.Armour)state.Armour=id;else if(item.Kind==ItemKind.Accessory)state.Accessory=id;else return false;
            state.Health=Math.Min(state.Health,ProgressionOperations.ResolveProfile(state,rules).Health);return true;
        }
        public static bool Unequip(PrototypeState state,ItemKind kind,PrototypeRules rules)
        {
            if(kind==ItemKind.Weapon){if(state.Weapon=="")return false;state.Weapon="";}else if(kind==ItemKind.Armour){if(state.Armour=="")return false;state.Armour="";}else if(kind==ItemKind.Accessory){if(state.Accessory=="")return false;state.Accessory="";}else return false;
            state.Health=Math.Min(state.Health,ProgressionOperations.ResolveProfile(state,rules).Health);return true;
        }
        public static bool Sell(PrototypeState state,string id,PrototypeRules rules)
        {
            var item=ItemCatalog.Find(id,rules.Progression?.Items);if(item==null||!Owns(state,id)||id==state.Weapon||id==state.Armour||id==state.Accessory||state.Gold>1000000-item.SellGold)return false;
            Remove(state,id);state.Gold+=item.SellGold;return true;
        }
        private static void Remove(PrototypeState state,string id){var item=state.Items.Find(i=>i.Id==id);if(--item.Count==0)state.Items.Remove(item);}
        public static string AdvancedWeapon(string classId)=>classId=="warrior"?"tempered-blade":classId=="monk"?"weighted-wraps":"runic-focus";
        public static bool Forge(PrototypeState state)
        {
            if(state.ProgressionRevision!=1||!state.Buildings.Any(b=>b.Id=="workshop"&&b.Level==2)||state.Gold<8||!Owns(state,"forge-ore"))return false;
            var id=AdvancedWeapon(state.ClassId);if(!WorldOperations.KnownItem(id))return false;
            // Ore and output both occupy one unit: preflight before spending anything.
            if(state.Items.Sum(i=>i.Count)>8)return false;Remove(state,"forge-ore");WorldOperations.AddItem(state,id,1);state.Gold-=8;return true;
        }
        public static bool BuyVest(PrototypeState state)
        {
            if(state.ProgressionRevision!=1||!SettlementOperations.Has(state,"workshop")||state.Gold<8||state.Wood<4||!WorldOperations.CanAdd(state,1))return false;
            if(!WorldOperations.AddItem(state,"reinforced-vest",1))return false;state.Gold-=8;state.Wood-=4;return true;
        }
        public static bool SellWood(PrototypeState state)
        {
            if(state.ProgressionRevision!=1||state.Wood<5||state.Gold>999995)return false;state.Wood-=5;state.Gold+=5;return true;
        }
        public static bool RegisterDungeonLoot(PrototypeState state)
        {
            if(state.ProgressionRevision!=1||!state.Facts.Contains("umbral-reward")||state.Facts.Contains(LootPending)||state.Facts.Contains(LootClaimed))return false;state.Facts.Add(LootPending);return true;
        }
        public static bool ClaimDungeonLoot(PrototypeState state,ProgressionRules rules)
        {
            if(rules==null||!state.Facts.Contains(LootPending)||state.Facts.Contains(LootClaimed)||!state.Facts.Contains("umbral-reward"))return false;
            var count=rules.DungeonLoot.Sum(i=>i.Count);if(!WorldOperations.CanAdd(state,count)||rules.DungeonLoot.Any(i=>!WorldOperations.KnownItem(i.Id)||i.Count<=0))return false;
            foreach(var item in rules.DungeonLoot)WorldOperations.AddItem(state,item.Id,item.Count);state.Facts.Remove(LootPending);state.Facts.Add(LootClaimed);return true;
        }
        public static bool Valid(PrototypeState state,PrototypeRules rules)
        {
            bool Slot(string id,ItemKind kind){if(string.IsNullOrEmpty(id))return true;var item=ItemCatalog.Find(id,rules.Progression?.Items);return item!=null&&item.Kind==kind&&Owns(state,id)&&(string.IsNullOrEmpty(item.ClassId)||item.ClassId==state.ClassId);}
            if(!Slot(state.Weapon,ItemKind.Weapon)||!Slot(state.Armour,ItemKind.Armour)||!Slot(state.Accessory,ItemKind.Accessory))return false;
            var pending=state.Facts.Contains(LootPending);var claimed=state.Facts.Contains(LootClaimed);return !(pending&&claimed)&&(!(pending||claimed)||(state.ProgressionRevision==1&&state.Facts.Contains("umbral-reward")));
        }
    }
}
