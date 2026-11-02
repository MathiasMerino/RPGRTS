using System.Collections;
using System.IO;
using Aethernia.Editor.Characters;
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
    public sealed class DemoAdventureTests
    {
        [Test] public void CameraRelativeDiagonalIsNormalized()
        {
            var camera=new GameObject("Test camera frame");camera.transform.rotation=Quaternion.Euler(35.26439f,45,0);
            try{var move=DemoHeroMotor.CameraRelative(Vector2.one,camera.transform);Assert.That(move.magnitude,Is.EqualTo(1).Within(.0001));Assert.That(move.y,Is.EqualTo(0));Assert.That(DemoHeroMotor.CameraRelative(Vector2.zero,camera.transform),Is.EqualTo(Vector3.zero));}
            finally{Object.DestroyImmediate(camera);}
        }
        [UnityTest] public IEnumerator ThreeClassesFinishTutorialAndWalkWithRealInput()=>Adventure();
        private static IEnumerator Adventure()
        {
            EditorSceneManager.OpenScene(FirstDemoBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;flow.SlotPath="Logs/E11/adventure-"+System.Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;flow.EncounterSeed=42;
            Directory.CreateDirectory("Logs/E11/Captures");
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                flow.ShowMain();Assert.That(flow.StartNew("Viajero",id,id=="mage"?"elf":"human"),Is.True);flow.CompleteIntro();yield return null;
                Assert.That(flow.Session.Mode,Is.EqualTo(PrototypeMode.TurnCombat));d.Encounter.Approach();yield return Arrive(d.Hero);
                for(var turn=0;turn<30&&flow.Session.State.Demo.Chapter==DemoChapter.FirstCombat;turn++)
                {
                    var b=d.Encounter.Battle;Assert.That(b,Is.Not.Null);
                    if(b.HeroHealth<8&&!b.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                    if(flow.Session.State.Demo.Chapter!=DemoChapter.FirstCombat)break;
                    Assert.That(d.Execute("end"),Is.True);var limit=Time.realtimeSinceStartup+5;
                    while(d.Encounter.Battle!=null&&d.Encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<limit)yield return null;
                }
                Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.RoadToRts),id+": victoria legal");Assert.That(flow.Session.State.Health,Is.GreaterThan(0));Assert.That(flow.IsPlaying&&!flow.Session.Paused,Is.True);
                Assert.That(d.Hero.Warp(new Vector3(-4,0,-11)),Is.True);yield return null;d.Hero.Stop();flow.CameraRig.FocusOn(d.Hero.transform.position);
                var motor=flow.GetComponent<DemoHeroMotor>();var keyboard=InputSystem.AddDevice<Keyboard>();var old=d.Hero.transform.position;
                var editorRouting=InputSystem.settings.editorInputBehaviorInPlayMode;var background=InputSystem.settings.backgroundBehavior;
                try
                {
                    InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
                    InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
                    EventSystem.current.SetSelectedGameObject(null);InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));InputSystem.Update();yield return TacticalCombatIntegrationTests.Wait(.35f);
                    var collisions=Physics.CapsuleCastAll(d.Hero.Agent.nextPosition+Vector3.up*.31f,d.Hero.Agent.nextPosition+Vector3.up*1.49f,.28f,DemoHeroMotor.CameraRelative(Vector2.up,flow.CameraRig.transform),.2f);
                    var diagnostic=id+": WASD real; ticks="+motor.UpdateTicks+"; last="+motor.LastReadInput+"; blocked="+motor.LastBlocked+"; active="+motor.isActiveAndEnabled+"; can="+motor.CanControl+"; key="+keyboard.wKey.isPressed+"; current="+(Keyboard.current==keyboard)+"; typing="+UiInputGuard.IsTyping+"; window="+flow.ExistingHud.CompactView.WindowOpen+"; help="+flow.ExistingHud.CompactView.HelpOpen+"; mesh="+d.Hero.Agent.isOnNavMesh+"; pos="+d.Hero.transform.position+"; timeScale="+Time.timeScale+"; delta="+Time.deltaTime+"; direct="+d.Hero.DirectControl+"; velocity="+d.Hero.PresentationVelocity;
                    foreach(var hit in collisions)diagnostic+="; hit="+hit.collider.name+"/"+hit.distance+"/top="+hit.collider.bounds.max.y+"/self="+hit.transform.IsChildOf(d.Hero.transform)+"/root="+hit.transform.root.name;
                    Assert.That(Vector3.Distance(old,d.Hero.transform.position),Is.GreaterThan(.3f),diagnostic);
                    var driver=d.Hero.GetComponent<CharacterVisualPresenter>().Model.GetComponent<HumanoidAnimationDriver>();Assert.That(driver.ObservedSpeed,Is.GreaterThan(1));
                    Assert.That(Vector3.Distance(flow.CameraRig.Focus,d.Hero.transform.position),Is.LessThan(.25f));
                    InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();yield return null;yield return null;Assert.That(d.Hero.IsMoving,Is.False);
                    var held=d.Hero.transform.position;flow.Pause();Assert.That(motor.ApplyInput(Vector2.up,.05f,false),Is.False);yield return TacticalCombatIntegrationTests.Wait(.1f);Assert.That(d.Hero.transform.position,Is.EqualTo(held));flow.Resume();
                    flow.ExistingHud.SelectPage(ExplorationPage.Character);yield return null;yield return null;var input=flow.ExistingHud.GetComponentInChildren<InputField>();Assert.That(input,Is.Not.Null);input.ActivateInputField();yield return null;
                    Assert.That(UiInputGuard.IsTyping,Is.True);held=d.Hero.transform.position;InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));InputSystem.Update();yield return TacticalCombatIntegrationTests.Wait(.15f);Assert.That(d.Hero.transform.position,Is.EqualTo(held));
                    InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();input.DeactivateInputField();flow.ExistingHud.ClosePanels();EventSystem.current.SetSelectedGameObject(null);
                    flow.Session.SetMode(PrototypeMode.Rts);Assert.That(motor.ApplyInput(Vector2.up,.05f,false),Is.False);flow.Session.SetMode(PrototypeMode.Exploration);
                }
                finally{InputSystem.RemoveDevice(keyboard);InputSystem.settings.editorInputBehaviorInPlayMode=editorRouting;InputSystem.settings.backgroundBehavior=background;}
                Assert.That(d.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);yield return TacticalCombatIntegrationTests.Wait(.3f);yield return DemoCapture.Save("Logs/E11/Captures/"+id+"-Exploration.png",1280,720);
                flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.State.ClassId,Is.EqualTo(id));Assert.That(flow.Session.State.Demo.IntroSeen,Is.True);Assert.That(flow.Front.Screen,Is.EqualTo(DemoScreen.None));
            }
            File.WriteAllText("Logs/E11/adventure.txt","PASS three classes: real tutorial dice/staged turns/victory/auto exploration, InputSystem WASD/actual displacement/Generic locomotion/camera follow, pause/typing/RTS mode gates and v1 save/continue without replaying intro.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator Arrive(ExplorationUnit unit){var end=Time.realtimeSinceStartup+15;yield return null;while(unit.IsMoving&&Time.realtimeSinceStartup<end)yield return null;Assert.That(unit.IsMoving,Is.False);}
        [UnityTearDown] public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
