using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Aethernia.Tests.Editor
{
    internal sealed class RtsTraceRecorder:IDisposable
    {
        private readonly RtsBattleController controller;private readonly DemoFlow flow;private readonly string id;private readonly int? cap;
        private readonly List<RtsTraceSample> samples=new List<RtsTraceSample>(32768);
        public RtsTraceRecorder(RtsBattleController value,DemoFlow game,string classId,int? frameCap){controller=value;flow=game;id=classId;cap=frameCap;controller.Diagnostic+=Record;}
        private void Record(RtsTraceSample row)=>samples.Add(row);
        public void Dispose()
        {
            controller.Diagnostic-=Record;Directory.CreateDirectory("Logs/E17/RTS");var path="Logs/E17/RTS/"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmssfff")+"-"+id+"-"+(cap?.ToString()??"default")+"-"+Guid.NewGuid().ToString("N");
            var b=new StringBuilder("kind,frame,time,delta,actor,hero,hp,attackAt,selected,recoveryUsed,target,targetHp,value,x,z,allies,enemies\n");
            foreach(var r in samples)b.AppendFormat(CultureInfo.InvariantCulture,"{0},{1},{2:F5},{3:F5},{4},{5},{6},{7:F5},{8},{9},{10},{11},{12},{13:F4},{14:F4},{15},{16}\n",r.Kind,r.Frame,r.Time,r.Delta,r.Actor,r.Hero,r.Health,r.AttackAt,r.Selected,r.RecoveryUsed,r.Target,r.TargetHealth,r.Value,r.Position.x,r.Position.z,r.Allies,r.Enemies);
            File.WriteAllText(path+".csv",b.ToString());
            File.WriteAllText(path+".summary.txt","class="+id+"; requestedFrameCap="+(cap?.ToString()??"default")+"; RPGseed="+flow.EncounterSeed+"; health="+flow.Session.State.Health+"; chapter="+flow.Session.State.Demo.Chapter+"; active="+controller.Active+"; recoverAccepted="+samples.Count(r=>r.Kind=="recover-accepted")+"; recoverCooldownRejected="+samples.Count(r=>r.Kind=="recover-cooldown")+"; strikes="+samples.Count(r=>r.Kind=="strike")+"; rows="+samples.Count);
        }
    }
}
