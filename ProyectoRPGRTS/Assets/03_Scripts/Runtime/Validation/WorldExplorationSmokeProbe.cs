using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    public sealed class WorldExplorationSmokeProbe:MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install()
        {var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"-aetherniaUx02Smoke")<0)return;var node=new GameObject("Explicit UX02 Windows validation");node.AddComponent<WorldExplorationSmokeProbe>();if(Array.IndexOf(args,"-demoRecord")>=0)node.AddComponent<DemoRecordingProbe>();}
        private IEnumerator Start()
        {
            Application.runInBackground=true;var args=Environment.GetCommandLineArgs();var slot=args[Array.IndexOf(args,"-aetherniaUx02Smoke")+1];var cls=args[Array.IndexOf(args,"-demoClass")+1];var race=args[Array.IndexOf(args,"-demoRace")+1];var resume=Array.IndexOf(args,"-demoResume")>=0;Directory.CreateDirectory(Path.GetDirectoryName(slot));
            yield return null;yield return null;var flow=FindAnyObjectByType<DemoFlow>();var stack=new Stack<IEnumerator>();stack.Push(resume?Resume(flow,slot,cls,race):WorldExplorationRoute.Run(flow,slot,cls,race,name=>DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(slot),"Captures",cls+"-"+name+".png"))));
            var start=Time.realtimeSinceStartup;
            while(stack.Count>0)
            {
                object step;try{var top=stack.Peek();if(!top.MoveNext()){stack.Pop();continue;}step=top.Current;if(step is IEnumerator nested){stack.Push(nested);continue;}}
                catch(Exception e){File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"FAIL: "+e);Debug.LogError(e);GetComponent<DemoRecordingProbe>()?.Finish();Application.Quit(2);yield break;}yield return step;
            }
            GetComponent<DemoRecordingProbe>()?.Finish();File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"PASS: UX02 "+race+"/"+cls+" "+(resume?"new process Continue exact identity, health, resources, buildings, army, quest and tutorial checkpoints":"menu/creation/intro/safe exploration/WASD/E resident/quest/story RPG/unique reward/real walk to military camp/single and multi selection/two actual formation arrivals/explicit attack/real victory/paid construction/production/recruitment/save/title")+"; seconds="+(Time.realtimeSinceStartup-start).ToString("F2"));flow.Front.Controls["quit"].onClick.Invoke();
        }
        private static IEnumerator Resume(DemoFlow flow,string slot,string cls,string race)
        {
            UxExperienceRoute.Require(PrototypeSaveStore.Load(slot,flow.Session.Rules,out var before),"read saved native slot");flow.Front.Controls["continue"].onClick.Invoke();flow.Pause();var after=flow.Session.State;
            UxExperienceRoute.Require(after.RaceId==race&&after.ClassId==cls&&after.Health==before.Health&&after.Gold==before.Gold&&after.Wood==before.Wood&&after.Food==before.Food,"exact identity, health and resources");
            UxExperienceRoute.Require(after.Demo.Chapter==DemoChapter.FreeExploration&&LostCargoQuest.Stage(after)==CargoQuestStage.Completed&&after.Buildings.Count==before.Buildings.Count&&after.Army.Single().Id==before.Army.Single().Id,"quest, buildings and army retained without duplicated rewards");UxExperienceRoute.Require(after.Facts.Where(f=>f.StartsWith("ux02-")).OrderBy(f=>f).SequenceEqual(before.Facts.Where(f=>f.StartsWith("ux02-")).OrderBy(f=>f))&&after.Companions.Select(c=>c.Id).SequenceEqual(before.Companions.Select(c=>c.Id)),"exact world discoveries, unique rewards and companions");flow.Resume();yield return DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(slot),"Captures",cls+"-23-Continued.png"));UxExperienceRoute.Require(flow.ReturnToTitle(),"return after Continue");
        }
    }
}
