using System.Collections;
using System.IO;
using System.Linq;
using Aethernia;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class CharacterVisualTests
    {
        [SetUp] public void Open()
        { if (!EditorApplication.isPlaying) EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath); }

        [UnityTest] public IEnumerator RoofOcclusionIsReproducedAndCutawayRestoresPresentation()
        {
            yield return new EnterPlayMode(); yield return null;
            var director=Object.FindAnyObjectByType<PrototypeDirector>();
            var hero=director.Hero; Assert.That(hero.Warp(new Vector3(6,0,5)),Is.True);
            var camera=Camera.main; var rig=camera.GetComponent<IsometricCameraRig>();
            Assert.That(camera,Is.Not.Null,"Main camera must remain loaded.");
            rig.FocusOn(new Vector3(6,0,6)); rig.ApplyInput(Vector2.zero,rig.Size-7,0,false);
            var roof=GameObject.Find(MedievalQuarterBuilder.RootName).GetComponentsInChildren<Transform>().First(t=>t.name=="GableRoof_660x860cm");
            var cut=roof.GetComponent<RoofCutaway>(); if(cut!=null) cut.enabled=false;
            var renderers=roof.GetComponentsInChildren<Renderer>();
            var point=hero.transform.position+Vector3.up*1.55f;
            Assert.That(camera,Is.Not.Null,"Camera after configuring framing.");
            Assert.That(roof,Is.Not.Null); Assert.That(renderers.Length,Is.GreaterThan(0));
            var forward=camera.transform.forward;
            var origin=camera.transform.position;
            var ray=new Ray(point-forward*Vector3.Dot(point-origin,forward),forward);
            var distance=Vector3.Dot(point-ray.origin,ray.direction);
            var hits=0;
            foreach(var filter in roof.GetComponentsInChildren<MeshFilter>())
            {
                Assert.That(filter.sharedMesh,Is.Not.Null,"Roof mesh: "+filter.name);
                if(MeshBlocks(filter,ray,distance)) hits++;
            }
            Assert.That(hits,Is.GreaterThan(0),"Actual roof triangles must block the view to the actor before applying the cutaway.");
            var canvas=Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>(); canvas.enabled=false;
            try
            {
                yield return SceneCapture.Save("Logs/E01-Roof-Before.png",1920,1080);
                if(cut==null) cut=roof.gameObject.AddComponent<RoofCutaway>();
                cut.Configure(camera,hero.transform,renderers); cut.enabled=true;
                yield return null; yield return null;
                Assert.That(cut.IsCutAway,Is.True); Assert.That(renderers.All(r=>r.forceRenderingOff),Is.True);
                yield return SceneCapture.Save("Logs/E01-Roof-After.png",1920,1080);
                Assert.That(hero.Agent.isOnNavMesh,Is.True); Assert.That(hero.Agent.radius,Is.EqualTo(.3f));
                cut.enabled=false; Assert.That(renderers.All(r=>!r.forceRenderingOff),Is.True,"Disabling the component must reverse the change.");
                cut.enabled=true; Assert.That(hero.Warp(new Vector3(-5,0,-4)),Is.True);
                yield return null; yield return null;
                Assert.That(cut.IsCutAway,Is.False); Assert.That(renderers.All(r=>!r.forceRenderingOff),Is.True);
                File.WriteAllText("Logs/E01-occlusion.txt","PASS: actual blocking roof meshes="+hits+"; cutaway, disable/restore and outside restore; camera/agent preserved.");
            }
            finally { canvas.enabled=true; }
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }
        private static bool MeshBlocks(MeshFilter filter,Ray ray,float maximum)
        {
            var vertices=filter.sharedMesh.vertices; var triangles=filter.sharedMesh.triangles;
            var matrix=filter.transform.localToWorldMatrix;
            for(var i=0;i<triangles.Length;i+=3)
            {
                var a=matrix.MultiplyPoint3x4(vertices[triangles[i]]);
                var b=matrix.MultiplyPoint3x4(vertices[triangles[i+1]]);
                var c=matrix.MultiplyPoint3x4(vertices[triangles[i+2]]);
                var e1=b-a; var e2=c-a; var p=Vector3.Cross(ray.direction,e2); var det=Vector3.Dot(e1,p);
                if(Mathf.Abs(det)<.000001f) continue;
                var offset=ray.origin-a; var u=Vector3.Dot(offset,p)/det;
                if(u<0||u>1) continue;
                var q=Vector3.Cross(offset,e1); var v=Vector3.Dot(ray.direction,q)/det;
                if(v<0||u+v>1) continue;
                var t=Vector3.Dot(e2,q)/det;
                if(t>0&&t<maximum) return true;
            }
            return false;
        }
        [Test] public void CatalogContainsMetricReplaceableModelsAndSharedEquipment()
        {
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            Assert.That(catalog,Is.Not.Null); Assert.That(catalog.Entries.Length,Is.EqualTo(3));
            var report=new System.Text.StringBuilder();
            foreach(var id in new[]{"warrior","mage","monk"})
            {
                var prefab=catalog.Find(id); Assert.That(prefab,Is.Not.Null); Assert.That(prefab.transform.localScale,Is.EqualTo(Vector3.one));
                Assert.That(prefab.GetComponent<CharacterVisualModel>(),Is.Not.Null);
                Assert.That(prefab.GetComponentsInChildren<Collider>(true).Length,Is.Zero);
                Assert.That(prefab.GetComponentsInChildren<Animator>(true).Length,Is.Zero);
                Assert.That(prefab.GetComponent<ExplorationUnit>(),Is.Null);
                var node=(GameObject)PrefabUtility.InstantiatePrefab(prefab);
                try
                {
                    var renderers=node.GetComponentsInChildren<Renderer>(true); var bounds=renderers[0].bounds;
                    var triangles=0;
                    foreach(var renderer in renderers) { bounds.Encapsulate(renderer.bounds); Assert.That(renderer.sharedMaterial.shader.isSupported,Is.True); }
                    foreach(var filter in node.GetComponentsInChildren<MeshFilter>(true)) triangles+=filter.sharedMesh.triangles.Length/3;
                    Assert.That(bounds.min.y,Is.EqualTo(0).Within(.005f)); Assert.That(bounds.max.y,Is.EqualTo(1.8f).Within(.015f));
                    Assert.That(bounds.size.x,Is.LessThan(.85f));
                    report.AppendLine(id+": bounds="+bounds.size.ToString("F3")+", renderers="+renderers.Length+", triangles="+triangles);
                }
                finally { Object.DestroyImmediate(node); }
            }
            Assert.That(AssetDatabase.FindAssets("t:Material",new[]{CharacterVisualBuilder.Root}).Length,Is.EqualTo(8));
            Assert.That(AssetDatabase.FindAssets("t:Prefab",new[]{CharacterVisualBuilder.Prefabs}).Length,Is.EqualTo(5));
            File.WriteAllText("Logs/E01-geometry.txt",report.ToString());
        }
        [UnityTest] public IEnumerator ClassEquipmentLoadMovementAndVisualReplacementStayConsistent()
        {
            yield return new EnterPlayMode(); yield return null; yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>(); var hero=d.Hero;
            var presenter=hero.GetComponent<CharacterVisualPresenter>(); Assert.That(presenter,Is.Not.Null);
            var legacy=hero.transform.Find("Height 1.80m - diameter 0.60m").GetComponent<Renderer>();
            foreach(var id in new[]{"warrior","mage","monk"})
            {
                d.Session.SelectClass(id); yield return null; yield return null;
                Assert.That(presenter.DisplayedClass,Is.EqualTo(id)); Assert.That(legacy.enabled,Is.False);
                Assert.That(hero.Agent.radius,Is.EqualTo(.3f)); Assert.That(hero.Agent.height,Is.EqualTo(1.8f));
                Assert.That(hero.Agent.speed,Is.EqualTo(d.Session.Rules.Find(id).Speed));
                var model=presenter.Model; d.Session.Say("Ensayo de presentación");
                Assert.That(presenter.Model,Is.SameAs(model),"Clock/message events must reuse the model.");
                Assert.That(presenter.VisualRoot.childCount,Is.EqualTo(1));
            }
            presenter.enabled=false; Assert.That(legacy.enabled,Is.True);
            d.Session.SelectClass("mage"); presenter.enabled=true; yield return null; yield return null;
            Assert.That(presenter.DisplayedClass,Is.EqualTo("mage"));
            var invalid=ScriptableObject.CreateInstance<CharacterVisualCatalog>(); var missingBinding=new GameObject("Invalid visual test");
            invalid.Entries=new[]{new CharacterVisualCatalog.Entry {ClassId="mage",Prefab=missingBinding}};
            try
            {
                presenter.Configure(d.Session,invalid,presenter.VisualRoot,legacy);
                d.Session.Say("Missing model binding"); d.Session.Say("Repeat invalid appearance");
                Assert.That(legacy.enabled,Is.True); Assert.That(presenter.VisualRoot.gameObject.activeSelf,Is.False);
                Assert.That(presenter.VisualRoot.childCount,Is.EqualTo(1),"Invalid replacements must not accumulate objects.");
            }
            finally
            {
                presenter.Configure(d.Session,AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath),presenter.VisualRoot,legacy);
                Object.Destroy(missingBinding); Object.Destroy(invalid);
            }
            WorldOperations.AddItem(d.Session.State,"training-sword",1); WorldOperations.AddItem(d.Session.State,"cloth",1);
            Assert.That(d.Execute("sword"),Is.True); Assert.That(d.Execute("armour"),Is.True);
            Assert.That(presenter.Model.SwordVisible&&presenter.Model.ArmourVisible,Is.True);
            var save="Logs/E01-test-save.json"; Assert.That(d.Save(save),Is.True);
            d.Session.SelectClass("warrior"); Assert.That(d.Load(save),Is.True); yield return null; yield return null;
            Assert.That(presenter.DisplayedClass,Is.EqualTo("mage"));
            Assert.That(presenter.Model.SwordVisible&&presenter.Model.ArmourVisible,Is.True);
            Assert.That(hero.TryMoveTo(new Vector3(6,0,5)),Is.True);
            var deadline=Time.realtimeSinceStartup+15;
            while(Vector3.Distance(hero.transform.position,new Vector3(6,0,5))>.3f&&Time.realtimeSinceStartup<deadline) yield return null;
            Assert.That(Vector3.Distance(hero.transform.position,new Vector3(6,0,5)),Is.LessThan(.3f));
            Assert.That(hero.TryMoveTo(new Vector3(-2,0,5)),Is.False,"Crate physics must remain unchanged.");
            hero.Stop(); Assert.That(hero.Warp(new Vector3(-4,0,-6)),Is.True); hero.Select(true);
            var camera=Camera.main; var rig=camera.GetComponent<IsometricCameraRig>();
            var canvas=Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>();
            rig.enabled=false; canvas.enabled=false;
            camera.orthographicSize=2.35f; camera.transform.position=hero.transform.position+Vector3.up*.88f-camera.transform.forward*45;
            try
            {
                d.Session.State.Weapon=""; d.Session.State.Armour="";
                foreach(var id in new[]{"warrior","mage","monk"})
                {
                    d.Session.SelectClass(id); hero.transform.rotation=Quaternion.Euler(0,210,0);
                    yield return null; yield return null;
                    Assert.That(presenter.Model.SwordVisible||presenter.Model.ArmourVisible,Is.False);
                    yield return SceneCapture.Save("Logs/E01-"+id+".png",1920,1080);
                }
                d.Session.SelectClass("warrior"); d.Execute("sword"); d.Execute("armour");
                yield return null; yield return null;
                yield return SceneCapture.Save("Logs/E01-Equipment.png",1920,1080);
            }
            finally { rig.enabled=true; canvas.enabled=true; }
            rig.FocusOn(hero.transform.position); yield return null;
            yield return SceneCapture.Save("Logs/E01-InGame.png",1920,1080);
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }
        [UnityTearDown] public IEnumerator Cleanup() { if(EditorApplication.isPlaying) yield return new ExitPlayMode(); }
    }
}
