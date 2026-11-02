using System.Collections;
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
    public sealed class ExpeditionIntegrationTests
    {
        [UnityTest,Timeout(600000)]public IEnumerator PartyFollowsTravelsFightsAndRestoresWithoutDuplicates()=>Run();
        private static IEnumerator Run()
        {
            EditorSceneManager.OpenScene(ExpeditionBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var party=Object.FindAnyObjectByType<PartyWorld>();var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();var d=flow.Director;
            flow.SlotPath="Logs/E14/integration-"+System.Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;dungeon.TestSeed=42;
            var state=ExpeditionRulesTests.Ready();WorldOperations.AddItem(state,"training-sword",1);WorldOperations.AddItem(state,"cloth",1);WorldOperations.Equip(state,"training-sword");WorldOperations.Equip(state,"cloth");
            Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;
            foreach(var role in party.Roles.Roles)Assert.That(party.Recruit(role.Id),Is.True);yield return null;yield return null;Assert.That(party.Units.Count,Is.EqualTo(3));
            Assert.That(d.Hero.TryMoveTo(new Vector3(11,0,-10.5f)),Is.True);yield return Arrive(d.Hero);yield return TacticalCombatIntegrationTests.Wait(1.2f);
            foreach(var u in party.Units.Values){Assert.That(u.Agent.isOnNavMesh,Is.True);Assert.That(Vector3.Distance(u.transform.position,d.Hero.transform.position),Is.LessThan(6));}
            Assert.That(d.Hero.TryMoveTo(new Vector3(4,0,-12)),Is.True);yield return Arrive(d.Hero);
            Assert.That(Find("umbral-entry").TryInteract(d.Hero,flow.Session),Is.True);yield return null;yield return null;Assert.That(dungeon.Inside,Is.True);Assert.That(party.Units.Count,Is.EqualTo(3));
            Directory.CreateDirectory("Logs/E14/Captures");yield return DemoCapture.Save("Logs/E14/Captures/Party-Interior.png",1280,720);
            Assert.That(d.Hero.TryMoveTo(new Vector3(76,0,-2)),Is.True);yield return Arrive(d.Hero);yield return TacticalCombatIntegrationTests.Wait(.5f);
            Assert.That(Find("umbral-battle-1").TryInteract(d.Hero,flow.Session),Is.True);Assert.That(d.Encounter.Group.Members.Count(a=>!a.Hostile),Is.EqualTo(4));var clock=flow.Session.State.WorldSeconds;
            yield return Fight(d.Encounter);Assert.That(flow.Session.HasFact("umbral-battle-1"),Is.True);Assert.That(flow.Session.State.WorldSeconds,Is.EqualTo(clock).Within(.1));
            Assert.That(d.Hero.TryMoveTo(new Vector3(84,0,-2)),Is.True);yield return Arrive(d.Hero);yield return TacticalCombatIntegrationTests.Wait(.5f);Assert.That(Find("umbral-battle-2").TryInteract(d.Hero,flow.Session),Is.True);
            yield return Fight(d.Encounter);Assert.That(flow.Session.HasFact("umbral-battle-2"),Is.True);
            Assert.That(d.Hero.TryMoveTo(new Vector3(89,0,3)),Is.True);yield return Arrive(d.Hero);var gold=flow.Session.State.Gold;Assert.That(Find("umbral-objective").TryInteract(d.Hero,flow.Session),Is.True);Assert.That(flow.Session.State.Gold,Is.EqualTo(gold+30));Assert.That(Find("umbral-objective").TryInteract(d.Hero,flow.Session),Is.False);
            yield return DemoCapture.Save("Logs/E14/Captures/Reward.png",1280,720);
            Assert.That(d.Hero.TryMoveTo(new Vector3(71,0,-4)),Is.True);yield return Arrive(d.Hero);Assert.That(Find("umbral-exit").TryInteract(d.Hero,flow.Session),Is.True);yield return null;
            var ids=flow.Session.State.Companions.Select(c=>c.Id).ToArray();Assert.That(d.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);flow.ShowMain();Assert.That(flow.Continue(),Is.True);yield return null;yield return null;Assert.That(flow.Session.State.Companions.Select(c=>c.Id),Is.EqualTo(ids));Assert.That(party.Units.Count,Is.EqualTo(flow.Session.State.Companions.Count(c=>c.InParty&&c.Health>0)));
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        public static InteractionPoint Find(string id)=>Object.FindObjectsByType<InteractionPoint>().Single(p=>p.FactId==id);
        public static IEnumerator Arrive(ExplorationUnit u){yield return null;var until=Time.realtimeSinceStartup+25;while(u.IsMoving&&Time.realtimeSinceStartup<until)yield return null;Assert.That(u.IsMoving,Is.False,"Navigation timeout: "+u.name+" pos="+u.transform.position+" destination="+u.Agent.destination+" remaining="+u.Agent.remainingDistance+" velocity="+u.Agent.velocity+" status="+u.Agent.pathStatus);}
        public static IEnumerator Fight(TurnEncounter encounter)
        {
            var until=Time.realtimeSinceStartup+120;var traceAt=0f;
            while(encounter.GroupActive&&Time.realtimeSinceStartup<until)
            {
                var g=encounter.Group;
                if(Time.realtimeSinceStartup>=traceAt){traceAt=Time.realtimeSinceStartup+1;File.AppendAllText("Logs/E14/group-trace.txt",Time.realtimeSinceStartup+" round="+g.Round+" current="+g.Current.Title+" action="+g.ActionAvailable+" remaining="+g.MovementRemaining+" paused="+Object.FindAnyObjectByType<GameSession>().Paused+" actors="+string.Join(";",g.Members.Select(a=>a.Id+":"+a.Health+":"+encounter.GroupUnit(a.Id).transform.position+":moving="+encounter.GroupUnit(a.Id).IsMoving))+"\n");}
                if(g.Current.Hostile){yield return null;continue;}
                var u=encounter.GroupUnit(g.Current.Id);if(u.IsMoving){yield return null;continue;}
                var target=g.Members.Where(a=>a.Hostile&&a.Health>0).OrderBy(a=>Vector3.Distance(u.transform.position,encounter.GroupUnit(a.Id).transform.position)).First();encounter.SelectGroupTarget(target.Id);
                if(g.ActionAvailable)
                {
                    if(g.Current.Health<g.Current.Profile.Health*.5f&&!g.Current.Recovered)encounter.GroupRecover();
                    else{encounter.GroupApproach();yield return Arrive(u);if(!encounter.GroupAttack()&&g.ActionAvailable)encounter.GroupGuard();}
                }
                if(encounter.GroupActive)encounter.GroupEnd();yield return null;
            }
            Assert.That(encounter.GroupActive,Is.False,"Group encounter must finish");Assert.That(encounter.InGroup&&encounter.Group.Won==false,Is.False,"Real victory required, not forced stats");
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
