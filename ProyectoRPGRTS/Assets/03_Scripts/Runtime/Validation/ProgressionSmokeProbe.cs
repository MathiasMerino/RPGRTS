using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace Aethernia
{
    // Explicit QA only. Composes existing legal-action runners; no artificial rewards or health.
    public sealed class ProgressionSmokeProbe:MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install()
        {var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"-aetherniaProgressionSmoke")<0)return;var go=new GameObject("Explicit E18 integration validation");go.AddComponent<ProgressionSmokeProbe>();if(Array.IndexOf(args,"-demoRecord")>=0)go.AddComponent<DemoRecordingProbe>();}
        private IEnumerator Start()
        {
            Application.runInBackground=true;var args=Environment.GetCommandLineArgs();var slot=args[Array.IndexOf(args,"-aetherniaProgressionSmoke")+1];var resume=Array.IndexOf(args,"-demoResume")>=0;var index=Array.IndexOf(args,"-demoClass");var id=index<0?"warrior":args[index+1];Directory.CreateDirectory(Path.GetDirectoryName(slot));
            var demo=gameObject.AddComponent<DemoSmokeProbe>();demo.enabled=false;var expedition=gameObject.AddComponent<ExpeditionSmokeProbe>();expedition.enabled=false;
            var stack=new Stack<IEnumerator>();stack.Push(Run(demo,expedition,slot,id,resume));var start=Time.realtimeSinceStartup;
            while(stack.Count>0)
            {object step;try{var top=stack.Peek();if(!top.MoveNext()){stack.Pop();continue;}step=top.Current;if(step is IEnumerator nested){stack.Push(nested);continue;}}catch(Exception e){File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"FAIL: "+e);Debug.LogError(e);GetComponent<DemoRecordingProbe>()?.Finish();Application.Quit(2);yield break;}yield return step;}
            GetComponent<DemoRecordingProbe>()?.Finish();File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"PASS: E18 "+id+" "+(resume?"new process exact persistent state, no duplicate rewards":"new game/intro/RPG/XP/level/skill/patrol/camp/quest/paid companions/two guardianies/companionXP/loot/workshop2/forge/equipment/paid army/training/conquest/territorial production/save/title")+"; seconds="+(Time.realtimeSinceStartup-start).ToString("F2")+"; automatic duration is not human playtime");var flow=FindAnyObjectByType<DemoFlow>();flow.Front.Controls["quit"].onClick.Invoke();
        }
        private static IEnumerator Run(DemoSmokeProbe demo,ExpeditionSmokeProbe expedition,string slot,string id,bool resume)
        {
            if(!resume)yield return demo.Validate(slot,id,false);yield return expedition.Validate(slot,resume);var flow=FindAnyObjectByType<DemoFlow>();
            if(!PrototypeSaveStore.Load(slot,flow.Session.Rules,out var s)||s.HeroLevel!=4||s.Experience!=60||s.Weapon!=EquipmentOperations.AdvancedWeapon(id)||s.Accessory!="pilgrim-charm"||!s.Facts.Contains(EquipmentOperations.LootClaimed)||!s.Facts.Contains("umbral-outpost-conquered")||s.Companions.Any(c=>c.Level!=2)||s.Skills.Sum(x=>x.Rank)<1)throw new InvalidOperationException("E18 complete persistent progression contract");
        }
    }
}
