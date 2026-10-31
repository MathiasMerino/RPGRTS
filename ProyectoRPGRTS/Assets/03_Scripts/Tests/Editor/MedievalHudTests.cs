using System.Collections;
using System.IO;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class MedievalHudTests
    {
        [SetUp]public void Open(){if(!EditorApplication.isPlaying)EditorSceneManager.OpenScene("Assets/01_Scenes/PlayablePrototype.unity");}
        [Test]public void SkinUsesOneAtlasAndMetricWorldResourcesStaySeparate()
        {
            var theme=AssetDatabase.LoadAssetAtPath<MedievalUiTheme>("Assets/Settings/Prototype/MedievalTheme.asset");
            foreach(var sprite in new[]{theme.PanelSprite,theme.PaperSprite,theme.ButtonSprite,theme.InsetSprite})
            {Assert.That(sprite,Is.Not.Null);Assert.That(sprite.texture,Is.SameAs(theme.PanelSprite.texture));Assert.That(sprite.border.x,Is.GreaterThan(0));}
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>("Assets/07_Characters/Provisional/ClassVisuals.asset");
            foreach(var entry in catalog.Entries){Assert.That(entry.ReferenceArt,Is.Not.Null);Assert.That(entry.ReferenceCaption,Does.Contain("provisional").And.Not.Contain("Ã").And.Not.Contain("Â"));}
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();Assert.That(hud.CompactView,Is.Not.Null);
            Assert.That(hud.GetComponent<CanvasScaler>().referenceResolution,Is.EqualTo(new Vector2(1280,720)));
            Assert.That(hud.transform.Find("Context Actions").GetComponent<Image>().enabled,Is.False,"Transparent container must not block the world.");
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/UI/MedievalPanel.prefab"),Is.Not.Null,"Historical prefabs preserved.");
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/UI/C02/MedievalWindow.prefab"),Is.Not.Null);
        }
        [UnityTest]public IEnumerator WindowsInventoryClassesAndSaveUseExistingStateAndSingleCommands()
        {
            yield return new EnterPlayMode();yield return null;yield return null;
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var view=hud.CompactView;var d=Object.FindAnyObjectByType<PrototypeDirector>();
            Assert.That(view.Window.activeSelf,Is.False);Assert.That(view.HelpPanel.activeSelf,Is.False);
            foreach(var name in new[]{"Guerrero","Mago","Monje"})
            {
                hud.SelectPage(ExplorationPage.Character);Click(name);yield return null;
                Assert.That(d.Session.State.ClassId,Is.EqualTo(name=="Guerrero"?"warrior":name=="Mago"?"mage":"monk"));
                Assert.That(view.Reference.gameObject.activeInHierarchy,Is.True);Assert.That(view.Reference.Portrait.texture,Is.Not.Null);
                var profile=d.Session.Rules.Find(d.Session.State.ClassId);Assert.That(view.Body.text,Does.Contain(profile.Title).And.Contain("Daño base: "+profile.Damage));
                Assert.That(view.HealthLabel.text,Is.EqualTo(profile.Health+" / "+profile.Health+" PV"));
            }
            var input=hud.GetComponentInChildren<InputField>();input.onEndEdit.Invoke("Viaje C02");Assert.That(d.Session.State.Name,Is.EqualTo("Viaje C02"));
            Click("Cerrar");Assert.That(view.Window.activeSelf,Is.False);Click("Ayuda");Assert.That(view.HelpPanel.activeSelf,Is.True);hud.ClosePanels();
            Assert.That(d.Hero.Warp(new Vector3(-3.4f,0,-5)),Is.True);Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(d.Hero,d.Session),Is.True);
            hud.SelectPage(ExplorationPage.Inventory);Click("Item training-sword");
            for(var i=0;i<5;i++){hud.enabled=false;hud.enabled=true;}
            Assert.That(d.Session,Is.Not.Null);Assert.That(view.Inventory,Is.Not.Null);
            AssertSingleCommand(d.Session,"Equipar espada");
            Assert.That(d.Session.State.Weapon,Is.EqualTo("training-sword"));
            Click("Item cloth");Click("Equipar túnica");Assert.That(d.Session.State.Armour,Is.EqualTo("cloth"));
            d.Session.State.Health-=4;d.Session.Say("Ensayo de salud.");Click("Item ration");var before=view.Inventory.Count(d.Session.State,"ration");Click("Usar ración");
            Assert.That(view.Inventory.Count(d.Session.State,"ration"),Is.EqualTo(before-1));Assert.That(d.Session.State.Health,Is.EqualTo(20));
            var gold=d.Session.State.Gold;Click("Comerciar");Assert.That(d.Session.State.Gold,Is.EqualTo(gold-5));
            Assert.That(d.Save("Logs/C02-save.json"),Is.True);hud.SelectPage(ExplorationPage.Character);Click("Mago");Assert.That(d.Load("Logs/C02-save.json"),Is.True);
            hud.SelectPage(ExplorationPage.Character);Assert.That(d.Session.State.ClassId,Is.EqualTo("monk"));Assert.That(view.Equipment.text,Does.Contain("espada").And.Contain("túnica"));
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest]public IEnumerator FourResolutionsContextModesAndWorldSpaceRemainReadable()
        {
            yield return new EnterPlayMode();yield return null;yield return null;
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var d=Object.FindAnyObjectByType<PrototypeDirector>();
            File.WriteAllText("Logs/C02-ui-metrics.txt","Canvas="+hud.GetComponents<Canvas>().Length+"; graphics="+hud.GetComponentsInChildren<Graphic>(true).Length+"; atlasRuntimeBytes="+UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(hud.CompactView.Theme.PanelSprite.texture)+"; persistentArea="+((1248f*66+1248f*106)/(1280*720)).ToString("P2")+"; authoring portrait cameras absent; metrics are not FPS/GC measurements.");
            d.Hero.Select(true);Camera.main.GetComponent<IsometricCameraRig>().FocusOn(new Vector3(0,0,1));
            yield return Captures("Exploration");
            Assert.That(UiInputGuard.HasUiAt(new Vector2(Screen.width/2f,Screen.height/2f)),Is.False,"Closed panels leave the world clickable.");
            foreach(var name in new[]{"Guerrero","Mago","Monje"}){hud.SelectPage(ExplorationPage.Character);Click(name);yield return Captures(name);}
            hud.SelectPage(ExplorationPage.Inventory);yield return Captures("Inventory");
            hud.SelectPage(ExplorationPage.Overview);yield return Captures("Journal");
            hud.SelectPage(ExplorationPage.Session);yield return Captures("Menu");
            Click("Ayuda");yield return Captures("Help");hud.ClosePanels();
            Assert.That(d.Execute("construct"),Is.True);yield return Captures("Construction");Click("Return to Exploration");Assert.That(d.Session.Mode,Is.EqualTo(PrototypeMode.Exploration));
            Assert.That(d.Hero.Warp(new Vector3(-6,0,-6)),Is.True);Assert.That(d.Encounter.Begin(42),Is.True);yield return Captures("RPG");
            Assert.That(d.Hero.Warp(d.Encounter.EnemyPosition+Vector3.right*1.3f),Is.True);
            for(var i=0;i<20&&!d.Encounter.Battle.Finished;i++){d.Encounter.Attack();if(!d.Encounter.Battle.Finished)d.Encounter.EndTurn();yield return null;}
            Assert.That(d.Encounter.Battle.Won,Is.True);Click("Return to Exploration");Assert.That(d.Execute("rts"),Is.True);
            Assert.That(d.Rts.SelectedAllies,Is.EqualTo(6));yield return Captures("RTS");
            Assert.That(Find("Return to Exploration").interactable,Is.False,"Do not bypass an active encounter.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator Captures(string name)
        {foreach(var width in new[]{1280,1600,1920,2560})yield return SceneCapture.Save("Logs/C02-"+name+"-"+width+".png",width,width*9/16,Layout);}
        internal static void Layout(int width,int height)
        {
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var canvas=hud.GetComponent<Canvas>();var camera=canvas.worldCamera;
            var buttons=hud.GetComponentsInChildren<Button>();var rects=new Rect[buttons.Length];
            for(var i=0;i<buttons.Length;i++)
            {
                var corners=new Vector3[4];buttons[i].GetComponent<RectTransform>().GetWorldCorners(corners);var min=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);var max=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);rects[i]=Rect.MinMaxRect(min.x,min.y,max.x,max.y);
                Assert.That(min.x,Is.GreaterThanOrEqualTo(0),buttons[i].name);Assert.That(min.y,Is.GreaterThanOrEqualTo(0),buttons[i].name);Assert.That(max.x,Is.LessThanOrEqualTo(width),buttons[i].name);Assert.That(max.y,Is.LessThanOrEqualTo(height),buttons[i].name);
                Assert.That(max.y-min.y,Is.GreaterThanOrEqualTo(32*width/1280f),"Minimum usable target: "+buttons[i].name);
                for(var j=0;j<i;j++)Assert.That(rects[i].Overlaps(rects[j]),Is.False,buttons[i].name+" overlaps "+buttons[j].name);
                var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=(min+max)*.5f};
                var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointer,hits);
                Assert.That(hits.Count,Is.GreaterThan(0),"No pointer hit: "+buttons[i].name);
                Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(buttons[i]),"Button obscured: "+buttons[i].name+" by "+hits[0].gameObject.name);
            }
            foreach(var text in hud.GetComponentsInChildren<Text>())
                if(text.GetComponentInParent<InputField>()==null&&!string.IsNullOrEmpty(text.text))Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+1),"Clipped: "+text.name+" "+text.text);
            var view=hud.CompactView;
            if(!view.WindowOpen&&!view.HelpOpen)
            {
                var raycastArea=(1248f*66+1248f*106)/(1280*720);
                Assert.That(raycastArea,Is.LessThan(.25f),"Persistent HUD should leave over 75% of the screen free.");
            }
        }
        // Create this closure after EnterPlayMode's domain reload, outside the test iterator.
        private static void AssertSingleCommand(GameSession session,string button)
        {
            var messages=0;System.Action<string> listener=_=>messages++;
            session.Message+=listener;try{Click(button);}finally{session.Message-=listener;}
            Assert.That(messages,Is.EqualTo(1));
        }
        private static Button Find(string name)
        {var hud=Object.FindAnyObjectByType<VerticalSliceHud>();Assert.That(hud,Is.Not.Null,"HUD during "+name);foreach(var b in hud.GetComponentsInChildren<Button>(true))if(b.name==name)return b;Assert.Fail("Missing "+name);return null;}
        private static void Click(string name){var b=Find(name);Assert.That(b,Is.Not.Null);Assert.That(b.gameObject.activeInHierarchy,Is.True,"Hidden action "+name);Assert.That(b.interactable,Is.True,"Disabled action "+name);Assert.That(b.onClick,Is.Not.Null,"Event "+name);b.onClick.Invoke();}
        [UnityTearDown]public IEnumerator Clean(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
