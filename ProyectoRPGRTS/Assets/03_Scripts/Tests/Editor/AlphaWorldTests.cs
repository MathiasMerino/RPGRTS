using System;
using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;
namespace Aethernia.Tests.Editor
{
    public sealed class AlphaWorldTests
    {
        [Test]public void SharedShaderCompilesAndKeepsOriginalTexturesAndPhysics()
        {
            EditorSceneManager.OpenScene(VisualPolishBuilder.ScenePath);var before=VisualPolishTests.PhysicsSignature();EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);Assert.That(VisualPolishTests.PhysicsSignature(),Is.EqualTo(before));
            var shader=AssetDatabase.LoadAssetAtPath<Shader>(AlphaSurfaceGraph.Path);Assert.That(shader,Is.Not.Null);Assert.That(ShaderUtil.ShaderHasError(shader),Is.False);Assert.That(shader.isSupported,Is.True);var materials=AssetDatabase.FindAssets("t:Material",new[]{AlphaPipelineBuilder.Root+"/Materials"}).Select(g=>AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g))).ToArray();Assert.That(materials.Length,Is.EqualTo(15));
            foreach(var m in materials){Assert.That(m.shader,Is.SameAs(shader));Assert.That(m.HasProperty("_WindStrength")&&m.HasProperty("_BaseColor")&&m.HasProperty("_BumpMap"),Is.True);var texture=m.GetTexture("_BaseMap");if(texture!=null)Assert.That(AssetDatabase.GetAssetPath(texture),Does.Not.StartWith(AlphaPipelineBuilder.Root),"Reuse original texture");}
            Assert.That(materials.Count(m=>m.GetFloat("_WindStrength")>0),Is.EqualTo(2));Assert.That(Object.FindObjectsByType<VisualWindBounds>(FindObjectsInactive.Include).Length,Is.GreaterThan(0));
        }
        [UnityTest]public IEnumerator RegionTravelAndCutawayRestore()=>Travel();
        private static IEnumerator Travel()
        {
            EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E17/world-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",flow.Session.Rules,out var state),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;Directory.CreateDirectory("Logs/E17/Captures/World");
            yield return DemoCapture.Save("Logs/E17/Captures/World/Village.png",1920,1080);var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();Assert.That(dungeon.Travel(true),Is.True);yield return TacticalCombatIntegrationTests.Wait(1.5f);Assert.That(Object.FindAnyObjectByType<RegionalAtmosphere>().Dungeon,Is.True);var marker=GameObject.Find("E16 landmark umbral-exit");Assert.That(Vector3.Distance(marker.transform.position,flow.Director.Hero.transform.position),Is.GreaterThan(1),"Visual marker must not occupy arrival point");yield return DemoCapture.Save("Logs/E17/Captures/World/Ruins.png",1920,1080);Assert.That(dungeon.Travel(false),Is.True);yield return null;
            flow.Session.Paused=true;var camera=Camera.main;var rig=flow.CameraRig;rig.enabled=false;var forward=camera.transform.forward;var cut=Object.FindObjectsByType<RoofCutaway>().First(c=>c.GetComponentsInChildren<Renderer>().Any(r=>r.gameObject.activeInHierarchy&&r.sharedMaterial!=null&&r.sharedMaterial.name.Contains("Slate")));var renderers=cut.GetComponentsInChildren<Renderer>().Where(r=>r.gameObject.activeInHierarchy&&r.enabled&&r.sharedMaterial!=null&&r.sharedMaterial.name.Contains("Slate")).ToArray();
            var bounds=renderers[0].bounds;var distance=(bounds.center.y-1.55f)/Mathf.Max(.1f,-forward.y);var dummy=new GameObject("Visual occlusion fixture");dummy.transform.position=bounds.center+forward*distance-Vector3.up*1.55f;camera.transform.position=dummy.transform.position+Vector3.up*1.55f-forward*16;
            cut.enabled=false;var originals=renderers.Select(r=>r.forceRenderingOff).ToArray();cut.Configure(camera,dummy.transform,renderers);cut.enabled=true;yield return null;yield return null;Assert.That(cut.IsCutAway,Is.True);Assert.That(renderers.All(r=>r.forceRenderingOff),Is.True);cut.enabled=false;for(var i=0;i<renderers.Length;i++)Assert.That(renderers[i].forceRenderingOff,Is.EqualTo(originals[i]));
            cut.Configure(camera,flow.Director.Hero.transform,renderers);cut.enabled=true;rig.enabled=true;Object.Destroy(dummy);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest]public IEnumerator WindMovesGpuPixelsAndStaticMaterialDoesNot()=>Wind();
        private static IEnumerator Wind()
        {
            EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();flow.Front.Hide();flow.ExistingHud.gameObject.SetActive(false);flow.Session.Paused=true;flow.CameraRig.enabled=false;Object.FindAnyObjectByType<RegionalAtmosphere>().enabled=false;
            var camera=Camera.main;camera.transform.position=new Vector3(0,-100, -2);camera.transform.rotation=Quaternion.identity;camera.orthographic=true;camera.orthographicSize=.8f;camera.cullingMask=1<<30;var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=false;data.antialiasing=AntialiasingMode.None;
            var leaf=GameObject.CreatePrimitive(PrimitiveType.Quad);leaf.layer=30;leaf.transform.position=new Vector3(0,-100,0);Object.DestroyImmediate(leaf.GetComponent<Collider>());var material=Object.Instantiate(AssetDatabase.LoadAssetAtPath<Material>(AlphaPipelineBuilder.Root+"/Materials/LeafLight_Alpha.mat"));leaf.GetComponent<Renderer>().sharedMaterial=material;var light=new GameObject("GPU fixture light").AddComponent<Light>();light.type=LightType.Point;light.range=5;light.intensity=3;light.cullingMask=1<<30;light.transform.position=leaf.transform.position+new Vector3(0,1,-1);
            Directory.CreateDirectory("Logs/E17/Captures/World");yield return TacticalCombatIntegrationTests.Wait(.5f);yield return DemoCapture.Save("Logs/E17/Captures/World/Wind-A.png",512,512);yield return TacticalCombatIntegrationTests.Wait(1);yield return DemoCapture.Save("Logs/E17/Captures/World/Wind-B.png",512,512);var moving=Difference("Wind-A","Wind-B");Assert.That(moving,Is.GreaterThan(50),"Shader must move visible geometry on GPU");
            material.SetFloat("_WindStrength",0);yield return null;yield return DemoCapture.Save("Logs/E17/Captures/World/Static-A.png",512,512);yield return TacticalCombatIntegrationTests.Wait(1);yield return DemoCapture.Save("Logs/E17/Captures/World/Static-B.png",512,512);var still=Difference("Static-A","Static-B");Assert.That(still,Is.EqualTo(0),"Static control isolates wind from other effects");File.WriteAllText("Logs/E17/WIND_GPU.txt","PASS movingPixels="+moving+" staticPixels="+still+" resolution=512x512; actual GPU captures, no physics movement");
            Object.Destroy(material);Object.Destroy(leaf);Object.Destroy(light.gameObject);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static int Difference(string a,string b)
        {var x=new Texture2D(2,2);var y=new Texture2D(2,2);try{x.LoadImage(File.ReadAllBytes("Logs/E17/Captures/World/"+a+".png"));y.LoadImage(File.ReadAllBytes("Logs/E17/Captures/World/"+b+".png"));var p=x.GetPixels32();var q=y.GetPixels32();var n=0;for(var i=0;i<p.Length;i++)if(!p[i].Equals(q[i]))n++;return n;}finally{Object.Destroy(x);Object.Destroy(y);}}
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
