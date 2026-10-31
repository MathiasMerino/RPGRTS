using System;
using System.IO;
using Aethernia.Editor.Environment;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class MonkIdentityBuilder
    {
        public const string Root="Assets/07_Characters/MonkProvisional";
        public const string ProxyPath="Assets/02_Prefabs/Characters/MonkProvisional/Monk_ConceptProxy.prefab";
        public const string CardPath="Assets/02_Prefabs/UI/ClassReferenceCard.prefab";
        public static void Create()
        {
            var scene=EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath);
            if(AssetDatabase.IsValidFolder(Root)||AssetDatabase.LoadMainAssetAtPath(CardPath)!=null) throw new InvalidOperationException("E02-B already exists; do not regenerate.");
            var physics=EnvironmentPhysicsSnapshot.Capture(scene);var nav=MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath);
            foreach(var path in new[]{Root+"/Materials",Root+"/Meshes",Root+"/Textures","Assets/02_Prefabs/Characters/MonkProvisional"}) EnvironmentMaterials.Folder(path);
            var orange=Material("BurntOrange",new Color32(166,87,45,255));
            var dark=Material("DarkTrousers",new Color32(74,67,55,255));
            var skin=Old("Skin");var linen=Old("MonkCloth");var leather=Old("Leather");var charcoal=Old("Hair");
            var root=new GameObject("Monk_ConceptProxy_178cm"); GameObject proxy;
            try
            {
                var mesh=new MonkProxyGeometry();
                mesh.Ellipsoid(skin,new Vector3(0,1.66f,0),new Vector3(.18f,.24f,.19f));
                mesh.Ellipsoid(skin,new Vector3(0,1.51f,0),new Vector3(.105f,.11f,.105f));
                mesh.Ellipsoid(skin,new Vector3(0,1.23f,0),new Vector3(.37f,.47f,.235f));
                mesh.Ellipsoid(dark,new Vector3(0,.93f,0),new Vector3(.35f,.22f,.23f));
                mesh.Ellipsoid(skin,new Vector3(0,1.646f,.099f),new Vector3(.035f,.043f,.035f));
                foreach(var sign in new[]{-1,1})
                {
                    mesh.Ellipsoid(charcoal,new Vector3(sign*.037f,1.685f,.087f),new Vector3(.022f,.013f,.014f));
                    mesh.Ellipsoid(skin,new Vector3(sign*.215f,1.30f,0),new Vector3(.12f,.30f,.13f));
                    mesh.Ellipsoid(skin,new Vector3(sign*.25f,1.055f,.008f),new Vector3(.10f,.27f,.115f));
                    mesh.Ellipsoid(skin,new Vector3(sign*.25f,.855f,.018f),new Vector3(.075f,.13f,.075f));
                    mesh.Tube(linen,sign*.25f,.008f,new[]{.925f,1.01f,1.13f},new[]{.052f,.055f,.058f});
                    mesh.Tube(dark,sign*.105f,0,new[]{.29f,.43f,.65f,.87f},new[]{.076f,.125f,.145f,.115f});
                    mesh.Tube(linen,sign*.105f,0,new[]{.105f,.23f,.365f},new[]{.057f,.065f,.077f});
                    mesh.Ellipsoid(leather,new Vector3(sign*.105f,.075f,.035f),new Vector3(.15f,.15f,.26f));
                }
                // Same asymmetrical cloth on front and back; front is Unity +Z, left shoulder is -X.
                var left=new[]{new Vector3(-.275f,1.435f,.095f),new Vector3(-.22f,1.31f,.14f),new Vector3(-.14f,1.17f,.14f),new Vector3(-.025f,1.015f,.125f)};
                var right=new[]{new Vector3(-.075f,1.435f,.095f),new Vector3(.01f,1.31f,.14f),new Vector3(.10f,1.17f,.14f),new Vector3(.185f,1.015f,.125f)};
                for(var side=-1;side<=1;side+=2)
                {
                    for(var i=1;i<left.Length;i++) mesh.Quad(orange,Z(left[i],side),Z(right[i],side),Z(right[i-1],side),Z(left[i-1],side),true);
                    mesh.Quad(orange,new Vector3(-.27f,.56f,side*.18f),new Vector3(-.03f,.53f,side*.20f),new Vector3(-.015f,.96f,side*.145f),new Vector3(-.20f,.96f,side*.145f),true);
                    mesh.Quad(orange,new Vector3(.03f,.57f,side*.20f),new Vector3(.26f,.61f,side*.18f),new Vector3(.20f,.96f,side*.145f),new Vector3(.015f,.96f,side*.145f),true);
                }
                mesh.Quad(orange,Z(left[0],1),Z(left[0],-1),Z(right[0],-1),Z(right[0],1),true);
                mesh.Ellipsoid(leather,new Vector3(0,.98f,0),new Vector3(.38f,.11f,.29f));
                mesh.Quad(leather,new Vector3(.09f,.79f,.155f),new Vector3(.14f,.79f,.155f),new Vector3(.14f,.96f,.16f),new Vector3(.09f,.96f,.16f),true);
                var body=new GameObject("BodyAndGarments").transform; body.SetParent(root.transform,false);mesh.Bake(body,Root+"/Meshes");
                var equipment=new GameObject("EquipmentRoot").transform;equipment.SetParent(root.transform,false);
                var sword=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/TrainingSword_96cm.prefab"),equipment);
                sword.transform.localPosition=new Vector3(.26f,.765f,.10f);sword.SetActive(false);
                var vest=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/ClothArmourOverlay.prefab"),equipment);
                vest.transform.localRotation=Quaternion.Euler(0,180,0);vest.SetActive(false);
                root.AddComponent<CharacterVisualModel>().Configure(sword,vest);
                proxy=PrefabUtility.SaveAsPrefabAsset(root,ProxyPath);
            }
            finally {Object.DestroyImmediate(root);}
            var texturePath=Root+"/Textures/MonkConceptSheet.png";
            File.Copy("../Documentacion/Personajes/Conceptos/MONJE_HOJA_PROPUESTA_v01.png",texturePath);
            AssetDatabase.ImportAsset(texturePath,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(texturePath);
            importer.textureType=TextureImporterType.Default;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;
            importer.mipmapEnabled=false;importer.isReadable=false;importer.wrapMode=TextureWrapMode.Clamp;importer.sRGBTexture=true;importer.SaveAndReimport();
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            var entry=catalog.FindEntry("monk");entry.Prefab=proxy;entry.ReferenceArt=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            entry.ReferenceUv=new Rect(119f/1536,771f/1024,150f/1536,205f/1024);EditorUtility.SetDirty(catalog);
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();
            var card=CreateCard(catalog);var placed=(GameObject)PrefabUtility.InstantiatePrefab(card,hud.transform.Find("Context Actions"));
            placed.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-336);
            PrefabUtility.RecordPrefabInstancePropertyModifications(placed.GetComponent<RectTransform>());
            hud.ConfigureReference(placed.GetComponent<ClassReferenceCard>());EditorUtility.SetDirty(hud);
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics||MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath)!=nav) throw new InvalidOperationException("E02-B changed physics/navigation.");
            AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            Debug.Log("E02B_CREATED: baked Monk proxy, shared equipment, original concept UV crop and existing character page; physics/nav unchanged.");
        }
        private static Vector3 Z(Vector3 value,int sign)=>new Vector3(value.x,value.y,value.z*sign);
        private static Material Old(string name)=>AssetDatabase.LoadAssetAtPath<Material>(CharacterVisualBuilder.Root+"/Materials/"+name+".mat");
        private static Material Material(string name,Color tint)
        {
            var shared=EnvironmentMaterials.Load("Plaster");var mat=new Material(shared){name=name,enableInstancing=true};
            mat.SetColor("_BaseColor",new Color(tint.r*1.35f,tint.g*1.35f,tint.b*1.35f,1));mat.SetFloat("_BumpScale",.15f);
            AssetDatabase.CreateAsset(mat,Root+"/Materials/"+name+".mat");return mat;
        }
        private static GameObject CreateCard(CharacterVisualCatalog catalog)
        {
            var theme=AssetDatabase.LoadAssetAtPath<MedievalUiTheme>("Assets/Settings/Prototype/MedievalTheme.asset");
            var node=new GameObject("ClassReferenceCard",typeof(RectTransform));
            try
            {
                var rect=node.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,1);rect.sizeDelta=new Vector2(248,76);
                var imageNode=new GameObject("Portrait crop",typeof(RectTransform),typeof(RawImage),typeof(AspectRatioFitter));imageNode.transform.SetParent(node.transform,false);
                var image=imageNode.GetComponent<RawImage>();image.raycastTarget=false;
                var photo=imageNode.GetComponent<RectTransform>();photo.anchorMin=photo.anchorMax=photo.pivot=new Vector2(0,1);photo.anchoredPosition=Vector2.zero;photo.sizeDelta=new Vector2(55,76);
                var aspect=imageNode.GetComponent<AspectRatioFitter>();aspect.aspectMode=AspectRatioFitter.AspectMode.HeightControlsWidth;
                var textNode=new GameObject("Class identity",typeof(RectTransform),typeof(Text));textNode.transform.SetParent(node.transform,false);
                var text=textNode.GetComponent<Text>();text.font=theme.Font;text.fontSize=14;text.color=theme.Text;text.raycastTarget=false;text.supportRichText=false;
                var area=text.rectTransform;area.anchorMin=area.anchorMax=area.pivot=new Vector2(0,1);area.anchoredPosition=new Vector2(64,0);area.sizeDelta=new Vector2(184,76);
                node.AddComponent<ClassReferenceCard>().Configure(catalog,image,aspect,text);
                return PrefabUtility.SaveAsPrefabAsset(node,CardPath);
            }
            finally{Object.DestroyImmediate(node);}
        }
    }
}
