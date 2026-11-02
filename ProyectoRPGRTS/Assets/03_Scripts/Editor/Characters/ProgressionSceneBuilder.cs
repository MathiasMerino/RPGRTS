using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;
namespace Aethernia.Editor.Characters
{
    public static class ProgressionSceneBuilder
    {
        public const string Root="Assets/14_Progression";
        public const string ScenePath="Assets/01_Scenes/AetherniaProgressionDemo.unity";
        public const string RulesPath=Root+"/PrototypeRules.asset";
        public static void Create()
        {
            if(File.Exists(ScenePath)||File.Exists(RulesPath))throw new InvalidOperationException("Progression scene/assets exist; preserve them.");
            AlphaPipelineBuilder.Folder(Root);var scene=EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);var session=Object.FindAnyObjectByType<GameSession>();var old=session.Rules;
            var progression=ScriptableObject.CreateInstance<ProgressionRules>();AssetDatabase.CreateAsset(progression,Root+"/ProgressionRules.asset");var rules=Object.Instantiate(old);rules.name="Progression Prototype Rules";rules.Progression=progression;AssetDatabase.CreateAsset(rules,RulesPath);
            foreach(var component in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {if(component==null)continue;var serialized=new SerializedObject(component);var it=serialized.GetIterator();while(it.NextVisible(true))if(it.propertyType==SerializedPropertyType.ObjectReference&&it.objectReferenceValue==old)it.objectReferenceValue=rules;serialized.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.RecordPrefabInstancePropertyModifications(component);EditorUtility.SetDirty(component);}
            EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();Debug.Log("E18_CREATED: opt-in progression rules, shared E17 art/physics, historical scenes intact.");
        }
    }
}
