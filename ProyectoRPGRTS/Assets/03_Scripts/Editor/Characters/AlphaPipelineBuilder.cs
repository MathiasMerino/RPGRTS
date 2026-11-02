using System;
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
    public static class AlphaPipelineBuilder
    {
        public const string Root="Assets/13_AlphaCandidate",CatalogPath=Root+"/Characters/ClassVisuals.asset",PrivateRoot="Assets/LocalOnly/E17",PrivateScene=PrivateRoot+"/MeshyReview.unity";
        public static void Folder(string path){if(AssetDatabase.IsValidFolder(path))return;Folder(Path.GetDirectoryName(path).Replace('\\','/'));AssetDatabase.CreateFolder(Path.GetDirectoryName(path).Replace('\\','/'),Path.GetFileName(path));}
        public static void Create()
        {
            if(File.Exists(CatalogPath))throw new InvalidOperationException("Pipeline E17 existente; no regenerar.");
            EditorSceneManager.OpenScene(VisualPolishBuilder.ScenePath);var original=Object.FindAnyObjectByType<DemoFlow>().Front.Catalog;
            Folder(Root+"/Characters");var catalog=Object.Instantiate(original);catalog.name="E17 reviewed provisional visuals";
            foreach(var entry in catalog.Entries){entry.ReplacementPrefab=entry.HumanoidPrefab;entry.UseReplacement=true;entry.ReplacementRequiresAnimation=true;}
            AssetDatabase.CreateAsset(catalog,CatalogPath);AssetDatabase.SaveAssets();
            try{InspectMeshy();}catch(Exception e){Directory.CreateDirectory("Logs/E17");File.WriteAllText("Logs/E17/MESHY_BLOCKED.txt",e.ToString());Debug.LogWarning("E17 Meshy local analysis blocked: "+e.GetType().Name);}
            Debug.Log("E17_PIPELINE_CREATED: shared references only; experimental Meshy remains local and unbound.");
        }
        [Serializable]private sealed class MeshReport{public string source,experimentalStatus,animationType,normalization;public float importerScale,normalizedHeight;public Vector3 originalSize,rootEuler;public int meshes,vertices,triangles,submeshes,materials,textures,transforms,skins,bones,animators,uvVertices,nonFiniteUv,degenerateUvTriangles;public bool validAvatar,hasUv;}
        private static void InspectMeshy()
        {
            const string folder="Assets/07_Characters/MonkMeshy";
            if(!Directory.Exists(folder)){File.WriteAllText("Logs/E17/MESHY_MISSING.txt","No experimental source available.");return;}
            var source=Directory.GetFiles(folder,"*.fbx").SingleOrDefault();if(source==null)return;source=source.Replace('\\','/');Folder(PrivateRoot);
            var copy=PrivateRoot+"/MeshyInspection.fbx";if(File.Exists(copy))throw new InvalidOperationException("Local review copy exists; preserve it.");
            if(!AssetDatabase.CopyAsset(source,copy))throw new IOException("Could not create isolated inspection copy.");
            var imported=(ModelImporter)AssetImporter.GetAtPath(copy);imported.isReadable=true;imported.SaveAndReimport();
            var sourceImporter=(ModelImporter)AssetImporter.GetAtPath(source);var original=AssetDatabase.LoadAssetAtPath<GameObject>(copy);
            var report=new MeshReport{source=source,experimentalStatus="Not approved for runtime or publication",animationType=sourceImporter.animationType.ToString(),importerScale=sourceImporter.globalScale,rootEuler=original.transform.localEulerAngles};
            var meshes=AssetDatabase.LoadAllAssetsAtPath(copy).OfType<Mesh>().ToArray();report.meshes=meshes.Length;
            foreach(var mesh in meshes)
            {
                report.vertices+=mesh.vertexCount;report.submeshes+=mesh.subMeshCount;var uv=mesh.uv;report.uvVertices+=uv.Length;report.hasUv|=uv.Length==mesh.vertexCount&&uv.Length>0;
                report.nonFiniteUv+=uv.Count(v=>float.IsNaN(v.x)||float.IsNaN(v.y)||float.IsInfinity(v.x)||float.IsInfinity(v.y));
                for(var sub=0;sub<mesh.subMeshCount;sub++)
                {
                    var triangles=mesh.GetTriangles(sub);report.triangles+=triangles.Length/3;
                    if(uv.Length==mesh.vertexCount)for(var t=0;t<triangles.Length;t+=3){var a=uv[triangles[t+1]]-uv[triangles[t]];var b=uv[triangles[t+2]]-uv[triangles[t]];if(Mathf.Abs(a.x*b.y-a.y*b.x)<1e-10f)report.degenerateUvTriangles++;}
                }
            }
            report.materials=original.GetComponentsInChildren<Renderer>(true).SelectMany(r=>r.sharedMaterials).Where(m=>m!=null).Distinct().Count();report.textures=AssetDatabase.LoadAllAssetsAtPath(copy).OfType<Texture2D>().Count();
            report.transforms=original.GetComponentsInChildren<Transform>(true).Length;var skins=original.GetComponentsInChildren<SkinnedMeshRenderer>(true);report.skins=skins.Length;report.bones=skins.Sum(s=>s.bones.Length);var animators=original.GetComponentsInChildren<Animator>(true);report.animators=animators.Length;report.validAvatar=animators.Any(a=>a.avatar!=null&&a.avatar.isValid);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var root=new GameObject("Experimental local inspection · not gameplay");var model=(GameObject)PrefabUtility.InstantiatePrefab(original);model.transform.SetParent(root.transform,false);
            foreach(var a in model.GetComponentsInChildren<Animator>(true))a.enabled=false;
            var bounds=BoundsOf(model);report.originalSize=bounds.size;
            var rotated=bounds.size.y<Mathf.Max(bounds.size.x,bounds.size.z)*.45f;if(rotated)model.transform.localRotation=Quaternion.Euler(-90,0,0)*model.transform.localRotation;
            bounds=BoundsOf(model);var factor=1.78f/Mathf.Max(bounds.size.y,.001f);model.transform.localScale*=factor;bounds=BoundsOf(model);model.transform.position-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
            report.normalizedHeight=BoundsOf(model).size.y;report.normalization="Wrapper root scale1; child scale factor="+factor+"; rotateMinus90X="+rotated+"; feetY0; front requires human review";
            var material=new Material(Shader.Find("Universal Render Pipeline/Lit"));
            // El identificador real de URP conserva su forma estándar en esta versión.
            if(material.shader==null)throw new InvalidOperationException("URP material missing");material.color=new Color(.27f,.29f,.30f);AssetDatabase.CreateAsset(material,PrivateRoot+"/Studio.mat");
            var floor=GameObject.CreatePrimitive(PrimitiveType.Plane);floor.name="Metre-scale studio floor";floor.transform.localScale=Vector3.one*.4f;floor.GetComponent<Renderer>().sharedMaterial=material;Object.DestroyImmediate(floor.GetComponent<Collider>());
            var camera=new GameObject("Main Camera").AddComponent<Camera>();camera.tag="MainCamera";camera.transform.position=new Vector3(2.8f,1.8f,3.8f);camera.transform.LookAt(new Vector3(0,.95f,0));camera.orthographic=true;camera.orthographicSize=1.25f;camera.backgroundColor=new Color(.08f,.09f,.10f);camera.clearFlags=CameraClearFlags.SolidColor;camera.GetUniversalAdditionalCameraData().renderPostProcessing=false;
            var key=new GameObject("Key").AddComponent<Light>();key.type=LightType.Directional;key.intensity=1.2f;key.transform.rotation=Quaternion.Euler(35,-40,0);key.shadows=LightShadows.None;var fill=new GameObject("Fill").AddComponent<Light>();fill.type=LightType.Directional;fill.intensity=.35f;fill.transform.rotation=Quaternion.Euler(15,140,0);fill.shadows=LightShadows.None;
            RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.28f,.30f,.33f);EditorSceneManager.SaveScene(scene,PrivateScene);AssetDatabase.SaveAssets();Directory.CreateDirectory("Logs/E17");File.WriteAllText("Logs/E17/MESHY_INSPECTION.json",JsonUtility.ToJson(report,true));
        }
        private static Bounds BoundsOf(GameObject root){var renderers=root.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);return bounds;}
    }
}
