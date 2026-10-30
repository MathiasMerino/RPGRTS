using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aethernia;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Aethernia.Tests.Editor
{
    public class MedievalEnvironmentTests
    {
        [SetUp] public void Open()
        { if(!EditorApplication.isPlaying) EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath); }

        [Test] public void LibraryHasSharedSupportedMaterialsAndMetricPrefabs()
        {
            foreach(var guid in AssetDatabase.FindAssets("t:Material",new[]{EnvironmentMaterials.Root}))
            {
                var material=AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                Assert.That(material.shader.name,Is.EqualTo("Universal Render Pipeline/Lit")); Assert.That(material.shader.isSupported,Is.True);
                Assert.That(material.GetTexture("_BaseMap"),Is.Not.Null); Assert.That(material.GetTexture("_BumpMap"),Is.Not.Null);
            }
            Assert.That(AssetDatabase.FindAssets("t:Material",new[]{EnvironmentMaterials.Root}).Length,Is.EqualTo(8));
            Assert.That(AssetDatabase.FindAssets("t:Prefab",new[]{MedievalPrefabs.Root}).Length,Is.EqualTo(11));
            var report=new StringBuilder("D01 library geometry (not a frame-time benchmark)\n");
            foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{MedievalPrefabs.Root}))
            {
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                Assert.That(prefab.transform.localScale,Is.EqualTo(Vector3.one));
                Assert.That(prefab.GetComponentsInChildren<Collider>(true).Length,Is.Zero,"Visual library must not introduce duplicate physics.");
                foreach(var filter in prefab.GetComponentsInChildren<MeshFilter>())
                { Assert.That(filter.sharedMesh,Is.Not.Null); Assert.That(filter.sharedMesh.vertexCount,Is.LessThan(20000)); }
                var triangles=0;
                foreach(var filter in prefab.GetComponentsInChildren<MeshFilter>()) triangles+=filter.sharedMesh.triangles.Length/3;
                report.AppendLine(prefab.name+": bounds="+BoundsOf(prefab).size.ToString("F3")+", renderers="+prefab.GetComponentsInChildren<Renderer>().Length+", triangles="+triangles);
            }
            var quarter=GameObject.Find(MedievalQuarterBuilder.RootName);
            var worldTriangles=0;
            foreach(var filter in quarter.GetComponentsInChildren<MeshFilter>()) worldTriangles+=filter.sharedMesh.triangles.Length/3;
            report.AppendLine("D01 scene root: renderers="+quarter.GetComponentsInChildren<Renderer>().Length+", triangles="+worldTriangles);
            File.WriteAllText("Logs/D01-geometry.txt",report.ToString());
            var door=MedievalPrefabs.Load("Architecture","OpenDoor_120x220cm");
            var bounds=BoundsOf(door); Assert.That(bounds.size.x,Is.EqualTo(1.6f).Within(.025f)); Assert.That(bounds.size.y,Is.EqualTo(2.4f).Within(.005f));
            var house=BoundsOf(MedievalPrefabs.Load("Architecture","TimberHouse_6x8m"));
            Assert.That(house.size.x,Is.InRange(6.6f,6.9f)); Assert.That(house.size.z,Is.InRange(8.6f,8.9f)); Assert.That(house.size.y,Is.InRange(5.5f,5.7f));
        }
        [Test] public void DressingPreservesOriginalPhysicsAndBakedNavigation()
        {
            var expected=File.ReadAllText(EnvironmentMaterials.Root+"/PhysicsBaseline.txt");
            Assert.That(EnvironmentPhysicsSnapshot.Capture(SceneManager.GetActiveScene()),Is.EqualTo(expected));
            Assert.That(MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath),Is.EqualTo(File.ReadAllText(EnvironmentMaterials.Root+"/NavigationBaseline.txt")));
            Assert.That(GameObject.Find(MedievalQuarterBuilder.RootName),Is.Not.Null);
            EnvironmentValidationTests.AssertSceneReferences(SceneManager.GetActiveScene());
        }
        [UnityTest] public IEnumerator IdleCameraDoesNotZoomWithoutWheelInput()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            yield return new EnterPlayMode();
            var mouse=InputSystem.AddDevice<Mouse>();
            var node=new GameObject("Idle camera test"); var rig=node.AddComponent<IsometricCameraRig>(); rig.Configure(Vector3.zero);
            try
            {
                var size=rig.Size;
                for(var i=0;i<5;i++) yield return null;
                Assert.That(rig.Size,Is.EqualTo(size),"No wheel input must preserve the authored camera size.");
            }
            finally { InputSystem.RemoveDevice(mouse); Object.Destroy(node); }
            yield return new ExitPlayMode();
        }
        [UnityTest] public IEnumerator QuarterNavigationAndPresentationWorkTogether()
        {
            yield return new EnterPlayMode(); yield return null; yield return null;
            var director=Object.FindAnyObjectByType<PrototypeDirector>(); var hero=director.Hero;
            Assert.That(hero.Agent.isOnNavMesh,Is.True);
            var pathEnd=new Vector3(6,0,5);
            Assert.That(hero.TryMoveTo(pathEnd),Is.True,"House entrance must remain accessible.");
            var deadline=Time.realtimeSinceStartup+15;
            while(Vector3.Distance(hero.transform.position,pathEnd)>.3f&&Time.realtimeSinceStartup<deadline) yield return null;
            Assert.That(Vector3.Distance(hero.transform.position,pathEnd),Is.LessThan(.3f));
            Assert.That(hero.TryMoveTo(new Vector3(-3.4f,0,-5)),Is.True);
            deadline=Time.realtimeSinceStartup+15;
            while(hero.IsMoving&&Time.realtimeSinceStartup<deadline) yield return null;
            Assert.That(Vector3.Distance(hero.transform.position,new Vector3(-3.4f,0,-5)),Is.LessThan(.3f));
            Assert.That(hero.TryMoveTo(new Vector3(-2,0,5)),Is.False,"Dressed crate stack must still block the original obstacle.");
            var rig=Camera.main.GetComponent<IsometricCameraRig>();
            hero.Select(true);
            // Original source renderers remain in the scene: compare presentation at identical framing.
            using(var baseline=new BaselinePresentation()) yield return CaptureQuarter(rig,"Before");
            yield return CaptureQuarter(rig,"After");
            Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(hero,director.Session),Is.True);
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }
        private static IEnumerator CaptureQuarter(IsometricCameraRig rig,string stage)
        {
            rig.FocusOn(new Vector3(3,0,4)); rig.ApplyInput(Vector2.zero,rig.Size-15,0,false);
            yield return SceneCapture.Save("Logs/D01-"+stage+"-UI-1280.png");
            yield return SceneCapture.Save("Logs/D01-"+stage+"-UI-1920.png",1920,1080);
            Assert.That(rig.Size,Is.EqualTo(15),"Capture must preserve the controlled framing.");
            var canvas=Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>(); canvas.enabled=false;
            try
            {
                rig.FocusOn(new Vector3(2.2f,0,2.5f)); rig.ApplyInput(Vector2.zero,rig.Size-10.5f,0,false);
                yield return SceneCapture.Save("Logs/D01-"+stage+"-Quarter.png",1920,1080);
                rig.FocusOn(new Vector3(6,0,6)); rig.ApplyInput(Vector2.zero,rig.Size-7,0,false);
                yield return SceneCapture.Save("Logs/D01-"+stage+"-House.png",1920,1080);
            }
            finally { canvas.enabled=true; }
            Assert.That(rig.Size,Is.EqualTo(7));
        }
        private sealed class BaselinePresentation : System.IDisposable
        {
            private readonly GameObject root=GameObject.Find(MedievalQuarterBuilder.RootName);
            private readonly Dictionary<Renderer,(bool enabled,Material[] materials)> renderers=new Dictionary<Renderer,(bool,Material[])>();
            private readonly Light sun=GameObject.Find("Directional Light").GetComponent<Light>();
            private readonly Color colour,background,sky,equator,ground,ambient;
            private readonly float intensity,strength;
            private readonly Quaternion rotation;
            private readonly AmbientMode mode;
            public BaselinePresentation()
            {
                colour=sun.color; intensity=sun.intensity; strength=sun.shadowStrength; rotation=sun.transform.rotation;
                background=Camera.main.backgroundColor; mode=RenderSettings.ambientMode; ambient=RenderSettings.ambientLight;
                sky=RenderSettings.ambientSkyColor; equator=RenderSettings.ambientEquatorColor; ground=RenderSettings.ambientGroundColor;
                foreach(var renderer in Object.FindObjectsByType<Renderer>()) renderers[renderer]=(renderer.enabled,renderer.sharedMaterials);
                root.SetActive(false);
                foreach(var name in new[]{"ReferenceHouse_6x8m","ReferenceCube_1m","Obstacle 3x2x2m","Interaction Chest","Faction A Market","Street marker 4x12m - no collider"})
                    foreach(var renderer in GameObject.Find(name).GetComponentsInChildren<Renderer>()) renderer.enabled=true;
                GameObject.Find("ReferenceGround_40m").GetComponentInChildren<Renderer>().sharedMaterial=OldMaterial("Ground");
                foreach(var name in new[]{"ReferenceDoor_120x220cm","ReferenceWindow_100x120cm","Narrow Gate - 0.40m clearance"})
                    foreach(var renderer in GameObject.Find(name).GetComponentsInChildren<Renderer>()) renderer.sharedMaterial=OldMaterial("Timber");
                sun.color=Color.white; sun.intensity=1.2f; sun.shadowStrength=1; sun.transform.rotation=Quaternion.Euler(50,-30,0);
                RenderSettings.ambientMode=AmbientMode.Flat; RenderSettings.ambientLight=new Color(.48f,.52f,.60f);
                Camera.main.backgroundColor=new Color(.10f,.14f,.19f);
            }
            private static Material OldMaterial(string name)=>AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Development/Development_"+name+".mat");
            public void Dispose()
            {
                foreach(var pair in renderers) if(pair.Key!=null) { pair.Key.enabled=pair.Value.enabled; pair.Key.sharedMaterials=pair.Value.materials; }
                root.SetActive(true); sun.color=colour; sun.intensity=intensity; sun.shadowStrength=strength; sun.transform.rotation=rotation;
                RenderSettings.ambientMode=mode; RenderSettings.ambientLight=ambient; RenderSettings.ambientSkyColor=sky;
                RenderSettings.ambientEquatorColor=equator; RenderSettings.ambientGroundColor=ground; Camera.main.backgroundColor=background;
            }
        }
        private static Bounds BoundsOf(GameObject prefab)
        {
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var renderers=instance.GetComponentsInChildren<Renderer>(); var bounds=renderers[0].bounds;
                foreach(var renderer in renderers) bounds.Encapsulate(renderer.bounds); return bounds;
            }
            finally { Object.DestroyImmediate(instance); }
        }
        [UnityTearDown] public IEnumerator Cleanup() { if(EditorApplication.isPlaying) yield return new ExitPlayMode(); }
    }
}
