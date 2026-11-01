using UnityEngine;

namespace Aethernia
{
    public enum FeedbackSound { Select, PanelOpen, PanelClose, Confirm, Sword, Punch, Block, Arcane, Hurt, Miss, Victory, Defeat, Quest, Recover }
    [CreateAssetMenu(menuName="Aethernia/Combat audio bank")]
    public sealed class CombatAudioBank : ScriptableObject
    {
        public AudioClip[] Clips;
        [Range(0,1)]public float InterfaceVolume=.18f,WorldVolume=.30f;
        public AudioClip Find(FeedbackSound sound)=>Clips!=null&&(int)sound<Clips.Length?Clips[(int)sound]:null;
    }
}
