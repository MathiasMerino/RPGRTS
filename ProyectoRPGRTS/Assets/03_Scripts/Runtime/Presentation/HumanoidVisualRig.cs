using UnityEngine;

namespace Aethernia
{
    public sealed class HumanoidVisualRig : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SkinnedMeshRenderer body;
        public Animator Animator => animator;
        public void Configure(Animator value)
        {
            animator = value; body = null;
            foreach (var skin in GetComponentsInChildren<SkinnedMeshRenderer>(true))
                if (skin.sharedMesh != null && skin.bones.Length > 0) { body = skin; break; }
        }
        public bool IsUsable
        {
            get
            {
                if (animator == null || animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman) return false;
                if (GetComponent<CharacterVisualModel>() == null || (transform.localScale - Vector3.one).sqrMagnitude > .0001f) return false;
                return body != null && body.sharedMesh != null && body.rootBone != null;
            }
        }
    }
}
