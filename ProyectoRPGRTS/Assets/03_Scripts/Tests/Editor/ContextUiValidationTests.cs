using System.Collections;
using Aethernia;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Aethernia.Tests.Editor
{
    public class ContextUiValidationTests
    {
        [SetUp] public void Open()
        { if (!EditorApplication.isPlaying) EditorSceneManager.OpenScene("Assets/01_Scenes/PlayablePrototype.unity"); }

        [UnityTest]
        public IEnumerator PagesPreserveActionsAndModesHaveReadableSeparateLayouts()
        {
            yield return new EnterPlayMode(); yield return null; yield return null;
            var d = Object.FindAnyObjectByType<PrototypeDirector>();
            var hud = Object.FindAnyObjectByType<VerticalSliceHud>();
            Assert.That(hud.ActivePage, Is.EqualTo(ExplorationPage.Overview));
            AssertActionCount(0); Assert.That(FindInput().gameObject.activeInHierarchy, Is.False);
            yield return CapturePair("Overview");

            FindButton("Personaje").onClick.Invoke();
            Assert.That(hud.ActivePage, Is.EqualTo(ExplorationPage.Character));
            AssertActionCount(3); Assert.That(FindInput().gameObject.activeInHierarchy, Is.True);
            FindButton("Mago").onClick.Invoke();
            Assert.That(d.Session.State.ClassId, Is.EqualTo("mage")); Assert.That(d.Session.State.Health, Is.EqualTo(16));
            yield return CapturePair("Character");

            Assert.That(d.Hero.Warp(new Vector3(-3.4f, 0, -5)), Is.True); yield return null;
            Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(d.Hero, d.Session), Is.True);
            FindButton("Inventario").onClick.Invoke();
            Assert.That(hud.ActivePage, Is.EqualTo(ExplorationPage.Inventory));
            AssertActionCount(2); Assert.That(FindInput().gameObject.activeInHierarchy, Is.False);
            AssertListenersAreNotDuplicated(hud, d.Session);
            Assert.That(d.Session.State.Weapon, Is.EqualTo("training-sword"));
            var gold = d.Session.State.Gold;
            FindButton("Comerciar").onClick.Invoke(); Assert.That(d.Session.State.Gold, Is.EqualTo(gold - 5));
            Assert.That(UiInputGuard.HasUiAt(RectTransformUtility.WorldToScreenPoint(null, FindButton("Inventario").transform.position)), Is.True);
            yield return CapturePair("Inventory");

            FindButton("Partida").onClick.Invoke(); AssertActionCount(2);
            Assert.That(FindButton("Guardar F5").gameObject.activeInHierarchy, Is.True);
            Assert.That(FindButton("Cargar F9").gameObject.activeInHierarchy, Is.True);
            // Do not invoke these buttons: they target the user's normal save slot.
            yield return CapturePair("Session");

            Assert.That(d.Execute("construct"), Is.True); AssertActionCount(0);
            Assert.That(HelpText().text, Does.StartWith("CONSTRUCCIÓN"));
            Assert.That(FindButton("Inventario").gameObject.activeInHierarchy, Is.False);
            yield return CapturePair("Construction");

            Assert.That(d.Execute("explore"), Is.True);
            Assert.That(hud.ActivePage, Is.EqualTo(ExplorationPage.Overview)); AssertActionCount(0);
            Assert.That(d.Encounter.Begin(42), Is.True); AssertActionCount(6);
            Assert.That(HelpText().text, Does.StartWith("COMBATE RPG"));
            hud.SelectPage(ExplorationPage.Inventory); Assert.That(hud.ActivePage, Is.EqualTo(ExplorationPage.Overview));
            yield return CapturePair("Rpg");
            Assert.That(d.Hero.Warp(d.Encounter.EnemyPosition + Vector3.right * 1.3f), Is.True);
            for (var i = 0; i < 20 && !d.Encounter.Battle.Finished; i++)
            { d.Encounter.Attack(); if (!d.Encounter.Battle.Finished) d.Encounter.EndTurn(); yield return null; }
            Assert.That(d.Encounter.Battle.Won, Is.True);
            Assert.That(d.Execute("escalate"), Is.True); AssertActionCount(2);
            Assert.That(HelpText().text, Does.StartWith("GUERRA RTS"));
            Assert.That(FindButton("Personaje").gameObject.activeInHierarchy, Is.False);
            yield return CapturePair("Rts");
            LogAssert.NoUnexpectedReceived(); yield return new ExitPlayMode();
        }

        private static VerticalSliceHud Hud() => Object.FindAnyObjectByType<VerticalSliceHud>();
        private static InputField FindInput() => Hud().GetComponentInChildren<InputField>(true);
        private static Text HelpText() => Hud().transform.Find("Controls").GetComponentInChildren<Text>(true);
        private static Button FindButton(string name)
        {
            foreach (var button in Hud().GetComponentsInChildren<Button>(true)) if (button.name == name) return button;
            Assert.Fail("Missing action: " + name); return null;
        }
        private static void AssertActionCount(int count)
        {
            var actual = 0;
            foreach (var button in Hud().transform.Find("Context Actions/Action Grid").GetComponentsInChildren<Button>(true))
                if (button.gameObject.activeInHierarchy) actual++;
            Assert.That(actual, Is.EqualTo(count), "Only current-page/mode actions should be exposed.");
            Assert.That(Object.FindAnyObjectByType<ExplorationHud>().ActionButton.gameObject.activeInHierarchy,
                Is.EqualTo(Object.FindAnyObjectByType<GameSession>().Mode == PrototypeMode.Exploration), "The exploration interaction control must be hidden in other modes.");
        }
        private static void AssertListenersAreNotDuplicated(VerticalSliceHud hud, GameSession session)
        {
            for (var i = 0; i < 5; i++) { hud.enabled = false; hud.enabled = true; }
            FindButton("Item training-sword").onClick.Invoke();
            var messages = 0; System.Action<string> listener = _ => messages++;
            session.Message += listener; FindButton("Equipar espada").onClick.Invoke(); session.Message -= listener;
            Assert.That(messages, Is.EqualTo(1), "One UI command must dispatch once after re-enabling.");
        }
        private static IEnumerator CapturePair(string page)
        {
            foreach (var width in new[] { 1280, 1920 })
                yield return SceneCapture.Save("Logs/Context-" + page + "-" + width + ".png", width, width * 9 / 16, AssertLayout);
        }
        private static void AssertLayout(int width, int height)
        {
            var hud = Hud(); var view = Camera.main;
            var buttons = hud.GetComponentsInChildren<Button>();
            var areas = new Rect[buttons.Length];
            for (var i = 0; i < buttons.Length; i++)
            {
                var corners = new Vector3[4]; buttons[i].GetComponent<RectTransform>().GetWorldCorners(corners);
                var min = RectTransformUtility.WorldToScreenPoint(view, corners[0]);
                var max = RectTransformUtility.WorldToScreenPoint(view, corners[2]);
                areas[i] = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
                Assert.That(min.x, Is.GreaterThanOrEqualTo(-1), buttons[i].name);
                Assert.That(min.y, Is.GreaterThanOrEqualTo(-1), buttons[i].name);
                Assert.That(max.x, Is.LessThanOrEqualTo(width + 1), buttons[i].name);
                Assert.That(max.y, Is.LessThanOrEqualTo(height + 1), buttons[i].name);
                for (var j = 0; j < i; j++) Assert.That(areas[i].Overlaps(areas[j]), Is.False, buttons[i].name + " overlaps " + buttons[j].name);
            }
            foreach (var text in hud.GetComponentsInChildren<Text>())
            {
                if (text.GetComponentInParent<InputField>() != null || string.IsNullOrEmpty(text.text)) continue;
                Assert.That(text.preferredHeight, Is.LessThanOrEqualTo(text.rectTransform.rect.height + 2), "Clipped text: " + text.transform.parent.name + " / " + text.text);
            }
        }
        [UnityTearDown] public IEnumerator Cleanup() { if (EditorApplication.isPlaying) yield return new ExitPlayMode(); }
    }
}
