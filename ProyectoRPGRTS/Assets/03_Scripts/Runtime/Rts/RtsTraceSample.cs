using UnityEngine;
namespace Aethernia
{
    // Observación de diagnóstico: no participa en decisiones, daño, navegación o guardado.
    public readonly struct RtsTraceSample
    {
        public readonly string Kind;public readonly int Frame,Actor,Health,Target,TargetHealth,Value,Allies,Enemies;
        public readonly float Time,Delta,AttackAt;public readonly bool Hero,Selected,RecoveryUsed;public readonly Vector3 Position;
        public RtsTraceSample(string kind,int frame,float time,float delta,int actor,bool hero,int health,float attackAt,bool selected,bool recoveryUsed,int target,int targetHealth,int value,Vector3 position,int allies,int enemies)
        {Kind=kind;Frame=frame;Time=time;Delta=delta;Actor=actor;Hero=hero;Health=health;AttackAt=attackAt;Selected=selected;RecoveryUsed=recoveryUsed;Target=target;TargetHealth=targetHealth;Value=value;Position=position;Allies=allies;Enemies=enemies;}
    }
}
