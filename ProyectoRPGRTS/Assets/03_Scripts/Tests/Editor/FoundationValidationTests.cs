using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class FoundationValidationTests
    {
        private const string Sandbox = "Assets/01_Scenes/TechnicalSandbox.unity";

        [SetUp]
        public void OpenSandbox()
        {
            Assert.That(File.Exists(Sandbox), Is.True, "Generate the sandbox before running Foundation tests.");
            // The runner invokes SetUp again after EnterPlayMode reloads the domain.
            if (EditorApplication.isPlaying)
                Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(Sandbox));
            else
                EditorSceneManager.OpenScene(Sandbox, OpenSceneMode.Single);
        }

        [TestCase("ReferenceCube_1m", 1, 1, 1)]
        [TestCase("ReferenceHuman_180cm", 0.6f, 1.8f, 0.6f)]
        [TestCase("ReferenceDoor_120x220cm", 1.6f, 2.4f, 0.25f)]
        [TestCase("ReferenceWindow_100x120cm", 1.4f, 1.6f, 0.25f)]
        [TestCase("ReferenceHouse_6x8m", 6, 3.15f, 8)]
        [TestCase("ReferenceGround_40m", 40, 0.2f, 40)]
        public void ImportedPrefabsHaveMetricDimensions(string name, float width, float height, float depth)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/ScaleReferences/" + name + ".prefab");
            Assert.That(prefab, Is.Not.Null);
            Assert.That(prefab.transform.localScale, Is.EqualTo(Vector3.one));
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var renderers = instance.GetComponentsInChildren<Renderer>();
                Assert.That(renderers.Length, Is.GreaterThan(0));
                var bounds = renderers[0].bounds;
                foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                Assert.That(bounds.size.x, Is.EqualTo(width).Within(0.005f), name + " width");
                Assert.That(bounds.size.y, Is.EqualTo(height).Within(0.005f), name + " height");
                Assert.That(bounds.size.z, Is.EqualTo(depth).Within(0.005f), name + " depth");
            }
            finally { Object.DestroyImmediate(instance); }
        }

        [Test]
        public void HumanCapsuleFitsDoorButCannotFitNarrowGate()
        {
            Physics.SyncTransforms();
            Assert.That(CapsuleHits(new Vector3(-2, 0, -3)), Is.False, "The 1.20m door must be clear.");
            Assert.That(CapsuleHits(new Vector3(-6, 0, -9)), Is.True, "The 0.40m gap must block the 0.60m capsule.");
            Assert.That(CapsuleHits(new Vector3(6, 0, 2.125f)), Is.False, "The house doorway must also be clear.");
            var human = GameObject.Find("ReferenceHuman_180cm").GetComponentInChildren<CapsuleCollider>();
            Assert.That(human.bounds.size.x, Is.EqualTo(0.6f).Within(0.005f));
            Assert.That(human.bounds.size.y, Is.EqualTo(1.8f).Within(0.005f));
            Assert.That(human.bounds.min.y, Is.EqualTo(0).Within(0.005f));
        }

        [Test]
        public void SandboxHasValidReferencesMaterialsAndGround()
        {
            EnvironmentValidationTests.AssertSceneReferences(SceneManager.GetActiveScene());
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
            foreach (var material in renderer.sharedMaterials)
            {
                Assert.That(material, Is.Not.Null, renderer.name);
                Assert.That(material.shader.name, Is.EqualTo("Universal Render Pipeline/Lit"), renderer.name);
                Assert.That(material.shader.isSupported, Is.True, renderer.name);
            }
            Physics.SyncTransforms();
            Assert.That(Physics.Raycast(new Vector3(15, 5, 15), Vector3.down, out var hit, 10), Is.True);
            Assert.That(hit.point.y, Is.EqualTo(0).Within(0.005f));
        }

        [UnityTest]
        public IEnumerator SandboxRunsAndRendersWithoutErrors()
        {
            Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null));
            yield return new EnterPlayMode();
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(Sandbox));
            var camera = Camera.main;
            Assert.That(camera, Is.Not.Null);
            var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32);
            var previousTarget = camera.targetTexture;
            Texture2D capture = null;
            try
            {
                camera.targetTexture = target;
                yield return null;
                yield return null;
                yield return null;
                capture = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                var previousActive = RenderTexture.active;
                try
                {
                    RenderTexture.active = target;
                    capture.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
                    capture.Apply();
                }
                finally { RenderTexture.active = previousActive; }
                Directory.CreateDirectory("Logs");
                File.WriteAllBytes("Logs/Foundation-preview.png", capture.EncodeToPNG());
                var pixels = capture.GetPixels();
                var min = Vector3.one;
                var max = Vector3.zero;
                for (var i = 0; i < pixels.Length; i += 31)
                {
                    var colour = new Vector3(pixels[i].r, pixels[i].g, pixels[i].b);
                    min = Vector3.Min(min, colour);
                    max = Vector3.Max(max, colour);
                }
                Assert.That((max - min).magnitude, Is.GreaterThan(0.25f), "The rendered image must not be blank.");
                LogAssert.NoUnexpectedReceived();
            }
            finally
            {
                camera.targetTexture = previousTarget;
                if (capture != null) Object.DestroyImmediate(capture);
                RenderTexture.ReleaseTemporary(target);
            }
            yield return new ExitPlayMode();
            Assert.That(EditorApplication.isPlaying, Is.False);
        }

        [UnityTearDown]
        public IEnumerator LeavePlayModeAfterFailure()
        {
            if (EditorApplication.isPlaying) yield return new ExitPlayMode();
        }

        private static bool CapsuleHits(Vector3 feet)
        {
            return Physics.CheckCapsule(feet + new Vector3(0, 0.32f, 0),
                feet + new Vector3(0, 1.52f, 0), 0.3f, ~0, QueryTriggerInteraction.Ignore);
        }
    }
}
