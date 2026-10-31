using System;
using System.Collections.Generic;
using System.IO;
using Aethernia.Editor.Environment;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Editor
{
    public static class C02HudBuilder
    {
        public const string Root="Assets/08_UI/Medieval";
        private static MedievalUiTheme theme;
        private static readonly List<HudBinding> extra=new List<HudBinding>();
        public static void Upgrade()
        {
            var scene=EditorSceneManager.OpenScene(PlayablePrototypeBuilder.ScenePath);
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();
            if(hud.GetComponent<MedievalHudView>()!=null)throw new InvalidOperationException("C02 already exists; do not regenerate.");
            var physics=EnvironmentPhysicsSnapshot.Capture(scene);var nav=MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath);
            theme=AssetDatabase.LoadAssetAtPath<MedievalUiTheme>("Assets/Settings/Prototype/MedievalTheme.asset");
            foreach(var folder in new[]{Root+"/Sprites",Root+"/Portraits", "Assets/02_Prefabs/UI/C02"})EnvironmentMaterials.Folder(folder);
            Skin();extra.Clear();
            var canvas=hud.transform;
            PrefabUtility.UnpackPrefabInstance(canvas.gameObject,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            hud.enabled=false;
            var context=canvas.Find("Context Actions");context.GetComponent<Image>().enabled=false;Stretch((RectTransform)context,0,0);
            var view=canvas.gameObject.AddComponent<MedievalHudView>();view.Theme=theme;
            view.Catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            Portraits(view.Catalog);
            var header=canvas.Find("Title Bar");Rect(header,16,16,1248,66);Panel(header,theme.PanelSprite);
            var brand=header.GetComponentInChildren<Text>();Rect(brand.transform,18,10,160,46);brand.fontSize=16;brand.fontStyle=FontStyle.Bold;brand.text="CRÓNICAS\nDE AETHERNIA";brand.color=theme.Gold;
            view.HeroName=Text(header,"Hero Name",196,9,276,24,17,theme.Text);
            var healthTrack=CreateImage(header,"Health Track",196,39,184,12,null,new Color(.10f,.06f,.04f));
            view.HealthFill=CreateImage(healthTrack.transform,"Health",0,0,184,12,null,new Color(.55f,.17f,.12f));
            view.HealthFill.type=Image.Type.Filled;view.HealthFill.fillMethod=Image.FillMethod.Horizontal;
            view.HealthLabel=Text(header,"Health Label",390,32,92,26,16,theme.Text);
            var resource=canvas.Find("Resources");resource.SetParent(header,false);Rect(resource,500,10,510,46);resource.GetComponent<Image>().enabled=false;
            view.Resources=resource.GetComponentInChildren<Text>();Rect(view.Resources.transform,0,0,510,46);view.Resources.fontSize=16;view.Resources.color=theme.Text;
            var dock=PanelNew(canvas,"C02 Action Dock",16,598,1248,106,theme.PanelSprite);
            var portraitFrame=PanelNew(dock,"Character Portrait",12,12,66,82,theme.InsetSprite);
            view.Portrait=Raw(portraitFrame,"Portrait",6,5,54,72);view.PortraitAspect=view.Portrait.gameObject.AddComponent<AspectRatioFitter>();view.PortraitAspect.aspectMode=AspectRatioFitter.AspectMode.FitInParent;
            var pages=context.Find("Exploration Pages");pages.SetParent(dock,false);Rect(pages,94,42,276,40);
            var pageLayout=pages.GetComponent<GridLayoutGroup>();pageLayout.cellSize=new Vector2(88,38);pageLayout.spacing=new Vector2(6,6);pageLayout.constraintCount=3;
            foreach(var b in pages.GetComponentsInChildren<Button>(true))Style(b);
            var menu=pages.Find("Partida");menu.SetParent(header,false);Rect(menu,1120,14,108,38);menu.GetComponentInChildren<Text>().text="Menú";
            pages.Find("Resumen").GetComponentInChildren<Text>().text="Registro";
            Text(dock,"Navigation Caption",96,12,270,22,14,theme.Gold).text="VIAJERO · ACCESOS";
            view.Context=Text(dock,"Mode Context",380,14,842,36,15,theme.Gold);
            view.Hint=Text(dock,"Pointer Hint",380,47,610,43,16,theme.Text);
            var message=canvas.Find("Message Bar");Rect(message,260,550,760,40);Panel(message,theme.InsetSprite);
            var explore=canvas.GetComponent<ExplorationHud>();var interact=explore.ActionButton;interact.transform.SetParent(dock,false);Rect(interact.transform,1024,46,198,38);Style(interact);
            using(var so=new SerializedObject(explore))view.Feedback=(Text)so.FindProperty("log").objectReferenceValue;
            Rect(view.Feedback.transform,16,5,728,30);view.Feedback.fontSize=14;view.Feedback.alignment=TextAnchor.MiddleLeft;
            var help=canvas.Find("Controls");Rect(help,16,342,338,240);Panel(help,theme.InsetSprite);view.HelpPanel=help.gameObject;
            var helpText=help.GetComponentInChildren<Text>();Rect(helpText.transform,20,18,298,204);helpText.fontSize=16;
            var helpButton=NewButton(header,"Ayuda",1034,14,78,38,"help");
            var window=PanelNew(context,"Window",190,112,900,472,theme.PanelSprite);view.Window=window.gameObject;
            view.WindowTitle=Text(window,"Window Title",32,20,790,32,24,theme.Gold);view.WindowTitle.fontStyle=FontStyle.Bold;
            NewButton(window,"Cerrar",832,16,42,38,"close").GetComponentInChildren<Text>().text="×";
            // A quiet parchment sheet carries dense character/inventory data.
            var paper=PanelNew(window,"Information Paper",242,74,626,316,theme.PaperSprite);paper.GetComponent<Image>().raycastTarget=false;view.Paper=(RectTransform)paper;
            using(var serialized=new SerializedObject(hud))view.Body=(Text)serialized.FindProperty("detail").objectReferenceValue;
            view.Body.transform.SetParent(window,false);Rect(view.Body.transform,260,126,600,142);view.Body.supportRichText=false;view.Body.fontSize=18;view.Body.color=theme.Ink;
            view.Equipment=Text(window,"Equipment",260,286,590,100,16,theme.Ink);
            var namePanel=canvas.Find("Character Creation");namePanel.SetParent(window,false);Rect(namePanel,260,86,580,34);namePanel.GetComponent<Image>().enabled=false;view.NamePanel=namePanel.gameObject;
            var input=namePanel.GetComponentInChildren<InputField>(true);Rect(input.transform,0,0,580,34);Panel(input.transform,theme.ButtonSprite);input.textComponent.color=theme.Text;
            PlayablePrototypeBuilder.Stretch(input.textComponent.rectTransform,12,4);
            var refCard=context.GetComponentInChildren<ClassReferenceCard>(true);refCard.transform.SetParent(window,false);Rect(refCard.transform,32,84,190,306);
            refCard.UsePortraitCaption();var photo=refCard.Portrait;Rect(photo.transform,4,0,174,238);photo.GetComponent<AspectRatioFitter>().aspectMode=AspectRatioFitter.AspectMode.HeightControlsWidth;
            Rect(refCard.Information.transform,0,254,192,48);refCard.Information.fontSize=14;refCard.Information.color=theme.Text;refCard.Information.alignment=TextAnchor.UpperCenter;view.Reference=refCard;
            view.Inventory=Inventory(window);
            var modes=canvas.Find("Modes");modes.SetParent(window,false);Rect(modes,32,248,820,94);modes.GetComponent<Image>().enabled=false;view.ModeMenu=modes.gameObject;
            var modeButtons=modes.GetComponentsInChildren<Button>(true);
            for(var i=0;i<modeButtons.Length;i++){Rect(modeButtons[i].transform,(i%4)*202,0,192,42);Style(modeButtons[i]);}
            var grid=context.Find("Action Grid");view.ActionGrid=(RectTransform)grid;view.ActionLayout=grid.GetComponent<GridLayoutGroup>();view.ActionLayout.spacing=new Vector2(8,8);
            foreach(var b in grid.GetComponentsInChildren<Button>(true))Style(b);
            NewButton(dock,"Return to Exploration",96,44,268,38,"back").GetComponentInChildren<Text>().text="Volver a explorar";
            // Highlight shared templates without modifying the historical UI prefabs.
            SaveTemplates();
            hud.ConfigureCompact(view,extra.ToArray());hud.enabled=true;EditorUtility.SetDirty(hud);EditorUtility.SetDirty(view);EditorUtility.SetDirty(refCard);
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics||MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath)!=nav)throw new InvalidOperationException("C02 changed physics/nav.");
            AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            Refine();
            Debug.Log("C02_CREATED: shared skin, compact HUD, closable windows, class portraits and inventory selection; physics/nav preserved.");
        }
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(PlayablePrototypeBuilder.ScenePath);var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var view=hud.CompactView;theme=view.Theme;
            var physics=EnvironmentPhysicsSnapshot.Capture(scene);
            // Added prefab children keep their own instance overrides after unpacking an outer HUD.
            // Unpack this scene's UI only; source/historical prefabs remain intact.
            foreach(var t in hud.GetComponentsInChildren<Transform>(true))
                if(PrefabUtility.IsAnyPrefabInstanceRoot(t.gameObject))PrefabUtility.UnpackPrefabInstance(t.gameObject,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            using(var so=new SerializedObject(hud))
            {
                var array=so.FindProperty("bindings");
                for(var i=0;i<array.arraySize;i++)
                {
                    var entry=array.GetArrayElementAtIndex(i);var id=entry.FindPropertyRelative("Id").stringValue;var b=(Button)entry.FindPropertyRelative("Button").objectReferenceValue;
                    if(b==null)throw new InvalidOperationException("Missing binding: "+id);
                    b.transform.localScale=Vector3.one;Style(b);
                    if(id=="page-session"){b.name="Partida";Rect(b.transform,1120,14,108,38);b.GetComponentInChildren<Text>().text="Menú";}
                    if(id=="help"){b.name="Ayuda";Rect(b.transform,1034,14,78,38);}
                    if(id=="close"){b.name="Cerrar";Rect(b.transform,832,16,42,38);b.GetComponentInChildren<Text>().text="×";}
                    if(id=="back"){b.name="Return to Exploration";Rect(b.transform,96,44,268,38);b.GetComponentInChildren<Text>().text="Volver a explorar";}
                    if(id.StartsWith("item-")){var index=id=="item-ration"?0:id=="item-training-sword"?1:2;b.name="Item "+id.Substring(5);Rect(b.transform,0,index*92,190,82);}
                }
            }
            view.Context.rectTransform.sizeDelta=new Vector2(842,36);view.Context.fontSize=15;
            hud.transform.Find("Context Actions/Action Grid").SetAsLastSibling();
            hud.transform.Find("Context Actions").SetAsLastSibling();
            var feedback=view.Feedback.transform.parent;feedback.SetAsLastSibling();Rect(view.Feedback.transform,16,3,728,34);
            view.Catalog.FindEntry("monk").ReferenceCaption="Ilustración conceptual\nMonje · provisional";EditorUtility.SetDirty(view.Catalog);
            hud.transform.Find("Title Bar").GetComponentInChildren<Text>().text="CRÓNICAS\nDE AETHERNIA";
            hud.transform.Find("C02 Action Dock/Navigation Caption").GetComponent<Text>().text="VIAJERO · ACCESOS";
            hud.transform.Find("C02 Action Dock/Exploration Pages/Resumen").GetComponentInChildren<Text>().text="Registro";
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics)throw new InvalidOperationException("UI refinement changed physics");
            EditorUtility.SetDirty(view);EditorUtility.SetDirty(hud);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("C02_REFINED: nested UI prefab overrides made explicit; source assets preserved.");
        }
        private static void Skin()
        {
            var path=Root+"/Textures/MedievalSkinAtlas.png";AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.mipmapEnabled=false;importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var w=texture.width/2f;var h=texture.height/2f;
            theme.PanelSprite=Sprite(texture,"Wood",new Rect(0,h,w,h));theme.PaperSprite=Sprite(texture,"Paper",new Rect(w,h,w,h));
            theme.ButtonSprite=Sprite(texture,"Iron",new Rect(0,0,w,h));theme.InsetSprite=Sprite(texture,"Leather",new Rect(w,0,w,h));
            theme.Gold=new Color32(225,194,136,255);theme.Text=new Color32(241,231,208,255);theme.BodyFontSize=17;EditorUtility.SetDirty(theme);
        }
        private static Sprite Sprite(Texture2D texture,string name,Rect rect)
        {var s=UnityEngine.Sprite.Create(texture,rect,Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(38,38,38,38));s.name=name;AssetDatabase.CreateAsset(s,Root+"/Sprites/"+name+".asset");return s;}
        private static void Portraits(CharacterVisualCatalog catalog)
        {
            foreach(var entry in catalog.Entries)
            {
                if(entry.ClassId=="monk"){entry.ReferenceCaption="Ilustración conceptual\nMonje · provisional";continue;}
                var model=Object.Instantiate(entry.Prefab,new Vector3(0,-100,0),Quaternion.identity);GameObject cameraObject=null,lightObject=null;RenderTexture target=null;Texture2D pixels=null;var old=RenderTexture.active;
                try
                {
                    foreach(var t in model.GetComponentsInChildren<Transform>(true))t.gameObject.layer=30;
                    cameraObject=new GameObject("C02 Portrait Authoring Camera",typeof(Camera));var camera=cameraObject.GetComponent<Camera>();camera.cullingMask=1<<30;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.07f,.06f,.045f,1);camera.orthographic=true;camera.orthographicSize=.98f;camera.nearClipPlane=.1f;camera.farClipPlane=20;camera.transform.position=new Vector3(2,-98.8f,3);camera.transform.LookAt(new Vector3(0,-99.12f,0));
                    lightObject=new GameObject("C02 Portrait Light",typeof(Light));var light=lightObject.GetComponent<Light>();light.type=LightType.Directional;light.intensity=2;light.cullingMask=1<<30;light.transform.rotation=Quaternion.Euler(40,210,0);
                    target=new RenderTexture(256,320,24);camera.targetTexture=target;camera.Render();RenderTexture.active=target;pixels=new Texture2D(256,320,TextureFormat.RGB24,false);pixels.ReadPixels(new Rect(0,0,256,320),0,0);pixels.Apply();var path=Root+"/Portraits/"+entry.ClassId+".png";File.WriteAllBytes(path,pixels.EncodeToPNG());
                    AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var imp=(TextureImporter)AssetImporter.GetAtPath(path);imp.mipmapEnabled=false;imp.npotScale=TextureImporterNPOTScale.None;imp.SaveAndReimport();entry.ReferenceArt=AssetDatabase.LoadAssetAtPath<Texture2D>(path);entry.ReferenceUv=new Rect(0,0,1,1);entry.ReferenceCaption="Modelo provisional\n"+(entry.ClassId=="mage"?"Mago":"Guerrero");
                }
                finally{RenderTexture.active=old;if(cameraObject!=null)Object.DestroyImmediate(cameraObject);if(lightObject!=null)Object.DestroyImmediate(lightObject);Object.DestroyImmediate(model);if(target!=null)Object.DestroyImmediate(target);if(pixels!=null)Object.DestroyImmediate(pixels);}
            }
            EditorUtility.SetDirty(catalog);
        }
        private static InventoryPanelView Inventory(Transform window)
        {
            var panel=new GameObject("Inventory Selection",typeof(RectTransform),typeof(InventoryPanelView));panel.transform.SetParent(window,false);Rect(panel.transform,32,90,820,292);
            var view=panel.GetComponent<InventoryPanelView>();view.Theme=theme;view.Slots=new Button[3];view.Labels=new Text[3];var ids=new[]{"ration","training-sword","cloth"};
            for(var i=0;i<3;i++) {view.Slots[i]=NewButton(panel.transform,"Item "+ids[i],0,i*92,190,82,"item-"+ids[i]);view.Labels[i]=view.Slots[i].GetComponentInChildren<Text>();view.Labels[i].fontSize=16;}
            view.Description=Text(panel.transform,"Item Description",228,0,580,280,18,theme.Ink);return view;
        }
        private static void SaveTemplates()
        {
            var panel=PanelNew(null,"MedievalWindow",0,0,900,472,theme.PanelSprite);PrefabUtility.SaveAsPrefabAsset(panel.gameObject,"Assets/02_Prefabs/UI/C02/MedievalWindow.prefab");Object.DestroyImmediate(panel.gameObject);
            var button=new GameObject("ActionSlot",typeof(RectTransform),typeof(Image),typeof(Button));Rect(button.transform,0,0,128,38);Style(button.GetComponent<Button>());var text=Text(button.transform,"Label",4,4,120,30,16,theme.Text);text.alignment=TextAnchor.MiddleCenter;text.text="Acción";PrefabUtility.SaveAsPrefabAsset(button,"Assets/02_Prefabs/UI/C02/ActionSlot.prefab");Object.DestroyImmediate(button);
        }
        private static Button NewButton(Transform parent,string name,float x,float y,float width,float height,string id)
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/UI/MedievalButton.prefab");var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab,parent);go.name=name;Rect(go.transform,x,y,width,height);var b=go.GetComponent<Button>();Style(b);b.GetComponentInChildren<Text>().text=name;extra.Add(new HudBinding{Id=id,Button=b});return b;
        }
        private static void Style(Button b)
        {Panel(b.transform,theme.ButtonSprite);var colors=b.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1,.87f,.65f);colors.pressedColor=new Color(.65f,.50f,.30f);colors.disabledColor=new Color(.5f,.5f,.5f);colors.selectedColor=new Color(1,.9f,.7f);b.colors=colors;var t=b.GetComponentInChildren<Text>(true);if(t!=null){t.font=theme.Font;t.fontSize=16;t.color=theme.Text;t.raycastTarget=false;PlayablePrototypeBuilder.Stretch(t.rectTransform,4,3);}}
        private static void Panel(Transform transform,Sprite sprite)
        {var image=transform.GetComponent<Image>();image.enabled=true;image.sprite=sprite;image.type=Image.Type.Sliced;image.pixelsPerUnitMultiplier=3;image.color=Color.white;}
        private static Transform PanelNew(Transform parent,string name,float x,float y,float width,float height,Sprite sprite)
        {var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);Rect(go.transform,x,y,width,height);Panel(go.transform,sprite);return go.transform;}
        private static Image CreateImage(Transform parent,string name,float x,float y,float width,float height,Sprite sprite,Color color)
        {var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);Rect(go.transform,x,y,width,height);var im=go.GetComponent<Image>();im.sprite=sprite;im.color=color;im.raycastTarget=false;return im;}
        private static RawImage Raw(Transform parent,string name,float x,float y,float width,float height)
        {var go=new GameObject(name,typeof(RectTransform),typeof(RawImage));go.transform.SetParent(parent,false);Rect(go.transform,x,y,width,height);var im=go.GetComponent<RawImage>();im.raycastTarget=false;return im;}
        private static Text Text(Transform parent,string name,float x,float y,float width,float height,int size,Color color)
        {var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);Rect(go.transform,x,y,width,height);var t=go.GetComponent<Text>();t.font=theme.Font;t.fontSize=size;t.color=color;t.raycastTarget=false;t.supportRichText=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;return t;}
        private static void Rect(Transform t,float x,float y,float width,float height)
        {var r=(RectTransform)t;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(width,height);}
        private static void Stretch(RectTransform r,float x,float y){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(x,y);r.offsetMax=new Vector2(-x,-y);}
    }
}
