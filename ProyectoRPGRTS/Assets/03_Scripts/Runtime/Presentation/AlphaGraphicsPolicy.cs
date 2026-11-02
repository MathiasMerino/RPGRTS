using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Aethernia
{
    // Complements the real URP assets; does not recreate or manage the options UI.
    public sealed class AlphaGraphicsPolicy:MonoBehaviour
    {
        public Camera MainCamera;
        private int previous=-1;
        private void LateUpdate()
        {
            var level=QualitySettings.GetQualityLevel();if(level==previous||MainCamera==null)return;previous=level;
            var cameraData=MainCamera.GetUniversalAdditionalCameraData();cameraData.renderPostProcessing=level>0;cameraData.antialiasing=AntialiasingMode.None;MainCamera.allowMSAA=true;
        }
    }
}
