using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object=UnityEngine.Object;
namespace Aethernia.Editor.Characters
{
    public static class AlphaSurfaceBuilder
    {
        [Serializable]private sealed class Entry{public string source,result;public float wind,edge,smoothness;public int renderers;}
        [Serializable]private sealed class Report{public string graph;public int materials,renderers;public bool physicsEdited;public List<Entry> entries=new List<Entry>();}
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);
            foreach(var renderer in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include))if(renderer.sharedMaterials.Any(m=>m!=null&&m.HasProperty("_WindStrength")&&m.GetFloat("_WindStrength")>0))
            {var wind=renderer.GetComponent<VisualWindBounds>();if(wind==null)wind=renderer.gameObject.AddComponent<VisualWindBounds>();wind.Margin=.12f;EditorUtility.SetDirty(wind);}
            foreach(var point in Object.FindObjectsByType<InteractionPoint>())if(point.FactId=="umbral-entry"||point.FactId=="umbral-exit")
            {
                var landmark=GameObject.Find("E16 landmark "+point.FactId);if(landmark!=null){landmark.transform.position=point.transform.position+Vector3.right*1.2f;PrefabUtility.RecordPrefabInstancePropertyModifications(landmark.transform);EditorUtility.SetDirty(landmark.transform);}
                var label=point.GetComponentInChildren<TMPro.TMP_Text>();if(label!=null){var pos=label.transform.position;pos.x=point.transform.position.x+1.2f;label.transform.position=pos;PrefabUtility.RecordPrefabInstancePropertyModifications(label.transform);EditorUtility.SetDirty(label.transform);}
            }
            var atmosphere=Object.FindAnyObjectByType<RegionalAtmosphere>();atmosphere.CharacterFill.intensity=2.8f;PrefabUtility.RecordPrefabInstancePropertyModifications(atmosphere.CharacterFill);EditorUtility.SetDirty(atmosphere.CharacterFill);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        public static void Apply()
        {
            var scene=EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);var shader=AlphaSurfaceGraph.CreateOrLoad();var root=AlphaPipelineBuilder.Root+"/Materials";AlphaPipelineBuilder.Folder(root);var map=new Dictionary<Material,Material>();var counts=new Dictionary<Material,int>();
            var grass=AssetDatabase.LoadAssetAtPath<Material>("Assets/12_VisualPolish/Materials/GrassWarm.mat");
            foreach(var renderer in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include))
            {
                var mats=renderer.sharedMaterials;var changed=false;
                for(var i=0;i<mats.Length;i++)
                {
                    var old=mats[i];if(old==null)continue;var path=AssetDatabase.GetAssetPath(old);if(!path.StartsWith("Assets/06_Environment/VillageE07/Materials/")&&!path.StartsWith("Assets/12_VisualPolish/Materials/"))continue;
                    if(!map.TryGetValue(old,out var material))
                    {
                        var destination=root+"/"+old.name+"_Alpha.mat";material=AssetDatabase.LoadAssetAtPath<Material>(destination);if(material==null){material=Object.Instantiate(old);material.name=old.name+" Alpha";material.shader=shader;material.shaderKeywords=Array.Empty<string>();material.enableInstancing=true;material.SetTexture("_BlendMap",grass.GetTexture("_BaseMap"));material.SetTextureScale("_BlendMap",Vector2.one*2);material.SetFloat("_SurfaceVariation",old.name.Contains("Grass")?.30f:.17f);material.SetFloat("_NoiseScale",.35f);material.SetFloat("_EdgeBlend",old.name=="Earth"?1:old.name=="ForestFloor"?.65f:0);material.SetFloat("_WindStrength",old.name.StartsWith("Leaf")?.035f:0);material.SetFloat("_Smoothness",old.name.Contains("Bronze")?.30f:old.name.Contains("Oak")?.22f:old.name=="Slate"?.18f:old.name.Contains("Grass")?.06f:.12f);if(old.name.Contains("Bronze"))material.SetFloat("_Metallic",.72f);AssetDatabase.CreateAsset(material,destination);}map.Add(old,material);counts.Add(old,0);
                    }
                    mats[i]=material;counts[old]++;changed=true;if(material.GetFloat("_WindStrength")>0){var wind=renderer.GetComponent<VisualWindBounds>();if(wind==null)wind=renderer.gameObject.AddComponent<VisualWindBounds>();wind.Margin=.12f;EditorUtility.SetDirty(wind);}
                }
                if(changed){renderer.sharedMaterials=mats;PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);EditorUtility.SetDirty(renderer);}
            }
            var volume=Object.FindObjectsByType<Volume>().First(v=>v.sharedProfile==AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/12_VisualPolish/Grade.asset"));AlphaPipelineBuilder.Folder(AlphaPipelineBuilder.Root+"/Lighting");var gradePath=AlphaPipelineBuilder.Root+"/Lighting/Grade.asset";var grade=AssetDatabase.LoadAssetAtPath<VolumeProfile>(gradePath);
            if(grade==null)
            {
                grade=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(grade,gradePath);foreach(var source in volume.sharedProfile.components){var copy=Object.Instantiate(source);grade.components.Add(copy);AssetDatabase.AddObjectToAsset(copy,grade);}var tone=grade.Add<Tonemapping>(true);tone.mode.Override(TonemappingMode.Neutral);AssetDatabase.AddObjectToAsset(tone,grade);grade.TryGet<ColorAdjustments>(out var color);color.contrast.Override(5);color.saturation.Override(-5);color.postExposure.Override(.08f);grade.TryGet<Bloom>(out var bloom);bloom.intensity.Override(.04f);bloom.threshold.Override(1.4f);grade.TryGet<Vignette>(out var vignette);vignette.intensity.Override(.10f);
            }
            volume.sharedProfile=grade;PrefabUtility.RecordPrefabInstancePropertyModifications(volume);EditorUtility.SetDirty(volume);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            var report=new Report{graph=AlphaSurfaceGraph.Path,materials=map.Count,renderers=counts.Values.Sum(),physicsEdited=false};foreach(var item in map)report.entries.Add(new Entry{source=AssetDatabase.GetAssetPath(item.Key),result=AssetDatabase.GetAssetPath(item.Value),wind=item.Value.GetFloat("_WindStrength"),edge=item.Value.GetFloat("_EdgeBlend"),smoothness=item.Value.GetFloat("_Smoothness"),renderers=counts[item.Key]});Directory.CreateDirectory("Logs/E17");File.WriteAllText("Logs/E17/SURFACES.json",JsonUtility.ToJson(report,true));Debug.Log("E17_SURFACES: shared graph, existing normals, organic path edges, subtle leaf wind; physics untouched.");
        }
    }
}
