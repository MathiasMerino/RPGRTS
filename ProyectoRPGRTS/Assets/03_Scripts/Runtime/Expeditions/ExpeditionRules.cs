using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    [Serializable] public sealed class TerritoryRecord
    {
        public string Id,Owner;
        public bool Secured;
        public TerritoryRecord(string id,string owner,bool secured=false){Id=id;Owner=owner;Secured=secured;}
    }
    [Serializable] public sealed class FactionDefinition
    {
        public string Id,Title,CultureId,KingdomId;
        public string[] Roles={"swordsman","archer","brawler"};
        public FactionDefinition(string id,string title){Id=id;Title=title;CultureId="";KingdomId="";}
    }
    [Serializable] public sealed class TerritoryDefinition
    {
        public string Id,Title,InitialOwner,ConnectedTo;
        public Vector3 Position;
        public int WoodPerCycle;
        public TerritoryDefinition(string id,string title,string owner,Vector3 position,string connection,int wood){Id=id;Title=title;InitialOwner=owner;Position=position;ConnectedTo=connection;WoodPerCycle=wood;}
    }
    [CreateAssetMenu(menuName="Aethernia/Expedition Rules")]
    public sealed class ExpeditionRules : ScriptableObject
    {
        public RtsDemoRules Units;
        public int GoldCost(string role)=>Units?.Find(role)?.RecruitGold>0?Units.Find(role).RecruitGold:RecruitGold;
        public int FoodCost(string role)=>Units?.Find(role)?.RecruitFood>0?Units.Find(role).RecruitFood:RecruitFood;
        public int MaximumCompanions=3,RecruitGold=5,RecruitFood=2,RestSeconds=10;
        public int DungeonGold=30,DungeonWood=15,DungeonFood=6,OutpostDefenders=3;
        public FactionDefinition[] Factions={new FactionDefinition("player-company","Compañía del campamento"),new FactionDefinition("faction-a","Facción A"),new FactionDefinition("faction-b","Facción B")};
        public TerritoryDefinition[] Territories={
            new TerritoryDefinition("camp","Campamento","player-company",new Vector3(9,0,-10),"",0),
            new TerritoryDefinition("crossing","Paso del bosque","",new Vector3(11,0,8),"camp",1),
            new TerritoryDefinition("outpost","Puesto del Umbral","faction-b",new Vector3(-12,0,-13),"crossing",1)};
        public TerritoryDefinition Find(string id)=>Array.Find(Territories,t=>t.Id==id);
        public static bool KnownTerritory(string id)=>id=="camp"||id=="crossing"||id=="outpost";
        public static bool KnownFaction(string id)=>id==""||id=="player-company"||id=="faction-a"||id=="faction-b";
        public static ClassProfile RoleProfile(RtsRoleProfile r)=>new ClassProfile(r.VisualClass,r.Title,r.Health,r.Damage,r.Range,r.Speed);
    }
    public static class ExpeditionOperations
    {
        public static bool Ready(PrototypeState s)=>s.Demo!=null&&s.Demo.Chapter>=DemoChapter.Settlement&&s.Facts.Contains("demo-camp-secured");
        public static bool Recruit(PrototypeState s,ExpeditionRules rules,RtsDemoRules roles,string role)
        {
            var p=roles.Find(role);if(!Ready(s)||p==null||s.Companions.Count>=rules.MaximumCompanions||s.Companions.Any(c=>c.Role==role)||s.Gold<rules.GoldCost(role)||s.Food<rules.FoodCost(role))return false;
            s.Gold-=rules.GoldCost(role);s.Food-=rules.FoodCost(role);s.Companions.Add(new RecruitRecord{Id=Guid.NewGuid().ToString("N"),Role=role,Health=p.Health,InParty=true});return true;
        }
        public static bool SetParty(PrototypeState s,ExpeditionRules rules,string id,bool active)
        {
            var c=s.Companions.Find(x=>x.Id==id);if(c==null||c.InParty==active||(active&&(c.Health<=0||s.Companions.Count(x=>x.InParty)>=rules.MaximumCompanions)))return false;c.InParty=active;return true;
        }
        public static void InitializeTerritories(PrototypeState s,ExpeditionRules rules)
        {
            if(!Ready(s))return;
            foreach(var p in rules.Territories)if(s.Territories.All(t=>t.Id!=p.Id))s.Territories.Add(new TerritoryRecord(p.Id,p.InitialOwner,p.Id=="camp"&&p.InitialOwner==s.FactionId));
            var camp=s.Territories.Find(t=>t.Id=="camp");if(camp?.Owner==s.FactionId)camp.Secured=true;
        }
        public static bool RewardDungeon(PrototypeState s,ExpeditionRules rules)
        {
            if(!s.Facts.Contains("umbral-battle-1")||!s.Facts.Contains("umbral-battle-2")||s.Facts.Contains("umbral-reward"))return false;
            s.Gold=Math.Min(1000000,s.Gold+rules.DungeonGold);s.Wood=Math.Min(1000000,s.Wood+rules.DungeonWood);s.Food=Math.Min(1000000,s.Food+rules.DungeonFood);s.Facts.Add("umbral-reward");return true;
        }
        public static bool ClaimNeutral(PrototypeState s,ExpeditionRules rules)
        {
            InitializeTerritories(s,rules);var t=s.Territories.Find(x=>x.Id=="crossing");if(t==null||t.Owner!="")return false;t.Owner=s.FactionId;t.Secured=true;return true;
        }
        public static bool Conquer(PrototypeState s,string id,bool actualWin)
        {
            var t=s.Territories.Find(x=>x.Id==id);if(!actualWin||id!="outpost"||t==null||t.Owner==s.FactionId||s.Health<=0)return false;t.Owner=s.FactionId;t.Secured=true;if(!s.Facts.Contains("umbral-outpost-conquered"))s.Facts.Add("umbral-outpost-conquered");s.ReputationB=Math.Max(-1000,s.ReputationB-5);return true;
        }
        public static int Relation(PrototypeState s,string faction)=>faction=="faction-a"?s.ReputationA:faction=="faction-b"?s.ReputationB:0;
        public static void ProduceTerritories(PrototypeState s,SettlementRules storage,int cycles)
        {
            if(s.Territories==null||s.FactionId!="player-company"||storage==null)return;
            var per=s.Territories.Count(t=>t.Secured&&t.Owner==s.FactionId&&(t.Id=="crossing"||t.Id=="outpost"));
            s.Wood=Math.Min(Math.Max(s.Wood,SettlementOperations.Capacity(s,storage)),s.Wood+per*cycles);
        }
        public static bool Valid(PrototypeState s,ProgressionRules progression=null)
        {
            if(s.Companions==null||s.ArmySelection==null||s.Territories==null||s.Companions.Count>3||s.Companions.Count(c=>c!=null&&c.InParty)>3||s.Companions.Any(c=>c==null||!ValidId(c.Id)||!SettlementRules.KnownRole(c.Role)||!TroopProgression.Valid(s,c,progression,false)))return false;
            if(s.Companions.Select(c=>c.Id).Distinct().Count()!=s.Companions.Count||s.Companions.Select(c=>c.Role).Distinct().Count()!=s.Companions.Count||s.Companions.Any(c=>s.Army.Any(a=>a.Id==c.Id)))return false;
            if(s.ArmySelection.Count>6||s.ArmySelection.Distinct().Count()!=s.ArmySelection.Count||s.ArmySelection.Any(id=>s.Army.All(a=>a.Id!=id)))return false;
            if(s.Territories.Count>3||s.Territories.Any(t=>t==null||!ExpeditionRules.KnownTerritory(t.Id)||!ExpeditionRules.KnownFaction(t.Owner)||(t.Secured&&t.Owner!="player-company")))return false;
            return s.Territories.Select(t=>t.Id).Distinct().Count()==s.Territories.Count&&s.FactionId=="player-company"&&(s.CultureId??"").Length<=64&&(s.KingdomId??"").Length<=64;
        }
        private static bool ValidId(string id)=>id!=null&&id.Length==32&&id.All(Uri.IsHexDigit);
    }
}
