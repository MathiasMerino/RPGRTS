using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;
namespace Aethernia.Tests.Editor
{
    public sealed class E18RtsScenarioTests
    {
        [UnityTest]public IEnumerator ThreeClassesWithLegalBacklineOrdersAt30()=>DemoRtsTests.Patrol(null,30,false,true);
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
