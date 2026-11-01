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
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab);front=instance.GetComponent<DemoFrontEnd>();front.Flow=flow;flow.Front=front;PrefabUtility.RecordPrefabInstancePropertyModifications(front);
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
    }
}
