using System;
using System.Collections;
using System.IO;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class AlphaStabilizationTests
    {
        [UnityTest]
        public IEnumerator InteractionHasOneClickableSurfaceAtThreeResolutions()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.TacticalScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            yield return VerifyInteraction();
        }

        private static IEnumerator VerifyInteraction()
        {
            Directory.CreateDirectory("Logs/E201/Captures");
            var flow = Object.FindAnyObjectByType<DemoFlow>();
            flow.SlotPath = "Logs/E201/interaction-" + Guid.NewGuid().ToString("N") + ".json";
            flow.Director.SavePathOverride = flow.SlotPath;
            Assert.That(flow.StartNew("Prueba", "warrior", "human"), Is.True);
            flow.CompleteIntro();

            var quest = Object.FindAnyObjectByType<LostCargoController>();
            yield return WorldExplorationRoute.MoveTo(flow, quest.Npc.transform.position + Vector3.back * 1.1f);
            yield return UxExperienceRoute.Wait(.25f);
            var hud = flow.ExistingHud;
            var surface = hud.transform.Find("UX Nearby interaction").GetComponent<Button>();
            var legacy = hud.GetComponent<ExplorationHud>().ActionButton;
            var caption = surface.GetComponentInChildren<TMP_Text>();
            Assert.That(surface.interactable, Is.True);
            Assert.That(caption.text, Does.StartWith("E · Hablar"));

            // La notificación se publica entre refrescos, tal como ocurría en la captura recortada.
            flow.Session.Say("Encuentra al habitante junto al camino.");
            Assert.That(legacy.gameObject.activeSelf, Is.False);
            var canvas = (RectTransform)hud.transform;
            foreach (var width in new[] { 1280, 1920, 2560 })
            {
                yield return DemoCapture.Save("Logs/E201/Captures/Interaction-" + width + ".png", width, width * 9 / 16, null,
                    (_, w, _) =>
                    {
                        caption.ForceMeshUpdate();
                        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas, surface.transform);
                        Assert.That(canvas.rect.Contains(bounds.min) && canvas.rect.Contains(bounds.max), Is.True, w.ToString());
                        Assert.That(caption.isTextOverflowing, Is.False, w.ToString());
                        Assert.That(legacy.gameObject.activeSelf, Is.False);
                    });
            }

            surface.onClick.Invoke();
            yield return null;
            Assert.That(quest.GetComponent<LostCargoView>().DialogueOpen, Is.True);
            Assert.That(surface.gameObject.activeInHierarchy, Is.False, "el diálogo tiene prioridad sobre la interacción");
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTearDown]
        public IEnumerator Exit()
        {
            if (EditorApplication.isPlaying) yield return new ExitPlayMode();
        }
    }
}
