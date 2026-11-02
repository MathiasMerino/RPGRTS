using System;
using System.IO;
using System.Threading;

namespace Aethernia
{
    internal static class AtomicFileReplacement
    {
        // ReplaceFile puede fallar momentáneamente por bloqueos/filtros de Windows.
        // Repetir la misma operación atómica; nunca borrar el destino para forzar un guardado.
        internal static void Replace(string temporary,string destination,Action<int> onRetry=null,Action<string,string> replace=null,Action<int> wait=null)
        {
            replace=replace??((source,target)=>File.Replace(source,target,null));wait=wait??Thread.Sleep;
            for(var attempt=0;;attempt++)
            {
                try{replace(temporary,destination);return;}
                catch(IOException e) when(attempt<4&&Retryable(e)&&File.Exists(temporary))
                {onRetry?.Invoke(attempt+1);wait(50<<attempt);}
            }
        }
        private static bool Retryable(IOException error)
        {var code=error.HResult&0xffff;return code==32||code==33||code==1175;}
    }
}
