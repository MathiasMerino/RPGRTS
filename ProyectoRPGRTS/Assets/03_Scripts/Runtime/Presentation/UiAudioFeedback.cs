using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Aethernia
{
    public sealed class UiAudioFeedback : MonoBehaviour
    {
        [SerializeField]private CombatAudioPlayer audioPlayer;
        [SerializeField]private VerticalSliceHud hud;
        [SerializeField]private LostCargoView quest;
        [SerializeField]private GameSession session;
        private readonly List<Button> buttons=new List<Button>();
        private UnityAction click;
        private bool known,wasOpen;
        private PrototypeState previousState;
        private CargoQuestStage previousStage;
        public void Configure(CombatAudioPlayer sound,VerticalSliceHud view,LostCargoView cargo,GameSession game)
        {audioPlayer=sound;hud=view;quest=cargo;session=game;}
        private void OnEnable()
        {
            if(hud==null)return;
            click=()=>{if(audioPlayer!=null)audioPlayer.Play(FeedbackSound.Select,Vector3.zero,true);};
            buttons.Clear();
            foreach(var b in hud.GetComponentsInChildren<Button>(true)){buttons.Add(b);b.onClick.AddListener(click);}
        }
        private void LateUpdate()
        {
            if(hud==null||audioPlayer==null||session==null)return;
            var open=hud.CompactView.WindowOpen||hud.CompactView.HelpOpen||(quest!=null&&quest.DialogueOpen);
            if(known&&open!=wasOpen)audioPlayer.Play(open?FeedbackSound.PanelOpen:FeedbackSound.PanelClose,Vector3.zero,true);
            wasOpen=open;known=true;var stage=LostCargoQuest.Stage(session.State);
            if(previousState==session.State&&stage==CargoQuestStage.Completed&&previousStage!=stage)audioPlayer.Play(FeedbackSound.Quest,Vector3.zero,true);
            previousState=session.State;previousStage=stage;
        }
        private void OnDisable(){foreach(var b in buttons)if(b!=null&&click!=null)b.onClick.RemoveListener(click);buttons.Clear();known=false;}
    }
}
