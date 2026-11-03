using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
namespace Aethernia.Tests.Editor
{
    public sealed class E18RtsScenarioTests
    {
        private int cap,sync;
        [UnitySetUp]public IEnumerator Setup(){cap=Application.targetFrameRate;sync=QualitySettings.vSyncCount;yield break;}
        [UnityTest]public IEnumerator ThreeClassesWithLegalBacklineOrdersAt30()=>DemoRtsTests.Patrol(null,30,false,true);
        [UnityTearDown]public IEnumerator Cleanup(){Application.targetFrameRate=cap;QualitySettings.vSyncCount=sync;if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
