using UnityEngine;
namespace Aethernia
{
    public sealed class TorchGlow : MonoBehaviour
    {
        public Light Source;public float BaseIntensity=2.2f;
        private void Update(){if(Source!=null)Source.intensity=BaseIntensity*(1+.045f*Mathf.Sin(Time.time*3.1f+transform.position.x));}
        private void OnDisable(){if(Source!=null)Source.intensity=BaseIntensity;}
    }
}
