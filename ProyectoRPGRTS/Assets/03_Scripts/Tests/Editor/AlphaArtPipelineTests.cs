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
using Object=UnityEngine.Object;
namespace Aethernia.Tests.Editor
{
    public sealed class AlphaArtPipelineTests
    {
        [Test]public void ReviewedCatalogHasNoExperimentalDependencies()
        {
            var p=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AlphaPipelineBuilder.CatalogPath);Assert.That(p,Is.Not.Null);Assert.That(p.Entries.Length,Is.EqualTo(3));
            foreach(var path in AssetDatabase.GetDependencies(AlphaPipelineBuilder.CatalogPath,true))Assert.That(path.Contains("MonkMeshy")||path.Contains("LocalOnly"),Is.False,path);
            foreach(var entry in p.Entries)Assert.That(CharacterVisualContract.CanUse(p.Find(entry.ClassId),true,out var reason,true),Is.True,reason);
        }
        [Test]public void InvalidOrDisabledReplacementsUseExistingClassVisuals()
        {
            var catalog=Object.Instantiate(AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AlphaPipelineBuilder.CatalogPath));var candidate=GameObject.CreatePrimitive(PrimitiveType.Cube);candidate.name="Local test visual";candidate.SetActive(false);candidate.AddComponent<CharacterVisualModel>();var collider=candidate.GetComponent<Collider>();
            try
            {
                foreach(var e in catalog.Entries)
                {
                    e.ReplacementPrefab=candidate;e.UseReplacement=true;e.ReplacementRequiresAnimation=false;
                    Assert.That(catalog.Find(e.ClassId),Is.Not.SameAs(candidate),"Collider-bearing visual rejected");
                }
                Object.DestroyImmediate(collider);
                foreach(var e in catalog.Entries)
                {
                    e.UseReplacement=false;Assert.That(catalog.Find(e.ClassId),Is.Not.SameAs(candidate));e.UseReplacement=true;
                    Assert.That(CharacterVisualFactory.TryCreate(catalog,e.ClassId,null,out var model,out var source,out _),Is.True);Assert.That(source,Is.SameAs(candidate));Object.DestroyImmediate(model.gameObject);
                    e.ReplacementRequiresAnimation=true;Assert.That(catalog.Find(e.ClassId),Is.Not.SameAs(candidate),"Static mesh cannot claim existing animations");
                    e.ReplacementPrefab=null;Assert.That(catalog.Find(e.ClassId),Is.Not.Null,"Missing asset fallback");
                }
            }
            finally{Object.DestroyImmediate(candidate);Object.DestroyImmediate(catalog);}
        }
        [UnityTest]public IEnumerator ReplacementPreservesHeroStateMovementAndSave()=>Gameplay();
        private static IEnumerator Gameplay()
        {
            EditorSceneManager.OpenScene(VisualPolishBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            var flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E17/pipeline-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;
            Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/warrior.json",flow.Session.Rules,out var baseline),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,baseline,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);yield return null;
            var hero=flow.Director.Hero;var actor=hero.GetComponent<CharacterVisualPresenter>();var catalog=Object.Instantiate(AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>(AlphaPipelineBuilder.CatalogPath));var bad=GameObject.CreatePrimitive(PrimitiveType.Cube);bad.SetActive(false);bad.AddComponent<CharacterVisualModel>();
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                flow.Session.SelectClass(id);flow.Session.Paused=true;var state=JsonUtility.ToJson(flow.Session.State);var e=catalog.FindEntry(id);e.UseReplacement=true;e.ReplacementPrefab=bad;
                actor.Configure(flow.Session,catalog,actor.VisualRoot,actor.LegacyBody);Assert.That(actor.Model,Is.Not.Null);Assert.That(actor.Model.GetComponent<HumanoidAnimationDriver>().CanAnimate,Is.True);Assert.That(JsonUtility.ToJson(flow.Session.State),Is.EqualTo(state));
                Assert.That(hero.GetComponentsInChildren<UnityEngine.AI.NavMeshAgent>(true).Length,Is.EqualTo(1));Assert.That(hero.GetComponentsInChildren<ExplorationUnit>(true).Length,Is.EqualTo(1));
                e.ReplacementPrefab=e.HumanoidPrefab;actor.Configure(flow.Session,catalog,actor.VisualRoot,actor.LegacyBody);Assert.That(JsonUtility.ToJson(flow.Session.State),Is.EqualTo(state));
                var current=actor.Model;var originalPrefab=e.Prefab;var originalHumanoid=e.HumanoidPrefab;var previous=e.PreviousPrefab;
                e.ReplacementPrefab=null;e.HumanoidPrefab=null;e.Prefab=null;e.PreviousPrefab=null;
                actor.Configure(flow.Session,catalog,actor.VisualRoot,actor.LegacyBody);Assert.That(actor.Model,Is.SameAs(current));Assert.That(actor.VisualRoot.gameObject.activeSelf,Is.True,"Loaded approved model survives missing catalog assets");Assert.That(JsonUtility.ToJson(flow.Session.State),Is.EqualTo(state));
                e.Prefab=originalPrefab;e.HumanoidPrefab=originalHumanoid;e.PreviousPrefab=previous;e.ReplacementPrefab=originalHumanoid;

                flow.Session.Paused=false;var start=hero.transform.position;Assert.That(hero.TryMoveTo(start+Vector3.right*.8f),Is.True);yield return ExpeditionIntegrationTests.Arrive(hero);Assert.That(Vector3.Distance(start,hero.transform.position),Is.GreaterThan(.3f));
                Assert.That(flow.Director.Save(flow.SlotPath),Is.True);Assert.That(flow.Continue(),Is.True);Assert.That(flow.Director.Hero,Is.SameAs(hero));Assert.That(flow.Session.State.ClassId,Is.EqualTo(id));
            }
            Object.Destroy(bad);Object.Destroy(catalog);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest]public IEnumerator LocalMeshyReviewCanRenderWithoutRuntimeBinding()=>LocalPreview();
        private static IEnumerator LocalPreview()
        {
            if(!File.Exists(AlphaPipelineBuilder.PrivateScene))Assert.Ignore("Experimental art absent; local review only.");
            EditorSceneManager.OpenScene(AlphaPipelineBuilder.PrivateScene);yield return new EnterPlayMode();yield return null;yield return null;Directory.CreateDirectory("Logs/E17/Private");
            Assert.That(Object.FindAnyObjectByType<GameSession>(),Is.Null);Assert.That(Object.FindAnyObjectByType<ExplorationUnit>(),Is.Null);
            yield return DemoCapture.Save("Logs/E17/Private/Meshy-normalized.png",1280,720);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
