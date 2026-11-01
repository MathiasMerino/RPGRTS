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
        private int sequence;
        private bool enemyActionAvailable;
        public TurnPhase Phase { get; private set; } = TurnPhase.Player;
        public int Round { get; private set; } = 1;
        public CombatResolution LastHeroResolution { get; private set; }
        public CombatResolution LastEnemyResolution { get; private set; }
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
            if (EnemyInitiative > HeroInitiative && enemyOpeningInReach) ResolveEnemyAttack(true);
            if (Finished) Phase = TurnPhase.Finished;
            Message = "Iniciativa " + HeroInitiative + " / " + EnemyInitiative + ". " + (EnemyInitiative > HeroInitiative ? "El rival actúa primero; ahora es tu turno." : "Actúas primero.");
        }
        public bool Attack(float distance)
        {
            if (Phase != TurnPhase.Player || Finished || !ActionAvailable || distance > profile.Range || distance < 0 || float.IsNaN(distance) || float.IsInfinity(distance)) { Message = "Sin acción o fuera de alcance."; return false; }
            var roll = random.Next(1, 21);
            var hit = roll + 2 >= 12;
            var before = EnemyHealth;
            if (hit) EnemyHealth = Math.Max(0, EnemyHealth - profile.Damage - weaponBonus);
            LastHeroResolution = new CombatResolution(++sequence, CombatSide.Player, CombatActionKind.Attack, roll, hit, before-EnemyHealth, 0, before, EnemyHealth);
            if (Finished) Phase = TurnPhase.Finished;
            ActionAvailable = false;
            Message = "d20=" + roll + (hit ? "; impacto. Enemigo: " + EnemyHealth : "; fallo.");
            return true;
        }
        public bool Guard()
        {
            if (Phase != TurnPhase.Player || Finished || !ActionAvailable) return false;
            guarding = true; ActionAvailable = false;
            LastHeroResolution = new CombatResolution(++sequence, CombatSide.Player, CombatActionKind.Guard);
            Message = "Guardia: reduce el siguiente daño."; return true;
        }
        public bool Recover()
        {
            if (Phase != TurnPhase.Player || Finished || !ActionAvailable || recovered || HeroHealth >= profile.Health) return false;
            var before = HeroHealth;
            HeroHealth = Math.Min(profile.Health, HeroHealth + 4);
            LastHeroResolution = new CombatResolution(++sequence, CombatSide.Player, CombatActionKind.Recover, healing:HeroHealth-before, before:before, after:HeroHealth);
            recovered = true; ActionAvailable = false; Message = "Recuperas hasta 4 puntos."; return true;
        }
        public bool Move(float distance)
        { if (Phase != TurnPhase.Player || Finished || float.IsNaN(distance) || float.IsInfinity(distance) || distance < 0 || distance > MovementRemaining + 0.01f) return false; MovementRemaining -= distance; LastHeroResolution = new CombatResolution(++sequence, CombatSide.Player, CombatActionKind.Move); return true; }
        public bool BeginEnemyTurn()
        {
            if (Phase != TurnPhase.Player || Finished) return false;
            Phase = TurnPhase.Enemy; enemyActionAvailable = true; Message = "Turno del enemigo."; return true;
        }
        public bool ResolveEnemyAction(bool inReach)
        {
            if (Phase != TurnPhase.Enemy || !enemyActionAvailable || Finished) return false;
            enemyActionAvailable = false; ResolveEnemyAttack(inReach); return true;
        }
        private void ResolveEnemyAttack(bool inReach)
        {
            var before = HeroHealth; var roll = inReach ? random.Next(1, 21) : 0;
            var hit = inReach && roll + 2 >= 12 + armourBonus;
            if (hit) HeroHealth = Math.Max(0, HeroHealth - (guarding ? 2 : 3));
            LastEnemyResolution = new CombatResolution(++sequence, CombatSide.Enemy,
                inReach ? CombatActionKind.Attack : CombatActionKind.Wait, roll, hit, before-HeroHealth, 0, before, HeroHealth, guarding && hit);
        }
        public bool FinishEnemyTurn()
        {
            if (Phase != TurnPhase.Enemy || enemyActionAvailable) return false;
            guarding = false; ActionAvailable = true; MovementRemaining = 4;
            Phase = Finished ? TurnPhase.Finished : TurnPhase.Player; Round++;
            Message = Finished ? "Has sido derrotado. Reinicia el encuentro para probar de nuevo." : "Tu turno. Salud: " + HeroHealth;
            return true;
        }
        // API síncrona previa: misma tirada y daño. La UI puede avanzar las tres fases por separado.
        public void EndTurn(bool enemyInReach)
        { if (BeginEnemyTurn()) { ResolveEnemyAction(enemyInReach); FinishEnemyTurn(); } }
    }
}
