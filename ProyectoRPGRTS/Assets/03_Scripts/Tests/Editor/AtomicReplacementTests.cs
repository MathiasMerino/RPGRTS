using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Aethernia.Tests.Editor
{
    public sealed class AtomicReplacementTests
    {
        [Test] public void NativeLockedDestinationRecoversAndLoadsNewState()
        {
            var root="Logs/Atomic-native-"+Guid.NewGuid().ToString("N");var path=Path.Combine(root,"save.json");var rules=ScriptableObject.CreateInstance<PrototypeRules>();
            FileStream held=null;System.Threading.Tasks.Task release=null;
            try
            {
                var state=new PrototypeState();Assert.That(PrototypeSaveStore.Save(path,state,rules),Is.True);
                held=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);var captured=held;
                release=System.Threading.Tasks.Task.Run(()=>{System.Threading.Thread.Sleep(200);captured.Dispose();});state.Gold=73;
                Assert.That(PrototypeSaveStore.Save(path,state,rules),Is.True,PrototypeSaveStore.LastWriteFailure);
                Assert.That(PrototypeSaveStore.LastWriteRetries,Is.GreaterThan(0),"Bloqueo real del archivo, no solo excepción simulada");
                Assert.That(PrototypeSaveStore.Load(path,rules,out var saved),Is.True);Assert.That(saved.Gold,Is.EqualTo(73));Assert.That(saved.Demo,Is.Null);
            }
            finally{release?.GetAwaiter().GetResult();held?.Dispose();UnityEngine.Object.DestroyImmediate(rules);}
        }
        [TestCase(32)][TestCase(33)][TestCase(1175)] public void TransientFileErrorRetriesTheSameOperation(int code)
        {
            var root="Logs/Atomic-"+Guid.NewGuid().ToString("N");Directory.CreateDirectory(root);var temp=Path.Combine(root,"save.tmp");var target=Path.Combine(root,"save.json");File.WriteAllText(temp,"new");File.WriteAllText(target,"old");
            var calls=0;var waits=new List<int>();var retries=0;
            AtomicFileReplacement.Replace(temp,target,n=>retries=n,(source,destination)=>
            {Assert.That(source,Is.EqualTo(temp));Assert.That(destination,Is.EqualTo(target));Assert.That(File.ReadAllText(target),Is.EqualTo("old"));if(++calls<3)throw Error(code);},waits.Add);
            Assert.That(calls,Is.EqualTo(3));Assert.That(retries,Is.EqualTo(2));Assert.That(waits,Is.EqualTo(new[]{50,100}));Assert.That(File.ReadAllText(temp),Is.EqualTo("new"));
        }
        [Test] public void ExhaustedRetryKeepsBothFilesAndRemainsBounded()
        {
            var root="Logs/Atomic-"+Guid.NewGuid().ToString("N");Directory.CreateDirectory(root);var temp=Path.Combine(root,"save.tmp");var target=Path.Combine(root,"save.json");File.WriteAllText(temp,"new");File.WriteAllText(target,"old");
            var calls=0;var waits=new List<int>();Assert.Throws<IOException>(()=>AtomicFileReplacement.Replace(temp,target,null,(a,b)=>{calls++;throw Error(1175);},waits.Add));
            Assert.That(calls,Is.EqualTo(5));Assert.That(waits,Is.EqualTo(new[]{50,100,200,400}));Assert.That(File.ReadAllText(target),Is.EqualTo("old"));Assert.That(File.ReadAllText(temp),Is.EqualTo("new"));
        }
        [Test] public void PermanentPermissionErrorIsNotRetried()
        {
            var calls=0;Assert.Throws<UnauthorizedAccessException>(()=>AtomicFileReplacement.Replace("unused","unused",null,(a,b)=>{calls++;throw new UnauthorizedAccessException();},_=>Assert.Fail("No retry")));Assert.That(calls,Is.EqualTo(1));
        }
        private static IOException Error(int code)=>new IOException("Injected filesystem failure",unchecked((int)0x80070000)|code);
    }
}
