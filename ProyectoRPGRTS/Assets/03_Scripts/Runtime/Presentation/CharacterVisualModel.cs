using UnityEngine;

namespace Aethernia
{
    // Equipment presentation only. Never changes inventory, damage or armour values.
    public sealed class CharacterVisualModel : MonoBehaviour
    {
        [SerializeField] private GameObject trainingSword;
        [SerializeField] private GameObject clothArmour;
        public bool SwordVisible => trainingSword != null && trainingSword.activeSelf;
        public bool ArmourVisible => clothArmour != null && clothArmour.activeSelf;
        public void Configure(GameObject sword, GameObject armour) { trainingSword = sword; clothArmour = armour; }
        public void ApplyEquipment(PrototypeState state)
        {
            if (trainingSword != null) trainingSword.SetActive(state.Weapon == "training-sword");
            if (clothArmour != null) clothArmour.SetActive(state.Armour == "cloth");
        }
    }
}
