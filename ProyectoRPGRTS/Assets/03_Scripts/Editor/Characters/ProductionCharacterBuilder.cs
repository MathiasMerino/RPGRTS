using System;
using System.IO;
using Aethernia.Editor.Environment;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class ProductionCharacterBuilder
    {
        public const string Root="Assets/07_Characters/ProductionProvisional";
        public const string Prefabs="Assets/02_Prefabs/Characters/E03";
        public static void Create()
        {
            if(AssetDatabase.IsValidFolder(Root))throw new InvalidOperationException("E03 already exists; do not regenerate.");
            var scene=EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath);var physics=EnvironmentPhysicsSnapshot.Capture(scene);
            foreach(var folder in new[]{Root+"/Materials",Root+"/Source",Prefabs})EnvironmentMaterials.Folder(folder);
            Tint("Steel",Old("Steel"),new Color32(120,130,135,255));Tint("Plum",Old("MageCloth"),new Color32(76,65,81,255));Tint("BlueGray",Old("MageCloth"),new Color32(84,102,115,255));
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                var dir=Root+"/Meshes/"+id;EnvironmentMaterials.Folder(dir);var node=new GameObject(id+"_E03_Proxy");
                try
                {
                    var mesh=new MonkProxyGeometry();Body(mesh,id);
                    var body=new GameObject("BodyAndClothing").transform;body.SetParent(node.transform,false);mesh.Bake(body,dir);
                    var equipment=new GameObject("EquipmentRoot").transform;equipment.SetParent(node.transform,false);
                    var hand=new GameObject("RightHand_Weapon").transform;hand.SetParent(equipment,false);hand.localPosition=new Vector3(.28f,.78f,.12f);
                    var sword=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/TrainingSword_96cm.prefab"),hand);sword.SetActive(false);
                    var cloth=new GameObject("EquippedCloth_Overlay");cloth.transform.SetParent(equipment,false);var overlay=new MonkProxyGeometry();
                    overlay.Box(Old("MonkCloth"),new Vector3(0,1.18f,.16f),new Vector3(.28f,.20f,.025f));
                    EnvironmentMaterials.Folder(dir+"/Equipment");overlay.Bake(cloth.transform,dir+"/Equipment");cloth.SetActive(false);
                    node.AddComponent<CharacterVisualModel>().Configure(sword,cloth);
                    var prefab=PrefabUtility.SaveAsPrefabAsset(node,Prefabs+"/"+id+"_Proxy.prefab");
                    var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);catalog.FindEntry(id).Prefab=prefab;EditorUtility.SetDirty(catalog);
                    var file=id=="monk"?"MONJE_HOJA_PROPUESTA_v02.png":id=="warrior"?"GUERRERO_HOJA_PROPUESTA_v01.png":"MAGO_HOJA_PROPUESTA_v01.png";
                    File.Copy("../Documentacion/Personajes/Conceptos/"+file,Root+"/Source/"+file);AssetDatabase.ImportAsset(Root+"/Source/"+file,ImportAssetOptions.ForceSynchronousImport);
                }
                finally{Object.DestroyImmediate(node);}
            }
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics)throw new InvalidOperationException("E03 changed world physics.");
            AssetDatabase.SaveAssets();Debug.Log("E03_CREATED: three baked provisional proxies, original sources and shared equipment. No rig/animations or physics changes.");
        }
        private static Material Old(string name)=>AssetDatabase.LoadAssetAtPath<Material>(CharacterVisualBuilder.Root+"/Materials/"+name+".mat");
        private static Material Mat(string name)=>AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/"+name+".mat");
        private static void Tint(string name,Material basis,Color color){var m=new Material(basis){name="E03_"+name};m.SetColor("_BaseColor",color);AssetDatabase.CreateAsset(m,Root+"/Materials/"+name+".mat");}
        private static Vector3 V(float x,float y,float z)=>new Vector3(x,y,z);
        private static void Body(MonkProxyGeometry m,string id)
        {
            var monk=id=="monk";var warrior=id=="warrior";var skin=Old("Skin");var leather=Old("Leather");var dark=AssetDatabase.LoadAssetAtPath<Material>(MonkIdentityBuilder.Root+"/Materials/DarkTrousers.mat");
            var cloth=monk?skin:warrior?dark:Mat("Plum");var steel=Mat("Steel");var linen=Old("MonkCloth");var hair=Old("Hair");var height=monk?1.78f:1.80f;
            m.Ellipsoid(skin,V(0,height-.12f,0),V(.18f,.24f,.19f));m.Ellipsoid(skin,V(0,1.51f,0),V(.115f,.12f,.115f));
            m.Ellipsoid(cloth,V(0,1.23f,0),V(warrior?.43f:monk?.37f:.32f,.48f,.25f));m.Ellipsoid(dark,V(0,.94f,0),V(.35f,.23f,.24f));
            m.Ellipsoid(skin,V(0,height-.13f,.098f),V(.032f,.044f,.035f));
            if(!monk){m.Ellipsoid(hair,V(0,height-.034f,-.012f),V(.185f,.064f,.18f));m.Ellipsoid(hair,V(0,height-.197f,.045f),V(.13f,.052f,.115f));}
            foreach(var s in new[]{-1,1})
            {
                var x=s*(warrior?.235f:.20f);
                m.Ellipsoid(cloth,V(x,1.39f,0),V(.17f,.21f,.18f));m.Ellipsoid(cloth,V(s*.24f,1.27f,0),V(.14f,.28f,.15f));
                m.Ellipsoid(cloth,V(s*.26f,1.13f,0),V(.115f,.12f,.13f));m.Ellipsoid(cloth,V(s*.275f,1.005f,.008f),V(.11f,.25f,.13f));
                m.Ellipsoid(monk?skin:leather,V(s*.275f,.85f,.02f),V(.08f,.15f,.085f));
                m.Ellipsoid(hair,V(s*.037f,height-.096f,.087f),V(.018f,.012f,.014f));
                m.Tube(dark,s*.105f,0,new[]{.23f,.43f,.65f,.92f},new[]{.063f,.115f,monk?.14f:.11f,.12f});
                m.Ellipsoid(leather,V(s*.105f,.075f,.035f),V(.15f,.15f,.26f));
                if(monk){m.Tube(linen,s*.275f,.008f,new[]{.91f,1.02f,1.13f},new[]{.052f,.057f,.061f});m.Tube(linen,s*.105f,0,new[]{.105f,.23f,.35f},new[]{.058f,.068f,.078f});}
                else{m.Tube(leather,s*.105f,0,new[]{.10f,.22f,.37f},new[]{.065f,.072f,.078f});m.Box(leather,V(s*.20f,.87f,.11f),V(.085f,.13f,.09f));}
                if(warrior)
                {m.Ellipsoid(steel,V(s*.24f,1.43f,.005f),V(.20f,.16f,.24f));m.Ellipsoid(steel,V(s*.275f,1.02f,.027f),V(.125f,.22f,.15f));m.Ellipsoid(steel,V(s*.105f,.43f,.09f),V(.13f,.16f,.08f));}
            }
            if(warrior)
            {
                m.Ellipsoid(steel,V(0,1.25f,.015f),V(.44f,.43f,.28f));
                for(var i=0;i<3;i++)m.Box(steel,V(0,1.07f-i*.055f,.015f),V(.40f+i*.012f,.05f,.28f));
                SplitPanels(m,dark,.96f,.54f,.22f);m.Box(Old("Sash"),V(.06f,.77f,.175f),V(.055f,.32f,.02f));
            }
            else if(monk)
            {
                var orange=AssetDatabase.LoadAssetAtPath<Material>(MonkIdentityBuilder.Root+"/Materials/BurntOrange.mat");
                for(var side=-1;side<=1;side+=2)
                {
                    m.Quad(orange,V(-.28f,1.46f,side*.09f),V(-.06f,1.46f,side*.09f),V(.19f,1.01f,side*.14f),V(-.02f,1.01f,side*.14f),true);
                }
                m.Quad(orange,V(-.28f,1.46f,-.09f),V(-.28f,1.46f,.09f),V(-.06f,1.46f,.09f),V(-.06f,1.46f,-.09f),true);SplitPanels(m,orange,.96f,.55f,.24f);
                m.Ellipsoid(leather,V(.17f,.86f,.17f),V(.035f,.05f,.035f));
            }
            else
            {
                m.Tube(dark,0,0,new[]{.18f,.64f,.98f},new[]{.18f,.17f,.145f});
                for(var i=0;i<10;i++)
                {
                    var a=i*Mathf.PI*2/10;var b=(i+1)*Mathf.PI*2/10;if(Mathf.Sin(a)>.8f)continue;
                    m.Quad(Mat("Plum"),V(Mathf.Cos(a)*.25f,.18f,Mathf.Sin(a)*.25f),V(Mathf.Cos(b)*.25f,.18f,Mathf.Sin(b)*.25f),V(Mathf.Cos(b)*.165f,.98f,Mathf.Sin(b)*.165f),V(Mathf.Cos(a)*.165f,.98f,Mathf.Sin(a)*.165f),true);
                }
                m.Tube(Mat("BlueGray"),0,0,new[]{1.23f,1.43f},new[]{.275f,.18f});
                m.Ellipsoid(Mat("Plum"),V(0,1.46f,-.08f),V(.18f,.12f,.12f));
                m.Box(linen,V(-.22f,.83f,.09f),V(.12f,.19f,.07f));m.Box(leather,V(-.22f,.83f,.13f),V(.13f,.20f,.018f));
                // Identity prop only: no staff inventory item exists in the current prototype.
                m.Tube(leather,.365f,.04f,new[]{.035f,1.57f},new[]{.022f,.022f});m.Ellipsoid(steel,V(.365f,1.62f,.04f),V(.075f,.12f,.075f));m.Ellipsoid(Mat("BlueGray"),V(.365f,1.63f,.07f),V(.045f,.07f,.045f));
            }
            m.Box(leather,V(0,.98f,0),V(warrior?.44f:.37f,.075f,.30f));
            for(var side=-1;side<=1;side+=2)m.Quad(leather,V(-.16f,1.44f,side*.15f),V(-.12f,1.44f,side*.15f),V(.12f,1.01f,side*.16f),V(.08f,1.01f,side*.16f),true);
        }
        private static void SplitPanels(MonkProxyGeometry m,Material cloth,float top,float bottom,float width)
        {
            foreach(var z in new[]{-1,1})foreach(var s in new[]{-1,1})m.Quad(cloth,V(s*.025f,bottom,z*.18f),V(s*width,bottom+.025f,z*.17f),V(s*.19f,top,z*.145f),V(s*.015f,top,z*.145f),true);
        }
    }
}
