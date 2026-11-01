using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Aethernia
{
    public static class TacticalCombatSmoke
    {
        public static IEnumerator Run(PrototypeDirector d,string output)
        {
            Require(d.Encounter.UsesStagedCommands,"staged scene");d.Execute("explore");d.Session.SelectClass("warrior");Require(d.Hero.Warp(new Vector3(-3.4f,0,-5)),"warp");yield return null;
            Require(d.Hero.Warp(new Vector3(-11.8f,0,1.4f)),"cargo clearing");yield return null;
            Require(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.left*1.2f,42),"begin");Camera.main.GetComponent<IsometricCameraRig>().FocusOn(d.Hero.transform.position);yield return new WaitForSeconds(.5f);
            var audio=UnityEngine.Object.FindAnyObjectByType<CombatAudioPlayer>();Require(audio!=null&&audio.Bank!=null&&audio.Bank.Clips.Length==14,"original audio bank");
            audio.Play(FeedbackSound.Confirm,Vector3.zero,true);var peak=0f;var samples=new float[1024];var until=Time.realtimeSinceStartup+.20f;
            while(Time.realtimeSinceStartup<until)
            {AudioListener.GetOutputData(samples,0);foreach(var sample in samples)peak=Mathf.Max(peak,Mathf.Abs(sample));yield return null;}
            var before=d.Encounter.Battle.EnemyHealth;d.Encounter.Attack();Require(d.Encounter.Battle.LastHeroResolution.Accepted,"legal selected attack");var damage=d.Encounter.Battle.LastHeroResolution.Damage;d.Encounter.Attack();Require(d.Encounter.Battle.EnemyHealth==before-damage,"single damage");
            Require(d.Execute("end"),"request");Require(d.Encounter.Battle.Phase==TurnPhase.Enemy&&!d.Execute("end"),"enemy/duplicate gate");
            if(SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null)
            {yield return new WaitForSeconds(1.1f);var shot=StandaloneGameCapture.Save(output+".tactical.png");while(shot.MoveNext())yield return shot.Current;}
            until=Time.realtimeSinceStartup+5;while(d.Encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<until)yield return null;
            Require(d.Encounter.Battle.Phase==TurnPhase.Player,"enemy finishes without clip callback");
            var ui=UnityEngine.Object.FindAnyObjectByType<CombatHudPanel>();Require(ui!=null&&ui.HealthFill.sprite!=null&&Mathf.Abs(ui.HealthFill.fillAmount-d.Encounter.Battle.EnemyHealth/18f)<.001f,"real hp bar");
            Require(audio.ActiveVoices<=6&&UnityEngine.Object.FindAnyObjectByType<CombatFeedbackPresenter>().ActiveEffects<=8,"budgets");
            // La misión con tres clases y turnos escalonados ya se comprobó en CargoQuestSmoke.
            File.WriteAllText(output+".tactical.txt","PASS: E09 real staged turn, AI decision="+d.Encounter.LastEnemyIntent+"; duplicate end/attack rejected; authoritative damage once; HP bar; 14 original clips, voices<=6/effects<=8; native output peak="+peak.ToString("F6")+"; DSP/editor evidence checked separately, human hearing pending.");
        }
        private static void Require(bool ok,string step){if(!ok)throw new InvalidOperationException("E09 player: "+step);}
    }
}
