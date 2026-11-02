using System;

namespace Aethernia
{
    // Valores comunes; turnos y tiempo real conservan sus propios presupuestos/ejecución.
    public static class HeroActionRules
    {
        public static int AttackDamage(ClassProfile profile,int weaponBonus)=>profile.Damage+weaponBonus;
        public static int GuardedDamage(int damage,bool guarding,int reduction=1)=>Math.Max(0,damage-(guarding?reduction:0));
        public static int Accuracy(ClassProfile p)=>p.Combat?.Accuracy??2;
        public static int Defense(ClassProfile p)=>p.Combat?.Defense??12;
        public static int GuardReduction(ClassProfile p)=>p.Combat?.GuardReduction??1;
        public static int Recovery(int health,int maximum)=>Math.Min(4,Math.Max(0,maximum-health));
        public static int WeaponBonus(PrototypeState state)=>state.Weapon=="training-sword"?1:0;
    }
}
