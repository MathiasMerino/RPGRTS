using UnityEngine;
namespace Aethernia
{
    [RequireComponent(typeof(Renderer))]
    public sealed class VisualWindBounds:MonoBehaviour
    {
        [Min(0)]public float Margin=.12f;private Renderer target;private Bounds original;private bool captured;
        private void OnEnable(){target=GetComponent<Renderer>();original=target.localBounds;captured=true;var bounds=original;bounds.Expand(Margin);target.localBounds=bounds;}
        private void OnDisable(){if(captured&&target!=null)target.localBounds=original;captured=false;}
    }
}
