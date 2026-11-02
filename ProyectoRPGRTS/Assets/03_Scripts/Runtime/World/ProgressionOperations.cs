using System;
using System.Linq;
namespace Aethernia
{
    public static class ProgressionOperations
    {
        public const string LedgerPrefix="xp-e18:";
        public static int Rank(PrototypeState state,string branch)=>state.Skills?.Find(s=>s!=null&&s.Id==state.ClassId+"-"+branch)?.Rank??0;
        public static int AvailablePoints(PrototypeState state)=>Math.Max(0,state.HeroLevel-1-(state.Skills?.Sum(s=>s.Rank)??0));
        public static bool Activate(PrototypeState state,ProgressionRules rules)
        {
            if(rules==null||state.ProgressionRevision!=0)return false;
            state.ProgressionRevision=1;state.Experience=0;state.HeroLevel=1;state.Skills=new System.Collections.Generic.List<SkillRank>();
            // A single migration credit for past real facts, never replayed on subsequent loads.
            foreach(var reward in rules.Rewards)GrantForFact(state,rules,reward.Fact);return true;
        }
        public static int GrantForFact(PrototypeState state,ProgressionRules rules,string fact)
        {
            if(rules==null||state.ProgressionRevision!=1||!state.Facts.Contains(fact)||state.Facts.Contains(LedgerPrefix+fact))return 0;
            var reward=Array.Find(rules.Rewards,r=>r.Fact==fact);if(reward==null||reward.Amount<=0)return 0;
            var before=state.Experience;state.Facts.Add(LedgerPrefix+fact);state.Experience=Math.Min(rules.MaximumExperience,state.Experience+reward.Amount);state.HeroLevel=rules.LevelAt(state.Experience);return state.Experience-before;
        }
        public static bool Learn(PrototypeState state,ProgressionRules rules,string id)
        {
            if(rules==null||state.ProgressionRevision!=1||(id!=state.ClassId+"-impact"&&id!=state.ClassId+"-discipline")||AvailablePoints(state)<=0)return false;
            var current=state.Skills.Find(s=>s.Id==id);var rank=current?.Rank??0;
            if(rank>=rules.MaximumSkillRank||state.HeroLevel<2+rank)return false;
            if(current==null)state.Skills.Add(new SkillRank(id,1));else current.Rank++;return true;
        }
        public static ClassProfile ResolveProfile(PrototypeState state,PrototypeRules rules)
        {
            var original=rules.Find(state.ClassId);var growth=rules.Progression?.Find(state.ClassId);
            if(state.ProgressionRevision!=1||growth==null)return original;
            var level=state.HeroLevel;var discipline=Rank(state,"discipline");var scores=new AttributeScores(growth.Primary,growth.Secondary,level);
            var result=new ClassProfile(original.Id,original.Title,original.Health+(level-1)*growth.HealthPerLevel+discipline*2+(ItemCatalog.Find(state.Accessory,rules.Progression.Items)?.MaximumHealth??0),original.Damage+Math.Max(0,scores.Modifier(growth.Primary)-2),original.Range,original.Speed);
            result.Combat=new HeroCombatModifiers{Attributes=scores,Accuracy=scores.Modifier(growth.Primary),Defense=12+Math.Max(0,scores.Modifier(HeroAttribute.Dexterity)-2),GuardReduction=1+discipline+(state.ClassId=="warrior"&&level>=2?1:0),TechniqueId=growth.TechniqueId,TechniqueTitle=growth.TechniqueTitle,TechniqueDamage=growth.TechniqueDamage+Rank(state,"impact")*2,TechniqueAccuracy=growth.TechniqueAccuracy,TechniqueRange=growth.TechniqueRange,TechniqueUnlocked=level>=2,FocusCapacity=rules.Progression.FocusCapacity,TechniqueCost=rules.Progression.TechniqueCost,TechniqueCooldown=rules.Progression.TechniqueCooldown};return result;
        }
        public static bool Valid(PrototypeState state,PrototypeRules rules)
        {
            if(state.Skills==null||state.Skills.Any(s=>s==null))return false;
            if(state.ProgressionRevision==0)return state.Experience==0&&state.HeroLevel==1&&state.Skills.Count==0;
            var config=rules.Progression;if(state.ProgressionRevision!=1||config==null||state.Experience<0||state.Experience>config.MaximumExperience||state.HeroLevel!=config.LevelAt(state.Experience))return false;
            if(state.Skills.Select(s=>s.Id).Distinct().Count()!=state.Skills.Count||state.Skills.Any(s=>(s.Id!=state.ClassId+"-impact"&&s.Id!=state.ClassId+"-discipline")||s.Rank<1||s.Rank>config.MaximumSkillRank||state.HeroLevel<1+s.Rank)||state.Skills.Sum(s=>s.Rank)>state.HeroLevel-1)return false;
            var sum=0;foreach(var fact in state.Facts.Where(f=>f.StartsWith(LedgerPrefix,StringComparison.Ordinal)))
            {var source=fact.Substring(LedgerPrefix.Length);var reward=Array.Find(config.Rewards,r=>r.Fact==source);if(reward==null||!state.Facts.Contains(source))return false;sum+=reward.Amount;}
            return state.Experience==Math.Min(config.MaximumExperience,sum);
        }
    }
}
