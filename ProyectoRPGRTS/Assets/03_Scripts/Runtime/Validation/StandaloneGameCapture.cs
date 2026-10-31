using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aethernia
{
    // Only called by the explicit smoke argument, never by normal gameplay.
    public static class StandaloneGameCapture
    {
        public static IEnumerator Save(string file)
        {
            var camera=Camera.main;var canvas=Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>();
            var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;var oldDistance=canvas.planeDistance;var oldTarget=camera.targetTexture;
            var target=RenderTexture.GetTemporary(1280,720,24);Texture2D pixels=null;
            try
            {
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=2;camera.targetTexture=target;
                yield return null;yield return null;yield return null;Canvas.ForceUpdateCanvases();
                // Hidden/batch players may skip automatic frame rendering. Request this real camera explicitly.
                RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                var oldActive=RenderTexture.active;
                try
                {
                    RenderTexture.active=target;pixels=new Texture2D(1280,720,TextureFormat.RGB24,false);pixels.ReadPixels(new Rect(0,0,1280,720),0,0);pixels.Apply();
                    var data=pixels.GetPixels32();var minimum=255;var maximum=0;for(var i=0;i<data.Length;i+=97){minimum=Mathf.Min(minimum,data[i].r,data[i].g,data[i].b);maximum=Mathf.Max(maximum,data[i].r,data[i].g,data[i].b);}
                    if(maximum-minimum<5)throw new System.InvalidOperationException("Standalone capture is empty; cannot claim visual verification.");
                    File.WriteAllBytes(file,pixels.EncodeToPNG());
                }
                finally{RenderTexture.active=oldActive;}
            }
            finally
            {
                camera.targetTexture=oldTarget;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.planeDistance=oldDistance;
                if(pixels!=null)Object.Destroy(pixels);RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
