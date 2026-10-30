using System;
using System.IO;
using System.Linq;
using Aethernia;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Aethernia.Editor
{
    public static class PlayablePrototypeBuilder
    {
        public const string ScenePath = "Assets/01_Scenes/PlayablePrototype.unity";
        private const string Settings = "Assets/Settings/Prototype";
        private static MedievalUiTheme theme;
        private static GameObject panelPrefab;
        private static GameObject buttonPrefab;

        public static void Create()
        {
            if (File.Exists(ScenePath) || Directory.Exists(Settings))
                throw new InvalidOperationException("Playable prototype already exists; it will not be overwritten.");
            var scene = EditorSceneManager.OpenScene("Assets/01_Scenes/TechnicalSandbox.unity");
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new IOException("Cannot create scene copy.");
            Folder("Assets/Settings", "Prototype");
            Folder("Assets/02_Prefabs", "Characters");
            Folder("Assets/02_Prefabs", "Camera");
            Folder("Assets/02_Prefabs", "UI");
            GameObject.Find("ReferenceHuman_180cm").SetActive(false);
            var surface = new GameObject("Baked Navigation").AddComponent<NavMeshSurface>();
            surface.agentTypeID = 0;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true;
            surface.voxelSize = 0.075f;
            surface.BuildNavMesh();
            AssetDatabase.CreateAsset(surface.navMeshData, Settings + "/PrototypeNavMesh.asset");

            var source = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/ScaleReferences/ReferenceHuman_180cm.prefab");
            var actorObject = (GameObject)PrefabUtility.InstantiatePrefab(source);
            PrefabUtility.UnpackPrefabInstance(actorObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            actorObject.name = "Player Character";
            var agent = actorObject.AddComponent<NavMeshAgent>();
            agent.radius = 0.3f; agent.height = 1.8f; agent.speed = 3.2f;
            agent.angularSpeed = 720; agent.acceleration = 20; agent.stoppingDistance = 0.1f;
            var indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            indicator.name = "Selection Indicator";
            indicator.transform.SetParent(actorObject.transform, false);
            indicator.transform.localPosition = new Vector3(0, 0.015f, 0);
            indicator.transform.localScale = new Vector3(0.9f, 0.008f, 0.9f);
            Object.DestroyImmediate(indicator.GetComponent<Collider>());
            indicator.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Development/Development_Road.mat");
            var actor = actorObject.AddComponent<ExplorationUnit>(); actor.Configure(indicator);
            PrefabUtility.SaveAsPrefabAssetAndConnect(actorObject, "Assets/02_Prefabs/Characters/PrototypeHero.prefab", InteractionMode.AutomatedAction);
            actorObject.transform.position = new Vector3(-5, 0, -4);

            var camera = GameObject.Find("Inspection Camera").GetComponent<Camera>();
            camera.name = "Isometric Camera";
            var rig = camera.gameObject.AddComponent<IsometricCameraRig>(); rig.Configure(new Vector3(-1, 0, 0));
            PrefabUtility.SaveAsPrefabAssetAndConnect(camera.gameObject, "Assets/02_Prefabs/Camera/IsometricCamera.prefab", InteractionMode.AutomatedAction);
            var session = new GameObject("Game Session").AddComponent<GameSession>();
            var commands = session.gameObject.AddComponent<PlayerCommands>(); commands.Configure(camera, actor, session);
            var chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chest.name = "Interaction Chest"; chest.transform.position = new Vector3(-2, 0.4f, -5);
            chest.transform.localScale = new Vector3(0.8f, 0.8f, 0.6f);
            chest.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Development/Development_Timber.mat");
            chest.AddComponent<InteractionPoint>();
            chest.AddComponent<NavMeshObstacle>().carving = true;
            CreateUi(session, commands);
            session.gameObject.AddComponent<BuildSmokeProbe>();
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) }
                .Concat(EditorBuildSettings.scenes.Where(s => s.path != ScenePath)).ToArray();
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene);
            Debug.Log("AETHERNIA_PLAYABLE_CREATED: " + ScenePath);
        }

        private static void CreateUi(GameSession session, PlayerCommands commands)
        {
            theme = ScriptableObject.CreateInstance<MedievalUiTheme>();
            theme.Font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            AssetDatabase.CreateAsset(theme, Settings + "/MedievalTheme.asset");
            var panel = new GameObject("Medieval Panel", typeof(RectTransform), typeof(Image));
            panel.GetComponent<Image>().color = theme.Panel;
            panelPrefab = PrefabUtility.SaveAsPrefabAsset(panel, "Assets/02_Prefabs/UI/MedievalPanel.prefab");
            Object.DestroyImmediate(panel);
            var button = new GameObject("Medieval Button", typeof(RectTransform), typeof(Image), typeof(Button));
            button.GetComponent<Image>().color = theme.Button;
            var label = Text(button.transform, "Action", 18, TextAnchor.MiddleCenter);
            Stretch(label.rectTransform);
            buttonPrefab = PrefabUtility.SaveAsPrefabAsset(button, "Assets/02_Prefabs/UI/MedievalButton.prefab");
            Object.DestroyImmediate(button);

            var canvasObject = new GameObject("Medieval HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = 0.5f;
            var top = Panel(canvasObject.transform, "Title Bar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 64), new Vector2(0, -32));
            var title = Text(top, "CRÓNICAS DE AETHERNIA  |  PROTOTIPO", 24, TextAnchor.MiddleLeft);
            Stretch(title.rectTransform, 24, 8);
            var help = Panel(canvasObject.transform, "Controls", Vector2.zero, Vector2.zero, new Vector2(285, 220), new Vector2(158, 222));
            var helpText = Text(help, "EXPLORACIÓN\n\nClic izquierdo: seleccionar\nClic derecho: caminar\nE: interactuar con objeto\nWASD / flechas: cámara\nRueda: acercar / alejar\nBotón central: desplazar", 17, TextAnchor.UpperLeft);
            Stretch(helpText.rectTransform, 18, 16);
            var bottom = Panel(canvasObject.transform, "Message Bar", Vector2.zero, new Vector2(1, 0), new Vector2(0, 92), new Vector2(0, 46));
            var message = Text(bottom, "", 18, TextAnchor.MiddleLeft); Stretch(message.rectTransform, 24, 12);
            message.rectTransform.offsetMax = new Vector2(-250, -12);
            var action = Button(bottom, "Interactuar", new Vector2(1, 0.5f), new Vector2(-130, 0));
            var hud = canvasObject.AddComponent<ExplorationHud>();
            PrefabUtility.SaveAsPrefabAssetAndConnect(canvasObject, "Assets/02_Prefabs/UI/ExplorationHud.prefab", InteractionMode.AutomatedAction);
            hud.Configure(session, commands, message, action);
            PrefabUtility.RecordPrefabInstancePropertyModifications(hud);
            new GameObject("Event System", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        public static void Build()
        {
            Directory.CreateDirectory("Builds/Windows");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { ScenePath }, locationPathName = "Builds/Windows/Aethernia.exe",
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development });
            if (report.summary.result != BuildResult.Succeeded) throw new Exception("Build failed: " + report.summary.result);
            File.WriteAllText("Logs/Build-summary.txt", "Succeeded; bytes=" + report.summary.totalSize + "; seconds=" + report.summary.totalTime.TotalSeconds);
        }

        private static void Folder(string parent, string name) { if (!AssetDatabase.IsValidFolder(parent + "/" + name)) AssetDatabase.CreateFolder(parent, name); }
        public static void Stretch(RectTransform rect, float x = 0, float y = 0)
        { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = new Vector2(x, y); rect.offsetMax = new Vector2(-x, -y); }
        private static Transform Panel(Transform parent, string name, Vector2 min, Vector2 max, Vector2 size, Vector2 position)
        {
            var panel = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, parent);
            panel.name = name; var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.sizeDelta = size; rect.anchoredPosition = position;
            return panel.transform;
        }
        private static Text Text(Transform parent, string value, int size, TextAnchor align)
        {
            var go = new GameObject("Label", typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>(); text.text = value; text.font = theme.Font;
            text.fontSize = size; text.color = theme.Text; text.alignment = align; text.raycastTarget = false;
            return text;
        }
        private static Button Button(Transform parent, string label, Vector2 anchor, Vector2 position)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, parent);
            go.name = label; var rect = go.GetComponent<RectTransform>(); rect.anchorMin = anchor; rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(220, 44); rect.anchoredPosition = position;
            go.GetComponentInChildren<Text>().text = label;
            return go.GetComponent<Button>();
        }
    }
}
