using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Aethernia
{
    public sealed class ClassCardView : MonoBehaviour
    {
        public Image Figure,Outline;public TMP_Text Title,Role,Selection;public Button Button;
        public void Bind(ClassPresentation art,System.Action select)
        {Figure.sprite=art.Figure;Figure.preserveAspect=true;Title.text=art.Title;Role.text=art.Role;Button.onClick.AddListener(()=>select());}
        public void Select(bool selected)
        {Outline.enabled=selected;Selection.text=selected?"SELECCIONADO":"ELEGIR CLASE";Selection.color=selected?new Color(.88f,.74f,.46f):new Color(.7f,.7f,.67f);}
    }
}
