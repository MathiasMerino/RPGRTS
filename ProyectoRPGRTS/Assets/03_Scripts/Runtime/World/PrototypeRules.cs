using System;
using UnityEngine;

namespace Aethernia
{
    [Serializable]
    public sealed class ClassProfile
    {
        public string Id;
        public string Title;
        [NonSerialized] public HeroCombatModifiers Combat;
        public int Health;
        public int Damage;
        public float Range;
        public float Speed;
        public ClassProfile(string id, string title, int hp, int damage, float range, float speed)
        { Id = id; Title = title; Health = hp; Damage = damage; Range = range; Speed = speed; }
    }

    [CreateAssetMenu(menuName = "Aethernia/Prototype Rules")]
    public sealed class PrototypeRules : ScriptableObject
    {
        public bool UxExperience; // Opt-in: las escenas históricas mantienen sus reglas de entrada.
        public bool WorldExploration; // UX02: misma partida, presentación y contenido de mundo optativos.
        public bool TacticalWarfare;
        public bool FrontierCampaign;
        public bool LivingWorld;
        public ProgressionRules Progression;
        public CivilizationCatalog Civilizations;
        public ClassProfile[] Classes = {
            new ClassProfile("warrior", "Guerrero", 24, 6, 1.8f, 3.2f),
            new ClassProfile("mage", "Mago", 16, 8, 6, 3.2f),
            new ClassProfile("monk", "Monje", 20, 5, 1.8f, 4)
        };
        public ClassProfile Find(string id) => Array.Find(Classes, p => p.Id == id);
    }
}
