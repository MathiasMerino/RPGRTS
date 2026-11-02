using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    // Adaptador reversible de presentación. C02 conserva bindings, botones y su gestor.
    // El Text legado sigue siendo la fuente de datos; solo TMP dibuja sus caracteres.
    public sealed class LegacySdfPresenter : MonoBehaviour
    {
        public Text Source;
        public TMP_Text Target;
        public float MinimumSize=17;
        private string previous;
        private void OnEnable() { if(Source!=null)Source.enabled=false; }
        private void LateUpdate()
        {
            if(Source==null||Target==null)return;
            if(previous!=Source.text){previous=Source.text;Target.text=previous;}
            Source.enabled=false;Target.extraPadding=true;Target.color=Source.color;Target.fontSize=Mathf.Max(MinimumSize,Source.fontSize);
            Target.fontStyle=Source.fontStyle==FontStyle.Bold?FontStyles.Bold:FontStyles.Normal;
            Target.alignment=Source.alignment==TextAnchor.MiddleCenter?TextAlignmentOptions.Midline:Source.alignment==TextAnchor.MiddleLeft?TextAlignmentOptions.MidlineLeft:TextAlignmentOptions.TopLeft;
        }
        private void OnDisable(){if(Source!=null)Source.enabled=true;}
    }
}
