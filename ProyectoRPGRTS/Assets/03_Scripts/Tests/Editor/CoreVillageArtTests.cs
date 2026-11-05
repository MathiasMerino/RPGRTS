using System;
using System.Collections;
using System.Linq;
using Aethernia.Editor.Characters;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class CoreVillageArtTests
    {
        [Test]
        public void CoreKeepsNavigationPhysicsAndOneBootstrap()
        {
            EditorSceneManager.OpenScene(FrontierWorldArt.Candidate);
            var physics=VisualPolishTests.PhysicsSignature();
            var navigation=Object.FindObjectsByType<NavMeshSurface>().Select(x=>x.navMeshData).ToArray();
            var interactions=InteractionLayout();
            EditorSceneManager.OpenScene(WorldExplorationBuilder.CoreRefineScenePath);
            Assert.That(VisualPolishTests.PhysicsSignature(),Is.EqualTo(physics));
            Assert.That(Object.FindObjectsByType<NavMeshSurface>().Select(x=>x.navMeshData),Is.EquivalentTo(navigation));
            Assert.That(InteractionLayout(),Is.EqualTo(interactions));
            Assert.That(Object.FindObjectsByType<DemoFlow>().Length,Is.EqualTo(1));
            Assert.That(Object.FindObjectsByType<PrototypeDirector>().Length,Is.EqualTo(1));
            Assert.That(Object.FindObjectsByType<IsometricCameraRig>().Length,Is.EqualTo(1));
            var dependencies=AssetDatabase.GetDependencies(WorldExplorationBuilder.CoreRefineScenePath);
            Assert.That(dependencies.Count(path=>path.StartsWith(CoreVillageArt.Library+"/Prefabs/")),Is.EqualTo(2));
            Assert.That(dependencies.Any(path=>path.Contains("LocalOnly")||path.Contains("Meshy")),Is.False);
        }

        private static (string id,Vector3 at,float reach)[] InteractionLayout() =>
            Object.FindObjectsByType<InteractionPoint>(FindObjectsInactive.Include)
                .Select(p=>(p.FactId,p.transform.position,p.Reach)).OrderBy(p=>p.FactId).ThenBy(p=>p.position.x).ToArray();

        [UnityTest] public IEnumerator ArtBaselineFromSameCamera()
        {
            EditorSceneManager.OpenScene(FrontierWorldArt.Candidate);
            yield return new EnterPlayMode();
            yield return Capture("ART-Posada");
        }

        [UnityTest] public IEnumerator CoreEntranceFromSameCamera()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.CoreRefineScenePath);
            yield return new EnterPlayMode();
            yield return Capture("CORE-Posada");
        }

        private static IEnumerator Capture(string name)
        {
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            flow.SlotPath="Logs/COREREFINE01/art-"+Guid.NewGuid().ToString("N")+".json";
            flow.Director.SavePathOverride=flow.SlotPath;
            yield return null;
            Assert.That(flow.StartNew("Viajero","warrior","human"),Is.True);
            flow.CompleteIntro();
            var rig=flow.CameraRig;
            rig.FollowTarget=null;
            rig.SetZoom(8.5f);
            rig.FocusOn(new Vector3(.1f,0,7.4f));
            yield return UxExperienceRoute.Wait(.8f);
            yield return DemoPlayerCapture.Save("Logs/COREREFINE01/Captures/"+name+".png");
        }

        [UnityTearDown] public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
