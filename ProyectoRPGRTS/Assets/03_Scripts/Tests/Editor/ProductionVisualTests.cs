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
        [Test]public void PortraitsUseAuthorizedOriginalSheetsAndCatalogMetadata()
        {
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            foreach(var e in catalog.Entries)
            {
                Assert.That(e.IsProvisional,Is.True);Assert.That(e.DisplayTitle,Is.Not.Empty);Assert.That(e.PreviousPrefab,Is.Not.Null);Assert.That(e.PortraitSprite,Is.Not.Null);Assert.That(e.PortraitSprite.texture,Is.SameAs(e.ReferenceArt));
                Assert.That(e.ConceptSheet,Is.SameAs(e.ReferenceArt));Assert.That(e.PortraitSprite.rect.size,Is.EqualTo(new Vector2(180,240)));Assert.That(e.ReferenceUv.width*e.ReferenceArt.width,Is.EqualTo(180).Within(.01f));
                var path=AssetDatabase.GetAssetPath(e.ConceptSheet);Assert.That(MedievalQuarterBuilder.Hash(path),Is.EqualTo(MedievalQuarterBuilder.Hash("../Documentacion/Personajes/Conceptos/"+Path.GetFileName(path))));
                Assert.That(e.SharedMaterials.Length,Is.GreaterThan(0));Assert.That(e.Prefab.GetComponent<CharacterVisualModel>().WeaponAnchor,Is.Not.Null);
                Assert.That(((TextureImporter)AssetImporter.GetAtPath(path)).textureType,Is.EqualTo(TextureImporterType.Sprite));
            }
        }
        [UnityTest]public IEnumerator PortraitsEquipmentAndSaveRestoreAcrossAllClasses()
        {
            EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var presenter=d.Hero.GetComponent<CharacterVisualPresenter>();
            WorldOperations.AddItem(d.Session.State,"training-sword",1);WorldOperations.AddItem(d.Session.State,"cloth",1);
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                d.Session.SelectClass(id);hud.SelectPage(ExplorationPage.Character);yield return null;yield return null;
                var model=presenter.Model;var renderers=model.GetComponentsInChildren<Renderer>(true).Length;
                for(var i=0;i<8;i++){Assert.That(d.Execute("sword"),Is.True);Assert.That(d.Execute("armour"),Is.True);}
                Assert.That(presenter.Model,Is.SameAs(model));Assert.That(model.GetComponentsInChildren<Renderer>(true).Length,Is.EqualTo(renderers));Assert.That(model.SwordVisible&&model.ArmourVisible,Is.True);
                var entry=hud.CompactView.Catalog.FindEntry(id);Assert.That(hud.CompactView.Reference.Portrait.texture,Is.SameAs(entry.ReferenceArt));Assert.That(hud.CompactView.Reference.Portrait.uvRect,Is.EqualTo(entry.ReferenceUv));
                Assert.That(d.Save("Logs/E04-"+id+".json"),Is.True);d.Session.State.Weapon="";d.Session.State.Armour="";d.Session.Say("Ensayo sin equipo");Assert.That(model.SwordVisible||model.ArmourVisible,Is.False);
                Assert.That(d.Load("Logs/E04-"+id+".json"),Is.True);hud.SelectPage(ExplorationPage.Character);yield return null;Assert.That(presenter.DisplayedClass,Is.EqualTo(id));Assert.That(presenter.Model.SwordVisible&&presenter.Model.ArmourVisible,Is.True);
                foreach(var width in new[]{1280,1920})yield return SceneCapture.Save("Logs/E04-"+id+"-"+width+".png",width,width*9/16,MedievalHudTests.Layout);
            }
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
    }
}
