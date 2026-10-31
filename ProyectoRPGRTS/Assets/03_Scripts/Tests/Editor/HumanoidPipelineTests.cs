using System;
using System.Collections;
using System.IO;
using System.Linq;
using Aethernia;
using Aethernia.Editor.Characters;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class HumanoidPipelineTests
    {
        [SetUp] public void Open() { if(!EditorApplication.isPlaying) EditorSceneManager.OpenScene(MedievalQuarterBuilder.ScenePath); }
        [Test] public void EmptyPilotHasSevenUnanimatedStatesAndPreservesFallback()
        {
            var template=AssetDatabase.LoadAssetAtPath<GameObject>(HumanoidPipelineBuilder.TemplatePath);
            var profile=AssetDatabase.LoadAssetAtPath<HumanoidAnimationProfile>(HumanoidPipelineBuilder.ProfilePath);
            Assert.That(template,Is.Not.Null); Assert.That(template.GetComponent<HumanoidVisualRig>().IsUsable,Is.False);
            Assert.That(template.GetComponentsInChildren<Renderer>(true).Length,Is.Zero,"Do not invent a finished Monk.");
            Assert.That(template.GetComponentsInChildren<Collider>(true).Length,Is.Zero);
            Assert.That(template.GetComponentInChildren<Animator>(true).enabled,Is.False);
            Assert.That(template.GetComponentInChildren<Animator>(true).applyRootMotion,Is.False);
            var controller=(AnimatorController)profile.Controller;
            var states=controller.layers[0].stateMachine.states;
            Assert.That(states.Select(s=>s.state.name),Is.EquivalentTo(Enum.GetNames(typeof(CharacterMotion))));
            foreach(var state in states) Assert.That(state.state.motion,Is.Null,"States are prepared; clips remain absent.");
            foreach(CharacterMotion motion in Enum.GetValues(typeof(CharacterMotion))) Assert.That(profile.HasClip(motion),Is.False);
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(CharacterVisualBuilder.CatalogPath);
            Assert.That(catalog.Find("monk"),Is.SameAs(catalog.Entries.First(e=>e.ClassId=="monk").Prefab));
            var invalid=ScriptableObject.CreateInstance<HumanoidAnimationProfile>(); var empty=new AnimationClip();
            try
            {
                invalid.Controller=controller; invalid.Idle=empty;
                Assert.Throws<InvalidOperationException>(()=>HumanoidPipelineBuilder.ApplyAnimationProfile(invalid));
                foreach(var state in states) Assert.That(state.state.motion,Is.Null);
            }
            finally { Object.DestroyImmediate(empty); Object.DestroyImmediate(invalid); }
            File.WriteAllText("Logs/E02-structure.txt","PASS: empty candidate, seven null motions, no fabricated clips, invalid clip rejected, Monk E01 fallback retained.");
        }
        [Test] public void NativeFbxImportsMetresAndRejectsNonHumanoidCandidate()
        {
            var path=AssetDatabase.GenerateUniqueAssetPath(HumanoidPipelineBuilder.Root+"/Models/E02_MeterCube_TEST_ONLY.fbx");
            GameObject instance=null;
            string receipt=null;
            try
            {
                File.WriteAllText(path,MeterCube()); AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
                var importer=(ModelImporter)AssetImporter.GetAtPath(path); Assert.That(importer,Is.Not.Null);
                HumanoidImportPipeline.ConfigureFbx(importer);
                Assert.That(importer.animationType,Is.EqualTo(ModelImporterAnimationType.Human));
                Assert.That(importer.avatarSetup,Is.EqualTo(ModelImporterAvatarSetup.CreateFromThisModel));
                Assert.That(importer.globalScale,Is.EqualTo(1)); Assert.That(importer.useFileScale,Is.True);
                Assert.That(importer.addCollider||importer.importLights||importer.importCameras||importer.optimizeGameObjects,Is.False);
                // This fixture has no skeleton. Test native import as Generic, never claim a real Human rig.
                HumanoidImportPipeline.ConfigureFbx(importer,ModelImporterAnimationType.Generic); importer.SaveAndReimport();
                var model=AssetDatabase.LoadAssetAtPath<GameObject>(path); Assert.That(model,Is.Not.Null);
                instance=(GameObject)PrefabUtility.InstantiatePrefab(model);
                var renderer=instance.GetComponentInChildren<Renderer>(); Assert.That(renderer,Is.Not.Null);
                Assert.That(renderer.bounds.size.x,Is.EqualTo(1).Within(.005f));
                Assert.That(renderer.bounds.size.y,Is.EqualTo(1).Within(.005f));
                Assert.That(renderer.bounds.size.z,Is.EqualTo(1).Within(.005f));
                Assert.That(HumanoidImportPipeline.Audit(instance).Any(x=>x.Contains("Humanoid")),Is.True);
                var candidate=AssetDatabase.GenerateUniqueAssetPath(HumanoidPipelineBuilder.Prefabs+"/E02_Invalid_TEST_ONLY.prefab");
                Assert.Throws<InvalidOperationException>(()=>HumanoidImportPipeline.CreateCandidate(model,null,null,candidate));
                Assert.That(AssetDatabase.LoadMainAssetAtPath(candidate),Is.Null);
                receipt="PASS: native FBX ModelImporter; metre cube bounds="+renderer.bounds.size.ToString("F3")+"; Humanoid settings checked without pretending this cube has a Human rig; invalid candidate rejected; fixture removed.";
            }
            finally { if(instance!=null) Object.DestroyImmediate(instance); AssetDatabase.DeleteAsset(path); }
            LogAssert.NoUnexpectedReceived();
            Assert.That(File.Exists(path),Is.False); File.WriteAllText("Logs/E02-fbx-import.txt",receipt);
        }
        [Test] public void FiveVisualSlotsReadExistingEquipmentAndRoundTripSave()
        {
            var root=new GameObject("E02 bindings test"); var outside=new GameObject("External object must not be hidden");
            var rules=ScriptableObject.CreateInstance<PrototypeRules>();
            try
            {
                var model=root.AddComponent<CharacterVisualModel>(); var bindings=new EquipmentVisualBinding[6];
                for(var i=0;i<5;i++)
                {
                    var view=new GameObject(((VisualEquipmentSlot)i).ToString()); view.transform.SetParent(root.transform,false);
                    var weapon=i==2||i==3;
                    bindings[i]=new EquipmentVisualBinding {Slot=(VisualEquipmentSlot)i,Source=weapon?EquipmentStateSource.Weapon:EquipmentStateSource.Armour,ItemId=i==3?"fixture-staff":weapon?"training-sword":"cloth",Visual=view};
                }
                bindings[5]=new EquipmentVisualBinding {ItemId="",Visual=outside}; model.ConfigureBindings(bindings);
                var state=new PrototypeState(); WorldOperations.AddItem(state,"training-sword",1); WorldOperations.AddItem(state,"cloth",1);
                Assert.That(WorldOperations.Equip(state,"training-sword")&&WorldOperations.Equip(state,"cloth"),Is.True);
                var before=JsonUtility.ToJson(state); model.ApplyEquipment(state); Assert.That(JsonUtility.ToJson(state),Is.EqualTo(before));
                Assert.That(model.SwordVisible&&model.ArmourVisible,Is.True); Assert.That(bindings[3].Visual.activeSelf,Is.False);
                Assert.That(bindings[0].Visual.activeSelf&&bindings[1].Visual.activeSelf&&bindings[4].Visual.activeSelf,Is.True);
                Assert.That(outside.activeSelf,Is.True);
                var save="Logs/E02-equipment-save.json"; Assert.That(PrototypeSaveStore.Save(save,state,rules),Is.True);
                Assert.That(PrototypeSaveStore.Load(save,rules,out var restored),Is.True); model.ApplyEquipment(restored);
                Assert.That(model.SwordVisible&&model.ArmourVisible,Is.True); Assert.That(restored.Version,Is.EqualTo(1));
                state.Weapon="fixture-staff"; model.ApplyEquipment(state);
                Assert.That(bindings[3].Visual.activeSelf,Is.True); Assert.That(bindings[2].Visual.activeSelf,Is.False);
                Assert.That(WorldOperations.KnownItem("fixture-staff"),Is.False,"A presentation fixture must not add an inventory item.");
            }
            finally { Object.DestroyImmediate(root); Object.DestroyImmediate(outside); Object.DestroyImmediate(rules); }
        }
        [UnityTest] public IEnumerator MissingAssetsLeaveMovementAndMonkFallbackFunctional()
        {
            yield return new EnterPlayMode(); yield return null; yield return null;
            var director=Object.FindAnyObjectByType<PrototypeDirector>(); director.Session.SelectClass("monk");
            var presenter=director.Hero.GetComponent<CharacterVisualPresenter>();
            Assert.That(presenter.DisplayedClass,Is.EqualTo("monk")); Assert.That(presenter.Model.GetComponent<HumanoidVisualRig>(),Is.Null);
            var template=AssetDatabase.LoadAssetAtPath<GameObject>(HumanoidPipelineBuilder.TemplatePath);
            var node=Object.Instantiate(template); var driver=node.GetComponent<HumanoidAnimationDriver>(); driver.Bind(director.Hero);
            try
            {
                Assert.That(director.Hero.TryMoveTo(new Vector3(-4,0,-6)),Is.True);
                var deadline=Time.realtimeSinceStartup+4;
                while(director.Hero.Agent.velocity.sqrMagnitude<.001f&&Time.realtimeSinceStartup<deadline) yield return null;
                yield return null; yield return null;
                Assert.That(driver.ObservedSpeed,Is.GreaterThan(0)); Assert.That(driver.CanAnimate,Is.False);
                foreach(CharacterMotion motion in Enum.GetValues(typeof(CharacterMotion))) Assert.That(driver.TryPlay(motion),Is.False);
                Assert.That(node.GetComponentInChildren<Animator>().enabled,Is.False);
                deadline=Time.realtimeSinceStartup+10;
                while(director.Hero.IsMoving&&Time.realtimeSinceStartup<deadline) yield return null;
                Assert.That(Vector3.Distance(director.Hero.transform.position,new Vector3(-4,0,-6)),Is.LessThan(.3f));
                Assert.That(director.Hero.Agent.radius,Is.EqualTo(.3f)); Assert.That(director.Hero.IsSelected,Is.False);
            }
            finally { Object.Destroy(node); }
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }
        [UnityTearDown] public IEnumerator Cleanup() { if(EditorApplication.isPlaying) yield return new ExitPlayMode(); }
        private static string MeterCube() => @"; FBX 7.4.0 project file
; Project-owned UNIT TEST fixture, not a character
FBXHeaderExtension: { FBXHeaderVersion: 1003
 FBXVersion: 7400
}
GlobalSettings: {
 Version: 1000
 Properties70: {
  P: ""UpAxis"", ""int"", ""Integer"", """",1
  P: ""UpAxisSign"", ""int"", ""Integer"", """",1
  P: ""FrontAxis"", ""int"", ""Integer"", """",2
  P: ""FrontAxisSign"", ""int"", ""Integer"", """",-1
  P: ""CoordAxis"", ""int"", ""Integer"", """",0
  P: ""CoordAxisSign"", ""int"", ""Integer"", """",1
  P: ""UnitScaleFactor"", ""double"", ""Number"", """",100
  P: ""OriginalUnitScaleFactor"", ""double"", ""Number"", """",100
 }
}
Documents: {
 Count: 1
 Document: 300, """", ""Scene"" {
  Properties70: { P: ""SourceObject"", ""object"", """", """" }
  RootNode: 0
 }
}
References: { }
Definitions: { Version: 100
 Count: 2
 ObjectType: ""Geometry"" { Count: 1 }
 ObjectType: ""Model"" { Count: 1 }
}
Objects: {
 Geometry: 100, ""Geometry::MeterCube"", ""Mesh"" {
  Vertices: *24 { a: 0,0,0,1,0,0,1,1,0,0,1,0,0,0,1,1,0,1,1,1,1,0,1,1 }
  PolygonVertexIndex: *24 { a: 0,3,2,-2,4,5,6,-8,0,4,7,-4,1,2,6,-6,3,7,6,-3,0,1,5,-5 }
  LayerElementNormal: 0 {
   Version: 101
   Name: """"
   MappingInformationType: ""ByPolygonVertex""
   ReferenceInformationType: ""Direct""
   Normals: *72 { a: 0,0,-1,0,0,-1,0,0,-1,0,0,-1,0,0,1,0,0,1,0,0,1,0,0,1,-1,0,0,-1,0,0,-1,0,0,-1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,1,0,0,1,0,0,-1,0,0,-1,0,0,-1,0,0,-1,0 }
  }
  LayerElementUV: 0 {
   Version: 101
   Name: ""UVMap""
   MappingInformationType: ""ByPolygonVertex""
   ReferenceInformationType: ""IndexToDirect""
   UV: *8 { a: 0,0,1,0,1,1,0,1 }
   UVIndex: *24 { a: 0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3 }
  }
  Layer: 0 {
   Version: 100
   LayerElement: { Type: ""LayerElementNormal""
    TypedIndex: 0
   }
   LayerElement: { Type: ""LayerElementUV""
    TypedIndex: 0
   }
  }
 }
 Model: 200, ""Model::MeterCube"", ""Mesh"" {
  Version: 232
  Properties70: {
   P: ""Lcl Translation"", ""Lcl Translation"", """", ""A"",0,0,0
   P: ""Lcl Rotation"", ""Lcl Rotation"", """", ""A"",0,0,0
   P: ""Lcl Scaling"", ""Lcl Scaling"", """", ""A"",1,1,1
  }
  Shading: T
  Culling: ""CullingOff""
 }
}
Connections: {
 C: ""OO"",100,200
 C: ""OO"",200,0
}
".Replace("{", "{\n").Replace("}", "\n}");
    }
}
