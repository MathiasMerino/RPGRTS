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
        public InteractionPoint Patrol;
        public GameSession Session=>Director.Session;
        public bool UxEnabled=>Session.Rules.UxExperience;
        public string SlotPath { get; set; }
        public bool IsPlaying { get; private set; }
        public bool LegacyProbe { get; private set; }
        public int EncounterSeed { get; set; }
        public event Action Started;
        private float entryVolume;
        private DemoChapter patrolReturn;
        private void Awake()
        {
            entryVolume=AudioListener.volume;
            if(CameraRig!=null){CameraRig.InputSession=Session;if(UxEnabled)CameraRig.FollowTarget=Director.Hero;}
            if(string.IsNullOrEmpty(SlotPath))SlotPath=Path.Combine(Application.persistentDataPath,"AetherniaPrototype","demo-v1.json");
        }
        private void OnEnable(){if(Director!=null){Director.Encounter.Resolved+=CombatResolved;Director.Rts.Resolved+=RtsResolved;Session.AdditionalInteraction+=Interact;}}
        private void OnDisable()
        {
            if(Director!=null){Director.Encounter.Resolved-=CombatResolved;Director.Rts.Resolved-=RtsResolved;Session.AdditionalInteraction-=Interact;}
            Time.timeScale=1;AudioListener.volume=entryVolume;
        }
        private void Start()
        {
            var args=Environment.GetCommandLineArgs();LegacyProbe=Array.IndexOf(args,"-aetherniaSmoke")>=0;
            if(LegacyProbe){Front.Hide();IsPlaying=true;return;}
            var index=Array.IndexOf(args,"-aetherniaDemoSmoke");
            if(index<0)index=Array.IndexOf(args,"-aetherniaExpeditionSmoke");
            if(index<0)index=Array.IndexOf(args,"-aetherniaProgressionSmoke");
            if(index<0)index=Array.IndexOf(args,"-aetherniaCivilizationSmoke");
            if(index<0)index=Array.IndexOf(args,"-aetherniaUx01Smoke");
            if(index<0)index=Array.IndexOf(args,"-aetherniaUx02Smoke");
            if(index<0)index=Array.IndexOf(args,"-aetherniaE20Smoke");
            if(index<0)index=Array.IndexOf(args,"-aetherniaSave");
            if(index>=0&&index+1<args.Length)SlotPath=args[index+1];
            Director.SavePathOverride=SlotPath;ShowMain();
        }
        private void Update()
        {
            if(IsPlaying&&Session.State.Demo!=null&&Session.State.Demo.Chapter==DemoChapter.Settlement&&Session.State.Demo.Produced>0&&SettlementOperations.Has(Session.State,"townhall")&&SettlementOperations.Has(Session.State,"workshop"))
            {
                Session.State.Demo.Chapter=DemoChapter.FreeExploration;Director.Execute("explore");Session.Say("El campamento ya produce. Puedes explorar libremente, atender misiones, construir o entrenar reclutas; no hay una coronación obligatoria.");Director.Save(SlotPath);
            }
            if(IsPlaying&&Keyboard.current!=null&&Keyboard.current.pKey.wasPressedThisFrame&&!UiInputGuard.BlocksWorldInput)
            {if(Session.Paused)Resume();else{Pause();Front.ShowOptions();}}
        }
        public bool HasSave()=>PrototypeSaveStore.Load(SlotPath,Session.Rules,out _)||PrototypeSaveStore.Load(Director.SavePath,Session.Rules,out _);
        public void ShowMain()
        {
            Time.timeScale=1;IsPlaying=false;Session.Paused=true;ExistingHud.gameObject.SetActive(false);
            Director.Hero.Stop();Session.SetMode(PrototypeMode.MainMenu);CameraRig.SetZoom(13);CameraRig.SetRegion(Vector3.zero);CameraRig.FocusOn(new Vector3(-1,0,1));Front.ShowMain();
        }
        public void CreateCharacter(){Session.SetMode(PrototypeMode.CharacterCreation);Front.ShowCreation();}
        public bool StartNew(string name,string classId,string lineage)
        {
            var profile=Session.Rules.Find(classId);
            if(profile==null||!CivilizationOperations.KnownRace(Session.Rules,lineage)||(Session.Rules.Civilizations!=null&&!Session.Rules.Civilizations.Allows(lineage,classId))||Director.IsBusy)return false;
            try
            {
                if(File.Exists(SlotPath))File.Copy(SlotPath,SlotPath+".backup-"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmssfff"),false);
            }
            catch(IOException){Front.Notify("No se pudo respaldar la partida anterior. Se conserva y no se inicia otra encima.");return false;}
            catch(UnauthorizedAccessException){Front.Notify("No hay permiso para respaldar la partida. El archivo anterior se conserva.");return false;}
            var state=new PrototypeState{ClassId=classId,RaceId=lineage,Health=profile.Health,Demo=new DemoProgress(),HeroPosition=new Vector3(-10,0,1.4f)};
            if(UxEnabled)state.Demo.UxRevision=1;
            if(Session.Rules.Civilizations!=null){state.IdentityRevision=1;state.CultureId="frontier";ProgressionOperations.Activate(state,Session.Rules.Progression);state.Health=ProgressionOperations.ResolveProfile(state,Session.Rules).Health;}
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
            if(Session.State.Demo!=null&&Session.State.Demo.Chapter==DemoChapter.FirstCombat&&!UxEnabled)BeginFirstCombat();
            if(UxEnabled&&Session.State.Demo!=null&&Session.State.Demo.UxFormation!=null&&Session.State.Demo.UxFormation.Length>0)BeginPatrol();
            return true;
        }
        public void CompleteIntro()
        {
            if(Session.State.Demo==null)return;
            Session.State.Demo.IntroSeen=true;Session.State.Demo.Chapter=DemoChapter.FirstCombat;ActivateGame();Director.Save(SlotPath);if(!UxEnabled)BeginFirstCombat();
        }
        private void ActivateGame()
        {
            Time.timeScale=1;Session.Paused=false;IsPlaying=true;ExistingHud.gameObject.SetActive(true);Front.Hide();
            Session.SetMode(PrototypeMode.Exploration);Director.Hero.Select(true);CameraRig.SetRegion(Director.Hero.transform.position.x>65?new Vector3(80,0,0):Director.Hero.transform.position.x>30?new Vector3(50,0,0):Vector3.zero);CameraRig.FocusOn(Director.Hero.transform.position);Started?.Invoke();
        }
        public bool BeginFirstCombat()
        {
            if(Session.State.Demo==null||Session.State.Demo.Chapter!=DemoChapter.FirstCombat)return false;
            return Director.Encounter.BeginAt(Director.Hero.transform.position+Vector3.left*3,EncounterSeed);
        }
        private void PrepareRetry(){if(Session.Rules.Progression!=null)Session.State.Health=Session.HeroProfile.Health;else Session.SelectClass(Session.State.ClassId);}
        public void RetryFirstCombat()
        {
            Director.Execute("explore");PrepareRetry();
            Director.Hero.Warp(new Vector3(-10,0,1.4f));ActivateGame();if(!UxEnabled)BeginFirstCombat();
        }
        public void RetryCurrentEncounter()
        {
            if(Director.Rts.IncludesHero&&!Director.Rts.Active)
            {Director.Execute("explore");PrepareRetry();Resume();BeginPatrol();}
            else RetryFirstCombat();
        }
        public static bool CanVisitPatrol(GameSession game)
        {var p=game.State.Demo;return p!=null&&p.Chapter>=DemoChapter.RoadToRts&&(!game.Rules.UxExperience||p.UxRevision!=1||p.Chapter>=DemoChapter.Settlement||LostCargoQuest.Stage(game.State)==CargoQuestStage.Completed);}
        private bool? Interact(string id,string title)
        {
            if(id!="demo-patrol")return null;
            if(Session.Rules.WorldExploration)return GetComponent<WorldExploration>().TalkCaptain();
            if(Session.State.Demo==null||Session.State.Demo.Chapter<DemoChapter.RoadToRts){Session.Say("Abre primero el sendero del prólogo.");return false;}
            if(UxEnabled&&Session.State.Demo.UxRevision==1&&Session.State.Demo.Chapter<DemoChapter.Settlement&&LostCargoQuest.Stage(Session.State)!=CargoQuestStage.Completed){Session.Say("Remite primero el cargamento al habitante.");return false;}
            return BeginPatrol();
        }
        public bool BeginPatrol()
        {
            if(!IsPlaying||Director.IsBusy||!CanVisitPatrol(Session))return false;
            patrolReturn=Session.State.Demo.Chapter;Director.Encounter.Clear();
            if(!(UxEnabled?Director.Rts.BeginPreparation(Director.Hero):Director.Rts.BeginWithHero(Director.Hero)))return false;
            if(patrolReturn<DemoChapter.Settlement)Session.State.Demo.Chapter=DemoChapter.FirstRts;CameraRig.SetRegion(Vector3.zero);CameraRig.SetZoom(8);CameraRig.FocusOn(UxEnabled?Director.Rts.ArenaOffset+new Vector3(-2,0,13):new Vector3(-9,0,13));return true;
        }
        private void RtsResolved(bool won)
        {
            if(Session.State.Demo==null||!Director.Rts.IncludesHero||Director.Rts.ContextId!="patrol")return;
            if(!won){Session.Paused=true;Front.ShowRetry();return;}
            if(patrolReturn>=DemoChapter.Settlement)
            {Director.Execute("explore");Session.State.Demo.Chapter=patrolReturn;CameraRig.SetZoom(11);CameraRig.FocusOn(Director.Hero.transform.position);Session.Say("Patrulla de entrenamiento terminada. No genera otra recompensa; el ejército conserva bajas y salud.");Director.Save(SlotPath);return;}
            Director.Execute("explore");Session.State.Demo.Chapter=DemoChapter.Settlement;
            if(!Session.HasFact("demo-camp-secured")){Session.State.Facts.Add("demo-camp-secured");Session.AwardForFact("demo-camp-secured");}
            Director.Hero.Warp(new Vector3(9,0,-10));CameraRig.SetZoom(11);CameraRig.FocusOn(Director.Hero.transform.position);
            Session.Say("La patrulla aseguró un campamento. Puedes fundar su centro y construir; ayudar aquí no te obliga a gobernar.");Director.Save(SlotPath);
        }
        private void CombatResolved(bool won)
        {
            if(Session.State.Demo==null||Session.State.Demo.Chapter!=DemoChapter.FirstCombat)return;
            if(!won){Session.Paused=true;Front.ShowRetry();return;}
            Director.Execute("explore");Session.State.Demo.Chapter=DemoChapter.RoadToRts;
            Session.Say("Camino despejado. Explora con WASD; busca la señal de patrulla al norte de la aldea. E: interactuar.");
            Director.Save(SlotPath);
        }
        public void Pause(){Session.Paused=true;Time.timeScale=0;if(!Director.Rts.TacticalBattle)Director.Hero.Stop();ExistingHud.gameObject.SetActive(false);}
        public void Resume(){Time.timeScale=1;Session.Paused=false;if(IsPlaying)ExistingHud.gameObject.SetActive(true);Front.Hide();}
        public bool ReturnToTitle()
        {
            if(Director.IsBusy&&!Director.Rts.Preparing){Front.Notify("Termina el encuentro antes de volver al título.");return false;}
            if(Director.Rts.Preparing){if(!Director.Save(SlotPath))return false;Director.Rts.Clear();}
            Director.Execute("explore");if(!Director.Save(SlotPath)){Front.Notify("No se pudo guardar; continúa para conservar la sesión.");return false;}
            ShowMain();return true;
        }
    }
}
