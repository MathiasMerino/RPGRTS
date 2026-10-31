using System.Collections;
using System.IO;
using Aethernia;
using Aethernia.Editor.Characters;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class MonkIdentityTests
    {
        [SetUp] public void Open() { if(!EditorApplication.isPlaying) EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath); }
        [Test] public void ProxyIsMetricLightAndKeepsOtherClassesAndOriginalArt()
        {
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            var prefab=catalog.Find("monk");
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>(MonkIdentityBuilder.ProxyPath),Is.Not.Null);
            Assert.That(catalog.Find("monk"),Is.SameAs(prefab));
            foreach(var id in new[]{"warrior","mage"}) Assert.That(catalog.Find(id),Is.SameAs(AssetDatabase.LoadAssetAtPath<GameObject>(ProductionCharacterBuilder.Prefabs+"/"+id+"_Proxy.prefab")));
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/monk_Visual.prefab"),Is.Not.Null);
            Assert.That(catalog.FindEntry("monk").HumanoidPrefab,Is.SameAs(AssetDatabase.LoadAssetAtPath<GameObject>(HumanoidPipelineBuilder.TemplatePath)));
            Assert.That(prefab.GetComponentsInChildren<Collider>(true).Length,Is.Zero);Assert.That(prefab.GetComponentsInChildren<Animator>(true).Length,Is.Zero);
            Assert.That(prefab.transform.localScale,Is.EqualTo(Vector3.one));
            var node=(GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var renderers=node.GetComponentsInChildren<Renderer>(true);var bounds=renderers[0].bounds;var triangles=0;
                foreach(var renderer in renderers){bounds.Encapsulate(renderer.bounds);Assert.That(renderer.sharedMaterial.shader.name,Is.EqualTo("Universal Render Pipeline/Lit"));}
                foreach(var filter in node.GetComponentsInChildren<MeshFilter>(true))triangles+=filter.sharedMesh.triangles.Length/3;
                Assert.That(bounds.min.y,Is.EqualTo(0).Within(.001f));Assert.That(bounds.max.y,Is.EqualTo(1.78f).Within(.005f));
                Assert.That(triangles,Is.LessThan(10000));Assert.That(renderers.Length,Is.LessThanOrEqualTo(10));
                File.WriteAllText("Logs/E02B-geometry.txt","PASS: bounds="+bounds.size.ToString("F3")+"; triangles (equipment included)="+triangles+"; renderers="+renderers.Length+"; no runtime primitives/colliders/Animator. Counts are not an FPS benchmark.");
            }
            finally{Object.DestroyImmediate(node);}
            Assert.That(AssetDatabase.FindAssets("t:Material",new[]{MonkIdentityBuilder.Root}).Length,Is.EqualTo(2));
            Assert.That(MedievalQuarterBuilder.Hash(MonkIdentityBuilder.Root+"/Textures/MonkConceptSheet.png"),Is.EqualTo(MedievalQuarterBuilder.Hash("../Documentacion/Personajes/Conceptos/MONJE_HOJA_PROPUESTA_v01.png")));
            var uv=catalog.FindEntry("monk").ReferenceUv;Assert.That(uv.width/uv.height,Is.GreaterThan(0));Assert.That(uv.xMin,Is.InRange(0,1));Assert.That(uv.yMax,Is.InRange(0,1));
        }
        [UnityTest] public IEnumerator MonkSelectionInteractionEquipmentLoadAndRpgStillWork()
        {
            yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var hud=Object.FindAnyObjectByType<VerticalSliceHud>();
            hud.SelectPage(ExplorationPage.Character);Button(hud,"Monje").onClick.Invoke();yield return null;yield return null;
            Assert.That(d.Session.State.ClassId,Is.EqualTo("monk"));Assert.That(d.Session.State.Health,Is.EqualTo(20));
            var profile=d.Session.Rules.Find("monk");Assert.That(profile.Damage,Is.EqualTo(5));Assert.That(profile.Range,Is.EqualTo(1.8f));Assert.That(profile.Speed,Is.EqualTo(4));
            var presenter=d.Hero.GetComponent<CharacterVisualPresenter>();Assert.That(presenter.Model.name,Does.StartWith("monk_E03_Proxy"));
            Assert.That(d.Hero.Agent.radius,Is.EqualTo(.3f));Assert.That(d.Hero.Agent.height,Is.EqualTo(1.8f));
            Physics.SyncTransforms();Assert.That(Physics.Raycast(d.Hero.transform.position+Vector3.up*3,Vector3.down,out var hit,4),Is.True);
            Assert.That(hit.collider.GetComponentInParent<ExplorationUnit>(),Is.SameAs(d.Hero));d.Hero.Select(true);Assert.That(d.Hero.IsSelected,Is.True);
            Assert.That(d.Hero.TryMoveTo(new Vector3(-3.4f,0,-5)),Is.True);var deadline=Time.realtimeSinceStartup+10;
            while(d.Hero.IsMoving&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(Vector3.Distance(d.Hero.transform.position,new Vector3(-3.4f,0,-5)),Is.LessThan(.3f));
            Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(d.Hero,d.Session),Is.True);
            Assert.That(d.Execute("sword")&&d.Execute("armour"),Is.True);Assert.That(presenter.Model.SwordVisible&&presenter.Model.ArmourVisible,Is.True);
            var card=hud.GetComponentInChildren<ClassReferenceCard>(true);Assert.That(card.gameObject.activeSelf,Is.True);
            var save="Logs/E02B-save.json";Assert.That(d.Save(save),Is.True);
            Button(hud,"Guerrero").onClick.Invoke();Assert.That(card.gameObject.activeSelf,Is.True);
            Assert.That(card.Portrait.texture,Is.SameAs(hud.CompactView.Catalog.FindEntry("warrior").ReferenceArt));
            Button(hud,"Mago").onClick.Invoke();Assert.That(presenter.DisplayedClass,Is.EqualTo("mage"));
            Assert.That(d.Load(save),Is.True);yield return null;yield return null;
            Assert.That(presenter.DisplayedClass,Is.EqualTo("monk"));Assert.That(presenter.Model.SwordVisible&&presenter.Model.ArmourVisible,Is.True);
            Assert.That(card.gameObject.activeSelf,Is.True);
            Assert.That(d.Hero.Warp(new Vector3(-6,0,-6)),Is.True);Assert.That(d.Encounter.Begin(42),Is.True);
            Assert.That(card.gameObject.activeSelf,Is.False);Assert.That(d.Hero.Warp(d.Encounter.EnemyPosition+Vector3.right*1.3f),Is.True);
            Camera.main.GetComponent<IsometricCameraRig>().FocusOn(d.Hero.transform.position);
            yield return SceneCapture.Save("Logs/E02B-RPG.png",1920,1080);
            for(var i=0;i<20&&!d.Encounter.Battle.Finished;i++)
            {d.Encounter.Attack();if(!d.Encounter.Battle.Finished)d.Encounter.EndTurn();yield return null;}
            Assert.That(d.Encounter.Battle.Won,Is.True);Assert.That(d.Execute("explore"),Is.True);
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest] public IEnumerator CharacterPageCropAndMatchedWorldCapturesAreReadable()
        {
            yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var hud=Object.FindAnyObjectByType<VerticalSliceHud>();
            hud.SelectPage(ExplorationPage.Character);Button(hud,"Monje").onClick.Invoke();
            Assert.That(d.Hero.Warp(new Vector3(-4,0,-6)),Is.True);d.Hero.Select(true);d.Hero.transform.rotation=Quaternion.Euler(0,210,0);
            var camera=Camera.main;var rig=camera.GetComponent<IsometricCameraRig>();rig.FocusOn(d.Hero.transform.position);rig.ApplyInput(Vector2.zero,rig.Size-7,0,false);
            foreach(var width in new[]{1280,1920})yield return SceneCapture.Save("Logs/E02B-Character-"+width+".png",width,width*9/16,CheckCardLayout);
            var canvas=hud.GetComponent<Canvas>();canvas.enabled=false;rig.enabled=false;
            camera.orthographicSize=2.35f;camera.transform.position=d.Hero.transform.position+Vector3.up*.88f-camera.transform.forward*45;
            var model=d.Hero.GetComponent<CharacterVisualPresenter>().Model;GameObject old=null;
            try
            {
                model.gameObject.SetActive(false);
                old=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/monk_Visual.prefab"),d.Hero.transform.position,d.Hero.transform.rotation);
                yield return SceneCapture.Save("Logs/E02B-Before.png",1920,1080);
                old.SetActive(false);Object.Destroy(old);old=null;model.gameObject.SetActive(true);
                yield return SceneCapture.Save("Logs/E02B-After.png",1920,1080);
                d.Hero.transform.rotation=Quaternion.Euler(0,30,0);yield return SceneCapture.Save("Logs/E02B-Back.png",1920,1080);
                d.Hero.transform.rotation=Quaternion.Euler(0,210,0);
                WorldOperations.AddItem(d.Session.State,"training-sword",1);WorldOperations.AddItem(d.Session.State,"cloth",1);d.Execute("sword");d.Execute("armour");
                yield return SceneCapture.Save("Logs/E02B-Equipment.png",1920,1080);
            }
            finally{if(old!=null)Object.Destroy(old);model.gameObject.SetActive(true);canvas.enabled=true;rig.enabled=true;}
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static Button Button(VerticalSliceHud hud,string name)
        {foreach(var button in hud.GetComponentsInChildren<Button>(true))if(button.name==name)return button;Assert.Fail("Missing button: "+name);return null;}
        private static void CheckCardLayout(int width,int height)
        {
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var card=hud.GetComponentInChildren<ClassReferenceCard>();Assert.That(card,Is.Not.Null);
            var context=hud.transform.Find("Context Actions").GetComponent<RectTransform>();var cardRect=card.GetComponent<RectTransform>();
            var corners=new Vector3[4];cardRect.GetWorldCorners(corners);
            foreach(var corner in corners)Assert.That(RectTransformUtility.RectangleContainsScreenPoint(context,RectTransformUtility.WorldToScreenPoint(Camera.main,corner),Camera.main),Is.True);
            Assert.That(card.Information.text,Does.Contain("Monje").And.Contain("provisional"));
            Assert.That(hud.CompactView.Body.text,Does.Contain("Daño base: 5").And.Contain("20 / 20 PV"));
            Assert.That(card.Information.preferredHeight,Is.LessThanOrEqualTo(card.Information.rectTransform.rect.height+1));
            var photo=card.Portrait;var ratio=photo.rectTransform.rect.width/photo.rectTransform.rect.height;
            var expected=photo.texture.width*photo.uvRect.width/(photo.texture.height*photo.uvRect.height);Assert.That(ratio,Is.EqualTo(expected).Within(.002f));
            var grid=hud.transform.Find("Context Actions/Action Grid");var photoCorners=new Vector3[4];photo.rectTransform.GetWorldCorners(photoCorners);
            foreach(var button in grid.GetComponentsInChildren<Button>())
                Assert.That(RectTransformUtility.RectangleContainsScreenPoint(button.GetComponent<RectTransform>(),RectTransformUtility.WorldToScreenPoint(Camera.main,photoCorners[1]),Camera.main),Is.False);
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
