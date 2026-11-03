using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Aethernia
{
    // Presentación regional reversible; no modifica tiempo, navegación o estado de partida.
    public sealed class RegionalAtmosphere : MonoBehaviour
    {
        public DemoFlow Flow;public Light Sun;public Light CharacterFill;public Volume Grade;
        private Color sky,equator,ground,sunColor;private float intensity;private bool captured;
        public bool Dungeon=>Flow!=null&&Flow.IsPlaying&&Flow.Director.Hero.transform.position.x>65;
        private void OnEnable(){sky=RenderSettings.ambientSkyColor;equator=RenderSettings.ambientEquatorColor;ground=RenderSettings.ambientGroundColor;if(Sun!=null){sunColor=Sun.color;intensity=Sun.intensity;}captured=true;}
        private void LateUpdate()
        {
            var inside=Dungeon;if(CharacterFill!=null)CharacterFill.enabled=inside;var step=1-Mathf.Exp(-Time.unscaledDeltaTime*4);
            if(Sun!=null){Sun.intensity=Mathf.Lerp(Sun.intensity,inside ? .70f : Flow.UxEnabled?1.30f:1.08f,step);Sun.color=Color.Lerp(Sun.color,inside?new Color(.78f,.83f,.90f):new Color(1,.89f,.74f),step);}
            RenderSettings.ambientSkyColor=Color.Lerp(RenderSettings.ambientSkyColor,inside?new Color(.35f,.40f,.48f):new Color(.58f,.65f,.72f),step);
            RenderSettings.ambientEquatorColor=Color.Lerp(RenderSettings.ambientEquatorColor,inside?new Color(.25f,.29f,.34f):new Color(.34f,.36f,.32f),step);
            RenderSettings.ambientGroundColor=Color.Lerp(RenderSettings.ambientGroundColor,inside?new Color(.17f,.19f,.23f):new Color(.20f,.19f,.15f),step);
        }
        private void OnDisable(){if(CharacterFill!=null)CharacterFill.enabled=false;if(!captured)return;RenderSettings.ambientSkyColor=sky;RenderSettings.ambientEquatorColor=equator;RenderSettings.ambientGroundColor=ground;if(Sun!=null){Sun.color=sunColor;Sun.intensity=intensity;}}
    }
}
