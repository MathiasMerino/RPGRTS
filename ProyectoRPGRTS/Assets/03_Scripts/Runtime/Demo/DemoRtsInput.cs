using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aethernia
{
    public sealed class DemoRtsInput : MonoBehaviour
    {
        public DemoFlow Flow;
        private Vector2 start;
        private PlayerCommands pointer;
        public bool Dragging { get; private set; }
        public Rect Selection { get; private set; }
        private readonly List<ExplorationUnit>[] groups={new List<ExplorationUnit>(),new List<ExplorationUnit>(),new List<ExplorationUnit>()};
        private void Start(){pointer=Object.FindAnyObjectByType<PlayerCommands>();}
        private void Update()
        {
            var rts=Flow.Director.Rts;var active=rts.Active&&rts.IncludesHero&&!Flow.Session.Paused;if(pointer!=null)pointer.ExternalRtsPointer=rts.IncludesHero;
            if(!active){Dragging=false;return;}
            var mouse=Mouse.current;var key=Keyboard.current;var additive=key!=null&&(key.leftShiftKey.isPressed||key.rightShiftKey.isPressed);
            if(key!=null&&!UiInputGuard.IsTyping)
            {
                var number=key.digit1Key.wasPressedThisFrame?0:key.digit2Key.wasPressedThisFrame?1:key.digit3Key.wasPressedThisFrame?2:-1;
                if(number>=0){if(key.leftCtrlKey.isPressed||key.rightCtrlKey.isPressed)rts.RememberGroup(groups[number]);else rts.SelectGroup(groups[number],additive);}
            }
            if(mouse==null)return;var at=mouse.position.ReadValue();var blocked=UiInputGuard.HasUiAt(at)||UiInputGuard.IsTyping;
            if(mouse.leftButton.wasPressedThisFrame&&!blocked){start=at;Dragging=true;}
            if(Dragging)Selection=Rect.MinMaxRect(Mathf.Min(start.x,at.x),Mathf.Min(start.y,at.y),Mathf.Max(start.x,at.x),Mathf.Max(start.y,at.y));
            if(mouse.leftButton.wasReleasedThisFrame&&Dragging)
            {
                Dragging=false;if(blocked)return;
                if((at-start).magnitude>8*Screen.width/1280f)rts.SelectArea(Camera.main,Selection,additive);
                else if(Physics.Raycast(Camera.main.ScreenPointToRay(at),out var hit,150))rts.SelectUnit(hit.collider,additive);
            }
            if(mouse.rightButton.wasPressedThisFrame&&!blocked&&Physics.Raycast(Camera.main.ScreenPointToRay(at),out var target,150))rts.Command(target.point,target.collider,false);
        }
    }
}
