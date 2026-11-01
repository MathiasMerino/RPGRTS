namespace Aethernia
{
    public enum CombatSide { Player, Enemy }
    public enum TurnPhase { Player, Enemy, Finished }
    public enum CombatActionKind { Attack, Guard, Recover, Move, Wait }
    public readonly struct CombatResolution
    {
        public readonly int Sequence;
        public readonly CombatSide Actor;
        public readonly CombatActionKind Kind;
        public readonly bool Hit, Guarded;
        public readonly int Roll, Damage, Healing, TargetHealthBefore, TargetHealthAfter;
        public bool Accepted => Sequence > 0;
        public CombatResolution(int sequence, CombatSide actor, CombatActionKind kind, int roll=0, bool hit=false,
            int damage=0, int healing=0, int before=0, int after=0, bool guarded=false)
        { Sequence=sequence;Actor=actor;Kind=kind;Roll=roll;Hit=hit;Damage=damage;Healing=healing;TargetHealthBefore=before;TargetHealthAfter=after;Guarded=guarded; }
    }
    public enum EnemyIntent { Attack, Approach, Wait }
    public readonly struct EnemySituation
    {
        public readonly bool EnemyTurn, Alive, ValidTarget, InReach, ClearLine, CanApproach;
        public EnemySituation(bool turn,bool alive,bool target,bool reach,bool line,bool approach)
        {EnemyTurn=turn;Alive=alive;ValidTarget=target;InReach=reach;ClearLine=line;CanApproach=approach;}
    }
    // Política local sin efectos secundarios ni lectura de datos ocultos. El encuentro valida el mundo.
    public static class MeleeEnemyPolicy
    {
        public static EnemyIntent Decide(EnemySituation state)
        {
            if(!state.EnemyTurn||!state.Alive||!state.ValidTarget)return EnemyIntent.Wait;
            if(state.InReach&&state.ClearLine)return EnemyIntent.Attack;
            return state.CanApproach?EnemyIntent.Approach:EnemyIntent.Wait;
        }
    }
}
