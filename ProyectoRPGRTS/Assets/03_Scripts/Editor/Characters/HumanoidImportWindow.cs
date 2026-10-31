using System;
using UnityEditor;
using UnityEngine;

namespace Aethernia.Editor.Characters
{
    public sealed class HumanoidImportWindow : EditorWindow
    {
        private GameObject fbx;
        private Material sharedMaterial;
        private HumanoidAnimationProfile profile;
        private string prefabName="Monk_Humanoid";
        private string report="Sin modelo seleccionado. E01 se conserva como alternativa.";
        [MenuItem("Aethernia/Personajes/Importar Humanoid")]
        private static void Open() { GetWindow<HumanoidImportWindow>("Importar Humanoid"); }
        private void OnEnable() { profile=AssetDatabase.LoadAssetAtPath<HumanoidAnimationProfile>(HumanoidPipelineBuilder.ProfilePath); }
        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Preparar un FBX aprobado. Crear un candidato no activa el modelo en el juego. No se convierte geometría provisional en personaje final.",MessageType.Info);
            fbx=(GameObject)EditorGUILayout.ObjectField("Modelo FBX",fbx,typeof(GameObject),false);
            sharedMaterial=(Material)EditorGUILayout.ObjectField("URP compartido (opcional)",sharedMaterial,typeof(Material),false);
            profile=(HumanoidAnimationProfile)EditorGUILayout.ObjectField("Perfil de clips",profile,typeof(HumanoidAnimationProfile),false);
            prefabName=EditorGUILayout.TextField("Nombre del candidato",prefabName);
            if(GUILayout.Button("1. Configurar importador nativo Humanoid")) Run(()=>
            {
                var path=AssetDatabase.GetAssetPath(fbx);
                if(!path.EndsWith(".fbx",StringComparison.OrdinalIgnoreCase) || !(AssetImporter.GetAtPath(path) is ModelImporter importer)) throw new InvalidOperationException("Selecciona un FBX importado.");
                HumanoidImportPipeline.ConfigureFbx(importer); importer.SaveAndReimport();
                report="Importador configurado. Revisar Rig/Configure, escala y remapeo URP; no implica Avatar aprobado.";
            });
            if(GUILayout.Button("2. Auditar escala / rig / materiales")) Run(()=>
            {
                if(fbx==null) throw new InvalidOperationException("Selecciona el modelo.");
                var instance=(GameObject)PrefabUtility.InstantiatePrefab(fbx);
                try { var issues=HumanoidImportPipeline.Audit(instance); report=issues.Count==0?"Auditoría técnica sin incidencias; revisar orientación y deformaciones visualmente.":string.Join("\n",issues); }
                finally { DestroyImmediate(instance); }
            });
            if(GUILayout.Button("3. Crear prefab candidato (sin activar)")) Run(()=>
            {
                if(string.IsNullOrWhiteSpace(prefabName) || prefabName.Contains("..") || prefabName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars())>=0) throw new InvalidOperationException("Usar un nombre de archivo válido, sin carpetas.");
                var path=AssetDatabase.GenerateUniqueAssetPath(HumanoidPipelineBuilder.Prefabs+"/"+prefabName+".prefab");
                var candidate=HumanoidImportPipeline.CreateCandidate(fbx,profile,sharedMaterial,path); Selection.activeObject=candidate;
                report="Candidato creado: "+path+". Asignar equipo/sockets, revisar y aprobar antes de enlazarlo al catálogo.";
            });
            if(GUILayout.Button("Actualizar estados desde clips del perfil")) Run(()=>
            { HumanoidPipelineBuilder.ApplyAnimationProfile(profile); report="Estados actualizados. Clips vacíos permanecen pendientes; esto no certifica animación."; });
            EditorGUILayout.HelpBox(report,MessageType.None);
        }
        private void Run(Action action) { try { action(); } catch(Exception error) { report=error.Message; } }
    }
}
