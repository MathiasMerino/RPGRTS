using UnityEngine;

namespace Aethernia
{
    public enum VisualEquipmentSlot { Clothing, Armour, Weapon, Staff, Accessory }
    public enum EquipmentStateSource { Weapon, Armour }
    [System.Serializable] public sealed class EquipmentVisualBinding
    {
        public VisualEquipmentSlot Slot;
        public EquipmentStateSource Source;
        public string ItemId;
        public GameObject Visual;
    }
    // Equipment presentation only. Never changes inventory, damage or armour values.
    public sealed class CharacterVisualModel : MonoBehaviour
    {
        [SerializeField] private GameObject trainingSword;
        [SerializeField] private GameObject clothArmour;
        [SerializeField] private EquipmentVisualBinding[] bindings = System.Array.Empty<EquipmentVisualBinding>();
        [SerializeField] private Transform weaponAnchor;
        public Transform WeaponAnchor => weaponAnchor;
        public void ConfigureAnchor(Transform anchor) { weaponAnchor=anchor; }
        private HumanoidAnimationDriver animationDriver;
        public bool SwordVisible => trainingSword != null ? trainingSword.activeSelf : BindingVisible("training-sword");
        public bool ArmourVisible => clothArmour != null ? clothArmour.activeSelf : BindingVisible("cloth");
        public void Configure(GameObject sword, GameObject armour) { trainingSword = sword; clothArmour = armour; }
        public void ConfigureBindings(EquipmentVisualBinding[] value) { bindings = value ?? System.Array.Empty<EquipmentVisualBinding>(); }
        private bool Owned(GameObject view) => view != null && view != gameObject && view.transform.IsChildOf(transform);
        private bool BindingVisible(string id)
        { foreach (var binding in bindings) if (binding.ItemId == id && Owned(binding.Visual) && binding.Visual.activeSelf) return true; return false; }
        public void BindActor(ExplorationUnit actor)
        { animationDriver = GetComponent<HumanoidAnimationDriver>(); if (animationDriver != null) animationDriver.Bind(actor); }
        public void ApplyEquipment(PrototypeState state)
        {
            if (trainingSword != null) trainingSword.SetActive(state.Weapon == "training-sword");
            if (clothArmour != null) clothArmour.SetActive(state.Armour == "cloth");
            foreach (var binding in bindings)
                if (Owned(binding.Visual)) binding.Visual.SetActive(!string.IsNullOrEmpty(binding.ItemId) && binding.ItemId == (binding.Source == EquipmentStateSource.Weapon ? state.Weapon : state.Armour));
            if (animationDriver != null) animationDriver.SetDefeated(state.Health <= 0);
        }
    }
}
