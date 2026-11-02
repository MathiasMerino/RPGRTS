using System;
using System.IO;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class ExpeditionBuilder
    {
        public const string ScenePath="Assets/01_Scenes/AetherniaExpeditions.unity";
        public const string Root="Assets/11_Expeditions";
        public static void Create()
        {
            if(File.Exists(ScenePath))throw new InvalidOperationException("Expedición ya creada; no regenerar sobre assets existentes.");
            var scene=EditorSceneManager.OpenScene(FirstDemoBuilder.ScenePath);var flow=Object.FindAnyObjectByType<DemoFlow>();
            if(!AssetDatabase.IsValidFolder(Root))AssetDatabase.CreateFolder("Assets","11_Expeditions");
            var rules=ScriptableObject.CreateInstance<ExpeditionRules>();AssetDatabase.CreateAsset(rules,Root+"/Rules.asset");
            var host=new GameObject("E14 E15 Expedition integration");var party=host.AddComponent<PartyWorld>();party.Flow=flow;party.Rules=rules;party.Feedback=Object.FindAnyObjectByType<CombatFeedbackPresenter>();
            flow.Director.Encounter.ConfigureGroup(party,party.Feedback);
            var dungeon=host.AddComponent<DungeonExpedition>();dungeon.Flow=flow;dungeon.Party=party;dungeon.Rules=rules;
            var territory=host.AddComponent<TerritoryCampaign>();territory.Flow=flow;territory.Party=party;territory.Rules=rules;
            var panel=host.AddComponent<ExpeditionPanel>();panel.Flow=flow;panel.Party=party;panel.Dungeon=dungeon;panel.Territory=territory;
            var env=new GameObject("E14 Ruinas del Umbral · provisional");
            var stone=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/GreyStone.mat");var dark=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/DarkOak.mat");
            Box(env.transform,"Stone floor",new Vector3(80,-.15f,0),new Vector3(24,.3f,16),stone);
            Box(env.transform,"West wall",new Vector3(68,1.5f,0),new Vector3(.4f,3,16),stone);Box(env.transform,"East wall",new Vector3(92,1.5f,0),new Vector3(.4f,3,16),stone);
            Box(env.transform,"North wall",new Vector3(80,1.5f,8),new Vector3(24,3,.4f),stone);Box(env.transform,"South wall",new Vector3(80,1.5f,-8),new Vector3(24,3,.4f),stone);
            foreach(var x in new[]{74f,82f})foreach(var z in new[]{-5f,5f})Box(env.transform,"Corridor wall",new Vector3(x,1.5f,z),new Vector3(.4f,3,6),stone);
            foreach(var x in new[]{78f,86f})foreach(var z in new[]{-4.7f,4.7f})
            {var column=GameObject.CreatePrimitive(PrimitiveType.Cylinder);column.name="Stone column 3m";column.transform.SetParent(env.transform);column.transform.position=new Vector3(x,1.5f,z);column.transform.localScale=new Vector3(.6f,1.5f,.6f);column.GetComponent<Renderer>().sharedMaterial=stone;}
            foreach(var x in new[]{72f,78f,86f,90f})
            {var torch=new GameObject("Warm torch");torch.transform.SetParent(env.transform);torch.transform.position=new Vector3(x,2.2f,6.8f);var light=torch.AddComponent<Light>();light.type=LightType.Point;light.range=8;light.intensity=1.8f;light.color=new Color(1,.65f,.28f);light.shadows=LightShadows.None;Box(torch.transform,"Sconce",torch.transform.position,new Vector3(.2f,.6f,.2f),dark);}
            Marker(env.transform,"umbral-exit","Salida al campamento",new Vector3(71,0,-4),dark);
            Marker(env.transform,"umbral-battle-1","Guardianía del corredor",new Vector3(76,0,-1),dark);Marker(env.transform,"umbral-battle-2","Guardianía de la cámara",new Vector3(84,0,-1),dark);
            Marker(env.transform,"umbral-cache","Alijo opcional",new Vector3(71,0,4),dark);Marker(env.transform,"umbral-objective","Objetivo de las ruinas",new Vector3(89,0,4),dark);
            var nav=env.AddComponent<NavMeshSurface>();nav.collectObjects=CollectObjects.Children;nav.useGeometry=NavMeshCollectGeometry.PhysicsColliders;nav.BuildNavMesh();AssetDatabase.CreateAsset(nav.navMeshData,Root+"/RuinsNav.asset");
            var sites=new GameObject("E14 E15 World access points");Marker(sites.transform,"umbral-entry","Ruinas del Umbral · E",new Vector3(4,0,-11),stone);
            Marker(sites.transform,"territory-crossing","Paso neutral · E",rules.Find("crossing").Position,dark);Marker(sites.transform,"territory-outpost","Puesto enemigo · E",rules.Find("outpost").Position,stone);
            EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();AssetDatabase.Refresh();Debug.Log("EXPEDITION_CREATED: new scene/nav/rules; original scene/metadata retained. Functional validation pending.");
        }
        private static void Box(Transform parent,string name,Vector3 point,Vector3 size,Material material)
        {var box=GameObject.CreatePrimitive(PrimitiveType.Cube);box.name=name;box.transform.SetParent(parent,true);box.transform.position=point;box.transform.localScale=size;box.GetComponent<Renderer>().sharedMaterial=material;}
        private static void Marker(Transform parent,string id,string title,Vector3 position,Material material)
        {
            var root=new GameObject(title);root.transform.SetParent(parent);root.transform.position=position;var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);cube.transform.SetParent(root.transform,false);cube.transform.localPosition=Vector3.up*.65f;cube.transform.localScale=new Vector3(.65f,1.3f,.65f);cube.GetComponent<Renderer>().sharedMaterial=material;cube.GetComponent<Collider>().isTrigger=true;
            var interaction=root.AddComponent<InteractionPoint>();interaction.Configure(id,title);
            var label=new GameObject("World label");label.transform.SetParent(root.transform,false);label.transform.localPosition=Vector3.up*1.7f;var text=label.AddComponent<TextMeshPro>();text.font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FirstDemoBuilder.FontPath);text.text=title;text.fontSize=2.2f;text.rectTransform.sizeDelta=new Vector2(5,1.4f);text.alignment=TextAlignmentOptions.Center;label.transform.rotation=Quaternion.Euler(35.26439f,45,0);
        }
        public static void Build()
        {
            Directory.CreateDirectory("Builds/ExpeditionsWindows");var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/ExpeditionsWindows/Aethernia.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
            if(r.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("Expedition build failed "+r.summary.result);
            File.WriteAllText("Logs/Expedition-build.txt","PASS; bytes="+r.summary.totalSize+"; seconds="+r.summary.totalTime.TotalSeconds);
        }
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);var root=GameObject.Find("E14 Ruinas del Umbral · provisional");
            if(root==null)throw new InvalidOperationException("Missing authored ruins");
            foreach(var name in new[]{"West wall","South wall"})
            {
                var wall=root.transform.Find(name);wall.GetComponent<Renderer>().enabled=false;
                if(root.transform.Find(name+" Cutaway")!=null)continue;
                var size=wall.localScale;size.y=.65f;Box(root.transform,name+" Cutaway",new Vector3(wall.position.x,.325f,wall.position.z),size,wall.GetComponent<Renderer>().sharedMaterial);
                var visual=root.transform.Find(name+" Cutaway");Object.DestroyImmediate(visual.GetComponent<Collider>());
            }
            // También recortar las divisiones del primer plano: ocultaban al héroe
            // junto a la guardianía aunque la interfaz dejara libre su proyección.
            foreach(var wall in root.GetComponentsInChildren<BoxCollider>())
            {
                if(wall.name!="Corridor wall"||wall.transform.position.z>=0)continue;
                var original=wall.GetComponent<Renderer>();original.enabled=false;
                if(wall.transform.Find("Cutaway stone")!=null)continue;
                var visual=GameObject.CreatePrimitive(PrimitiveType.Cube);visual.name="Cutaway stone";visual.transform.SetParent(wall.transform,false);
                visual.transform.localPosition=new Vector3(0,(-1+.65f/3)/2,0);visual.transform.localScale=new Vector3(1,.65f/3,1);visual.GetComponent<Renderer>().sharedMaterial=original.sharedMaterial;Object.DestroyImmediate(visual.GetComponent<Collider>());
            }
            // UV por metro: conserva el material compartido y el collider del suelo original.
            const string path=Root+"/RuinsFloor.asset";var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(mesh==null)
            {
                mesh=new Mesh{name="Ruins floor 24x16m UV2m"};mesh.vertices=new[]{new Vector3(-12,0,-8),new Vector3(-12,0,8),new Vector3(12,0,8),new Vector3(12,0,-8)};mesh.triangles=new[]{0,1,2,0,2,3};mesh.uv=new[]{Vector2.zero,new Vector2(0,8),new Vector2(12,8),new Vector2(12,0)};mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);
            }
            var floor=root.transform.Find("Stone floor");floor.GetComponent<Renderer>().enabled=false;
            if(root.transform.Find("Stone floor surface")==null){var surface=new GameObject("Stone floor surface",typeof(MeshFilter),typeof(MeshRenderer));surface.transform.SetParent(root.transform);surface.transform.position=new Vector3(80,.002f,0);surface.GetComponent<MeshFilter>().sharedMesh=mesh;surface.GetComponent<Renderer>().sharedMaterial=floor.GetComponent<Renderer>().sharedMaterial;}
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("EXPEDITION_REFINED: visual cutaway only; original wall colliders/nav retained; floor UV 2m.");
        }
    }
}
