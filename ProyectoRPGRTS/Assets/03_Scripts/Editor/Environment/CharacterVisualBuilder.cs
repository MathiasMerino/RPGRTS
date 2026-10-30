using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using G = Aethernia.Editor.Environment.EnvironmentGeometry;
using Object = UnityEngine.Object;

namespace Aethernia.Editor.Environment
{
    public static class CharacterVisualBuilder
    {
        public const string Root="Assets/07_Characters/Provisional";
        public const string Prefabs="Assets/02_Prefabs/Characters/Visuals";
        public const string CatalogPath=Root+"/ClassVisuals.asset";
        public static void Create()
        {
            var scene=EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath);
            if(AssetDatabase.IsValidFolder(Root)) throw new InvalidOperationException("E01 already exists; do not generate again.");
            var physics=EnvironmentPhysicsSnapshot.Capture(scene); var navigation=MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath);
            EnvironmentMaterials.Folder(Root+"/Materials"); EnvironmentMaterials.Folder(Root+"/Meshes"); EnvironmentMaterials.Folder(Prefabs);
            Material("Skin",new Color(.64f,.43f,.30f),.23f);
            Material("Hair",new Color(.12f,.10f,.085f),.12f);
            Material("Leather",new Color(.32f,.20f,.12f),.19f);
            Material("Steel",new Color(.56f,.60f,.61f),.30f,.70f);
            Material("WarriorCloth",new Color(.25f,.36f,.43f),.12f);
            Material("MageCloth",new Color(.30f,.25f,.41f),.12f);
            Material("MonkCloth",new Color(.72f,.65f,.49f),.10f);
            Material("Sash",new Color(.48f,.19f,.13f),.10f);
            var sword=Save("TrainingSword_96cm",p=>
            {
                Shape(p,"Blade",PrimitiveType.Cube,new Vector3(0,.57f,0),new Vector3(.045f,.78f,.014f),"Steel");
                Shape(p,"Guard",PrimitiveType.Cube,new Vector3(0,.18f,0),new Vector3(.19f,.025f,.035f),"Steel");
                Shape(p,"Grip",PrimitiveType.Cylinder,new Vector3(0,.09f,0),new Vector3(.035f,.16f,.035f),"Leather");
                Shape(p,"Pommel",PrimitiveType.Sphere,new Vector3(0,.018f,0),new Vector3(.045f,.036f,.045f),"Steel");
            });
            var armour=Save("ClothArmourOverlay",p=>
            {
                Shape(p,"Quilted vest",PrimitiveType.Sphere,new Vector3(0,1.22f,-.025f),new Vector3(.39f,.40f,.265f),"MonkCloth");
                Shape(p,"Vertical seam",PrimitiveType.Cube,new Vector3(0,1.22f,-.16f),new Vector3(.025f,.33f,.015f),"Leather");
            });
            var entries=new CharacterVisualCatalog.Entry[3]; var ids=new[]{"warrior","mage","monk"};
            for(var i=0;i<ids.Length;i++)
            {
                var id=ids[i]; var node=new GameObject(id+"_Visual");
                try
                {
                    var body=new GameObject("Body").transform; body.SetParent(node.transform,false);
                    Human(body,id); G.Consolidate(body.gameObject,id,Root);
                    body.localRotation=Quaternion.Euler(0,180,0); // Prefab forward is Unity +Z.
                    var equipment=new GameObject("EquipmentRoot").transform; equipment.SetParent(node.transform,false);
                    equipment.localRotation=Quaternion.Euler(0,180,0);
                    var weapon=(GameObject)PrefabUtility.InstantiatePrefab(sword,equipment); weapon.transform.localPosition=new Vector3(.27f,.79f,-.10f); weapon.SetActive(false);
                    var vest=(GameObject)PrefabUtility.InstantiatePrefab(armour,equipment); vest.SetActive(false);
                    node.AddComponent<CharacterVisualModel>().Configure(weapon,vest);
                    var path=Prefabs+"/"+id+"_Visual.prefab";
                    entries[i]=new CharacterVisualCatalog.Entry {ClassId=id,Prefab=PrefabUtility.SaveAsPrefabAsset(node,path)};
                }
                finally { Object.DestroyImmediate(node); }
            }
            var catalog=ScriptableObject.CreateInstance<CharacterVisualCatalog>(); catalog.Entries=entries; AssetDatabase.CreateAsset(catalog,CatalogPath);
            var director=Object.FindAnyObjectByType<PrototypeDirector>(); var hero=director.Hero;
            var visualRoot=new GameObject("VisualRoot").transform; visualRoot.SetParent(hero.transform,false);
            var legacy=hero.transform.Find("Height 1.80m - diameter 0.60m").GetComponent<Renderer>();
            hero.gameObject.AddComponent<CharacterVisualPresenter>().Configure(director.Session,catalog,visualRoot,legacy);
            var roof=GameObject.Find(MedievalQuarterBuilder.RootName).GetComponentsInChildren<Transform>().First(t=>t.name=="GableRoof_660x860cm");
            roof.gameObject.AddComponent<RoofCutaway>().Configure(Camera.main,hero.transform,roof.GetComponentsInChildren<Renderer>());
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics || MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath)!=navigation)
                throw new InvalidOperationException("E01 must not modify physics/navigation.");
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            Debug.Log("E01_CREATED: three class visuals, shared equipment/materials, preserved controller/physics/nav and reversible roof cutaway.");
        }
        private static Material Load(string name)
        { return AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/"+name+".mat") ?? EnvironmentMaterials.Load(name); }
        public static void Refine()
        {
            if(!AssetDatabase.IsValidFolder(Root)) throw new InvalidOperationException("Create E01 first.");
            foreach(var guid in AssetDatabase.FindAssets("t:Material",new[]{Root}))
            {
                var material=AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                var colour=material.GetColor("_BaseColor"); colour.a=1; material.SetColor("_BaseColor",colour); EditorUtility.SetDirty(material);
            }
            var steel=Load("Steel"); var maps=EnvironmentMaterials.Load("Plaster");
            steel.SetTexture("_BaseMap",maps.GetTexture("_BaseMap")); steel.SetTexture("_BumpMap",maps.GetTexture("_BumpMap"));
            steel.SetFloat("_Smoothness",.30f); steel.SetFloat("_Metallic",.70f); EditorUtility.SetDirty(steel);
            foreach(var id in new[]{"warrior","mage","monk"})
            {
                var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Meshes/"+id+"_Hair.asset"); var vertices=mesh.vertices;
                // Cover the scalp at the crown without touching the shared eye geometry.
                var rise=Mathf.Max(0,1.808f-mesh.bounds.max.y);
                for(var i=0;i<vertices.Length;i++) vertices[i].y+=Mathf.InverseLerp(1.714f,1.734f,vertices[i].y)*rise;
                mesh.vertices=vertices; mesh.RecalculateNormals(); mesh.RecalculateTangents(); mesh.RecalculateBounds(); EditorUtility.SetDirty(mesh);
            }
            AssetDatabase.SaveAssets(); Debug.Log("E01_REFINED: legible steel, crown coverage, GUIDs and world unchanged.");
        }
        private static void Material(string name,Color colour,float smoothness,float metallic=0)
        {
            var mat=new Material(Shader.Find("Universal Render Pipeline/Lit")) {name=name,enableInstancing=true};
            var shared=EnvironmentMaterials.Load("Plaster");
            mat.SetTexture("_BaseMap",shared.GetTexture("_BaseMap")); mat.SetTexture("_BumpMap",shared.GetTexture("_BumpMap"));
            mat.SetColor("_BaseColor",new Color(colour.r*1.35f,colour.g*1.35f,colour.b*1.35f,1)); mat.SetFloat("_BumpScale",.15f); mat.EnableKeyword("_NORMALMAP");
            mat.SetFloat("_Smoothness",smoothness); mat.SetFloat("_Metallic",metallic); AssetDatabase.CreateAsset(mat,Root+"/Materials/"+name+".mat");
        }
        private static GameObject Save(string name,Action<Transform> build)
        {
            var node=new GameObject(name);
            try { build(node.transform); G.Consolidate(node,name,Root); return PrefabUtility.SaveAsPrefabAsset(node,Prefabs+"/"+name+".prefab"); }
            finally { Object.DestroyImmediate(node); }
        }
        private static GameObject Shape(Transform parent,string name,PrimitiveType type,Vector3 centre,Vector3 size,string material,float tilt=0)
        {
            var go=GameObject.CreatePrimitive(type); go.name=name; Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent,false); go.transform.localPosition=centre;
            var bounds=go.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            go.transform.localScale=new Vector3(size.x/bounds.x,size.y/bounds.y,size.z/bounds.z);
            go.transform.localRotation=Quaternion.Euler(0,0,tilt); go.GetComponent<Renderer>().sharedMaterial=Load(material); return go;
        }
        private static void Human(Transform p,string id)
        {
            var cloth=id=="warrior"?"WarriorCloth":id=="mage"?"MageCloth":"MonkCloth";
            Shape(p,"Head",PrimitiveType.Sphere,new Vector3(0,1.68f,0),new Vector3(.18f,.24f,.19f),"Skin");
            Shape(p,"Hair",PrimitiveType.Sphere,new Vector3(0,1.734f,.018f),new Vector3(.19f,.132f,.19f),"Hair");
            Shape(p,"Nose",PrimitiveType.Sphere,new Vector3(0,1.665f,-.099f),new Vector3(.035f,.05f,.04f),"Skin");
            foreach(var side in new[]{-1,1}) Shape(p,"Eye",PrimitiveType.Sphere,new Vector3(side*.040f,1.705f,-.088f),new Vector3(.024f,.014f,.014f),"Hair");
            Shape(p,"Neck",PrimitiveType.Cylinder,new Vector3(0,1.515f,0),new Vector3(.105f,.12f,.105f),"Skin");
            Shape(p,"Torso",PrimitiveType.Sphere,new Vector3(0,1.23f,0),new Vector3(.35f,.52f,.22f),cloth);
            Shape(p,"Hips",PrimitiveType.Sphere,new Vector3(0,.92f,0),new Vector3(.32f,.24f,.20f),cloth);
            Shape(p,"Belt",PrimitiveType.Cube,new Vector3(0,.98f,0),new Vector3(.34f,.075f,.235f),id=="monk"?"Sash":"Leather");
            foreach(var side in new[]{-1,1})
            {
                Shape(p,"Upper arm",PrimitiveType.Capsule,new Vector3(side*.215f,1.315f,0),new Vector3(id=="mage"?.13f:.10f,.31f,.11f),id=="monk"?"Skin":cloth,side*12);
                Shape(p,"Forearm",PrimitiveType.Capsule,new Vector3(side*.25f,1.07f,-.015f),new Vector3(id=="mage"?.14f:.09f,.25f,.10f),id=="monk"?"Skin":cloth);
                Shape(p,"Hand",PrimitiveType.Sphere,new Vector3(side*.25f,.90f,-.025f),new Vector3(.075f,.13f,.07f),"Skin");
                Shape(p,"Upper leg",PrimitiveType.Capsule,new Vector3(side*.095f,.64f,0),new Vector3(.13f,.42f,.14f),cloth);
                Shape(p,"Lower leg",PrimitiveType.Capsule,new Vector3(side*.095f,.285f,0),new Vector3(.115f,.39f,.12f),id=="monk"?"Skin":cloth);
                Shape(p,"Boot",PrimitiveType.Sphere,new Vector3(side*.095f,.085f,-.035f),new Vector3(.15f,.17f,.25f),"Leather");
                if(id=="warrior") Shape(p,"Pauldron",PrimitiveType.Sphere,new Vector3(side*.21f,1.435f,0),new Vector3(.19f,.16f,.26f),"Steel");
                if(id=="monk") Shape(p,"Wrist wrap",PrimitiveType.Cylinder,new Vector3(side*.25f,1.01f,-.015f),new Vector3(.10f,.12f,.105f),"MonkCloth");
            }
            if(id=="warrior")
            {
                Shape(p,"Breastplate",PrimitiveType.Sphere,new Vector3(0,1.25f,-.015f),new Vector3(.36f,.38f,.25f),"Steel");
                var shield=Shape(p,"Stowed shield rim",PrimitiveType.Cylinder,new Vector3(0,1.23f,.18f),new Vector3(.48f,.055f,.60f),"Steel"); shield.transform.localRotation=Quaternion.Euler(90,0,0);
                var face=Shape(p,"Stowed shield wood",PrimitiveType.Cylinder,new Vector3(0,1.23f,.215f),new Vector3(.43f,.025f,.55f),"Oak"); face.transform.localRotation=Quaternion.Euler(90,0,0);
            }
            else if(id=="mage")
            {
                var node=new GameObject("Robe skirt",typeof(MeshFilter),typeof(MeshRenderer)); node.transform.SetParent(p,false);
                node.GetComponent<MeshFilter>().sharedMesh=G.Lathe("E01_Robe",new[]{.18f,.5f,.84f,1f},new[]{.28f,.25f,.20f,.17f}); node.GetComponent<MeshRenderer>().sharedMaterial=Load(cloth);
                Shape(p,"Folded hood",PrimitiveType.Sphere,new Vector3(0,1.48f,.045f),new Vector3(.31f,.20f,.24f),cloth);
                Shape(p,"Stowed staff - design accessory",PrimitiveType.Cylinder,new Vector3(.29f,.9f,.14f),new Vector3(.035f,1.65f,.035f),"Oak");
                Shape(p,"Staff finial",PrimitiveType.Sphere,new Vector3(.29f,1.735f,.14f),new Vector3(.08f,.08f,.08f),"Steel");
                Shape(p,"Satchel",PrimitiveType.Cube,new Vector3(-.21f,.96f,.02f),new Vector3(.10f,.15f,.10f),"Leather");
            }
            else
            {
                Shape(p,"Crossed wrap",PrimitiveType.Cube,new Vector3(-.025f,1.25f,-.117f),new Vector3(.085f,.40f,.025f),"Sash",-28);
                Shape(p,"Sash ends",PrimitiveType.Cube,new Vector3(-.10f,.81f,-.12f),new Vector3(.065f,.26f,.025f),"Sash",7);
            }
        }
    }
}
