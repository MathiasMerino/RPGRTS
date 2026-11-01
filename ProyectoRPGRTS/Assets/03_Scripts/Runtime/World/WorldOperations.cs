using System;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    public static class WorldOperations
    {
        public static bool KnownItem(string id) => id == "training-sword" || id == "cloth" || id == "ration";
        public static string ItemTitle(string id) => id == "training-sword" ? "espada" : id == "cloth" ? "túnica" : "ración";
        public static bool CanAdd(PrototypeState state, int count) => count > 0 && count <= 8 && state.Items.Sum(i => (long)i.Count) + count <= 8;
        public static bool AddItem(PrototypeState state, string id, int count)
        {
            if (!KnownItem(id) || !CanAdd(state, count)) return false;
            var item = state.Items.Find(i => i.Id == id);
            if (item == null) state.Items.Add(new ItemStack(id, count)); else item.Count += count;
            return true;
        }
        public static bool Equip(PrototypeState state, string id)
        {
            if (!state.Items.Any(i => i.Id == id && i.Count > 0)) return false;
            if (id == "training-sword") state.Weapon = id;
            else if (id == "cloth") state.Armour = id;
            else return false;
            return true;
        }
        public static bool UseRation(PrototypeState state, int maxHealth)
        {
            var item = state.Items.Find(i => i.Id == "ration" && i.Count > 0);
            if (item == null || state.Health >= maxHealth) return false;
            state.Health = Math.Min(maxHealth, state.Health + 4);
            if (--item.Count == 0) state.Items.Remove(item);
            return true;
        }
        public static bool Trade(PrototypeState state)
        { if (state.Gold < 5) return false; state.Gold -= 5; state.Wood += 2; return true; }
        public static bool BuildWorkshop(PrototypeState state, Vector3 position)
        {
            if (state.Gold < 10 || state.Wood < 5 || state.Buildings.Count >= 32) return false;
            state.Gold -= 10; state.Wood -= 5;
            state.Buildings.Add(new BuildingRecord("workshop", position)); return true;
        }
        public static void Advance(PrototypeState state, float seconds,SettlementRules settlement=null)
        {
            if (seconds <= 0 || float.IsNaN(seconds) || float.IsInfinity(seconds)) return;
            state.WorldSeconds += seconds;
            var next = Mathf.FloorToInt(state.WorldSeconds / 10);
            var cycles = Mathf.Clamp(next - state.ProductionCycle, 0, 1000);
            if(state.Demo!=null&&settlement!=null)SettlementOperations.Produce(state,settlement,cycles);
            else{var produced = Math.Min(state.Food, cycles * state.Buildings.Count);state.Food -= produced; state.Wood += produced;}
            state.ProductionCycle = next;
        }
        public static bool CompleteQuest(PrototypeState state)
        {
            if (state.Facts.Contains("dungeon-objective")) return false;
            state.Facts.Add("dungeon-objective"); state.ReputationA += 10; state.ReputationB -= 5;
            state.Gold += 15; return true;
        }
    }
}
