using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class AnimatedCharacterTests
    {
        [TestCase("monk")][TestCase("warrior")][TestCase("mage")]
        public void AuthoredRigHasValidSkinAndArticulatedClips(string id)
        {
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AnimatedCharacterBuilder.CatalogPath);
            Assert.That(catalog,Is.Not.Null);var prefab=catalog.Find(id);Assert.That(prefab,Is.SameAs(catalog.FindEntry(id).HumanoidPrefab));
            var node=Object.Instantiate(prefab);var baked=new Mesh();var before=new Mesh();
            try
            {
                var rig=node.GetComponent<HumanoidVisualRig>();var driver=node.GetComponent<HumanoidAnimationDriver>();var skin=node.GetComponentInChildren<SkinnedMeshRenderer>();
                Assert.That(rig.IsUsable&&rig.AllowsGeneric,Is.True);Assert.That(rig.Animator.avatar.isHuman,Is.False,"Generic explícito; no certificar retargeting Humanoid.");
                Assert.That(skin.bones.Length,Is.EqualTo(24));Assert.That(skin.sharedMesh.bindposes.Length,Is.EqualTo(skin.bones.Length));Assert.That(skin.sharedMesh.vertexCount,Is.GreaterThan(3000));
                Assert.That(skin.sharedMesh.bounds.size.y,Is.InRange(1.75f,1.85f));Assert.That(node.GetComponentsInChildren<Collider>(true),Is.Empty);
                foreach(var w in skin.sharedMesh.boneWeights)
                {Assert.That(w.weight0+w.weight1+w.weight2+w.weight3,Is.EqualTo(1).Within(.0001f));Assert.That(w.boneIndex0,Is.InRange(0,skin.bones.Length-1));Assert.That(w.boneIndex1,Is.InRange(0,skin.bones.Length-1));}
                Assert.That(skin.sharedMaterials.All(m=>m!=null&&m.shader.name.Contains("Universal")),Is.True);
                if(id=="monk")
                {
                    var points=skin.sharedMesh.vertices;
                    var wrap=skin.sharedMesh.GetTriangles(4).Distinct().Select(i=>points[i]).Where(p=>p.x>.569f&&p.x<.59f&&p.y>1.2f).ToArray();
                    Assert.That(wrap,Is.Not.Empty);
                    Assert.That(wrap.Min(p=>new Vector2(p.y-1.42f,p.z).magnitude),Is.GreaterThan(.065f),"Vendas rodean la piel; comprobar buffers guardados, no solo código de autoría");
                }
                foreach(var material in new[]{0,3,4,6,8})
                {
                    var indices=skin.sharedMesh.GetTriangles(material);var points=skin.sharedMesh.vertices;var normal=skin.sharedMesh.normals;var inward=0;
                    for(var i=0;i<indices.Length;i+=3){var a=indices[i];var b=indices[i+1];var c=indices[i+2];var cross=Vector3.Cross(points[b]-points[a],points[c]-points[a]);if(Vector3.Dot(cross,normal[a]+normal[b]+normal[c])<-.0000001f)inward++;}
                    Assert.That(inward,Is.Zero,"Winding/normal coherentes en material "+material);
                }
                var root=rig.Animator.gameObject;var position=node.transform.position;var maximum=0f;
                driver.Profile.Idle.SampleAnimation(root,0);skin.BakeMesh(before);var original=before.vertices;
                foreach(CharacterMotion motion in Enum.GetValues(typeof(CharacterMotion)))
                {
                    Assert.That(driver.Profile.HasClip(motion),Is.True,motion.ToString());var clip=driver.Profile.Clip(motion);Assert.That(clip.events,Is.Empty);Assert.That(clip.legacy,Is.False);
                    var bindings=AnimationUtility.GetCurveBindings(clip);Assert.That(bindings.Any(b=>b.path.Contains("Forearm")),Is.True);Assert.That(bindings.Any(b=>b.path.Contains("LowerLeg")),Is.True);
                    clip.SampleAnimation(root,clip.length*.5f);skin.BakeMesh(baked);Assert.That(baked.vertexCount,Is.EqualTo(original.Length));
                    var v=baked.vertices;var displacement=0f;for(var i=0;i<v.Length;i++){Assert.That(float.IsNaN(v[i].x)||float.IsInfinity(v[i].y),Is.False);displacement=Mathf.Max(displacement,Vector3.Distance(v[i],original[i]));}
                    if(motion==CharacterMotion.Attack){Assert.That(displacement,Is.GreaterThan(.08f),"Skin deforma de verdad");maximum=displacement;}
                    Assert.That(node.transform.position,Is.EqualTo(position),"Root actor no se desplaza desde clips");
                }
                Directory.CreateDirectory("Logs/E08");File.WriteAllText("Logs/E08/rig-"+id+".txt","PASS Generic valid; bones="+skin.bones.Length+" vertices="+original.Length+" triangles="+skin.sharedMesh.triangles.Length/3+"; normalized skin; seven articulated clips; attack max displacement="+maximum.ToString("F3")+" m; root unchanged. Human deformation/art review pending.");
            }
            finally{Object.DestroyImmediate(before);Object.DestroyImmediate(baked);Object.DestroyImmediate(node);}
        }
        [Test]public void IntegrationPreservesFallbackAndOriginalScene()
        {
            var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AnimatedCharacterBuilder.CatalogPath);
            var previous=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(Aethernia.Editor.Environment.CharacterVisualBuilder.CatalogPath);
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                Assert.That(catalog.FindEntry(id).Prefab,Is.SameAs(previous.FindEntry(id).Prefab));Assert.That(previous.Find(id).GetComponent<HumanoidVisualRig>(),Is.Null);
                var strict=ScriptableObject.CreateInstance<HumanoidAnimationProfile>();try{strict.Idle=catalog.Find(id).GetComponent<HumanoidAnimationDriver>().Profile.Idle;Assert.That(strict.HasClip(CharacterMotion.Idle),Is.False);}finally{Object.DestroyImmediate(strict);}
            }
            EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);var d=Object.FindAnyObjectByType<PrototypeDirector>();Assert.That(d.Hero.GetComponent<CharacterCombatAnimationPresenter>(),Is.Not.Null);Assert.That(GameObject.Find("E07 Medieval Village"),Is.Not.Null);
        }
        [UnityTest]public IEnumerator RealMovementAcceptedCombatAndEquipment()=>Integrated();
        private static IEnumerator Integrated()
        {
            EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var output=new StringBuilder();
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                d.Execute("explore");d.Session.SelectClass(id);Assert.That(d.Hero.Warp(new Vector3(-3,0,-6)),Is.True);yield return null;yield return null;yield return null;
                var view=d.Hero.GetComponent<CharacterVisualPresenter>();var driver=view.Model.GetComponent<HumanoidAnimationDriver>();Assert.That(driver.CanAnimate,Is.True);Assert.That(driver.Rig.Animator.applyRootMotion,Is.False);
                Assert.That(d.Hero.TryMoveTo(new Vector3(-8,0,-6)),Is.True);var walk=false;var run=false;var deadline=Time.realtimeSinceStartup+10;
                while(d.Hero.IsMoving&&Time.realtimeSinceStartup<deadline)
                {yield return null;var speed=driver.ObservedSpeed;walk|=speed>.05f&&speed<driver.Profile.RunSpeed&&InState(driver.Rig.Animator,"Walk");run|=speed>=driver.Profile.RunSpeed&&InState(driver.Rig.Animator,"Run");}
                Assert.That(Vector3.Distance(d.Hero.transform.position,new Vector3(-8,0,-6)),Is.LessThan(.3f));Assert.That(walk&&run,Is.True,"Velocidades reales Walk/Run");
                yield return Wait(.3f);Assert.That(driver.Rig.Animator.GetInteger("Locomotion"),Is.Zero);
                WorldOperations.AddItem(d.Session.State,"training-sword",1);Assert.That(d.Execute("sword"),Is.True);Assert.That(view.Model.SwordVisible,Is.True);
                var socket=view.Model.WeaponAnchor;Assert.That(socket.parent.name,Is.EqualTo("RightHand"));var socketStart=socket.position;
                var presenter=d.Hero.GetComponent<CharacterCombatAnimationPresenter>();var count=presenter.PlayedActions;
                Assert.That(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.forward*1.6f,42),Is.True);yield return Wait(.7f);
                d.Encounter.Attack();Assert.That(d.Encounter.Battle.ActionAvailable,Is.False);yield return Wait(.20f);
                Assert.That(presenter.PlayedActions,Is.GreaterThan(count));Assert.That(InState(driver.Rig.Animator,"Attack"),Is.True);
                yield return Wait(.18f);Assert.That(Vector3.Distance(socket.position,socketStart),Is.GreaterThan(.03f),"Espada sigue hueso animado");
                Camera.main.GetComponent<IsometricCameraRig>().FocusOn(d.Hero.transform.position);
                var cameraRig=Camera.main.GetComponent<IsometricCameraRig>();cameraRig.ApplyInput(Vector2.zero,cameraRig.Size-4.3f,0,false);
                Directory.CreateDirectory("Logs/E08/Captures");yield return SceneCapture.Save("Logs/E08/Captures/Actual-"+id+"-Attack.png",1280,720);
                var after=presenter.PlayedActions;d.Encounter.Attack();yield return Wait(.1f);Assert.That(presenter.PlayedActions,Is.EqualTo(after),"Comando sin acción no anima otro ataque");
                yield return Wait(driver.Profile.Attack.length+.25f);
                var initialHealth=d.Session.State.Health;
                for(var turn=0;turn<15&&d.Session.State.Health==initialHealth;turn++){d.Encounter.EndTurn();yield return Wait(.02f);}
                Assert.That(d.Session.State.Health,Is.LessThan(initialHealth));yield return Wait(.10f);
                Assert.That(InState(driver.Rig.Animator,"Hit"),Is.True,"Hit disparado por daño real");yield return SceneCapture.Save("Logs/E08/Captures/Actual-"+id+"-Hit.png",1280,720);
                yield return Wait(.6f);
                d.Encounter.Guard();yield return Wait(.25f);Assert.That(driver.Rig.Animator.GetBool("Defending"),Is.True);Assert.That(InState(driver.Rig.Animator,"Defend"),Is.True);yield return SceneCapture.Save("Logs/E08/Captures/Actual-"+id+"-Defend.png",1280,720);d.Encounter.EndTurn();yield return Wait(.6f);
                var hitSeen=false;
                for(var turn=0;turn<40&&!d.Encounter.Battle.Finished;turn++)
                {
                    var health=d.Session.State.Health;d.Encounter.EndTurn();yield return Wait(.18f);
                    hitSeen|=d.Session.State.Health<health;
                }
                Assert.That(hitSeen,Is.True);Assert.That(d.Session.State.Health,Is.Zero,"Derrota por resultados reales, sin inyectar daño");
                yield return Wait(1.5f);
                var status=driver.Rig.Animator;var info=status.GetCurrentAnimatorStateInfo(0);var names=string.Join(",",status.GetCurrentAnimatorClipInfo(0).Select(c=>c.clip.name));
                Assert.That(info.IsName("Defeat"),Is.True,"class="+id+" clips="+names+" defeated="+status.GetBool("Defeated")+" health="+d.Session.State.Health);
                yield return SceneCapture.Save("Logs/E08/Captures/Actual-"+id+"-Defeat.png",1280,720);
                d.Execute("explore");var save="Logs/E08-save-"+id+".json";Assert.That(d.Save(save)&&d.Load(save),Is.True);yield return null;Assert.That(d.Session.State.ClassId,Is.EqualTo(id));Assert.That(view.Model.SwordVisible,Is.True);Assert.That(d.Hero.Agent.radius,Is.EqualTo(.3f));Assert.That(d.Hero.Agent.height,Is.EqualTo(1.8f));
                output.AppendLine(id+": PASS real walk/run/arrival; accepted/rejected strike; animated hand socket; guard/end; real hits and defeat; save v1/equipment; unchanged agent.");
            }
            d.Session.SelectClass("warrior");WorldOperations.AddItem(d.Session.State,"cloth",1);Assert.That(d.Execute("armour"),Is.True);yield return null;yield return null;yield return null;
            Assert.That(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.forward*1.6f,42),Is.True);
            for(var turn=0;turn<30&&!d.Encounter.Battle.Finished;turn++)
            {if(d.Session.State.Health<8&&!d.Encounter.Battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();if(d.Encounter.Battle.Finished)break;d.Encounter.EndTurn();yield return null;}
            Assert.That(d.Encounter.Battle.Won,Is.True);var finalPresenter=d.Hero.GetComponent<CharacterCombatAnimationPresenter>();var played=finalPresenter.PlayedActions;var healthAfterVictory=d.Session.State.Health;
            Assert.That(d.Execute("explore"),Is.True);yield return Wait(.2f);
            Assert.That(finalPresenter.PlayedActions,Is.GreaterThan(played),"El regreso a explorar conserva el golpe aceptado del mismo frame");
            Assert.That(InState(d.Hero.GetComponent<CharacterVisualPresenter>().Model.GetComponent<HumanoidAnimationDriver>().Rig.Animator,"Attack"),Is.True);
            Assert.That(d.Encounter.Battle,Is.Null);Assert.That(d.Session.State.Health,Is.EqualTo(healthAfterVictory));output.AppendLine("PASS final accepted strike survives victory cleanup; ordinary reset/load still cancels pending presentation.");
            Directory.CreateDirectory("Logs/E08");File.WriteAllText("Logs/E08/integration.txt",output.ToString());LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest]public IEnumerator CaptureActualSkinnedPosesAndAngles()=>Evidence();
        private static IEnumerator Evidence()
        {
            EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            Directory.CreateDirectory("Logs/E08/Captures");var d=Object.FindAnyObjectByType<PrototypeDirector>();var camera=Camera.main;camera.GetComponent<IsometricCameraRig>().enabled=false;camera.orthographic=true;camera.orthographicSize=1.45f;
            Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>().enabled=false;
            var fill=new GameObject("E08 temporary inspection fill").AddComponent<Light>();fill.type=LightType.Directional;fill.intensity=1.1f;fill.shadows=LightShadows.None;
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                d.Session.SelectClass(id);Assert.That(d.Hero.Warp(new Vector3(0,0,-4)),Is.True);yield return null;yield return null;
                var model=d.Hero.GetComponent<CharacterVisualPresenter>().Model;var driver=model.GetComponent<HumanoidAnimationDriver>();driver.enabled=false;driver.Rig.Animator.enabled=false;
                var target=d.Hero.transform.position+Vector3.up*.91f;model.transform.rotation=Quaternion.identity;
                driver.Profile.Idle.SampleAnimation(driver.Rig.Animator.gameObject,0);
                foreach(var angle in new[]{0,90,180,35})
                {
                    var direction=Quaternion.Euler(0,angle,0)*new Vector3(0,0,1);camera.transform.position=target+direction*4+Vector3.up*(angle==35?2.5f:.8f);camera.transform.LookAt(target);fill.transform.rotation=camera.transform.rotation;
                    yield return SceneCapture.Save("Logs/E08/Captures/"+id+"-View-"+angle+".png",1280,1024);
                }
                camera.transform.position=target+new Vector3(2.4f,2.2f,3.6f);camera.transform.LookAt(target);fill.transform.rotation=camera.transform.rotation;
                foreach(CharacterMotion motion in Enum.GetValues(typeof(CharacterMotion)))
                {
                    var clip=driver.Profile.Clip(motion);var phase=motion==CharacterMotion.Defeat?.95f:motion==CharacterMotion.Walk||motion==CharacterMotion.Run?.25f:.50f;clip.SampleAnimation(driver.Rig.Animator.gameObject,clip.length*phase);
                    yield return SceneCapture.Save("Logs/E08/Captures/"+id+"-Pose-"+motion+".png",1280,1024);
                }
                // Una secuencia de deformación real muestreada; no se presenta como comandos de combate.
                for(var frame=0;frame<12;frame++)
                {
                    driver.Profile.Attack.SampleAnimation(driver.Rig.Animator.gameObject,driver.Profile.Attack.length*frame/11f);
                    yield return SceneCapture.Save("Logs/E08/Captures/"+id+"-Attack-"+frame.ToString("D2")+".png",640,512);
                }
                driver.enabled=true;
            }
            File.WriteAllText("Logs/E08/captures.txt","PASS Unity/URP native camera: 12 class angles + 21 seven-state poses + 36 attack samples. Poses sampled on actual rig with temporary inspection fill; command integration verified separately. Human art approval pending.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator Wait(float seconds)
        {var until=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<until)yield return null;}
        private static bool InState(Animator animator,string name)
        {return animator.GetCurrentAnimatorStateInfo(0).IsName(name)||(animator.IsInTransition(0)&&animator.GetNextAnimatorStateInfo(0).IsName(name));}
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
