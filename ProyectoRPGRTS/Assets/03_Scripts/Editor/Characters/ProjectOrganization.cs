using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Aethernia.Editor.Characters
{
    // Maintenance only: moves preserve GUIDs and the historical/gameplay scene assets.
    public static class ProjectOrganization
    {
        private const string Output="../Docs/Validacion/Organizacion";
        private static bool Private(string p)=>p=="Assets/LocalOnly"||p.StartsWith("Assets/LocalOnly/",StringComparison.Ordinal)||p=="Assets/07_Characters/MonkMeshy"||p.StartsWith("Assets/07_Characters/MonkMeshy/",StringComparison.Ordinal);
        private static HashSet<string> Dependencies()=>new HashSet<string>(AssetDatabase.GetDependencies(ProgressionSceneBuilder.ScenePath,true).Select(AssetDatabase.AssetPathToGUID));
        [MenuItem("Aethernia/Proyecto/Organizar archivos sueltos y auditar")]
        public static void Apply()
        {
            Directory.CreateDirectory(Output);var before=Dependencies();
            Folder("Assets/Settings/Input");Folder("Assets/05_Docs/PlantillaUnity");
            Move("Assets/InputSystem_Actions.inputactions","Assets/Settings/Input/InputSystem_Actions.inputactions");
            Move("Assets/Readme.asset","Assets/05_Docs/PlantillaUnity/Readme.asset");
            AssetDatabase.SaveAssets();var after=Dependencies();if(!before.SetEquals(after))throw new InvalidOperationException("Scene dependency GUID set changed");
            var scene=EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);var missing=0;
            foreach(var root in scene.GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<Transform>(true))missing+=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
            if(missing!=0)throw new InvalidOperationException("Missing scene scripts: "+missing);
            var dependencyPaths=new HashSet<string>(AssetDatabase.GetDependencies(ProgressionSceneBuilder.ScenePath,true));
            var rows=new List<string>{"path,guid,classification"};var privateCount=0;
            foreach(var guid in AssetDatabase.FindAssets("",new[]{"Assets"}).OrderBy(x=>x,StringComparer.Ordinal))
            {
                var path=AssetDatabase.GUIDToAssetPath(guid);if(Private(path)){privateCount++;continue;}if(AssetDatabase.IsValidFolder(path))continue;
                var category=dependencyPaths.Contains(path)?"dependencia_actual_E18":path.EndsWith(".cs",StringComparison.Ordinal)||path.EndsWith(".asmdef",StringComparison.Ordinal)?"codigo_y_herramientas_conservar":path.EndsWith(".unity",StringComparison.Ordinal)?"escena_regresion_conservar":path.EndsWith(".docx",StringComparison.Ordinal)?"fuente_diseno_conservar":"reutilizable_provisional_o_historico_conservar";
                rows.Add("\""+path.Replace("\"","\"\"")+"\","+guid+","+category);
            }
            File.WriteAllLines(Output+"/RECURSOS_UNITY.csv",rows);
            File.WriteAllText(Output+"/UNITY_ORGANIZATION.txt","PASS Unity "+Application.unityVersion+"; moved2asset/meta pairs with preserved GUID; dependencyGUIDs="+after.Count+" unchanged; missingScripts="+missing+"; inventoriedPublicAssets="+(rows.Count-1)+"; privateEntriesExcluded="+privateCount+"; no scenes saved, no gameplay or asset deletion.");
            Debug.Log("ORGANIZATION_PASS: GUID dependencies preserved; scene loads; no missing scripts.");
        }
        private static void Folder(string path)
        {if(AssetDatabase.IsValidFolder(path))return;var parent=path.Substring(0,path.LastIndexOf('/'));Folder(parent);AssetDatabase.CreateFolder(parent,path.Substring(path.LastIndexOf('/')+1));}
        private static void Move(string from,string to)
        {
            var source=AssetDatabase.AssetPathToGUID(from);var target=AssetDatabase.AssetPathToGUID(to);
            if(string.IsNullOrEmpty(source)){if(string.IsNullOrEmpty(target))throw new FileNotFoundException(from);return;}
            if(!string.IsNullOrEmpty(target))throw new InvalidOperationException("Destination exists: "+to);
            var error=AssetDatabase.MoveAsset(from,to);if(!string.IsNullOrEmpty(error))throw new InvalidOperationException(error);
            if(AssetDatabase.AssetPathToGUID(to)!=source)throw new InvalidOperationException("GUID changed: "+to);
        }
    }
}
