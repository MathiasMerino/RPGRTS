using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;
namespace Aethernia.Editor.Characters
{
    public static class CivilizationSceneBuilder
    {
        public const string Root="Assets/15_Civilizations",ScenePath="Assets/01_Scenes/AetherniaCivilizationsDemo.unity",RulesPath=Root+"/PrototypeRules.asset";
        public static void Build()
        {Directory.CreateDirectory("Builds/CivilizationsE19Windows");var report=UnityEditor.BuildPipeline.BuildPlayer(new UnityEditor.BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/CivilizationsE19Windows/Aethernia.exe",target=UnityEditor.BuildTarget.StandaloneWindows64,options=UnityEditor.BuildOptions.Development});if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new InvalidOperationException("E19 build failed");Directory.CreateDirectory("Logs/E19");File.WriteAllText("Logs/E19/build.txt","PASS Unity "+Application.unityVersion+"; bytes="+report.summary.totalSize+"; seconds="+report.summary.totalTime.TotalSeconds);}
        private static T Clone<T>(T original,string name)where T:Object{var copy=Object.Instantiate(original);AssetDatabase.CreateAsset(copy,Root+"/"+name+".asset");return copy;}
        public static void Create()
        {
            if(File.Exists(ScenePath)||AssetDatabase.IsValidFolder(Root))throw new InvalidOperationException("E19 exists; do not regenerate existing assets.");
            AlphaPipelineBuilder.Folder(Root);AlphaPipelineBuilder.Folder(Root+"/Prefabs");var scene=EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var session=flow.Session;var oldRules=session.Rules;var oldSettlement=session.Settlement;var oldRoles=flow.Director.Rts.DemoRules;var oldContent=flow.Content;var oldExpedition=Object.FindAnyObjectByType<PartyWorld>().Rules;
            var catalog=ScriptableObject.CreateInstance<CivilizationCatalog>();AssetDatabase.CreateAsset(catalog,Root+"/Civilizations.asset");
            var roles=Clone(oldRoles,"RtsRoles");roles.Civilizations=catalog;
            var growth=Clone(oldRules.Progression,"ProgressionRules");growth.Units=roles;
            var rules=Clone(oldRules,"PrototypeRules");rules.Progression=growth;rules.Civilizations=catalog;
            var settlement=Clone(oldSettlement,"SettlementRules");settlement.Units=roles;settlement.Civilizations=catalog;
            var expedition=Clone(oldExpedition,"ExpeditionRules");expedition.Units=roles;
            expedition.Factions[0].Cultures=new[]{"frontier","sylvan","stonehall","clanroad"};expedition.Factions[1].Title="Liga de Frontera";expedition.Factions[1].CultureId="frontier";expedition.Factions[1].Cultures=new[]{"frontier","sylvan"};expedition.Factions[2].Title="Guarnición de Forja";expedition.Factions[2].CultureId="stonehall";expedition.Factions[2].Cultures=new[]{"stonehall","clanroad"};
            var content=Clone(oldContent,"DemoContent");content.Lineages=catalog.Races.Select(r=>new LineageProfile(r.Id,r.Title,r.Description+" "+r.Trait)).ToArray();content.VisualNotice="E19: proporciones y arte provisionales; cultura y facción independientes de raza. Todas las clases disponibles.";
            var originals=new Object[]{oldRules,oldRules.Progression,oldRoles,oldSettlement,oldExpedition,oldContent};var copies=new Object[]{rules,growth,roles,settlement,expedition,content};
            foreach(var component in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {if(component==null)continue;var data=new SerializedObject(component);var it=data.GetIterator();while(it.NextVisible(true))if(it.propertyType==SerializedPropertyType.ObjectReference){var index=Array.IndexOf(originals,it.objectReferenceValue);if(index>=0)it.objectReferenceValue=copies[index];}data.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.RecordPrefabInstancePropertyModifications(component);EditorUtility.SetDirty(component);}
            var construction=flow.Director.Construction;var serialized=new SerializedObject(construction);var bindings=serialized.FindProperty("blueprints");var count=bindings.arraySize;
            foreach(var culture in catalog.Cultures.Where(c=>c.Pilot))
            {
                GameObject original=null;for(var i=0;i<count;i++){var b=bindings.GetArrayElementAtIndex(i);if(b.FindPropertyRelative("Id").stringValue==culture.Building)original=b.FindPropertyRelative("Prefab").objectReferenceValue as GameObject;}
                if(original==null)throw new InvalidOperationException("Missing shared building "+culture.Building);
                var instance=(GameObject)PrefabUtility.InstantiatePrefab(original);instance.name=culture.Title+" "+culture.Building;
                // A small original sign, no collider/material duplicate; inherited building physics retained.
                var mark=GameObject.CreatePrimitive(PrimitiveType.Cube);mark.name="Cultural sign provisional";mark.transform.SetParent(instance.transform,false);mark.transform.localPosition=new Vector3(culture.Id=="frontier"?.8f:-.8f,2.2f,-1.15f);mark.transform.localScale=new Vector3(.32f,.24f,.08f);Object.DestroyImmediate(mark.GetComponent<Collider>());var shared=original.GetComponentInChildren<Renderer>().sharedMaterial;mark.GetComponent<Renderer>().sharedMaterial=shared;
                var variant=PrefabUtility.SaveAsPrefabAsset(instance,Root+"/Prefabs/"+culture.Id+"-"+culture.Building+".prefab");Object.DestroyImmediate(instance);
                var index=bindings.arraySize;bindings.InsertArrayElementAtIndex(index);var binding=bindings.GetArrayElementAtIndex(index);binding.FindPropertyRelative("Id").stringValue=culture.Building;binding.FindPropertyRelative("CultureId").stringValue=culture.Id;binding.FindPropertyRelative("Prefab").objectReferenceValue=variant;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.RecordPrefabInstancePropertyModifications(construction);
            foreach(var asset in copies){EditorUtility.SetDirty(asset);}EditorUtility.SetDirty(catalog);EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();Debug.Log("E19_CREATED four races/twelve combinations; two culture pilots, shared E07/E08 art, old scenes preserved.");
        }
    }
}
