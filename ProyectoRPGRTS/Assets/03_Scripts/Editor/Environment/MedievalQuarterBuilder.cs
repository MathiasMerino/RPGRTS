using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using G = Aethernia.Editor.Environment.EnvironmentGeometry;
using Object = UnityEngine.Object;

namespace Aethernia.Editor.Environment
{
    public static class MedievalQuarterBuilder
    {
        public const string ScenePath="Assets/01_Scenes/PlayablePrototype.unity";
        public const string RootName="D01 Medieval Quarter";
        public const string NavPath="Assets/Settings/Prototype/VerticalNavMesh.asset";
        public static string Hash(string path)
        { using(var sha=SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant(); }
        public static void Upgrade()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);
            if(GameObject.Find(RootName)!=null || AssetDatabase.IsValidFolder(EnvironmentMaterials.Root)) throw new InvalidOperationException("D01 already exists; existing assets will not be generated again.");
            var baseline=EnvironmentPhysicsSnapshot.Capture(scene); var navHash=Hash(NavPath);
            EnvironmentMaterials.Create(); MedievalPrefabs.Create();
            File.WriteAllText(EnvironmentMaterials.Root+"/PhysicsBaseline.txt",baseline);
            File.WriteAllText(EnvironmentMaterials.Root+"/NavigationBaseline.txt",navHash);
            AssetDatabase.ImportAsset(EnvironmentMaterials.Root+"/PhysicsBaseline.txt"); AssetDatabase.ImportAsset(EnvironmentMaterials.Root+"/NavigationBaseline.txt");
            var root=new GameObject(RootName).transform;
            Skin("ReferenceHouse_6x8m",MedievalPrefabs.Load("Architecture","TimberHouse_6x8m"),root,new Vector3(6,0,6));
            Skin("ReferenceCube_1m",MedievalPrefabs.Load("Props","Barrel_100cm"),root,new Vector3(-8,0,-4));
            Skin("Obstacle 3x2x2m",MedievalPrefabs.Load("Props","CrateStack_3x2x2m"),root,new Vector3(-2,0,5));
            Skin("Interaction Chest",MedievalPrefabs.Load("Props","Chest_80x80x60cm"),root,new Vector3(-2,0,-5));
            Skin("Faction A Market",MedievalPrefabs.Load("Props","Crate_80cm"),root,new Vector3(1,.1f,-8));
            // Keep reference frames/colliders visible, adding material detail rather than moving them.
            foreach(var name in new[]{"ReferenceDoor_120x220cm","ReferenceWindow_100x120cm","Narrow Gate - 0.40m clearance"})
                foreach(var renderer in GameObject.Find(name).GetComponentsInChildren<Renderer>()) Assign(renderer,"Oak");
            var ground=GameObject.Find("ReferenceGround_40m").GetComponentInChildren<Renderer>(); Assign(ground,"Ground");
            var road=GameObject.Find("Street marker 4x12m - no collider").GetComponent<Renderer>(); road.enabled=false; PrefabUtility.RecordPrefabInstancePropertyModifications(road);
            Road(root);
            var random=new System.Random(1729);
            for(var i=0;i<62;i++)
            {
                var position=new Vector3((float)random.NextDouble()*24-12,0,(float)random.NextDouble()*20-9);
                if(position.x>2.5f&&position.x<9.5f&&position.z>1.4f&&position.z<10.5f) continue;
                if(position.z>9&&position.x< -4) continue;
                // Soft, passable ground plants: no trunks or new physical obstacles.
                var plant=MedievalPrefabs.Place(MedievalPrefabs.Load("Vegetation",i%4==0?"GroundFern":"GrassClump"),root,position,(float)random.NextDouble()*360);
                plant.transform.localScale=Vector3.one*(.7f+(float)random.NextDouble()*.7f);
            }
            var sun=GameObject.Find("Directional Light").GetComponent<Light>();
            sun.color=new Color(1,.92f,.80f); sun.intensity=1.35f; sun.shadows=LightShadows.Soft; sun.shadowStrength=.85f;
            sun.transform.rotation=Quaternion.Euler(50,-35,0);
            RenderSettings.ambientMode=AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=new Color(.48f,.54f,.61f); RenderSettings.ambientEquatorColor=new Color(.38f,.37f,.32f); RenderSettings.ambientGroundColor=new Color(.20f,.24f,.17f);
            Camera.main.backgroundColor=new Color(.15f,.19f,.21f);
            var rig=Camera.main.GetComponent<IsometricCameraRig>();
            using(var serialized=new SerializedObject(rig))
            {
                serialized.FindProperty("focus").vector3Value=new Vector3(2,0,2); serialized.FindProperty("size").floatValue=14.5f; serialized.ApplyModifiedPropertiesWithoutUndo();
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(rig);
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=baseline || Hash(NavPath)!=navHash) throw new InvalidOperationException("D01 modified existing world physics/navigation unexpectedly.");
            G.Persist(root.gameObject); EditorSceneManager.MarkSceneDirty(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
            Debug.Log("D01_CREATED: original materials, modular prefabs, physics/nav unchanged; no gameplay or character changes.");
        }
        private static void Skin(string source,GameObject prefab,Transform parent,Vector3 position)
        {
            foreach(var renderer in GameObject.Find(source).GetComponentsInChildren<Renderer>())
            { renderer.enabled=false; PrefabUtility.RecordPrefabInstancePropertyModifications(renderer); }
            MedievalPrefabs.Place(prefab,parent,position);
        }
        private static void Assign(Renderer renderer,string material)
        { renderer.sharedMaterial=EnvironmentMaterials.Load(material); PrefabUtility.RecordPrefabInstancePropertyModifications(renderer); }
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);
            var root=GameObject.Find(RootName);
            if(root==null) throw new InvalidOperationException("D01 must already exist.");
            var physics=EnvironmentPhysicsSnapshot.Capture(scene); var navigation=Hash(NavPath);
            EnvironmentMaterials.Create(); // Update our maps/materials in place; preserve their GUIDs.
            var existing=AssetDatabase.LoadAssetAtPath<Mesh>(EnvironmentMaterials.Root+"/Meshes/EarthLane.asset");
            var refined=RoadMesh(); EditorUtility.CopySerialized(refined,existing); Object.DestroyImmediate(refined); EditorUtility.SetDirty(existing);
            if(root.transform.Find("Window display support left")==null)
            {
                G.Box(root.transform,"Window display support left",new Vector3(-8.6f,.4f,3),new Vector3(.2f,.8f,.25f),"Oak");
                G.Box(root.transform,"Window display support right",new Vector3(-7.4f,.4f,3),new Vector3(.2f,.8f,.25f),"Oak");
            }
            var rig=Camera.main.GetComponent<IsometricCameraRig>();
            using(var serialized=new SerializedObject(rig))
            {
                serialized.FindProperty("focus").vector3Value=new Vector3(3,0,4);
                serialized.FindProperty("size").floatValue=15; serialized.ApplyModifiedPropertiesWithoutUndo();
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(rig);
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics || Hash(NavPath)!=navigation) throw new InvalidOperationException("Refinement modified original physics/navigation.");
            G.Persist(root); EditorSceneManager.MarkSceneDirty(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
            Debug.Log("D01_REFINED: material relief, continuous lane, framing; physics/nav unchanged.");
        }
        private static void Road(Transform parent)
        {
            var mesh=RoadMesh();
            G.Part(parent,"Worn earth lane",mesh,"Earth",Vector3.zero);
        }
        private static Mesh RoadMesh()
        {
            var points=new[]{new Vector3(-13,0,-6),new Vector3(-5,0,-5.7f),new Vector3(1,0,-3),new Vector3(6,0,0),new Vector3(6,0,2)};
            var centres=new List<Vector3>();
            for(var segment=0;segment<points.Length-1;segment++)
            {
                var steps=Mathf.CeilToInt(Vector3.Distance(points[segment],points[segment+1])/.5f);
                for(var i=0;i<steps;i++) centres.Add(Vector3.Lerp(points[segment],points[segment+1],(float)i/steps));
            }
            centres.Add(points[points.Length-1]);
            var vertices=new List<Vector3>(); var uv=new List<Vector2>(); var tris=new List<int>(); var distance=0f;
            for(var i=0;i<centres.Count;i++)
            {
                var centre=centres[i];
                var forward=(centres[Mathf.Min(i+1,centres.Count-1)]-centres[Mathf.Max(0,i-1)]).normalized;
                var side=Vector3.Cross(Vector3.up,forward);
                var width=1.4f+(Mathf.PerlinNoise(centre.x+30,centre.z+30)-.5f)*.3f;
                if(i>0) distance+=Vector3.Distance(centres[i-1],centre);
                vertices.Add(centre-side*width+Vector3.up*.018f); vertices.Add(centre+side*width+Vector3.up*.018f);
                uv.Add(new Vector2(0,distance/2)); uv.Add(new Vector2(width,distance/2));
                var n=vertices.Count; if(n>2) tris.AddRange(new[]{n-4,n-2,n-3,n-3,n-2,n-1});
            }
            var mesh=new Mesh {name="EarthLane",vertices=vertices.ToArray(),triangles=tris.ToArray(),uv=uv.ToArray()};
            mesh.RecalculateNormals(); mesh.RecalculateTangents(); mesh.RecalculateBounds(); return mesh;
        }
    }
}
