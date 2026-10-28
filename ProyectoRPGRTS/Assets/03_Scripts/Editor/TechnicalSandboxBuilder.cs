using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Aethernia.Editor
{
    /// <summary>Creates metric development assets once, without replacing existing work.</summary>
    public static class TechnicalSandboxBuilder
    {
        private const string ScenePath = "Assets/01_Scenes/TechnicalSandbox.unity";
        private const string PrefabFolder = "Assets/02_Prefabs/ScaleReferences";
        private const string MaterialFolder = "Assets/Settings/Development";

        [MenuItem("Aethernia/Preparacion/Crear referencias y escena de pruebas")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before creating development assets.");
            for (var i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty)
                    throw new InvalidOperationException("Save your open scenes before running this tool.");
            if (File.Exists(ScenePath) || Directory.Exists(PrefabFolder) || Directory.Exists(MaterialFolder))
                throw new InvalidOperationException("Development outputs already exist. Existing assets will not be replaced.");
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");

            AssetDatabase.CreateFolder("Assets/02_Prefabs", "ScaleReferences");
            AssetDatabase.CreateFolder("Assets/Settings", "Development");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var stone = Material("Stone", new Color(0.43f, 0.46f, 0.48f), shader);
            var human = Material("Human", new Color(0.12f, 0.40f, 0.68f), shader);
            var timber = Material("Timber", new Color(0.38f, 0.22f, 0.11f), shader);
            var plaster = Material("Plaster", new Color(0.72f, 0.66f, 0.52f), shader);
            var roof = Material("Roof", new Color(0.32f, 0.15f, 0.10f), shader);
            var grass = Material("Ground", new Color(0.23f, 0.32f, 0.23f), shader);
            var road = Material("Road", new Color(0.43f, 0.37f, 0.28f), shader);

            var cubePrefab = Save("ReferenceCube_1m", root =>
                Box(root.transform, "One metre", new Vector3(0, 0.5f, 0), Vector3.one, stone));
            var humanPrefab = Save("ReferenceHuman_180cm", root =>
            {
                var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "Height 1.80m - diameter 0.60m";
                body.transform.SetParent(root.transform, false);
                body.transform.localPosition = new Vector3(0, 0.9f, 0);
                body.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
                body.GetComponent<Renderer>().sharedMaterial = human;
            });
            var doorPrefab = Save("ReferenceDoor_120x220cm", root =>
            {
                Box(root.transform, "Left post", new Vector3(-0.7f, 1.2f, 0), new Vector3(0.2f, 2.4f, 0.25f), timber);
                Box(root.transform, "Right post", new Vector3(0.7f, 1.2f, 0), new Vector3(0.2f, 2.4f, 0.25f), timber);
                Box(root.transform, "Header - clearance 2.20m", new Vector3(0, 2.3f, 0), new Vector3(1.2f, 0.2f, 0.25f), timber);
            });
            var windowPrefab = Save("ReferenceWindow_100x120cm", root =>
            {
                Box(root.transform, "Left jamb", new Vector3(-0.6f, 1.6f, 0), new Vector3(0.2f, 1.6f, 0.25f), timber);
                Box(root.transform, "Right jamb", new Vector3(0.6f, 1.6f, 0), new Vector3(0.2f, 1.6f, 0.25f), timber);
                Box(root.transform, "Sill - clearance starts at 1m", new Vector3(0, 0.9f, 0), new Vector3(1, 0.2f, 0.25f), timber);
                Box(root.transform, "Header", new Vector3(0, 2.3f, 0), new Vector3(1, 0.2f, 0.25f), timber);
            });
            var housePrefab = Save("ReferenceHouse_6x8m", root =>
            {
                Box(root.transform, "Back wall", new Vector3(0, 1.5f, 3.9f), new Vector3(6, 3, 0.2f), plaster);
                Box(root.transform, "Left wall", new Vector3(-2.9f, 1.5f, 0), new Vector3(0.2f, 3, 7.6f), plaster);
                Box(root.transform, "Right wall", new Vector3(2.9f, 1.5f, 0), new Vector3(0.2f, 3, 7.6f), plaster);
                Box(root.transform, "Front left", new Vector3(-1.9f, 1.5f, -3.9f), new Vector3(2.2f, 3, 0.2f), plaster);
                Box(root.transform, "Front right", new Vector3(1.9f, 1.5f, -3.9f), new Vector3(2.2f, 3, 0.2f), plaster);
                Box(root.transform, "Front lintel", new Vector3(0, 2.7f, -3.9f), new Vector3(1.6f, 0.6f, 0.2f), plaster);
                var door = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab);
                door.transform.SetParent(root.transform, false);
                door.transform.localPosition = new Vector3(0, 0, -3.875f);
                Box(root.transform, "Roof", new Vector3(0, 3.075f, 0), new Vector3(6, 0.15f, 8), roof);
            });
            var groundPrefab = Save("ReferenceGround_40m", root =>
                Box(root.transform, "Surface at Y=0", new Vector3(0, -0.1f, 0), new Vector3(40, 0.2f, 40), grass));

            var references = new GameObject("Scale References").transform;
            Place(cubePrefab, references, new Vector3(-8, 0, -4));
            Place(humanPrefab, references, new Vector3(-6, 0, -4));
            Place(doorPrefab, references, new Vector3(-2, 0, -3));
            Place(windowPrefab, references, new Vector3(-8, 0, 3));
            Place(housePrefab, references, new Vector3(6, 0, 6));
            Place(groundPrefab, null, Vector3.zero);

            var geometry = new GameObject("Navigation Test Geometry").transform;
            Box(geometry, "Obstacle 3x2x2m", new Vector3(-2, 1, 5), new Vector3(3, 2, 2), stone);
            var narrow = new GameObject("Narrow Gate - 0.40m clearance");
            narrow.transform.SetParent(geometry, false);
            narrow.transform.localPosition = new Vector3(-6, 0, -9);
            Box(narrow.transform, "Left post", new Vector3(-0.3f, 1.2f, 0), new Vector3(0.2f, 2.4f, 0.25f), timber);
            Box(narrow.transform, "Right post", new Vector3(0.3f, 1.2f, 0), new Vector3(0.2f, 2.4f, 0.25f), timber);
            var street = Box(geometry, "Street marker 4x12m - no collider", new Vector3(-6, 0.005f, 8), new Vector3(4, 0.01f, 12), road);
            Object.DestroyImmediate(street.GetComponent<Collider>());

            var cameraObject = new GameObject("Inspection Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            camera.orthographic = true;
            camera.orthographicSize = 23;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 150;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.10f, 0.14f, 0.19f);
            camera.allowHDR = false;
            camera.transform.position = new Vector3(24, 26, -24);
            camera.transform.LookAt(new Vector3(0, 0.5f, 0));
            var sun = new GameObject("Directional Light").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(50, -30, 0);
            RenderSettings.sun = sun;
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.48f, 0.52f, 0.60f);
            AssetDatabase.SaveAssets();
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new IOException("Unity could not save the development scene.");
            Debug.Log("AETHERNIA_SANDBOX_CREATED: " + ScenePath + "; 6 metric prefabs; 7 URP materials.");
        }

        private static Material Material(string name, Color color, Shader shader)
        {
            var material = new Material(shader) { name = "Development_" + name };
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0.15f);
            AssetDatabase.CreateAsset(material, MaterialFolder + "/" + material.name + ".mat");
            return material;
        }

        private static GameObject Save(string name, Action<GameObject> populate)
        {
            var root = new GameObject(name);
            try
            {
                populate(root);
                var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabFolder + "/" + name + ".prefab");
                if (prefab == null) throw new IOException("Could not save prefab " + name);
                return prefab;
            }
            finally { Object.DestroyImmediate(root); }
        }

        private static GameObject Box(Transform parent, string name, Vector3 centre, Vector3 size, Material material)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent, false);
            box.transform.localPosition = centre;
            box.transform.localScale = size;
            box.GetComponent<Renderer>().sharedMaterial = material;
            return box;
        }

        private static void Place(GameObject prefab, Transform parent, Vector3 position)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
        }
    }
}
