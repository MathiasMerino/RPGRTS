using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace Aethernia
{
    // Solo lo invoca el recorrido explícito de validación de la build.
    public static class AnimatedCharacterSmoke
    {
        public static IEnumerator Run(PrototypeDirector d,string output)
        {
            var baseline=output+".animated-base.json";Require(d.Save(baseline),"baseline save");var report=new StringBuilder("PASS: E08 standalone actual animation checks\n");
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                Require(d.Load(baseline),"baseline load");d.Session.SelectClass(id);Require(d.Hero.Warp(new Vector3(-3,0,-6)),"initial nav position");yield return null;yield return null;yield return null;
                var model=d.Hero.GetComponent<CharacterVisualPresenter>().Model;var driver=model.GetComponent<HumanoidAnimationDriver>();
                Require(driver!=null&&driver.CanAnimate&&driver.Rig.IsUsable,"rig/profile "+id);Require(!driver.Rig.Animator.applyRootMotion,"root motion off");
                Require(d.Hero.TryMoveTo(new Vector3(-8,0,-6)),"path");var limit=Time.realtimeSinceStartup+10;var walk=false;var run=false;
                while(d.Hero.IsMoving&&Time.realtimeSinceStartup<limit){yield return null;walk|=driver.ObservedSpeed>.05f&&driver.ObservedSpeed<driver.Profile.RunSpeed;run|=driver.ObservedSpeed>=driver.Profile.RunSpeed;}
                Require(walk&&run&&!d.Hero.IsMoving,"real walk/run/arrival");yield return new WaitForSeconds(.25f);Require(driver.Rig.Animator.GetInteger("Locomotion")==0,"idle after arrival");
                var rig=Camera.main.GetComponent<IsometricCameraRig>();rig.FocusOn(d.Hero.transform.position);rig.ApplyInput(Vector2.zero,rig.Size-5,0,false);
                if(SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null)
                {var image=StandaloneGameCapture.Save(output+"."+id+".png");while(image.MoveNext())yield return image.Current;}
                var presenter=d.Hero.GetComponent<CharacterCombatAnimationPresenter>();var before=presenter.PlayedActions;
                Require(d.Encounter.BeginAt(d.Hero.transform.position+Vector3.forward*1.6f,42),"encounter");yield return new WaitForSeconds(.5f);d.Encounter.Attack();
                Require(!d.Encounter.Battle.ActionAvailable,"accepted command consumes action");yield return new WaitForSeconds(.22f);
                Require(presenter.PlayedActions>before,"actual attack cue");Require(driver.Rig.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")||driver.Rig.Animator.IsInTransition(0),"Attack state");
                var after=presenter.PlayedActions;d.Encounter.Attack();yield return new WaitForSeconds(.1f);Require(presenter.PlayedActions==after,"rejected repeat does not animate");
                yield return new WaitForSeconds(driver.Profile.Attack.length+.15f);d.Encounter.EndTurn();yield return new WaitForSeconds(.6f);d.Encounter.Guard();yield return new WaitForSeconds(.25f);Require(driver.Rig.Animator.GetBool("Defending"),"guard state");
                for(var turn=0;turn<40&&!d.Encounter.Battle.Finished;turn++){d.Encounter.EndTurn();yield return new WaitForSeconds(.05f);}
                Require(d.Session.State.Health==0,"real combat defeat");yield return new WaitForSeconds(1.5f);Require(driver.Rig.Animator.GetCurrentAnimatorStateInfo(0).IsName("Defeat"),"Defeat state");
                d.Execute("explore");Require(d.Save(output+".animated-"+id+".json")&&d.Load(output+".animated-"+id+".json"),"defeated v1 roundtrip");
                report.AppendLine(id+": valid Generic/skin, actual walk/run/idle, accepted/rejected attack, guard, actual damage/defeat, save v1; root motion off.");
            }
            Require(d.Load(baseline),"restore baseline");File.WriteAllText(output+".animated.txt",report.ToString());
        }
        private static void Require(bool ok,string step){if(!ok)throw new InvalidOperationException("Animated characters: "+step);}
    }
}
