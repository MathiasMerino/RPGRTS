using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class FirstDemoTests
    {
        [Test] public void FontAndProvisionalDataHaveRealSources()
        {
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FirstDemoBuilder.FontPath);
            Assert.That(font,Is.Not.Null);Assert.That(font.atlasWidth,Is.EqualTo(2048));
            Assert.That(font.HasCharacters("áéíóúñüÁÉÍÓÚÑÜ¿¡",out uint[] missing,true,true),Is.True);
            Assert.That(File.ReadAllText("Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt"),Does.Contain("SIL Open Font License"));
            var data=AssetDatabase.LoadAssetAtPath<DemoContent>(FirstDemoBuilder.Root+"/DemoContent.asset");
            Assert.That(data.Lineages.Select(x=>x.Id),Is.EquivalentTo(new[]{"human","elf","dwarf"}));Assert.That(data.Intro.Length,Is.EqualTo(3));
        }
        [Test] public void SelectionSaveAndPreferencesAreSeparateAndCompatible()
        {
            var rules=ScriptableObject.CreateInstance<PrototypeRules>();Directory.CreateDirectory("Logs/E10");var path="Logs/E10/persistence-"+System.Guid.NewGuid().ToString("N")+".json";
            try
            {
                var state=new PrototypeState{ClassId="mage",Health=16,RaceId="elf",Demo=new DemoProgress()};Assert.That(PrototypeSaveStore.Save(path,state,rules),Is.True);
                Assert.That(PrototypeSaveStore.Load(path,rules,out var saved),Is.True);Assert.That(saved.RaceId,Is.EqualTo("elf"));Assert.That(saved.Demo.Chapter,Is.EqualTo(DemoChapter.Intro));
                var legacy=JsonUtility.ToJson(new PrototypeState()).Replace("\"RaceId\":\"human\",","");File.WriteAllText(path,legacy);Assert.That(PrototypeSaveStore.Load(path,rules,out saved),Is.True);Assert.That(saved.RaceId,Is.EqualTo("human"));
                saved.RaceId="unknown-race";Assert.That(PrototypeSaveStore.Save(path,saved,rules),Is.False);
                var key="Aethernia.Tests.Options."+System.Guid.NewGuid().ToString("N");var pref=DemoPreferences.Load(key);pref.Master=.42f;pref.Effects=.31f;pref.Save();Assert.That(DemoPreferences.Load(key).Master,Is.EqualTo(.42f).Within(.001));PlayerPrefs.DeleteKey(key);
            }
            finally{Object.DestroyImmediate(rules);}
        }
        [UnityTest] public IEnumerator MainMenuActionsLayoutsAndNewGame()=>Menu();
        private static IEnumerator Menu()
        {
            EditorSceneManager.OpenScene(FirstDemoBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var view=flow.Front;flow.SlotPath="Logs/E10/menu-"+System.Guid.NewGuid().ToString("N")+".json";view.Preferences.StorageKey="Aethernia.Tests.Menu."+System.Guid.NewGuid().ToString("N");flow.ShowMain();
            Assert.That(view.Screen,Is.EqualTo(DemoScreen.Main));Assert.That(flow.Session.Paused,Is.True);Assert.That(flow.Session.Mode,Is.EqualTo(PrototypeMode.MainMenu));
            Assert.That(view.Controls["continue"].interactable,Is.EqualTo(flow.HasSave()));
            Directory.CreateDirectory("Logs/E10/Captures");yield return TacticalCombatIntegrationTests.Wait(.35f);
            foreach(var width in new[]{1280,1920,2560,3840})yield return DemoCapture.Save("Logs/E10/Captures/Main-"+width+".png",width,width*9/16,view);
            Click(view,"options");yield return TacticalCombatIntegrationTests.Wait(.3f);Assert.That(view.Screen,Is.EqualTo(DemoScreen.Options));
            view.Preferences.Master=.41f;Click(view,"apply");Assert.That(DemoPreferences.Load(view.Preferences.StorageKey).Master,Is.EqualTo(.41f).Within(.001));
            Click(view,"credits");yield return TacticalCombatIntegrationTests.Wait(.3f);yield return DemoCapture.Save("Logs/E10/Captures/Credits.png",1280,720,view);Click(view,"credits-back");
            Click(view,"new");view.ChooseClass("mage");Click(view,"lineage");view.NameInput.text="Íñigo";yield return TacticalCombatIntegrationTests.Wait(.3f);yield return DemoCapture.Save("Logs/E10/Captures/Character.png",1280,720,view);
            Click(view,"begin");Assert.That(flow.Session.State.ClassId,Is.EqualTo("mage"));Assert.That(flow.Session.State.RaceId,Is.EqualTo("elf"));Assert.That(view.Screen,Is.EqualTo(DemoScreen.Intro));
            yield return TacticalCombatIntegrationTests.Wait(.3f);yield return DemoCapture.Save("Logs/E10/Captures/Intro.png",1280,720,view);
            Assert.That(PrototypeSaveStore.Load(flow.SlotPath,flow.Session.Rules,out var saved),Is.True);Assert.That(saved.Name,Is.EqualTo("Íñigo"));Assert.That(saved.RaceId,Is.EqualTo("elf"));
            view.Quit();Assert.That(Application.isPlaying,Is.True,"Salir en editor no debe cerrar Unity");
            PlayerPrefs.DeleteKey(view.Preferences.StorageKey);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        public static void Click(DemoFrontEnd view,string id){var b=view.Controls[id];Assert.That(b.gameObject.activeInHierarchy&&b.interactable,Is.True,id);b.onClick.Invoke();}
        [UnityTearDown] public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
    public static class DemoCapture
    {
        public static IEnumerator Save(string path,int width,int height,DemoFrontEnd view=null)
        {
            var canvases=Object.FindObjectsByType<Canvas>().Where(c=>c.isRootCanvas&&c.renderMode!=RenderMode.WorldSpace).ToArray();var modes=canvases.Select(c=>c.renderMode).ToArray();var cameras=canvases.Select(c=>c.worldCamera).ToArray();var distances=canvases.Select(c=>c.planeDistance).ToArray();
            var camera=Camera.main;var old=camera.targetTexture;var target=RenderTexture.GetTemporary(width,height,24);Texture2D image=null;
            try
            {
                camera.targetTexture=target;foreach(var canvas in canvases){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=2;}
                yield return null;yield return null;yield return null;Canvas.ForceUpdateCanvases();
                if(view!=null)Validate(view,width,height,camera);
                var previous=RenderTexture.active;RenderTexture.active=target;image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();RenderTexture.active=previous;File.WriteAllBytes(path,image.EncodeToPNG());
            }
            finally
            {camera.targetTexture=old;for(var i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}if(image!=null)Object.DestroyImmediate(image);RenderTexture.ReleaseTemporary(target);}
        }
        private static void Validate(DemoFrontEnd view,int width,int height,Camera camera)
        {
            foreach(var button in view.GetComponentsInChildren<Button>())
            {
                var p=new Vector3[4];button.GetComponent<RectTransform>().GetWorldCorners(p);var min=RectTransformUtility.WorldToScreenPoint(camera,p[0]);var max=RectTransformUtility.WorldToScreenPoint(camera,p[2]);
                Assert.That(min.x,Is.GreaterThanOrEqualTo(-1),button.name);Assert.That(min.y,Is.GreaterThanOrEqualTo(-1),button.name);Assert.That(max.x,Is.LessThanOrEqualTo(width+1),button.name);Assert.That(max.y,Is.LessThanOrEqualTo(height+1),button.name);
                var hits=new System.Collections.Generic.List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=(min+max)*.5f},hits);
                Assert.That(hits.Count,Is.GreaterThan(0),button.name);Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),"Obstruido "+button.name);
            }
            foreach(var text in view.GetComponentsInChildren<TMP_Text>())
            {text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,"Texto recortado: "+text.name+" / "+text.text);}
        }
    }
}
