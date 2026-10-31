using System;
using System.IO;
using Aethernia.Editor.Environment;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace Aethernia.Editor
{
    public static class ProductionVillageBuilder
    {
        public const string ScenePath="Assets/01_Scenes/AetherniaVillageDemo.unity";
        public const string Prefabs="Assets/02_Prefabs/Quests";
        private static MedievalUiTheme theme;
        public static void Create()
        {
            if(File.Exists(ScenePath))throw new InvalidOperationException("Village demo already exists; do not regenerate.");
            var scene=EditorSceneManager.OpenScene(PlayablePrototypeBuilder.ScenePath);EnvironmentMaterials.Folder(Prefabs);
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var hud=Object.FindAnyObjectByType<VerticalSliceHud>();theme=hud.CompactView.Theme;
            var root=new GameObject("E05 Lost Cargo Demo");var controller=root.AddComponent<LostCargoController>();
            var npc=Point(root.transform,"Village Resident",LostCargoQuest.NpcId,"Habitante de la aldea",new Vector3(-7.5f,0,-3),true);
            var cargo=Point(root.transform,"Lost Cargo",LostCargoQuest.CargoId,"Cargamento perdido",new Vector3(-10,0,3),false);
            var view=root.AddComponent<LostCargoView>();view.Session=d.Session;view.Hud=hud;view.Quest=controller;
            var tracker=Panel(hud.transform,"Quest Tracker",16,96,350,102,theme.InsetSprite);tracker.GetComponent<Image>().raycastTarget=false;view.Tracker=tracker.gameObject;
            view.ObjectiveText=Text(tracker,"Quest Objective",16,10,318,82,15);view.ObjectiveText.color=theme.Text;
            var panel=Panel(hud.transform,"Cargo Dialogue",330,210,620,300,theme.PanelSprite);view.Dialogue=panel.gameObject;
            var title=Text(panel,"NPC Title",24,18,572,40,22);title.fontStyle=FontStyle.Bold;title.color=theme.Gold;title.text="HABITANTE · MISIÓN PROVISIONAL";
            view.DialogueText=Text(panel,"Dialogue Text",28,76,564,120,18);
            view.AcceptButton=Button(panel,"Accept Cargo Quest","Aceptar misión",28,230,270);
            view.ClaimButton=Button(panel,"Claim Cargo Reward","Entregar cargamento",28,230,270);
            view.CloseButton=Button(panel,"Close Cargo Dialogue","Cerrar",310,230,282);
            controller.Configure(d.Session,d,npc,cargo,view);
            PrefabUtility.SaveAsPrefabAsset(panel.gameObject,Prefabs+"/CargoDialogue.prefab");panel.gameObject.SetActive(false);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene,ScenePath);AssetDatabase.SaveAssets();
            Debug.Log("E05_CREATED: separate village demo, reused D01/NavMesh/assets, NPC and cargo interaction, independent C02-styled quest dialogue. Original scene preserved.");
        }
        private static InteractionPoint Point(Transform parent,string name,string id,string title,Vector3 at,bool npc)
        {
            var savedPath=Prefabs+"/"+(npc?"VillageResident":"LostCargo")+".prefab";
            var existing=AssetDatabase.LoadAssetAtPath<GameObject>(savedPath);
            if(existing!=null){var reused=(GameObject)PrefabUtility.InstantiatePrefab(existing,parent);reused.transform.position=at;return reused.GetComponent<InteractionPoint>();}
            var node=new GameObject(name);node.transform.SetParent(parent,false);node.transform.position=at;
            if(npc)
            {
                var collider=node.AddComponent<CapsuleCollider>();collider.radius=.25f;collider.height=1.8f;collider.center=Vector3.up*.9f;
                var visual=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CharacterVisualBuilder.Prefabs+"/warrior_Visual.prefab"),node.transform);
                visual.name="Resident Visual (reused E01)";
            }
            else
            {
                var collider=node.AddComponent<BoxCollider>();collider.size=new Vector3(.8f,.8f,.65f);collider.center=Vector3.up*.4f;
                var visual=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/Environment/Medieval/Props/Chest_80x80x60cm.prefab");
                if(visual==null)throw new InvalidOperationException("D01 chest prefab missing");PrefabUtility.InstantiatePrefab(visual,node.transform);
            }
            var obstacle=node.AddComponent<NavMeshObstacle>();obstacle.carving=true;obstacle.shape=NavMeshObstacleShape.Box;obstacle.size=npc?new Vector3(.55f,1.8f,.55f):new Vector3(.85f,.8f,.70f);obstacle.center=Vector3.up*(npc?.9f:.4f);
            var point=node.AddComponent<InteractionPoint>();point.Configure(id,title);
            var label=new GameObject("World Label",typeof(RectTransform),typeof(Canvas));label.transform.SetParent(node.transform,false);label.transform.localPosition=Vector3.up*(npc?2.25f:1.25f);label.transform.rotation=Camera.main.transform.rotation;label.transform.localScale=Vector3.one*.008f;
            var canvas=label.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.sortingOrder=-10;var rect=label.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(270,52);
            var text=Text(label.transform,"Name",0,0,270,52,22);text.alignment=TextAnchor.MiddleCenter;text.fontStyle=FontStyle.Bold;text.text=title;
            PrefabUtility.SaveAsPrefabAsset(node,savedPath);return point;
        }
        public static void Build()
        {
            Directory.CreateDirectory("Builds/Windows");var report=UnityEditor.BuildPipeline.BuildPlayer(new UnityEditor.BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/Windows/Aethernia.exe",target=BuildTarget.StandaloneWindows64,options=UnityEditor.BuildOptions.Development});
            if(report.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("Village build failed: "+report.summary.result);
            File.WriteAllText("Logs/BuildVillage-summary.txt","Succeeded; bytes="+report.summary.totalSize+"; seconds="+report.summary.totalTime.TotalSeconds);
        }
        public static void Polish()
        {
            foreach(var path in new[]{Prefabs+"/VillageResident.prefab",Prefabs+"/LostCargo.prefab"})
            {var root=PrefabUtility.LoadPrefabContents(path);try{foreach(var canvas in root.GetComponentsInChildren<Canvas>(true))canvas.sortingOrder=-10;PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}}
            var scene=EditorSceneManager.OpenScene(ScenePath);var q=Object.FindAnyObjectByType<LostCargoController>();
            foreach(var canvas in q.GetComponentsInChildren<Canvas>(true)){canvas.sortingOrder=-10;PrefabUtility.RecordPrefabInstancePropertyModifications(canvas);}
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        private static Transform Panel(Transform parent,string name,float x,float y,float width,float height,Sprite sprite)
        {var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);Rect(go.transform,x,y,width,height);var image=go.GetComponent<Image>();image.sprite=sprite;image.type=Image.Type.Sliced;image.pixelsPerUnitMultiplier=3;return go.transform;}
        private static Text Text(Transform parent,string name,float x,float y,float width,float height,int size)
        {var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);Rect(go.transform,x,y,width,height);var t=go.GetComponent<Text>();t.font=theme.Font;t.fontSize=size;t.color=theme.Text;t.raycastTarget=false;return t;}
        private static Button Button(Transform parent,string name,string label,float x,float y,float width)
        {var t=Panel(parent,name,x,y,width,40,theme.ButtonSprite);var button=t.gameObject.AddComponent<Button>();var text=Text(t,"Label",6,5,width-12,30,17);text.alignment=TextAnchor.MiddleCenter;text.text=label;return button;}
        private static void Rect(Transform t,float x,float y,float width,float height)
        {var r=(RectTransform)t;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(width,height);}
    }
}
