using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Aethernia
{
    // Explicit native benchmark only; absent from normal gameplay and smoke recordings.
    public sealed class AlphaBenchmarkProbe:MonoBehaviour
    {
        [Serializable]private sealed class Metric{public string name,unit;public bool available;public double mean,p95,maximum;}
        [Serializable]private sealed class Row{public string workload,quality;public int width,height,frames,renderSubmissions;public double seconds,meanFrameMs,p95FrameMs;public List<Metric> counters=new List<Metric>();}
        [Serializable]private sealed class Report{public string unity,cpu,gpu,driver,os,status="RUNNING",limitations="Development batch player with explicit offscreen URP render requests including UI and live preview. CPU frame deltas include submissions, not display FPS or GPU time. Fixed paused village/menu/RTS, 2s warmup; no readback or capture during samples. No large-army extrapolation.";public int memoryMB;public List<Row> rows=new List<Row>();}
        private string output;
        private bool complete;
        private Camera mainCamera;
        private Camera[] previewCameras;
        private RenderTexture renderTarget;
        private int submissions;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install()
        {if(Array.IndexOf(Environment.GetCommandLineArgs(),"-aetherniaAlphaBenchmark")>=0)new GameObject("Explicit alpha performance measurement").AddComponent<AlphaBenchmarkProbe>();}
        private IEnumerator Start()
        {
            yield return null;yield return null;Application.runInBackground=true;QualitySettings.vSyncCount=0;Application.targetFrameRate=-1;
            var args=Environment.GetCommandLineArgs();output=args[Array.IndexOf(args,"-aetherniaAlphaBenchmark")+1];Directory.CreateDirectory(Path.GetDirectoryName(output));
            var baselineIndex=Array.IndexOf(args,"-benchmarkBaseline");if(baselineIndex<0)throw new InvalidOperationException("Benchmark needs an existing real E13 save.");var baseline=args[baselineIndex+1];var flow=FindAnyObjectByType<DemoFlow>();flow.SlotPath=output+".slot.json";File.Copy(baseline,flow.SlotPath,false);flow.Director.SavePathOverride=flow.SlotPath;
            if(!flow.Continue())throw new InvalidOperationException("Benchmark could not continue baseline.");flow.Director.Hero.Stop();flow.Session.Paused=true;
            var report=new Report{unity=Application.unityVersion,cpu=SystemInfo.processorType,gpu=SystemInfo.graphicsDeviceName,driver=SystemInfo.graphicsDeviceVersion,os=SystemInfo.operatingSystem,memoryMB=SystemInfo.systemMemorySize};
            var names=new[]{"Draw Calls Count","Batches Count","SetPass Calls Count","Triangles Count","GC Allocated In Frame","Main Thread"};var units=new[]{"count","count","count","count","bytes","nanoseconds"};
            for(var workload=0;workload<3;workload++)
            {
                if(workload==1)flow.ShowMain();
                if(workload==2){if(!flow.Continue())throw new InvalidOperationException("RTS benchmark could not continue baseline.");flow.Session.Paused=false;if(!flow.BeginPatrol())throw new InvalidOperationException("RTS benchmark could not deploy.");flow.Session.Paused=true;}
                yield return null;mainCamera=Camera.main;previewCameras=FindObjectsByType<Camera>().Where(c=>c!=mainCamera&&c.enabled&&c.targetTexture!=null).ToArray();
                foreach(var canvas in FindObjectsByType<Canvas>())if(canvas.isRootCanvas&&canvas.renderMode!=RenderMode.WorldSpace){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=mainCamera;canvas.planeDistance=2;}
                foreach(var width in new[]{1280,1920})
                {
                    Screen.SetResolution(width,width*9/16,FullScreenMode.Windowed);var resolutionLimit=Time.realtimeSinceStartup+8;while((Screen.width!=width||Screen.height!=width*9/16)&&Time.realtimeSinceStartup<resolutionLimit)yield return null;
                    if(renderTarget!=null){renderTarget.Release();Destroy(renderTarget);}renderTarget=new RenderTexture(Screen.width,Screen.height,24,RenderTextureFormat.DefaultHDR);renderTarget.Create();
                    for(var quality=0;quality<QualitySettings.names.Length;quality++)
                    {
                        QualitySettings.SetQualityLevel(quality,true);QualitySettings.vSyncCount=0;Application.targetFrameRate=-1;var warm=Time.realtimeSinceStartup+2;while(Time.realtimeSinceStartup<warm)yield return null;
                        var row=new Row{workload=workload==0?"Paused village / real E13 state":workload==1?"Main menu / animated 3D preview":"Paused RTS / real deployment and selection UI",quality=QualitySettings.names[quality],width=Screen.width,height=Screen.height};
                        var recorders=new ProfilerRecorder[names.Length];var values=new List<double>[names.Length];var deltas=new List<double>(2048);
                        try
                        {
                            for(var c=0;c<names.Length;c++){values[c]=new List<double>(2048);try{recorders[c]=ProfilerRecorder.StartNew(c<4?ProfilerCategory.Render:c==4?ProfilerCategory.Memory:ProfilerCategory.Internal,names[c],1);}catch(InvalidOperationException){recorders[c]=default;}}
                            var start=Time.realtimeSinceStartupAsDouble;var beforeSubmissions=submissions;
                            do{yield return null;deltas.Add(Time.unscaledDeltaTime*1000d);for(var c=0;c<names.Length;c++)if(recorders[c].Valid)values[c].Add(recorders[c].LastValue);}while((Time.realtimeSinceStartupAsDouble-start<3||deltas.Count<120)&&Time.realtimeSinceStartupAsDouble-start<12);
                            row.seconds=Time.realtimeSinceStartupAsDouble-start;row.frames=deltas.Count;row.renderSubmissions=submissions-beforeSubmissions;row.meanFrameMs=Mean(deltas);row.p95FrameMs=Percentile(deltas);
                            for(var c=0;c<names.Length;c++){var v=values[c];var available=recorders[c].Valid&&v.Count>0;row.counters.Add(new Metric{name=names[c],unit=units[c],available=available,mean=available?Mean(v):0,p95=available?Percentile(v):0,maximum=available?v[v.Count-1]:0});}
                        }
                        finally{foreach(var recorder in recorders)recorder.Dispose();}
                        if(row.renderSubmissions<row.frames-1||!row.counters.Exists(m=>m.available&&m.unit=="count"&&m.mean>0))throw new InvalidOperationException("No verified render work in performance row.");
                        report.rows.Add(row);File.WriteAllText(output,JsonUtility.ToJson(report,true));Debug.Log("E17_BENCHMARK "+row.workload+" "+row.quality+" "+row.width+" frames="+row.frames+" meanMs="+row.meanFrameMs.ToString("F2"));
                        // Capture after sampling, never counted as a normal frame performance sample.
                        if(workload==0&&width==1280)yield return DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(output),"Quality-"+quality+".png"));
                    }
                }
            }
            report.status="PASS";File.WriteAllText(output,JsonUtility.ToJson(report,true));complete=true;Application.Quit(0);
        }
        private void LateUpdate()
        {
            if(renderTarget==null||mainCamera==null)return;
            foreach(var camera in previewCameras)if(camera!=null&&camera.isActiveAndEnabled&&camera.targetTexture!=null)RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=camera.targetTexture});
            RenderPipeline.SubmitRenderRequest(mainCamera,new UniversalRenderPipeline.SingleCameraRequest{destination=renderTarget});submissions++;
        }
        private void OnDestroy(){if(renderTarget!=null){renderTarget.Release();Destroy(renderTarget);}}
        private static double Mean(List<double> values){var sum=0d;foreach(var v in values)sum+=v;return values.Count==0?0:sum/values.Count;}
        private static double Percentile(List<double> values){values.Sort();return values.Count==0?0:values[Mathf.Clamp(Mathf.CeilToInt(values.Count*.95f)-1,0,values.Count-1)];}
        private void OnApplicationQuit(){if(!complete&&output!=null)Debug.LogError("E17 benchmark incomplete; do not report it as passed.");}
    }
}
