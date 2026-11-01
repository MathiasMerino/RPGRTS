using System;
using UnityEngine;

namespace Aethernia
{
    [Serializable] public sealed class RtsRoleProfile
    {
        public string Id,Title,VisualClass;
        public int Health=10,Damage=2;
        public float Range=1.5f,Speed=2.5f;
        public RtsRoleProfile(string id,string title,string model,float range,float speed){Id=id;Title=title;VisualClass=model;Range=range;Speed=speed;}
    }
    [CreateAssetMenu(menuName="Aethernia/RTS Demo Rules")]
    public sealed class RtsDemoRules : ScriptableObject
    {
        public RtsRoleProfile[] Roles={
            new RtsRoleProfile("swordsman","Espadachín","warrior",1.5f,2.5f),
            new RtsRoleProfile("brawler","Luchador","monk",1.3f,3.2f),
            new RtsRoleProfile("archer","Arquero","warrior",6,2.5f)
        };
        public RtsRoleProfile Find(string id)=>Array.Find(Roles,x=>x.Id==id);
    }
    public readonly struct RtsUnitStatus
    {
        public readonly ExplorationUnit Unit;
        public readonly string Role;
        public readonly int Team,Health,Maximum;
        public readonly bool Hero;
        public RtsUnitStatus(ExplorationUnit unit,string role,int team,int health,int maximum,bool hero){Unit=unit;Role=role;Team=team;Health=health;Maximum=maximum;Hero=hero;}
    }
}
