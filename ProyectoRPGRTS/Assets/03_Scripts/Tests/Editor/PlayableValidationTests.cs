using System.Collections;
using System.IO;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Aethernia.Tests.Editor
{
    public class PlayableValidationTests
    {
        [SetUp]
        public void OpenPlayable()
        {
            if (!EditorApplication.isPlaying)
                EditorSceneManager.OpenScene("Assets/01_Scenes/PlayablePrototype.unity");
        }

        [UnityTest]
        public IEnumerator CameraSelectionAndUiBlockWork()
        {
            yield return new EnterPlayMode();
            yield return null;
            var commands = Object.FindAnyObjectByType<PlayerCommands>();
            var camera = Camera.main;
            var rig = camera.GetComponent<IsometricCameraRig>();
            var hero = commands.Hero;
            var screen = camera.WorldToScreenPoint(hero.transform.position + Vector3.up * 0.9f);
            Assert.That(commands.IssuePointer(screen, true, false), Is.True);
            Assert.That(hero.IsSelected, Is.True);
            Assert.That(commands.IssuePointer(screen, false, true), Is.False);
            var position = rig.Focus;
            var size = rig.Size;
            rig.ApplyInput(Vector2.one, 10, 2, true);
            Assert.That(rig.Focus, Is.EqualTo(position));
            Assert.That(rig.Size, Is.EqualTo(size));
            rig.ApplyInput(Vector2.one, 100, 100, false);
            Assert.That(rig.Size, Is.EqualTo(7));
            Assert.That(Mathf.Abs(rig.Focus.x), Is.LessThanOrEqualTo(18));
            rig.ApplyInput(-Vector2.one, -100, 100, false);
            Assert.That(rig.Size, Is.EqualTo(25));
            Assert.That(Mathf.Abs(rig.Focus.z), Is.LessThanOrEqualTo(18));
            yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator MovementAndInteractionCompleteTheExplorationLoop()
        {
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            var commands = Object.FindAnyObjectByType<PlayerCommands>();
            var session = Object.FindAnyObjectByType<GameSession>();
            var hero = commands.Hero;
            Assert.That(hero.Agent.isOnNavMesh, Is.True);
            Assert.That(hero.TryMoveTo(new Vector3(200, 0, 200)), Is.False);
            Assert.That(hero.TryMoveTo(new Vector3(-2, 0, 5)), Is.False, "Obstacle interior is not walkable.");
            var destination = new Vector3(-2, 0, 8);
            Assert.That(hero.TryMoveTo(destination), Is.True);
            var deadline = Time.realtimeSinceStartup + 12;
            while (Vector3.Distance(hero.transform.position, destination) > 0.3f && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.That(Vector3.Distance(hero.transform.position, destination), Is.LessThan(0.3f));
            var target = GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>();
            Assert.That(target.TryInteract(hero, session), Is.False, "Distance must be enforced.");
            Assert.That(hero.Warp(new Vector3(-3.4f, 0, -5)), Is.True);
            yield return null;
            Assert.That(target.TryInteract(hero, session), Is.True);
            Assert.That(session.HasFact(target.FactId), Is.True);
            Assert.That(target.TryInteract(hero, session), Is.False, "No repeated rewards.");
            target.gameObject.SetActive(false);
            Assert.That(target.TryInteract(hero, session), Is.False);
            LogAssert.NoUnexpectedReceived();
            yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator HudRendersAtTwoResolutionsAndDoesNotDuplicateListeners()
        {
            yield return new EnterPlayMode();
            var hud = Object.FindAnyObjectByType<ExplorationHud>();
            var session = Object.FindAnyObjectByType<GameSession>();
            Assert.That(hud, Is.Not.Null, "The HUD must survive scene serialization.");
            Assert.That(session, Is.Not.Null);
            Assert.That(hud.ActionButton, Is.Not.Null, "The action reference must be serialized.");
            AssertNoDuplicateListeners(hud, session);
            Canvas.ForceUpdateCanvases();
            Assert.That(UiInputGuard.HasUiAt(RectTransformUtility.WorldToScreenPoint(null, hud.ActionButton.transform.position)), Is.True);
            Object.FindAnyObjectByType<VerticalSliceHud>().SelectPage(ExplorationPage.Character);
            var nameInput = Object.FindAnyObjectByType<InputField>();
            nameInput.ActivateInputField(); yield return null; yield return null;
            Assert.That(UiInputGuard.BlocksWorldInput, Is.True, "Typing must not pan the world camera or interact.");
            nameInput.DeactivateInputField();
            var canvas = hud.GetComponent<Canvas>();
            var camera = Camera.main;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 2;
            foreach (var width in new[] { 1280, 1920 })
            {
                var height = width * 9 / 16;
                var target = RenderTexture.GetTemporary(width, height, 24);
                var previous = camera.targetTexture;
                Texture2D image = null;
                try
                {
                    camera.targetTexture = target;
                    yield return null;
                    yield return null;
                    yield return null;
                    var active = RenderTexture.active;
                    RenderTexture.active = target;
                    image = new Texture2D(width, height, TextureFormat.RGB24, false);
                    image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply();
                    RenderTexture.active = active;
                    File.WriteAllBytes("Logs/Playable-" + width + ".png", image.EncodeToPNG());
                }
                finally
                {
                    camera.targetTexture = previous;
                    if (image != null) Object.DestroyImmediate(image);
                    RenderTexture.ReleaseTemporary(target);
                }
            }
            LogAssert.NoUnexpectedReceived();
            yield return new ExitPlayMode();
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        { if (EditorApplication.isPlaying) yield return new ExitPlayMode(); }

        private static void AssertNoDuplicateListeners(ExplorationHud hud, GameSession session)
        {
            // Keep captured locals out of the coroutine restored across domain reload.
            for (var i = 0; i < 5; i++) { hud.enabled = false; hud.enabled = true; }
            var messages = 0;
            System.Action<string> listener = _ => messages++;
            session.Message += listener;
            hud.ActionButton.onClick.Invoke();
            session.Message -= listener;
            Assert.That(messages, Is.EqualTo(1));
        }
    }
}
