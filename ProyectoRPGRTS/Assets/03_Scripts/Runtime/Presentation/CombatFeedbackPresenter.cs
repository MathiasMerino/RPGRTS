using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aethernia
{
    public sealed class CombatFeedbackPresenter : MonoBehaviour
    {
        [SerializeField]private TurnEncounter encounter;
        [SerializeField]private CharacterVisualPresenter hero;
        [SerializeField]private CharacterCombatAnimationPresenter animationView;
        [SerializeField]private CombatAudioPlayer audioPlayer;
        [SerializeField]private GameObject impactPrefab;
        [SerializeField]private Material sharedMaterial;
        [SerializeField]private Font font;
        private sealed class Pending {public CombatFeedback Data;public float Due,Deadline;public bool Waiting;}
        private readonly List<Pending> pending=new List<Pending>(8);
        private readonly List<GameObject> effects=new List<GameObject>(8);
        public event Action<CombatFeedback> ImpactPresented;
        public int PresentedCount { get; private set; }
        public int ActiveEffects { get {effects.RemoveAll(x=>x==null);return effects.Count;} }
        public int PendingCount=>pending.Count;
        public Material SharedEffectMaterial=>sharedMaterial;
        public void PresentRealtime(CombatFeedback result){if(!result.Result.Accepted)return;if(result.ClassId=="mage"||result.Result.Kind==CombatActionKind.Guard)Launch(result);Present(result);}
        public void Configure(TurnEncounter combat,CharacterVisualPresenter actor,CharacterCombatAnimationPresenter animator,
            CombatAudioPlayer sound,GameObject impact,Material material,Font textFont)
        {encounter=combat;hero=actor;animationView=animator;audioPlayer=sound;impactPrefab=impact;sharedMaterial=material;font=textFont;}
        private void OnEnable()
        {
            if(encounter!=null){encounter.ActionResolved+=Receive;encounter.Presentation+=Cue;encounter.Resolved+=Finish;}
            if(animationView!=null)animationView.MotionStarted+=Started;
        }
        private void OnDisable()
        {
            if(encounter!=null){encounter.ActionResolved-=Receive;encounter.Presentation-=Cue;encounter.Resolved-=Finish;}
            if(animationView!=null)animationView.MotionStarted-=Started;Clear();
        }
        private void Clear(){pending.Clear();foreach(var effect in effects)if(effect!=null)Destroy(effect);effects.Clear();}
        private void Cue(EncounterPresentation cue)
        {
            if(cue.Cue==EncounterCue.Reset)Clear();
            if(cue.Cue==EncounterCue.Completed)
            {Pending last=null;foreach(var item in pending)if(item.Data.Result.Actor==CombatSide.Player&&item.Data.Result.Kind==CombatActionKind.Attack)last=item;pending.Clear();if(last!=null)pending.Add(last);}
        }
        private void Finish(bool won)
        {
            if(audioPlayer!=null)audioPlayer.Play(won?FeedbackSound.Victory:FeedbackSound.Defeat,Vector3.zero,true);
            if(hero!=null){var at=hero.transform.position+Vector3.up;Burst(at,won?new Color(.70f,.58f,.28f):new Color(.30f,.27f,.24f));Float(at,won?"Victoria":"Derrota",new Color(.90f,.77f,.49f));}
        }
        private void Receive(CombatFeedback result)
        {
            if(!result.Result.Accepted)return;
            var driver=hero!=null&&hero.Model!=null?hero.Model.GetComponent<HumanoidAnimationDriver>():null;
            var wait=result.Result.Actor==CombatSide.Player&&result.Result.Kind==CombatActionKind.Attack&&driver!=null&&driver.CanAnimate&&driver.Profile.HasClip(CharacterMotion.Attack);
            if(pending.Count==8)pending.RemoveAt(0);
            pending.Add(new Pending{Data=result,Due=Time.time,Waiting=wait,Deadline=Time.time+1.6f});
            if(!wait)Launch(result);
        }
        private void Started(EncounterPresentation cue)
        {
            if(cue.Cue!=EncounterCue.Attack||!cue.Feedback.Result.Accepted)return;
            foreach(var item in pending)
            {
                if(item.Data.EncounterId!=cue.Feedback.EncounterId||item.Data.Result.Sequence!=cue.Feedback.Result.Sequence)continue;
                var driver=hero.Model.GetComponent<HumanoidAnimationDriver>();item.Waiting=false;item.Due=Time.time+driver.Profile.Attack.length*.45f;Launch(item.Data);break;
            }
        }
        private void Launch(CombatFeedback data)
        {
            if(audioPlayer==null)return;
            if(data.Result.Kind==CombatActionKind.Attack&&data.Result.Actor==CombatSide.Player)
            {audioPlayer.Play(data.ClassId=="mage"?FeedbackSound.Arcane:FeedbackSound.Confirm,data.Source,data.ClassId!="mage");if(data.ClassId=="mage")Projectile(data);}
            else if(data.Result.Kind==CombatActionKind.Guard)audioPlayer.Play(FeedbackSound.Confirm,Vector3.zero,true);
        }
        private void Update()
        {
            for(var i=pending.Count-1;i>=0;i--)
            {
                var item=pending[i];if(item.Waiting&&Time.time<item.Deadline)continue;
                if(item.Waiting){item.Waiting=false;Launch(item.Data);}if(Time.time<item.Due)continue;
                pending.RemoveAt(i);Present(item.Data);
            }
        }
        private void Present(CombatFeedback data)
        {
            var r=data.Result;var point=data.Target+Vector3.up;
            if(r.Kind==CombatActionKind.Attack)
            {
                if(r.Hit)
                {
                    Burst(point,r.Guarded?new Color(.45f,.60f,.70f):data.ClassId=="mage"&&r.Actor==CombatSide.Player?new Color(.42f,.43f,.70f):new Color(.72f,.53f,.29f));
                    var sound=r.Guarded?FeedbackSound.Block:r.Actor==CombatSide.Enemy||data.ClassId=="warrior"?FeedbackSound.Sword:data.ClassId=="monk"?FeedbackSound.Punch:FeedbackSound.Hurt;
                    audioPlayer?.Play(sound,point);if(r.Actor==CombatSide.Enemy)audioPlayer?.Play(FeedbackSound.Hurt,point);Float(point,"−"+r.Damage+(r.Guarded?" · guardia":""),new Color(.95f,.70f,.45f));
                }
                else{audioPlayer?.Play(FeedbackSound.Miss,point);Float(point,r.Actor==CombatSide.Enemy?"Rival falla":"Fallo",new Color(.85f,.82f,.72f));}
            }
            else if(r.Kind==CombatActionKind.Guard){Burst(data.Source+Vector3.up,new Color(.45f,.58f,.65f));Float(data.Source+Vector3.up,"Guardia",new Color(.70f,.80f,.88f));}
            else if(r.Kind==CombatActionKind.Recover){Burst(data.Source+Vector3.up,new Color(.45f,.65f,.45f));audioPlayer?.Play(FeedbackSound.Recover,data.Source);Float(data.Source+Vector3.up,"+"+r.Healing,new Color(.65f,.85f,.65f));}
            PresentedCount++;ImpactPresented?.Invoke(data);
        }
        private void Track(GameObject effect)
        {
            effects.RemoveAll(x=>x==null);if(effects.Count>=8){Destroy(effects[0]);effects.RemoveAt(0);}effects.Add(effect);
        }
        private void Burst(Vector3 point,Color color)
        {
            if(impactPrefab==null)return;var effect=Instantiate(impactPrefab,point,Quaternion.identity);var ps=effect.GetComponent<ParticleSystem>();var main=ps.main;main.startColor=color;ps.Play();effect.AddComponent<CombatTransient>().Configure(.8f);Track(effect);
        }
        private void Float(Vector3 point,string text,Color color)
        {
            var effect=new GameObject("Combat result · "+text);effect.transform.position=point+Vector3.up*.8f;var label=effect.AddComponent<TextMesh>();label.text=text;label.font=font;label.fontSize=48;label.characterSize=.10f;label.anchor=TextAnchor.MiddleCenter;label.color=color;
            if(font!=null)effect.GetComponent<MeshRenderer>().sharedMaterial=font.material;
            effect.AddComponent<CombatTransient>().Configure(1.1f,label,Camera.main!=null?Camera.main.transform:null);Track(effect);
        }
        private void Projectile(CombatFeedback data)
        {
            var effect=new GameObject("Arcane trace · presentation only");effect.transform.position=data.Source+Vector3.up*1.1f;var line=effect.AddComponent<LineRenderer>();line.sharedMaterial=sharedMaterial;line.useWorldSpace=false;line.positionCount=2;line.SetPosition(0,Vector3.zero);line.SetPosition(1,new Vector3(0,0,-.18f));line.startWidth=.05f;line.endWidth=.012f;line.startColor=new Color(.46f,.50f,.80f);line.endColor=new Color(.22f,.22f,.40f,0);
            effect.transform.rotation=Quaternion.LookRotation(data.Target-data.Source);var life=.3f;
            if(hero!=null&&hero.Model!=null){var driver=hero.Model.GetComponent<HumanoidAnimationDriver>();if(driver!=null&&driver.Profile!=null&&driver.Profile.HasClip(CharacterMotion.Attack))life=driver.Profile.Attack.length*.45f;}
            effect.AddComponent<CombatTransient>().Projectile(data.Target+Vector3.up,life);Track(effect);
        }
    }
}
