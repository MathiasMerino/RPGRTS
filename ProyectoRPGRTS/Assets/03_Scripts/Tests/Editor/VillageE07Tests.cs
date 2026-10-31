using System.Collections;
using System.IO;
using System.Text;
using Aethernia;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class VillageE07Tests
    {
        public const string RootName="E07 Medieval Village";
        [UnityTest]public IEnumerator ComparableViewsAndMissionUseRealGameCamera()
        {
            EditorSceneManager.OpenScene(LostCargoTests.Demo);
            if(GameObject.Find(RootName)==null)
            {
                File.WriteAllText("Logs/E07-physics-before.txt",EnvironmentPhysicsSnapshot.Capture(UnityEngine.SceneManagement.SceneManager.GetActiveScene()));
                File.WriteAllText("Logs/E07-nav-before.txt",MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath));
                var inventory=new StringBuilder();
                foreach(var c in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))inventory.AppendLine(c.name+" | "+c.bounds.center+" | "+c.bounds.size);
                File.WriteAllText("Logs/E07-colliders-before.txt",inventory.ToString());
            }
            yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var q=Object.FindAnyObjectByType<LostCargoController>();
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var canvas=hud.GetComponent<Canvas>();
            var label=GameObject.Find(RootName)==null?"Before":"After";Directory.CreateDirectory("Logs/E07-"+label);
            Camera.main.GetComponent<IsometricCameraRig>().enabled=false;canvas.enabled=false;d.Session.SelectClass("monk");
            yield return Shot(label,"01-Overview",new Vector3(-1,0,2),18);
            yield return Shot(label,"02-Plaza",new Vector3(-3,0,-3),8);
            yield return Shot(label,"03-Architecture",new Vector3(6,1,6),8.5f);
            yield return Shot(label,"04-Forest",new Vector3(-12,0,6),8);
            Assert.That(d.Hero.Warp(new Vector3(-4,0,-4)),Is.True);d.Hero.Select(true);
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                d.Session.SelectClass(id);yield return null;
                yield return Shot(label,"05-Class-"+id,new Vector3(-4,.7f,-4),4.5f);
                Assert.That(d.Hero.GetComponent<CharacterVisualPresenter>().DisplayedClass,Is.EqualTo(id));
            }
            d.Session.SelectClass("monk");Assert.That(d.Hero.Warp(q.Npc.transform.position+Vector3.back*1.1f),Is.True);
            yield return Shot(label,"06-Npc-area",q.Npc.transform.position,7);
            canvas.enabled=true;Assert.That(q.Npc.TryInteract(d.Hero,d.Session),Is.True);
            yield return Shot(label,"07-Dialogue",q.Npc.transform.position,7);
            q.GetComponent<LostCargoView>().AcceptButton.onClick.Invoke();Assert.That(LostCargoQuest.Stage(d.Session.State),Is.EqualTo(CargoQuestStage.Accepted));
            q.EncounterSeed=42;Assert.That(d.Hero.TryMoveTo(q.Cargo.transform.position+Vector3.back*1.6f),Is.True);
            yield return Arrive(d.Hero);yield return Shot(label,"08-Quest-route",q.Cargo.transform.position,8);
            Assert.That(q.Cargo.TryInteract(d.Hero,d.Session),Is.True);Assert.That(d.Session.Mode,Is.EqualTo(PrototypeMode.TurnCombat));
            yield return Shot(label,"09-Rpg",q.Cargo.transform.position,7);
            d.Encounter.Approach();yield return Arrive(d.Hero);
            for(var turn=0;turn<30&&q.CargoEncounter;turn++)
            {
                if(d.Session.State.Health<8&&!d.Encounter.Battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                if(q.CargoEncounter&&d.Encounter.Battle!=null&&!d.Encounter.Battle.Finished)d.Encounter.EndTurn();yield return null;
            }
            Assert.That(LostCargoQuest.Stage(d.Session.State),Is.EqualTo(CargoQuestStage.Recovered));hud.SelectPage(ExplorationPage.Character);
            yield return Shot(label,"10-Hud",new Vector3(-3,0,-3),10);
            File.WriteAllText("Logs/E07-"+label+"/capture-receipt.txt","PASS: native Unity Game camera, same 35.26439/45 angle and explicit focus/orthographic size, 1920x1080; 12 views including three classes, actual NPC acceptance and real RPG start. Temporary framing only, not saved to scene.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator Shot(string label,string name,Vector3 focus,float size)
        {
            var c=Camera.main;c.transform.rotation=Quaternion.Euler(35.26439f,45,0);c.orthographicSize=size;c.transform.position=focus-c.transform.forward*45;
            yield return SceneCapture.Save("Logs/E07-"+label+"/"+name+".png",1920,1080);
        }
        private static IEnumerator Arrive(ExplorationUnit hero)
        {yield return null;var until=Time.realtimeSinceStartup+15;while(hero.IsMoving&&Time.realtimeSinceStartup<until)yield return null;Assert.That(hero.IsMoving,Is.False,"E07 route timed out");}
        [UnityTearDown]public IEnumerator End(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
