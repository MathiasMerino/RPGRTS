using UnityEngine;
namespace Aethernia
{
    // Presentation only. Rig, actor scale, colliders and navigation remain unchanged.
    public sealed class RaceVisualVariant:MonoBehaviour
    {
        private Vector3 baseline,expected;private bool captured;
        public static bool ValidScale(Transform model)
        {
            if((model.localScale-Vector3.one).sqrMagnitude<.0001f)return true;
            var variation=model.GetComponent<RaceVisualVariant>();
            return variation!=null&&variation.captured&&(variation.baseline-Vector3.one).sqrMagnitude<.0001f&&(model.localScale-variation.expected).sqrMagnitude<.0001f;
        }
        public static void Apply(Transform model,CivilizationCatalog catalog,string race)
        {if(model==null||catalog==null)return;var view=model.GetComponent<RaceVisualVariant>();if(view==null)view=model.gameObject.AddComponent<RaceVisualVariant>();if(!view.captured){view.baseline=model.localScale;view.captured=true;}view.expected=Vector3.Scale(view.baseline,catalog.Race(race)?.VisualScale??Vector3.one);model.localScale=view.expected;}
    }
}
