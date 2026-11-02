using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using Object=UnityEngine.Object;
namespace Aethernia.Tests.Editor
{
    public sealed class AlphaExperienceTests
    {
        [UnityTest]public IEnumerator AlphaWindowsCloseByEscapeAndPointer()=>WindowInput();
        private static IEnumerator WindowInput()
        {
            EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E17/input-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",flow.Session.Rules,out var state),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;
            var keyboard=InputSystem.AddDevice<Keyboard>();var oldMode=InputSystem.settings.updateMode;var oldFocus=InputSystem.settings.editorInputBehaviorInPlayMode;var oldBackground=InputSystem.settings.backgroundBehavior;var oldRun=Application.runInBackground;
            try
            {
                InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsManually;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;Application.runInBackground=true;
                foreach(var page in new[]{ExplorationPage.Character,ExplorationPage.Inventory,ExplorationPage.Session})
                {
                    InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();flow.ExistingHud.SelectPage(page);yield return null;
                    var close=flow.ExistingHud.GetComponentsInChildren<Button>().Single(b=>b.name=="Cerrar");PointerClose(close);Assert.That(flow.ExistingHud.CompactView.WindowOpen,Is.False);
                    flow.ExistingHud.SelectPage(page);InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape));InputSystem.Update();Assert.That(flow.ExistingHud.CompactView.WindowOpen,Is.False);yield return null;
                }
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();var expedition=Object.FindAnyObjectByType<ExpeditionPanel>();expedition.Open();yield return null;PointerClose(expedition.Controls["close"]);Assert.That(flow.Session.WorldUiOpen,Is.False);
                expedition.Open();InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape));InputSystem.Update();yield return null;yield return null;Assert.That(flow.Session.WorldUiOpen,Is.False,"Escape closes the expedition window");
                File.WriteAllText("Logs/E17/WINDOW_INPUT.txt","PASS automatic pointer raycast X and queued Escape: character, inventory, session, expedition. Human evaluation remains pending.");
            }
            finally{InputSystem.RemoveDevice(keyboard);InputSystem.settings.updateMode=oldMode;InputSystem.settings.editorInputBehaviorInPlayMode=oldFocus;InputSystem.settings.backgroundBehavior=oldBackground;Application.runInBackground=oldRun;}
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static void PointerClose(Button button)
        {
            Canvas.ForceUpdateCanvases();var canvas=button.GetComponentInParent<Canvas>();var rect=button.GetComponent<RectTransform>();var point=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,rect.TransformPoint(rect.rect.center));var pointer=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerClickHandler);
        }
        [UnityTest]public IEnumerator ContextAndSelectionRemainReadable()=>Contexts();
        private static IEnumerator Contexts()
        {
            EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E17/experience-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",flow.Session.Rules,out var state),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;
            Directory.CreateDirectory("Logs/E17/Captures/Experience");var transition=Object.FindAnyObjectByType<ModeTransitionView>();Assert.That(transition,Is.Not.Null);Assert.That(transition.Announcement,Does.Contain("CAMINO"));yield return TacticalCombatIntegrationTests.Wait(.25f);Assert.That(transition.VeilAlpha,Is.Zero);Assert.That(flow.Session.State.Health,Is.EqualTo(state.Health));var camp=Object.FindAnyObjectByType<DemoSettlementPanel>();camp.Open();yield return null;yield return null;
            var panel=flow.ExistingHud.transform.Find("Settlement management");Assert.That(panel.gameObject.activeInHierarchy,Is.True);flow.ExistingHud.CompactView.ToggleHelp();flow.Session.Say("Ayuda contextual");yield return null;yield return null;
            var overlaps=panel.gameObject.activeInHierarchy;yield return DemoCapture.Save("Logs/E17/Captures/Experience/Construction-help.png",1280,720);flow.ExistingHud.ClosePanels();yield return null;yield return null;Assert.That(panel.gameObject.activeInHierarchy,Is.True,"Closing help restores the same construction context");Assert.That(transition.Announcement,Does.Contain("CAMPAMENTO"));flow.Director.Execute("explore");
            var expedition=Object.FindAnyObjectByType<ExpeditionPanel>();expedition.Open();yield return null;Assert.That(flow.Session.WorldUiOpen,Is.True);expedition.Controls["close"].onClick.Invoke();yield return null;Assert.That(flow.Session.WorldUiOpen,Is.False);expedition.Open();flow.ExistingHud.SelectPage(ExplorationPage.Inventory);yield return null;yield return null;Assert.That(flow.Session.WorldUiOpen,Is.False,"Group closes when inventory takes focus");Assert.That(flow.ExistingHud.CompactView.Window.activeInHierarchy,Is.True);flow.ExistingHud.ClosePanels();
            Assert.That(flow.BeginPatrol(),Is.True);yield return null;Assert.That(transition.Announcement,Does.Contain("BATALLA"));var units=new List<RtsUnitStatus>();flow.Director.Rts.CopyStatus(units);var selected=units.Where(s=>s.Team==0&&!s.Hero).ToArray();var disabled=0;
            foreach(var s in selected){var so=new SerializedObject(s.Unit);var indicator=(GameObject)so.FindProperty("selectionIndicator").objectReferenceValue;Assert.That(indicator,Is.Not.Null);if(!indicator.activeInHierarchy||indicator.GetComponentsInChildren<Renderer>().Any(r=>!r.enabled))disabled++;}
            Assert.That(flow.ExistingHud.transform.Find("Hero RTS commands").GetComponent<RectTransform>().anchoredPosition.x,Is.EqualTo(920));yield return DemoCapture.Save("Logs/E17/Captures/Experience/Rts-selection.png",1920,1080);File.WriteAllText("Logs/E17/EXPERIENCE_CONTEXTS.txt","constructionHelpOverlap="+overlaps+"; selectedAllyIndicatorsDisabled="+disabled+"/"+selected.Length);
            Assert.That(overlaps,Is.False,"Construction must yield to help");Assert.That(disabled,Is.Zero,"Selected ally rings must stay visible after visual replacement");flow.ExistingHud.CompactView.ToggleHelp();flow.Session.Say("Ayuda RTS");yield return null;yield return null;Assert.That(flow.ExistingHud.transform.Find("Hero RTS commands").gameObject.activeInHierarchy,Is.False);flow.ExistingHud.ClosePanels();flow.Director.Rts.Clear();LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
