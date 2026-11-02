using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.TestTools;
namespace Aethernia.Tests.Editor
{
    public sealed class AlphaQualityTests
    {
        [Test]public void QualityProfilesHaveRealUrpDifferences()
        {
            Assert.That(QualitySettings.names,Is.EqualTo(new[]{"Bajo","Medio","Alto"}));var names=QualitySettings.names;
            for(var i=0;i<3;i++){var asset=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(AlphaPipelineBuilder.Root+"/Graphics/"+names[i]+".asset");Assert.That(asset.shadowDistance,Is.EqualTo(new[]{20,40,60}[i]));Assert.That(asset.msaaSampleCount,Is.EqualTo(new[]{1,2,4}[i]));Assert.That(asset.renderScale,Is.EqualTo(i==0?.8f:1f));Assert.That(asset.supportsSoftShadows,Is.EqualTo(i==2));Assert.That(asset.maxAdditionalLightsCount,Is.EqualTo(new[]{1,2,4}[i]));}
            var low=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(AlphaPipelineBuilder.Root+"/Graphics/LowRenderer.asset");Assert.That(low.rendererFeatures.All(f=>!f.isActive),Is.True);var original=AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/PC_Renderer.asset");Assert.That(original.rendererFeatures.Any(f=>f.isActive),Is.True,"Original renderer untouched");
        }
        [UnityTest]public IEnumerator OptionActuallyChangesPipelineAndCamera()=>ApplyProfiles();
        private static IEnumerator ApplyProfiles()
        {
            EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;var flow=Object.FindAnyObjectByType<DemoFlow>();var old=QualitySettings.GetQualityLevel();var before=JsonUtility.ToJson(flow.Session.State);Directory.CreateDirectory("Logs/E17/Captures/Quality");
            for(var i=0;i<3;i++){flow.Front.Preferences.Quality=i;flow.Front.Preferences.Apply(flow.Front.Audio,flow.CameraRig,false);yield return null;yield return null;var rp=GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;Assert.That(rp,Is.Not.Null);Assert.That(rp.msaaSampleCount,Is.EqualTo(new[]{1,2,4}[i]));Assert.That(Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing,Is.EqualTo(i>0));Assert.That(Camera.main.GetUniversalAdditionalCameraData().antialiasing,Is.EqualTo(AntialiasingMode.None));Assert.That(JsonUtility.ToJson(flow.Session.State),Is.EqualTo(before));}
            QualitySettings.SetQualityLevel(old,true);LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
