using TMPro;
using UnityEngine;

namespace Aethernia
{
    public sealed class DemoGuidance : MonoBehaviour
    {
        public DemoFlow Flow;
        private GameObject panel;
        private TMP_Text label;
        private void Start()
        {
            var ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);
            var rect=DemoWidgets.Rect(Flow.ExistingHud.transform,"Demo contextual guidance",16,218,350,112);panel=rect.gameObject;
            ui.Surface(rect,new Color(.035f,.043f,.046f,.94f),false).raycastTarget=false;label=ui.Text(rect,"Guidance","",12,8,326,96,17);
        }
        private void LateUpdate()
        {
            if(panel==null)return;var state=Flow.Session.State;var visible=Flow.IsPlaying&&!Flow.LegacyProbe&&state.Demo!=null&&!Flow.ExistingHud.CompactView.WindowOpen&&!Flow.ExistingHud.CompactView.HelpOpen;
            panel.SetActive(visible);if(!visible)return;
            var text="";var battle=Flow.Director.Encounter.Battle;
            if(Flow.Session.Mode==PrototypeMode.Rts)text=(Flow.Director.Rts.ContextId=="conquest-outpost"?"CONQUISTA DEL PUESTO":"DIRIGE LA PATRULLA")+"\nClic o arrastre: seleccionar. Shift: añadir. Clic derecho: mover/atacar. Selecciona tu héroe para usar Guardia o Recuperar.";
            else if(Flow.Director.Encounter.InGroup)text="GRUPO EN COMBATE\nSelecciona objetivo y posición. Una acción y4m por turno. Los enemigos actúan automáticamente.";
            else if(Flow.Director.Hero.transform.position.x>65)text="RUINAS DEL UMBRAL\nE: guardianías, objetivo y salida. Vence ambos encuentros; viajar conserva salud.";
            else if(state.Demo.Chapter==DemoChapter.FirstCombat&&battle!=null)
            {
                if(battle.Phase==TurnPhase.Enemy)text="TURNO DEL RIVAL\nEspera a que termine. El daño y las tiradas aparecen en el panel del encuentro.";
                else if(!battle.ActionAvailable)text="ACCIÓN REALIZADA\nPulsa Fin de turno. La guardia reduce el siguiente daño; Recuperar se usa una vez.";
                else text="TU PRIMER ENCUENTRO\nSelecciona al rival. Acércate o usa clic derecho si no llegas; después Atacar o Guardia.";
            }
            else if(state.Demo.Chapter==DemoChapter.RoadToRts)text="EL SENDERO ABIERTO\nWASD: caminar · E: interactuar cerca. Busca la señal de patrulla al norte (−12, 8). P: pausa/opciones.";
            else if(state.Demo.Chapter==DemoChapter.FirstRts)text="DIRIGE LA PATRULLA\nClic o arrastre: seleccionar. Shift: añadir. Clic derecho: mover o atacar al rival.";
            else if(state.Demo.Chapter==DemoChapter.Settlement)text="UN LUGAR SEGURO\nFunda el campamento, levanta un taller y activa la producción. No es necesario convertirse en gobernante.";
            else text="TU PROPIO CAMINO\nExplora, habla con el habitante y busca El cargamento perdido. F5/F9: guardar/cargar fuera de combates.";
            if(label.text!=text)label.text=text;
        }
    }
}
