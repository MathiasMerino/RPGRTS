using System;
using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
namespace Aethernia.Tests.Editor
{
    public sealed class AlphaPresentationTests
    {
        [Test]public void NewSceneRetainsPhysicsAndPublicArtOnly()
        {
            EditorSceneManager.OpenScene(VisualPolishBuilder.ScenePath);var before=VisualPolishTests.PhysicsSignature();EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);Assert.That(VisualPolishTests.PhysicsSignature(),Is.EqualTo(before));
            foreach(var dependency in AssetDatabase.GetDependencies(AlphaSceneBuilder.ScenePath,true))Assert.That(dependency.Contains("MonkMeshy")||dependency.Contains("LocalOnly"),Is.False,dependency);
            var flow=UnityEngine.Object.FindAnyObjectByType<DemoFlow>();Assert.That(flow.Front.AlphaLayout,Is.True);
            foreach(var c in "ÁÉÍÓÚÜÑáéíóúüñ¿¡")Assert.That(flow.Front.Font.HasCharacter(c,true,true),Is.True,"Spanish glyph: "+c);
        }
        [UnityTest,Timeout(600000)]public IEnumerator FourResolutionsPreviewAndOptions()=>Presentation();
        private static IEnumerator Presentation()
        {
            EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=UnityEngine.Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E17/ui-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;flow.Front.Preferences.StorageKey="Aethernia.Tests.E17."+Guid.NewGuid().ToString("N");flow.ShowMain();yield return TacticalCombatIntegrationTests.Wait(.25f);Directory.CreateDirectory("Logs/E17/Captures/UI");
            foreach(var size in Sizes())yield return Capture("Menu",size,flow);
            var before=JsonUtility.ToJson(flow.Session.State);FirstDemoTests.Click(flow.Front,"new");yield return null;
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                FirstDemoTests.Click(flow.Front,"class-"+id);yield return null;yield return null;
                Assert.That(JsonUtility.ToJson(flow.Session.State),Is.EqualTo(before),"Presentation must not select gameplay class early");
                var showing=flow.Front.GetComponentsInChildren<RuntimeClassPreview>().Where(p=>p.IsShowing).ToArray();Assert.That(showing.Length,Is.EqualTo(1));Assert.That(showing[0].Model.GetComponent<HumanoidAnimationDriver>().CanAnimate,Is.True);
                foreach(var size in Sizes())yield return Capture("Class-"+id,size,flow);
            }
            FirstDemoTests.Click(flow.Front,"character-back");FirstDemoTests.Click(flow.Front,"options");yield return TacticalCombatIntegrationTests.Wait(.3f);foreach(var size in Sizes())yield return Capture("Options",size,flow);FirstDemoTests.Click(flow.Front,"apply");
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",flow.Session.Rules,out var state),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;yield return null;
            Assert.That(UnityEngine.Object.FindObjectsByType<RuntimeClassPreview>(FindObjectsInactive.Include).Any(p=>p.IsShowing),Is.False,"Preview rendering stops in gameplay");
            PlayerPrefs.DeleteKey(flow.Front.Preferences.StorageKey);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static Vector2Int[] Sizes()=>new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080),new Vector2Int(2560,1440),new Vector2Int(3840,2160)};
        private static IEnumerator Capture(string name,Vector2Int size,DemoFlow flow)=>DemoCapture.Save("Logs/E17/Captures/UI/"+name+"-"+size.x+".png",size.x,size.y,flow.Front,ValidateText);
        private static void ValidateText(Camera camera,int width,int height)
        {foreach(var text in UnityEngine.Object.FindAnyObjectByType<DemoFrontEnd>().GetComponentsInChildren<TMP_Text>()){text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,text.name+": "+text.text+" rect="+text.rectTransform.rect);}}
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
