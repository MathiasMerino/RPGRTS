using System;
using Aethernia;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Aethernia.Editor
{
    public static class ContextUiBuilder
    {
        public static void Upgrade()
        {
            var scene = EditorSceneManager.OpenScene(PlayablePrototypeBuilder.ScenePath);
            var hud = Object.FindAnyObjectByType<VerticalSliceHud>();
            var context = hud.transform.Find("Context Actions");
            if (context.Find("Exploration Pages") != null) throw new InvalidOperationException("Context UI already exists; no duplicate controls were created.");
            var theme = AssetDatabase.LoadAssetAtPath<MedievalUiTheme>("Assets/Settings/Prototype/MedievalTheme.asset");
            EditorUtility.SetDirty(theme);
            hud.enabled = false;
            var pages = new GameObject("Exploration Pages", typeof(RectTransform), typeof(GridLayoutGroup));
            pages.transform.SetParent(context, false);
            var rect = pages.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(248, theme.ActionButtonSize.y * 2 + theme.Spacing);
            rect.anchoredPosition = new Vector2(0, -theme.Padding);
            var layout = pages.GetComponent<GridLayoutGroup>();
            layout.cellSize = theme.ActionButtonSize; layout.spacing = Vector2.one * theme.Spacing;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount; layout.constraintCount = 2;
            var template = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/UI/MedievalButton.prefab");
            var ids = new[] { "overview", "character", "inventory", "session" };
            var titles = new[] { "Resumen", "Personaje", "Inventario", "Partida" };
            var bindings = new HudBinding[ids.Length];
            for (var i = 0; i < ids.Length; i++)
            {
                var button = (GameObject)PrefabUtility.InstantiatePrefab(template, pages.transform);
                button.name = titles[i]; button.GetComponentInChildren<Text>().text = titles[i];
                bindings[i] = new HudBinding { Id = "page-" + ids[i], Button = button.GetComponent<Button>() };
            }
            using (var serialized = new SerializedObject(hud))
            {
                var detail = (Text)serialized.FindProperty("detail").objectReferenceValue;
                detail.fontSize = theme.BodyFontSize; detail.supportRichText = true;
                var detailRect = detail.rectTransform;
                detailRect.anchorMin = detailRect.anchorMax = detailRect.pivot = new Vector2(0.5f, 1);
                detailRect.sizeDelta = new Vector2(248, 128);
                detailRect.anchoredPosition = new Vector2(0, -108);
            }
            var grid = context.Find("Action Grid").GetComponent<RectTransform>();
            grid.anchorMin = grid.anchorMax = grid.pivot = new Vector2(0.5f, 1);
            grid.sizeDelta = new Vector2(248, 150); grid.anchoredPosition = new Vector2(0, -252);
            grid.GetComponent<GridLayoutGroup>().cellSize = theme.ActionButtonSize;
            grid.GetComponent<GridLayoutGroup>().spacing = Vector2.one * theme.Spacing;
            var help = hud.transform.Find("Controls").GetComponentInChildren<Text>();
            help.fontSize = theme.BodyFontSize; PrefabUtility.RecordPrefabInstancePropertyModifications(help);
            foreach (var button in hud.GetComponentsInChildren<Button>(true))
            {
                var label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                label.font = theme.Font; label.fontSize = theme.ButtonFontSize; label.color = theme.Text;
                PrefabUtility.RecordPrefabInstancePropertyModifications(label);
            }
            var character = hud.transform.Find("Character Creation").gameObject;
            character.SetActive(false);
            hud.ConfigureContext(theme, help, character, pages, grid, bindings);
            hud.enabled = true; EditorUtility.SetDirty(hud);
            EditorSceneManager.MarkSceneDirty(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
            Debug.Log("AETHERNIA_CONTEXT_UI: reused actions; four exploration sections and mode help configured.");
        }
    }
}
