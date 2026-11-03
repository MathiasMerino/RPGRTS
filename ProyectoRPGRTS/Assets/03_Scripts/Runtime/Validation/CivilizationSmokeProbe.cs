using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace Aethernia
{
    // Explicit native QA: composes previous legal-action loops, then operates real culture pilots.
    public sealed class CivilizationSmokeProbe:MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install()
        {var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"-aetherniaCivilizationSmoke")<0)return;var go=new GameObject("Explicit E19 integration validation");go.AddComponent<CivilizationSmokeProbe>();if(Array.IndexOf(args,"-demoRecord")>=0)go.AddComponent<DemoRecordingProbe>();}
        private IEnumerator Start()
        {
            Application.runInBackground=true;var args=Environment.GetCommandLineArgs();var slot=args[Array.IndexOf(args,"-aetherniaCivilizationSmoke")+1];var resume=Array.IndexOf(args,"-demoResume")>=0;var cls=args[Array.IndexOf(args,"-demoClass")+1];var race=args[Array.IndexOf(args,"-demoRace")+1];Directory.CreateDirectory(Path.GetDirectoryName(slot));
            var demo=gameObject.AddComponent<DemoSmokeProbe>();demo.enabled=false;var expedition=gameObject.AddComponent<ExpeditionSmokeProbe>();expedition.enabled=false;var stack=new Stack<IEnumerator>();stack.Push(Run(demo,expedition,slot,race,cls,resume));var start=Time.realtimeSinceStartup;
            while(stack.Count>0)
            {object step;try{var top=stack.Peek();if(!top.MoveNext()){stack.Pop();continue;}step=top.Current;if(step is IEnumerator nested){stack.Push(nested);continue;}}catch(Exception e){File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"FAIL: "+e);Debug.LogError(e);GetComponent<DemoRecordingProbe>()?.Finish();Application.Quit(2);yield break;}yield return step;}
            GetComponent<DemoRecordingProbe>()?.Finish();File.WriteAllText(slot+(resume?".resume.txt":".start.txt"),"PASS: E19 "+race+"/"+cls+" "+(resume?"new process exact identity, skills, equipment, two cultural units/buildings and conquest retained without duplicate rewards":"new game/race/class/intro/RPG/level/skill/WASD/quest/three companions/two guardianies/loot/forge/training/conquest/territorial production/two paid cultural buildings and units/real RTS deployment/save/title")+"; seconds="+(Time.realtimeSinceStartup-start).ToString("F2")+"; automated duration is not human playtime");FindAnyObjectByType<DemoFlow>().Front.Controls["quit"].onClick.Invoke();
        }
        private static IEnumerator Run(DemoSmokeProbe demo,ExpeditionSmokeProbe expedition,string slot,string race,string cls,bool resume)
        {
            if(!resume)yield return demo.Validate(slot,cls,false);yield return expedition.Validate(slot,resume);var flow=FindAnyObjectByType<DemoFlow>();if(!resume)yield return Pilots(flow,slot);
            Require(PrototypeSaveStore.Load(slot,flow.Session.Rules,out var s),"valid final save");Require(s.RaceId==race&&s.ClassId==cls&&s.IdentityRevision==1&&s.HeroLevel==4&&s.Experience==60,"persistent hero progression and race");Require(s.Weapon==EquipmentOperations.AdvancedWeapon(cls)&&s.Accessory=="pilgrim-charm"&&s.Facts.Contains("umbral-outpost-conquered"),"same equipment and real conquest");Require(s.Buildings.Any(b=>b.Id=="workshop"&&b.CultureId=="stonehall")&&s.Buildings.Any(b=>b.Id=="warehouse"&&b.CultureId=="frontier"),"two useful cultural buildings");Require(new[]{"frontier-guard","stonehall-striker"}.All(id=>s.Army.Any(r=>r.Role==id&&r.CultureId==flow.Session.Rules.Civilizations.Unit(id).CultureId)),"two persistent cultural units");
        }
        private static IEnumerator Pilots(DemoFlow flow,string slot)
        {
            var d=flow.Director;flow.Front.Controls["continue"].onClick.Invoke();yield return null;var s=flow.Session.State;var camp=flow.GetComponent<DemoSettlementPanel>();var panel=FindAnyObjectByType<ExpeditionPanel>();var party=FindAnyObjectByType<PartyWorld>();Require(party.AtCamp,"camp reached by previous real expedition");Require(s.CultureId=="stonehall","culture independent from hero race");
            panel.Open();panel.Controls["culture-tab"].onClick.Invoke();yield return null;yield return Capture(slot,"PilotsBefore");panel.Close();
            // Finance the two pilots with real online production and existing wood sales.
            var workshop=s.Buildings.FindIndex(b=>b.Id=="workshop");Require(camp.SelectBuilding(workshop)&&camp.ToggleProduction(),"activate paid Forja workshop");var deadline=Time.realtimeSinceStartup+480;
            while((s.Gold<27||s.Wood<5)&&Time.realtimeSinceStartup<deadline)
            {if(s.Wood>=10&&s.Gold<27)Require(camp.SellWood(),"existing paid wood sale");yield return null;}
            Require(s.Gold>=27&&s.Wood>=5,"production finances cultural pilots");Require(camp.SelectBuilding(workshop)&&camp.ToggleProduction(),"pause workshop to replenish food");
            deadline=Time.realtimeSinceStartup+180;while(s.Food<6&&Time.realtimeSinceStartup<deadline)yield return null;Require(s.Food>=6,"actual farm supplies recruitment");Require(panel.SetCulture("frontier"),"choose independent frontier culture");
            var choices=new[]{new Vector3(6,0,-6),new Vector3(5,0,-8),new Vector3(5,0,-15),new Vector3(15,0,-3),new Vector3(10,0,-4)};var at=choices.FirstOrDefault(p=>d.Construction.CanPlace(p));Require(at!=Vector3.zero,"free warehouse footprint");Require(camp.SelectBlueprint("warehouse")&&d.Construction.Place(at),"paid frontier warehouse");d.Execute("explore");Require(SettlementOperations.Capacity(s,flow.Session.Settlement)>=320,"real cultural storage");flow.CameraRig.FocusOn(new Vector3(10,0,-10));yield return Capture(slot,"CulturalBuildings");
            panel.Open();panel.Controls["culture-tab"].onClick.Invoke();yield return null;
            foreach(var id in new[]{"frontier-guard","stonehall-striker"}){var button=panel.Controls["culture-recruit-"+id];Require(button.interactable,"culture unit available "+id);button.onClick.Invoke();yield return null;Require(s.Army.Any(r=>r.Role==id),"paid military recruitment "+id);}
            yield return Capture(slot,"CulturalRecruitment");panel.Controls["army-tab"].onClick.Invoke();yield return null;yield return Capture(slot,"MulticulturalArmy");panel.Close();
            var originalPilots=s.Army.Where(r=>flow.Session.Rules.Civilizations.MilitaryRole(r.Role)!=null).ToArray();Require(flow.BeginPatrol(),"existing replayable patrol with cultural military roster");var statuses=new List<RtsUnitStatus>();d.Rts.CopyStatus(statuses);foreach(var id in new[]{"frontier-guard","stonehall-striker"}){var p=flow.Session.Rules.Civilizations.Unit(id).Profile;var u=statuses.Single(u=>u.Role==p.Title);Require(u.Maximum==p.Health&&Math.Abs(u.Unit.Agent.speed-p.Speed)<.01,"culture stats deployed "+id);}
            var events=new List<RtsTraceSample>();Action<RtsTraceSample> trace=events.Add;d.Rts.Diagnostic+=trace;d.Rts.SelectArea(Camera.main,new Rect(0,0,Screen.width,Screen.height),false);var foe=statuses.First(u=>u.Team==1).Unit;Require(d.Rts.Command(foe.transform.position,foe.GetComponentInChildren<Collider>(),false),"multicultural army attack order");yield return Wait(.5f);yield return Capture(slot,"CulturalRts");
            deadline=Time.realtimeSinceStartup+100;while(d.Rts.Active&&Time.realtimeSinceStartup<deadline){if(s.Health<flow.Session.HeroProfile.Health)d.Rts.HeroAction("recover");d.Rts.HeroAction("technique");yield return null;}d.Rts.Diagnostic-=trace;Require(d.Rts.LastResolvedVictory==true&&s.Health>0,"real multicultural patrol victory");
            var proof=new CombatProof{Strikes=events.Count(e=>e.Kind=="strike"),GuardStrikes=events.Count(e=>e.Kind=="strike"&&e.Actor>=0&&e.Actor<statuses.Count&&statuses[e.Actor].Role=="Guardia de frontera"),ForjaStrikes=events.Count(e=>e.Kind=="strike"&&e.Actor>=0&&e.Actor<statuses.Count&&statuses[e.Actor].Role=="Choque de forja"),Race=s.RaceId,Class=s.ClassId};
            var lost=originalPilots.Where(r=>s.Army.All(a=>a.Id!=r.Id)).ToArray();proof.LostSoldiers=lost.Select(r=>r.Id).ToArray();
            panel.Open();panel.Controls["army-tab"].onClick.Invoke();yield return null;yield return Capture(slot,"CulturalCasualties");panel.Close();
            Require(d.Hero.TryMoveTo(new Vector3(9,0,-10)),"return to multicultural settlement");yield return Arrive(d.Hero);
            var goldNeeded=lost.Sum(r=>flow.Session.Rules.Civilizations.Unit(r.Role).Profile.RecruitGold);var foodNeeded=lost.Sum(r=>flow.Session.Rules.Civilizations.Unit(r.Role).Profile.RecruitFood);deadline=Time.realtimeSinceStartup+180;
            while((s.Gold<goldNeeded||s.Food<foodNeeded)&&Time.realtimeSinceStartup<deadline){if(s.Gold<goldNeeded&&s.Wood>=5)Require(camp.SellWood(),"real wood sale finances a replacement");yield return null;}
            Require(s.Gold>=goldNeeded&&s.Food>=foodNeeded,"real territorial production and farm finance replacements");var replacements=new List<string>();
            foreach(var record in lost){Require(panel.RecruitCulture(record.Role),"paid replacement after legitimate RTS casualty");var next=s.Army.Last();Require(next.Id!=record.Id&&s.Army.All(r=>r.Id!=record.Id),"dead soldier is not resurrected or duplicated");replacements.Add(next.Id);}proof.RecruitedReplacements=replacements.ToArray();
            Require(new[]{"frontier-guard","stonehall-striker"}.All(id=>s.Army.Any(r=>r.Role==id&&r.Health>0)),"two living pilot profiles available for persistent save after paid replacements");File.WriteAllText(slot+".cultural-combat.json",JsonUtility.ToJson(proof,true));
            panel.Open();panel.Controls["territory-tab"].onClick.Invoke();yield return null;panel.Controls["all-army"].onClick.Invoke();panel.Controls["culture-tab"].onClick.Invoke();yield return null;yield return Capture(slot,"PilotsAfter");panel.Close();Require(d.Save(slot)&&flow.ReturnToTitle(),"final cultural save/title");File.WriteAllText(slot+".identity.json",JsonUtility.ToJson(s,true));
        }
        [Serializable]private sealed class CombatProof{public int Strikes,GuardStrikes,ForjaStrikes;public string Race,Class;public string[] LostSoldiers,RecruitedReplacements;}
        private static IEnumerator Capture(string slot,string name)=>DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(slot),"Captures",Path.GetFileNameWithoutExtension(slot)+"-"+name+".png"));
        private static IEnumerator Wait(float seconds){var end=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<end)yield return null;}
        private static IEnumerator Arrive(ExplorationUnit u){yield return null;var end=Time.realtimeSinceStartup+30;while(u.IsMoving&&Time.realtimeSinceStartup<end)yield return null;Require(!u.IsMoving,"navigation arrival");}
        private static void Require(bool value,string message){if(!value)throw new InvalidOperationException("E19 smoke: "+message);}
    }
}
