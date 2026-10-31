using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class HumanoidImportPipeline
    {
        public static void ConfigureFbx(ModelImporter importer, ModelImporterAnimationType rigType = ModelImporterAnimationType.Human)
        {
            importer.globalScale = 1; importer.useFileScale = true;
            importer.addCollider = false; importer.importCameras = false; importer.importLights = false;
            importer.animationType = rigType; importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.optimizeGameObjects = false; importer.importAnimation = true;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            // No SaveAndReimport here: the explicit editor action controls that mutation.
        }
        public static List<string> Audit(GameObject model)
        {
            var errors = new List<string>();
            if (model == null) { errors.Add("Falta el prefab/modelo."); return errors; }
            if ((model.transform.localScale-Vector3.one).sqrMagnitude>.0001f) errors.Add("La raíz debe tener escala uno.");
            if (Quaternion.Angle(model.transform.localRotation,Quaternion.identity)>.1f) errors.Add("La raíz debe tener rotación cero; orientar el modelo a +Z al exportar.");
            if (model.GetComponentsInChildren<Collider>(true).Length>0 || model.GetComponentsInChildren<NavMeshAgent>(true).Length>0 || model.GetComponentsInChildren<Rigidbody>(true).Length>0)
                errors.Add("El visual no puede incluir colliders, Rigidbody o NavMeshAgent.");
            var animator=model.GetComponentInChildren<Animator>(true);
            if(animator==null || animator.avatar==null || !animator.avatar.isValid || !animator.avatar.isHuman) errors.Add("Falta un Avatar Humanoid válido; revisar Rig/Configure.");
            else if(animator.GetBoneTransform(HumanBodyBones.LeftHand)==null || animator.GetBoneTransform(HumanBodyBones.RightHand)==null) errors.Add("Faltan manos mapeadas en el Avatar.");
            if(animator!=null && animator.applyRootMotion) errors.Add("Desactivar Apply Root Motion: el agente controla el desplazamiento.");
            var skins=model.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            if(skins.Length==0 || !skins.Any(s=>s.sharedMesh!=null&&s.bones.Length>0&&s.bones.All(b=>b!=null))) errors.Add("Falta una malla skinned con huesos válidos.");
            var renderers=model.GetComponentsInChildren<Renderer>(true);
            if(renderers.Length==0) errors.Add("Falta geometría visual; la plantilla vacía no es un personaje importado.");
            else
            {
                var bounds=renderers[0].bounds; foreach(var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                if(bounds.size.y<1.70f || bounds.size.y>1.90f) errors.Add("Talla piloto fuera de 1,70–1,90 m; objetivo 1,80 m, sin escalar el controlador.");
                if(Mathf.Abs(bounds.min.y-model.transform.position.y)>.05f) errors.Add("El pivote debe situar los pies a Y=0 (tolerancia 5 cm).");
                foreach(var renderer in renderers) foreach(var material in renderer.sharedMaterials)
                    if(material==null || material.shader==null || !material.shader.name.StartsWith("Universal Render Pipeline/",StringComparison.Ordinal))
                        errors.Add("Remapear todos los materiales a URP; no se convierten shaders a ciegas.");
            }
            return errors.Distinct().ToList();
        }
        public static GameObject CreateCandidate(GameObject fbx, HumanoidAnimationProfile profile, Material overrideMaterial, string path)
        {
            var source=AssetDatabase.GetAssetPath(fbx);
            if(fbx==null || !source.EndsWith(".fbx",StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Selecciona un FBX real importado.");
            if(!path.StartsWith(HumanoidPipelineBuilder.Prefabs+"/",StringComparison.Ordinal) || !path.EndsWith(".prefab",StringComparison.OrdinalIgnoreCase) || path.Contains(".."))
                throw new InvalidOperationException("Guardar el candidato dentro de la carpeta de prefabs Humanoid.");
            if(AssetDatabase.LoadMainAssetAtPath(path)!=null) throw new InvalidOperationException("No sobrescribir un prefab existente.");
            var root=new GameObject(System.IO.Path.GetFileNameWithoutExtension(path));
            try
            {
                var body=(GameObject)PrefabUtility.InstantiatePrefab(fbx,root.transform);
                var animator=body.GetComponentInChildren<Animator>(true);
                if(animator!=null) { animator.enabled=false; animator.applyRootMotion=false; animator.fireEvents=false; }
                if(overrideMaterial!=null)
                    foreach(var renderer in body.GetComponentsInChildren<Renderer>(true))
                    {
                        renderer.sharedMaterials=Enumerable.Repeat(overrideMaterial,renderer.sharedMaterials.Length).ToArray();
                        PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
                    }
                var errors=Audit(root); if(errors.Count>0) throw new InvalidOperationException(string.Join("\n",errors));
                var rig=root.AddComponent<HumanoidVisualRig>(); rig.Configure(animator);
                root.AddComponent<HumanoidAnimationDriver>().Configure(rig,profile);
                PrefabUtility.RecordPrefabInstancePropertyModifications(animator);
                var bindings=HumanoidPipelineBuilder.CreateSockets(root.transform,animator);
                root.AddComponent<CharacterVisualModel>().ConfigureBindings(bindings);
                return PrefabUtility.SaveAsPrefabAsset(root,path);
            }
            finally { Object.DestroyImmediate(root); }
        }
    }
}
