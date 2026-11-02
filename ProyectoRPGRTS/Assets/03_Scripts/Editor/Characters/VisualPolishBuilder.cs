using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class VisualPolishBuilder
    {
        public const string Root="Assets/12_VisualPolish";
        public const string ScenePath="Assets/01_Scenes/AetherniaVisualPolish.unity";
        private static void Folder(string path){if(AssetDatabase.IsValidFolder(path))return;var parent=Path.GetDirectoryName(path).Replace('\\','/');Folder(parent);AssetDatabase.CreateFolder(parent,Path.GetFileName(path));}
        public static void Create()
        {
            if(File.Exists(ScenePath))throw new InvalidOperationException("E16 ya existe: no regenerar.");
            var scene=EditorSceneManager.OpenScene(ExpeditionBuilder.ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var old=flow.Front.Theme;
            Folder(Root+"/UI");Folder(Root+"/Portraits");Folder(Root+"/Prefabs");Folder(Root+"/Materials");
            var profile=ScriptableObject.CreateInstance<VisualPresentationProfile>();AssetDatabase.CreateAsset(profile,Root+"/Presentation.asset");
            Atlas(profile);
            var theme=Object.Instantiate(old);theme.name="E16 Medieval restrained";theme.PanelSprite=profile.Panel;theme.PaperSprite=profile.Paper;theme.ButtonSprite=profile.Button;theme.InsetSprite=profile.Panel;theme.Gold=profile.Gold;theme.Text=profile.Text;theme.ButtonFontSize=16;theme.BodyFontSize=18;
            AssetDatabase.CreateAsset(theme,Root+"/UI/Theme.asset");profile.Theme=theme;
            var catalog=flow.Front.Catalog;profile.Classes=catalog.Entries.Select(e=>Art(e)).ToArray();
            var ui=new DemoWidgets(flow.Front.Font,profile.Button,profile);
            var button=ui.Button(null,"Button normal","Acción",0,0,240,44,null);profile.ButtonPrefab=PrefabUtility.SaveAsPrefabAsset(button.gameObject,Root+"/Prefabs/ButtonNormal.prefab");Object.DestroyImmediate(button.gameObject);
            var primary=ui.Button(null,"Button primary","Continuar",0,0,240,44,null);ui.Primary(primary);Save(primary.gameObject,"ButtonPrimary");
            var secondary=ui.Button(null,"Button secondary","Volver",0,0,240,44,null);Save(secondary.gameObject,"ButtonSecondary");
            Card(profile,ui);
            foreach(var kind in new[]{"PanelLeather","PanelParchment","PortraitFrame","Tooltip","DialogNotice"})
            {
                var node=DemoWidgets.Rect(null,kind,0,0,kind=="PortraitFrame"?240:480,kind=="PortraitFrame"?320:140);var img=ui.Surface(node,Color.white,kind=="PanelParchment");if(kind=="PortraitFrame")img.sprite=profile.Frame;
                if(kind=="Tooltip"||kind=="DialogNotice")ui.Text(node,"Message","Texto contextual",16,16,448,108,18);Save(node.gameObject,kind);
            }
            flow.Front.Presentation=profile;flow.Front.Theme=theme;PrefabUtility.RecordPrefabInstancePropertyModifications(flow.Front);
            flow.ExistingHud.CompactView.Theme=theme;
            foreach(var image in flow.ExistingHud.GetComponentsInChildren<Image>(true))
            {
                if(image.type==Image.Type.Filled)continue;var s=image.sprite;if(s==old.PanelSprite||s==old.InsetSprite){image.sprite=profile.Panel;image.color=Color.white;}
                else if(s==old.PaperSprite){image.sprite=profile.Paper;image.color=Color.white;}
                else if(s==old.ButtonSprite){image.sprite=profile.Button;if(image.type!=Image.Type.Filled)image.color=Color.white;}
            }
            foreach(var bridge in flow.ExistingHud.GetComponentsInChildren<LegacySdfPresenter>(true)){bridge.MinimumSize=14;bridge.Target.extraPadding=true;}
            flow.ExistingHud.GetComponent<Canvas>().pixelPerfect=true;
            Environment(flow);
            EditorUtility.SetDirty(profile);EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Debug.Log("E16_CREATED: optional UI profile, shared atlas/prefabs, three existing sheets, regional URP atmosphere. Original scenes/colliders/nav preserved; QA pending.");
        }
        private static ClassPresentation Art(CharacterVisualCatalog.Entry entry)
        {
            var id=entry.ClassId;var texture=entry.ConceptSheet;var x=id=="monk"?166:id=="warrior"?150:120;
            var rect=new Rect(x,texture.height-510,280,475);var sprite=Sprite.Create(texture,rect,new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);sprite.name=id+"_Figure";AssetDatabase.CreateAsset(sprite,Root+"/Portraits/"+id+"_Figure.asset");
            return new ClassPresentation{Id=id,Title=entry.DisplayTitle,Portrait=entry.PortraitSprite,Figure=sprite,Role=id=="monk"?"Disciplina · movilidad · combate cercano":id=="warrior"?"Firmeza · protección · combate frontal":"Sabiduría · misterio · poder a distancia",Accent=id=="monk"?new Color(.65f,.35f,.18f):id=="warrior"?new Color(.35f,.48f,.54f):new Color(.43f,.36f,.51f)};
        }
        private static void Card(VisualPresentationProfile profile,DemoWidgets ui)
        {
            var b=ui.Button(null,"Class card","",0,0,368,350,null);b.GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);var view=b.gameObject.AddComponent<ClassCardView>();view.Button=b;
            view.Title=ui.Text(b.transform,"Class title","Clase",16,12,336,30,25);view.Title.alignment=TextAlignmentOptions.Center;view.Title.color=profile.Gold;
            var picture=DemoWidgets.Stretch(b.transform,"Portrait mount");picture.anchorMin=new Vector2(0,.22f);picture.anchorMax=new Vector2(1,.84f);picture.offsetMin=new Vector2(16,0);picture.offsetMax=new Vector2(-16,0);DemoWidgets.Fill(picture,new Color(.67f,.66f,.62f),false);
            var image=DemoWidgets.Stretch(picture,"Figure");image.pivot=Vector2.one*.5f;image.offsetMin=new Vector2(6,3);image.offsetMax=new Vector2(-6,-3);view.Figure=image.gameObject.AddComponent<Image>();view.Figure.preserveAspect=true;view.Figure.raycastTarget=false;
            view.Role=ui.Text(b.transform,"Class role","",16,282,336,32,16);view.Role.alignment=TextAlignmentOptions.Center;
            view.Selection=ui.Text(b.transform,"Selected class","",16,322,336,22,15);view.Selection.alignment=TextAlignmentOptions.Center;
            foreach(var text in new[]{view.Role,view.Selection}){var r=text.rectTransform;r.anchorMin=r.anchorMax=r.pivot=Vector2.zero;r.anchoredPosition=new Vector2(16,text==view.Role?34:6);}
            var border=DemoWidgets.Stretch(b.transform,"Selected frame");view.Outline=border.gameObject.AddComponent<Image>();view.Outline.sprite=profile.Frame;view.Outline.type=Image.Type.Sliced;view.Outline.raycastTarget=false;
            profile.CardPrefab=PrefabUtility.SaveAsPrefabAsset(b.gameObject,Root+"/Prefabs/ClassCard.prefab");Object.DestroyImmediate(b.gameObject);
        }
        private static void Save(GameObject node,string name){PrefabUtility.SaveAsPrefabAsset(node,Root+"/Prefabs/"+name+".prefab");Object.DestroyImmediate(node);}
        private static void Atlas(VisualPresentationProfile p)
        {
            var texture=new Texture2D(320,64,TextureFormat.RGBA32,false){name="E16 shared surfaces",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
            for(var cell=0;cell<5;cell++)for(var y=0;y<64;y++)for(var x=0;x<64;x++)
            {
                var edge=Mathf.Min(x,y,63-x,63-y);var color=cell==1?new Color(.78f,.73f,.63f):cell==3?new Color(.65f,.52f,.31f):new Color(.085f,.095f,.095f);
                var grain=(Mathf.Sin(x*3.1f+y*1.7f)+Mathf.Sin(y*2.3f))*.007f;color+=new Color(grain,grain,grain,0);
                if(edge<2)color=new Color(.43f,.35f,.23f);else if(edge==3)color=new Color(.22f,.20f,.16f);
                if(cell==4)color=edge<3?p.Gold:Color.clear;texture.SetPixel(cell*64+x,y,color);
            }
            texture.Apply();AssetDatabase.CreateAsset(texture,Root+"/UI/SurfaceAtlas.asset");
            var sprites=new Sprite[5];for(var i=0;i<5;i++){sprites[i]=Sprite.Create(texture,new Rect(i*64,0,64,64),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(8,8,8,8));sprites[i].name=new[]{"Leather","Parchment","Button","Primary","Frame"}[i];AssetDatabase.AddObjectToAsset(sprites[i],texture);}
            p.Panel=sprites[0];p.Paper=sprites[1];p.Button=sprites[2];p.Primary=sprites[3];p.Frame=sprites[4];
        }
        private static void Environment(DemoFlow flow)
        {
            var root=new GameObject("E16 visual environment · no physics");var ruins=GameObject.Find("E14 Ruinas del Umbral · provisional");
            var stone=Material("DungeonStone","GreyStone",new Color(.58f,.64f,.68f),.13f);var floor=Material("DungeonFloor","GreyStone",new Color(.65f,.67f,.63f),.08f);
            foreach(var renderer in ruins.GetComponentsInChildren<Renderer>())if(renderer.sharedMaterial!=null&&renderer.sharedMaterial.name=="GreyStone")renderer.sharedMaterial=renderer.name.Contains("floor")?floor:stone;
            var oldGrass=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/Grass.mat");var grass=Material("GrassWarm","Grass",new Color(.80f,.84f,.68f),.08f);
            foreach(var renderer in Object.FindObjectsByType<MeshRenderer>())if(renderer.sharedMaterial==oldGrass)renderer.sharedMaterial=grass;
            foreach(var light in ruins.GetComponentsInChildren<Light>())
            {light.range=9;light.intensity=2.2f;light.shadows=LightShadows.None;light.gameObject.AddComponent<TorchGlow>().Source=light;var prop=Prop(root.transform,"Props/Torch_170cm",new Vector3(light.transform.position.x,.3f,light.transform.position.z));foreach(var r in light.GetComponentsInChildren<Renderer>())r.enabled=false;}
            foreach(var pt in new[]{new Vector3(-17,0,2),new Vector3(-16,0,3.3f),new Vector3(5,0,5.8f),new Vector3(6,0,6.2f),new Vector3(15,0,4.8f)})Prop(root.transform,"Vegetation/Shrub_90cm",pt);
            foreach(var pt in new[]{new Vector3(69.5f,0,6),new Vector3(90.5f,0,-6)})Prop(root.transform,"Props/Runestone_80cm",pt);
            foreach(var pt in new[]{new Vector3(72,0,6),new Vector3(88,0,6)})Prop(root.transform,"Props/SupplySack_75cm",pt);
            foreach(var point in Object.FindObjectsByType<InteractionPoint>())if(point.FactId.StartsWith("umbral-")||point.FactId.StartsWith("territory-"))
            {
                foreach(var renderer in point.GetComponentsInChildren<MeshRenderer>())if(renderer.gameObject.name=="Cube")renderer.enabled=false;
                var prop=Prop(root.transform,"Props/Runestone_80cm",point.transform.position);prop.name="E16 landmark "+point.FactId;
                var label=point.GetComponentInChildren<TMP_Text>();if(label!=null){label.fontSize=3;label.rectTransform.sizeDelta=new Vector2(6,2);}
            }
            // Dos señales de tela al margen de la zona de despliegue, sin tocar rutas.
            var blue=AssetDatabase.LoadAssetAtPath<Material>("Assets/07_Characters/ProductionProvisional/Materials/BlueGray.mat");var red=AssetDatabase.LoadAssetAtPath<Material>("Assets/07_Characters/ProductionProvisional/Materials/Plum.mat");
            Banner(root.transform,new Vector3(-13,0,18.5f),blue);Banner(root.transform,new Vector3(-5,0,18.5f),red);
            var atmosphere=root.AddComponent<RegionalAtmosphere>();atmosphere.Flow=flow;atmosphere.Sun=Object.FindObjectsByType<Light>().First(l=>l.type==LightType.Directional);atmosphere.Sun.shadows=LightShadows.Soft;atmosphere.Sun.shadowStrength=.72f;
            var volume=root.AddComponent<Volume>();volume.isGlobal=true;volume.priority=5;var grade=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(grade,Root+"/Grade.asset");var color=grade.Add<ColorAdjustments>(true);color.saturation.Override(-6);color.contrast.Override(7);color.postExposure.Override(-.05f);var bloom=grade.Add<Bloom>(true);bloom.intensity.Override(.06f);bloom.threshold.Override(1.25f);var vignette=grade.Add<Vignette>(true);vignette.intensity.Override(.12f);
            foreach(var component in grade.components)AssetDatabase.AddObjectToAsset(component,grade);volume.sharedProfile=grade;atmosphere.Grade=volume;
            FillLight(flow,atmosphere);
            var camera=Camera.main;camera.backgroundColor=new Color(.055f,.070f,.085f);var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;data.antialiasing=AntialiasingMode.FastApproximateAntialiasing;
        }
        private static Material Material(string name,string source,Color tint,float smoothness)
        {var material=Object.Instantiate(AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/"+source+".mat"));material.name=name;material.SetColor("_BaseColor",tint);material.SetFloat("_Smoothness",smoothness);AssetDatabase.CreateAsset(material,Root+"/Materials/"+name+".mat");return material;}
        private static GameObject Prop(Transform parent,string path,Vector3 at)
        {var node=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/Environment/VillageE07/"+path+".prefab"));node.transform.SetParent(parent,true);node.transform.position=at;foreach(var c in node.GetComponentsInChildren<Collider>())Object.DestroyImmediate(c);foreach(var l in node.GetComponentsInChildren<Light>())Object.DestroyImmediate(l);return node;}
        private static void Banner(Transform parent,Vector3 at,Material cloth)
        {var pole=GameObject.CreatePrimitive(PrimitiveType.Cylinder);pole.name="E16 field standard";pole.transform.SetParent(parent);pole.transform.position=at+Vector3.up*1.3f;pole.transform.localScale=new Vector3(.07f,1.3f,.07f);Object.DestroyImmediate(pole.GetComponent<Collider>());var flag=GameObject.CreatePrimitive(PrimitiveType.Cube);flag.transform.SetParent(pole.transform.parent);flag.transform.position=at+new Vector3(.43f,2,0);flag.transform.localScale=new Vector3(.8f,1,.035f);flag.GetComponent<Renderer>().sharedMaterial=cloth;Object.DestroyImmediate(flag.GetComponent<Collider>());}
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();
            foreach(var bridge in flow.ExistingHud.GetComponentsInChildren<LegacySdfPresenter>(true))bridge.MinimumSize=14;
            var path=Root+"/Prefabs/ClassCard.prefab";var node=PrefabUtility.LoadPrefabContents(path);
            try{var image=node.GetComponent<ClassCardView>().Figure.rectTransform;image.pivot=Vector2.one*.5f;image.offsetMin=new Vector2(6,3);image.offsetMax=new Vector2(-6,-3);PrefabUtility.SaveAsPrefabAsset(node,path);}finally{PrefabUtility.UnloadPrefabContents(node);}
            flow.ExistingHud.CompactView.HealthFill.sprite=null;
            FillLight(flow,Object.FindAnyObjectByType<RegionalAtmosphere>());
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("E16_REFINED: portrait centering and legacy navigation font sizes, no gameplay changes.");
        }
        private static void FillLight(DemoFlow flow,RegionalAtmosphere atmosphere)
        {
            if(atmosphere.CharacterFill!=null)return;var node=new GameObject("E16 expedition fill");node.transform.SetParent(flow.Director.Hero.transform,false);node.transform.localPosition=Vector3.up*2.8f;var light=node.AddComponent<Light>();light.type=LightType.Point;light.range=8;light.intensity=1.4f;light.color=new Color(.73f,.81f,.92f);light.shadows=LightShadows.None;light.enabled=false;atmosphere.CharacterFill=light;
        }
        public static void Build()
        {Directory.CreateDirectory("Builds/VisualPolishWindows");var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/VisualPolishWindows/Aethernia.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});if(r.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("E16 build failed");File.WriteAllText("Logs/E16/build.txt","PASS; bytes="+r.summary.totalSize+"; seconds="+r.summary.totalTime.TotalSeconds);}
    }
}
