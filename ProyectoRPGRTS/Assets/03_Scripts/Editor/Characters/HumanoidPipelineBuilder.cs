using System;
using System.Linq;
using Aethernia.Editor.Environment;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class HumanoidPipelineBuilder
    {
        public const string Root="Assets/07_Characters/Humanoid";
        public const string Prefabs="Assets/02_Prefabs/Characters/Humanoid";
        public const string ProfilePath=Root+"/Animation/MonkAnimations.asset";
        public const string ControllerPath=Root+"/Animation/HumanoidBase.controller";
        public const string TemplatePath=Prefabs+"/Monk_ImportTemplate.prefab";
        public static void Create()
        {
            if(AssetDatabase.IsValidFolder(Root)) throw new InvalidOperationException("E02 already exists; do not generate again.");
            foreach(var folder in new[]{Root+"/Animation",Root+"/Models",Root+"/Textures",Root+"/Equipment",Prefabs}) EnvironmentMaterials.Folder(folder);
            var controller=AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("Speed",AnimatorControllerParameterType.Float);
            controller.AddParameter("Locomotion",AnimatorControllerParameterType.Int);
            controller.AddParameter("Defending",AnimatorControllerParameterType.Bool);
            controller.AddParameter("Defeated",AnimatorControllerParameterType.Bool);
            controller.AddParameter("Attack",AnimatorControllerParameterType.Trigger); controller.AddParameter("Hit",AnimatorControllerParameterType.Trigger);
            var machine=controller.layers[0].stateMachine;
            var states=Enum.GetNames(typeof(CharacterMotion)).Select((name,index)=>machine.AddState(name,new Vector3(260+(index%3)*230,80+(index/3)*150))).ToArray();
            machine.defaultState=states[0];
            for(var i=0;i<3;i++) for(var j=0;j<3;j++) if(i!=j)
            { var transition=states[i].AddTransition(states[j]); transition.hasExitTime=false; transition.duration=.12f; transition.AddCondition(AnimatorConditionMode.Equals,j,"Locomotion"); }
            foreach(var index in new[]{3,5})
            {
                var enter=machine.AddAnyStateTransition(states[index]); enter.hasExitTime=false; enter.duration=.08f; enter.canTransitionToSelf=false;
                enter.AddCondition(AnimatorConditionMode.If,0,states[index].name);
                var exit=states[index].AddTransition(states[0]); exit.hasExitTime=true; exit.exitTime=.95f; exit.duration=.1f;
            }
            foreach(var index in new[]{4,6})
            {
                var parameter=index==4?"Defending":"Defeated";
                var enter=machine.AddAnyStateTransition(states[index]); enter.hasExitTime=false; enter.duration=.1f; enter.canTransitionToSelf=false; enter.AddCondition(AnimatorConditionMode.If,0,parameter);
                var exit=states[index].AddTransition(states[0]); exit.hasExitTime=false; exit.duration=.1f; exit.AddCondition(AnimatorConditionMode.IfNot,0,parameter);
            }
            var profile=ScriptableObject.CreateInstance<HumanoidAnimationProfile>(); profile.Controller=controller; AssetDatabase.CreateAsset(profile,ProfilePath);
            var root=new GameObject("Monk_ImportTemplate_EMPTY");
            GameObject template;
            try
            {
                var body=new GameObject("ModelRoot - FBX pending"); body.transform.SetParent(root.transform,false);
                var animator=body.AddComponent<Animator>(); animator.enabled=false; animator.applyRootMotion=false; animator.fireEvents=false; animator.runtimeAnimatorController=controller;
                var rig=root.AddComponent<HumanoidVisualRig>(); rig.Configure(animator);
                root.AddComponent<HumanoidAnimationDriver>().Configure(rig,profile);
                root.AddComponent<CharacterVisualModel>().ConfigureBindings(CreateSockets(root.transform,null));
                template=PrefabUtility.SaveAsPrefabAsset(root,TemplatePath);
            }
            finally { Object.DestroyImmediate(root); }
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            catalog.Entries.First(e=>e.ClassId=="monk").HumanoidPrefab=template; EditorUtility.SetDirty(catalog); AssetDatabase.SaveAssets();
            Debug.Log("E02_PREPARED: empty Monk template, seven states without clips; existing Monk fallback remains active. No real humanoid model/animation verified.");
        }
        public static EquipmentVisualBinding[] CreateSockets(Transform root,Animator animator)
        {
            var bindings=new EquipmentVisualBinding[5];
            for(var i=0;i<5;i++)
            {
                var slot=(VisualEquipmentSlot)i;
                var parent=animator!=null && (slot==VisualEquipmentSlot.Weapon||slot==VisualEquipmentSlot.Staff) ? animator.GetBoneTransform(HumanBodyBones.RightHand) : root;
                var socket=new GameObject(slot+"Socket - assign compatible view").transform; socket.SetParent(parent!=null?parent:root,false);
                bindings[i]=new EquipmentVisualBinding {Slot=slot,Source=(slot==VisualEquipmentSlot.Weapon||slot==VisualEquipmentSlot.Staff)?EquipmentStateSource.Weapon:EquipmentStateSource.Armour,ItemId="",Visual=null};
            }
            return bindings;
        }
        public static void ApplyAnimationProfile(HumanoidAnimationProfile profile)
        {
            if(profile==null || !(profile.Controller is AnimatorController controller)) throw new InvalidOperationException("Se requiere un AnimatorController editable y perfil.");
            // Validate all inputs before mutating any state.
            foreach(CharacterMotion motion in Enum.GetValues(typeof(CharacterMotion)))
                if(profile.Clip(motion)!=null && !profile.HasClip(motion)) throw new InvalidOperationException("Clip incompatible/vacío: "+motion);
            var names=controller.layers[0].stateMachine.states.Select(s=>s.state.name).ToArray();
            if(!Enum.GetNames(typeof(CharacterMotion)).All(names.Contains)) throw new InvalidOperationException("El controller no contiene los siete estados del contrato.");
            if(profile.Idle!=null && AssetDatabase.GetAssetPath(controller)==ControllerPath)
            {
                // A profile gets its own motions; the shared empty graph stays reusable.
                var stem=System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(profile));
                var copy=AssetDatabase.GenerateUniqueAssetPath(Root+"/Animation/"+stem+"_Playback.controller");
                if(!AssetDatabase.CopyAsset(ControllerPath,copy)) throw new InvalidOperationException("No se pudo copiar el grafo de reproducción.");
                controller=AssetDatabase.LoadAssetAtPath<AnimatorController>(copy); profile.Controller=controller; EditorUtility.SetDirty(profile);
            }
            foreach(var child in controller.layers[0].stateMachine.states)
                if(Enum.TryParse(child.state.name,out CharacterMotion motion)) { child.state.motion=profile.Clip(motion); EditorUtility.SetDirty(child.state); }
            EditorUtility.SetDirty(controller); AssetDatabase.SaveAssets();
        }
    }
}
