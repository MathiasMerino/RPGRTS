using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Environment
{
    public static class CoreVillageArt
    {
        public const string ScenePath="Assets/01_Scenes/AetherniaCoreRefine01.unity";
        public const string Library="Assets/06_Environment/CoreVillage";
        public const string InnEntrance="Posada_Marquesina_Y_Farol";
        public const string Forecourt="Plaza_Losa_Irregular";

        [MenuItem("Aethernia/Arte/CORE - Crear entrada de posada")]
        public static void Author()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);
            var inn=GameObject.Find("VillageInn_55x6m");
            if(inn==null||AssetDatabase.IsValidFolder(Library))
                throw new InvalidOperationException("La posada no está disponible o el kit CORE ya existe.");
            var cut=inn.GetComponent<RoofCutaway>();
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            var roof=new SerializedObject(cut).FindProperty("roof");
            var roofRenderers=new List<Renderer>();
            for(var i=0;i<roof.arraySize;i++)
                if(roof.GetArrayElementAtIndex(i).objectReferenceValue is Renderer renderer)roofRenderers.Add(renderer);
            foreach(var folder in new[]{"","/Meshes","/Prefabs"})EnvironmentMaterials.Folder(Library+folder);
            var canopy=Place(Save(InnEntrance,Canopy),inn.transform,Vector3.zero);
            roofRenderers.AddRange(canopy.GetComponentsInChildren<Renderer>());
            cut.Configure(flow.CameraRig.GetComponent<Camera>(),flow.Director.Hero.transform,roofRenderers.ToArray());
            PrefabUtility.RecordPrefabInstancePropertyModifications(cut);
            var plaza=new GameObject("CORE-REFINE-01 · umbral de la posada");
            var stones=Save(Forecourt,Flagstones,false);
            Place(stones,plaza.transform,new Vector3(-.9f,0,7.35f));
            Place(stones,plaza.transform,new Vector3(1.2f,0,4.78f),28);
            Place(stones,plaza.transform,new Vector3(.85f,0,2.18f),-28);
            EditorSceneManager.MarkSceneDirty(scene);
            if(!EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("No se guardó la escena CORE.");
            AssetDatabase.SaveAssets();
        }

        private static GameObject Place(GameObject prefab,Transform parent,Vector3 point,float yaw=0)
        {
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab,parent);
            instance.transform.SetLocalPositionAndRotation(point,Quaternion.Euler(0,yaw,0));
            return instance;
        }

        private static GameObject Save(string name,Action<FrontierArtMesh> sculpt,bool shadows=true)
        {
            var root=new GameObject(name);
            try
            {
                var mesh=new FrontierArtMesh();
                sculpt(mesh);
                mesh.Save(root.transform,name,Library+"/Meshes/"+name+".asset",shadows);
                return PrefabUtility.SaveAsPrefabAsset(root,Library+"/Prefabs/"+name+".prefab");
            }
            finally{Object.DestroyImmediate(root);}
        }

        private static void Canopy(FrontierArtMesh mesh)
        {
            var oak=VillageE07Materials.Load("AgedOak");
            var dark=VillageE07Materials.Load("DarkOak");
            var slate=VillageE07Materials.Load("Slate");
            var iron=VillageE07Materials.Load("Iron");
            var amber=VillageE07Materials.Load("EmberGlass");
            const int bays=10;
            Vector3 Eave(float x,float run)=>new(x*(1+.06f*run),3.11f-.46f*run+.09f*Mathf.Sin(run*Mathf.PI),-3.17f-1.25f*run);
            for(var bay=0;bay<bays;bay++)
            {
                var x=-1.57f+bay*3.14f/bays;
                var end=x+3.14f/bays;
                for(var row=0;row<4;row++)
                {
                    var front=(row+1)/4f;
                    var rear=row/4f;
                    var a=Eave(x,rear);var b=Eave(x,front);var c=Eave(end,front);var d=Eave(end,rear);
                    mesh.Face(slate,b,a,d,c);
                    mesh.Face(dark,a,b,c,d);
                    mesh.Beam(oak,b,c,.033f,.038f);
                }
            }
            foreach(var x in new[]{-1.44f,1.44f})
            {
                mesh.Beam(dark,new Vector3(x,2.62f,-3.14f),Eave(x,.95f),.116f,.135f);
                mesh.Beam(oak,new Vector3(x,2.12f,-3.14f),Eave(x,.87f),.112f,.128f);
            }
            var left=Eave(-1.62f,1);var right=Eave(1.62f,1);
            mesh.Beam(oak,left,right,.112f,.102f);
            var lamp=new Vector3(1.35f,2.31f,-4.17f);
            mesh.Beam(iron,lamp+Vector3.up*.12f,Eave(1.30f,.8f),.031f,.031f);
            mesh.Block(amber,lamp,new Vector3(.16f,.215f,.16f),.035f,Quaternion.identity);
            for(var i=0;i<4;i++)
            {
                var angle=(i+.5f)*Mathf.PI*.5f;
                var corner=lamp+new Vector3(Mathf.Cos(angle)*.107f,0,Mathf.Sin(angle)*.107f);
                mesh.Beam(iron,corner-Vector3.up*.123f,corner+Vector3.up*.123f,.027f,.029f);
            }
            mesh.Block(iron,lamp+Vector3.up*.122f,new Vector3(.20f,.06f,.20f),.043f,Quaternion.identity);
            mesh.Block(iron,lamp-Vector3.up*.125f,new Vector3(.19f,.044f,.19f),.043f,Quaternion.identity);
        }

        private static void Flagstones(FrontierArtMesh mesh)
        {
            var cold=VillageE07Materials.Load("GreyStone");
            var warm=VillageE07Materials.Load("WarmStone");
            var random=new System.Random(123);
            for(var row=0;row<6;row++)
            {
                for(var col=0;col<7;col++)
                {
                    var x=(col-3)*.56f+(row%2)*.23f;
                    var z=(row-2.5f)*.48f;
                    var ragged=Mathf.Abs(x)*.65f+Mathf.Abs(z)*.55f>1.28f;
                    if(ragged&&random.NextDouble()<.55)continue;
                    var tint=random.Next(4)==0?warm:cold;
                    var thickness=.035f+(float)random.NextDouble()*.018f;
                    var rotation=Quaternion.Euler(0,random.Next(-10,11),0);
                    mesh.Block(tint,new Vector3(x,thickness*.5f,z),new Vector3(.51f,thickness,.44f),.086f,rotation);
                }
            }
        }
    }
}
