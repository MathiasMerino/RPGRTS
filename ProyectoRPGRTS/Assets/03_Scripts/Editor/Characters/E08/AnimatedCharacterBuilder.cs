using System;
using System.Linq;
using Aethernia.Editor.Environment;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class AnimatedCharacterBuilder
    {
        public const string Root="Assets/07_Characters/AnimatedE08";
        public const string Prefabs="Assets/02_Prefabs/Characters/E08";
        public const string CatalogPath=Root+"/ClassVisuals.asset";
        public const string ScenePath="Assets/01_Scenes/AetherniaVillageDemo.unity";
        public static void Create()
        {
            if(AssetDatabase.IsValidFolder(Root))throw new InvalidOperationException("E08 existe: no regenerar assets ni GUID.");
            foreach(var folder in new[]{Root+"/Meshes",Root+"/Animation",Root+"/Rigs",Prefabs})EnvironmentMaterials.Folder(folder);
            if(!AssetDatabase.CopyAsset(CharacterVisualBuilder.CatalogPath,CatalogPath))throw new InvalidOperationException("Catálogo fuente ausente.");
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CatalogPath);
            AnimatorController graph=null;AnimationClip[] common=null;
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                var node=new GameObject(id+"_E08_Animated");
                try
                {
                    var model=new GameObject("ModelRoot");model.transform.SetParent(node.transform,false);
                    var rig=new NativeCharacterSkin(model.transform,id,Materials());rig.Body();rig.Clothing();
                    rig.Bake(Root+"/Meshes/"+id+".asset",id+"_SkinnedBodyAndClothing");
                    var avatar=AvatarBuilder.BuildGenericAvatar(model,"Root");avatar.name=id+"_Generic";
                    if(!avatar.isValid)throw new InvalidOperationException("Avatar Generic inválido: "+id);
                    AssetDatabase.CreateAsset(avatar,Root+"/Rigs/"+id+".asset");
                    var animator=model.AddComponent<Animator>();animator.avatar=avatar;animator.applyRootMotion=false;animator.fireEvents=false;animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;
                    var clips=new AnimationClip[7];
                    foreach(CharacterMotion motion in Enum.GetValues(typeof(CharacterMotion)))
                    {
                        var index=(int)motion;var shared=motion==CharacterMotion.Walk||motion==CharacterMotion.Run||motion==CharacterMotion.Hit||motion==CharacterMotion.Defeat;
                        clips[index]=shared&&common!=null?common[index]:NativeCharacterClips.Create(rig,id,motion,Root+"/Animation/"+(shared?"Shared":id)+"_"+motion+".anim");
                    }
                    if(graph==null)
                    {
                        var path=Root+"/Animation/CharacterShared.controller";AssetDatabase.CopyAsset(HumanoidPipelineBuilder.ControllerPath,path);graph=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
                        graph.AddParameter("GaitRate",AnimatorControllerParameterType.Float);common=clips;
                        foreach(var child in graph.layers[0].stateMachine.states)
                        {
                            child.state.motion=clips[(int)Enum.Parse(typeof(CharacterMotion),child.state.name)];
                            if(child.state.name=="Walk"||child.state.name=="Run"){child.state.speedParameter="GaitRate";child.state.speedParameterActive=true;}
                        }
                        foreach(var tr in graph.layers[0].stateMachine.anyStateTransitions)
                            if(tr.destinationState.name!="Defeat")tr.AddCondition(AnimatorConditionMode.IfNot,0,"Defeated");
                        EditorUtility.SetDirty(graph);
                    }
                    RuntimeAnimatorController playback=graph;
                    if(id!="monk")
                    {
                        var overrides=new AnimatorOverrideController(graph){name=id+"_Overrides"};
                        foreach(var motion in new[]{CharacterMotion.Idle,CharacterMotion.Attack,CharacterMotion.Defend})overrides[common[(int)motion].name]=clips[(int)motion];
                        AssetDatabase.CreateAsset(overrides,Root+"/Animation/"+id+".overrideController");playback=overrides;
                    }
                    var profile=ScriptableObject.CreateInstance<HumanoidAnimationProfile>();profile.Controller=playback;profile.AllowGenericClips=true;profile.UseGaitRate=true;profile.RunSpeed=2.8f;
                    profile.Idle=clips[0];profile.Walk=clips[1];profile.Run=clips[2];profile.Attack=clips[3];profile.Defend=clips[4];profile.Hit=clips[5];profile.Defeat=clips[6];
                    AssetDatabase.CreateAsset(profile,Root+"/Animation/"+id+".asset");
                    var view=node.AddComponent<CharacterVisualModel>();
                    var socket=new GameObject("WeaponSocket").transform;socket.SetParent(rig.Get("RightHand"),false);socket.localPosition=new Vector3(.045f,0,.005f);socket.localRotation=Quaternion.Euler(0,0,-75);
                    var sword=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/TrainingSword_96cm.prefab"),socket);sword.transform.localPosition=new Vector3(0,-.09f,0);sword.SetActive(false);view.ConfigureAnchor(socket);
                    // Pechera blanda ligada al torso, visible únicamente al equipar el item existente.
                    var overlay=new GameObject("EquippedCloth");overlay.transform.SetParent(rig.Get("Chest"),false);overlay.transform.localPosition=new Vector3(0,-.14f,.157f);
                    var cloth=GameObject.CreatePrimitive(PrimitiveType.Cube);Object.DestroyImmediate(cloth.GetComponent<Collider>());cloth.name="ClothPatch";cloth.transform.SetParent(overlay.transform,false);cloth.transform.localScale=new Vector3(.21f,.16f,.008f);cloth.GetComponent<Renderer>().sharedMaterial=Materials()[4];overlay.SetActive(false);
                    view.Configure(sword,overlay);
                    if(id=="mage")CreateStaff(rig);
                    var visual=node.AddComponent<HumanoidVisualRig>();visual.Configure(animator,true);node.AddComponent<HumanoidAnimationDriver>().Configure(visual,profile);
                    if(!visual.IsUsable)throw new InvalidOperationException("Rig sin skin: "+id);
                    var entry=catalog.FindEntry(id);entry.HumanoidPrefab=PrefabUtility.SaveAsPrefabAsset(node,Prefabs+"/"+id+"_Animated.prefab");entry.ReferenceHeightMeters=1.80f;
                    entry.EquipmentPolicy="E08: esqueleto Generic compartido; espada según inventario, bastón Mago accesorio de identidad sin estadísticas.";
                    Debug.Log("E08_RIG "+id+" bones="+rig.Bones.Count+" avatarValid="+avatar.isValid+" human="+avatar.isHuman);
                }
                finally{Object.DestroyImmediate(node);}
            }
            EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssets();Integrate();
        }
        private static Material[] Materials()
        {
            Material Old(string n)=>AssetDatabase.LoadAssetAtPath<Material>(CharacterVisualBuilder.Root+"/Materials/"+n+".mat");
            Material At(string p)=>AssetDatabase.LoadAssetAtPath<Material>(p);
            return new[]{Old("Skin"),At(MonkIdentityBuilder.Root+"/Materials/BurntOrange.mat"),Old("Leather"),At(MonkIdentityBuilder.Root+"/Materials/DarkTrousers.mat"),Old("MonkCloth"),At(ProductionCharacterBuilder.Root+"/Materials/Plum.mat"),At(ProductionCharacterBuilder.Root+"/Materials/Steel.mat"),At(ProductionCharacterBuilder.Root+"/Materials/BlueGray.mat"),Old("Hair")};
        }
        private static void CreateStaff(NativeCharacterSkin rig)
        {
            var staff=new GameObject("Staff_Identity_NoItem").transform;staff.SetParent(rig.Get("LeftHand"),false);staff.localRotation=Quaternion.Euler(0,0,-90);staff.localPosition=new Vector3(-.04f,0,0);
            var pole=GameObject.CreatePrimitive(PrimitiveType.Cylinder);pole.name="AshStaff";Object.DestroyImmediate(pole.GetComponent<Collider>());pole.transform.SetParent(staff,false);pole.transform.localPosition=new Vector3(0,.27f,0);pole.transform.localScale=new Vector3(.035f,.78f,.035f);pole.GetComponent<Renderer>().sharedMaterial=Materials()[2];
            var cap=GameObject.CreatePrimitive(PrimitiveType.Sphere);cap.name="ArcaneFocus";Object.DestroyImmediate(cap.GetComponent<Collider>());cap.transform.SetParent(staff,false);cap.transform.localPosition=new Vector3(0,1.05f,0);cap.transform.localScale=new Vector3(.07f,.11f,.07f);cap.GetComponent<Renderer>().sharedMaterial=Materials()[6];
        }
        public static void Integrate()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var physics=EnvironmentPhysicsSnapshot.Capture(scene);var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CatalogPath);
            var director=Object.FindAnyObjectByType<PrototypeDirector>();
            foreach(var presenter in Object.FindObjectsByType<CharacterVisualPresenter>(FindObjectsInactive.Include))SetCatalog(presenter,"catalog",catalog);
            foreach(var hud in Object.FindObjectsByType<MedievalHudView>(FindObjectsInactive.Include)){hud.Catalog=catalog;EditorUtility.SetDirty(hud);}
            foreach(var card in Object.FindObjectsByType<ClassReferenceCard>(FindObjectsInactive.Include))SetCatalog(card,"catalog",catalog);
            var action=director.Hero.GetComponent<CharacterCombatAnimationPresenter>();if(action==null)action=director.Hero.gameObject.AddComponent<CharacterCombatAnimationPresenter>();action.Configure(Object.FindAnyObjectByType<TurnEncounter>(),director.Hero.GetComponent<CharacterVisualPresenter>());
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics)throw new InvalidOperationException("E08 cambió física del escenario.");
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("E08_INTEGRATED: solo presentación en aldea, escena original y proxies intactos.");
        }
        private static void SetCatalog(Object target,string field,Object value){var so=new SerializedObject(target);so.FindProperty(field).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
        public static void RefineMeshes()
        {
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                var node=new GameObject("E08 mesh refinement");var stage=Root+"/Meshes/"+id+"_Staging.asset";
                try
                {
                    var source=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Meshes/"+id+".asset");if(source==null)throw new InvalidOperationException("Malla E08 ausente.");
                    var rig=new NativeCharacterSkin(node.transform,id,Materials());rig.Body();rig.Clothing();var skin=rig.Bake(stage,source.name);
                    // CopySerialized no invalida todos los buffers nativos de Mesh en este editor.
                    // Las APIs de Mesh actualizan CPU/GPU y conservan el asset/GUID de destino.
                    var generated=skin.sharedMesh;source.Clear();source.indexFormat=generated.indexFormat;
                    source.vertices=generated.vertices;source.normals=generated.normals;source.uv=generated.uv;
                    source.boneWeights=generated.boneWeights;source.bindposes=generated.bindposes;source.subMeshCount=generated.subMeshCount;
                    for(var i=0;i<generated.subMeshCount;i++)source.SetTriangles(generated.GetTriangles(i),i);
                    source.RecalculateBounds();source.name=id;EditorUtility.SetDirty(source);
                    var tempClip=Root+"/Animation/"+id+"_Idle_Staging.anim";
                    try
                    {
                        var generatedIdle=NativeCharacterClips.Create(rig,id,CharacterMotion.Idle,tempClip);
                        var idle=AssetDatabase.LoadAssetAtPath<HumanoidAnimationProfile>(Root+"/Animation/"+id+".asset").Idle;idle.ClearCurves();
                        foreach(var binding in AnimationUtility.GetCurveBindings(generatedIdle))AnimationUtility.SetEditorCurve(idle,binding,AnimationUtility.GetEditorCurve(generatedIdle,binding));
                        AnimationUtility.SetAnimationClipSettings(idle,AnimationUtility.GetAnimationClipSettings(generatedIdle));idle.EnsureQuaternionContinuity();EditorUtility.SetDirty(idle);AssetDatabase.SaveAssets();
                    }
                    finally{AssetDatabase.DeleteAsset(tempClip);}
                }
                finally{Object.DestroyImmediate(node);AssetDatabase.DeleteAsset(stage);}
            }
            FinalizeAssets();Integrate();
            Debug.Log("E08_REFINED: materiales de piel, ajuste de ropa/vendas y continuidad de pesos; GUID originales conservados.");
        }
        public static void FinalizeAssets()
        {
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Meshes/"+id+".asset");mesh.name=id;EditorUtility.SetDirty(mesh);
                var path=Prefabs+"/"+id+"_Animated.prefab";var node=PrefabUtility.LoadPrefabContents(path);
                try
                {
                    var socket=node.GetComponent<CharacterVisualModel>().WeaponAnchor;socket.localRotation=Quaternion.Euler(0,0,-75);socket.GetChild(0).localPosition=new Vector3(0,-.09f,0);
                    var staff=node.GetComponentsInChildren<Transform>(true).FirstOrDefault(t=>t.name=="Staff_Identity_NoItem");if(staff!=null)staff.localRotation=Quaternion.Euler(0,0,-90);
                    PrefabUtility.SaveAsPrefabAsset(node,path);
                }
                finally{PrefabUtility.UnloadPrefabContents(node);}
            }
            AssetDatabase.SaveAssets();Debug.Log("E08_FINISH: anclajes de manos ajustados, nombres de malla normalizados, GUID conservados.");
        }
    }
}
