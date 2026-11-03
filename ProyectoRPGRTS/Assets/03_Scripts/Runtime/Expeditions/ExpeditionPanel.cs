using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Aethernia
{
    // Ventana adicional: no sustituye ficha/inventario ni almacena datos de partida.
    public sealed class ExpeditionPanel : MonoBehaviour
    {
        public DemoFlow Flow;public PartyWorld Party;public DungeonExpedition Dungeon;public TerritoryCampaign Territory;
        private GameObject window,groupCombat;private TMP_Text summary,combatText;private DemoWidgets ui;
        private RectTransform layer;
        private readonly Dictionary<string,Button> controls=new Dictionary<string,Button>();
        public IReadOnlyDictionary<string,Button> Controls=>controls;
        private int tab;
        private void Start()
        {
            ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);
            // El marco histórico de1280x720 debe caber también en pantallas4:3.
            Flow.ExistingHud.GetComponent<CanvasScaler>().screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
            var root=new GameObject("Expedition interface",typeof(RectTransform));root.transform.SetParent(Flow.ExistingHud.transform,false);
            layer=(RectTransform)root.transform;layer.anchorMin=layer.anchorMax=layer.pivot=Vector2.one;layer.anchoredPosition=Vector2.zero;layer.sizeDelta=new Vector2(1280,720);
            Add(root.transform,"open","Grupo y expediciones",1000,286,264,Open);
            var panel=DemoWidgets.Rect(root.transform,"Expedition window",384,92,880,500);window=panel.gameObject;ui.Surface(panel,new Color(.04f,.045f,.045f,1));
            ui.Text(panel,"Title","EXPEDICIONES · COMPAÑÍA DEL CAMPAMENTO",16,10,785,36,24).color=ui.Gold;
            Add(panel,"close","×",816,6,48,Close);
            Add(panel,"party-tab","Grupo",16,52,204,()=>tab=0);Add(panel,"army-tab","Ejército",232,52,204,()=>tab=1);Add(panel,"territory-tab","Territorios",448,52,204,()=>tab=2);Add(panel,"rest","Descansar10s",664,52,200,()=>Dungeon.Rest());
            summary=ui.Text(panel,"State","",16,104,848,208,18);
            var i=0;foreach(var role in Party.Roles.Roles){var id=role.Id;Add(panel,"companion-"+id,"Incorporar "+role.Title,16+i*284,324,272,()=>Party.Recruit(id));i++;}
            for(i=0;i<3;i++){var index=i;Add(panel,"toggle-party-"+i,"Grupo / reserva "+(i+1),16+i*284,374,272,()=>ToggleCompanion(index));}
            for(i=0;i<6;i++){var index=i;var advanced=Flow.Session.Rules.Progression!=null;Add(panel,"army-"+i,"Unidad "+(i+1),16+(i%3)*284,324+(i/3)*50,advanced?178:272,()=>ToggleArmy(index));if(advanced)Add(panel,"train-"+i,"Mejorar",200+(i%3)*284,324+(i/3)*50,88,()=>Train(index));}
            Add(panel,"march-crossing","Marchar al paso",16,324,272,()=>{Close();Territory.March("crossing");});Add(panel,"march-outpost","Marchar al puesto",300,324,272,()=>{Close();Territory.March("outpost");});
            Add(panel,"all-army","Preparar todo el ejército",584,324,280,()=>{foreach(var r in Flow.Session.State.Army)Territory.SelectSoldier(r.Id,true);});
            ui.Text(panel,"Guide",Flow.Session.Rules.Progression!=null?"Grupo: espada6/luchador5/arco7oro +2alimento. Un rol por compañero, máximo3.\nEntrenar ejército:8oro/3madera, cuartel+taller2; máximo6reclutas, sin curación.\nDescanso4PV/10s: héroe, grupo y ejército. Reserva conserva identidad/salud.":"Grupo:5oro/2alimento por rol único. Reserva conserva identidad y salud.\nWASD explora; E interactúa. Descanso recupera4PV en10s.\nEjército requiere cuartel; capacidad6 separada del grupo3.",16,432,848,62,16);
            var combat=DemoWidgets.Rect(root.transform,"Group turn interface",920,92,344,500);groupCombat=combat.gameObject;ui.Surface(combat,new Color(.04f,.045f,.045f,1));
            combatText=ui.Text(combat,"Initiative and health","",16,12,312,280,16);
            Add(combat,"target-0","Guardián1",16,300,148,()=>Flow.Director.Encounter.SelectGroupTarget("enemy-0"));Add(combat,"target-1","Guardián2",180,300,148,()=>Flow.Director.Encounter.SelectGroupTarget("enemy-1"));
            Add(combat,"group-approach","Acercarse",16,350,100,()=>Flow.Director.Encounter.GroupApproach());Add(combat,"group-attack","Atacar",128,350,100,()=>Flow.Director.Encounter.GroupAttack());Add(combat,"group-guard","Guardia",240,350,88,()=>Flow.Director.Encounter.GroupGuard());Add(combat,"group-recover","Recuperar",16,396,148,()=>Flow.Director.Encounter.GroupRecover());Add(combat,"group-end","Fin de turno",180,396,148,()=>Flow.Director.Encounter.GroupEnd());
            Add(combat,"retreat","Retirada al campamento",16,446,312,()=>Dungeon.Retreat());window.SetActive(false);groupCombat.SetActive(false);
        }
        private Button Add(Transform parent,string id,string title,float x,float y,float width,UnityEngine.Events.UnityAction action)
        {var b=ui.Button(parent,id,title,x,y,width,40,()=>{Flow.Front.Audio?.Play(FeedbackSound.Select,Vector3.zero,true);action();});if(width<180)b.GetComponentInChildren<TMP_Text>().fontSize=16;controls.Add(id,b);return b;}
        public void Open(){if(!Flow.IsPlaying||Flow.Session.Mode!=PrototypeMode.Exploration)return;Flow.ExistingHud.ClosePanels();Flow.Director.Hero.Stop();window.SetActive(true);Flow.Session.WorldUiOpen=true;}
        public void Close(){if(window!=null)window.SetActive(false);Flow.Session.WorldUiOpen=false;}
        private void ToggleCompanion(int i){var list=Flow.Session.State.Companions;if(i<list.Count)Party.SetParty(list[i].Id,!list[i].InParty);}
        public bool Train(int i){var s=Flow.Session.State;var ok=Party.AtCamp&&!Flow.Director.IsBusy&&!Flow.Session.Paused&&i>=0&&i<s.Army.Count&&TroopProgression.Train(s,s.Army[i]);Flow.Session.Say(ok?"Unidad entrenada a nivel2: +2PV máximos y +1daño, sin curación.":"Entrenar requiere campamento, taller2, cuartel,8oro/3madera y unidad nivel1.");return ok;}
        private void ToggleArmy(int i){var s=Flow.Session.State;if(i<s.Army.Count)Territory.SelectSoldier(s.Army[i].Id,!s.ArmySelection.Contains(s.Army[i].Id));}
        private void Update(){if(Keyboard.current?.escapeKey.wasPressedThisFrame==true)Close();}
        private void OnDisable(){if(Flow!=null)Flow.Session.WorldUiOpen=false;}
        private void LateUpdate()
        {
            if(window==null)return;var s=Flow.Session.State;var e=Flow.Director.Encounter;var available=Flow.IsPlaying&&ExpeditionOperations.Ready(s)&&Flow.Session.Mode==PrototypeMode.Exploration&&!Flow.Session.Paused;
            var existing=Flow.ExistingHud.CompactView;if(window.activeSelf&&(existing.WindowOpen||existing.HelpOpen))Close();
            var bounds=((RectTransform)layer.parent).rect;layer.localScale=Vector3.one*Mathf.Min(1,bounds.width/1280,bounds.height/720);
            controls["open"].gameObject.SetActive(available&&!window.activeSelf&&(Flow.Front.Presentation==null||(!existing.WindowOpen&&!existing.HelpOpen)));if(!available)Close();
            if(window.activeSelf)
            {
                var text=new StringBuilder();text.AppendLine("Oro "+s.Gold+" · Madera "+s.Wood+" · Alimento "+s.Food+" · Héroe "+s.Health+"PV");
                if(tab==0){text.AppendLine("GRUPO · "+s.Companions.Count(c=>c.InParty)+"/3 acompañantes, más el héroe");foreach(var c in s.Companions){var p=TroopProgression.Profile(c,Party.Roles.Find(c.Role));text.AppendLine(p.Title+" Nv"+c.Level+" XP"+c.Experience+"/8"+" · "+c.Id.Substring(0,6)+" · "+c.Health+"/"+p.Health+"PV · daño"+p.Damage+" · alcance"+p.Range+"m · "+(c.InParty?"grupo":"reserva"));}text.AppendLine("Ruinas: entrada al oeste del campamento(4,−11). Dos encuentros y objetivo final.\nFondos iniciales escasos: cofre del poblado o misión El cargamento perdido.");}
                if(tab==1){text.AppendLine("EJÉRCITO · "+s.ArmySelection.Count+"/6 preparados; compañeros RPG no se despliegan");for(var i=0;i<s.Army.Count;i++)text.AppendLine((s.ArmySelection.Contains(s.Army[i].Id)?"[X] ":"[ ] ")+Party.Roles.Find(s.Army[i].Role).Title+" · "+s.Army[i].Health+"/"+TroopProgression.Profile(s.Army[i],Party.Roles.Find(s.Army[i].Role)).Health+"PV · Nv"+s.Army[i].Level+" · daño"+TroopProgression.Profile(s.Army[i],Party.Roles.Find(s.Army[i].Role)).Damage);}
                if(tab==2){text.AppendLine("Raza: "+(Flow.Content.Lineages.FirstOrDefault(l=>l.Id==s.RaceId)?.Title??s.RaceId)+" · Cultura: "+(string.IsNullOrEmpty(s.CultureId)?"sin asignar":s.CultureId)+" · Reino: "+(string.IsNullOrEmpty(s.KingdomId)?"sin asignar":s.KingdomId));text.AppendLine("Facción voluntaria: "+Territory.FactionTitle(s.FactionId));foreach(var t in s.Territories)text.AppendLine(Territory.Rules.Find(t.Id).Title+" · "+Territory.FactionTitle(t.Owner)+" · "+(t.Secured?"asegurado":"sin asegurar"));text.AppendLine("Relaciones A:"+s.ReputationA+" B:"+s.ReputationB+" · paso/puesto: +1madera por ciclo cada uno.\n"+Territory.LastResult);}
                summary.text=text.ToString();
                foreach(var id in controls.Keys.Where(k=>k.StartsWith("companion-")||k.StartsWith("toggle-party-")))controls[id].gameObject.SetActive(tab==0);
                foreach(var id in controls.Keys.Where(k=>k.StartsWith("army-")&&k!="army-tab"))controls[id].gameObject.SetActive(tab==1);
                controls["march-crossing"].gameObject.SetActive(tab==2);controls["march-outpost"].gameObject.SetActive(tab==2);controls["all-army"].gameObject.SetActive(tab==2);
                foreach(var role in Party.Roles.Roles)controls["companion-"+role.Id].interactable=Party.AtCamp&&s.Companions.Count<Party.Rules.MaximumCompanions&&!s.Companions.Any(c=>c.Role==role.Id)&&s.Gold>=Party.Rules.GoldCost(role.Id)&&s.Food>=Party.Rules.FoodCost(role.Id);
                for(var i=0;i<3;i++)controls["toggle-party-"+i].interactable=Party.AtCamp&&i<s.Companions.Count&&(s.Companions[i].InParty||s.Companions[i].Health>0);
                for(var i=0;i<6;i++){var b=controls["army-"+i];b.interactable=i<s.Army.Count;b.GetComponentInChildren<TMP_Text>().text=i<s.Army.Count?(s.ArmySelection.Contains(s.Army[i].Id)?"[X] ":"[ ] ")+Party.Roles.Find(s.Army[i].Role).Title:"Sin recluta";}
                for(var i=0;i<6;i++)if(controls.TryGetValue("train-"+i,out var train)){train.gameObject.SetActive(tab==1);train.interactable=Party.AtCamp&&i<s.Army.Count&&TroopProgression.CanTrain(s,s.Army[i]);}
                controls["rest"].interactable=Party.AtCamp&&!Dungeon.Resting;
                controls["all-army"].interactable=s.Army.Count>0;
            }
            groupCombat.SetActive(Flow.IsPlaying&&e.InGroup&&!Flow.Session.Paused&&!existing.WindowOpen&&!existing.HelpOpen);
            if(groupCombat.activeSelf)
            {
                var g=e.Group;var t=new StringBuilder("RUINAS · RONDA "+g.Round+"\n"+(g.Finished?(g.Won?"VICTORIA":"DERROTA · retirada disponible"):(g.Current.Hostile?"ENEMIGO ACTUANDO":"TURNO: "+g.Current.Title))+"\n");
                foreach(var a in g.Members)t.AppendLine((a==g.Current?"> ":"  ")+a.Title.Replace("Guardián del Umbral ","Guardián ")+" · "+a.Health+"/"+a.Profile.Health+"PV · ini"+a.Initiative+(a.Health<=0?" · KO":""));t.AppendLine("Objetivo: "+(g.Members.FirstOrDefault(a=>a.Id==e.GroupTarget)?.Title??"sin objetivo")+"\nMovimiento "+g.MovementRemaining.ToString("0.#")+"m · "+(g.ActionAvailable?"acción disponible":"acción usada"));combatText.text=t.ToString();
                foreach(var id in new[]{"group-approach","group-attack","group-guard","group-recover","group-end","target-0","target-1"})controls[id].interactable=!g.Finished&&!g.Current.Hostile;
                var ready=!g.Finished&&!g.Current.Hostile&&!e.GroupUnit(g.Current.Id).IsMoving;
                controls["group-approach"].interactable=ready&&g.MovementRemaining>.05f;
                controls["group-attack"].interactable=controls["group-guard"].interactable=ready&&g.ActionAvailable;
                controls["group-recover"].interactable=ready&&g.ActionAvailable&&!g.Current.Recovered&&g.Current.Health<g.Current.Profile.Health;
                for(var i=0;i<2;i++)controls["target-"+i].interactable=!g.Finished&&!g.Current.Hostile&&g.Members.Any(a=>a.Id=="enemy-"+i&&a.Health>0);
                controls["retreat"].gameObject.SetActive(g.Finished&&!g.Won);
            }
            if(window.activeSelf||groupCombat.activeSelf)layer.SetAsLastSibling();
        }
    }
}
