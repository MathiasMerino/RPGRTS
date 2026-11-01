using UnityEngine;

namespace Aethernia
{
    // Seis voces fijas: dos UI y cuatro mundo. No crea fuentes/materiales por golpe.
    public sealed class CombatAudioPlayer : MonoBehaviour
    {
        [SerializeField]private CombatAudioBank bank;
        private AudioSource[] voices;
        private int uiIndex,worldIndex;
        public int PlayedCount { get; private set; }
        public int ActiveVoices { get {var n=0;if(voices!=null)foreach(var s in voices)if(s.isPlaying)n++;return n;} }
        public CombatAudioBank Bank=>bank;
        public float EffectsGain { get; set; } = 1;
        public void Configure(CombatAudioBank data){bank=data;}
        private void Awake()
        {
            voices=new AudioSource[6];for(var i=0;i<6;i++)
            {
                var node=new GameObject(i<2?"UI voice":"World voice");node.transform.SetParent(transform,false);
                var source=node.AddComponent<AudioSource>();source.playOnAwake=false;source.loop=false;source.spatialBlend=i<2?0:1;
                source.minDistance=2;source.maxDistance=30;source.rolloffMode=AudioRolloffMode.Logarithmic;voices[i]=source;
            }
        }
        public bool Play(FeedbackSound sound,Vector3 point,bool ui=false)
        {
            var clip=bank==null?null:bank.Find(sound);if(clip==null||voices==null)return false;
            var index=ui?uiIndex++%2:2+worldIndex++%4;var source=voices[index];source.Stop();source.transform.position=point;
            source.clip=clip;source.volume=(ui?bank.InterfaceVolume:bank.WorldVolume)*Mathf.Clamp01(EffectsGain);source.Play();PlayedCount++;return true;
        }
        private void OnDisable(){if(voices!=null)foreach(var voice in voices)if(voice!=null)voice.Stop();}
    }
}
