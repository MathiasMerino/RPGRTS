using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class WorldExplorationTests
    {
        private DemoFlow flow;
        private void Open() { EditorSceneManager.OpenScene(WorldExplorationBuilder.ScenePath); }
        private void Begin(bool legacy=false)
        {
            flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/UX02/test-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;
            if(legacy)Assert.That(flow.Continue("../Docs/Validacion/E19/PartidasFinales/human-warrior.json"),Is.True);
            else{Assert.That(flow.StartNew("Explorador","warrior","human"),Is.True);flow.CompleteIntro();}
        }
        [Test]public void SceneReusesSystemsAndPublicAssetsWithOwnNavigation()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.ScenePath);var f=Object.FindAnyObjectByType<DemoFlow>();var w=f.GetComponent<WorldExploration>();
            Assert.That(w.Interactions.Length,Is.EqualTo(8));Assert.That(w.RoutePoints.Length,Is.GreaterThanOrEqualTo(8));Assert.That(f.Session.Rules.WorldExploration,Is.True);
            var old=AssetDatabase.LoadAssetAtPath<PrototypeRules>(UxExperienceBuilder.Root+"/PrototypeRules.asset");Assert.That(old.WorldExploration,Is.False);Assert.That(f.Session.Rules.Civilizations,Is.SameAs(old.Civilizations));
            Assert.That(AssetDatabase.GetDependencies(WorldExplorationBuilder.ScenePath,true).Any(p=>p.Contains("LocalOnly")||p.Contains("Meshy")),Is.False);
            Assert.That(Object.FindObjectsByType<IsometricCameraRig>().Length,Is.EqualTo(1));Assert.That(Object.FindObjectsByType<VillageNpcActivity>().Length,Is.EqualTo(3));
            Assert.That(Object.FindObjectsByType<VillageNpcActivity>().All(n=>!n.Actor.Agent.enabled),Is.True,"scene agents enable in Start after surface Awake");
        }
        [UnityTest]public IEnumerator ForestLoopDiscoveriesRewardsAndModalControls()
        {
            Open();yield return new EnterPlayMode();yield return null;yield return null;Begin();yield return WorldExplorationRoute.Explore(flow,Capture);
            var before=flow.Session.State.Facts.ToArray();Assert.That(flow.Director.Save(flow.SlotPath),Is.True);Assert.That(flow.ReturnToTitle()&&flow.Continue(),Is.True);flow.Pause();Assert.That(flow.Session.State.Facts,Is.EquivalentTo(before));flow.Resume();LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]public IEnumerator InteractionCannotPassThroughWallAndFocusIsStable()
        {
            Open();yield return new EnterPlayMode();yield return null;yield return null;Begin();var point=flow.GetComponent<WorldExploration>().Interactions.Single(i=>i.Kind==WorldInteractionKind.Chest).GetComponent<InteractionPoint>();
            yield return WorldExplorationRoute.MoveTo(flow,point.transform.position+Vector3.back*1.1f);var commands=Object.FindAnyObjectByType<PlayerCommands>();Assert.That(commands.RefreshNearby(),Is.SameAs(point));
            for(var i=0;i<5;i++)Assert.That(commands.RefreshNearby(),Is.SameAs(point));
            var wall=GameObject.CreatePrimitive(PrimitiveType.Cube);wall.name="Explicit line-of-sight fixture wall";wall.transform.position=(point.transform.position+flow.Director.Hero.transform.position)*.5f+Vector3.up;wall.transform.localScale=new Vector3(2,2,.1f);Physics.SyncTransforms();
            try{Assert.That(commands.HasInteractionSight(point),Is.False);Assert.That(commands.RefreshNearby(),Is.Not.SameAs(point));Assert.That(point.TryInteract(flow.Director.Hero,flow.Session),Is.False);}
            finally{Object.Destroy(wall);}yield return null;Physics.SyncTransforms();Assert.That(commands.HasInteractionSight(point),Is.True);LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]public IEnumerator LegacyDungeonEnterReturnAndSaveRetainSharedWorld()
        {
            Open();yield return new EnterPlayMode();yield return null;yield return null;Begin(true);var before=flow.Session.State;var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();
            yield return WorldExplorationRoute.Use(flow,"umbral-entry",Capture,"Legacy-Ruins");Assert.That(dungeon.Inside,Is.True);yield return Capture("Legacy-Inside");
            var insideSlot=flow.SlotPath+".inside.json";Assert.That(flow.Director.Save(insideSlot),Is.True);
            yield return WorldExplorationRoute.Use(flow,"umbral-exit",Capture,"Legacy-Exit");Assert.That(dungeon.Inside,Is.False);Assert.That(flow.Session.State,Is.SameAs(before));
            Assert.That(flow.Director.Load(insideSlot),Is.True,"direct F9 load inside dungeon");Assert.That(dungeon.Inside,Is.True);Assert.That(flow.CameraRig.Focus.x,Is.EqualTo(80),"correct dungeon region on direct load");
            yield return WorldExplorationRoute.Use(flow,"umbral-exit",Capture,"Legacy-ReloadExit");
            Assert.That(flow.Director.Save(flow.SlotPath),Is.True);Assert.That(flow.ReturnToTitle()&&flow.Continue(),Is.True);flow.Pause();Assert.That(flow.Session.State.Army.Select(r=>r.Id),Is.EquivalentTo(before.Army.Select(r=>r.Id)));Assert.That(flow.Session.State.Companions.Select(r=>r.Id),Is.EquivalentTo(before.Companions.Select(r=>r.Id)));flow.Resume();LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]public IEnumerator SafeFormationRequiresSelectionAndExplicitValidAttack()
        {
            Open();yield return new EnterPlayMode();yield return null;yield return null;Begin(true);var rts=flow.Director.Rts;Assert.That(flow.BeginPatrol(),Is.True);var units=new List<RtsUnitStatus>();rts.CopyStatus(units);
            var health=flow.Session.State.Health;rts.SelectGroup(new List<ExplorationUnit>(),false);Assert.That(rts.Command(rts.PreparationTarget,null,false),Is.False);Assert.That(rts.Preparing,Is.True);
            var ally=units.First(u=>!u.Hero).Unit;Assert.That(rts.SelectUnit(ally.GetComponentInChildren<Collider>(),false),Is.True);yield return UxExperienceRoute.Wait(.3f);rts.SelectAll();
            Assert.That(rts.Command(new Vector3(float.NaN,0,0),null,false),Is.False);Assert.That(rts.Command(new Vector3(300,0,300),null,false),Is.False);
            for(var step=1;step<3;step++)
            {Assert.That(rts.Command(rts.PreparationTarget,null,false),Is.True);var end=Time.realtimeSinceStartup+25;while(rts.PreparationStep<=step&&Time.realtimeSinceStartup<end)yield return null;Assert.That(rts.PreparationStep,Is.GreaterThan(step));}
            Assert.That(flow.Session.State.Health,Is.EqualTo(health));rts.CopyStatus(units);var enemy=units.First(u=>u.Team==1&&u.Health>0).Unit;
            rts.SelectGroup(new List<ExplorationUnit>(),false);Assert.That(rts.Command(enemy.transform.position,enemy.GetComponentInChildren<Collider>(),false),Is.False);Assert.That(rts.Preparing,Is.True,"no selected unit must not activate hostility");
            rts.SelectAll();yield return Capture("Tactical-Formation");Assert.That(rts.Command(enemy.transform.position,enemy.GetComponentInChildren<Collider>(),false),Is.True);Assert.That(rts.Preparing,Is.False);Assert.That(rts.LastOrderText,Does.Contain("Atacar"));flow.Director.Rts.Clear();LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]public IEnumerator ActualWoodlandOcclusionRestoresVisibilityAndKeepsPhysicsMaterials()
        {
            Open();yield return new EnterPlayMode();yield return null;yield return null;Begin();var tree=Object.FindObjectsByType<RoofCutaway>().Single(c=>Vector3.Distance(c.transform.position,new Vector3(-30,0,5))<.1f);
            var renderers=tree.GetComponentsInChildren<Renderer>();var materials=renderers.Select(r=>r.sharedMaterial).ToArray();var enabled=tree.GetComponentsInChildren<Collider>().Select(c=>c.enabled).ToArray();
            Assert.That(flow.Director.Hero.Warp(tree.transform.position+new Vector3(3,0,3)),Is.True);yield return UxExperienceRoute.Wait(.4f);Assert.That(tree.IsCutAway,Is.True,"actual woodland canopy between camera and hero");Assert.That(renderers.Any(r=>r.forceRenderingOff),Is.True);
            yield return Capture("ActualTree-Cutaway");tree.enabled=false;Assert.That(renderers.All(r=>!r.forceRenderingOff),Is.True);Assert.That(renderers.Select(r=>r.sharedMaterial),Is.EqualTo(materials));Assert.That(tree.GetComponentsInChildren<Collider>().Select(c=>c.enabled),Is.EqualTo(enabled));tree.enabled=true;
            Assert.That(flow.Director.Hero.Warp(new Vector3(-10,0,1.4f)),Is.True);yield return null;Assert.That(tree.IsCutAway,Is.False);LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]public IEnumerator ContextualDialogueAndCameraRemainReadableAtThreeResolutions()
        {
            Open();yield return new EnterPlayMode();yield return null;yield return null;Begin();
            yield return Readability();
        }
        private IEnumerator Readability()
        {
            yield return WorldExplorationRoute.Use(flow,"ux02-merchant",Capture,"UI-Merchant");flow=Object.FindAnyObjectByType<DemoFlow>();Assert.That(flow,Is.Not.Null,"active flow after dialogue");var world=Object.FindAnyObjectByType<WorldExploration>();Assert.That(world,Is.Not.Null,"active world content");Assert.That(world.DialogueOpen,Is.True);
            foreach(var width in new[]{1280,1920,2560})
            {
                yield return DemoCapture.Save("Logs/UX02/Captures/WorldDialogue-"+width+".png",width,width*9/16,null,(camera,w,h)=>
                {
                    foreach(var label in world.ConfirmButton.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>())Assert.That(label.isTextOverflowing,Is.False,label.name+" overflows at "+w);
                    Assert.That(flow.Session.WorldUiOpen,Is.True);Assert.That(flow.GetComponent<UxExperienceView>().PromptText,Is.Not.Null);
                });
            }
            world.Close();var motor=flow.GetComponent<DemoHeroMotor>();var hero=flow.Director.Hero;var start=hero.transform.position;
            for(var i=0;i<15;i++){motor.ApplyInput(Vector2.down,.03f,false);yield return null;}
            Assert.That(Vector3.Distance(hero.transform.position,start),Is.GreaterThan(.2f));yield return UxExperienceRoute.Wait(.4f);
            var projected=flow.CameraRig.GetComponent<Camera>().WorldToViewportPoint(hero.transform.position+Vector3.up);Assert.That(projected.x,Is.InRange(.2f,.8f));Assert.That(projected.y,Is.InRange(.2f,.8f));Assert.That(flow.CameraRig.Size,Is.InRange(4.5f,6));LogAssert.NoUnexpectedReceived();
        }
        private IEnumerator Capture(string name)
        {Directory.CreateDirectory("Logs/UX02/Captures");yield return DemoCapture.Save("Logs/UX02/Captures/"+name+"-1280.png",1280,720);}
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
