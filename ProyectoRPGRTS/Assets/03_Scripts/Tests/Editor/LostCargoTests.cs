using System.Collections;
using System.IO;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class LostCargoTests
    {
        public const string Demo="Assets/01_Scenes/AetherniaVillageDemo.unity";
        [SetUp]public void Open(){if(!EditorApplication.isPlaying)EditorSceneManager.OpenScene(Demo);}
        [Test]public void FactsPersistWithoutSchemaChangeAndRewardsCannotRepeat()
        {
            var rules=ScriptableObject.CreateInstance<PrototypeRules>();
            try
            {
                var state=new PrototypeState();Assert.That(LostCargoQuest.Stage(state),Is.EqualTo(CargoQuestStage.Available));
                Assert.That(PrototypeSaveStore.Save("Logs/E05-old-v1.json",state,rules),Is.True);Assert.That(PrototypeSaveStore.Load("Logs/E05-old-v1.json",rules,out state),Is.True);
                Assert.That(state.Version,Is.EqualTo(1));Assert.That(LostCargoQuest.Accept(state),Is.True);Roundtrip(state,rules,CargoQuestStage.Accepted);
                Assert.That(LostCargoQuest.Accept(state),Is.False);Assert.That(LostCargoQuest.Claim(state,out _),Is.False);
                Assert.That(LostCargoQuest.Secure(state),Is.True);Roundtrip(state,rules,CargoQuestStage.Recovered);
                var gold=state.Gold;Assert.That(LostCargoQuest.Claim(state,out var ration),Is.True);Assert.That(ration,Is.True);Assert.That(state.Gold,Is.EqualTo(gold+20));Roundtrip(state,rules,CargoQuestStage.Completed);
                Assert.That(LostCargoQuest.Claim(state,out _),Is.False);Assert.That(state.Gold,Is.EqualTo(gold+20));
                var full=new PrototypeState();full.Items[0].Count=8;LostCargoQuest.Accept(full);LostCargoQuest.Secure(full);Assert.That(LostCargoQuest.Claim(full,out ration),Is.True);Assert.That(ration,Is.False);Assert.That(full.Items[0].Count,Is.EqualTo(8));Assert.That(PrototypeSaveStore.Valid(full,rules),Is.True);
            }finally{Object.DestroyImmediate(rules);}
        }
        private static void Roundtrip(PrototypeState state,PrototypeRules rules,CargoQuestStage expected)
        {var path="Logs/E05-"+expected+".json";Assert.That(PrototypeSaveStore.Save(path,state,rules),Is.True);Assert.That(PrototypeSaveStore.Load(path,rules,out var restored),Is.True);Assert.That(LostCargoQuest.Stage(restored),Is.EqualTo(expected));}
        [UnityTest]public IEnumerator MonkCompletesQuest()=>Complete("monk");
        [UnityTest]public IEnumerator WarriorCompletesQuest()=>Complete("warrior");
        [UnityTest]public IEnumerator MageCompletesQuest()=>Complete("mage");
        private static IEnumerator Complete(string id)
        {
            yield return new EnterPlayMode();yield return null;yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var q=Object.FindAnyObjectByType<LostCargoController>();var view=q.GetComponent<LostCargoView>();var s=d.Session;var hud=Object.FindAnyObjectByType<VerticalSliceHud>();
            q.EncounterSeed=42;
            s.SelectClass(id);d.Hero.Select(true);var rig=Camera.main.GetComponent<IsometricCameraRig>();rig.FocusOn(q.Npc.transform.position);rig.ApplyInput(Vector2.zero,rig.Size-7,0,false);
            Assert.That(q.Accept(),Is.False,"Must be near NPC to accept, not remotely.");Assert.That(LostCargoQuest.Stage(s.State),Is.EqualTo(CargoQuestStage.Available));
            Assert.That(d.Hero.TryMoveTo(q.Npc.transform.position+Vector3.back*1.1f),Is.True);yield return WaitMove(d.Hero);
            Assert.That(q.Npc.TryInteract(d.Hero,s),Is.True);Assert.That(view.DialogueOpen,Is.True);
            if(id=="monk")yield return Pair("NPC",MedievalHudTests.Layout);
            view.AcceptButton.onClick.Invoke();Assert.That(LostCargoQuest.Stage(s.State),Is.EqualTo(CargoQuestStage.Accepted));Assert.That(view.DialogueOpen,Is.False);
            Assert.That(d.Save("Logs/E05-accepted-"+id+".json"),Is.True);Assert.That(d.Load("Logs/E05-accepted-"+id+".json"),Is.True);
            Assert.That(d.Hero.TryMoveTo(q.Cargo.transform.position+Vector3.back*1.6f),Is.True);yield return WaitMove(d.Hero);rig.FocusOn(d.Hero.transform.position);
            if(id=="monk")yield return Pair("Active",MedievalHudTests.Layout);
            Assert.That(q.Cargo.TryInteract(d.Hero,s),Is.True);Assert.That(d.Encounter.Battle,Is.Not.Null);Assert.That(d.Session.Mode,Is.EqualTo(PrototypeMode.TurnCombat));Assert.That(q.CargoEncounter,Is.True);
            if(id=="monk")yield return Pair("Encounter",MedievalHudTests.Layout);
            d.Encounter.Approach();yield return WaitMove(d.Hero);
            for(var turn=0;turn<30&&q.CargoEncounter;turn++)
            {
                if(s.State.Health<8&&!d.Encounter.Battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                if(q.CargoEncounter&&d.Encounter.Battle!=null&&!d.Encounter.Battle.Finished)d.Encounter.EndTurn();yield return null;
            }
            Assert.That(LostCargoQuest.Stage(s.State),Is.EqualTo(CargoQuestStage.Recovered),"Actual RPG victory required with "+id);Assert.That(s.Mode,Is.EqualTo(PrototypeMode.Exploration));Assert.That(d.Encounter.Battle,Is.Null);
            Assert.That(d.Save("Logs/E05-recovered-"+id+".json"),Is.True);Assert.That(d.Load("Logs/E05-recovered-"+id+".json"),Is.True);
            Assert.That(d.Hero.TryMoveTo(q.Npc.transform.position+Vector3.back*1.1f),Is.True);yield return WaitMove(d.Hero);rig.FocusOn(q.Npc.transform.position);
            Assert.That(q.Npc.TryInteract(d.Hero,s),Is.True);var gold=s.State.Gold;var ration=s.State.Items.Find(i=>i.Id=="ration").Count;view.ClaimButton.onClick.Invoke();
            Assert.That(s.State.Gold,Is.EqualTo(gold+20));Assert.That(s.State.Items.Find(i=>i.Id=="ration").Count,Is.EqualTo(ration+1));Assert.That(q.Claim(),Is.False);Assert.That(LostCargoQuest.Stage(s.State),Is.EqualTo(CargoQuestStage.Completed));
            if(id=="monk")yield return Pair("Completed",MedievalHudTests.Layout);
            view.CloseDialogue();Assert.That(d.Save("Logs/E05-complete-"+id+".json"),Is.True,PrototypeSaveStore.LastWriteFailure);Assert.That(d.Load("Logs/E05-complete-"+id+".json"),Is.True);Assert.That(LostCargoQuest.Stage(s.State),Is.EqualTo(CargoQuestStage.Completed));Assert.That(q.Claim(),Is.False);
            Assert.That(d.Hero.GetComponent<CharacterVisualPresenter>().DisplayedClass,Is.EqualTo(id));File.WriteAllText("Logs/E05-route-"+id+".txt","PASS: NPC distance, real navigation, dialogue acceptance, v1 stage save/load, real RPG victory, exploration return, unique reward and completion restore.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest]public IEnumerator UnrelatedVictoryAndCargoBeforeAcceptanceCannotCompleteMission()
        {
            yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var q=Object.FindAnyObjectByType<LostCargoController>();var s=d.Session;
            Assert.That(d.Hero.Warp(q.Cargo.transform.position+Vector3.back*1.6f),Is.True);Assert.That(q.Cargo.TryInteract(d.Hero,s),Is.False);Assert.That(d.Encounter.Battle,Is.Null);
            Assert.That(d.Hero.Warp(q.Npc.transform.position+Vector3.back*1.1f),Is.True);Assert.That(q.Accept(),Is.True);
            Assert.That(d.Hero.Warp(new Vector3(-6,0,-6)),Is.True);Assert.That(d.Encounter.Begin(42),Is.True);Assert.That(d.Hero.Warp(d.Encounter.EnemyPosition+Vector3.right*1.3f),Is.True);
            for(var i=0;i<25&&!d.Encounter.Battle.Finished;i++){d.Encounter.Attack();if(!d.Encounter.Battle.Finished)d.Encounter.EndTurn();yield return null;}
            Assert.That(d.Encounter.Battle.Won,Is.True);Assert.That(LostCargoQuest.Stage(s.State),Is.EqualTo(CargoQuestStage.Accepted));Assert.That(q.Claim(),Is.False);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator WaitMove(ExplorationUnit hero)
        {var limit=Time.realtimeSinceStartup+15;yield return null;while(hero.IsMoving&&Time.realtimeSinceStartup<limit)yield return null;Assert.That(hero.IsMoving,Is.False,"Movement timed out");}
        private static IEnumerator Pair(string label,System.Action<int,int> check)
        {foreach(var width in new[]{1280,1920})yield return SceneCapture.Save("Logs/E05-"+label+"-"+width+".png",width,width*9/16,check);}
        [UnityTearDown]public IEnumerator End(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
