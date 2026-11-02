using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object=UnityEngine.Object;
namespace Aethernia.Editor.Characters
{
    public static class AlphaQualityBuilder
    {
        public static void Apply()
        {
            var root=AlphaPipelineBuilder.Root+"/Graphics";AlphaPipelineBuilder.Folder(root);
            var names=new[]{"Bajo","Medio","Alto"};var guids=new string[3];var distances=new[]{20f,40f,60f};var sizes=new[]{512,1024,2048};
            for(var i=0;i<3;i++)
            {
                var path=root+"/"+names[i]+".asset";if(!File.Exists(path))AssetDatabase.CopyAsset("Assets/Settings/PC_RPAsset.asset",path);var asset=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);var data=new SerializedObject(asset);
                Set(data,"m_RenderScale",i==0?.8f:1f);Set(data,"m_MSAA",i==0?1:i==1?2:4);Set(data,"m_ShadowDistance",distances[i]);Set(data,"m_ShadowCascadeCount",i==0?1:2);Set(data,"m_MainLightShadowmapResolution",sizes[i]);Set(data,"m_AdditionalLightsShadowmapResolution",sizes[i]);Set(data,"m_SoftShadowsSupported",i==2);Set(data,"m_AdditionalLightsRenderingMode",i==0?1:2);Set(data,"m_AdditionalLightsPerObjectLimit",i==0?1:i==1?2:4);Set(data,"m_AdditionalLightShadowsSupported",i==2);
                if(i==0)
                {
                    var rendererPath=root+"/LowRenderer.asset";if(!File.Exists(rendererPath))AssetDatabase.CopyAsset("Assets/Settings/PC_Renderer.asset",rendererPath);var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);foreach(var feature in renderer.rendererFeatures){feature.SetActive(false);EditorUtility.SetDirty(feature);}EditorUtility.SetDirty(renderer);data.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue=renderer;
                }
                data.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(asset);guids[i]=AssetDatabase.AssetPathToGUID(path);
            }
            AssetDatabase.SaveAssets();var qualityPath="ProjectSettings/QualitySettings.asset";var text=File.ReadAllText(qualityPath);var start=text.IndexOf("  m_QualitySettings:",StringComparison.Ordinal);var end=text.IndexOf("  m_TextureMipmapLimitGroupNames:",start,StringComparison.Ordinal);
            var entries=text.Substring(start,end-start);var match=Regex.Match(entries,@"  - serializedVersion: 4\r?\n    name: (PC|Medio)\r?\n[\s\S]*?(?=  - serializedVersion: 4|$)");if(!match.Success)throw new InvalidOperationException("Expected historical PC or existing Medio quality entry.");
            var result="  m_QualitySettings:\n";for(var i=0;i<3;i++){var copy=Regex.Replace(match.Value,@"    name: (PC|Medio)","    name: "+names[i]);copy=Regex.Replace(copy,@"customRenderPipeline: \{fileID: 11400000, guid: [0-9a-f]+,\s*type: 2\}","customRenderPipeline: {fileID: 11400000, guid: "+guids[i]+", type: 2}");copy=Regex.Replace(copy,@"    shadowDistance: [^\r\n]+","    shadowDistance: "+distances[i]);copy=Regex.Replace(copy,@"    shadowCascades: [^\r\n]+","    shadowCascades: "+(i==0?1:2));result+=copy.TrimEnd()+"\n";}File.WriteAllText(qualityPath,text.Substring(0,start)+result+text.Substring(end));
            var scene=EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var policy=flow.GetComponent<AlphaGraphicsPolicy>();if(policy==null)policy=flow.gameObject.AddComponent<AlphaGraphicsPolicy>();policy.MainCamera=Camera.main;EditorUtility.SetDirty(policy);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("E17_QUALITY: three distinct URP profiles, low AO/post disabled, shared source renderer for medium/high. No package changes.");
        }
        private static void Set(SerializedObject data,string name,float value){var p=data.FindProperty(name);if(p==null)throw new InvalidOperationException(name);p.floatValue=value;}
        private static void Set(SerializedObject data,string name,int value){var p=data.FindProperty(name);if(p==null)throw new InvalidOperationException(name);p.intValue=value;}
        private static void Set(SerializedObject data,string name,bool value){var p=data.FindProperty(name);if(p==null)throw new InvalidOperationException(name);p.boolValue=value;}
        public static void Build()
        {
            Directory.CreateDirectory("Builds/AlphaCandidateE17Windows");var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{AlphaSceneBuilder.ScenePath},locationPathName="Builds/AlphaCandidateE17Windows/Aethernia.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
            if(report.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("E17 build failed");Directory.CreateDirectory("Logs/E17");File.WriteAllText("Logs/E17/build.txt","PASS Unity "+Application.unityVersion+"; development candidate E17; bytes="+report.summary.totalSize+"; seconds="+report.summary.totalTime.TotalSeconds+"; native validation required separately.");
        }
    }
}
