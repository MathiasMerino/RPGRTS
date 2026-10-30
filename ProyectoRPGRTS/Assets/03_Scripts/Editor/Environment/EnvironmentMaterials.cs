using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Aethernia.Editor.Environment
{
    // Deterministic, original authoring textures; no downloaded source images.
    public static class EnvironmentMaterials
    {
        public const string Root = "Assets/06_Environment/Medieval";
        public static void Folder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var cut = path.LastIndexOf('/'); Folder(path.Substring(0, cut));
            AssetDatabase.CreateFolder(path.Substring(0, cut), path.Substring(cut + 1));
        }
        public static Material Load(string name) => AssetDatabase.LoadAssetAtPath<Material>(Root + "/Materials/" + name + ".mat");
        public static void Create()
        {
            Folder(Root + "/Materials"); Folder(Root + "/Textures"); Folder(Root + "/Meshes");
            Make("Ground", new Color(.22f, .28f, .15f), .08f);
            Make("Earth", new Color(.39f, .30f, .19f), .09f);
            Make("Stone", new Color(.53f, .52f, .46f), .16f);
            Make("Oak", new Color(.34f, .23f, .13f), .19f);
            Make("Plaster", new Color(.73f, .67f, .54f), .08f);
            Make("Slate", new Color(.26f, .30f, .33f), .28f);
            Make("Iron", new Color(.12f, .13f, .125f), .38f);
            Make("Foliage", new Color(.30f, .39f, .17f), .10f);
        }
        private static float Noise(float u, float v, float frequency, float seed)
        {
            var x = u * frequency; var y = v * frequency;
            var a = Mathf.PerlinNoise(x + seed, y + seed);
            var b = Mathf.PerlinNoise(x - frequency + seed, y + seed);
            var c = Mathf.PerlinNoise(x + seed, y - frequency + seed);
            var d = Mathf.PerlinNoise(x - frequency + seed, y - frequency + seed);
            return Mathf.Lerp(Mathf.Lerp(a, b, u), Mathf.Lerp(c, d, u), v);
        }
        private static void Make(string name, Color colour, float smoothness)
        {
            const int size = 512;
            var pixels = new Color[size * size]; var heights = new float[pixels.Length];
            for (var y = 0; y < size; y++) for (var x = 0; x < size; x++)
            {
                var u = (float)x / size; var v = (float)y / size;
                var broad = Noise(u, v, 5, 19); var grain = Noise(u, v, 96, 37);
                var variation = .72f + broad * .5f + grain * .20f;
                var height = .35f + broad * .3f + grain * .12f;
                if (name == "Stone" || name == "Slate")
                {
                    var rows = name == "Stone" ? 8 : 10;
                    var row = Mathf.FloorToInt(v * rows);
                    var bx = Mathf.Repeat(u * (name == "Stone" ? 4 : 7) + (row % 2) * .5f + (broad - .5f) * .035f, 1);
                    var by = Mathf.Repeat(v * rows, 1);
                    var seam = Mathf.Min(Mathf.Min(bx, 1 - bx), Mathf.Min(by, 1 - by));
                    var edge = Mathf.SmoothStep(0, 1, Mathf.Clamp01(seam / .055f));
                    var cell = Mathf.PerlinNoise(Mathf.Floor(u * 4 + (row % 2) * .5f) + 17, row + 11);
                    variation *= Mathf.Lerp(.33f, .82f + cell * .35f, edge);
                    height = edge * (.58f + grain * .12f);
                }
                else if (name == "Oak")
                {
                    var warped = u * 60 + Mathf.Sin(v * Mathf.PI * 4) * .45f + broad * 3;
                    var lines = Mathf.Pow(Mathf.Abs(Mathf.Sin(warped * Mathf.PI)), 12);
                    variation *= .79f + grain * .23f - lines * .2f;
                    var plank = Mathf.Repeat(u * 8, 1);
                    if (Mathf.Min(plank, 1 - plank) < .012f) variation *= .55f;
                    height = .5f + lines * .16f + grain * .10f;
                }
                else if (name == "Ground")
                {
                    colour = Color.Lerp(new Color(.32f, .27f, .17f), new Color(.25f, .34f, .17f), Mathf.SmoothStep(.32f, .69f, broad));
                    variation *= .9f + grain * .24f;
                }
                else if (name == "Foliage")
                {
                    variation *= .65f + v * .4f + grain * .22f;
                    height = Mathf.Abs(u - .5f) < .02f ? .8f : .3f + grain * .1f;
                }
                pixels[y * size + x] = new Color(colour.r * variation, colour.g * variation, colour.b * variation, 1);
                heights[y * size + x] = height;
            }
            var normal = new Color[pixels.Length];
            for (var y = 0; y < size; y++) for (var x = 0; x < size; x++)
            {
                var dx = heights[y * size + (x + 1) % size] - heights[y * size + (x + size - 1) % size];
                var dy = heights[((y + 1) % size) * size + x] - heights[((y + size - 1) % size) * size + x];
                var n = new Vector3(-dx * 2, -dy * 2, 1).normalized;
                normal[y * size + x] = new Color(n.x * .5f + .5f, n.y * .5f + .5f, n.z * .5f + .5f, 1);
            }
            var albedo = Write(name + "_Albedo", pixels, false);
            var bump = Write(name + "_Normal", normal, true);
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit missing.");
            var path = Root + "/Materials/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader) { name = name, enableInstancing = true };
                AssetDatabase.CreateAsset(material, path);
            }
            material.SetTexture("_BaseMap", albedo); material.SetColor("_BaseColor", Color.white);
            material.SetTexture("_BumpMap", bump); material.SetFloat("_BumpScale", name == "Stone" ? .7f : .35f);
            material.EnableKeyword("_NORMALMAP"); material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", name == "Iron" ? .7f : 0);
            if (name == "Ground") material.SetTextureScale("_BaseMap", Vector2.one * 5);
            EditorUtility.SetDirty(material);
        }
        private static Texture2D Write(string name, Color[] pixels, bool normal)
        {
            var path = Root + "/Textures/" + name + ".png";
            var texture = new Texture2D(512, 512, TextureFormat.RGB24, false, normal);
            texture.SetPixels(pixels); texture.Apply(); File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture); AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            importer.sRGBTexture = !normal; importer.mipmapEnabled = true; importer.wrapMode = TextureWrapMode.Repeat;
            importer.maxTextureSize = 512; importer.anisoLevel = 4; importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}
