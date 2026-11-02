using System;
using System.Collections.Generic;
using System.Linq;

namespace Aethernia
{
    // Actor enlazado al estado existente; no almacena otra copia de salud.
    public sealed class TacticalCombatant
    {
        public string Id,Title;public ClassProfile Profile;public bool Hostile,Hero;
        public int Weapon,Armour,Initiative;
        public Func<int> ReadHealth;public Action<int> WriteHealth;
        public int Health{get=>ReadHealth();set=>WriteHealth(value);}
        internal bool Guarding,Recovered;
    }
    public sealed class GroupTurnSequence
    {
        public readonly IReadOnlyList<TacticalCombatant> Members;
        private readonly Random random;private int index,sequence;
        public int Round{get;private set;}=1;
        public bool ActionAvailable{get;private set;}=true;
        public float MovementRemaining{get;private set;}=4;
        public CombatResolution LastResolution{get;private set;}
        public TacticalCombatant Current=>Members[index];
        public bool Finished=>Members.First(x=>x.Hero).Health<=0||Members.All(x=>!x.Hostile||x.Health<=0);
        public bool Won=>Members.First(x=>x.Hero).Health>0&&Members.All(x=>!x.Hostile||x.Health<=0);
        public GroupTurnSequence(IEnumerable<TacticalCombatant> members,int seed)
        {
            random=new Random(seed);var list=members.ToList();if(list.Count<2||list.Count(x=>x.Hero)!=1||!list.Any(x=>x.Hostile)||list.Select(x=>x.Id).Distinct().Count()!=list.Count)throw new ArgumentException("Invalid tactical roster");
            foreach(var a in list)a.Initiative=random.Next(1,21)+(a.Profile.Id=="monk"?2:0);
            Members=list.OrderByDescending(x=>x.Initiative).ThenBy(x=>x.Id,StringComparer.Ordinal).ToArray();
            while(index<Members.Count-1&&Current.Health<=0)index++;
        }
        public bool Attack(TacticalCombatant target,float distance,bool clear)
        {
            if(Finished||!ActionAvailable||target==null||!Members.Contains(target)||target.Hostile==Current.Hostile||target.Health<=0||!clear||float.IsNaN(distance)||float.IsInfinity(distance)||distance<0||distance>Current.Profile.Range)return false;
            var roll=random.Next(1,21);var hit=roll+HeroActionRules.Accuracy(Current.Profile)>=HeroActionRules.Defense(target.Profile)+target.Armour;var before=target.Health;
            if(hit)target.Health=Math.Max(0,before-HeroActionRules.GuardedDamage(HeroActionRules.AttackDamage(Current.Profile,Current.Weapon),target.Guarding,HeroActionRules.GuardReduction(target.Profile)));
            LastResolution=new CombatResolution(++sequence,Current.Hostile?CombatSide.Enemy:CombatSide.Player,CombatActionKind.Attack,roll,hit,before-target.Health,0,before,target.Health,target.Guarding&&hit);target.Guarding=false;ActionAvailable=false;return true;
        }
        public bool Guard()
        {if(Finished||!ActionAvailable)return false;Current.Guarding=true;ActionAvailable=false;LastResolution=new CombatResolution(++sequence,Current.Hostile?CombatSide.Enemy:CombatSide.Player,CombatActionKind.Guard);return true;}
        public bool Recover()
        {
            if(Finished||!ActionAvailable||Current.Recovered)return false;var amount=HeroActionRules.Recovery(Current.Health,Current.Profile.Health);if(amount<=0)return false;
            var before=Current.Health;Current.Health+=amount;Current.Recovered=true;ActionAvailable=false;LastResolution=new CombatResolution(++sequence,Current.Hostile?CombatSide.Enemy:CombatSide.Player,CombatActionKind.Recover,healing:amount,before:before,after:Current.Health);return true;
        }
        public bool Move(float metres)
        {if(Finished||float.IsNaN(metres)||float.IsInfinity(metres)||metres<0||metres>MovementRemaining+.01f)return false;MovementRemaining-=metres;return true;}
        public bool EndTurn()
        {
            if(Finished)return false;do{index++;if(index>=Members.Count){index=0;Round++;}}while(Current.Health<=0);
            Current.Guarding=false;ActionAvailable=true;MovementRemaining=4;return true;
        }
    }
}
