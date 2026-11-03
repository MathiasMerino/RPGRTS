using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class DemoRtsTests
    {
        private sealed class Counter{public int Guards,ResolutionCount;public bool? Result;public void Outcome(bool won){Result=won;ResolutionCount++;}public void Receive(CombatFeedback r){if(r.Result.Kind==CombatActionKind.Guard)Guards++;}}
        [UnityTest] public IEnumerator PatrolUsesSameHeroAndRealOrdersForThreeClasses()=>Patrol();
        internal static IEnumerator Patrol(string[] order=null,int? frameCap=null,bool validateTerminalOnly=false,bool protectHero=false)
        {
            EditorSceneManager.OpenScene(FirstDemoBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;
            var oldCap=Application.targetFrameRate;var oldSync=QualitySettings.vSyncCount;if(frameCap.HasValue){QualitySettings.vSyncCount=0;Application.targetFrameRate=frameCap.Value;}flow.SlotPath="Logs/E12/patrol-"+System.Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;flow.EncounterSeed=42;Directory.CreateDirectory("Logs/E12/Captures");
            foreach(var id in order??new[]{"warrior","monk","mage"})
            {
                flow.ShowMain();Assert.That(flow.StartNew("Patrullero",id,"human"),Is.True);flow.CompleteIntro();yield return null;
                d.Encounter.Approach();yield return Arrive(d.Hero);
                for(var turn=0;turn<30&&flow.Session.State.Demo.Chapter==DemoChapter.FirstCombat;turn++)
                {
                    var b=d.Encounter.Battle;if(b.HeroHealth<8&&!b.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                    if(flow.Session.State.Demo.Chapter!=DemoChapter.FirstCombat)break;Assert.That(d.Execute("end"),Is.True);var phase=Time.realtimeSinceStartup+5;
                    while(d.Encounter.Battle!=null&&d.Encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<phase)yield return null;
                }
                Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.RoadToRts),"Tutorial legal antes de patrulla");
                Assert.That(d.Execute("ration"),Is.True);Assert.That(d.Hero.TryMoveTo(flow.Patrol.transform.position+Vector3.back*1.1f),Is.True);yield return Arrive(d.Hero);
                var hero=d.Hero;var model=hero.GetComponent<CharacterVisualPresenter>().Model;var health=flow.Session.State.Health;var rts=d.Rts;
                using var trace=new RtsTraceRecorder(d.Rts,flow,id,frameCap);
                Assert.That(flow.Patrol.TryInteract(hero,flow.Session),Is.True);Assert.That(rts.IncludesHero&&rts.Active,Is.True);var units=new List<RtsUnitStatus>();rts.CopyStatus(units);
                Assert.That(units.Count,Is.EqualTo(13));Assert.That(units.Single(x=>x.Hero).Unit,Is.SameAs(hero));Assert.That(units.Single(x=>x.Hero).Health,Is.EqualTo(health));Assert.That(hero.GetComponent<CharacterVisualPresenter>().Model,Is.SameAs(model));
                Assert.That(units.Where(x=>!x.Hero).Select(x=>x.Role).Distinct().Count(),Is.EqualTo(3));
                var ally=units.First(x=>x.Team==0&&!x.Hero).Unit;var foe=units.First(x=>x.Team==1).Unit;
                Assert.That(rts.SelectUnit(ally.GetComponentInChildren<Collider>(),false),Is.True);Assert.That(rts.SelectedAllies,Is.EqualTo(1));Assert.That(rts.SelectUnit(hero.GetComponentInChildren<Collider>(),true),Is.True);Assert.That(rts.SelectedAllies,Is.EqualTo(2));
                var counter=new Counter();rts.ActionResolved+=counter.Receive;rts.Resolved+=counter.Outcome;Assert.That(rts.HeroAction("guard"),Is.True);Assert.That(rts.HeroAction("guard"),Is.False);Assert.That(counter.Guards,Is.EqualTo(1));
                rts.SelectArea(Camera.main,new Rect(0,0,Screen.width,Screen.height),false);Assert.That(rts.SelectedAllies,Is.EqualTo(7));var group=new List<ExplorationUnit>();rts.RememberGroup(group);rts.SelectUnit(ally.GetComponentInChildren<Collider>(),false);rts.SelectGroup(group,false);Assert.That(rts.SelectedAllies,Is.EqualTo(7));
                Assert.That(rts.Command(new Vector3(-10,0,13),null,false),Is.True);yield return TacticalCombatIntegrationTests.Wait(.15f);Assert.That(rts.Command(foe.transform.position,foe.GetComponentInChildren<Collider>(),false),Is.True);
                yield return TacticalCombatIntegrationTests.Wait(.4f);yield return DemoCapture.Save("Logs/E12/Captures/"+id+"-Patrol.png",1280,720);
                var orderAt=0f;var limit=Time.realtimeSinceStartup+45;while(rts.Active&&Time.realtimeSinceStartup<limit)
                {
                    if(protectHero&&Time.time>=orderAt)
                    {
                        // Legal hold orders keep the vulnerable hero behind the advancing army.
                        Assert.That(rts.SelectUnit(hero.GetComponentInChildren<Collider>(),false),Is.True);
                        Assert.That(rts.Command(new Vector3(-14.2f,0,9.5f),null,false),Is.True);orderAt=Time.time+.6f;
                    }
                    if(flow.Session.State.Health<flow.Session.Rules.Find(id).Health)rts.HeroAction("recover");yield return null;
                }
                Assert.That(rts.Active,Is.False,"RTS debe terminar con combate real: "+id);
                // Contrato de resolución bajo distintas temporizaciones; el caso histórico
                // de victoria conserva sus aserciones y estrategia originales.
                Assert.That(counter.ResolutionCount,Is.EqualTo(1));Assert.That(rts.LastResolvedVictory,Is.EqualTo(counter.Result));
                if(validateTerminalOnly&&counter.Result==false)
                {
                    Assert.That(flow.Session.State.Health==0||rts.Allies==0,Is.True,"Derrota justificada por estado real");
                    Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.FirstRts));Assert.That(flow.Session.Paused,Is.True);
                    Assert.That(flow.Front.Screen,Is.EqualTo(DemoScreen.Retry));
                    flow.RetryCurrentEncounter();Assert.That(rts.Active&&rts.IncludesHero,Is.True,"Reintento mediante acción pública existente");
                    Assert.That(flow.Session.State.Health,Is.EqualTo(flow.Session.Rules.Find(id).Health));
                    Assert.That(rts.LastResolvedVictory,Is.Null,"Nuevo intento sin resultado anterior");rts.Clear();rts.ActionResolved-=counter.Receive;rts.Resolved-=counter.Outcome;continue;
                }
                Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.Settlement),id+": victoria RTS real");Assert.That(flow.Session.State.ClassId,Is.EqualTo(id));Assert.That(d.Hero,Is.SameAs(hero));Assert.That(d.Hero.gameObject.activeInHierarchy,Is.True);rts.ActionResolved-=counter.Receive;rts.Resolved-=counter.Outcome;
                Assert.That(d.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.State.Demo.Chapter,Is.EqualTo(DemoChapter.Settlement));
                yield return TacticalCombatIntegrationTests.Wait(.3f);yield return DemoCapture.Save("Logs/E12/Captures/"+id+"-Secured.png",1280,720);
            }
            File.WriteAllText("Logs/E12/patrol.txt","PASS three classes: actual RPG -> navigation/interaction -> 6v6+same hero RTS, role/range data, single/additive/rectangle selection and groups, move/attack orders, real guard with duplicate cooldown rejection, combat victory -> secured camp and v1 continue. No forced health/victory.");
            Application.targetFrameRate=oldCap;QualitySettings.vSyncCount=oldSync;
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator Arrive(ExplorationUnit unit){var end=Time.realtimeSinceStartup+20;yield return null;while(unit.IsMoving&&Time.realtimeSinceStartup<end)yield return null;Assert.That(unit.IsMoving,Is.False);}
        [UnityTearDown] public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
