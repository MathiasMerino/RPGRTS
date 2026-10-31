using System.IO;
using System.Text;
using System.Collections;
using Aethernia;
using Aethernia.Editor.Characters;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class ProductionVisualTests
    {
        [UnityTest]public IEnumerator ThreeProxiesLoadAndRenderInPlayableScene()
        {
            EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var canvas=Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>();var camera=Camera.main;var rig=camera.GetComponent<IsometricCameraRig>();
            rig.enabled=false;canvas.enabled=false;camera.orthographicSize=2.6f;camera.transform.position=d.Hero.transform.position+Vector3.up*.85f-camera.transform.forward*45;
            d.Hero.transform.rotation=Quaternion.Euler(0,210,0);
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                d.Session.SelectClass(id);yield return null;
                Assert.That(d.Hero.GetComponent<CharacterVisualPresenter>().DisplayedClass,Is.EqualTo(id));
                yield return SceneCapture.Save("Logs/E03-"+id+".png",1920,1080);
            }
            canvas.enabled=true;rig.enabled=true;LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [Test]public void NewProxiesAreMetricSharedAndKeepRecoveryAssets()
        {
            var report=new StringBuilder();var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(ProductionCharacterBuilder.Prefabs+"/"+id+"_Proxy.prefab");Assert.That(catalog.Find(id),Is.SameAs(prefab));
                Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/"+id+"_Visual.prefab"),Is.Not.Null);
                Assert.That(prefab.GetComponentsInChildren<Collider>(true).Length,Is.Zero);Assert.That(prefab.GetComponentsInChildren<Animator>(true).Length,Is.Zero);Assert.That(prefab.transform.localScale,Is.EqualTo(Vector3.one));
                var node=Object.Instantiate(prefab);
                try
                {
                    var renderers=node.GetComponentsInChildren<Renderer>(true);var bounds=renderers[0].bounds;var triangles=0;
                    foreach(var r in renderers){bounds.Encapsulate(r.bounds);Assert.That(r.sharedMaterial.shader.name,Is.EqualTo("Universal Render Pipeline/Lit"));}
                    foreach(var f in node.GetComponentsInChildren<MeshFilter>(true))triangles+=f.sharedMesh.triangles.Length/3;
                    Assert.That(bounds.min.y,Is.EqualTo(0).Within(.005f));Assert.That(bounds.max.y,Is.EqualTo(id=="monk"?1.78f:1.80f).Within(.015f));Assert.That(bounds.size.x,Is.LessThan(.9f));Assert.That(triangles,Is.LessThan(18000));
                    report.AppendLine(id+": "+bounds.size.ToString("F3")+"; tris="+triangles+"; renderers="+renderers.Length);
                }finally{Object.DestroyImmediate(node);}
            }
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>(MonkIdentityBuilder.ProxyPath),Is.Not.Null);File.WriteAllText("Logs/E03-geometry.txt",report.ToString());
        }
    }
}
