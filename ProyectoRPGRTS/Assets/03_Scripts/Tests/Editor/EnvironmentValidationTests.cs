using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Aethernia.Tests.Editor
{
    public class EnvironmentValidationTests
    {
        private const string SampleScene = "Assets/01_Scenes/SampleScene.unity";

        [Test]
        public void ExactEditorAndDeclaredPackagesAreAvailable()
        {
            Assert.That(Application.unityVersion, Is.EqualTo("6000.5.8f1"));
            Assert.That(EditorUtility.scriptCompilationFailed, Is.False);
            Assert.That(GraphicsSettings.defaultRenderPipeline, Is.Not.Null);
            var packages = PackageInfo.GetAllRegisteredPackages().ToDictionary(p => p.name, p => p.version);
            var manifest = File.ReadAllText("Packages/manifest.json");
            var declarations = Regex.Matches(manifest, "\"(com\\.unity[^\"]+)\"\\s*:\\s*\"([^\"]+)\"");
            Assert.That(declarations.Count, Is.GreaterThan(0));
            foreach (Match declaration in declarations)
            {
                var name = declaration.Groups[1].Value;
                Assert.That(packages.ContainsKey(name), Is.True, "Package not registered: " + name);
                Assert.That(packages[name], Is.EqualTo(declaration.Groups[2].Value), name);
            }
        }

        [Test]
        public void OriginalSceneHasNoMissingScriptsOrSerializedReferences()
        {
            var scene = EditorSceneManager.OpenScene(SampleScene, OpenSceneMode.Single);
            Assert.That(scene.GetRootGameObjects().Length, Is.EqualTo(3));
            AssertSceneReferences(scene);
        }

        [UnityTest]
        public IEnumerator OriginalSceneEntersAndExitsPlayMode()
        {
            EditorSceneManager.OpenScene(SampleScene, OpenSceneMode.Single);
            yield return new EnterPlayMode();
            Assert.That(Application.isPlaying, Is.True);
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(SampleScene));
            Assert.That(Camera.main, Is.Not.Null);
            AssertSceneReferences(SceneManager.GetActiveScene());
            yield return null;
            yield return new ExitPlayMode();
            Assert.That(EditorApplication.isPlaying, Is.False);
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTearDown]
        public IEnumerator LeavePlayModeAfterFailure()
        {
            if (EditorApplication.isPlaying)
                yield return new ExitPlayMode();
        }

        internal static void AssertSceneReferences(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject),
                    Is.Zero, "Missing script: " + transform.name);
                foreach (var component in transform.GetComponents<Component>())
                {
                    Assert.That(component, Is.Not.Null, transform.name);
                    using (var serialized = new SerializedObject(component))
                    {
                        var property = serialized.GetIterator();
                        while (property.Next(true))
                        {
                            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                                property.objectReferenceValue == null && property.objectReferenceEntityIdValue != EntityId.None)
                                Assert.Fail("Missing reference: " + transform.name + "." + property.propertyPath);
                        }
                    }
                }
            }
        }
    }
}
