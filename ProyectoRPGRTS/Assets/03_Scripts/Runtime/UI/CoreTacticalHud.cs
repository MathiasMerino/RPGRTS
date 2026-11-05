using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    // One presentation surface for both encounter types. The encounter owns all actions and state.
    [DefaultExecutionOrder(520)]
    public sealed class CoreTacticalHud : MonoBehaviour
    {
        private static readonly string[] commands={"attack","guard","recover","approach","technique","end"};
        private static readonly string[] captions={"Atacar","Guardia","Recuperar","Acercarse","Técnica","Fin de turno"};
        private readonly Button[] actors=new Button[6];
        private readonly TMP_Text[] actorText=new TMP_Text[6];
        private readonly Button[] actions=new Button[6];
        private readonly string[] targetIds=new string[6];
        private DemoFlow flow;
        private RectTransform timeline,dock;
        private TMP_Text state,notice,techniqueLabel;
        private Button exit;
        private float refreshAt,movementNoticeUntil,resultUntil;
        private string movementNotice;
        private bool shown;
        private readonly Queue<string> history=new();
        private bool expanded,aimTechnique;
        public bool Visible=>shown;
        public Button AttackButton=>actions[0];
        public Button EndButton=>actions[5];
        public int VisibleActors { get; private set; }

        public void Configure(DemoFlow owner,DemoWidgets ui)
        {
            flow=owner;
            var parent=flow.ExistingHud.transform;
            timeline=Band(parent,"CORE Iniciativa",792,52,true,76);
            dock=Band(parent,"CORE Acciones",768,132,false,14);
            ui.Surface(timeline,new Color(.06f,.066f,.065f,.95f)).raycastTarget=false;
            ui.Surface(dock,new Color(.06f,.066f,.065f,.95f)).raycastTarget=false;
            state=ui.Text(dock,"CORE Estado","",12,10,744,23,17);
            state.alignment=TextAlignmentOptions.Center;
            notice=ui.Text(dock,"CORE Resultado","",12,39,622,42,15);
            notice.alignment=TextAlignmentOptions.Center;
            var log=ui.Button(dock,"CORE Registro","Registro",640,43,116,32,()=>{expanded=!expanded;Render();});
            log.GetComponentInChildren<TMP_Text>().fontSize=15;
            for(var i=0;i<actors.Length;i++)
            {
                var slot=i;
                actors[i]=ui.Button(timeline,"CORE Actor "+i,"",8+i*130,5,126,42,()=>Choose(targetIds[slot]));
                actorText[i]=actors[i].GetComponentInChildren<TMP_Text>();
                actorText[i].fontSize=13;
            }
            for(var i=0;i<actions.Length;i++)
            {
                var id=commands[i];
                actions[i]=ui.Button(dock,"CORE "+id,captions[i],12+i*124,84,120,38,()=>Execute(id));
                actions[i].GetComponentInChildren<TMP_Text>().fontSize=15;
            }
            techniqueLabel=actions[4].GetComponentInChildren<TMP_Text>();
            exit=ui.Button(dock,"CORE Retirada","Retirada",210,84,344,38,LeaveDefeat);
            flow.Director.Encounter.ActionResolved+=Record;
            flow.Director.Encounter.GroupActionResolved+=Record;
            flow.Session.Message+=RefreshMessage;
            timeline.gameObject.SetActive(false);
            dock.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if(flow==null)return;
            flow.Director.Encounter.ActionResolved-=Record;
            flow.Director.Encounter.GroupActionResolved-=Record;
            flow.Session.Message-=RefreshMessage;
        }

        private static RectTransform Band(Transform parent,string name,int width,int height,bool top,int inset)
        {
            var rect=DemoWidgets.Rect(parent,name,0,0,width,height);
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,top?1:0);
            rect.anchoredPosition=new Vector2(0,top?-inset:inset);
            return rect;
        }

        private void LateUpdate()
        {
            if(flow==null)return;
            var view=flow.ExistingHud.CompactView;
            var active=flow.IsPlaying&&flow.Session.Mode==PrototypeMode.TurnCombat&&!flow.Session.Paused&&
                !flow.Session.WorldUiOpen&&!view.WindowOpen&&!view.HelpOpen;
            if(active!=shown)
            {
                shown=active;
                timeline.gameObject.SetActive(shown);
                dock.gameObject.SetActive(shown);
                if(!shown&&flow.Session.Mode!=PrototypeMode.TurnCombat){history.Clear();expanded=false;aimTechnique=false;movementNoticeUntil=0;resultUntil=0;}
                refreshAt=0;
            }
            if(shown&&UnityEngine.InputSystem.Keyboard.current?.escapeKey.wasPressedThisFrame==true)
            {
                flow.Director.Encounter.ClearCoreTarget();
                aimTechnique=false;
                refreshAt=0;
            }
            if(shown&&Time.unscaledTime>=refreshAt){refreshAt=Time.unscaledTime+.10f;Render();}
        }

        private void Execute(string id)
        {
            if(!shown||flow.Session.Mode!=PrototypeMode.TurnCombat||flow.Session.Paused||flow.Session.WorldUiOpen)return;
            if(id=="technique"&&!aimTechnique)
            {
                aimTechnique=true;
                Render();
                return;
            }
            aimTechnique=false;
            flow.Director.Execute(id);
            if(shown&&flow.Session.Mode==PrototypeMode.TurnCombat)Render();
        }

        private void LeaveDefeat()
        {
            if(flow.Director.Encounter.InGroup)
                FindAnyObjectByType<DungeonExpedition>()?.Retreat();
            else if(!flow.Continue(flow.SlotPath))
                flow.Session.Say("El último guardado no se pudo abrir.");
        }

        private void Choose(string id)
        {
            if(!shown||flow.Session.Mode!=PrototypeMode.TurnCombat||flow.Session.WorldUiOpen||string.IsNullOrEmpty(id))return;
            var encounter=flow.Director.Encounter;
            if(encounter.InGroup)encounter.SelectGroupTarget(id);
            else if(encounter.Enemy!=null)encounter.TrySelect(encounter.Enemy.GetComponentInChildren<Collider>());
            Render();
        }

        private void RefreshMessage(string message)
        {
            refreshAt=0;
            if(flow.Session.Mode!=PrototypeMode.TurnCombat||!message.StartsWith("Destino inaccesible"))return;
            movementNotice=message;
            movementNoticeUntil=Time.unscaledTime+3;
        }

        private void Record(CombatFeedback feedback)
        {
            var result=feedback.Result;
            var who=result.Actor==CombatSide.Player?"Aliado":"Rival";
            string entry;
            if(result.Roll>0)
            {
                var roll=result.Defense>0?$"{result.Roll} {result.Modifier:+0;-0;+0} = {result.Roll+result.Modifier} contra {result.Defense}":result.Roll.ToString();
                entry=who+" · "+roll+" · "+(result.Hit?"Impacto -"+result.Damage+" PV":"Fallo");
            }
            else entry=who+" · "+(result.Healing>0?"Recupera "+result.Healing+" PV":result.Kind==CombatActionKind.Guard?"Guardia":"Espera");
            while(history.Count>=4)history.Dequeue();
            history.Enqueue(entry);
            movementNoticeUntil=0;
            resultUntil=Time.unscaledTime+2.5f;
        }

        private void Render()
        {
            var encounter=flow.Director.Encounter;
            var battle=encounter.Battle;
            var group=encounter.Group;
            if(group==null&&battle==null)return;
            var finished=group?.Finished??battle.Finished;
            var allied=group!=null?!group.Current.Hostile:battle.Phase==TurnPhase.Player;
            var ready=encounter.CanUseCoreCommands;
            var action=group?.ActionAvailable??battle.ActionAvailable;
            var movement=group?.MovementRemaining??battle.MovementRemaining;
            var round=group?.Round??battle.Round;
            var profile=group?.Current.Profile??flow.Session.HeroProfile;
            var skill=profile.Combat;
            var recover=group!=null?!group.Current.Recovered&&group.Current.Health<profile.Health:!battle.RecoveryUsed&&battle.HeroHealth<profile.Health;
            var attackReason=encounter.CoreTargetStatus(false);
            var techniqueReason=encounter.CoreTargetStatus(true);
            var techniqueAvailable=ready&&action&&(group==null||group.Current.Hero)&&
                skill?.TechniqueUnlocked==true&&encounter.TechniqueFocus>=skill.TechniqueCost;
            if(!techniqueAvailable)aimTechnique=false;
            var activeName=group?.Current.Title??(allied?flow.Session.State.Name:"Merodeador");
            var won=group?.Won??battle.Won;
            state.text=finished?(won?"Victoria":"Derrota"):
                $"Ronda {round} · {activeName} · {(action?"1 acción":"acción usada")} · {movement:0.#} m";
            var last=history.Count==0?"":history.Last();
            if(expanded)notice.text=string.Join("\n",history.TakeLast(2));
            else if(finished)notice.text=!won&&group==null?"Puedes volver al último guardado y reintentar.":flow.Session.LastMessage;
            else if(aimTechnique)
            {
                var requirement=techniqueReason??"Objetivo válido. Pulsa Confirmar.";
                notice.text=$"{skill.TechniqueTitle} · 1 acción · {skill.TechniqueCost} foco · {skill.TechniqueRange:0.#} m\n{requirement}";
            }
            else if(Time.unscaledTime<movementNoticeUntil)notice.text=movementNotice;
            else if(!allied)notice.text="Turno rival · "+last;
            else if(!action||Time.unscaledTime<resultUntil)notice.text=last;
            else if(attackReason!=null)notice.text=attackReason;
            else notice.text=encounter.CoreTargetSummary;
            actions[0].interactable=ready&&action&&attackReason==null;
            actions[1].interactable=ready&&action;
            actions[2].interactable=ready&&action&&recover;
            actions[3].interactable=ready&&movement>.05f&&encounter.HasCoreTarget;
            actions[4].interactable=techniqueAvailable&&(!aimTechnique||techniqueReason==null);
            actions[5].interactable=ready;
            if(aimTechnique)techniqueLabel.text="Confirmar";
            else if(group!=null&&!group.Current.Hero)techniqueLabel.text="Solo héroe";
            else techniqueLabel.text=skill?.TechniqueUnlocked==true?"Técnica "+encounter.TechniqueFocus:"Técnica Nv2";
            foreach(var button in actions)button.gameObject.SetActive(!finished);
            exit.gameObject.SetActive(finished&&!won);
            if(finished&&!won)exit.GetComponentInChildren<TMP_Text>().text=group==null?"Volver al último guardado":"Retirada al campamento";
            if(group!=null)
            {
                for(var i=0;i<group.Members.Count;i++)
                {
                    var actor=group.Members[i];
                    var title=actor.Hostile?actor.Title.Replace(" del Umbral",""):actor.Title;
                    Row(i,title,actor.Initiative,actor.Health,actor.Hostile,actor==group.Current,actor.Id==encounter.GroupTarget,actor.Id);
                }
                VisibleActors=group.Members.Count;
            }
            else
            {
                var heroFirst=battle.HeroInitiative>=battle.EnemyInitiative;
                Row(heroFirst?0:1,flow.Session.State.Name,battle.HeroInitiative,battle.HeroHealth,false,allied,false,null);
                Row(heroFirst?1:0,"Merodeador",battle.EnemyInitiative,battle.EnemyHealth,true,!allied,encounter.TargetSelected,"solo");
                VisibleActors=2;
            }
            for(var i=VisibleActors;i<actors.Length;i++)actors[i].gameObject.SetActive(false);
            timeline.sizeDelta=new Vector2(12+130*VisibleActors,52);
        }

        private void Row(int index,string title,int initiative,int health,bool hostile,bool active,bool selected,string id)
        {
            var button=actors[index];button.gameObject.SetActive(true);
            targetIds[index]=hostile?id:null;
            button.interactable=hostile&&health>0;
            var caption=title.Length>13?title.Substring(0,10)+"...":title;
            var valid=aimTechnique&&hostile&&health>0&&flow.Director.Encounter.CoreTargetStatus(true,id)==null;
            var marker=active?"> ":selected?"* ":valid?"+ ":"";
            actorText[index].text=marker+caption+"\n"+(hostile?"R":"A")+" · "+health+" PV · I"+initiative;
            actorText[index].color=active?new Color(1,.81f,.44f):valid?new Color(.7f,.91f,1):hostile?new Color(.98f,.74f,.66f):new Color(.79f,.89f,.84f);
        }
    }
}
