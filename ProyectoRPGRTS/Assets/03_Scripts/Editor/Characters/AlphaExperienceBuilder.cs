using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Aethernia.Editor.Characters
{
    public static class AlphaExperienceBuilder
    {
        public static void Apply()
        {
            var scene=EditorSceneManager.OpenScene(AlphaSceneBuilder.ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();var view=flow.GetComponent<ModeTransitionView>();if(view==null)view=flow.gameObject.AddComponent<ModeTransitionView>();view.Flow=flow;EditorUtility.SetDirty(view);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
    }
}
