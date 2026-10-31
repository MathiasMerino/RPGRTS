using System.Collections;
using System.Collections.Generic;
using System.IO;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class ProductionInputTests
    {
        private Keyboard addedKeyboard;
        private InputSettings.UpdateMode previousMode;
        private bool manualInput;
        private InputSettings.BackgroundBehavior previousBackground;
        private bool previousRunInBackground;
        private InputSettings.EditorInputBehaviorInPlayMode previousFocus;
        [UnityTest]public IEnumerator C02ClosesByPointerAndEscapeIncludingFocusedName()
        {
            EditorSceneManager.OpenScene(LostCargoTests.Demo);yield return new EnterPlayMode();yield return null;yield return null;
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();addedKeyboard=InputSystem.AddDevice<Keyboard>();var keyboard=addedKeyboard;
            previousBackground=InputSystem.settings.backgroundBehavior;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;previousRunInBackground=Application.runInBackground;Application.runInBackground=true;
            previousFocus=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            previousMode=InputSystem.settings.updateMode;InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsManually;manualInput=true;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());ProcessInput();yield return null;
            foreach(var width in new[]{1280,1920})
            {
                hud.SelectPage(ExplorationPage.Character);yield return SceneCapture.Save("Logs/E06-Character-"+width+".png",width,width*9/16,MedievalHudTests.Layout);yield return null;
                Canvas.ForceUpdateCanvases();var close=System.Array.Find(hud.GetComponentsInChildren<Button>(),b=>b.name=="Cerrar");Assert.That(close,Is.Not.Null);
                var canvas=hud.GetComponent<Canvas>();var rect=close.GetComponent<RectTransform>();var point=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,rect.TransformPoint(rect.rect.center));var pointer=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(close));ExecuteEvents.Execute(close.gameObject,pointer,ExecuteEvents.pointerClickHandler);
                Assert.That(hud.CompactView.WindowOpen,Is.False,"Pointer X closes C02");
                hud.SelectPage(ExplorationPage.Character);var input=hud.GetComponentInChildren<InputField>();input.ActivateInputField();yield return null;Assert.That(input.isFocused,Is.True);
                var action=(InputAction)typeof(VerticalSliceHud).GetField("closeAction",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(hud);
                Assert.That(action,Is.Not.Null,"close action initialized");Assert.That(action.enabled,Is.True);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape));ProcessInput();Assert.That(hud.CompactView.WindowOpen,Is.False,"ESC immediate callback; key="+keyboard.escapeKey.isPressed+" performed="+action.WasPerformedThisFrame());yield return null;yield return null;
                Assert.That(hud.CompactView.WindowOpen,Is.False,"Escape with focused name closes C02");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());ProcessInput();yield return null;
                hud.SelectPage(ExplorationPage.Inventory);InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape));ProcessInput();yield return null;yield return null;
                Assert.That(hud.CompactView.WindowOpen,Is.False,"Escape closes inventory");InputSystem.QueueStateEvent(keyboard,new KeyboardState());ProcessInput();yield return null;
            }
            File.WriteAllText("Logs/E06-window-input.txt","PASS: C02 X uses top EventSystem raycast/pointer callback; ESC uses queued KeyboardState, including focused name/inventory. Automatic reproduction only; F2-07 human confirmation still pending.");
            var d=Object.FindAnyObjectByType<PrototypeDirector>();Assert.That(d.Hero.Warp(new Vector3(-3.4f,0,-5)),Is.True);Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(d.Hero,d.Session),Is.True);hud.SelectPage(ExplorationPage.Inventory);Assert.That(d.Execute("sword")&&d.Execute("armour"),Is.True);
            yield return SceneCapture.Save("Logs/E06-Inventory-1920.png",1920,1080,MedievalHudTests.Layout);
            InputSystem.settings.backgroundBehavior=previousBackground;Application.runInBackground=previousRunInBackground;InputSystem.settings.updateMode=previousMode;InputSystem.settings.editorInputBehaviorInPlayMode=previousFocus;manualInput=false;LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        // Batch editor has no focused Game view. Public Update selects Editor and suppresses gameplay actions.
        // Invoke the installed Input System's manual update explicitly, only in this test harness.
        private static void ProcessInput()
        {typeof(InputSystem).GetMethod("Update",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic,null,new[]{typeof(InputUpdateType)},null).Invoke(null,new object[]{InputUpdateType.Manual});}
        [UnityTearDown]public IEnumerator End(){if(manualInput){InputSystem.settings.backgroundBehavior=previousBackground;Application.runInBackground=previousRunInBackground;InputSystem.settings.updateMode=previousMode;InputSystem.settings.editorInputBehaviorInPlayMode=previousFocus;manualInput=false;}if(addedKeyboard!=null){InputSystem.RemoveDevice(addedKeyboard);addedKeyboard=null;}if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
