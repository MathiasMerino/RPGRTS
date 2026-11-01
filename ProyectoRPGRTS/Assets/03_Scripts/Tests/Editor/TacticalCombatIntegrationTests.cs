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
    public class TacticalCombatIntegrationTests
    {
        private sealed class Counter
        {public int Count;public void Receive(CombatFeedback r){if(r.Result.Actor==CombatSide.Player&&r.Result.Kind==CombatActionKind.Attack)Count++;}}
        [Test]public void OriginalPcmAndUrpResourcesAreUsable()
        {
            var bank=AssetDatabase.LoadAssetAtPath<CombatAudioBank>(E09CombatBuilder.BankPath);Assert.That(bank,Is.Not.Null);Assert.That(bank.Clips.Length,Is.EqualTo(14));
            foreach(var clip in bank.Clips)
            {
                Assert.That(clip,Is.Not.Null);Assert.That(clip.channels,Is.EqualTo(1));Assert.That(clip.length,Is.GreaterThan(.05f));var data=new float[clip.samples];Assert.That(clip.GetData(data,0),Is.True);
                Assert.That(data.Max(x=>Mathf.Abs(x)),Is.InRange(.50f,.61f));Assert.That(Mathf.Sqrt(data.Average(x=>x*x)),Is.GreaterThan(.005f));
            }
            var impact=AssetDatabase.LoadAssetAtPath<GameObject>(E09CombatBuilder.ImpactPath);Assert.That(impact.GetComponent<ParticleSystem>().main.maxParticles,Is.EqualTo(24));Assert.That(impact.GetComponent<ParticleSystemRenderer>().sharedMaterial.shader.name,Does.Contain("Universal"));
            EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);Assert.That(Object.FindAnyObjectByType<PrototypeDirector>().Encounter.UsesStagedCommands,Is.True);Assert.That(Object.FindAnyObjectByType<CombatHudPanel>(),Is.Not.Null);
            Assert.That(Object.FindAnyObjectByType<CombatHudPanel>().HealthFill.sprite,Is.Not.Null,"uGUI necesita sprite para mostrar relleno parcial");
        }
        [UnityTest]public IEnumerator EnemyPhaseRejectsDuplicatesAndMissingClipsNeverBlock()=>Phases();
        private static IEnumerator Phases()
        {
            EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();d.Session.SelectClass("warrior");Assert.That(d.Hero.Warp(new Vector3(-3.4f,0,-5)),Is.True);yield return null;
            Assert.That(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.forward*5,42),Is.True);var before=d.Encounter.EnemyPosition;var health=d.Session.State.Health;
            Assert.That(d.Encounter.TrySelect(d.Encounter.Enemy.GetComponentInChildren<Collider>()),Is.True);Assert.That(d.Encounter.TrySelect(d.Hero.GetComponentInChildren<Collider>()),Is.False);
            Assert.That(d.Execute("end"),Is.True);Assert.That(d.Encounter.Battle.Phase,Is.EqualTo(TurnPhase.Enemy));Assert.That(d.Execute("end"),Is.False);
            d.Encounter.Attack();Assert.That(d.Encounter.Battle.ActionAvailable,Is.True);Assert.That(d.Encounter.Battle.EnemyHealth,Is.EqualTo(18));yield return WaitPhase(d);
            Assert.That(d.Encounter.LastEnemyIntent,Is.EqualTo(EnemyIntent.Approach));Assert.That(Vector3.Distance(before,d.Encounter.EnemyPosition),Is.InRange(.1f,3.05f));Assert.That(d.Session.State.Health,Is.EqualTo(health));Assert.That(d.Encounter.Battle.LastEnemyResolution.Kind,Is.EqualTo(CombatActionKind.Wait));
            d.Encounter.Approach();yield return Arrive(d.Hero);var model=d.Hero.GetComponent<CharacterVisualPresenter>().Model;var driver=model.GetComponent<HumanoidAnimationDriver>();var original=driver.Profile;
            var missing=Object.Instantiate(original);missing.Attack=null;driver.Configure(driver.Rig,missing);yield return null;yield return null;yield return null;
            var counter=new Counter();d.Encounter.ActionResolved+=counter.Receive;
            var hp=d.Encounter.Battle.EnemyHealth;d.Encounter.Attack();var resolution=d.Encounter.Battle.LastHeroResolution;Assert.That(resolution.Accepted,Is.True);Assert.That(d.Encounter.Battle.EnemyHealth,Is.EqualTo(hp-resolution.Damage));
            d.Encounter.Attack();Assert.That(counter.Count,Is.EqualTo(1));Assert.That(d.Encounter.Battle.EnemyHealth,Is.EqualTo(hp-resolution.Damage));Assert.That(driver.TryPlay(CharacterMotion.Attack),Is.False);
            var fx=Object.FindAnyObjectByType<CombatFeedbackPresenter>();var presented=fx.PresentedCount;yield return Wait(.20f);Assert.That(fx.PresentedCount,Is.GreaterThan(presented));
            Assert.That(d.Execute("end"),Is.True);yield return WaitPhase(d);Assert.That(d.Encounter.Battle.Phase,Is.EqualTo(TurnPhase.Player));
            driver.Configure(driver.Rig,null);Assert.That(driver.CanAnimate,Is.False);d.Encounter.Guard();Assert.That(d.Execute("end"),Is.True);yield return WaitPhase(d);Assert.That(d.Encounter.Battle.Phase,Is.EqualTo(TurnPhase.Player));
            driver.Configure(driver.Rig,original);Object.Destroy(missing);d.Encounter.ActionResolved-=counter.Receive;
            d.Encounter.Clear();d.Session.SelectClass("warrior");Assert.That(d.Hero.Warp(new Vector3(-3.4f,0,-5)),Is.True);yield return null;
            Assert.That(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.forward*1.2f,42),Is.True);
            var obstacle=GameObject.CreatePrimitive(PrimitiveType.Cube);obstacle.name="E09 temporary LOS assertion";obstacle.transform.position=(d.Hero.transform.position+d.Encounter.EnemyPosition)*.5f+Vector3.up;
            obstacle.transform.localScale=new Vector3(1.5f,2,.15f);Physics.SyncTransforms();var blockedHp=d.Session.State.Health;
            d.Encounter.Attack();Assert.That(d.Encounter.Battle.ActionAvailable,Is.True,"LOS bloqueada rechaza el ataque sin gastar accion");
            Assert.That(d.Execute("end"),Is.True);yield return WaitPhase(d);Assert.That(d.Session.State.Health,Is.EqualTo(blockedHp));Assert.That(d.Encounter.Battle.LastEnemyResolution.Kind,Is.EqualTo(CombatActionKind.Wait));
            Assert.That(d.Encounter.LastEnemyIntent,Is.EqualTo(EnemyIntent.Wait));Object.Destroy(obstacle);
            yield return null;d.Encounter.Clear();
            var empty=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(AnimatedCharacterBuilder.Prefabs+"/warrior_Animated.prefab"));empty.SetActive(false);var emptyDriver=empty.GetComponent<HumanoidAnimationDriver>();emptyDriver.Configure(emptyDriver.Rig,null);
            d.Encounter.ConfigureTactics(true,empty);Assert.That(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.forward*1.2f,42),Is.True);yield return null;
            Assert.That(Object.FindAnyObjectByType<EnemyCombatVisual>().Visual,Is.Null,"Perfil enemigo ausente usa capsula sin bloquear reglas");Assert.That(d.Execute("end"),Is.True);yield return WaitPhase(d);Object.Destroy(empty);
            Assert.That(fx.ActiveEffects,Is.LessThanOrEqualTo(8));Assert.That(Object.FindAnyObjectByType<CombatAudioPlayer>().ActiveVoices,Is.LessThanOrEqualTo(6));
            Directory.CreateDirectory("Logs/E09");File.WriteAllText("Logs/E09/phases-fallback.txt","PASS: staged enemy phase rejects duplicate end/hero input; melee navigates <=3m and waits out of reach; accepted attack damages once, repeat rejected; missing Attack/all profiles do not block turn; physical LOS obstacle rejects player attack and enemy damage; effect/voice budgets bounded.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest]public IEnumerator ThreeClassesCompleteRealStagedQuestWithEvidence()=>Quest();
        private static IEnumerator Quest()
        {
            EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            Directory.CreateDirectory("Logs/E09/Captures");var d=Object.FindAnyObjectByType<PrototypeDirector>();var q=Object.FindAnyObjectByType<LostCargoController>();q.EncounterSeed=42;
            Assert.That(d.Hero.Warp(new Vector3(-3.4f,0,-5)),Is.True);yield return null;Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(d.Hero,d.Session),Is.True);Assert.That(d.Execute("sword")&&d.Execute("armour"),Is.True);
            Assert.That(d.Save("Logs/E09/base.json"),Is.True,PrototypeSaveStore.LastWriteFailure);var report=new StringBuilder();
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                Assert.That(d.Load("Logs/E09/base.json"),Is.True);d.Session.SelectClass(id);d.Hero.Select(true);
                Assert.That(d.Hero.TryMoveTo(q.Npc.transform.position+Vector3.back*1.1f),Is.True);yield return Arrive(d.Hero);Assert.That(q.Npc.TryInteract(d.Hero,d.Session)&&q.Accept(),Is.True);
                Assert.That(d.Save("Logs/E09/accepted-"+id+".json")&&d.Load("Logs/E09/accepted-"+id+".json"),Is.True,PrototypeSaveStore.LastWriteFailure);
                Assert.That(d.Hero.TryMoveTo(q.Cargo.transform.position+Vector3.back*1.6f),Is.True);yield return Arrive(d.Hero);Assert.That(q.Cargo.TryInteract(d.Hero,d.Session),Is.True);
                d.Encounter.Approach();yield return Arrive(d.Hero);var cameraRig=Camera.main.GetComponent<IsometricCameraRig>();cameraRig.FocusOn(d.Hero.transform.position);yield return Wait(.35f);
                yield return SceneCapture.Save("Logs/E09/Captures/"+id+"-Ready.png",1280,720);
                var strike=false;var enemyShot=false;var damageShot=false;
                for(var turn=0;turn<30&&q.CargoEncounter;turn++)
                {
                    var battle=d.Encounter.Battle;
                    if(d.Session.State.Health<8&&!battle.RecoveryUsed)d.Encounter.Recover();
                    else
                    {
                        d.Encounter.Attack();
                        if(!strike&&battle.LastHeroResolution.Hit)
                        {
                            yield return Wait(.48f);yield return SceneCapture.Save("Logs/E09/Captures/"+id+"-Attack.png",1280,720);strike=true;
                            if(battle.EnemyHealth>0)
                            {
                                var widget=Object.FindAnyObjectByType<CombatHudPanel>();Canvas.ForceUpdateCanvases();var mesh=widget.HealthFill.canvasRenderer.GetMesh();
                                Assert.That(mesh.bounds.size.x,Is.EqualTo(widget.HealthFill.rectTransform.rect.width*battle.EnemyHealth/18f).Within(2),"Geometría visible de la barra, no solo fillAmount");
                            }
                        }
                    }
                    if(!q.CargoEncounter)break;
                    var hp=d.Session.State.Health;Assert.That(d.Execute("end"),Is.True);
                    if(!enemyShot){yield return Wait(1.12f);Assert.That(battle.Phase,Is.EqualTo(TurnPhase.Enemy));yield return SceneCapture.Save("Logs/E09/Captures/"+id+"-EnemyTurn.png",1280,720);enemyShot=true;}
                    yield return WaitPhase(d);
                    if(!damageShot&&d.Session.State.Health<hp){yield return Wait(.12f);yield return SceneCapture.Save("Logs/E09/Captures/"+id+"-Damage.png",1280,720);damageShot=true;}
                }
                Assert.That(LostCargoQuest.Stage(d.Session.State),Is.EqualTo(CargoQuestStage.Recovered),id+": victoria real");Assert.That(strike&&enemyShot,Is.True);
                yield return Wait(.60f);yield return SceneCapture.Save("Logs/E09/Captures/"+id+"-Victory.png",1280,720);
                Assert.That(d.Save("Logs/E09/recovered-"+id+".json")&&d.Load("Logs/E09/recovered-"+id+".json"),Is.True,PrototypeSaveStore.LastWriteFailure);
                Assert.That(d.Hero.TryMoveTo(q.Npc.transform.position+Vector3.back*1.1f),Is.True);yield return Arrive(d.Hero);cameraRig.FocusOn(q.Npc.transform.position);var gold=d.Session.State.Gold;Assert.That(q.Claim(),Is.True);Assert.That(q.Claim(),Is.False);Assert.That(d.Session.State.Gold,Is.EqualTo(gold+20));
                Assert.That(d.Save("Logs/E09/completed-"+id+".json")&&d.Load("Logs/E09/completed-"+id+".json"),Is.True,PrototypeSaveStore.LastWriteFailure);Assert.That(LostCargoQuest.Stage(d.Session.State),Is.EqualTo(CargoQuestStage.Completed));yield return null;yield return SceneCapture.Save("Logs/E09/Captures/"+id+"-Completed.png",1280,720);
                report.AppendLine(id+": PASS NPC/path/accept/actual dice combat/staged enemy turns/victory/return/unique reward/v1 save-load; no forced health/victory.");
            }
            File.WriteAllText("Logs/E09/quest.txt",report.ToString());LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        public static IEnumerator Wait(float seconds){var end=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<end)yield return null;}
        private static IEnumerator Arrive(ExplorationUnit actor)
        {var end=Time.realtimeSinceStartup+15;yield return null;while(actor.IsMoving&&Time.realtimeSinceStartup<end)yield return null;Assert.That(actor.IsMoving,Is.False,"Arrival timeout");}
        private static IEnumerator WaitPhase(PrototypeDirector d)
        {var end=Time.realtimeSinceStartup+5;while(d.Encounter.Battle!=null&&d.Encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<end)yield return null;Assert.That(d.Encounter.Battle==null||d.Encounter.Battle.Phase!=TurnPhase.Enemy,Is.True,"Enemy must finish independently of animation");}
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
