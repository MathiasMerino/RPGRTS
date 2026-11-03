using UnityEngine;

namespace Aethernia
{
    public sealed class HumanoidVisualRig : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SkinnedMeshRenderer body;
        [SerializeField] private bool allowGeneric;
        public Animator Animator => animator;
        public bool AllowsGeneric => allowGeneric;
        public void Configure(Animator value, bool generic = false)
        {
            animator = value; allowGeneric = generic; body = null;
            foreach (var skin in GetComponentsInChildren<SkinnedMeshRenderer>(true))
                if (skin.sharedMesh != null && skin.bones.Length > 0) { body = skin; break; }
        }
        public bool IsUsable
        {
            get
            {
                if (animator == null || animator.avatar == null || !animator.avatar.isValid || (!animator.avatar.isHuman && !allowGeneric)) return false;
                if (GetComponent<CharacterVisualModel>() == null || !RaceVisualVariant.ValidScale(transform)) return false;
                return body != null && body.sharedMesh != null && body.rootBone != null;
            }
        }
    }
}
