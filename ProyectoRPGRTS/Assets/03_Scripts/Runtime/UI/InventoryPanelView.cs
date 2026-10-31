using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    // Selection is presentation state; inventory and equipment still belong to PrototypeState.
    public sealed class InventoryPanelView : MonoBehaviour
    {
        public Button[] Slots;
        public Text[] Labels;
        public Text Description;
        public MedievalUiTheme Theme;
        private static readonly string[] ids={"ration","training-sword","cloth"};
        public string SelectedId { get; private set; }="ration";
        public void Select(string id) { if(System.Array.IndexOf(ids,id)>=0) SelectedId=id; }
        public int Count(PrototypeState state,string id)
        { var stack=state.Items.Find(i=>i.Id==id); return stack==null?0:stack.Count; }
        public bool Shows(string action) => action=="trade" || action=="ration"&&SelectedId=="ration" || action=="sword"&&SelectedId=="training-sword" || action=="armour"&&SelectedId=="cloth";
        public void Render(PrototypeState state)
        {
            for(var i=0;i<ids.Length;i++)
            {
                var count=Count(state,ids[i]);var chosen=SelectedId==ids[i];
                Slots[i].interactable=count>0;
                Labels[i].text=(chosen?"SELECCIONADO\n":"\n")+WorldOperations.ItemTitle(ids[i])+"\n"+(count>0?"Cantidad: "+count:"No disponible");
                Labels[i].fontStyle=chosen?FontStyle.Bold:FontStyle.Normal;
                Labels[i].color=chosen?Theme.Gold:Theme.Text;
            }
            var amount=Count(state,SelectedId);
            var effect=SelectedId=="ration"?"Consumible · recupera hasta 4 PV.\nSe consume al utilizarlo.":SelectedId=="training-sword"?"Arma de entrenamiento · +1 al daño.\nSe muestra al equiparla.":"Armadura de tela · +1 a la defensa.\nSe muestra al equiparla.";
            var equipped=SelectedId==state.Weapon||SelectedId==state.Armour;
            Description.text=WorldOperations.ItemTitle(SelectedId)+"\n\n"+effect+"\n\n"+(amount>0?"Disponible: "+amount:"No quedan unidades.")+(equipped?" · EQUIPADO":"")+"\n\nSelecciona una pieza y usa la acción inferior.\nComercio: 5 oro por 2 madera.";
        }
    }
}
