using System;
using System.Collections;
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
    public sealed class CampaignIntegrationTests
    {
        [UnityTest,Timeout(600000)]public IEnumerator DefeatKeepsEnemyOwnershipAndAllowsPaidTimeRecovery()=>Defeat();
        private static IEnumerator Defeat()
        {
            EditorSceneManager.OpenScene(ExpeditionBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var party=Object.FindAnyObjectByType<PartyWorld>();var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();var territory=Object.FindAnyObjectByType<TerritoryCampaign>();
            // Fixture de derrota explícita: héroe/recluta heridos, perfiles de combate sin cambiar.
            var state=ExpeditionRulesTests.Ready();state.Health=1;state.Army.Add(new RecruitRecord{Id=Guid.NewGuid().ToString("N"),Role="swordsman",Health=1});state.ArmySelection.Add(state.Army[0].Id);
            ExpeditionOperations.Recruit(state,party.Rules,party.Roles,"archer");ExpeditionOperations.InitializeTerritories(state,party.Rules);ExpeditionOperations.ClaimNeutral(state,party.Rules);
            state.HeroPosition=new Vector3(-12,0,-11.5f);flow.SlotPath="Logs/E14/defeat-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;
            Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;yield return null;
            Assert.That(territory.BeginConquest(),Is.True);var clock=flow.Session.State.WorldSeconds;var deadline=Time.realtimeSinceStartup+80;
            while(flow.Director.Rts.Active&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(flow.Director.Rts.Active,Is.False,"Real enemy attacks must resolve defeat");Assert.That(flow.Session.State.Health,Is.Zero);Assert.That(flow.Session.State.WorldSeconds,Is.EqualTo(clock).Within(.1));
            Assert.That(flow.Session.State.Territories.Single(t=>t.Id=="outpost").Owner,Is.EqualTo("faction-b"));Assert.That(flow.Session.State.Facts,Does.Not.Contain("umbral-outpost-conquered"));Assert.That(flow.Session.State.Army,Is.Empty);Assert.That(flow.Session.State.ArmySelection,Is.Empty);
            Assert.That(flow.Session.State.Companions.Single().Health,Is.EqualTo(10));Assert.That(party.Units.Count,Is.EqualTo(1));
            Assert.That(dungeon.Rest(),Is.True);while(dungeon.Resting)yield return null;Assert.That(flow.Session.State.Health,Is.EqualTo(4));
            Assert.That(dungeon.Rest(),Is.True);Assert.That(flow.Director.Save(flow.SlotPath),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;yield return null;Assert.That(dungeon.Resting,Is.False,"Loading cancels a rest from the previous state");Assert.That(flow.Session.State.Health,Is.EqualTo(4));
            Assert.That(flow.Director.Hero.TryMoveTo(new Vector3(10,0,-10)),Is.True);yield return ExpeditionIntegrationTests.Arrive(flow.Director.Hero);
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest,Timeout(600000)]public IEnumerator MageWithTwoCompanionsCanWinWithoutFullParty()=>MageParty();
        private static IEnumerator MageParty()
        {
            EditorSceneManager.OpenScene(ExpeditionBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();var party=Object.FindAnyObjectByType<PartyWorld>();var dungeon=Object.FindAnyObjectByType<DungeonExpedition>();var state=ExpeditionRulesTests.Ready("mage");
            ExpeditionOperations.Recruit(state,party.Rules,party.Roles,"swordsman");ExpeditionOperations.Recruit(state,party.Rules,party.Roles,"archer");state.HeroPosition=new Vector3(76,0,-2);
            flow.SlotPath="Logs/E14/mage-two-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;dungeon.TestSeed=42;
            Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;yield return null;
            Assert.That(ExpeditionIntegrationTests.Find("umbral-battle-1").TryInteract(flow.Director.Hero,flow.Session),Is.True);Assert.That(flow.Director.Encounter.Group.Members.Count(a=>!a.Hostile),Is.EqualTo(3));
            yield return ExpeditionIntegrationTests.Fight(flow.Director.Encounter);Assert.That(flow.Session.HasFact("umbral-battle-1"),Is.True);Assert.That(flow.Session.State.Health,Is.GreaterThan(0));
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
