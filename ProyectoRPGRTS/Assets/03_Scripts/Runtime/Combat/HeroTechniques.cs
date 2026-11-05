using System;
namespace Aethernia
{
    public sealed partial class TurnBattle
    {
        public int TechniqueFocus {get;private set;}
        public string LastAbilityId {get;private set;}
        public bool UseTechnique(float distance)
        {
            var skill=profile.Combat;
            if(skill==null||!skill.TechniqueUnlocked||Phase!=TurnPhase.Player||Finished||!ActionAvailable||TechniqueFocus<skill.TechniqueCost||float.IsNaN(distance)||float.IsInfinity(distance)||distance<0||distance>skill.TechniqueRange)return false;
            var roll=random.Next(1,21);var modifier=skill.Accuracy+skill.TechniqueAccuracy;var hit=roll+modifier>=12;var before=EnemyHealth;
            if(hit)EnemyHealth=Math.Max(0,before-HeroActionRules.AttackDamage(profile,weaponBonus)-skill.TechniqueDamage);
            TechniqueFocus-=skill.TechniqueCost;ActionAvailable=false;LastAbilityId=skill.TechniqueId;
            LastHeroResolution=new CombatResolution(++sequence,CombatSide.Player,CombatActionKind.Attack,roll,hit,before-EnemyHealth,0,before,EnemyHealth,modifier:modifier,defense:12);
            if(Finished)Phase=TurnPhase.Finished;Message=skill.TechniqueTitle+" · foco "+TechniqueFocus+" · "+(hit?"Impacto: "+(before-EnemyHealth):"Fallo");return true;
        }
    }
    public sealed partial class GroupTurnSequence
    {
        public int TechniqueFocus {get;private set;}
        public string LastAbilityId {get;private set;}
        public bool UseTechnique(TacticalCombatant target,float distance,bool clear)
        {
            var skill=Current.Profile.Combat;
            if(Finished||!Current.Hero||skill==null||!skill.TechniqueUnlocked||!ActionAvailable||TechniqueFocus<skill.TechniqueCost||target==null||!Contains(target)||!target.Hostile||target.Health<=0||!clear||float.IsNaN(distance)||float.IsInfinity(distance)||distance<0||distance>skill.TechniqueRange)return false;
            var roll=random.Next(1,21);var modifier=skill.Accuracy+skill.TechniqueAccuracy;var defense=HeroActionRules.Defense(target.Profile)+target.Armour;var hit=roll+modifier>=defense;var before=target.Health;
            if(hit)target.Health=Math.Max(0,before-HeroActionRules.GuardedDamage(HeroActionRules.AttackDamage(Current.Profile,Current.Weapon)+skill.TechniqueDamage,target.Guarding,HeroActionRules.GuardReduction(target.Profile)));
            TechniqueFocus-=skill.TechniqueCost;ActionAvailable=false;LastAbilityId=skill.TechniqueId;target.Guarding=false;
            LastResolution=new CombatResolution(++sequence,CombatSide.Player,CombatActionKind.Attack,roll,hit,before-target.Health,0,before,target.Health,modifier:modifier,defense:defense);return true;
        }
        private bool Contains(TacticalCombatant actor){foreach(var m in Members)if(m==actor)return true;return false;}
    }
    public sealed partial class TurnEncounter
    {
        public int TechniqueFocus=>InGroup?Group.TechniqueFocus:Battle?.TechniqueFocus??0;
        public bool Technique()
        {
            if(InGroup)
            {
                if(!GroupReady())return false;var source=GroupUnit(Group.Current.Id);TacticalCombatant target=null;foreach(var m in Group.Members)if(m.Id==groupTarget&&m.Health>0){target=m;break;}
                if(target==null)return false;var victim=GroupUnit(target.Id);if(!Group.UseTechnique(target,UnityEngine.Vector3.Distance(source.transform.position,victim.transform.position),HasClearAttackLine(source,victim)))return false;
                GroupPresent(Group.Current,target);session.Say(Group.Current.Profile.Combat.TechniqueTitle+" · foco "+Group.TechniqueFocus);GroupRefresh();return true;
            }
            if(!Ready()||!TargetSelected||!HasClearAttackLine(hero,enemy)||!Battle.UseTechnique(UnityEngine.Vector3.Distance(hero.transform.position,EnemyPosition)))return false;
            Show(EncounterCue.Attack,Battle.LastHeroResolution);Publish(Battle.LastHeroResolution);Sync();return true;
        }
    }
}
