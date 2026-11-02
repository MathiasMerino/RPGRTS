using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
namespace Aethernia.Tests.Editor
{
    // Comprueba coherencia de victoria/derrota/reintento; no exige ganar todas las cargas.
    public sealed class E17RtsInvestigationTests
    {
        private int cap,sync;
        [UnitySetUp]public IEnumerator Setup(){cap=Application.targetFrameRate;sync=QualitySettings.vSyncCount;yield break;}
        [UnityTest]public IEnumerator ThreeClassesDefault()=>DemoRtsTests.Patrol(validateTerminalOnly:true);
        [UnityTest]public IEnumerator ThreeClasses30()=>DemoRtsTests.Patrol(frameCap:30,validateTerminalOnly:true);
        [UnityTest]public IEnumerator ThreeClasses120()=>DemoRtsTests.Patrol(frameCap:120,validateTerminalOnly:true);
        [UnityTest]public IEnumerator MageIsolated30()=>DemoRtsTests.Patrol(new[]{"mage"},30,validateTerminalOnly:true);
        [UnityTest]public IEnumerator MageIsolated120()=>DemoRtsTests.Patrol(new[]{"mage"},120,validateTerminalOnly:true);
        [UnityTearDown]public IEnumerator Cleanup(){Application.targetFrameRate=cap;QualitySettings.vSyncCount=sync;if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
