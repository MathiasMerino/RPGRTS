using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;
namespace Aethernia.Tests.Editor
{
    public sealed class UxSurfaceDiagnosticTests
    {
        [Test]public void DecorativeLandscapeDoesNotCoverMilitaryUnitsOrConnectingPath()
        {
            EditorSceneManager.OpenScene(UxExperienceBuilder.ScenePath);
            var expected=AssetDatabase.LoadAssetAtPath<Mesh>(UxExperienceBuilder.Root+"/WesternGround.asset");Assert.That(expected,Is.Not.Null);
            var surfaces=Object.FindObjectsByType<MeshFilter>().Where(f=>f.name=="Grass"&&f.GetComponent<Renderer>()!=null&&f.GetComponent<Renderer>().bounds.size.x>60).ToArray();Assert.That(surfaces.Length,Is.GreaterThanOrEqualTo(2));
            foreach(var landscape in surfaces)
            {
            Assert.That(AssetDatabase.GetAssetPath(landscape.sharedMesh),Does.StartWith(UxExperienceBuilder.Root+"/WesternGround"));expected=landscape.sharedMesh;
            var vertices=expected.vertices.Select(landscape.transform.TransformPoint).ToArray();
            var arena=vertices.Where(v=>v.x>=-54&&v.x<=-21&&v.z>=-12&&v.z<=15).ToArray();
            Assert.That(arena.Length,Is.GreaterThan(100));Assert.That(arena.Max(v=>v.y),Is.LessThanOrEqualTo(.01f),"Decorative hills must remain below physical flat ground.");
            var road=vertices.Where(v=>v.x>=-49&&v.x<=-11&&v.z>=0&&v.z<=3).ToArray();
            Assert.That(road.Length,Is.GreaterThan(10));Assert.That(road.Max(v=>v.y),Is.LessThanOrEqualTo(.01f));
            Assert.That(vertices.Any(v=>v.y>.5f),Is.True,"Surrounding landscape remains varied; original mesh not flattened globally.");
            }
        }
    }
}
