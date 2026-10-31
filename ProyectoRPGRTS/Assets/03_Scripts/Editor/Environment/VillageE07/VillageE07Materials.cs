using System;
using UnityEditor;
using UnityEngine;

namespace Aethernia.Editor.Environment
{
    public static class VillageE07Materials
    {
        public const string Root="Assets/06_Environment/VillageE07";
        public const string Prefabs="Assets/02_Prefabs/Environment/VillageE07";
        public static Material Load(string name)
        {
            if(name=="Iron")return EnvironmentMaterials.Load("Iron");
            if(name=="Leather"||name=="Linen")return AssetDatabase.LoadAssetAtPath<Material>("Assets/07_Characters/Provisional/Materials/"+(name=="Linen"?"MonkCloth":"Leather")+".mat");
            var m=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/"+name+".mat");
            if(m==null)throw new InvalidOperationException("Missing E07 material: "+name);return m;
        }
        public static void Create()
        {
            if(AssetDatabase.IsValidFolder(Root+"/Materials"))throw new InvalidOperationException("E07 materials exist; do not regenerate.");
            EnvironmentMaterials.Folder(Root+"/Materials");EnvironmentMaterials.Folder(Root+"/Meshes");
            foreach(var name in new[]{"Stone","Wood","Soil","Slate","Grass"})
            {
                var path=Root+"/Textures/"+name+"_Albedo.png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);
                if(importer==null)throw new InvalidOperationException("Import original texture first: "+path);
                importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.mipmapEnabled=true;
                importer.wrapMode=TextureWrapMode.Repeat;importer.filterMode=FilterMode.Trilinear;importer.anisoLevel=4;
                importer.maxTextureSize=1024;importer.npotScale=TextureImporterNPOTScale.None;importer.isReadable=false;importer.SaveAndReimport();
            }
            Make("AgedOak","Wood",new Color(.9f,.85f,.76f),.18f,"Oak");
            Make("DarkOak","Wood",new Color(.50f,.48f,.43f),.16f,"Oak");
            Make("GreyStone","Stone",new Color(.85f,.9f,.94f),.17f,"Stone");
            Make("WarmStone","Stone",new Color(1,.93f,.78f),.18f,"Stone");
            Make("Slate","Slate",new Color(.77f,.82f,.89f),.24f,"Slate");
            Make("Earth","Soil",new Color(.83f,.77f,.68f),.10f,"Earth");
            Make("Grass","Grass",new Color(.93f,.94f,.80f),.09f,"Ground");
            Make("ForestFloor","Soil",new Color(.47f,.49f,.35f),.10f,"Ground");
            Clone("Plaster","Plaster",new Color(.90f,.88f,.81f));
            Clone("LeafLight","Foliage",new Color(.76f,.85f,.63f));
            Clone("LeafDark","Foliage",new Color(.47f,.63f,.45f));
            var bronze=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="E07_Bronze",enableInstancing=true};
            bronze.SetColor("_BaseColor",new Color(.37f,.22f,.08f));bronze.SetFloat("_Metallic",.78f);bronze.SetFloat("_Smoothness",.38f);Save("Bronze",bronze);
            var glow=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="E07_EmberGlass",enableInstancing=true};
            glow.SetColor("_BaseColor",new Color(.51f,.28f,.09f));glow.SetColor("_EmissionColor",new Color(2.1f,.85f,.18f));glow.EnableKeyword("_EMISSION");glow.SetFloat("_Smoothness",.25f);Save("EmberGlass",glow);
            AssetDatabase.SaveAssets();
        }
        private static void Make(string name,string map,Color tint,float smooth,string microDetail)
        {
            var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="E07_"+name,enableInstancing=true};
            m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+map+"_Albedo.png"));m.SetColor("_BaseColor",tint);
            // Existing tiny procedural relief is shared, not claimed as a scanned/generated PBR normal.
            m.SetTexture("_BumpMap",EnvironmentMaterials.Load(microDetail).GetTexture("_BumpMap"));m.SetFloat("_BumpScale",.16f);m.EnableKeyword("_NORMALMAP");
            m.SetFloat("_Smoothness",smooth);m.SetFloat("_Metallic",0);Save(name,m);
        }
        private static void Clone(string name,string source,Color tint)
        {var m=new Material(EnvironmentMaterials.Load(source)){name="E07_"+name,enableInstancing=true};m.SetColor("_BaseColor",tint);Save(name,m);}
        private static void Save(string name,Material material)=>AssetDatabase.CreateAsset(material,Root+"/Materials/"+name+".mat");
    }
}
