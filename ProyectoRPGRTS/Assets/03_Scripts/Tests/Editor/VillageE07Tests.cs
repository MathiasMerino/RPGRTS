using System.Collections;
using System.IO;
using System.Text;
using System.Linq;
using Aethernia;
using Aethernia.Editor.Environment;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;

namespace Aethernia.Tests.Editor
{
    public class VillageE07Tests
    {
        public const string RootName="E07 Medieval Village";
        [Test]public void ReusableLibraryHasSharedUrpMaterialsAndMetricArchitecture()
        {
            var root=VillageE07Materials.Root;
            Assert.That(AssetDatabase.FindAssets("t:Texture2D",new[]{root+"/Textures"}).Length,Is.EqualTo(5));
            foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{root+"/Textures"}))
            {var p=AssetDatabase.GUIDToAssetPath(guid);var i=(TextureImporter)AssetImporter.GetAtPath(p);Assert.That(i.maxTextureSize,Is.EqualTo(1024));Assert.That(i.mipmapEnabled,Is.True);Assert.That(i.wrapMode,Is.EqualTo(TextureWrapMode.Repeat));}
            foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{VillageE07Materials.Prefabs}))
            {
                var go=AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));Assert.That(go.transform.localScale,Is.EqualTo(Vector3.one));
                foreach(var t in go.GetComponentsInChildren<Transform>(true))Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject),Is.Zero);
                foreach(var f in go.GetComponentsInChildren<MeshFilter>(true))Assert.That(f.sharedMesh,Is.Not.Null);
                foreach(var r in go.GetComponentsInChildren<Renderer>(true))foreach(var m in r.sharedMaterials){Assert.That(m,Is.Not.Null);Assert.That(AssetDatabase.Contains(m),Is.True);Assert.That(m.shader.name,Is.EqualTo("Universal Render Pipeline/Lit"));Assert.That(m.shader.isSupported,Is.True);}
            }
            foreach(var name in new[]{"TimberHouse_6x8m","VillageInn_55x6m","StoneCottage_45x45m","Storehouse_42x5m"})
            {
                var go=VillageE07Geometry.Load("Architecture",name);Assert.That(go,Is.Not.Null);var rs=go.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);
                Assert.That(b.min.y,Is.EqualTo(0).Within(.02f));Assert.That(b.size.y,Is.InRange(4f,8f));Assert.That(go.transform.Find("Contextual cutaway/Pitched roof"),Is.Not.Null);
                Assert.That(go.GetComponentsInChildren<Collider>().Length,Is.EqualTo(name=="TimberHouse_6x8m"?0:1));
            }
        }
        [Test]public void TerrainAndVegetationKeepFlatPlayableGroundAndAppropriatePhysics()
        {
            foreach(var name in new[]{"Broadleaf_7m","SlenderTree_8m"})
            {
                var t=VillageE07Geometry.Load("Vegetation",name);Assert.That(t,Is.Not.Null);Assert.That(t.GetComponent<CapsuleCollider>(),Is.Not.Null);
                Assert.That(t.GetComponent<UnityEngine.AI.NavMeshObstacle>().carving,Is.True);Assert.That(t.GetComponentsInChildren<Renderer>().Length,Is.LessThanOrEqualTo(3));
            }
            foreach(var name in new[]{"Shrub_90cm","Understory_35cm"})Assert.That(VillageE07Geometry.Load("Vegetation",name).GetComponentsInChildren<Collider>().Length,Is.Zero);
            var ground=VillageE07Geometry.Load("Ground","Backdrop_80m");Assert.That(ground.GetComponentsInChildren<Collider>().Length,Is.Zero);
            foreach(var v in ground.GetComponentInChildren<MeshFilter>().sharedMesh.vertices)if(Mathf.Abs(v.x)<=20&&Mathf.Abs(v.z)<=20)Assert.That(v.y,Is.EqualTo(0).Within(.0001f));
            var report=new StringBuilder("E07 reusable library; dimensions in meters; includes inactive geometry. Not a frame-time benchmark.\n");
            foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{VillageE07Materials.Prefabs}))
            {
                var go=AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));var rs=go.GetComponentsInChildren<Renderer>();var bounds=rs[0].bounds;var tris=0;
                foreach(var r in rs)bounds.Encapsulate(r.bounds);foreach(var f in go.GetComponentsInChildren<MeshFilter>()){Assert.That(f.sharedMesh.vertexCount,Is.LessThan(32000));tris+=f.sharedMesh.triangles.Length/3;}
                report.AppendLine(go.name+" | "+bounds.size.ToString("F3")+" | tris="+tris+" | renderers="+rs.Length+" | colliders="+go.GetComponentsInChildren<Collider>().Length);
            }
            File.WriteAllText("Logs/E07-library-geometry.txt",report.ToString());
        }
        [Test]public void TransformedDemoPreservesOriginalPhysicsNavigationAndQuestReferences()
        {
            EditorSceneManager.OpenScene(LostCargoTests.Demo);var root=GameObject.Find(RootName);Assert.That(root,Is.Not.Null);
            var now=EnvironmentPhysicsSnapshot.Capture(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            var original=string.Join("\n",now.Split('\n').Where(l=>!l.StartsWith(RootName+"/",System.StringComparison.Ordinal)));
            Assert.That(original,Is.EqualTo(File.ReadAllText("../Docs/Validacion/E07_BASE_FISICA.txt").Replace("\r\n","\n")));
            Assert.That(MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath),Is.EqualTo(File.ReadAllText(EnvironmentMaterials.Root+"/NavigationBaseline.txt").Trim()));
            var q=Object.FindAnyObjectByType<LostCargoController>();Assert.That(q.Npc.transform.position,Is.EqualTo(new Vector3(-7.5f,0,-3)));Assert.That(q.Cargo.transform.position,Is.EqualTo(new Vector3(-10,0,3)));
            foreach(var sceneRoot in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())foreach(var t in sceneRoot.GetComponentsInChildren<Transform>(true))Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject),Is.Zero);
            foreach(var r in root.GetComponentsInChildren<MeshRenderer>())foreach(var m in r.sharedMaterials)Assert.That(m,Is.Not.Null);
            Assert.That(RenderSettings.fog,Is.True);Assert.That(GameObject.Find("Directional Light").GetComponent<Light>().shadows,Is.EqualTo(LightShadows.Soft));
        }
        [UnityTest]public IEnumerator DoorBehindBuildingForestAndReversibleCutawayStayNavigable()
        {
            EditorSceneManager.OpenScene(LostCargoTests.Demo);yield return new EnterPlayMode();yield return null;yield return new WaitForSecondsRealtime(.3f);
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var root=GameObject.Find(RootName);var rig=Camera.main.GetComponent<IsometricCameraRig>();var canvas=Object.FindAnyObjectByType<VerticalSliceHud>().GetComponent<Canvas>();
            Assert.That(d.Hero.TryMoveTo(new Vector3(6,0,.3f)),Is.True);yield return Arrive(d.Hero);
            Assert.That(d.Hero.TryMoveTo(new Vector3(6,0,5)),Is.True);yield return Arrive(d.Hero);rig.FocusOn(d.Hero.transform.position);yield return null;yield return null;
            var cuts=root.GetComponentsInChildren<RoofCutaway>();Assert.That(cuts.Any(c=>c.IsCutAway),Is.True);Assert.That(VisualBlockers(root,d.Hero.transform.position+Vector3.up*1.55f),Is.Empty,"Hero head visible through native cutaway");
            canvas.enabled=false;yield return SceneCapture.Save("Logs/E07-cutaway-enabled.png",1920,1080);
            var physical=EnvironmentPhysicsSnapshot.Capture(UnityEngine.SceneManagement.SceneManager.GetActiveScene());foreach(var c in cuts)c.enabled=false;
            Assert.That(VisualBlockers(root,d.Hero.transform.position+Vector3.up*1.55f).Length,Is.GreaterThan(0),"Reproduce geometric obstruction when cutaway is disabled");yield return SceneCapture.Save("Logs/E07-cutaway-disabled.png",1920,1080);
            Assert.That(EnvironmentPhysicsSnapshot.Capture(UnityEngine.SceneManagement.SceneManager.GetActiveScene()),Is.EqualTo(physical));foreach(var c in cuts)c.enabled=true;canvas.enabled=true;yield return null;yield return null;
            Assert.That(VisualBlockers(root,d.Hero.transform.position+Vector3.up*1.55f),Is.Empty);
            foreach(var point in new[]{new Vector3(10,0,10.7f),new Vector3(-10.5f,0,7.5f),new Vector3(-17.3f,0,1.7f),new Vector3(-7.5f,0,-4.1f)})
            {
                Assert.That(UnityEngine.AI.NavMesh.SamplePosition(point,out var destination,.6f,d.Hero.Agent.areaMask),Is.True);
                Assert.That(d.Hero.TryMoveTo(point),Is.True);yield return Arrive(d.Hero);rig.FocusOn(point);yield return null;yield return null;
                Assert.That(Vector3.Distance(d.Hero.transform.position,destination.position),Is.LessThan(d.Hero.Agent.stoppingDistance+.08f),"Arrival at reachable sampled destination: "+point);Assert.That(VisualBlockers(root,d.Hero.transform.position+Vector3.up*1.55f),Is.Empty,"Visible on route at "+point);
            }
            Assert.That(Object.FindAnyObjectByType<LostCargoController>().Npc.TryInteract(d.Hero,d.Session),Is.True,"NPC remains within real interaction range after return");
            File.WriteAllText("Logs/E07-navigation-cutaway.txt","PASS: real NavMesh movement through retained open door/interior, behind building, woodland and NPC; native geometric occlusion reproduced with cutaway disabled, removed with cutaway enabled, original physics unchanged on toggling. Human review pending.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTest]public IEnumerator ComparableViewsAndMissionUseRealGameCamera()
        {
            EditorSceneManager.OpenScene(LostCargoTests.Demo);
            if(GameObject.Find(RootName)==null)
            {
                File.WriteAllText("Logs/E07-physics-before.txt",EnvironmentPhysicsSnapshot.Capture(UnityEngine.SceneManagement.SceneManager.GetActiveScene()));
                File.WriteAllText("Logs/E07-nav-before.txt",MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath));
                var inventory=new StringBuilder();
                foreach(var c in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))inventory.AppendLine(c.name+" | "+c.bounds.center+" | "+c.bounds.size);
                File.WriteAllText("Logs/E07-colliders-before.txt",inventory.ToString());
            }
            yield return new EnterPlayMode();yield return null;yield return null;
            var d=Object.FindAnyObjectByType<PrototypeDirector>();var q=Object.FindAnyObjectByType<LostCargoController>();
            var hud=Object.FindAnyObjectByType<VerticalSliceHud>();var canvas=hud.GetComponent<Canvas>();
            var label=GameObject.Find(RootName)==null?"Before":"After";Directory.CreateDirectory("Logs/E07-"+label);
            Camera.main.GetComponent<IsometricCameraRig>().enabled=false;canvas.enabled=false;d.Session.SelectClass("monk");
            yield return Shot(label,"01-Overview",new Vector3(-1,0,2),18);
            yield return Shot(label,"02-Plaza",new Vector3(-3,0,-3),8);
            yield return Shot(label,"03-Architecture",new Vector3(6,1,6),8.5f);
            yield return Shot(label,"04-Forest",new Vector3(-12,0,6),8);
            Assert.That(d.Hero.Warp(new Vector3(-4,0,-4)),Is.True);d.Hero.Select(true);
            foreach(var id in new[]{"monk","warrior","mage"})
            {
                d.Session.SelectClass(id);yield return null;
                yield return Shot(label,"05-Class-"+id,new Vector3(-4,.7f,-4),4.5f);
                Assert.That(d.Hero.GetComponent<CharacterVisualPresenter>().DisplayedClass,Is.EqualTo(id));
            }
            d.Session.SelectClass("monk");Assert.That(d.Hero.Warp(q.Npc.transform.position+Vector3.back*1.1f),Is.True);
            yield return Shot(label,"06-Npc-area",q.Npc.transform.position,7);
            canvas.enabled=true;Assert.That(q.Npc.TryInteract(d.Hero,d.Session),Is.True);
            yield return Shot(label,"07-Dialogue",q.Npc.transform.position,7);
            q.GetComponent<LostCargoView>().AcceptButton.onClick.Invoke();Assert.That(LostCargoQuest.Stage(d.Session.State),Is.EqualTo(CargoQuestStage.Accepted));
            q.EncounterSeed=42;Assert.That(d.Hero.TryMoveTo(q.Cargo.transform.position+Vector3.back*1.6f),Is.True);
            yield return Arrive(d.Hero);yield return Shot(label,"08-Quest-route",q.Cargo.transform.position,8);
            Assert.That(q.Cargo.TryInteract(d.Hero,d.Session),Is.True);Assert.That(d.Session.Mode,Is.EqualTo(PrototypeMode.TurnCombat));
            yield return Shot(label,"09-Rpg",q.Cargo.transform.position,7);
            d.Encounter.Approach();yield return Arrive(d.Hero);
            for(var turn=0;turn<30&&q.CargoEncounter;turn++)
            {
                if(d.Session.State.Health<8&&!d.Encounter.Battle.RecoveryUsed)d.Encounter.Recover();else d.Encounter.Attack();
                if(q.CargoEncounter&&d.Encounter.Battle!=null&&!d.Encounter.Battle.Finished)d.Encounter.EndTurn();yield return null;
            }
            Assert.That(LostCargoQuest.Stage(d.Session.State),Is.EqualTo(CargoQuestStage.Recovered));hud.SelectPage(ExplorationPage.Character);
            yield return Shot(label,"10-Hud",new Vector3(-3,0,-3),10);
            File.WriteAllText("Logs/E07-"+label+"/capture-receipt.txt","PASS: native Unity Game camera, same 35.26439/45 angle and explicit focus/orthographic size, 1920x1080; 12 views including three classes, actual NPC acceptance and real RPG start. Temporary framing only, not saved to scene.");
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        private static IEnumerator Shot(string label,string name,Vector3 focus,float size)
        {
            var c=Camera.main;c.transform.rotation=Quaternion.Euler(35.26439f,45,0);c.orthographicSize=size;c.transform.position=focus-c.transform.forward*45;
            yield return SceneCapture.Save("Logs/E07-"+label+"/"+name+".png",1920,1080);
            if(name=="01-Overview")
            {
                var root=GameObject.Find(RootName);var triangles=0;var renderers=0;var active=0;var activeTriangles=0;
                if(root!=null){renderers=root.GetComponentsInChildren<Renderer>().Length;active=root.GetComponentsInChildren<Renderer>().Count(r=>r.enabled&&!r.forceRenderingOff);foreach(var f in root.GetComponentsInChildren<MeshFilter>()){triangles+=f.sharedMesh.triangles.Length/3;if(f.GetComponent<Renderer>().enabled&&!f.GetComponent<Renderer>().forceRenderingOff)activeTriangles+=f.sharedMesh.triangles.Length/3;}}
                File.WriteAllText("Logs/E07-"+label+"/rendering.txt","UnityStats after native Game render: drawCalls="+UnityStats.drawCalls+"; setPass="+UnityStats.setPassCalls+"; frameTriangles="+UnityStats.triangles+"; E07 renderers="+renderers+"; active="+active+"; retained triangles="+triangles+"; activeTriangles="+activeTriangles+". Counters are editor batch/offscreen samples, not FPS; retained includes disabled source foliage, zero counters mean unavailable.");
            }
        }
        private static string[] VisualBlockers(GameObject root,Vector3 target)
        {
            var c=Camera.main;var depth=Vector3.Dot(target-c.transform.position,c.transform.forward);var ray=new Ray(target-c.transform.forward*depth,c.transform.forward);var hits=new System.Collections.Generic.List<string>();
            foreach(var f in root.GetComponentsInChildren<MeshFilter>())
            {
                var r=f.GetComponent<Renderer>();if(r==null||!r.enabled||r.forceRenderingOff||!r.bounds.IntersectRay(ray,out var boundHit)||boundHit>=depth-.05f)continue;
                var vertices=f.sharedMesh.vertices;var tris=f.sharedMesh.triangles;var matrix=f.transform.localToWorldMatrix;
                for(var i=0;i<tris.Length;i+=3)if(Triangle(ray,matrix.MultiplyPoint3x4(vertices[tris[i]]),matrix.MultiplyPoint3x4(vertices[tris[i+1]]),matrix.MultiplyPoint3x4(vertices[tris[i+2]]),depth-.05f)){hits.Add(f.transform.parent.name+"/"+f.name);break;}
            }
            return hits.ToArray();
        }
        private static bool Triangle(Ray ray,Vector3 a,Vector3 b,Vector3 c,float max)
        {
            var e1=b-a;var e2=c-a;var p=Vector3.Cross(ray.direction,e2);var det=Vector3.Dot(e1,p);if(Mathf.Abs(det)<.000001f)return false;
            var inv=1/det;var s=ray.origin-a;var u=Vector3.Dot(s,p)*inv;if(u<0||u>1)return false;var q=Vector3.Cross(s,e1);var v=Vector3.Dot(ray.direction,q)*inv;if(v<0||u+v>1)return false;var distance=Vector3.Dot(e2,q)*inv;return distance>0&&distance<max;
        }
        private static IEnumerator Arrive(ExplorationUnit hero)
        {yield return null;var until=Time.realtimeSinceStartup+15;while(hero.IsMoving&&Time.realtimeSinceStartup<until)yield return null;Assert.That(hero.IsMoving,Is.False,"E07 route timed out");}
        [UnityTearDown]public IEnumerator End(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
