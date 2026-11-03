using System;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class UxExperienceBuilder
    {
        public const string Root="Assets/16_UX01",ScenePath="Assets/01_Scenes/AetherniaUx01Demo.unity";
        public static void Create()
        {
            if(File.Exists(ScenePath)||AssetDatabase.IsValidFolder(Root))throw new InvalidOperationException("UX01 exists; do not regenerate.");
            AlphaPipelineBuilder.Folder(Root);
            var scene=EditorSceneManager.OpenScene(CivilizationSceneBuilder.ScenePath);
            var flow=Object.FindAnyObjectByType<DemoFlow>();var original=flow.Session.Rules;
            var rules=Object.Instantiate(original);rules.UxExperience=true;AssetDatabase.CreateAsset(rules,Root+"/PrototypeRules.asset");
            var content=Object.Instantiate(flow.Content);content.Intro=new[]{"LA ALDEA DE FRONTERA\n\nLlegas a un camino tranquilo. Un habitante necesita recuperar sus provisiones. Explora primero y habla con él cuando estés preparado.\n\nRelato provisional de demostración."};AssetDatabase.CreateAsset(content,Root+"/DemoContent.asset");flow.Content=content;
            foreach(var component in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {
                if(component==null)continue;var data=new SerializedObject(component);var it=data.GetIterator();
                while(it.NextVisible(true))if(it.propertyType==SerializedPropertyType.ObjectReference&&it.objectReferenceValue==original)it.objectReferenceValue=rules;
                data.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            }
            flow.CameraRig.FollowTarget=flow.Director.Hero;PrefabUtility.RecordPrefabInstancePropertyModifications(flow.CameraRig);
            var view=flow.gameObject.AddComponent<UxExperienceView>();view.Flow=flow;view.Cargo=Object.FindAnyObjectByType<LostCargoController>();view.Commands=Object.FindAnyObjectByType<PlayerCommands>();
            flow.Patrol.transform.position=new Vector3(-47,0,-.5f);PrefabUtility.RecordPrefabInstancePropertyModifications(flow.Patrol.transform);
            var root=new GameObject("UX01 Western muster ground").transform;
            var ground=GameObject.Find("ReferenceGround_40m").GetComponentInChildren<Renderer>().sharedMaterial;
            var road=AssetDatabase.FindAssets("t:Material").Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault(p=>p.EndsWith("/Soil.mat"))??AssetDatabase.FindAssets("t:Material").Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault(p=>p.EndsWith("/Ground.mat"));
            var pathMaterial=road==null?ground:AssetDatabase.LoadAssetAtPath<Material>(road);
            Box(root,"Muster soil 40x34m",new Vector3(-34,-.12f,0),new Vector3(40,.24f,34),ground,true);
            Box(root,"Village to camp path 4m",new Vector3(-29,.012f,1.5f),new Vector3(36,.025f,4),pathMaterial,false);
            Box(root,"Staging path 4m",new Vector3(-45,.015f,-3.5f),new Vector3(4,.03f,10),pathMaterial,false);
            var sign=GameObject.CreatePrimitive(PrimitiveType.Cylinder);sign.name="First move destination";sign.transform.SetParent(root);sign.transform.position=new Vector3(-42,.03f,-4);sign.transform.localScale=new Vector3(2,.025f,2);Object.DestroyImmediate(sign.GetComponent<Collider>());sign.GetComponent<Renderer>().sharedMaterial=pathMaterial;
            var second=Object.Instantiate(sign,root);second.name="Regroup destination";second.transform.position=new Vector3(-39,.03f,2);
            // Shared public prefabs only; clear 30x18m army corridor and 4m footpath.
            var props=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/02_Prefabs"}).Select(AssetDatabase.GUIDToAssetPath).Where(p=>p.Contains("Barrel")||p.Contains("GrassClump")||p.Contains("GroundFern")).ToArray();
            for(var i=0;i<props.Length&&i<12;i++)
            {var instance=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(props[i]),root);instance.transform.position=new Vector3(-49+i*2.4f,0,-11);}
            var surface=GameObject.Find("Baked Navigation").GetComponent<NavMeshSurface>();
            surface.navMeshData=null;surface.collectObjects=CollectObjects.All;surface.useGeometry=UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;surface.BuildNavMesh();AssetDatabase.CreateAsset(surface.navMeshData,Root+"/SurfaceNavigation.asset");PrefabUtility.RecordPrefabInstancePropertyModifications(surface);
            var combat=Object.FindAnyObjectByType<CombatHudPanel>();var rect=(RectTransform)combat.Panel.transform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(1,1);rect.anchoredPosition=new Vector2(-20,-92);rect.sizeDelta=new Vector2(560,104);
            foreach(var light in Object.FindObjectsByType<Light>())if(light.type==LightType.Directional){light.intensity=Mathf.Max(light.intensity,1.35f);light.shadowStrength=.7f;}
            EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();Debug.Log("UX01_CREATED safe exploration, shared E19 identity, 1m units, separate navigation and muster ground.");
        }
        private static void Box(Transform parent,string name,Vector3 at,Vector3 scale,Material mat,bool collision)
        {var node=GameObject.CreatePrimitive(PrimitiveType.Cube);node.name=name;node.transform.SetParent(parent);node.transform.position=at;node.transform.localScale=scale;node.GetComponent<Renderer>().sharedMaterial=mat;if(!collision)Object.DestroyImmediate(node.GetComponent<Collider>());}
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);
            foreach(var name in new[]{"ReferenceWindow_100x120cm","ReferenceDoor_120x220cm","Narrow Gate - 0.40m clearance"})
            {var item=GameObject.Find(name);if(item!=null){item.SetActive(false);PrefabUtility.RecordPrefabInstancePropertyModifications(item);}}
            var surface=GameObject.Find("Baked Navigation").GetComponent<NavMeshSurface>();
            foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include))
                if((t.name=="Door_140x240cm"||t.name=="Window_100x120cm"||t.name=="Window ruin support"||t.name=="Old narrow gate posts")&&t.parent!=null&&t.parent.name=="Skins over original logical objects")
                {t.gameObject.SetActive(false);PrefabUtility.RecordPrefabInstancePropertyModifications(t.gameObject);}
            var soil=GameObject.Find("Muster soil 40x34m");soil.transform.position=new Vector3(-34,-.14f,0);soil.transform.localScale=new Vector3(64,.24f,60);soil.GetComponent<BoxCollider>().size=new Vector3(40f/64,1,30f/60);soil.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/Grass.mat");
            foreach(var name in new[]{"Village to camp path 4m","Staging path 4m","First move destination","Regroup destination"})GameObject.Find(name).GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/Earth.mat");
            surface.collectObjects=CollectObjects.Volume;surface.center=new Vector3(-2,0,0);surface.size=new Vector3(132,12,64);
            foreach(var landscape in Object.FindObjectsByType<MeshFilter>().Where(f=>f.name=="Grass"&&f.GetComponent<Renderer>()!=null&&f.GetComponent<Renderer>().bounds.size.x>60))
            {
            var current=AssetDatabase.GetAssetPath(landscape.sharedMesh);
            var terrainPath=current.StartsWith(Root+"/WesternGround")?current:Root+"/WesternGround-"+landscape.sharedMesh.name+".asset";
            var flat=AssetDatabase.LoadAssetAtPath<Mesh>(terrainPath);
            if(flat==null)
            {
                flat=Object.Instantiate(landscape.sharedMesh);flat.name="UX01 military ground and connecting path";var vertices=flat.vertices;
                for(var i=0;i<vertices.Length;i++)
                {
                    var point=landscape.transform.TransformPoint(vertices[i]);
                    var field=Mathf.Min(Mathf.InverseLerp(-64,-58,point.x),Mathf.InverseLerp(-13,-19,point.x),Mathf.InverseLerp(-24,-16,point.z),Mathf.InverseLerp(26,18,point.z));
                    var road=Mathf.Min(Mathf.InverseLerp(-53,-49,point.x),Mathf.InverseLerp(-7,-11,point.x),Mathf.InverseLerp(-5,-.5f,point.z),Mathf.InverseLerp(8,3.5f,point.z));
                    var blend=Mathf.SmoothStep(0,1,Mathf.Max(field,road));point.y=Mathf.Lerp(point.y,-.04f,blend);vertices[i]=landscape.transform.InverseTransformPoint(point);
                }
                flat.vertices=vertices;flat.RecalculateNormals();flat.RecalculateBounds();AssetDatabase.CreateAsset(flat,terrainPath);
            }
            landscape.sharedMesh=flat;PrefabUtility.RecordPrefabInstancePropertyModifications(landscape);EditorUtility.SetDirty(landscape);
            }
            var grassMaterial=Object.FindObjectsByType<MeshFilter>().First(f=>f.name=="Grass"&&f.GetComponent<Renderer>()!=null&&f.GetComponent<Renderer>().bounds.size.x>60).GetComponent<Renderer>().sharedMaterial;
            var earthMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/13_AlphaCandidate/Materials/Earth_Alpha.mat")??AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/Earth.mat");
            TiledSurface(soil,"MusterFloorSurface",grassMaterial);TiledSurface(GameObject.Find("Village to camp path 4m"),"VillageRoadSurface",earthMaterial);TiledSurface(GameObject.Find("Staging path 4m"),"CampRoadSurface",earthMaterial);
            var worldRoot=GameObject.Find("UX01 Western muster ground").transform;
            if(worldRoot.Find("Camp dressing")==null)
            {
                var dressing=new GameObject("Camp dressing").transform;dressing.SetParent(worldRoot,false);
                PlaceShared("OpenWoodshed_3x3m",dressing,new Vector3(-51,0,5));PlaceShared("Barrel_100cm",dressing,new Vector3(-50,0,1));PlaceShared("Crate_80cm",dressing,new Vector3(-52,0,-.5f));
            }
            var saved=AssetDatabase.LoadAssetAtPath<UnityEngine.AI.NavMeshData>(Root+"/SurfaceNavigation.asset");surface.RemoveData();surface.navMeshData=null;surface.BuildNavMesh();var generated=surface.navMeshData;surface.RemoveData();EditorUtility.CopySerialized(generated,saved);surface.navMeshData=saved;surface.AddData();Object.DestroyImmediate(generated);EditorUtility.SetDirty(saved);PrefabUtility.RecordPrefabInstancePropertyModifications(surface);
            var flow=Object.FindAnyObjectByType<DemoFlow>();flow.CameraRig.FollowTarget=flow.Director.Hero;PrefabUtility.RecordPrefabInstancePropertyModifications(flow.CameraRig);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("UX01_REFINED diagnostic door/window/gate removed from new scene only; same navigation GUID.");
        }
        private static void PlaceShared(string name,Transform parent,Vector3 at)
        {var path=AssetDatabase.FindAssets(name+" t:Prefab",new[]{"Assets/02_Prefabs"}).Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault(p=>System.IO.Path.GetFileNameWithoutExtension(p)==name);if(path==null)throw new InvalidOperationException("Missing public environment prefab "+name);var instance=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path),parent);instance.transform.position=at;PrefabUtility.RecordPrefabInstancePropertyModifications(instance.transform);}
        private static void TiledSurface(GameObject node,string name,Material material)
        {
            var path=Root+"/"+name+".asset";var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(mesh==null){mesh=new Mesh{name=name};var v=new[]{new Vector3(-.5f,.5f,-.5f),new Vector3(.5f,.5f,-.5f),new Vector3(.5f,.5f,.5f),new Vector3(-.5f,.5f,.5f)};mesh.vertices=v;mesh.triangles=new[]{0,2,1,0,3,2};mesh.uv=v.Select(x=>{var w=node.transform.TransformPoint(x);return new Vector2(w.x*.5f,w.z*.5f);}).ToArray();mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);}
            node.GetComponent<MeshFilter>().sharedMesh=mesh;node.GetComponent<Renderer>().sharedMaterial=material;PrefabUtility.RecordPrefabInstancePropertyModifications(node.GetComponent<MeshFilter>());PrefabUtility.RecordPrefabInstancePropertyModifications(node.GetComponent<Renderer>());
        }
        public static void Build()
        {Directory.CreateDirectory("Builds/Ux01Windows");var report=UnityEditor.BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/Ux01Windows/Aethernia.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new InvalidOperationException("UX01 build failed");Directory.CreateDirectory("Logs/UX01");File.WriteAllText("Logs/UX01/build.txt","PASS Unity "+Application.unityVersion+" bytes="+report.summary.totalSize);}
    }
}
