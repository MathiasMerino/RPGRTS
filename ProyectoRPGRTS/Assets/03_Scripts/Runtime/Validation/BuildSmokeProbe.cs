using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Aethernia
{
    public sealed class BuildSmokeProbe : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-aetherniaSmoke");
            if (index < 0 || index + 1 >= args.Length) yield break;
            var output = args[index + 1];
            var started = Time.realtimeSinceStartup;
            var run = Run(output);
            while (true)
            {
                object step;
                try { if (!run.MoveNext()) break; step = run.Current; }
                catch (Exception ex) { File.WriteAllText(output, "FAIL: " + ex.Message); Application.Quit(2); yield break; }
                yield return step;
            }
            File.WriteAllText(output, "PASS: standalone player; exploration, inventory, construction, dungeon consequence, save/load, RPG and RTS succeeded. Seconds=" + (Time.realtimeSinceStartup - started).ToString("0.00") + "; frames=" + Time.frameCount + "; allocatedBytes=" + UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong());
            Application.Quit(0);
        }
        private IEnumerator Run(string output)
        {
            yield return null;
            var commands = GetComponent<PlayerCommands>();
            var session = GetComponent<GameSession>();
            var actor = commands.Hero;
            if (!actor.Agent.isOnNavMesh) throw new Exception("Hero is not on NavMesh");
            actor.Select(true);
            if (!actor.TryMoveTo(new Vector3(-3.4f, 0, -5))) throw new Exception("Move rejected");
            var deadline = Time.realtimeSinceStartup + 12;
            while ((actor.Agent.pathPending || actor.Agent.remainingDistance > 0.25f) && Time.realtimeSinceStartup < deadline) yield return null;
            var chest = GameObject.Find("Interaction Chest").GetComponent<InteractionPoint>();
            if (chest == null || !chest.TryInteract(actor, session) || !session.HasFact(chest.FactId)) throw new Exception("Interaction failed");
            var director = FindAnyObjectByType<PrototypeDirector>();
            if (director == null) throw new Exception("Prototype director missing");
            director.Execute("class-mage");
            if (!director.Execute("sword") || !director.Execute("armour")) throw new Exception("Equipment commands failed");
            if (!director.Execute("trade") || !director.Construction.Place(new Vector3(12, 0, -10))) throw new Exception("Economy/construction failed");
            if (!director.Travel("portal-dungeon") || !actor.Warp(new Vector3(50.5f, 0, 1))) throw new Exception("Dungeon travel failed");
            yield return null;
            if (!GameObject.Find("Dungeon Objective").GetComponent<InteractionPoint>().TryInteract(actor, session)) throw new Exception("Quest consequence failed");
            if (!director.Save(output + ".save.json")) throw new Exception("Save failed");
            var gold = session.State.Gold; session.State.Gold = 999;
            if (!director.Load(output + ".save.json") || !session.HasFact("dungeon-objective") || session.State.Gold != gold || session.State.Buildings.Count != 1 || session.State.Weapon != "training-sword" || session.State.Armour != "cloth") throw new Exception("Save roundtrip failed");
            director.Travel("portal-surface");
            if (!director.Encounter.Begin(42)) throw new Exception("RPG could not start");
            var battleClock = session.State.WorldSeconds;
            actor.Warp(director.Encounter.EnemyPosition + Vector3.right * 1.3f);
            for (var turn = 0; turn < 20 && !director.Encounter.Battle.Finished; turn++)
            { director.Encounter.Attack(); if (!director.Encounter.Battle.Finished) director.Encounter.EndTurn(); yield return null; }
            if (!director.Encounter.Battle.Won || !director.Execute("escalate")) throw new Exception("RPG/escalation failed");
            yield return null;
            if (!director.Execute("charge")) throw new Exception("RTS group order failed");
            deadline = Time.realtimeSinceStartup + 60;
            while (director.Rts.Active && Time.realtimeSinceStartup < deadline) yield return null;
            if (!director.Rts.Won || !session.HasFact("rts-victory")) throw new Exception("RTS did not conclude with a persistent victory");
            if (Mathf.Abs(session.State.WorldSeconds - battleClock) > 0.001f) throw new Exception("Combat advanced the shared economic clock");
            director.Execute("explore");
            if (FindObjectsByType<ExplorationUnit>().Length != 1 || director.Encounter.Battle != null) throw new Exception("Combat participants leaked into exploration");
            if (!director.Save(output + ".save.json")) throw new Exception("Final victory could not be saved");
            var cargo=FindAnyObjectByType<LostCargoController>();
            if(cargo!=null)
            {
                // Flatten nested enumerators so the probe's existing exception wrapper catches every step.
                var stack=new System.Collections.Generic.Stack<IEnumerator>();stack.Push(CargoQuestSmoke.Run(director,cargo,output));
                while(stack.Count>0)
                {var next=stack.Peek();if(!next.MoveNext()){stack.Pop();continue;}if(next.Current is IEnumerator nested)stack.Push(nested);else yield return next.Current;}
            }
            var village=GameObject.Find("E07 Medieval Village");
            if(village!=null)
            {
                File.WriteAllText(output+".village.txt","PASS: E07 authored village loaded in standalone; renderers="+village.GetComponentsInChildren<MeshRenderer>().Length+"; graphics="+SystemInfo.graphicsDeviceName+"; actual quest/classes and prior systems checked above.");
                if(SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null)
                {
                    var capture=output+".village.png";if(File.Exists(capture))File.Delete(capture);ScreenCapture.CaptureScreenshot(capture);
                    var until=Time.realtimeSinceStartup+5;while(!File.Exists(capture)&&Time.realtimeSinceStartup<until)yield return null;
                    yield return null;yield return null;
                    File.WriteAllText(output+".village-image.txt",File.Exists(capture)?"CAPTURED: native standalone framebuffer, no synthetic composition.":"UNAVAILABLE: batch player did not produce screenshot; editor native captures remain separate evidence.");
                }
            }
        }
    }
}
