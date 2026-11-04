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
using Object = UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public sealed class IntegratedAlphaTests
    {
        [Test]
        public void CandidateUsesTheLivingWorldWithoutPrivateDependencies()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.LivingScenePath);
            var prior=Object.FindAnyObjectByType<DemoFlow>().Session.Rules;
            EditorSceneManager.OpenScene(WorldExplorationBuilder.IntegratedAlphaScenePath);
            var alpha=Object.FindAnyObjectByType<DemoFlow>().Session.Rules;
            Assert.That(prior.IntegratedAlpha,Is.False);
            Assert.That(alpha.IntegratedAlpha&&alpha.LivingWorld&&alpha.FrontierCampaign&&alpha.TacticalWarfare,Is.True);
            Assert.That(alpha.Progression,Is.SameAs(prior.Progression));
            Assert.That(alpha.Civilizations,Is.SameAs(prior.Civilizations));
            Assert.That(AssetDatabase.GetDependencies(WorldExplorationBuilder.IntegratedAlphaScenePath,true)
                .Any(p=>p.Contains("LocalOnly")||p.Contains("Meshy")),Is.False);
        }

        [UnityTest]
        public IEnumerator PromptsAndContractGuidanceUseTheRealWorld()
        {
            EditorSceneManager.OpenScene(WorldExplorationBuilder.IntegratedAlphaScenePath);
            yield return new EnterPlayMode();
            yield return null;
            yield return VerifyWorld();
        }

        private static IEnumerator VerifyWorld()
        {
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            flow.SlotPath="Logs/E23/alpha-"+Guid.NewGuid().ToString("N")+".json";
            Assert.That(flow.Continue("../Docs/Validacion/UX02/PartidasFinales/human-warrior.json"),Is.True);
            var world=flow.GetComponent<WorldExploration>();
            yield return WorldExplorationRoute.Use(flow,FrontierContracts.Board,CaptureBoardPrompt,"Board");
            Directory.CreateDirectory("Logs/E23/Layout");
            foreach(var width in new[]{1280,1920,2560})
                yield return DemoCapture.Save("Logs/E23/Layout/Board-"+width+".png",width,width*9/16,
                    validate:(_,w,h)=>ValidateBoard(world,w,h));
            world.ConfirmButton.onClick.Invoke();
            world.Close();
            Assert.That(world.TryContractGuidance(false,out var objective,out var position),Is.True);
            Assert.That(objective,Does.Contain("Mojón del bosque"));
            Assert.That(position,Is.EqualTo(Point(FrontierContracts.Cairn).transform.position));
            yield return WorldExplorationRoute.Use(flow,FrontierContracts.Cairn,NoCapture,"Cairn");
            world.ConfirmButton.onClick.Invoke();
            Assert.That(world.TryContractGuidance(false,out objective,out position),Is.True);
            Assert.That(objective,Does.Contain("Entrega"));
            Assert.That(position,Is.EqualTo(Point(FrontierContracts.Board).transform.position));
        }

        private static void ValidateBoard(WorldExploration world,int width,int height)
        {
            var controls=new[]{world.ConfirmButton,world.AlternativeButton,world.CloseButton};
            var canvas=controls[0].GetComponentInParent<Canvas>().rootCanvas;
            var corners=new Vector3[4];
            foreach(var control in controls)
            {
                var label=control.GetComponentInChildren<TMP_Text>();
                label.ForceMeshUpdate();
                Assert.That(label.isTextOverflowing,Is.False,width+" "+label.text);
                ((RectTransform)control.transform).GetWorldCorners(corners);
                foreach(var corner in corners)
                {
                    var screen=RectTransformUtility.WorldToScreenPoint(canvas.worldCamera,corner);
                    Assert.That(screen.x,Is.InRange(0f,(float)width));
                    Assert.That(screen.y,Is.InRange(0f,(float)height));
                }
            }
        }

        private static InteractionPoint Point(string id)=>Object.FindObjectsByType<InteractionPoint>().Single(p=>p.FactId==id);
        private static IEnumerator CaptureBoardPrompt(string _)
        {
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            Assert.That(flow.GetComponent<UxExperienceView>().PromptText,Does.StartWith("E — Consultar"));
            yield break;
        }
        private static IEnumerator NoCapture(string _) { yield break; }
        [UnityTearDown] public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
