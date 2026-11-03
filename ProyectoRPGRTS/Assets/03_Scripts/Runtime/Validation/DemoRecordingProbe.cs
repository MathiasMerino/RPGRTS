using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Aethernia
{
    // Grabación nativa opcional de QA: frames continuos y muestras del DSP, sin componer poses.
    [DefaultExecutionOrder(10000)] public sealed class DemoRecordingProbe : MonoBehaviour
    {
        private string directory;private RenderTexture target;private Texture2D image;private DemoFlow flow;
        private int oldRate,frames,channels,rate;private bool recording,finished;
        private readonly List<float> audio=new List<float>();private readonly StringBuilder timeline=new StringBuilder();
        private void Start()
        {
            var args=Environment.GetCommandLineArgs();var index=Array.IndexOf(args,"-aetherniaDemoSmoke");if(index<0)index=Array.IndexOf(args,"-aetherniaExpeditionSmoke");if(index<0)index=Array.IndexOf(args,"-aetherniaProgressionSmoke");if(index<0)index=Array.IndexOf(args,"-aetherniaCivilizationSmoke");if(index<0)index=Array.IndexOf(args,"-aetherniaUx01Smoke");var slot=args[index+1];
            directory=Path.Combine(Path.GetDirectoryName(slot),"Recording");Directory.CreateDirectory(directory);
            if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null){File.WriteAllText(Path.Combine(directory,"proof.txt"),"UNAVAILABLE: graphics device Null");enabled=false;return;}
            flow=FindAnyObjectByType<DemoFlow>();oldRate=Time.captureFramerate;Time.captureFramerate=12;
            channels=AudioSettings.speakerMode==AudioSpeakerMode.Mono?1:2;rate=AudioSettings.outputSampleRate;recording=AudioRenderer.Start();
            target=RenderTexture.GetTemporary(1280,720,24);image=new Texture2D(1280,720,TextureFormat.RGB24,false);
        }
        private void LateUpdate()
        {
            if(finished||target==null||flow==null)return;
            var camera=Camera.main;var oldTarget=camera.targetTexture;
            var canvases=FindObjectsByType<Canvas>().Where(c=>c.isRootCanvas&&c.enabled&&c.renderMode!=RenderMode.WorldSpace).ToArray();
            var modes=canvases.Select(c=>c.renderMode).ToArray();var cameras=canvases.Select(c=>c.worldCamera).ToArray();var distances=canvases.Select(c=>c.planeDistance).ToArray();
            var previous=RenderTexture.active;
            try
            {
                camera.targetTexture=target;foreach(var canvas in canvases){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=2;}
                Canvas.ForceUpdateCanvases();RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                RenderTexture.active=target;image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();File.WriteAllBytes(Path.Combine(directory,"frame-"+frames.ToString("D5")+".jpg"),image.EncodeToJPG(88));
                if(recording)
                {
                    var count=AudioRenderer.GetSampleCountForCaptureFrame();
                    if(count>0)using(var buffer=new NativeArray<float>(count*channels,Allocator.Temp))
                    {if(!AudioRenderer.Render(buffer))throw new InvalidOperationException("Native DSP render failed");for(var i=0;i<buffer.Length;i++)audio.Add(buffer[i]);}
                }
                timeline.AppendLine(frames+"|"+Time.time.ToString("F3")+"|"+flow.Front.Screen+"|"+flow.Session.Mode+"|"+flow.Session.State.Demo?.Chapter+"|hp="+flow.Session.State.Health+"|enemies="+flow.Director.Rts.Enemies);frames++;
            }
            finally
            {
                RenderTexture.active=previous;camera.targetTexture=oldTarget;for(var i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}
            }
        }
        public void Finish()
        {
            if(finished)return;finished=true;if(recording)AudioRenderer.Stop();Time.captureFramerate=oldRate;
            if(directory!=null)
            {
                var sum=0d;var peak=0f;foreach(var value in audio){sum+=value*value;peak=Mathf.Max(peak,Mathf.Abs(value));}
                var rms=audio.Count==0?0:Math.Sqrt(sum/audio.Count);
                if(recording)WriteWav(Path.Combine(directory,"Native-DSP.wav"));
                File.WriteAllText(Path.Combine(directory,"timeline.txt"),timeline.ToString());
                File.WriteAllText(Path.Combine(directory,"proof.txt"),(frames>12&&rms>.0001?"PASS: ":"INCOMPLETE: ")+frames+" continuous native Windows Game camera frames at fixed capture step12Hz; audio DSP samples="+audio.Count+" channels="+channels+" rate="+rate+" rms="+rms.ToString("F6")+" peak="+peak.ToString("F6")+". Capture step is not measured FPS. Gameplay actions are those of the explicit demo smoke; no pose sampling or fabricated results.");
            }
            if(target!=null)RenderTexture.ReleaseTemporary(target);if(image!=null)Destroy(image);target=null;image=null;
        }
        private void WriteWav(string path)
        {
            using(var writer=new BinaryWriter(File.Create(path)))
            {
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+audio.Count*2);writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)channels);writer.Write(rate);writer.Write(rate*channels*2);writer.Write((short)(channels*2));writer.Write((short)16);writer.Write(Encoding.ASCII.GetBytes("data"));writer.Write(audio.Count*2);
                foreach(var value in audio)writer.Write((short)Mathf.RoundToInt(Mathf.Clamp(value,-1,1)*32767));
            }
        }
        private void OnDisable(){Finish();}
    }
}
