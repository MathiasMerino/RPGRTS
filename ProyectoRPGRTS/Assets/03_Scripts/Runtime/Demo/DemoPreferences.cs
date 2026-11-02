using System;
using UnityEngine;

namespace Aethernia
{
    [Serializable]
    public sealed class DemoPreferences
    {
        public float Master = .8f;
        public float Effects = .85f;
        public float CameraSpeed = 12;
        public int Width = 1280, Height = 720, Quality = 1;
        public bool Fullscreen;
        private const string Key = "Aethernia.Demo.Options.v1";
        internal static string SessionStorageKey;
        [NonSerialized] public string StorageKey=Key;
        public static DemoPreferences Load(string storageKey=null)
        {
            storageKey=storageKey??SessionStorageKey??Key;
            DemoPreferences p;
            try { p = JsonUtility.FromJson<DemoPreferences>(PlayerPrefs.GetString(storageKey, "")); }
            catch (ArgumentException) { p = null; }
            if (p == null) p = new DemoPreferences();
            p.StorageKey=storageKey;
            p.Master = Safe(p.Master, 0, 1, .8f); p.Effects = Safe(p.Effects, 0, 1, .85f);
            p.CameraSpeed = Safe(p.CameraSpeed, 4, 24, 12);
            if (p.Width < 800 || p.Width > 7680 || p.Height < 600 || p.Height > 4320) { p.Width = 1280; p.Height = 720; }
            p.Quality = Mathf.Clamp(p.Quality, 0, Mathf.Max(0, QualitySettings.names.Length - 1));
            return p;
        }
        private static float Safe(float x, float min, float max, float fallback) => float.IsNaN(x) || float.IsInfinity(x) ? fallback : Mathf.Clamp(x, min, max);
        public void Apply(CombatAudioPlayer audio, IsometricCameraRig camera, bool display)
        {
            AudioListener.volume = Master;
            if (audio != null) audio.EffectsGain = Effects;
            if (camera != null) camera.SetPanSpeed(CameraSpeed);
            QualitySettings.SetQualityLevel(Quality, true);
            if (display && !Application.isEditor) Screen.SetResolution(Width, Height, Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }
        public void Save() { PlayerPrefs.SetString(StorageKey, JsonUtility.ToJson(this)); PlayerPrefs.Save(); }
    }
}
