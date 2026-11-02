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
    public sealed class DemoCollisionTests
    {
        [UnityTest] public IEnumerator WasdStopsAtPhysicalWallAndOptionsRemainReadable()=>Run();
        private static IEnumerator Run()
        {
            EditorSceneManager.OpenScene(FirstDemoBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;Directory.CreateDirectory("Logs/DemoQA/Captures");
            flow.SlotPath="Logs/DemoQA/collision-"+System.Guid.NewGuid().ToString("N")+".json";
            Assert.That(flow.StartNew("Colisión aislada","warrior","human"),Is.True);flow.CompleteIntro();d.Encounter.Clear();Assert.That(d.Execute("explore"),Is.True); // Fixture de colisión, no recorrido narrativo.
            Assert.That(flow.GetComponent<DemoHeroMotor>().CanControl,Is.True);
            Assert.That(d.Hero.Warp(new Vector3(-4,0,-11)),Is.True);yield return null;d.Hero.Stop();flow.CameraRig.FocusOn(d.Hero.transform.position);
            var origin=d.Hero.transform.position;var direction=DemoHeroMotor.CameraRelative(Vector2.up,flow.CameraRig.transform);
            var wall=GameObject.CreatePrimitive(PrimitiveType.Cube);wall.name="QA temporary physical wall";wall.transform.position=origin+direction*.95f+Vector3.up;
            wall.transform.rotation=Quaternion.LookRotation(direction);wall.transform.localScale=new Vector3(4,2,.2f);Physics.SyncTransforms();
            var keyboard=InputSystem.AddDevice<Keyboard>();var routing=InputSystem.settings.editorInputBehaviorInPlayMode;var background=InputSystem.settings.backgroundBehavior;
            try
            {
                InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
                InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;EventSystem.current.SetSelectedGameObject(null);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));yield return TacticalCombatIntegrationTests.Wait(.7f);
                var along=Vector3.Dot(d.Hero.transform.position-origin,direction);Assert.That(along,Is.GreaterThan(.1f));Assert.That(along,Is.LessThan(.58f),"El cuerpo no cruza el muro físico, aun sin NavMeshObstacle");
                var stopped=d.Hero.transform.position;yield return TacticalCombatIntegrationTests.Wait(.2f);Assert.That(Vector3.Distance(stopped,d.Hero.transform.position),Is.LessThan(.035f));
                Object.Destroy(wall);yield return null;Physics.SyncTransforms();yield return TacticalCombatIntegrationTests.Wait(.35f);Assert.That(Vector3.Distance(stopped,d.Hero.transform.position),Is.GreaterThan(.5f),"Se puede continuar tras retirar el obstáculo");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;
                flow.Pause();flow.Front.ShowOptions();yield return TacticalCombatIntegrationTests.Wait(.3f);
                Assert.That(flow.ExistingHud.gameObject.activeSelf,Is.False,"HUD oculto durante opciones de pausa");
                var key="Aethernia.Tests.CollisionOptions."+System.Guid.NewGuid().ToString("N");flow.Front.Preferences.StorageKey=key;
                var slider=System.Array.Find(flow.Front.GetComponentsInChildren<Slider>(),x=>x.name=="Master");slider.value=.3f;
                Assert.That(flow.Front.Preferences.Master,Is.EqualTo(.3f).Within(.001));Assert.That(AudioListener.volume,Is.EqualTo(.3f).Within(.001));
                Assert.That(DemoPreferences.Load(key).Master,Is.EqualTo(.3f).Within(.001));PlayerPrefs.DeleteKey(key);
                foreach(var width in new[]{1280,1920,2560,3840})yield return DemoCapture.Save("Logs/DemoQA/Captures/Options-"+width+".png",width,width*9/16,flow.Front);
                Assert.That(flow.Front.Controls["return-title"].interactable,Is.True);flow.Resume();
            }
            finally
            {if(wall!=null)Object.Destroy(wall);InputSystem.RemoveDevice(keyboard);InputSystem.settings.editorInputBehaviorInPlayMode=routing;InputSystem.settings.backgroundBehavior=background;}
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown] public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
