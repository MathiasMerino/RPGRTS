using System.Collections;
using System.IO;
using UnityEngine;

namespace Aethernia.Tests.Editor
{
    internal static class SceneCapture
    {
        public static IEnumerator Save(string file, int width = 1280, int height = 720, System.Action<int, int> validate = null)
        {
            var canvas = Object.FindAnyObjectByType<Aethernia.VerticalSliceHud>().GetComponent<Canvas>();
            var camera = Camera.main; var oldMode = canvas.renderMode; var oldCamera = canvas.worldCamera; var oldDistance = canvas.planeDistance;
            var previous = camera.targetTexture; var target = RenderTexture.GetTemporary(width, height, 24); Texture2D image = null;
            try
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 2; camera.targetTexture = target;
                yield return null; yield return null; yield return null;
                Canvas.ForceUpdateCanvases();
                var active = RenderTexture.active; RenderTexture.active = target;
                image = new Texture2D(width, height, TextureFormat.RGB24, false); image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply(); RenderTexture.active = active;
                File.WriteAllBytes(file, image.EncodeToPNG());
                validate?.Invoke(width, height);
            }
            finally
            {
                camera.targetTexture = previous; canvas.renderMode = oldMode; canvas.worldCamera = oldCamera; canvas.planeDistance = oldDistance;
                if (image != null) Object.DestroyImmediate(image); RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
