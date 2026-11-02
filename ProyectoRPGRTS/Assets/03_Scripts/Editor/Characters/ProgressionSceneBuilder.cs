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
        public static void Build()
        {Directory.CreateDirectory("Builds/ProgressionE18Windows");var report=UnityEditor.BuildPipeline.BuildPlayer(new UnityEditor.BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/ProgressionE18Windows/Aethernia.exe",target=UnityEditor.BuildTarget.StandaloneWindows64,options=UnityEditor.BuildOptions.Development});if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new InvalidOperationException("E18 build failed");Directory.CreateDirectory("Logs/E18");File.WriteAllText("Logs/E18/build.txt","PASS Unity "+Application.unityVersion+"; E18 provisional progression candidate; bytes="+report.summary.totalSize+"; seconds="+report.summary.totalTime.TotalSeconds+"; native validation separate");}
        public static void Balance()
        {var rules=AssetDatabase.LoadAssetAtPath<PrototypeRules>(RulesPath);rules.Progression.DungeonHealthBonus=4;rules.Progression.DungeonDamageBonus=1;EditorUtility.SetDirty(rules.Progression);var expedition=AssetDatabase.LoadAssetAtPath<ExpeditionRules>(Root+"/ExpeditionRules.asset");expedition.OutpostDefenders=4;EditorUtility.SetDirty(expedition);AssetDatabase.SaveAssets();}
        public static void Troops()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var party=Object.FindAnyObjectByType<PartyWorld>();
            var units=Clone(flow.Director.Rts.DemoRules,Root+"/RtsRoles.asset");
            foreach(var r in units.Roles){r.Health=r.Id=="swordsman"?12:10;r.Damage=r.Id=="brawler"?3:2;r.Speed=r.Id=="archer"?2.3f:r.Id=="brawler"?3.2f:2.5f;r.RecruitGold=r.Id=="archer"?7:r.Id=="swordsman"?6:5;r.RecruitFood=2;}EditorUtility.SetDirty(units);
            var settlement=Clone(flow.Session.Settlement,Root+"/SettlementRules.asset");settlement.Units=units;EditorUtility.SetDirty(settlement);
            var expedition=Clone(party.Rules,Root+"/ExpeditionRules.asset");expedition.Units=units;EditorUtility.SetDirty(expedition);
            var oldUnits=flow.Director.Rts.DemoRules;var oldSettlement=flow.Session.Settlement;var oldExpedition=party.Rules;
            foreach(var component in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {if(component==null)continue;var data=new SerializedObject(component);var it=data.GetIterator();while(it.NextVisible(true))if(it.propertyType==SerializedPropertyType.ObjectReference){var value=it.objectReferenceValue;if(value==oldUnits)it.objectReferenceValue=units;else if(value==oldSettlement)it.objectReferenceValue=settlement;else if(value==oldExpedition)it.objectReferenceValue=expedition;}data.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.RecordPrefabInstancePropertyModifications(component);EditorUtility.SetDirty(component);}
            flow.Session.Rules.Progression.Units=units;EditorUtility.SetDirty(flow.Session.Rules.Progression);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        private static T Clone<T>(T original,string path) where T:UnityEngine.Object
        {var result=AssetDatabase.LoadAssetAtPath<T>(path);if(result!=null)return result;result=Object.Instantiate(original);AssetDatabase.CreateAsset(result,path);return result;}
        public static void Items()
        {
            var rules=AssetDatabase.LoadAssetAtPath<PrototypeRules>(RulesPath);var path=Root+"/Items.asset";var catalog=AssetDatabase.LoadAssetAtPath<ItemCatalog>(path);if(catalog==null){catalog=ScriptableObject.CreateInstance<ItemCatalog>();AssetDatabase.CreateAsset(catalog,path);}rules.Progression.Items=catalog;if(rules.Progression.DungeonLoot==null||rules.Progression.DungeonLoot.Length==0)rules.Progression.DungeonLoot=new[]{new ItemStack("forge-ore",1),new ItemStack("pilgrim-charm",1)};EditorUtility.SetDirty(rules.Progression);var scene=EditorSceneManager.OpenScene(ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var view=flow.GetComponent<ProgressionInventoryView>();if(view==null)view=flow.gameObject.AddComponent<ProgressionInventoryView>();view.Flow=flow;EditorUtility.SetDirty(view);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        public static void TechniqueUi()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var view=flow.GetComponent<ProgressionPanelView>();if(view==null)view=flow.gameObject.AddComponent<ProgressionPanelView>();view.Flow=flow;EditorUtility.SetDirty(view);var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var data=new SerializedObject(hud);var bindings=data.FindProperty("bindings");
            for(var i=0;i<bindings.arraySize;i++)
            {
                var binding=bindings.GetArrayElementAtIndex(i);var id=binding.FindPropertyRelative("Id");if(id.stringValue!="escalate")continue;id.stringValue="technique";
                var button=binding.FindPropertyRelative("Button").objectReferenceValue as UnityEngine.UI.Button;var label=button.GetComponentInChildren<UnityEngine.UI.Text>(true);label.text="Técnica";PrefabUtility.RecordPrefabInstancePropertyModifications(label);EditorUtility.SetDirty(label);
            }
            data.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.RecordPrefabInstancePropertyModifications(hud);EditorUtility.SetDirty(hud);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
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
