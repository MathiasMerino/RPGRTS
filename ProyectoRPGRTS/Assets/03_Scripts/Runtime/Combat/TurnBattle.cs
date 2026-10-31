using System;

namespace Aethernia
{
    public sealed class TurnBattle
    {
        private readonly Random random;
        private readonly ClassProfile profile;
        private readonly int weaponBonus;
        private readonly int armourBonus;
        private bool guarding;
        private bool recovered;
        public int HeroHealth { get; private set; }
        public int EnemyHealth { get; private set; } = 18;
        public bool ActionAvailable { get; private set; } = true;
        public bool Guarding => guarding;
        public bool RecoveryUsed => recovered;
        public float MovementRemaining { get; private set; } = 4;
        public int HeroInitiative { get; }
        public int EnemyInitiative { get; }
        public bool Finished => HeroHealth <= 0 || EnemyHealth <= 0;
        public bool Won => EnemyHealth <= 0 && HeroHealth > 0;
        public string Message { get; private set; } = "Tu turno: moverte, atacar o defender; después Finalizar turno.";
        public TurnBattle(ClassProfile hero, int health, int weapon, int armour, int seed, bool enemyOpeningInReach = true)
        {
            profile = hero; HeroHealth = health; weaponBonus = weapon; armourBonus = armour; random = new Random(seed);
            HeroInitiative = random.Next(1, 21) + (hero.Id == "monk" ? 2 : 0);
            EnemyInitiative = random.Next(1, 21);
            if (EnemyInitiative > HeroInitiative && enemyOpeningInReach && random.Next(1, 21) + 2 >= 12 + armourBonus) HeroHealth = Math.Max(0, HeroHealth - 3);
            Message = "Iniciativa " + HeroInitiative + " / " + EnemyInitiative + ". " + (EnemyInitiative > HeroInitiative ? "El rival actúa primero; ahora es tu turno." : "Actúas primero.");
        }
        public bool Attack(float distance)
        {
            if (Finished || !ActionAvailable || distance > profile.Range) { Message = "Sin acción o fuera de alcance."; return false; }
            var roll = random.Next(1, 21);
            var hit = roll + 2 >= 12;
            if (hit) EnemyHealth = Math.Max(0, EnemyHealth - profile.Damage - weaponBonus);
            ActionAvailable = false;
            Message = "d20=" + roll + (hit ? "; impacto. Enemigo: " + EnemyHealth : "; fallo.");
            return true;
        }
        public bool Guard()
        { if (Finished || !ActionAvailable) return false; guarding = true; ActionAvailable = false; Message = "Guardia: reduce el siguiente daño."; return true; }
        public bool Recover()
        {
            if (Finished || !ActionAvailable || recovered || HeroHealth >= profile.Health) return false;
            HeroHealth = Math.Min(profile.Health, HeroHealth + 4); recovered = true; ActionAvailable = false; Message = "Recuperas hasta 4 puntos."; return true;
        }
        public bool Move(float distance)
        { if (Finished || distance < 0 || distance > MovementRemaining + 0.01f) return false; MovementRemaining -= distance; return true; }
        public void EndTurn(bool enemyInReach)
        {
            if (Finished) return;
            if (enemyInReach && random.Next(1, 21) + 2 >= 12 + armourBonus)
                HeroHealth = Math.Max(0, HeroHealth - (guarding ? 2 : 3));
            guarding = false; ActionAvailable = true; MovementRemaining = 4;
            Message = Finished ? "Has sido derrotado. Reinicia el encuentro para probar de nuevo." : "Tu turno. Salud: " + HeroHealth;
        }
    }
}
