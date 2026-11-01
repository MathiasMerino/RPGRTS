using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using Unity.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class TacticalCombatRecordingTests
    {
        [UnityTest]public IEnumerator RealCombatVideoFramesAndNativeDsp()=>Record();
        private static IEnumerator Record()
        {
            EditorSceneManager.OpenScene(AnimatedCharacterBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;
            Directory.CreateDirectory("Logs/E09/Recording");var d=Object.FindAnyObjectByType<PrototypeDirector>();d.Session.SelectClass("warrior");
            Assert.That(d.Hero.Warp(new Vector3(-3.4f,0,-5)),Is.True);yield return null;Assert.That(GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>().TryInteract(d.Hero,d.Session),Is.True);Assert.That(d.Execute("sword")&&d.Execute("armour"),Is.True);
            Assert.That(d.Hero.Warp(new Vector3(-11.8f,0,1.4f)),Is.True);yield return null;
            Assert.That(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.left*1.2f,42),Is.True);yield return TacticalCombatIntegrationTests.Wait(.4f);
            var camera=Camera.main;var rig=camera.GetComponent<IsometricCameraRig>();rig.enabled=false;camera.orthographicSize=3.8f;var focus=(d.Hero.transform.position+d.Encounter.EnemyPosition)*.5f+Vector3.up*.8f;camera.transform.position=focus+new Vector3(5,7,6);camera.transform.LookAt(focus);
            var canvas=Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>();var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;var oldDistance=canvas.planeDistance;var oldTarget=camera.targetTexture;var target=RenderTexture.GetTemporary(960,540,24);
            var image=new Texture2D(960,540,TextureFormat.RGB24,false);var oldRate=Time.captureFramerate;var audio=new List<float>();var timeline=new StringBuilder();var recording=false;var channels=AudioSettings.speakerMode==AudioSpeakerMode.Mono?1:2;var rate=AudioSettings.outputSampleRate;
            try
            {
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=2;camera.targetTexture=target;yield return null;yield return null;yield return null;
                Time.captureFramerate=12;recording=AudioRenderer.Start();Assert.That(recording,Is.True,"El motor debe entrar en captura DSP real");
                var endAfter=int.MaxValue;
                for(var frame=0;frame<120;frame++)
                {
                    if(d.Encounter.PlayerCanAct&&d.Encounter.Battle.ActionAvailable)
                    {var sequence=d.Encounter.Battle.LastHeroResolution.Sequence;d.Execute("attack");Assert.That(d.Encounter.Battle.LastHeroResolution.Sequence,Is.GreaterThan(sequence));endAfter=frame+12;}
                    if(d.Encounter.PlayerCanAct&&!d.Encounter.Battle.ActionAvailable&&frame>=endAfter)d.Execute("end");
                    yield return null;Canvas.ForceUpdateCanvases();
                    var active=RenderTexture.active;RenderTexture.active=target;image.ReadPixels(new Rect(0,0,960,540),0,0);image.Apply();RenderTexture.active=active;
                    File.WriteAllBytes("Logs/E09/Recording/frame-"+frame.ToString("D3")+".jpg",image.EncodeToJPG(90));
                    var count=AudioRenderer.GetSampleCountForCaptureFrame();
                    if(count>0)
                    {
                        using(var buffer=new NativeArray<float>(count*channels,Allocator.Temp))
                        {Assert.That(AudioRenderer.Render(buffer),Is.True);for(var i=0;i<buffer.Length;i++)audio.Add(buffer[i]);}
                    }
                    var battle=d.Encounter.Battle;timeline.AppendLine(frame+"|"+Time.time.ToString("F3")+"|"+(battle==null?"none":battle.Phase.ToString())+"|"+d.Session.State.Health+"|"+(battle==null?-1:battle.EnemyHealth));
                }
                File.WriteAllText("Logs/E09/Recording/timeline.txt",timeline.ToString());
                Assert.That(d.Encounter.Battle.EnemyHealth,Is.LessThan(18),"La toma debe incluir daño real del jugador, no solo comandos bloqueados");
                Assert.That(d.Session.State.Health,Is.LessThan(24),"La toma debe incluir respuesta enemiga real");
                Assert.That(audio.Count,Is.GreaterThan(rate));var energy=0d;var peak=0f;foreach(var value in audio){energy+=value*value;peak=Mathf.Max(peak,Mathf.Abs(value));}
                var rms=System.Math.Sqrt(energy/audio.Count);Assert.That(rms,Is.GreaterThan(.0001),"Audio no silencioso debe proceder del DSP, no solo de un banco de clips");
                Wav("Logs/E09/Recording/Native-DSP.wav",audio,rate,channels);
                File.WriteAllText("Logs/E09/Recording/timeline.txt",timeline.ToString());File.WriteAllText("Logs/E09/Recording/proof.txt","PASS: 120 real Play Mode frames at fixed capture step 12 Hz, authoritative commands/dice/enemy phase; native AudioRenderer DSP samples="+audio.Count+" channels="+channels+" rate="+rate+" RMS="+rms.ToString("F6")+" peak="+peak.ToString("F6")+". Capture rate is not measured FPS. No clip pose sampling.");
            }
            finally
            {
                if(recording)AudioRenderer.Stop();Time.captureFramerate=oldRate;camera.targetTexture=oldTarget;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.planeDistance=oldDistance;rig.enabled=true;Object.Destroy(image);RenderTexture.ReleaseTemporary(target);
            }
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static void Wav(string path,List<float> samples,int rate,int channels)
        {
            using(var writer=new BinaryWriter(File.Create(path)))
            {
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+samples.Count*2);writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)channels);writer.Write(rate);writer.Write(rate*channels*2);writer.Write((short)(channels*2));writer.Write((short)16);writer.Write(Encoding.ASCII.GetBytes("data"));writer.Write(samples.Count*2);
                foreach(var value in samples)writer.Write((short)Mathf.RoundToInt(Mathf.Clamp(value,-1,1)*32767));
            }
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
