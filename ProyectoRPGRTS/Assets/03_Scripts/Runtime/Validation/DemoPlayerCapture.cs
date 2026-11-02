using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aethernia
{
    // Solo validación explícita; captura todos los Canvas activos, incluido el menú sin HUD.
    public static class DemoPlayerCapture
    {
        public static IEnumerator Save(string file)
        {
            if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null)yield break;
            var camera=Camera.main;
            var canvases=Object.FindObjectsByType<Canvas>().Where(c=>c.isRootCanvas&&c.enabled&&c.renderMode!=RenderMode.WorldSpace).ToArray();
            var modes=canvases.Select(c=>c.renderMode).ToArray();var cameras=canvases.Select(c=>c.worldCamera).ToArray();var distances=canvases.Select(c=>c.planeDistance).ToArray();
            var old=camera.targetTexture;var target=RenderTexture.GetTemporary(1280,720,24);Texture2D pixels=null;
            try
            {
                camera.targetTexture=target;foreach(var canvas in canvases){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=2;}
                yield return null;yield return null;yield return null;Canvas.ForceUpdateCanvases();
                RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                var previous=RenderTexture.active;
                try
                {
                    RenderTexture.active=target;pixels=new Texture2D(1280,720,TextureFormat.RGB24,false);pixels.ReadPixels(new Rect(0,0,1280,720),0,0);pixels.Apply();
                    var data=pixels.GetPixels32();var min=255;var max=0;for(var i=0;i<data.Length;i+=97){min=Mathf.Min(min,data[i].r,data[i].g,data[i].b);max=Mathf.Max(max,data[i].r,data[i].g,data[i].b);}
                    if(max-min<5)throw new System.InvalidOperationException("Captura nativa vacía");
                    Directory.CreateDirectory(Path.GetDirectoryName(file));File.WriteAllBytes(file,pixels.EncodeToPNG());
                }
                finally{RenderTexture.active=previous;}
            }
            finally
            {
                camera.targetTexture=old;for(var i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}
                if(pixels!=null)Object.Destroy(pixels);RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
