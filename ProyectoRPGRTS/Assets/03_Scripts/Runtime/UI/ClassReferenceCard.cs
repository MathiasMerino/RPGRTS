using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    // Presentation within the existing character page; class selection and rules stay in the HUD/session.
    public sealed class ClassReferenceCard : MonoBehaviour
    {
        [SerializeField] private CharacterVisualCatalog catalog;
        [SerializeField] private RawImage portrait;
        [SerializeField] private AspectRatioFitter aspect;
        [SerializeField] private Text information;
        [SerializeField] private bool portraitOnly;
        public void UsePortraitCaption() { portraitOnly=true; }
        public RawImage Portrait => portrait;
        public Text Information => information;
        public void Configure(CharacterVisualCatalog data, RawImage image, AspectRatioFitter fitter, Text text)
        { catalog=data; portrait=image; aspect=fitter; information=text; }
        public void Refresh(bool characterPage, ClassProfile profile)
        {
            var entry=catalog==null || profile==null ? null : catalog.FindEntry(profile.Id);
            var visible=characterPage && entry!=null && entry.ReferenceArt!=null;
            gameObject.SetActive(visible); if(!visible) return;
            portrait.texture=entry.ReferenceArt; portrait.uvRect=entry.ReferenceUv;
            aspect.aspectRatio=entry.ReferenceArt.width*entry.ReferenceUv.width/(entry.ReferenceArt.height*entry.ReferenceUv.height);
            information.text=portraitOnly ? entry.ReferenceCaption : profile.Title+" · "+profile.Health+" PV base\nDaño base "+profile.Damage+"\n"+profile.Range.ToString("0.#")+" m · "+profile.Speed.ToString("0.#")+" m/s\nReferencia 2D provisional";
        }
    }
}
