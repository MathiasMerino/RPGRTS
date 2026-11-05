using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Aethernia
{
    // Independent quest dialogue/tracker; does not replace or patch C02's pending window behaviour.
    public sealed class LostCargoView : MonoBehaviour
    {
        public GameSession Session;
        public VerticalSliceHud Hud;
        public LostCargoController Quest;
        public GameObject Dialogue,Tracker;
        public Text DialogueText,ObjectiveText;
        public Button AcceptButton,ClaimButton,CloseButton;
        private bool open;
        public bool DialogueOpen=>open;
        private void OnEnable()
        {if(Session==null)return;Session.Changed+=Refresh;AcceptButton.onClick.AddListener(Accept);ClaimButton.onClick.AddListener(Claim);CloseButton.onClick.AddListener(CloseDialogue);Refresh();}
        private void OnDisable()
        {if(Session==null)return;ReleaseDialogue();Session.Changed-=Refresh;AcceptButton.onClick.RemoveListener(Accept);ClaimButton.onClick.RemoveListener(Claim);CloseButton.onClick.RemoveListener(CloseDialogue);}
        public void ShowDialogue(){Hud.ClosePanels();open=true;if(Session.Rules.CoreRefine)Session.WorldUiOpen=true;Refresh();}
        public void CloseDialogue(){ReleaseDialogue();Refresh();}
        private void ReleaseDialogue(){var owned=open;open=false;if(owned&&Session.Rules.CoreRefine)Session.WorldUiOpen=false;}
        private void Accept(){Quest.Accept();Refresh();}
        private void Claim(){Quest.Claim();Refresh();}
        private void Update(){if(open&&Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame)CloseDialogue();}
        private void LateUpdate()
        {if(Hud!=null)Tracker.SetActive(!Session.Rules.UxExperience&&!open&&!Hud.CompactView.WindowOpen&&!Hud.CompactView.HelpOpen);}
        private void Refresh()
        {
            if(Session==null)return;if(Session.Mode!=PrototypeMode.Exploration)ReleaseDialogue();
            Dialogue.SetActive(open);var stage=LostCargoQuest.Stage(Session.State);
            ObjectiveText.text=LostCargoQuest.Title+" · provisional\n"+LostCargoQuest.Objective(Session.State);
            AcceptButton.gameObject.SetActive(stage==CargoQuestStage.Available);ClaimButton.gameObject.SetActive(stage==CargoQuestStage.Recovered);
            DialogueText.text=stage==CargoQuestStage.Available?"Un arcón de suministros se perdió al oeste de la casa. Hay un saqueador cerca. ¿Puedes recuperar la carga?\n\nTe daré 20 oro y una ración si tienes espacio.":stage==CargoQuestStage.Accepted?"Busca el arcón en las afueras, al oeste de la casa. Derrota al saqueador y vuelve con la carga.":stage==CargoQuestStage.Recovered?"Has recuperado nuestros suministros. Entrega la carga para recibir la recompensa.":"Gracias. La carga está a salvo y ya has recibido tu recompensa.";
            if(Session.Rules.FrontierCampaign)DialogueText.text="Mara · vecina\n"+DialogueText.text;
        }
    }
}
