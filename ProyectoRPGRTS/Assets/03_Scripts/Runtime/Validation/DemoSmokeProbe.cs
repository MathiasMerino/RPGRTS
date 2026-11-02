using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Aethernia
{
    // Ausente en sesiones ordinarias: solo se crea si el proceso recibe el argumento de QA.
    public sealed class DemoSmokeProbe : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void IsolateOptions()
        {
            var args=Environment.GetCommandLineArgs();var index=Array.IndexOf(args,"-aetherniaDemoSmoke");
            DemoPreferences.SessionStorageKey=index>=0&&index+1<args.Length?"Aethernia.Tests.NativeOptions."+args[index+1]:null;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] private static void Install()
        {
            var args=Environment.GetCommandLineArgs();
            if(Array.IndexOf(args,"-aetherniaDemoSmoke")>=0)
            {
                var node=new GameObject("Explicit demo validation");node.AddComponent<DemoSmokeProbe>();
                if(Array.IndexOf(args,"-demoRecord")>=0)node.AddComponent<DemoRecordingProbe>();
            }
        }
        private DemoFlow flow;private PrototypeDirector d;private string slot,receipt;private float started;
        private IEnumerator Start()
        {
            Application.runInBackground=true;started=Time.realtimeSinceStartup;
            var args=Environment.GetCommandLineArgs();slot=args[Array.IndexOf(args,"-aetherniaDemoSmoke")+1];
            var resume=Array.IndexOf(args,"-demoResume")>=0;var index=Array.IndexOf(args,"-demoClass");var id=index<0?"warrior":args[index+1];
            receipt=slot+(resume?".resume.txt":".start.txt");Directory.CreateDirectory(Path.GetDirectoryName(slot));
            var stack=new Stack<IEnumerator>();stack.Push(Run(id,resume));
            while(stack.Count>0)
            {
                object step;
                try
                {
                    var top=stack.Peek();if(!top.MoveNext()){stack.Pop();continue;}step=top.Current;
                    if(step is IEnumerator nested){stack.Push(nested);continue;}
                }
                catch(Exception ex){File.WriteAllText(receipt,"FAIL: "+ex);Debug.LogError(ex);Application.Quit(2);yield break;}
                yield return step;
            }
            GetComponent<DemoRecordingProbe>()?.Finish();
            if(resume){PlayerPrefs.DeleteKey(DemoPreferences.SessionStorageKey);PlayerPrefs.Save();}
            File.WriteAllText(receipt,"PASS: "+(resume?"nuevo proceso: título -> Continuar, identidad/salud/equipo/edificios/ejército/progreso conservados, sin repetir prólogo":"menú -> creación -> prólogo -> RPG legal -> WASD -> interacción -> RTS mismo héroe -> construcción con fondos iniciales -> activación/producción -> cuartel/recluta -> guardado")+"; class="+id+"; seconds="+(Time.realtimeSinceStartup-started).ToString("0.00")+"; allocatedBytes="+UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong());
            // La salida pasa por el botón que usa el jugador. El comprobante se escribe antes de cerrar.
            Click("quit");
        }
        private IEnumerator Run(string id,bool resume)
        {
            yield return null;yield return null;flow=FindAnyObjectByType<DemoFlow>();Require(flow!=null,"demo scene");d=flow.Director;
            Require(flow.Front.Screen==DemoScreen.Main&&!flow.IsPlaying,"main menu first");yield return Wait(.3f);yield return Capture("Main");
            if(resume)
            {
                var p=flow.Front.Preferences;Require(Mathf.Abs(p.Master-.63f)<.001f&&Mathf.Abs(p.Effects-.57f)<.001f&&Mathf.Abs(flow.CameraRig.PanSpeed-14)<.001f,"persisted native volume/effects/camera options");
                Require(Mathf.Abs(AudioListener.volume-.63f)<.001f&&Mathf.Abs(flow.Front.Audio.EffectsGain-.57f)<.001f,"applied native audio options");
                Require(p.Width==1920&&p.Height==1080&&QualitySettings.GetQualityLevel()==p.Quality,"persisted native display/quality");yield return CheckDisplay();
            }
            else yield return Options();
            if(resume)
            {
                Require(PrototypeSaveStore.Load(slot,flow.Session.Rules,out var before),"valid saved checkpoint");Click("continue");flow.Pause();
                var now=flow.Session.State;Require(flow.Front.Screen==DemoScreen.None&&now.Demo.IntroSeen&&now.Demo.Chapter==DemoChapter.FreeExploration,"continue bypasses intro");
                Require(now.Name==before.Name&&now.ClassId==id&&now.ClassId==before.ClassId&&now.RaceId==before.RaceId&&now.Health==before.Health,"identity and health");
                Require(now.Gold==before.Gold&&now.Wood==before.Wood&&now.Food==before.Food&&now.Weapon==before.Weapon&&now.Armour==before.Armour,"resources and equipment");
                Require(JsonUtility.ToJson(new Buildings{Items=now.Buildings})==JsonUtility.ToJson(new Buildings{Items=before.Buildings}),"building levels/activation/position");
                Require(JsonUtility.ToJson(new Army{Items=now.Army})==JsonUtility.ToJson(new Army{Items=before.Army}),"army ids/roles/health");
                Require(Vector3.Distance(d.Hero.transform.position,before.HeroPosition)<.1f,"restored hero position");flow.Resume();Require(flow.ExistingHud.gameObject.activeInHierarchy,"restored HUD");yield return Capture("Continued");
                Require(flow.ReturnToTitle(),"save and title after continue");yield break;
            }
            Require(!File.Exists(slot),"fresh isolated slot required");flow.EncounterSeed=42;
            Click("new");Click("class-"+id);if(id=="mage")Click("lineage");flow.Front.NameInput.text="Viajero "+id;
            yield return Wait(.3f);yield return Capture("Creation");Click("begin");Require(flow.Session.State.ClassId==id,"chosen class");
            yield return Wait(.3f);yield return Capture("Intro");if(id=="mage")Click("skip");else for(var i=0;i<flow.Content.Intro.Length;i++)Click("next");yield return null;
            Require(flow.Session.Mode==PrototypeMode.TurnCombat,"RPG starts");var hero=d.Hero;var model=hero.GetComponent<CharacterVisualPresenter>().Model;
            d.Encounter.Approach();yield return Arrive(hero);yield return Capture("Rpg");
            for(var turn=0;turn<30&&flow.Session.State.Demo.Chapter==DemoChapter.FirstCombat;turn++)
            {
                var battle=d.Encounter.Battle;Require(battle!=null,"active RPG");if(battle.HeroHealth<8&&!battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                if(flow.Session.State.Demo.Chapter!=DemoChapter.FirstCombat)break;
                Require(d.Execute("end"),"end actual turn");var limit=Time.realtimeSinceStartup+5;
                while(d.Encounter.Battle!=null&&d.Encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<limit)yield return null;
            }
            Require(flow.Session.State.Demo.Chapter==DemoChapter.RoadToRts,"actual RPG victory");
            var keyboard=InputSystem.AddDevice<Keyboard>();var background=InputSystem.settings.backgroundBehavior;
            try
            {
                InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;EventSystem.current.SetSelectedGameObject(null);
                var position=hero.transform.position;var peakSpeed=0f;var movingUntil=Time.realtimeSinceStartup+.4f;InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));
                while(Time.realtimeSinceStartup<movingUntil){yield return null;peakSpeed=Mathf.Max(peakSpeed,hero.GetComponent<CharacterVisualPresenter>().Model.GetComponent<HumanoidAnimationDriver>().ObservedSpeed);}
                Require(Vector3.Distance(position,hero.transform.position)>.25f,"native WASD displacement");Require(peakSpeed>.1f,"WASD locomotion observed during movement");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;yield return Capture("Wasd");
            }
            finally{InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=background;}
            Require(d.Execute("ration"),"legal existing ration");Require(hero.TryMoveTo(flow.Patrol.transform.position+Vector3.back*1.1f),"navigation to captain");yield return Arrive(hero);
            var hp=flow.Session.State.Health;Require(flow.Patrol.TryInteract(hero,flow.Session),"captain interaction");var rts=d.Rts;var units=new List<RtsUnitStatus>();rts.CopyStatus(units);
            Require(units.Count==13&&rts.IncludesHero&&units.Single(x=>x.Hero).Unit==hero&&flow.Session.State.Health==hp&&hero.GetComponent<CharacterVisualPresenter>().Model==model,"same hero/model/HP into RTS");
            rts.SelectArea(Camera.main,new Rect(0,0,Screen.width,Screen.height),false);Require(rts.SelectedAllies==7,"rectangle selection");Require(rts.HeroAction("guard"),"real hero guard");Require(!rts.HeroAction("guard"),"cooldown rejection");
            Require(rts.Command(new Vector3(-10,0,13),null,false),"real move order");yield return Wait(.15f);
            var enemy=units.First(x=>x.Team==1).Unit;Require(rts.Command(enemy.transform.position,enemy.GetComponentInChildren<Collider>(),false),"real attack order");yield return Wait(.4f);yield return Capture("Rts");
            var deadline=Time.realtimeSinceStartup+60;while(rts.Active&&Time.realtimeSinceStartup<deadline){if(flow.Session.State.Health<flow.Session.Rules.Find(id).Health)rts.HeroAction("recover");yield return null;}
            Require(flow.Session.State.Demo.Chapter==DemoChapter.Settlement&&hero.gameObject.activeInHierarchy,"actual RTS victory/camp");
            var panel=flow.GetComponent<DemoSettlementPanel>();Require(panel.SelectBlueprint("townhall")&&d.Construction.Place(new Vector3(12,0,-8)),"paid center placement");
            Require(panel.SelectBlueprint("workshop")&&d.Construction.Place(new Vector3(8,0,-13)),"paid workshop placement");yield return null;
            Require(panel.SelectBuilding(1)&&panel.ToggleProduction(),"activate real workshop");yield return Capture("Camp");
            deadline=Time.realtimeSinceStartup+13;while(flow.Session.State.Demo.Chapter!=DemoChapter.FreeExploration&&Time.realtimeSinceStartup<deadline)yield return null;
            Require(flow.Session.State.Demo.Produced>0&&flow.Session.State.Demo.Chapter==DemoChapter.FreeExploration,"online production unlock");
            Require(panel.SelectBlueprint("barracks")&&d.Construction.Place(new Vector3(15,0,-13)),"paid barracks from initial funds/production");Require(panel.Recruit("archer"),"paid recruit");d.Execute("explore");yield return null;
            Require(flow.Session.State.Buildings.Count==3&&flow.Session.State.Army.Count==1,"persistent camp and roster");
            flow.CameraRig.FocusOn(new Vector3(11,0,-10));yield return Capture("Free");Require(d.Save(slot),"atomic final save: "+PrototypeSaveStore.LastWriteFailure);
            Require(PrototypeSaveStore.Load(slot,flow.Session.Rules,out var saved)&&saved.Demo.IntroSeen&&saved.Demo.Chapter==DemoChapter.FreeExploration,"valid final checkpoint");
            File.WriteAllText(slot+".identity.json",JsonUtility.ToJson(saved,true));flow.Pause();flow.Front.ShowOptions();yield return Wait(.3f);Click("return-title");Require(flow.Front.Screen==DemoScreen.Main,"return to main");
        }
        [Serializable] private sealed class Buildings{public List<BuildingRecord> Items;}
        [Serializable] private sealed class Army{public List<RecruitRecord> Items;}
        private IEnumerator Options()
        {
            Click("options");yield return Wait(.25f);
            foreach(var slider in flow.Front.GetComponentsInChildren<UnityEngine.UI.Slider>())
            {if(slider.name=="Master")slider.value=.63f;else if(slider.name=="Effects")slider.value=.57f;else if(slider.name=="Camera")slider.value=.5f;}
            Click("resolution");var original=flow.Front.Preferences.Quality;Click("quality");var changed=flow.Front.Preferences.Quality;Click("apply");yield return Wait(.2f);
            Require(QualitySettings.GetQualityLevel()==changed,"actual native quality change");yield return CheckDisplay();
            Click("options");for(var i=0;i<QualitySettings.names.Length&&flow.Front.Preferences.Quality!=original;i++)Click("quality");
            Click("apply");yield return Wait(.25f);var stored=DemoPreferences.Load();Require(Mathf.Abs(stored.Master-.63f)<.001f&&Mathf.Abs(stored.Effects-.57f)<.001f&&Mathf.Abs(stored.CameraSpeed-14)<.001f,"native option event/save");
            Click("options");yield return Wait(.25f);yield return Capture("Options");Click("apply");
        }
        private IEnumerator CheckDisplay()
        {
            if(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Null)yield break;
            var end=Time.realtimeSinceStartup+3;while((UnityEngine.Screen.width!=1920||UnityEngine.Screen.height!=1080)&&Time.realtimeSinceStartup<end)yield return null;
            Require(UnityEngine.Screen.width==1920&&UnityEngine.Screen.height==1080,"native resolution 1920x1080 applied/restored");
        }
        private IEnumerator Capture(string name)=>DemoPlayerCapture.Save(Path.Combine(Path.GetDirectoryName(slot),"Captures",Path.GetFileNameWithoutExtension(slot)+"-"+name+".png"));
        private void Click(string id){var button=flow.Front.Controls[id];Require(button.gameObject.activeInHierarchy&&button.interactable,"button "+id);button.onClick.Invoke();}
        private static void Require(bool condition,string step){if(!condition)throw new InvalidOperationException("Demo smoke: "+step);}
        private static IEnumerator Wait(float seconds){var end=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<end)yield return null;}
        private static IEnumerator Arrive(ExplorationUnit unit){yield return null;var end=Time.realtimeSinceStartup+25;while(unit.IsMoving&&Time.realtimeSinceStartup<end)yield return null;Require(!unit.IsMoving,"navigation arrival");}
    }
}
