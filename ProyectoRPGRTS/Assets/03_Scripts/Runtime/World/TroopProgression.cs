using System;
using System.Collections.Generic;
using System.Linq;
namespace Aethernia
{
    public static class TroopProgression
    {
        private const string Prefix="troop-e18:";
        public static ClassProfile Profile(RecruitRecord record,RtsRoleProfile role)
        {var bonus=Math.Max(0,record.Level-1);return new ClassProfile(role.VisualClass,role.Title,role.Health+bonus*2,role.Damage+bonus,role.Range,role.Speed);}
        public static int AwardCompanions(PrototypeState state,string fact,IEnumerable<TacticalCombatant> participants)
        {
            if(state.ProgressionRevision!=1||!state.Facts.Contains(fact)||(fact!="umbral-battle-1"&&fact!="umbral-battle-2"))return 0;
            var ids=participants.Where(p=>!p.Hero&&!p.Hostile).Select(p=>p.Id).ToArray();var count=0;
            foreach(var c in state.Companions.Where(c=>ids.Contains(c.Id)))
            {var key=Prefix+fact+":"+c.Id;if(state.Facts.Contains(key))continue;state.Facts.Add(key);c.Experience=Math.Min(8,c.Experience+4);c.Level=c.Experience>=8?2:1;count++;}return count;
        }
        public static bool CanTrain(PrototypeState s,RecruitRecord r)=>s.ProgressionRevision==1&&r!=null&&s.Army.Contains(r)&&r.Level==1&&s.Gold>=8&&s.Wood>=3&&SettlementOperations.Has(s,"barracks")&&s.Buildings.Any(b=>b.Id=="workshop"&&b.Level==2);
        public static bool Train(PrototypeState s,RecruitRecord r)
        {if(!CanTrain(s,r))return false;s.Gold-=8;s.Wood-=3;r.Level=2;return true;}
        public static bool Valid(PrototypeState s,RecruitRecord r,ProgressionRules rules,bool army)
        {
            if(r.Level<1||r.Level>2||r.Experience<0||r.Experience>8)return false;
            if(s.ProgressionRevision!=1||rules?.Units==null)return r.Level==1&&r.Experience==0&&r.Health>=(army?1:0)&&r.Health<=10;
            var role=rules.Units.Find(r.Role);if(role==null||r.Health<(army?1:0)||r.Health>Profile(r,role).Health)return false;
            if(army)return r.Experience==0;
            var expected=0;foreach(var fact in new[]{"umbral-battle-1","umbral-battle-2"})if(s.Facts.Contains(Prefix+fact+":"+r.Id)){if(!s.Facts.Contains(fact))return false;expected+=4;}
            return r.Experience==expected&&r.Level==(r.Experience>=8?2:1);
        }
    }
}
