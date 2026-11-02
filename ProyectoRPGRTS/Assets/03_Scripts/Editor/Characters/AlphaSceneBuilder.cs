using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;
namespace Aethernia.Editor.Characters
{
    public static class AlphaSceneBuilder
    {
        public const string ScenePath="Assets/01_Scenes/AetherniaAlphaCandidate.unity";
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var front=Object.FindAnyObjectByType<DemoFrontEnd>();front.AlphaLayout=true;front.AlphaClassOption=AssetDatabase.LoadAssetAtPath<GameObject>(AlphaPipelineBuilder.Root+"/Prefabs/ClassOption.prefab");front.Catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AlphaPipelineBuilder.CatalogPath);PrefabUtility.RecordPrefabInstancePropertyModifications(front);EditorUtility.SetDirty(front);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        public static void Create()
        {
            if(File.Exists(ScenePath))throw new InvalidOperationException("Alpha scene exists; preserve it.");
            var scene=EditorSceneManager.OpenScene(VisualPolishBuilder.ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var original=flow.Front.Catalog;var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AlphaPipelineBuilder.CatalogPath);if(catalog==null)throw new InvalidOperationException("Prepare pipeline first.");
            foreach(var component in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {if(component==null)continue;var so=new SerializedObject(component);var it=so.GetIterator();while(it.NextVisible(true))if(it.propertyType==SerializedPropertyType.ObjectReference&&it.objectReferenceValue==original)it.objectReferenceValue=catalog;so.ApplyModifiedPropertiesWithoutUndo();}
            AlphaPipelineBuilder.Folder(AlphaPipelineBuilder.Root+"/Prefabs");var ui=new DemoWidgets(flow.Front.Font,flow.Front.Presentation.Button,flow.Front.Presentation);var button=ui.Button(null,"Class option","",0,0,300,112,null);button.GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);var card=button.gameObject.AddComponent<ClassCardView>();card.Button=button;card.UsePortrait=true;
            var picture=DemoWidgets.Rect(button.transform,"Portrait",12,10,76,92);card.Figure=picture.gameObject.AddComponent<Image>();card.Figure.preserveAspect=true;card.Figure.raycastTarget=false;
            card.Title=ui.Text(button.transform,"Class title","",104,10,184,28,23);card.Title.color=flow.Front.Presentation.Gold;card.Role=ui.Text(button.transform,"Role","",104,42,184,38,14);card.Selection=ui.Text(button.transform,"Selection","",104,85,184,20,13);
            card.Outline=DemoWidgets.Stretch(button.transform,"Selection frame").gameObject.AddComponent<Image>();card.Outline.sprite=flow.Front.Presentation.Frame;card.Outline.type=Image.Type.Sliced;card.Outline.raycastTarget=false;
            flow.Front.AlphaClassOption=PrefabUtility.SaveAsPrefabAsset(button.gameObject,AlphaPipelineBuilder.Root+"/Prefabs/ClassOption.prefab");Object.DestroyImmediate(button.gameObject);flow.Front.AlphaLayout=true;PrefabUtility.RecordPrefabInstancePropertyModifications(flow.Front);EditorUtility.SetDirty(flow.Front);
            EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();Debug.Log("E17_ALPHA_SCENE_CREATED: optional layout, shared portraits and isolated previews, original scene preserved.");
        }
    }
}
