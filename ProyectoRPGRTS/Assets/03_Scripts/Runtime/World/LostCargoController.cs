using UnityEngine;

namespace Aethernia
{
    public sealed class LostCargoController : MonoBehaviour
    {
        [SerializeField]private GameSession session;
        [SerializeField]private PrototypeDirector director;
        [SerializeField]private InteractionPoint npc,cargo;
        [SerializeField]private LostCargoView view;
        private bool cargoEncounter;
        public GameSession Session=>session;
        public InteractionPoint Npc=>npc;
        public InteractionPoint Cargo=>cargo;
        public bool CargoEncounter=>cargoEncounter;
        public int EncounterSeed { get; set; } // Runtime default 0 keeps normal randomness; validation may reproduce a seed.
        public void Configure(GameSession game,PrototypeDirector control,InteractionPoint villager,InteractionPoint load,LostCargoView presentation)
        {session=game;director=control;npc=villager;cargo=load;view=presentation;}
        private void OnEnable(){if(session==null)return;session.AdditionalInteraction=Interact;director.Encounter.Resolved+=Resolved;}
        private void OnDisable(){if(session==null)return;if(session.AdditionalInteraction==Interact)session.AdditionalInteraction=null;director.Encounter.Resolved-=Resolved;}
        private bool? Interact(string id,string title)
        {
            if(id==LostCargoQuest.NpcId){view.ShowDialogue();session.Say("Habitante de la aldea: conversación abierta.");return true;}
            if(id!=LostCargoQuest.CargoId)return null;
            if(LostCargoQuest.Stage(session.State)!=CargoQuestStage.Accepted){session.Say(LostCargoQuest.Objective(session.State));return false;}
            if(director.IsBusy||session.State.Health<=0)return false;
            view.CloseDialogue();cargoEncounter=true;
            // Spawn beside the approached crate, not through its collision volume.
            if(!director.Encounter.BeginAt(director.Hero.transform.position+Vector3.left*3,EncounterSeed)){cargoEncounter=false;return false;}
            return true;
        }
        public bool Accept()
        {
            if(!AtNpc()||!LostCargoQuest.Accept(session.State))return false;
            session.Say("Misión aceptada: "+LostCargoQuest.Title+". "+LostCargoQuest.Objective(session.State));view.CloseDialogue();return true;
        }
        public bool Claim()
        {
            if(!AtNpc()||!LostCargoQuest.Claim(session.State,out var ration))return false;
            session.Say("Misión completada: 20 oro"+(ration?" y una ración.":". Inventario lleno: no se añade ración."));return true;
        }
        private bool AtNpc()
        {
            if(session.Mode!=PrototypeMode.Exploration||director.IsBusy)return false;
            if(Vector3.Distance(director.Hero.transform.position,npc.transform.position)<=2.2f)return true;
            session.Say("Acércate al habitante para conversar.");return false;
        }
        private void Resolved(bool won)
        {
            if(!cargoEncounter)return;cargoEncounter=false;
            if(won&&LostCargoQuest.Secure(session.State))
            {director.Execute("explore");session.Say("Cargamento recuperado. Regresa al habitante para cobrar la recompensa.");}
            else session.Say("No recuperaste la carga. Vuelve a explorar; la misión aceptada puede reintentarse.");
        }
    }
}
