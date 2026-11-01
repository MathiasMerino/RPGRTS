using System.Collections.Generic;
using UnityEngine;

namespace Aethernia
{
    // Consume resultados aceptados. La cola es cosmética: jamás retarda turnos ni aplica daño.
    public sealed class CharacterCombatAnimationPresenter : MonoBehaviour
    {
        [SerializeField] private TurnEncounter encounter;
        [SerializeField] private CharacterVisualPresenter visuals;
        private readonly Queue<EncounterPresentation> pending = new Queue<EncounterPresentation>();
        private HumanoidAnimationDriver driver;
        private float nextAction;
        private bool guarding;
        public int PlayedActions { get; private set; }
        public void Configure(TurnEncounter combat, CharacterVisualPresenter presenter)
        {
            var active=isActiveAndEnabled;if(active)Unsubscribe();encounter=combat;visuals=presenter;if(active)Subscribe();
        }
        private void OnEnable() { Subscribe(); }
        private void OnDisable() { Unsubscribe(); ResetView(); }
        private void Subscribe() { if(encounter!=null)encounter.Presentation+=Receive; }
        private void Unsubscribe() { if(encounter!=null)encounter.Presentation-=Receive; }
        private void ResetView() { pending.Clear();guarding=false;nextAction=0;if(driver!=null)driver.SetGuarding(false); }
        private void Receive(EncounterPresentation cue)
        {
            if(cue.Cue==EncounterCue.Reset){ResetView();return;}
            if(cue.Cue==EncounterCue.Completed)
            {
                // La misión limpia el encuentro en el mismo frame del golpe ganador.
                // Conservar solo el último golpe pendiente evita perderlo o reproducir una cola atrasada.
                guarding=false;var found=false;var strike=default(EncounterPresentation);
                foreach(var action in pending)if(action.Cue==EncounterCue.Attack){strike=action;found=true;}
                pending.Clear();if(found){pending.Enqueue(strike);nextAction=0;}return;
            }
            if(cue.Cue==EncounterCue.Guard){guarding=true;return;}
            if(cue.Cue==EncounterCue.GuardEnded){guarding=false;return;}
            // Un modelo ausente no retiene eventos para otro personaje futuro.
            if(visuals==null||visuals.Model==null||visuals.Model.GetComponent<HumanoidAnimationDriver>()==null)return;
            if(cue.Cue==EncounterCue.Defeat){pending.Clear();nextAction=0;}
            // Capacidad acotada para una ráfaga de comandos; solo presentación.
            if(pending.Count<8)pending.Enqueue(cue);
        }
        private void LateUpdate()
        {
            var current=visuals!=null&&visuals.Model!=null?visuals.Model.GetComponent<HumanoidAnimationDriver>():null;
            if(current!=driver){driver=current;pending.Clear();nextAction=0;guarding=false;}
            if(driver==null||!driver.CanAnimate)return;
            driver.SetGuarding(guarding&&encounter!=null&&encounter.Battle!=null&&!encounter.Battle.Finished);
            if(driver.IsMoving){driver.transform.localRotation=Quaternion.identity;return;}
            if(Time.time<nextAction||pending.Count==0)return;
            var cue=pending.Peek();var motion=cue.Cue==EncounterCue.Attack?CharacterMotion.Attack:cue.Cue==EncounterCue.Hit?CharacterMotion.Hit:CharacterMotion.Defeat;
            if(!driver.Profile.HasClip(motion)){pending.Dequeue();return;}
            if(!driver.TryPlay(motion))return;
            var delta=cue.Target-driver.transform.position;delta.y=0;
            if(delta.sqrMagnitude>.01f)driver.transform.rotation=Quaternion.LookRotation(delta);
            pending.Dequeue();nextAction=Time.time+driver.Profile.Clip(motion).length+.1f;PlayedActions++;
        }
    }
}
