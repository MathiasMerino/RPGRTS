using UnityEngine;

namespace Aethernia
{
    // Reads movement; never moves the actor, changes health, or applies combat effects.
    public sealed class HumanoidAnimationDriver : MonoBehaviour
    {
        [SerializeField] private HumanoidVisualRig rig;
        [SerializeField] private HumanoidAnimationProfile profile;
        private ExplorationUnit actor;
        private bool defeated, guarding;
        private bool controllerBound;
        public float ObservedSpeed { get; private set; }
        public bool IsMoving => actor != null && actor.IsMoving && ObservedSpeed > .05f;
        public HumanoidVisualRig Rig => rig;
        public HumanoidAnimationProfile Profile => profile;
        public bool CanAnimate => controllerBound && rig != null && rig.IsUsable && profile != null && profile.HasClip(CharacterMotion.Idle);
        public void Configure(HumanoidVisualRig value, HumanoidAnimationProfile animations)
        {
            rig = value; profile = animations;
            controllerBound = false;
            if (profile != null && profile.Controller != null && profile.Idle != null)
                foreach (var clip in profile.Controller.animationClips) if (clip == profile.Idle) { controllerBound = true; break; }
            if (rig != null && rig.Animator != null)
            {
                rig.Animator.applyRootMotion = false;
                rig.Animator.fireEvents = false;
                rig.Animator.runtimeAnimatorController = profile == null ? null : profile.Controller;
                rig.Animator.enabled = false;
            }
        }
        public void Bind(ExplorationUnit unit) { actor = unit; }
        private void OnEnable() { Configure(rig, profile); }
        public void SetDefeated(bool value) { defeated = value; }
        public void SetGuarding(bool value) { guarding = value; }
        public bool TryPlay(CharacterMotion motion)
        {
            if (!CanAnimate || !profile.HasClip(motion) || !rig.Animator.isInitialized || !rig.Animator.enabled) return false;
            if (defeated && motion != CharacterMotion.Defeat) return false;
            if (motion == CharacterMotion.Attack || motion == CharacterMotion.Hit) rig.Animator.SetTrigger(motion.ToString());
            else if (motion == CharacterMotion.Defend) guarding = true;
            else if (motion == CharacterMotion.Defeat) defeated = true;
            else return false;
            return true;
        }
        private void LateUpdate()
        {
            ObservedSpeed = actor != null && actor.Agent.enabled && actor.Agent.isOnNavMesh ? actor.PresentationVelocity.magnitude : 0;
            if (rig == null || rig.Animator == null) return;
            var animator = rig.Animator;
            if (!CanAnimate) { animator.enabled = false; return; }
            if (!animator.enabled) animator.enabled = true;
            animator.SetFloat("Speed", ObservedSpeed);
            var moving = IsMoving;
            var locomotion = moving && profile.HasClip(CharacterMotion.Walk) ? 1 : 0;
            if (moving && ObservedSpeed >= profile.RunSpeed && profile.HasClip(CharacterMotion.Run)) locomotion = 2;
            animator.SetInteger("Locomotion", locomotion);
            if (profile.UseGaitRate) animator.SetFloat("GaitRate", locomotion == 0 ? 1 : Mathf.Clamp(ObservedSpeed / (locomotion == 2 ? profile.RunReferenceSpeed : profile.WalkReferenceSpeed), .25f, 1.5f));
            animator.SetBool("Defending", !defeated && guarding && profile.HasClip(CharacterMotion.Defend));
            animator.SetBool("Defeated", defeated && profile.HasClip(CharacterMotion.Defeat));
        }
        private void OnDisable()
        { if (rig != null && rig.Animator != null) rig.Animator.enabled = false; }
    }
}
