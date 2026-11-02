using System;
using UnityEngine;
namespace Aethernia
{
    public enum HeroAttribute { Strength,Dexterity,Constitution,Intelligence,Wisdom,Charisma }
    [Serializable]public sealed class SkillRank { public string Id;public int Rank;public SkillRank(string id,int rank){Id=id;Rank=rank;} }
    [Serializable]public sealed class ClassGrowth
    {
        public string ClassId,TechniqueId,TechniqueTitle;
        public HeroAttribute Primary,Secondary;
        public int HealthPerLevel,TechniqueDamage,TechniqueAccuracy;
        public float TechniqueRange;
        public ClassGrowth(string id,HeroAttribute primary,HeroAttribute secondary,int health,string technique,string title,int damage,int accuracy,float range)
        {ClassId=id;Primary=primary;Secondary=secondary;HealthPerLevel=health;TechniqueId=technique;TechniqueTitle=title;TechniqueDamage=damage;TechniqueAccuracy=accuracy;TechniqueRange=range;}
    }
    [Serializable]public sealed class ExperienceReward
    {public string Fact;public int Amount;public ExperienceReward(string fact,int amount){Fact=fact;Amount=amount;} }
    [CreateAssetMenu(menuName="Aethernia/Progression Rules")]
    public sealed class ProgressionRules:ScriptableObject
    {
        public ItemCatalog Items;
        public RtsDemoRules Units;
        public int DungeonHealthBonus=4,DungeonDamageBonus=1;
        public ItemStack[] DungeonLoot={new ItemStack("forge-ore",1),new ItemStack("pilgrim-charm",1)};
        public int[] ExperienceThresholds={0,12,32,60};
        public int MaximumSkillRank=2,FocusCapacity=2,TechniqueCost=1;
        public float TechniqueCooldown=3;
        public ClassGrowth[] Classes={
            new ClassGrowth("warrior",HeroAttribute.Strength,HeroAttribute.Constitution,3,"power-strike","Golpe poderoso",3,0,1.8f),
            new ClassGrowth("monk",HeroAttribute.Dexterity,HeroAttribute.Wisdom,2,"focused-strike","Golpe concentrado",1,4,2.2f),
            new ClassGrowth("mage",HeroAttribute.Intelligence,HeroAttribute.Wisdom,2,"arcane-bolt","Proyectil arcano",2,0,8)};
        public ExperienceReward[] Rewards={new ExperienceReward("rpg-victory",12),new ExperienceReward("demo-camp-secured",8),new ExperienceReward("lost-cargo-completed",8),new ExperienceReward("umbral-battle-1",10),new ExperienceReward("umbral-battle-2",10),new ExperienceReward("umbral-reward",12),new ExperienceReward("umbral-outpost-conquered",8)};
        public ClassGrowth Find(string id)=>Array.Find(Classes,c=>c.ClassId==id);
        public int MaximumExperience=>ExperienceThresholds[ExperienceThresholds.Length-1];
        public int LevelAt(int xp){var level=1;for(var i=1;i<ExperienceThresholds.Length;i++)if(xp>=ExperienceThresholds[i])level=i+1;return level;}
    }
    public readonly struct AttributeScores
    {
        private readonly HeroAttribute primary,secondary;private readonly int primaryValue;
        public AttributeScores(HeroAttribute main,HeroAttribute second,int level){primary=main;secondary=second;primaryValue=level>=3?16:14;}
        public int this[HeroAttribute attribute]=>attribute==primary?primaryValue:attribute==secondary?12:10;
        public int Modifier(HeroAttribute attribute)=>Mathf.FloorToInt((this[attribute]-10)/2f);
    }
    // A derived snapshot only. Never serialized into class assets or saved as accumulated bonuses.
    public sealed class HeroCombatModifiers
    {
        public AttributeScores Attributes;
        public int Accuracy=2,Defense=12,GuardReduction=1,TechniqueDamage,TechniqueAccuracy,FocusCapacity=2,TechniqueCost=1;
        public float TechniqueRange,TechniqueCooldown=3;
        public string TechniqueId,TechniqueTitle;
        public bool TechniqueUnlocked;
    }
}
