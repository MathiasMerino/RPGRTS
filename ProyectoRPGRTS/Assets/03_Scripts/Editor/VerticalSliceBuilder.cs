using System;
using System.Collections.Generic;
using Aethernia;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Aethernia.Editor
{
    public static class VerticalSliceBuilder
    {
        private static MedievalUiTheme theme;
        private static GameObject buttonPrefab;
        private static GameObject panelPrefab;
        public static void FinalisePresentation()
        {
            var scene = EditorSceneManager.OpenScene(PlayablePrototypeBuilder.ScenePath);
            foreach (var name in new[] { "South Wall", "West Wall" })
            {
                var wall = GameObject.Find(name);
                if (wall == null) throw new InvalidOperationException("Dungeon wall missing: " + name);
                var position = wall.transform.localPosition; position.y = 0.4f; wall.transform.localPosition = position;
                var scale = wall.transform.localScale; scale.y = 0.8f; wall.transform.localScale = scale;
            }
            // Low foreground parapets reveal the exit; footprint and impassable step remain unchanged.
            EditorSceneManager.SaveScene(scene);
        }
        public static void Upgrade()
        {
            var scene = EditorSceneManager.OpenScene(PlayablePrototypeBuilder.ScenePath);
            if (Object.FindAnyObjectByType<PrototypeDirector>() != null) throw new InvalidOperationException("Vertical slice already configured.");
            var game = Object.FindAnyObjectByType<GameSession>();
            var commands = game.GetComponent<PlayerCommands>();
            var hero = commands.Hero;
            var rules = ScriptableObject.CreateInstance<PrototypeRules>();
            AssetDatabase.CreateAsset(rules, "Assets/Settings/Prototype/ApprovedRules.asset"); game.Configure(rules);
            var hostile = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            hostile.SetColor("_BaseColor", new Color(0.65f, 0.12f, 0.11f));
            AssetDatabase.CreateAsset(hostile, "Assets/Settings/Prototype/Hostile.mat");
            var stone = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Development/Development_Stone.mat");
            var wood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Development/Development_Timber.mat");
            var roof = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Development/Development_Roof.mat");
            var floorMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Development/Development_Ground.mat");
            var dungeon = new GameObject("Dungeon Test Region").transform;
            Box(dungeon, "Dungeon Floor", new Vector3(50, -0.1f, 0), new Vector3(10, 0.2f, 10), stone);
            Box(dungeon, "North Wall", new Vector3(50, 1.5f, 4.9f), new Vector3(10, 3, 0.2f), wood);
            Box(dungeon, "South Wall", new Vector3(50, 1.5f, -4.9f), new Vector3(10, 3, 0.2f), wood);
            Box(dungeon, "West Wall", new Vector3(45.1f, 1.5f, 0), new Vector3(0.2f, 3, 9.6f), wood);
            Box(dungeon, "East Wall", new Vector3(54.9f, 1.5f, 0), new Vector3(0.2f, 3, 9.6f), wood);
            Node("Dungeon Objective", "dungeon-objective", "Hito de la mazmorra", new Vector3(52, 0.5f, 1), wood);
            Node("Dungeon Exit", "portal-surface", "Regresar al asentamiento", new Vector3(46.5f, 0.4f, -3), floorMaterial);
            Node("Dungeon Entrance", "portal-dungeon", "Entrar a la mazmorra de prueba", new Vector3(-10, 0.4f, 5), stone);
            Node("Faction A Market", "market", "Mercado de la Facción A", new Vector3(1, 0.5f, -8), wood);
            Node("Faction B Marker", "faction-b", "Representante de la Facción B", new Vector3(-15, 0.5f, 5), hostile);

            var workshop = new GameObject("Prototype Workshop");
            Box(workshop.transform, "Body", new Vector3(0, 1, 0), new Vector3(3, 2, 3), wood);
            Box(workshop.transform, "Roof", new Vector3(0, 2.1f, 0), new Vector3(3, 0.2f, 3), roof);
            var obstacle = workshop.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box; obstacle.center = Vector3.up * 1.1f; obstacle.size = new Vector3(3, 2.2f, 3); obstacle.carving = true;
            var workshopPrefab = PrefabUtility.SaveAsPrefabAsset(workshop, "Assets/02_Prefabs/Characters/PrototypeWorkshop.prefab");
            Object.DestroyImmediate(workshop);
            hero.gameObject.SetActive(false);
            var surface = Object.FindAnyObjectByType<NavMeshSurface>(); surface.BuildNavMesh();
            AssetDatabase.CreateAsset(surface.navMeshData, "Assets/Settings/Prototype/VerticalNavMesh.asset");
            hero.gameObject.SetActive(true);
            var pawn = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/Characters/PrototypeHero.prefab");
            var directorObject = new GameObject("Prototype Director");
            var construction = directorObject.AddComponent<WorldConstruction>(); construction.Configure(game, hero, workshopPrefab);
            var encounter = directorObject.AddComponent<TurnEncounter>(); encounter.Configure(game, hero, pawn, hostile);
            var rts = directorObject.AddComponent<RtsBattleController>(); rts.Configure(game, pawn, hostile);
            var director = directorObject.AddComponent<PrototypeDirector>();
            director.Configure(game, hero, Camera.main.GetComponent<IsometricCameraRig>(), construction, encounter, rts);
            CreateHud(game, director);
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
            Debug.Log("AETHERNIA_VERTICAL_CREATED: classes, inventory, persistence, dungeon, turns, economy, construction and RTS.");
        }

        private static void CreateHud(GameSession game, PrototypeDirector director)
        {
            theme = AssetDatabase.LoadAssetAtPath<MedievalUiTheme>("Assets/Settings/Prototype/MedievalTheme.asset");
            panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/UI/MedievalPanel.prefab");
            buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/UI/MedievalButton.prefab");
            var canvas = Object.FindAnyObjectByType<ExplorationHud>().transform;
            var menu = Panel(canvas, "Modes", new Vector2(0.5f, 1), new Vector2(1280, 48), new Vector2(0, -92));
            var bindings = new List<HudBinding>();
            var modes = new[] { "explore", "combat", "construct", "rts" };
            var titles = new[] { "Explorar", "Combate RPG", "Construir", "Batalla RTS" };
            for (var i = 0; i < modes.Length; i++)
            {
                var button = Button(menu, titles[i]); var rect = button.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = new Vector2(0, 0.5f); rect.sizeDelta = new Vector2(170, 36); rect.anchoredPosition = new Vector2(105 + i * 185, 0);
                bindings.Add(new HudBinding { Id = "mode-" + modes[i], Button = button });
            }
            var resources = Panel(canvas, "Resources", new Vector2(0.5f, 1), new Vector2(1280, 42), new Vector2(0, -138));
            var stats = Label(resources, "", 17); PlayablePrototypeBuilder.Stretch(stats.rectTransform, 20, 4);
            var actions = Panel(canvas, "Context Actions", new Vector2(1, 0.5f), new Vector2(280, 420), new Vector2(-154, -25));
            var detail = Label(actions, "", 16); detail.rectTransform.anchorMin = detail.rectTransform.anchorMax = new Vector2(0.5f, 1);
            detail.rectTransform.sizeDelta = new Vector2(248, 120); detail.rectTransform.anchoredPosition = new Vector2(0, -70);
            var grid = new GameObject("Action Grid", typeof(RectTransform), typeof(GridLayoutGroup)); grid.transform.SetParent(actions, false);
            var gridRect = grid.GetComponent<RectTransform>(); gridRect.anchorMin = gridRect.anchorMax = new Vector2(0.5f, 0);
            gridRect.sizeDelta = new Vector2(248, 270); gridRect.anchoredPosition = new Vector2(0, 150);
            var layout = grid.GetComponent<GridLayoutGroup>(); layout.cellSize = new Vector2(116, 34); layout.spacing = new Vector2(8, 8);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount; layout.constraintCount = 2;
            var ids = new[] { "class-warrior", "class-mage", "class-monk", "sword", "armour", "ration", "trade", "save", "load", "attack", "guard", "recover", "end", "approach", "select-all", "charge", "escalate" };
            var labels = new[] { "Guerrero", "Mago", "Monje", "Equipar espada", "Equipar túnica", "Usar ración", "Comerciar", "Guardar F5", "Cargar F9", "Atacar", "Guardia", "Recuperar", "Fin de turno", "Acercarse", "Todo el grupo", "Avanzar", "Escalar a RTS" };
            for (var i = 0; i < ids.Length; i++) bindings.Add(new HudBinding { Id = ids[i], Button = Button(grid.transform, labels[i]) });
            var namePanel = Panel(canvas, "Character Creation", new Vector2(0, 1), new Vector2(285, 56), new Vector2(158, -196));
            var inputObject = new GameObject("Character Name", typeof(RectTransform), typeof(Image), typeof(InputField)); inputObject.transform.SetParent(namePanel, false);
            PlayablePrototypeBuilder.Stretch(inputObject.GetComponent<RectTransform>(), 12, 10); inputObject.GetComponent<Image>().color = theme.Button;
            var text = Label(inputObject.transform, "Aventurero", 17); PlayablePrototypeBuilder.Stretch(text.rectTransform, 8, 3); text.alignment = TextAnchor.MiddleLeft;
            var input = inputObject.GetComponent<InputField>(); input.textComponent = text; input.text = "Aventurero"; input.characterLimit = 32;
            var hud = canvas.gameObject.AddComponent<VerticalSliceHud>(); hud.Configure(game, director, stats, detail, bindings.ToArray(), input);
            var help = GameObject.Find("Controls").GetComponentInChildren<Text>();
            help.text = "EXPLORACIÓN\nClic izq.: seleccionar\nClic der.: mover / construir\nE: activar objeto cercano\nWASD: cámara · rueda: zoom\nPortal gris: mazmorra\nCofre: equipo de prueba\nF5/F9: guardar / cargar";
            PrefabUtility.RecordPrefabInstancePropertyModifications(help);
        }

        private static void Node(string name, string id, string title, Vector3 position, Material material)
        { var go = Box(null, name, position, Vector3.one * 0.8f, material); go.AddComponent<InteractionPoint>().Configure(id, title); }
        private static GameObject Box(Transform parent, string name, Vector3 centre, Vector3 size, Material material)
        { var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; go.transform.SetParent(parent, false); go.transform.localPosition = centre; go.transform.localScale = size; go.GetComponent<Renderer>().sharedMaterial = material; return go; }
        private static Transform Panel(Transform parent, string name, Vector2 anchor, Vector2 size, Vector2 position)
        { var go = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, parent); go.name = name; var rect = go.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = size; rect.anchoredPosition = position; return go.transform; }
        private static Text Label(Transform parent, string value, int size)
        { var go = new GameObject("Label", typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false); var text = go.GetComponent<Text>(); text.font = theme.Font; text.text = value; text.fontSize = size; text.color = theme.Text; text.raycastTarget = false; return text; }
        private static Button Button(Transform parent, string title)
        { var go = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, parent); go.name = title; var text = go.GetComponentInChildren<Text>(); text.text = title; text.fontSize = 15; return go.GetComponent<Button>(); }
    }
}
