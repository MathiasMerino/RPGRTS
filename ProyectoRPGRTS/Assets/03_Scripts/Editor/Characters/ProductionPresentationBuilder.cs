using System;
using System.Linq;
using Aethernia.Editor.Environment;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class ProductionPresentationBuilder
    {
        public static void Configure()
        {
            var root=ProductionCharacterBuilder.Root;var portraits=root+"/Portraits";
            if(AssetDatabase.IsValidFolder(portraits))throw new InvalidOperationException("E04 already configured; do not regenerate.");
            EnvironmentMaterials.Folder(portraits);var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            foreach(var e in catalog.Entries)
            {
                var id=e.ClassId;var title=id=="monk"?"Monje":id=="warrior"?"Guerrero":"Mago";
                var file=id=="monk"?"MONJE_HOJA_PROPUESTA_v02.png":id=="warrior"?"GUERRERO_HOJA_PROPUESTA_v01.png":"MAGO_HOJA_PROPUESTA_v01.png";
                var path=root+"/Source/"+file;var importer=(TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.npotScale=TextureImporterNPOTScale.None;importer.mipmapEnabled=false;importer.maxTextureSize=2048;importer.wrapMode=TextureWrapMode.Clamp;importer.sRGBTexture=true;importer.SaveAndReimport();
                var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var x=id=="monk"?195:id=="warrior"?192:160;var rect=new Rect(x,texture.height-38-240,180,240);
                var sprite=Sprite.Create(texture,rect,Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name=title+"_Portrait";AssetDatabase.CreateAsset(sprite,portraits+"/"+id+".asset");
                e.DisplayTitle=title;e.PortraitSprite=sprite;e.ConceptSheet=texture;e.ReferenceArt=texture;e.ReferenceUv=new Rect(rect.x/texture.width,rect.y/texture.height,rect.width/texture.width,rect.height/texture.height);
                e.ReferenceCaption="Concepto provisional\n"+title;e.ReferenceHeightMeters=id=="monk"?1.78f:1.80f;e.IsProvisional=true;
                e.PreviousPrefab=AssetDatabase.LoadAssetAtPath<GameObject>(id=="monk"?MonkIdentityBuilder.ProxyPath:CharacterVisualBuilder.Prefabs+"/"+id+"_Visual.prefab");
                e.EquipmentPolicy=id=="mage"?"Bastón/grimorio: identidad, sin item o bonus. Espada/tela observan estado v1.":id=="monk"?"Sin arma inicial; espada solo si equipada, conforme a reglas v1.":"Armadura de identidad no otorga bonus. Espada/tela observan estado v1.";
                var prefabPath=AssetDatabase.GetAssetPath(e.Prefab);var node=PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    var model=node.GetComponent<CharacterVisualModel>();var anchor=node.transform.Find("EquipmentRoot/RightHand_Weapon");model.ConfigureAnchor(anchor);
                    var sword=anchor.GetChild(0).gameObject;var cloth=node.transform.Find("EquipmentRoot/EquippedCloth_Overlay").gameObject;
                    model.Configure(null,null);model.ConfigureBindings(new[]{new EquipmentVisualBinding{Slot=VisualEquipmentSlot.Weapon,Source=EquipmentStateSource.Weapon,ItemId="training-sword",Visual=sword},new EquipmentVisualBinding{Slot=VisualEquipmentSlot.Armour,Source=EquipmentStateSource.Armour,ItemId="cloth",Visual=cloth}});
                    PrefabUtility.SaveAsPrefabAsset(node,prefabPath);
                }finally{PrefabUtility.UnloadPrefabContents(node);}
                e.SharedMaterials=e.Prefab.GetComponentsInChildren<Renderer>(true).Select(r=>r.sharedMaterial).Distinct().ToArray();
            }
            SetTint("Plum",new Color32(113,83,137,255));SetTint("BlueGray",new Color32(112,142,167,255));
            EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();Debug.Log("E04_CONFIGURED: native 3:4 portrait sprites/UV crops, original sheets, shared metadata/materials and equipment bindings; no gameplay stats copied.");
        }
        private static void SetTint(string name,Color color){var m=AssetDatabase.LoadAssetAtPath<Material>(ProductionCharacterBuilder.Root+"/Materials/"+name+".mat");m.SetColor("_BaseColor",color);EditorUtility.SetDirty(m);}
    }
}
