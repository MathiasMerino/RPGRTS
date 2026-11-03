using UnityEngine;
namespace Aethernia
{
    // Presentation only. Rig, actor scale, colliders and navigation remain unchanged.
    public sealed class RaceVisualVariant:MonoBehaviour
    {
        private Vector3 baseline;private bool captured;
        public static void Apply(Transform model,CivilizationCatalog catalog,string race)
        {if(model==null||catalog==null)return;var view=model.GetComponent<RaceVisualVariant>();if(view==null)view=model.gameObject.AddComponent<RaceVisualVariant>();if(!view.captured){view.baseline=model.localScale;view.captured=true;}model.localScale=Vector3.Scale(view.baseline,catalog.Race(race)?.VisualScale??Vector3.one);}
    }
}
