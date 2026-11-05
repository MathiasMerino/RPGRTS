using System;
using System.Collections;
using System.IO;
using System.Linq;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class CoreRefineTests
    {
        [Test]
        public void OpeningEnemyTurnAndBudgetRespectInitiative()
        {
            var profile=new ClassProfile("warrior","Guerrero",24,6,1.8f,3.2f);
            var battle=Enumerable.Range(1,100).Select(seed=>new TurnBattle(profile,24,0,0,seed,false))
                .First(b=>b.EnemyInitiative>b.HeroInitiative);
            Assert.That(battle.BeginOpeningEnemyTurn(),Is.True);
            Assert.That(battle.Attack(1),Is.False);
            Assert.That(battle.ResolveEnemyAction(false)&&battle.FinishEnemyTurn(),Is.True);
            Assert.That(battle.Round,Is.EqualTo(1));
            Assert.That(battle.Attack(3),Is.False);
            Assert.That(battle.ActionAvailable,Is.True);
            Assert.That(battle.Move(1.3f)&&battle.Attack(1.4f),Is.True);
            Assert.That(battle.LastHeroResolution.Defense,Is.EqualTo(12));
            Assert.That(battle.LastHeroResolution.Hit,Is.EqualTo(battle.LastHeroResolution.Roll+battle.LastHeroResolution.Modifier>=12));
            Assert.That(battle.Attack(1.4f),Is.False);
            Assert.That(battle.MovementRemaining,Is.EqualTo(2.7f).Within(.001f));
            Assert.That(battle.BeginEnemyTurn(),Is.True);
            Assert.That(battle.Round,Is.EqualTo(2));
        }

        [Test]
        public void FlankingRequiresOppositeNearbySameLevelPositions()
        {
            Assert.That(TurnEncounter.HasFlank(Vector3.left*1.2f,Vector3.right*1.5f),Is.True);
            Assert.That(TurnEncounter.HasFlank(Vector3.left,Vector3.left+Vector3.forward),Is.False);
            Assert.That(TurnEncounter.HasFlank(Vector3.left,Vector3.right*2.3f),Is.False);
            Assert.That(TurnEncounter.HasFlank(Vector3.left,Vector3.right+Vector3.up),Is.False);
        }

        [UnityTest] public IEnumerator RealDialogueAndCargoUseOneContextualHud()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.CoreRefineScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return Exercise();
        }

        private static IEnumerator Exercise()
        {
            const string output="Logs/COREREFINE01/Captures/";
            Directory.CreateDirectory(output);
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            var director=flow.Director;
            flow.SlotPath="Logs/COREREFINE01/qa-"+Guid.NewGuid().ToString("N")+".json";
            director.SavePathOverride=flow.SlotPath;
            Assert.That(flow.StartNew("Viajero","warrior","human"),Is.True);
            flow.CompleteIntro();
            var hud=flow.GetComponent<CoreTacticalHud>();
            var quest=Object.FindAnyObjectByType<LostCargoController>();
            var view=quest.GetComponent<LostCargoView>();
            yield return UxExperienceRoute.Wait(.45f);
            Assert.That(hud.Visible,Is.False);
            Assert.That(flow.ExistingHud.FindControl("attack").gameObject.activeInHierarchy,Is.False);
            yield return DemoPlayerCapture.Save(output+"Exploracion.png");
            yield return WorldExplorationRoute.Use(flow,LostCargoQuest.NpcId,NoCapture,"Mara");
            Assert.That(view.DialogueOpen&&flow.Session.WorldUiOpen,Is.True);
            Assert.That(flow.ExistingHud.FindControl("page-inventory").gameObject.activeInHierarchy,Is.False);
            foreach(var width in new[]{1280,1920,2560})
                yield return DemoCapture.Save(output+"Dialogo-"+width+".png",width,width*9/16,
                    validate:(cam,w,h)=>ValidateDialogue(view,cam));
            view.AcceptButton.onClick.Invoke();
            Assert.That(view.DialogueOpen||flow.Session.WorldUiOpen,Is.False);
            quest.EncounterSeed=42;
            yield return WorldExplorationRoute.Use(flow,LostCargoQuest.CargoId,NoCapture,"Cargo");
            var encounter=director.Encounter;
            yield return UxExperienceRoute.Wait(.2f);
            Assert.That(hud.Visible&&hud.VisibleActors==2,Is.True);
            Assert.That(flow.ExistingHud.transform.Find("Title Bar").gameObject.activeInHierarchy,Is.False);
            Assert.That(flow.ExistingHud.FindControl("attack").gameObject.activeInHierarchy,Is.False);
            Assert.That(Object.FindAnyObjectByType<CombatHudPanel>().Panel.activeInHierarchy,Is.False);
            Assert.That(Object.FindAnyObjectByType<ExpeditionPanel>().Controls["group-attack"].gameObject.activeInHierarchy,Is.False);
            Assert.That(hud.AttackButton.interactable,Is.False);
            Assert.That(encounter.TrySelect(encounter.Enemy.GetComponentInChildren<Collider>()),Is.True);
            var until=Time.realtimeSinceStartup+9;
            while(encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<until)yield return null;
            Assert.That(encounter.Battle.Phase,Is.EqualTo(TurnPhase.Player));
            var remaining=encounter.Battle.MovementRemaining;
            Assert.That(encounter.Move(new Vector3(600,0,600)),Is.False);
            Assert.That(encounter.Battle.MovementRemaining,Is.EqualTo(remaining));
            Assert.That(encounter.Battle.ActionAvailable,Is.True);
            encounter.Approach();
            yield return UxExperienceRoute.Arrive(director.Hero);
            yield return UxExperienceRoute.Wait(.22f);
            Assert.That(hud.AttackButton.interactable,Is.True);
            hud.AttackButton.onClick.Invoke();
            Assert.That(encounter.Battle.ActionAvailable,Is.False);
            Assert.That(GameObject.Find("CORE Resultado").GetComponent<TMP_Text>().text,Does.Contain("contra 12"));
            foreach(var width in new[]{1280,1920,2560})
                yield return DemoCapture.Save(output+"RPG-"+width+".png",width,width*9/16,
                    validate:(cam,w,h)=>ValidateButtons(hud,cam,w,h));
            Assert.That(hud.EndButton.interactable,Is.True);
            hud.EndButton.onClick.Invoke();
            Assert.That(encounter.Battle.Phase,Is.EqualTo(TurnPhase.Enemy));
            until=Time.realtimeSinceStartup+9;
            while(encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<until)yield return null;
            Assert.That(encounter.Battle.Phase,Is.EqualTo(TurnPhase.Player));
            Assert.That(encounter.Battle.ActionAvailable&&encounter.Enemy.Agent.isOnNavMesh,Is.True);
            var enemyPosition=encounter.EnemyPosition;
            encounter.Clear();
            flow.Session.State.Health=1;
            var seed=Enumerable.Range(1,100).First(n=>new TurnBattle(flow.Session.HeroProfile,1,0,0,n,true).Finished);
            Assert.That(encounter.BeginAt(enemyPosition,seed),Is.True);
            Assert.That(encounter.Battle.Phase,Is.EqualTo(TurnPhase.Enemy));
            until=Time.realtimeSinceStartup+9;
            while(encounter.Battle.Phase==TurnPhase.Enemy&&Time.realtimeSinceStartup<until)yield return null;
            Assert.That(encounter.Battle.Finished&&!encounter.Battle.Won,Is.True);
            yield return UxExperienceRoute.Wait(.2f);
            Assert.That(flow.Front.Screen,Is.EqualTo(DemoScreen.Retry));
            Assert.That(hud.Visible,Is.False);
            flow.Front.Controls["retry"].onClick.Invoke();
            Assert.That(flow.Session.Mode,Is.EqualTo(PrototypeMode.Exploration));
            Assert.That(flow.Session.State.Health,Is.GreaterThan(1));
            flow.Session.State.Demo.Chapter=DemoChapter.Settlement;
            flow.ExistingHud.FindControl("page-session").onClick.Invoke();
            flow.ExistingHud.FindControl("mode-construct").onClick.Invoke();
            yield return UxExperienceRoute.Wait(.2f);
            Assert.That(flow.Session.Mode,Is.EqualTo(PrototypeMode.Construction));
            Assert.That(flow.ExistingHud.FindControl("page-session").gameObject.activeInHierarchy,Is.False);
            Assert.That(flow.ExistingHud.CompactView.Resources.transform.parent.gameObject.activeInHierarchy,Is.False);
        }

        private static void ValidateDialogue(LostCargoView view,Camera camera)
        {
            var bridge=view.Dialogue.GetComponentsInChildren<LegacySdfPresenter>().Single(b=>b.Source==view.DialogueText);
            var panel=ScreenBounds((RectTransform)view.Dialogue.transform,camera);
            var body=ScreenBounds(bridge.Target.rectTransform,camera);
            var button=ScreenBounds((RectTransform)view.AcceptButton.transform,camera);
            bridge.Target.ForceMeshUpdate();
            Assert.That(bridge.Target.isTextOverflowing,Is.False);
            Assert.That(body.xMax,Is.LessThan(panel.xMax));
            Assert.That(body.yMin,Is.GreaterThan(button.yMax));
        }

        private static Rect ScreenBounds(RectTransform rect,Camera camera)
        {
            var corners=new Vector3[4];
            rect.GetWorldCorners(corners);
            var min=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);
            var max=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
            return Rect.MinMaxRect(min.x,min.y,max.x,max.y);
        }

        private static void ValidateButtons(CoreTacticalHud hud,Camera camera,int width,int height)
        {
            var root=hud.AttackButton.transform.parent;
            var initiative=GameObject.Find("CORE Iniciativa").GetComponentsInChildren<Button>();
            Assert.That(initiative.Length,Is.EqualTo(2));
            var controls=root.GetComponentsInChildren<Button>().Concat(initiative);
            var prior=new System.Collections.Generic.List<Rect>();
            var corners=new Vector3[4];
            foreach(var button in controls)
            {
                var label=button.GetComponentInChildren<TMP_Text>();
                label.ForceMeshUpdate();
                Assert.That(label.isTextOverflowing,Is.False,label.text);
                ((RectTransform)button.transform).GetWorldCorners(corners);
                var min=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);
                var max=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
                Assert.That(min.x,Is.GreaterThanOrEqualTo(-1));
                Assert.That(min.y,Is.GreaterThanOrEqualTo(-1));
                Assert.That(max.x,Is.LessThanOrEqualTo(width+1));
                Assert.That(max.y,Is.LessThanOrEqualTo(height+1));
                var bounds=Rect.MinMaxRect(min.x,min.y,max.x,max.y);
                Assert.That(prior.Any(r=>r.Overlaps(bounds)),Is.False,button.name);
                prior.Add(bounds);
            }
        }

        private static IEnumerator NoCapture(string _) {yield break;}
        [UnityTearDown] public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
