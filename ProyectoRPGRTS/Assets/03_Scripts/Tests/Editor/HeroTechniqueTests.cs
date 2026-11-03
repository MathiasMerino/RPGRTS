using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;
namespace Aethernia.Tests.Editor
{
    public sealed class HeroTechniqueTests
    {
        private PrototypeRules rules;private ProgressionRules config;
        [SetUp]public void Setup(){rules=ScriptableObject.CreateInstance<PrototypeRules>();config=ScriptableObject.CreateInstance<ProgressionRules>();rules.Progression=config;}
        [TearDown]public void Dispose(){Object.DestroyImmediate(rules);Object.DestroyImmediate(config);}
        private PrototypeState State(string id)
        {var s=new PrototypeState{ClassId=id,Health=rules.Find(id).Health};ProgressionOperations.Activate(s,config);s.Facts.Add("rpg-victory");ProgressionOperations.GrantForFact(s,config,"rpg-victory");return s;}
        [TestCase("warrior"),TestCase("monk"),TestCase("mage")]
        public void TechniqueCostsAnActionAndFiniteFocusOnlyOnValidExecution(string id)
        {
            var state=State(id);var profile=ProgressionOperations.ResolveProfile(state,rules);var b=new TurnBattle(profile,profile.Health,1,1,42,false);var focus=b.TechniqueFocus;
            Assert.That(b.UseTechnique(float.NaN),Is.False);Assert.That(b.UseTechnique(profile.Combat.TechniqueRange+1),Is.False);Assert.That(b.TechniqueFocus,Is.EqualTo(focus));
            Assert.That(b.UseTechnique(profile.Combat.TechniqueRange),Is.True);Assert.That(b.TechniqueFocus,Is.EqualTo(focus-1));Assert.That(b.ActionAvailable,Is.False);Assert.That(b.LastAbilityId,Is.EqualTo(profile.Combat.TechniqueId));Assert.That(b.UseTechnique(0),Is.False);Assert.That(b.LastHeroResolution.Damage,Is.EqualTo(b.LastHeroResolution.Hit?Math.Min(18,profile.Damage+1+profile.Combat.TechniqueDamage):0));
            b.EndTurn(false);Assert.That(b.UseTechnique(0),Is.True);Assert.That(b.TechniqueFocus,Is.Zero);if(!b.Finished)b.EndTurn(false);Assert.That(b.UseTechnique(0),Is.False);Assert.That(b.TechniqueFocus,Is.Zero);
            state.HeroLevel=1;state.Experience=0;state.Facts.Clear();profile=ProgressionOperations.ResolveProfile(state,rules);Assert.That(new TurnBattle(profile,profile.Health,1,1,42,false).UseTechnique(0),Is.False);
        }
        [Test]public void GroupTechniqueRejectsAlliesObstaclesAndOutOfTurnActors()
        {
            var profile=ProgressionOperations.ResolveProfile(State("mage"),rules);var hp=profile.Health;var enemyHp=80;
            var hero=new TacticalCombatant{Id="hero",Hero=true,Profile=profile,ReadHealth=()=>hp,WriteHealth=v=>hp=v};
            var enemy=new TacticalCombatant{Id="enemy",Hostile=true,Profile=new ClassProfile("enemy","Guardián",80,2,1.5f,2),ReadHealth=()=>enemyHp,WriteHealth=v=>enemyHp=v};var group=new GroupTurnSequence(new[]{hero,enemy},42);
            while(!group.Current.Hero)group.EndTurn();Assert.That(group.UseTechnique(hero,1,true),Is.False);Assert.That(group.UseTechnique(enemy,1,false),Is.False);Assert.That(group.TechniqueFocus,Is.EqualTo(2));Assert.That(group.UseTechnique(enemy,8,true),Is.True);Assert.That(group.TechniqueFocus,Is.EqualTo(1));Assert.That(group.UseTechnique(enemy,1,true),Is.False);group.EndTurn();Assert.That(group.UseTechnique(hero,1,true),Is.False);
        }
        [UnityTest]public IEnumerator RealControllersAndUiUseSameProgressionInRpgAndRts()=>Controllers();
        [UnityTest]public IEnumerator SkillSheetUsesRealPointsAndFitsTwoResolutions()=>Sheet();
        private static IEnumerator Sheet()
        {
            EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;var flow=Object.FindAnyObjectByType<DemoFlow>();flow.SlotPath="Logs/E18/sheet-"+Guid.NewGuid().ToString("N")+".json";flow.Director.SavePathOverride=flow.SlotPath;Directory.CreateDirectory("Logs/E18/Captures");
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/"+id+".json",flow.Session.Rules,out var state),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);flow.ExistingHud.SelectPage(ExplorationPage.Character);yield return null;yield return null;
                var sheet=flow.GetComponent<ProgressionPanelView>();Assert.That(sheet.Impact.gameObject.activeInHierarchy&&sheet.Impact.interactable,Is.True);var hp=flow.Session.State.Health;sheet.Impact.onClick.Invoke();Assert.That(ProgressionOperations.Rank(flow.Session.State,"impact"),Is.EqualTo(1));Assert.That(flow.Session.State.Health,Is.EqualTo(hp));Assert.That(ProgressionOperations.AvailablePoints(flow.Session.State),Is.Zero);
                foreach(var width in new[]{1280,1920})yield return DemoCapture.Save("Logs/E18/Captures/Skill-"+id+"-"+width+".png",width,width*9/16,null,(c,w,h)=>{foreach(var button in new[]{sheet.Impact,sheet.Discipline}){var corners=new Vector3[4];button.GetComponent<RectTransform>().GetWorldCorners(corners);var min=RectTransformUtility.WorldToScreenPoint(c,corners[0]);var max=RectTransformUtility.WorldToScreenPoint(c,corners[2]);Assert.That(min.x,Is.GreaterThanOrEqualTo(0));Assert.That(min.y,Is.GreaterThanOrEqualTo(0));Assert.That(max.x,Is.LessThanOrEqualTo(w));Assert.That(max.y,Is.LessThanOrEqualTo(h));}foreach(var text in sheet.Impact.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,text.name+": "+text.text);}});
                flow.ExistingHud.ClosePanels();yield return null;Assert.That(sheet.Impact.gameObject.activeInHierarchy,Is.False);Assert.That(flow.Director.Save(flow.SlotPath),Is.True);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(ProgressionOperations.Rank(flow.Session.State,"impact"),Is.EqualTo(1));
            }
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator Controllers()
        {
            EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;
            flow.SlotPath="Logs/E18/tech-"+Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E14E15/EntradaE13/"+id+".json",flow.Session.Rules,out var state),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,state,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);var level=flow.Session.State.HeroLevel;Assert.That(level,Is.GreaterThanOrEqualTo(2));
                Assert.That(d.Encounter.Begin(42),Is.True);yield return null;d.Encounter.Approach();yield return ExpeditionIntegrationTests.Arrive(d.Hero);Assert.That(d.Execute("technique"),Is.True);Assert.That(d.Encounter.TechniqueFocus,Is.EqualTo(1));Assert.That(d.Execute("technique"),Is.False);d.Encounter.Clear();flow.Session.SetMode(PrototypeMode.Exploration);
                Assert.That(flow.BeginPatrol(),Is.True);var rts=d.Rts;var statuses=new List<RtsUnitStatus>();rts.CopyStatus(statuses);var foe=statuses.First(s=>s.Team==1).Unit;
                Assert.That(d.Hero.Warp(foe.transform.position+Vector3.back*1.2f),Is.True);Assert.That(rts.HeroAction("technique"),Is.True);Assert.That(rts.TechniqueFocus,Is.EqualTo(1));Assert.That(rts.HeroAction("technique"),Is.False);Assert.That(flow.Session.State.HeroLevel,Is.EqualTo(level));rts.Clear();flow.Session.Paused=false;flow.Session.SetMode(PrototypeMode.Exploration);
                Assert.That(d.Save(flow.SlotPath),Is.True);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.State.HeroLevel,Is.EqualTo(level));
            }
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Cleanup(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
