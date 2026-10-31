using UnityEngine;

namespace Aethernia
{
    public enum CharacterMotion { Idle, Walk, Run, Attack, Defend, Hit, Defeat }

    [CreateAssetMenu(menuName = "Aethernia/Humanoid Animation Profile")]
    public sealed class HumanoidAnimationProfile : ScriptableObject
    {
        public RuntimeAnimatorController Controller;
        public AnimationClip Idle, Walk, Run, Attack, Defend, Hit, Defeat;
        [Min(.1f)] public float RunSpeed = 3.8f;
        public AnimationClip Clip(CharacterMotion motion)
        {
            switch (motion)
            {
                case CharacterMotion.Idle: return Idle;
                case CharacterMotion.Walk: return Walk;
                case CharacterMotion.Run: return Run;
                case CharacterMotion.Attack: return Attack;
                case CharacterMotion.Defend: return Defend;
                case CharacterMotion.Hit: return Hit;
                default: return Defeat;
            }
        }
        public bool HasClip(CharacterMotion motion)
        { var clip = Clip(motion); return clip != null && !clip.legacy && clip.humanMotion && !clip.empty; }
    }
}
