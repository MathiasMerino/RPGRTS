using System;
using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class GuardianCompositionTests
    {
        [UnityTest, Timeout(420000)]
        public IEnumerator ObservesBothGuardianiesWithTheAvailablePartySizes()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.IntegratedAlphaScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return VerifyCompositions();
        }

        private static IEnumerator VerifyCompositions()
        {
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            var party=Object.FindAnyObjectByType<PartyWorld>();
            var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();
            var camp=flow.GetComponent<DemoSettlementPanel>();
            const string report="Logs/E23/guardian-compositions.csv";
            Directory.CreateDirectory("Logs/E23");
            File.WriteAllText(report,"companions,guardian,outcome,hero_hp,max_hp\n");
            flow.SlotPath="Logs/E23/guardians-"+Guid.NewGuid().ToString("N")+".json";
            Assert.That(flow.Continue("../Docs/Validacion/UX02/PartidasFinales/human-warrior.json"),Is.True);
            yield return WorldExplorationRoute.MoveTo(flow,new Vector3(9,0,-10));
            var state=flow.Session.State;
            var workshop=state.Buildings.FindIndex(b=>b.Id=="workshop");
            if(!state.Buildings[workshop].ProductionPaused)
                Assert.That(camp.SelectBuilding(workshop)&&camp.ToggleProduction(),Is.True);
            Assert.That(camp.SelectBlueprint("farm")&&flow.Director.Construction.Place(new Vector3(11.5f,0,-14)),Is.True);
            Assert.That(camp.SelectBuilding(state.Buildings.Count-1)&&camp.ToggleProduction(),Is.True);
            Assert.That(flow.Director.Execute("explore"),Is.True);
            for(var rest=0;rest<8&&state.Health<flow.Session.HeroProfile.Health;rest++)
            {
                Assert.That(dungeon.Rest(),Is.True);
                while(dungeon.Resting)yield return null;
            }
            Assert.That(party.Recruit("archer")&&party.Recruit("brawler"),Is.True);
            var checkpoint=flow.SlotPath+".camp";
            Assert.That(flow.Director.Save(checkpoint),Is.True);
            dungeon.TestSeed=42;
            for(var count=0;count<=3;count++)
            {
                Assert.That(flow.Continue(checkpoint),Is.True);
                state=flow.Session.State;
                for(var i=count;i<state.Companions.Count;i++)
                    Assert.That(party.SetParty(state.Companions[i].Id,false),Is.True);
                yield return WorldExplorationRoute.Use(flow,"umbral-entry",NoCapture,"Entrada");
                for(var guardian=1;guardian<=2;guardian++)
                {
                    var fact="umbral-battle-"+guardian;
                    yield return WorldExplorationRoute.Use(flow,fact,NoCapture,"Guardianía");
                    var encounter=flow.Director.Encounter;
                    Assert.That(encounter.Group.Members.Count(m=>!m.Hostile),Is.EqualTo(count+1),
                        "Cada compañero disponible debe participar al iniciar la guardianía "+guardian);
                    yield return Fight(flow);
                    var won=state.Facts.Contains(fact);
                    File.AppendAllText(report,count+","+guardian+","+(won?"victoria":"derrota")+","+state.Health+","+flow.Session.HeroProfile.Health+"\n");
                    if(!won)
                    {
                        Assert.That(encounter.Group?.Finished,Is.True);
                        Assert.That(dungeon.Retreat(),Is.True);
                        break;
                    }
                    if(guardian==2)
                    {
                        yield return WorldExplorationRoute.Use(flow,"umbral-objective",NoCapture,"Hallazgo");
                        Assert.That(state.Facts,Does.Contain("umbral-reward"));
                    }
                }
            }
        }

        private static IEnumerator Fight(DemoFlow flow)
        {
            var encounter=flow.Director.Encounter;
            var deadline=Time.realtimeSinceStartup+100;
            while(encounter.GroupActive&&Time.realtimeSinceStartup<deadline)
            {
                var group=encounter.Group;
                var actor=encounter.GroupUnit(group.Current.Id);
                if(group.Current.Hostile||actor.IsMoving){yield return null;continue;}
                var enemy=group.Members.Where(m=>m.Hostile&&m.Health>0)
                    .OrderBy(m=>Vector3.Distance(actor.transform.position,encounter.GroupUnit(m.Id).transform.position)).First();
                encounter.SelectGroupTarget(enemy.Id);
                if(group.ActionAvailable)
                {
                    if(group.Current.Health<group.Current.Profile.Health/2f&&!group.Current.Recovered)encounter.GroupRecover();
                    else
                    {
                        encounter.GroupApproach();
                        yield return UxExperienceRoute.Arrive(actor);
                        if((!group.Current.Hero||!encounter.Technique())&&!encounter.GroupAttack()&&group.ActionAvailable)encounter.GroupGuard();
                    }
                }
                if(encounter.GroupActive)encounter.GroupEnd();
                yield return null;
            }
            Assert.That(encounter.GroupActive,Is.False,"El encuentro debe resolverse con victoria o derrota dentro del límite.");
        }

        private static IEnumerator NoCapture(string _) { yield break; }
        [UnityTearDown] public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}

