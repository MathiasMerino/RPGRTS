using System;
using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class FrontierWorldArtTests
    {
        [Test]
        public void CandidateKeepsNavigationAndPublicGameplayReferences()
        {
            EditorSceneManager.OpenScene(FrontierWorldArt.Source);
            var physics=VisualPolishTests.PhysicsSignature();
            var nav=Object.FindObjectsByType<NavMeshSurface>().Select(s=>s.navMeshData).ToArray();
            var rules=Object.FindAnyObjectByType<DemoFlow>().Session.Rules;
            var targets=Object.FindObjectsByType<InteractionPoint>(FindObjectsInactive.Include)
                .Select(p=>(p.FactId,p.transform.position,p.Reach)).OrderBy(p=>p.FactId).ThenBy(p=>p.position.x).ToArray();
            EditorSceneManager.OpenScene(FrontierWorldArt.Candidate);
            Assert.That(VisualPolishTests.PhysicsSignature(),Is.EqualTo(physics));
            Assert.That(Object.FindObjectsByType<NavMeshSurface>().Select(s=>s.navMeshData),Is.EquivalentTo(nav));
            Assert.That(Object.FindAnyObjectByType<DemoFlow>().Session.Rules,Is.SameAs(rules));
            Assert.That(Object.FindObjectsByType<InteractionPoint>(FindObjectsInactive.Include)
                .Select(p=>(p.FactId,p.transform.position,p.Reach)).OrderBy(p=>p.FactId).ThenBy(p=>p.position.x),Is.EqualTo(targets));
            Assert.That(AssetDatabase.GetDependencies(FrontierWorldArt.Candidate,true)
                .Any(path=>path.Contains("LocalOnly")||path.Contains("Meshy")),Is.False);
            Assert.That(AssetDatabase.FindAssets("t:Prefab",new[]{FrontierWorldArt.Library+"/Prefabs"}).Length,Is.GreaterThanOrEqualTo(14));
            var meshes=AssetDatabase.FindAssets("t:Mesh",new[]{FrontierWorldArt.Library+"/Meshes"})
                .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Mesh>)
                .Where(mesh=>mesh!=null).OrderBy(mesh=>mesh.name).ToArray();
            Directory.CreateDirectory("Logs/ARTENV01");
            File.WriteAllLines("Logs/ARTENV01/mesh-counts.csv",new[]{"mesh,vertices,triangles,submeshes"}.Concat(
                meshes.Select(mesh=>mesh.name+","+mesh.vertexCount+","+
                    Enumerable.Range(0,mesh.subMeshCount).Sum(i=>(long)mesh.GetIndexCount(i)/3)+","+mesh.subMeshCount)));
        }

        [UnityTest] public IEnumerator BaselineCapturedInUnity()
        {
            EditorSceneManager.OpenScene(FrontierWorldArt.Source);
            yield return new EnterPlayMode();
            yield return Capture("E23",false);
        }

        [UnityTest] public IEnumerator CandidateCapturedAndVillageRoutesReachable()
        {
            EditorSceneManager.OpenScene(FrontierWorldArt.Candidate);
            yield return new EnterPlayMode();
            yield return Capture("ART",true);
        }

        [UnityTest] public IEnumerator CulturalStudyCapturedInUnity()
        {
            EditorSceneManager.OpenScene(FrontierWorldArt.Study);
            yield return new EnterPlayMode();
            yield return UxExperienceRoute.Wait(.4f);
            yield return DemoPlayerCapture.Save("Logs/ARTENV01/Captures/ART-08-Kits.png");
        }

        private static IEnumerator Capture(string phase,bool candidate)
        {
            const string output="Logs/ARTENV01/Captures/";
            Directory.CreateDirectory(output);
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            flow.SlotPath="Logs/ARTENV01/art-"+Guid.NewGuid().ToString("N")+".json";
            flow.Director.SavePathOverride=flow.SlotPath;
            yield return null;
            Assert.That(flow.StartNew("Viajero","warrior","human"),Is.True);
            flow.CompleteIntro();
            yield return UxExperienceRoute.Wait(.7f);
            if(candidate)
            {
                var hero=flow.Director.Hero;
                var market=Object.FindObjectsByType<InteractionPoint>().Single(p=>p.FactId==FrontierChapter.Merchant);
                var resident=Object.FindAnyObjectByType<LostCargoController>().Npc;
                var path=new NavMeshPath();
                foreach(var point in new[]{resident,market})
                {
                    Assert.That(NavMesh.SamplePosition(point.transform.position+Vector3.back*1.1f,out var destination,.65f,NavMesh.AllAreas),Is.True);
                    Assert.That(NavMesh.CalculatePath(hero.transform.position,destination.position,NavMesh.AllAreas,path),Is.True);
                    Assert.That(path.status,Is.EqualTo(NavMeshPathStatus.PathComplete));
                }
            }
            var rig=flow.CameraRig;
            rig.FollowTarget=null;
            rig.SetZoom(9);
            foreach(var frame in new[]{
                ("01-Aldea",new Vector3(-3,0,1)),("02-Casas",new Vector3(3,0,7)),
                ("03-Mercado",new Vector3(.1f,0,-6.5f)),("04-Bosque",new Vector3(-23,0,-4.6f)),
                ("05-Ruinas",new Vector3(-16,0,-16)),("06-Campamento",new Vector3(-45,0,-4)),
                ("07-Puesto",new Vector3(-28,0,9))})
            {
                rig.FocusOn(frame.Item2);
                yield return UxExperienceRoute.Wait(.65f);
                yield return DemoPlayerCapture.Save(output+phase+"-"+frame.Item1+".png");
            }
        }

        [UnityTearDown] public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
