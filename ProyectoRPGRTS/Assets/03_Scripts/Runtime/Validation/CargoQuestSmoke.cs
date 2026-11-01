using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Aethernia
{
    // Development build probe only, invoked by the existing explicit -aetherniaSmoke route.
    public static class CargoQuestSmoke
    {
        public static IEnumerator Run(PrototypeDirector d,LostCargoController q,string output)
        {
            var baseline=output+".cargo-base.json";Require(d.Save(baseline),"base save");q.EncounterSeed=42;
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                Require(d.Load(baseline),"base load");d.Session.SelectClass(id);d.Hero.Select(true);
                Require(d.Hero.TryMoveTo(q.Npc.transform.position+Vector3.back*1.1f),"NPC path");yield return Arrive(d.Hero);
                Require(q.Npc.TryInteract(d.Hero,d.Session)&&q.Accept(),"dialogue/accept");
                Require(d.Save(output+".cargo-accepted.json")&&d.Load(output+".cargo-accepted.json"),"accepted persistence");
                Require(d.Hero.TryMoveTo(q.Cargo.transform.position+Vector3.back*1.6f),"cargo path");yield return Arrive(d.Hero);
                Require(q.Cargo.TryInteract(d.Hero,d.Session),"encounter");d.Encounter.Approach();yield return Arrive(d.Hero);
                for(var turn=0;turn<30&&q.CargoEncounter;turn++)
                {
                    if(d.Session.State.Health<8&&!d.Encounter.Battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                    if(q.CargoEncounter&&d.Encounter.Battle!=null&&!d.Encounter.Battle.Finished)
                    {
                        if(d.Encounter.UsesStagedCommands)
                        {
                            Require(d.Execute("end"),"staged enemy request");var phaseLimit=Time.realtimeSinceStartup+5;
                            while(d.Encounter.Battle!=null&&d.Encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<phaseLimit)yield return null;
                            Require(d.Encounter.Battle==null||d.Encounter.Battle.Phase!=TurnPhase.Enemy,"enemy phase timeout");
                        }
                        else d.Encounter.EndTurn();
                    }
                    yield return null;
                }
                Require(LostCargoQuest.Stage(d.Session.State)==CargoQuestStage.Recovered&&d.Session.Mode==PrototypeMode.Exploration,"actual victory/return: "+id);
                Require(d.Save(output+".cargo-recovered.json")&&d.Load(output+".cargo-recovered.json"),"recovered persistence");
                Require(d.Hero.TryMoveTo(q.Npc.transform.position+Vector3.back*1.1f),"return path");yield return Arrive(d.Hero);
                var gold=d.Session.State.Gold;Require(q.Claim()&&!q.Claim()&&d.Session.State.Gold==gold+20,"unique reward");
                Require(d.Save(output+".cargo-completed.json")&&d.Load(output+".cargo-completed.json"),"completed persistence");
                Require(LostCargoQuest.Stage(d.Session.State)==CargoQuestStage.Completed,"completion restored");
            }
            File.WriteAllText(output+".cargo.txt","PASS: standalone actual NPC/path/quest/combat/reward/v1 save-load for monk, warrior and mage; staged enemy phases="+d.Encounter.UsesStagedCommands+"; normal RPG/RTS/economy also checked by base probe.");
        }
        private static IEnumerator Arrive(ExplorationUnit hero)
        {var limit=Time.realtimeSinceStartup+15;yield return null;while(hero.IsMoving&&Time.realtimeSinceStartup<limit)yield return null;Require(!hero.IsMoving,"move timeout");}
        private static void Require(bool ok,string step){if(!ok)throw new InvalidOperationException("Cargo quest smoke: "+step);}
    }
}
