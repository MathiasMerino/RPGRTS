using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Aethernia
{
    public sealed class WorldExplorationSmokeProbe:MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install()
        {var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"-aetherniaUx02Smoke")<0&&Array.IndexOf(args,"-aetherniaE20Smoke")<0)return;var node=new GameObject("Explicit UX02 Windows validation");node.AddComponent<WorldExplorationSmokeProbe>();if(Array.IndexOf(args,"-demoRecord")>=0)node.AddComponent<DemoRecordingProbe>();}
        private IEnumerator Start()
        {
            Application.runInBackground=true;var args=Environment.GetCommandLineArgs();var tactical=Array.IndexOf(args,"-aetherniaE20Smoke");var slot=args[(tactical>=0?tactical:Array.IndexOf(args,"-aetherniaUx02Smoke"))+1];var cls=args[Array.IndexOf(args,"-demoClass")+1];var race=args[Array.IndexOf(args,"-demoRace")+1];var resume=Array.IndexOf(args,"-demoResume")>=0;
            var stabilization=Array.IndexOf(args,"-demoStabilization")>=0;
            Directory.CreateDirectory(Path.GetDirectoryName(slot));
            if(stabilization&&!resume)File.WriteAllText(slot+".balance.csv","stage,time_s,class,race,hp,max_hp,attack,guard,level,xp,gold,wood,food\n");
            yield return null;yield return null;var flow=FindAnyObjectByType<DemoFlow>();var stack=new Stack<IEnumerator>();stack.Push(resume?Resume(flow,slot,cls,race):WorldExplorationRoute.Run(flow,slot,cls,race,name=>Capture(flow,slot,cls,name,stabilization),allowPatrolRetry:!stabilization));
            var start=Time.realtimeSinceStartup;
            while(stack.Count>0)
            {
                object step;try{var top=stack.Peek();if(!top.MoveNext()){stack.Pop();continue;}step=top.Current;if(step is IEnumerator nested){stack.Push(nested);continue;}}
                catch(Exception e){File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"FAIL: "+e);Debug.LogError(e);GetComponent<DemoRecordingProbe>()?.Finish();Application.Quit(2);yield break;}yield return step;
            }
            GetComponent<DemoRecordingProbe>()?.Finish();
            var phase=flow.Session.Rules.CoreRefine?"CORE-REFINE-01":flow.Session.Rules.IntegratedAlpha?"E23":flow.Session.Rules.LivingWorld?"E22":flow.Session.Rules.FrontierCampaign?"E21":stabilization?"E20.1":tactical>=0?"E20":"UX02";
            var coverage=flow.Session.Rules.LivingWorld?"capítulo "+cls+", contratos, comercio, guardia opcional, asentamiento y guardado":
                "menu/creation/intro/safe exploration/WASD/E resident/quest/story RPG/unique reward/real walk to military camp/single and multi selection/two actual formation arrivals/explicit attack/real victory/paid construction/production/recruitment/save/title";
            File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"PASS: "+phase+" "+race+"/"+cls+" "+
                (resume?"new process Continue exact identity, health, resources, buildings, army and quests":coverage)+
                "; seconds="+(Time.realtimeSinceStartup-start).ToString("F2"));
            flow.Front.Controls["quit"].onClick.Invoke();
        }
        private static IEnumerator Capture(DemoFlow flow,string slot,string cls,string name,bool stabilization)
        {
            if(stabilization&&BalanceCheckpoint(name))
            {
                var game=flow.Session;var s=game.State;var profile=game.HeroProfile;
                var damage=HeroActionRules.AttackDamage(profile,HeroActionRules.WeaponBonus(s,game.Rules.Progression?.Items));
                File.AppendAllText(slot+".balance.csv",FormattableString.Invariant(
                    $"{name},{Time.realtimeSinceStartupAsDouble:F2},{s.ClassId},{s.RaceId},{s.Health},{profile.Health},{damage},{HeroActionRules.GuardReduction(profile)},{s.HeroLevel},{s.Experience},{s.Gold},{s.Wood},{s.Food}\n"));
            }
            if(name=="T02-Deployment" && SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null &&
                Array.IndexOf(Environment.GetCommandLineArgs(),"-demoRecord")<0)
            {
                const int frames=120;
                var samples=new double[frames];
                var camera=Camera.main;
                var renders=0;
                void CountRender(ScriptableRenderContext context,Camera rendered)
                {
                    if(rendered==camera)renders++;
                }
                RenderPipelineManager.endCameraRendering+=CountRender;
                try
                {
                    var last=Time.realtimeSinceStartupAsDouble;
                    for(var i=0;i<frames;i++)
                    {
                        yield return null;
                        var now=Time.realtimeSinceStartupAsDouble;
                        samples[i]=now-last;
                        last=now;
                    }
                }
                finally{RenderPipelineManager.endCameraRendering-=CountRender;}
                Array.Sort(samples);
                var elapsed=samples.Sum();
                var rts=flow.Director.Rts;
                File.WriteAllText(slot+".frames.txt",FormattableString.Invariant(
                    $"RTS deployment | {rts.Allies}+{rts.Enemies} units | {Screen.width}x{Screen.height} | {SystemInfo.graphicsDeviceName} | {frames} engine frames, {renders} URP main camera renders | {frames/elapsed:F2} engine Hz | {renders/elapsed:F2} camera renders/s | {samples[113]*1000:F2} ms p95 engine frame | hidden Windows smoke; no GPU presentation, GC or scalability benchmark"));
            }
            yield return DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(slot),"Captures",cls+"-"+name+".png"));
        }
        private static bool BalanceCheckpoint(string name) =>
            name=="04-SafeVillage"||name=="11-FirstRpg"||name=="13-ExploreAgain"||
            name=="18-Positioned"||name=="20-Victory"||name=="22-Camp"||
            name=="T02-Deployment"||name=="T07-Conquered"||name=="T08-Return";

        private static IEnumerator Resume(DemoFlow flow,string slot,string cls,string race)
        {
            UxExperienceRoute.Require(PrototypeSaveStore.Load(slot,flow.Session.Rules,out var before),"read saved native slot");flow.Front.Controls["continue"].onClick.Invoke();flow.Pause();var after=flow.Session.State;
            UxExperienceRoute.Require(after.RaceId==race&&after.ClassId==cls&&after.Health==before.Health&&after.Gold==before.Gold&&after.Wood==before.Wood&&after.Food==before.Food,"exact identity, health and resources");
            UxExperienceRoute.Require(after.Demo.Chapter==DemoChapter.FreeExploration&&LostCargoQuest.Stage(after)==CargoQuestStage.Completed&&after.Buildings.Count==before.Buildings.Count&&after.Army.Select(r=>r.Id+"|"+r.Health+"|"+r.CultureId).SequenceEqual(before.Army.Select(r=>r.Id+"|"+r.Health+"|"+r.CultureId)),"quest, buildings and army retained without duplicated rewards");UxExperienceRoute.Require(after.Facts.Where(f=>f.StartsWith("ux02-")).OrderBy(f=>f).SequenceEqual(before.Facts.Where(f=>f.StartsWith("ux02-")).OrderBy(f=>f))&&after.Companions.Select(c=>c.Id).SequenceEqual(before.Companions.Select(c=>c.Id)),"exact world discoveries, unique rewards and companions");if(flow.Session.Rules.TacticalWarfare)
            {
                var conquest=!flow.Session.Rules.LivingWorld||cls=="warrior";
                UxExperienceRoute.Require(after.Territories.Single(t=>t.Id=="outpost").Owner==(conquest?after.FactionId:"faction-b")&&
                    after.Facts.Count(f=>f=="umbral-outpost-conquered")== (conquest?1:0)&&after.Skills.Count==before.Skills.Count&&
                    after.HeroLevel==before.HeroLevel&&after.Experience==before.Experience&&after.Weapon==before.Weapon&&
                    after.Armour==before.Armour&&after.Accessory==before.Accessory&&after.ArmySelection.SequenceEqual(before.ArmySelection)&&
                    after.Skills.Select(x=>x.Id+"|"+x.Rank).SequenceEqual(before.Skills.Select(x=>x.Id+"|"+x.Rank))&&
                    after.Items.Select(x=>x.Id+"|"+x.Count).SequenceEqual(before.Items.Select(x=>x.Id+"|"+x.Count)),
                    "conquest, gear, progression and deployed roster persisted");
                UxExperienceRoute.Require(!flow.Director.Rts.Active&&!flow.Director.Rts.TacticalDeployment,"orders and combat do not leak across processes");
            }
            if(flow.Session.Rules.FrontierCampaign)
                UxExperienceRoute.Require(FrontierChapter.Step(after)==FrontierStep.Free&&
                    FrontierChapter.Decision(after)==(cls=="mage"?RoadChoice.Repair:RoadChoice.Report)&&
                    after.Facts.OrderBy(f=>f).SequenceEqual(before.Facts.OrderBy(f=>f))&&
                    after.Companions.All(c=>before.Companions.Any(b=>b.Id==c.Id&&b.Health==c.Health&&b.Level==c.Level)),
                    "campaign, exclusive choice, dungeon and companions retained after restart");
            if(flow.Session.Rules.LivingWorld)
            {
                UxExperienceRoute.Require(Enumerable.Range(0,FrontierContracts.Count).Select(FrontierContracts.At)
                    .All(c=>FrontierContracts.Status(after,c)==ContractStatus.Completed&&!FrontierContracts.Claim(after,c)),
                    "E22 contracts and unique rewards persist in a new process");
                UxExperienceRoute.Require(after.Buildings.Any(b=>b.Id=="farm"&&b.Level==2)&&
                    after.Facts.Contains(CivilizationOperations.Pact)==(cls=="mage")&&
                    after.Facts.Contains(FrontierChapter.Independent)==(cls=="monk"),
                    "paid upgrade and distinct E21 ending persisted");
            }
            flow.Resume();yield return DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(slot),"Captures",cls+"-23-Continued.png"));UxExperienceRoute.Require(flow.ReturnToTitle(),"return after Continue");
        }
    }
}
