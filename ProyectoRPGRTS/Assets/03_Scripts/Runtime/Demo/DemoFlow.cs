using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aethernia
{
    // Coordina progreso persistente; los modos y las reglas pertenecen a GameSession/controladores existentes.
    public sealed class DemoFlow : MonoBehaviour
    {
        public PrototypeDirector Director;
        public VerticalSliceHud ExistingHud;
        public DemoFrontEnd Front;
        public DemoContent Content;
        public IsometricCameraRig CameraRig;
        public GameSession Session=>Director.Session;
        public string SlotPath { get; set; }
        public bool IsPlaying { get; private set; }
        public bool LegacyProbe { get; private set; }
        public int EncounterSeed { get; set; }
        public event Action Started;
        private float entryVolume;
        private void Awake()
        {
            entryVolume=AudioListener.volume;
            if(CameraRig!=null)CameraRig.InputSession=Session;
            if(string.IsNullOrEmpty(SlotPath))SlotPath=Path.Combine(Application.persistentDataPath,"AetherniaPrototype","demo-v1.json");
        }
        private void OnEnable(){if(Director!=null)Director.Encounter.Resolved+=CombatResolved;}
        private void OnDisable()
        {
            if(Director!=null)Director.Encounter.Resolved-=CombatResolved;
            Time.timeScale=1;AudioListener.volume=entryVolume;
        }
        private void Start()
        {
            var args=Environment.GetCommandLineArgs();LegacyProbe=Array.IndexOf(args,"-aetherniaSmoke")>=0;
            if(LegacyProbe){Front.Hide();IsPlaying=true;return;}
            var index=Array.IndexOf(args,"-aetherniaDemoSmoke");
            if(index>=0&&index+1<args.Length)SlotPath=args[index+1];
            Director.SavePathOverride=SlotPath;ShowMain();
        }
        private void Update()
        {
            if(IsPlaying&&Keyboard.current!=null&&Keyboard.current.pKey.wasPressedThisFrame&&!UiInputGuard.BlocksWorldInput)
            {if(Session.Paused)Resume();else{Pause();Front.ShowOptions();}}
        }
        public bool HasSave()=>PrototypeSaveStore.Load(SlotPath,Session.Rules,out _)||PrototypeSaveStore.Load(Director.SavePath,Session.Rules,out _);
        public void ShowMain()
        {
            Time.timeScale=1;IsPlaying=false;Session.Paused=true;ExistingHud.gameObject.SetActive(false);
            Director.Hero.Stop();Session.SetMode(PrototypeMode.MainMenu);CameraRig.SetRegion(Vector3.zero);CameraRig.FocusOn(new Vector3(-1,0,1));Front.ShowMain();
        }
        public void CreateCharacter(){Session.SetMode(PrototypeMode.CharacterCreation);Front.ShowCreation();}
        public bool StartNew(string name,string classId,string lineage)
        {
            var profile=Session.Rules.Find(classId);
            if(profile==null||!DemoContent.KnownLineage(lineage)||Director.IsBusy)return false;
            try
            {
                if(File.Exists(SlotPath))File.Copy(SlotPath,SlotPath+".backup-"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmssfff"),false);
            }
            catch(IOException){Front.Notify("No se pudo respaldar la partida anterior. Se conserva y no se inicia otra encima.");return false;}
            catch(UnauthorizedAccessException){Front.Notify("No hay permiso para respaldar la partida. El archivo anterior se conserva.");return false;}
            var state=new PrototypeState{ClassId=classId,RaceId=lineage,Health=profile.Health,Demo=new DemoProgress(),HeroPosition=new Vector3(-10,0,1.4f)};
            WorldOperations.AddItem(state,"training-sword",1);WorldOperations.AddItem(state,"cloth",1);
            WorldOperations.Equip(state,"training-sword");WorldOperations.Equip(state,"cloth");state.Facts.Add("demo-starter-kit");
            Director.Encounter.Clear();Director.Rts.Clear();Session.ReplaceState(state);Session.Rename(string.IsNullOrWhiteSpace(name)?"Viajero":name);
            if(!Director.Hero.Warp(state.HeroPosition))return false;
            Director.Construction.Restore();Session.Paused=true;Session.SetMode(PrototypeMode.Intro);Front.ShowIntro(0);
            if(!PrototypeSaveStore.Save(SlotPath,state,Session.Rules))Front.Notify("Inicio preparado; no se pudo guardar: "+PrototypeSaveStore.LastWriteFailure);
            return true;
        }
        public bool Continue(string path=null)
        {
            var chosen=path??(PrototypeSaveStore.Load(SlotPath,Session.Rules,out _)?SlotPath:Director.SavePath);
            if(!Director.Load(chosen)){Front.Notify("No hay una partida válida y compatible en esa ubicación.");return false;}
            Director.SavePathOverride=SlotPath;
            if(Session.State.Demo!=null&&Session.State.Demo.Chapter==DemoChapter.Intro)
            {Session.Paused=true;ExistingHud.gameObject.SetActive(false);Session.SetMode(PrototypeMode.Intro);Front.ShowIntro(0);return true;}
            ActivateGame();
            if(Session.State.Demo!=null&&Session.State.Demo.Chapter==DemoChapter.FirstCombat)BeginFirstCombat();
            return true;
        }
        public void CompleteIntro()
        {
            if(Session.State.Demo==null)return;
            Session.State.Demo.IntroSeen=true;Session.State.Demo.Chapter=DemoChapter.FirstCombat;ActivateGame();Director.Save(SlotPath);BeginFirstCombat();
        }
        private void ActivateGame()
        {
            Time.timeScale=1;Session.Paused=false;IsPlaying=true;ExistingHud.gameObject.SetActive(true);Front.Hide();
            Session.SetMode(PrototypeMode.Exploration);Director.Hero.Select(true);CameraRig.SetRegion(Director.Hero.transform.position.x>30?new Vector3(50,0,0):Vector3.zero);CameraRig.FocusOn(Director.Hero.transform.position);Started?.Invoke();
        }
        public bool BeginFirstCombat()
        {
            if(Session.State.Demo==null||Session.State.Demo.Chapter!=DemoChapter.FirstCombat)return false;
            return Director.Encounter.BeginAt(Director.Hero.transform.position+Vector3.left*3,EncounterSeed);
        }
        public void RetryFirstCombat()
        {
            Director.Execute("explore");Session.SelectClass(Session.State.ClassId);
            Director.Hero.Warp(new Vector3(-10,0,1.4f));ActivateGame();BeginFirstCombat();
        }
        private void CombatResolved(bool won)
        {
            if(Session.State.Demo==null||Session.State.Demo.Chapter!=DemoChapter.FirstCombat)return;
            if(!won){Session.Paused=true;Front.ShowRetry();return;}
            Director.Execute("explore");Session.State.Demo.Chapter=DemoChapter.RoadToRts;
            Session.Say("Camino despejado. Explora con WASD; busca la señal de patrulla al norte de la aldea. E: interactuar.");
            Director.Save(SlotPath);
        }
        public void Pause(){Session.Paused=true;Time.timeScale=0;Director.Hero.Stop();}
        public void Resume(){Time.timeScale=1;Session.Paused=false;Front.Hide();}
        public bool ReturnToTitle()
        {
            if(Director.IsBusy){Front.Notify("Termina el encuentro antes de volver al título.");return false;}
            Director.Execute("explore");if(!Director.Save(SlotPath)){Front.Notify("No se pudo guardar; continúa para conservar la sesión.");return false;}
            ShowMain();return true;
        }
    }
}
