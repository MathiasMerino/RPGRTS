using System;
using System.Linq;
namespace Aethernia
{
    public enum FactionRelation { Ally,Neutral,Hostile }
    // Pure transactions; identity never determines political allegiance.
    public static class CivilizationOperations
    {
        public const string Pact="e19-pact-faction-b";
        public static bool KnownRace(PrototypeRules rules,string id)=>rules.Civilizations!=null?rules.Civilizations.Race(id)!=null:DemoContent.KnownLineage(id);
        public static string Identity(PrototypeState s,CivilizationCatalog c)=>c==null?"":(c.Race(s.RaceId)?.Title??s.RaceId)+" · "+(c.Culture(s.CultureId)?.Title??(string.IsNullOrEmpty(s.CultureId)?"cultura sin registrar":s.CultureId));
        public static FactionRelation Relation(PrototypeState s,string faction)
        {if(faction==s.FactionId||faction=="faction-a"&&s.ReputationA>=0||faction=="faction-b"&&s.Facts.Contains(Pact))return FactionRelation.Ally;if(string.IsNullOrEmpty(faction))return FactionRelation.Neutral;return faction=="faction-b"||faction=="faction-a"&&s.ReputationA<0?FactionRelation.Hostile:FactionRelation.Neutral;}
        public static bool ChooseCulture(PrototypeState s,CivilizationCatalog c,string id)
        {if(c?.Culture(id)?.Pilot!=true||s.CultureId==id)return false;s.CultureId=id;return true;}
        public static bool Treaty(PrototypeState s,CivilizationCatalog c)
        {if(c==null||!ExpeditionOperations.Ready(s)||!SettlementOperations.Has(s,"townhall")||s.Facts.Contains(Pact)||s.Territories.Any(t=>t.Id=="outpost"&&t.Owner==s.FactionId)||s.Gold<c.TreatyGold||s.Food<c.TreatyFood)return false;s.Gold-=c.TreatyGold;s.Food-=c.TreatyFood;s.Facts.Add(Pact);return true;}
        public static string RecruitRequirement(PrototypeState s,SettlementRules rules,string id)
        {
            var c=rules.Civilizations;var u=c?.Unit(id);if(u==null||!u.Enabled||c.Culture(u.CultureId)?.Pilot!=true)return "Perfil preparado, no jugable";
            if(!ExpeditionOperations.Ready(s))return "Campamento asegurado";
            if(!SettlementOperations.Has(s,"barracks"))return "Requiere cuartel";
            if(!s.Buildings.Any(b=>b.Id==u.Building&&b.CultureId==u.CultureId))return "Requiere "+rules.Find(u.Building).Title+" de "+c.Culture(u.CultureId).Title;
            if(!string.IsNullOrEmpty(u.AccessTerritory)&&!s.Territories.Any(t=>t.Id==u.AccessTerritory&&t.Owner==s.FactionId&&t.Secured)&&Relation(s,c.Culture(u.CultureId).Faction)!=FactionRelation.Ally)return "Conquista el puesto o acuerda un pacto";
            if(s.Army.Count>=rules.MaximumRecruits)return "Ejército completo (6)";
            if(s.Gold<u.Profile.RecruitGold||s.Food<u.Profile.RecruitFood)return "Recursos insuficientes";
            return "Disponible";
        }
        public static bool Recruit(PrototypeState s,SettlementRules rules,string id)
        {if(RecruitRequirement(s,rules,id)!="Disponible")return false;var u=rules.Civilizations.Unit(id);s.Gold-=u.Profile.RecruitGold;s.Food-=u.Profile.RecruitFood;s.Army.Add(new RecruitRecord{Id=Guid.NewGuid().ToString("N"),Role=id,Health=u.Profile.Health,RaceId=u.RaceId,CultureId=u.CultureId});return true;}
        public static bool Valid(PrototypeState s,PrototypeRules rules)
        {
            var c=rules.Civilizations;if(s.IdentityRevision<0||s.IdentityRevision>1)return false;
            if(s.IdentityRevision==1&&(c==null||!c.Allows(s.RaceId,s.ClassId)||(!string.IsNullOrEmpty(s.CultureId)&&c.Culture(s.CultureId)?.Pilot!=true)))return false;
            if(s.Army==null||s.Companions==null||s.Buildings==null)return false;
            foreach(var r in s.Army.Concat(s.Companions))
            {if(r==null)return false;if(!string.IsNullOrEmpty(r.RaceId)&&(c?.Race(r.RaceId)==null))return false;if(!string.IsNullOrEmpty(r.CultureId)&&c?.Culture(r.CultureId)==null)return false;if(!SettlementRules.KnownRole(r.Role)&&(c?.MilitaryRole(r.Role)==null||r.RaceId!=c.Unit(r.Role).RaceId||r.CultureId!=c.Unit(r.Role).CultureId))return false;}
            return s.Buildings.All(b=>b!=null&&(string.IsNullOrEmpty(b.CultureId)||c?.Culture(b.CultureId)?.Pilot==true));
        }
        public static void IdentifyCompanion(RecruitRecord r,CivilizationCatalog c)
        {if(c==null)return;r.RaceId=r.Role=="brawler"?"orc":r.Role=="archer"?"elf":"human";r.CultureId="frontier";}
    }
}
