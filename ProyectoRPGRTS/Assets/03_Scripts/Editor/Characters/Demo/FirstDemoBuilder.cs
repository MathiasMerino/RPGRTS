using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class FirstDemoBuilder
    {
        public const string Root="Assets/10_Demo";
        public const string ScenePath="Assets/01_Scenes/AetherniaFirstDemo.unity";
        public const string FontPath=Root+"/Fonts/AetherniaBodySDF.asset";
        public static void Create()
        {
            if(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)!=null)throw new InvalidOperationException("La demo ya existe: no regenerar sobre escenas/recursos existentes.");
            var fontSource="Assets/TextMesh Pro/Fonts/LiberationSans.ttf";
            if(!File.Exists(fontSource))
            {
                var package=UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMP_Text).Assembly);
                AssetDatabase.ImportPackage(Path.Combine(package.resolvedPath,"Package Resources/TMP Essential Resources.unitypackage"),false);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            Folder(Root+"/Fonts");Folder(Root+"/Prefabs");
            var font=TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>(fontSource),90,9,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,true);
            font.name="Aethernia Body SDF";font.TryAddCharacters(new string(Enumerable.Range(32,224).Select(x=>(char)x).ToArray())+"¿¡áéíóúñüÁÉÍÓÚÑÜ—–−×·→",out var missing);
            AssetDatabase.CreateAsset(font,FontPath);AssetDatabase.AddObjectToAsset(font.material,font);
            foreach(var atlas in font.atlasTextures)AssetDatabase.AddObjectToAsset(atlas,font);
            EditorUtility.SetDirty(font);
            var content=ScriptableObject.CreateInstance<DemoContent>();AssetDatabase.CreateAsset(content,Root+"/DemoContent.asset");
            var scene=EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var hud=Object.FindAnyObjectByType<VerticalSliceHud>();
            var root=new GameObject("E10-E13 Demo Flow");var flow=root.AddComponent<DemoFlow>();flow.Director=d;flow.ExistingHud=hud;flow.CameraRig=Camera.main.GetComponent<IsometricCameraRig>();flow.Content=content;
            var ui=new GameObject("Demo Front End",typeof(RectTransform));ui.layer=5;
            var front=ui.AddComponent<DemoFrontEnd>();front.Font=font;front.Theme=hud.CompactView.Theme;front.Catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AnimatedCharacterBuilder.Root+"/ClassVisuals.asset");front.Audio=Object.FindAnyObjectByType<CombatAudioPlayer>();
            var prefab=PrefabUtility.SaveAsPrefabAsset(ui,Root+"/Prefabs/DemoFrontEnd.prefab");Object.DestroyImmediate(ui);
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab);front=instance.GetComponent<DemoFrontEnd>();front.Flow=flow;front.Audio=Object.FindAnyObjectByType<CombatAudioPlayer>();flow.Front=front;PrefabUtility.RecordPrefabInstancePropertyModifications(front);
            foreach(var text in scene.GetRootGameObjects().SelectMany(go=>go.GetComponentsInChildren<Text>(true)).ToArray())
            {
                // El campo legado mantiene su caret; el creador nuevo usa TMP_InputField real.
                if(text.GetComponentInParent<InputField>()!=null)continue;
                var child=DemoWidgets.Stretch(text.transform,"SDF presentation");var target=child.gameObject.AddComponent<TextMeshProUGUI>();target.font=font;target.fontSize=Mathf.Max(17,text.fontSize);target.text=text.text;target.raycastTarget=false;target.color=text.color;
                target.textWrappingMode=text.horizontalOverflow==HorizontalWrapMode.Wrap?TextWrappingModes.Normal:TextWrappingModes.NoWrap;target.overflowMode=TextOverflowModes.Truncate;
                var bridge=text.gameObject.AddComponent<LegacySdfPresenter>();bridge.Source=text;bridge.Target=target;
            }
            EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Directory.CreateDirectory("Logs/E10");File.WriteAllText("Logs/E10/authoring.txt","Created new scene/prefab/content and SDF2048; source LiberationSans/OFL bundled; missing optional glyphs="+missing+". Original scene/prefabs preserved. Technical/human acceptance pending.");
            Debug.Log("FIRST_DEMO_CREATED: new scene; shared E07/E08/E09 and native TMP SDF; originals preserved.");
            Controls();
        }
        private static void Folder(string path){if(AssetDatabase.IsValidFolder(path))return;Folder(Path.GetDirectoryName(path).Replace('\\','/'));AssetDatabase.CreateFolder(Path.GetDirectoryName(path).Replace('\\','/'),Path.GetFileName(path));}
        public static void Controls()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();
            flow.Front.Audio=Object.FindAnyObjectByType<CombatAudioPlayer>();if(flow.Front.Audio==null)throw new InvalidOperationException("Falta reproductor de audio compartido E09");PrefabUtility.RecordPrefabInstancePropertyModifications(flow.Front);
            var motor=flow.GetComponent<DemoHeroMotor>();if(motor==null)motor=flow.gameObject.AddComponent<DemoHeroMotor>();motor.Flow=flow;
            var guidance=flow.GetComponent<DemoGuidance>();if(guidance==null)guidance=flow.gameObject.AddComponent<DemoGuidance>();guidance.Flow=flow;
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);Debug.Log("DEMO_CONTROLS_BOUND: direct hero/camera opt-in, original scenes preserved.");
        }
        public static void Build()
        {
            Directory.CreateDirectory("Builds/FirstDemoWindows");var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/FirstDemoWindows/Aethernia.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
            if(report.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("First demo build: "+report.summary.result);
            File.WriteAllText("Logs/BuildFirstDemo-summary.txt","Succeeded; bytes="+report.summary.totalSize+"; seconds="+report.summary.totalTime.TotalSeconds);
        }
        public static void Rts()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();
            var path=Root+"/RtsRules.asset";var rules=AssetDatabase.LoadAssetAtPath<RtsDemoRules>(path);if(rules==null){rules=ScriptableObject.CreateInstance<RtsDemoRules>();AssetDatabase.CreateAsset(rules,path);}
            flow.Director.Rts.ConfigureDemo(rules,flow.Front.Catalog,Object.FindAnyObjectByType<CombatFeedbackPresenter>());
            var input=flow.GetComponent<DemoRtsInput>();if(input==null)input=flow.gameObject.AddComponent<DemoRtsInput>();input.Flow=flow;
            var overlay=flow.GetComponent<DemoRtsOverlay>();if(overlay==null)overlay=flow.gameObject.AddComponent<DemoRtsOverlay>();overlay.Flow=flow;
            if(flow.Patrol==null)
            {
                var npc=Object.FindAnyObjectByType<LostCargoController>().Npc.gameObject;var source=PrefabUtility.GetCorrespondingObjectFromSource(npc);var marker=(GameObject)PrefabUtility.InstantiatePrefab(source);
                marker.name="Capitan de patrulla · demo";marker.transform.position=new Vector3(-12,0,8);flow.Patrol=marker.GetComponent<InteractionPoint>();flow.Patrol.Configure("demo-patrol","Capitán de patrulla");
                foreach(var text in marker.GetComponentsInChildren<Text>(true))text.text="Capitán de patrulla · E";
                PrefabUtility.RecordPrefabInstancePropertyModifications(flow.Patrol);PrefabUtility.RecordPrefabInstancePropertyModifications(marker.transform);
            }
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("DEMO_RTS_BOUND: shared hero, role data, selection/commands and world interaction; validation pending.");
        }
        public static void Settlement()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();Folder(Root+"/Prefabs/Buildings");
            var rules=AssetDatabase.LoadAssetAtPath<SettlementRules>(Root+"/SettlementRules.asset");if(rules==null){rules=ScriptableObject.CreateInstance<SettlementRules>();AssetDatabase.CreateAsset(rules,Root+"/SettlementRules.asset");}
            var original=(GameObject)new SerializedObject(flow.Director.Construction).FindProperty("prefab").objectReferenceValue;
            var table=rules.Blueprints.Select(p=>new BuildingPrefabBinding{Id=p.Id,Prefab=CampPrefab(original,p.Id)}).ToArray();
            flow.Session.ConfigureSettlement(rules);flow.Director.Construction.ConfigureSettlement(rules,table);
            var panel=flow.GetComponent<DemoSettlementPanel>();if(panel==null)panel=flow.gameObject.AddComponent<DemoSettlementPanel>();panel.Flow=flow;
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("DEMO_SETTLEMENT_BOUND: shared 3x3 collision/nav obstacle, E07 shell/materials, five functional blueprints; tests pending.");
        }
        private static GameObject CampPrefab(GameObject original,string id)
        {
            var path=Root+"/Prefabs/Buildings/"+id+".prefab";var existing=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(existing!=null)return existing;
            var node=(GameObject)PrefabUtility.InstantiatePrefab(original);node.name="Camp "+id;
            try
            {
                foreach(var renderer in node.GetComponentsInChildren<Renderer>())renderer.enabled=false;
                var shed=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/Environment/VillageE07/Architecture/OpenWoodshed_3x3m.prefab");var skin=(GameObject)PrefabUtility.InstantiatePrefab(shed);skin.transform.SetParent(node.transform,false);
                foreach(var collider in skin.GetComponentsInChildren<Collider>())Object.DestroyImmediate(collider);
                var wall=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/Plaster.mat");var wood=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/DarkOak.mat");
                if(id!="farm")
                {
                    Box(node.transform,"Plaster rear",new Vector3(0,1.22f,1.25f),new Vector3(2.6f,2.44f,.12f),wall);
                    Box(node.transform,"Side panel",new Vector3(1.25f,1.22f,0),new Vector3(.12f,2.44f,2.6f),wall);
                    Box(node.transform,"Door left",new Vector3(-1.02f,1.22f,-1.25f),new Vector3(.56f,2.44f,.12f),wall);
                    Box(node.transform,"Door right",new Vector3(1.02f,1.22f,-1.25f),new Vector3(.56f,2.44f,.12f),wall);
                    Box(node.transform,"Lintel",new Vector3(0,2.35f,-1.25f),new Vector3(1.5f,.18f,.16f),wood);
                }
                else
                {
                    var earth=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/Earth.mat");for(var i=0;i<3;i++)Box(node.transform,"Planting bed",new Vector3(-.8f+i*.8f,.07f,0),new Vector3(.5f,.1f,2.4f),earth);
                }
                var decoration=id=="warehouse"?"SupplySack_75cm":id=="barracks"?"ToolRack_140cm":id=="workshop"?"Woodpile_2m":"WayfindingSign_190cm";
                var prop=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/Environment/VillageE07/Props/"+decoration+".prefab"));prop.transform.SetParent(node.transform,false);prop.transform.localPosition=new Vector3(-.6f,0,.4f);foreach(var c in prop.GetComponentsInChildren<Collider>())Object.DestroyImmediate(c);
                return PrefabUtility.SaveAsPrefabAsset(node,path);
            }
            finally{Object.DestroyImmediate(node);}
        }
        private static void Box(Transform parent,string name,Vector3 point,Vector3 size,Material material)
        {var box=GameObject.CreatePrimitive(PrimitiveType.Cube);box.name=name;box.transform.SetParent(parent,false);box.transform.localPosition=point;box.transform.localScale=size;box.GetComponent<Renderer>().sharedMaterial=material;Object.DestroyImmediate(box.GetComponent<Collider>());}
    }
}
