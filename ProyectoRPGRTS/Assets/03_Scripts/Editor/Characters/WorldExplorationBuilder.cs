using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using V=Aethernia.Editor.Environment.VillageE07Geometry;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Characters
{
    public static class WorldExplorationBuilder
    {
        public const string Root="Assets/17_UX02",ScenePath="Assets/01_Scenes/AetherniaUx02World.unity";
        private static DemoFlow flow;
        private static Transform world;
        private static readonly List<WorldInteractable> interactions=new List<WorldInteractable>();
        private static readonly List<Transform> routes=new List<Transform>();
        public static void Create()
        {
            if(File.Exists(ScenePath)||AssetDatabase.IsValidFolder(Root))throw new InvalidOperationException("UX02 already authored; use targeted refinements.");
            var scene=EditorSceneManager.OpenScene(UxExperienceBuilder.ScenePath);flow=Object.FindAnyObjectByType<DemoFlow>();
            Directory.CreateDirectory("Logs/UX02");
            File.WriteAllLines("Logs/UX02/before-world.txt",Object.FindObjectsByType<InteractionPoint>(FindObjectsInactive.Include).Select(p=>p.FactId+" "+p.transform.position));
            AlphaPipelineBuilder.Folder(Root);
            var previous=flow.Session.Rules;var rules=Object.Instantiate(previous);rules.WorldExploration=true;AssetDatabase.CreateAsset(rules,Root+"/PrototypeRules.asset");
            foreach(var component in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {
                var so=new SerializedObject(component);var it=so.GetIterator();
                while(it.NextVisible(true))if(it.propertyType==SerializedPropertyType.ObjectReference&&it.objectReferenceValue==previous)it.objectReferenceValue=rules;
                so.ApplyModifiedPropertiesWithoutUndo();PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            }
            var content=Object.Instantiate(flow.Content);content.Intro=new[]{"LA ALDEA DE FRONTERA\n\nUn habitante busca ayuda con sus provisiones. El camino occidental cruza el bosque y se bifurca hacia las ruinas y el campamento de patrulla.\n\nExplora a tu ritmo. Los combates necesitan una interacción u orden explícita. Relato provisional."};AssetDatabase.CreateAsset(content,Root+"/DemoContent.asset");flow.Content=content;
            world=new GameObject("UX02 connected frontier world").transform;
            var controller=flow.gameObject.AddComponent<WorldExploration>();controller.Flow=flow;
            // Las posiciones lógicas heredadas se mantienen; los nuevos accesos forman un bucle al oeste.
            Floor(new Vector3(-22,-.15f,-11),new Vector3(34,.3f,24));
            Road("Forest main road",new[]{new Vector3(-12,0,1.5f),new Vector3(-19,0,1),new Vector3(-25,0,-2),new Vector3(-33,0,-3),new Vector3(-41,0,-2),new Vector3(-47,0,-.5f)},1.9f);
            Road("Ruins branch",new[]{new Vector3(-25,0,-2),new Vector3(-26,0,-8),new Vector3(-22,0,-13),new Vector3(-16,0,-16)},1.75f);
            Road("Village return loop",new[]{new Vector3(-16,0,-16),new Vector3(-10,0,-17),new Vector3(-9,0,-11)},1.75f);
            foreach(var point in new[]{new Vector3(-18,0,1),new Vector3(-25,0,-2),new Vector3(-26,0,-8),new Vector3(-22,0,-13),new Vector3(-16,0,-17),new Vector3(-33,0,-3),new Vector3(-40,0,-2),new Vector3(-47,0,-1)})
            {var waypoint=new GameObject("Walkable route checkpoint").transform;waypoint.SetParent(world);waypoint.position=point;routes.Add(waypoint);}
            FlattenBackdrops();
            var forest=new GameObject("Woodland edge and landmarks").transform;forest.SetParent(world);
            var random=new System.Random(2102);
            foreach(var at in new[]{new Vector3(-18,0,-5),new Vector3(-22,0,5.5f),new Vector3(-30,0,5),new Vector3(-34,0,-9),new Vector3(-31,0,-15),new Vector3(-25,0,-18),new Vector3(-17,0,-21),new Vector3(-37,0,6),new Vector3(-43,0,-8)})
            {
                var tree=Place("Vegetation",random.Next(2)==0?"Broadleaf_7m":"SlenderTree_8m",forest,at,(float)random.NextDouble()*360);
                var cut=tree.AddComponent<RoofCutaway>();cut.Configure(Camera.main,flow.Director.Hero.transform,tree.GetComponentsInChildren<Renderer>());
                Place("Vegetation","Shrub_90cm",forest,at+new Vector3(1.1f,0,.8f));
                Place("Vegetation","Boulder_120cm",forest,at+new Vector3(-1.4f,0,-.5f),random.Next(360));
            }
            for(var i=0;i<20;i++)Place("Vegetation","Understory_35cm",forest,new Vector3(-19-i*1.2f,0,4.2f+(float)random.NextDouble()));
            var ruinEntrance=Object.FindObjectsByType<InteractionPoint>().Single(p=>p.FactId=="umbral-entry");
            ruinEntrance.transform.position=new Vector3(-16,0,-16);PrefabUtility.RecordPrefabInstancePropertyModifications(ruinEntrance.transform);
            foreach(var r in ruinEntrance.GetComponentsInChildren<Renderer>())r.enabled=false;
            var ruinArt=new GameObject("Ruins entrance framing").transform;ruinArt.SetParent(world);
            foreach(var x in new[]{-18f,-14f})
            {Place("Vegetation","Boulder_120cm",ruinArt,new Vector3(x,0,-15));Place("Props","Runestone_80cm",ruinArt,new Vector3(x,0,-16.3f));}
            Place("Props","Runestone_80cm",ruinArt,new Vector3(-16,0,-18.8f));
            Object.FindAnyObjectByType<DungeonExpedition>().SurfaceReturn=new Vector3(-16,0,-17.6f);
            // Pocos objetos con consecuencias verificables; datos en Facts, recursos compartidos.
            Item("ux02-crossroads","Señal del cruce",WorldInteractionKind.Sign,new Vector3(-24,0,-4),"OESTE: campamento de patrulla y puesto hostil.\nSUR: ruinas y regreso a la aldea.\nLas ruinas se abren al asegurar el campamento.","Leer","Runestone_80cm");
            Item("ux02-cache","Alijo del sendero",WorldInteractionKind.Chest,new Vector3(-30,0,-6.2f),"Un pequeño alijo fuera del camino.","Abrir","WickerBasket_60cm");
            Item("ux02-branches","Ramas secas",WorldInteractionKind.Materials,new Vector3(-22,0,-10),"Madera aprovechable para el campamento.","Recoger","Woodpile_2m");
            Item("ux02-store","Almacén de la aldea",WorldInteractionKind.Store,new Vector3(12,0,-.5f),"Reserva compartida.","Consultar","SupplySack_75cm");
            Item("ux02-door","Puerta de provisiones",WorldInteractionKind.Door,new Vector3(14,0,1.2f),"El almacén entrega una pequeña provisión de alimento por viajero. La puerta permite pedirla; el interior de esta casa aún no es jugable.","Llamar","Barrel_100cm");
            Npc("ux02-merchant","Comerciante",WorldInteractionKind.Merchant,new Vector3(2.4f,0,-8.5f),"monk",Array.Empty<Vector3>());
            Npc("ux02-guard","Guardia del sendero",WorldInteractionKind.Guard,new Vector3(-19,0,-1),"warrior",new[]{new Vector3(-19,0,-1),new Vector3(-21,0,-1.5f)});
            interactions.Last().Description="El cargamento está junto al bosque. Para entrar en las ruinas, asegura antes el campamento. El capitán espera al oeste; ningún encuentro empieza sólo por caminar.";
            Npc("ux02-recruiter","Enlace del campamento",WorldInteractionKind.Recruiter,new Vector3(5.5f,0,-11.5f),"warrior",Array.Empty<Vector3>());
            // Reutilizar el habitante/animación existente si es articulado; NPC funcionales nuevos usan el mismo catálogo.
            var cargo=Object.FindAnyObjectByType<LostCargoController>();
            foreach(var point in new[]{cargo.Npc,flow.Patrol})
            {
                var visual=NpcVisual(point.transform,point==cargo.Npc?"monk":"warrior");
                if(visual!=null)point.gameObject.AddComponent<WorldNpcLook>().Configure(flow,visual.transform);
            }
            HostileDressing();
            controller.Interactions=interactions.ToArray();controller.RoutePoints=routes.ToArray();
            controller.Discoveries=new[]{new Vector3(-23,0,-2),new Vector3(-16,0,-16),new Vector3(-47,0,-.5f),new Vector3(-28,0,4)};
            controller.DiscoveryNames=new[]{"Sendero de frontera","Entrada de las ruinas","Campamento de patrulla","Puesto hostil"};
            BakeNavigation();
            foreach(var component in Object.FindObjectsByType<Component>(FindObjectsInactive.Include))if(component!=null)PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            EditorSceneManager.SaveScene(scene,ScenePath,true);AssetDatabase.SaveAssets();Debug.Log("UX02_CREATED connected roads, existing dungeon, NPC and world state; functional verification pending.");
        }
        private static GameObject Place(string category,string name,Transform parent,Vector3 at,float yaw=0)
        {var prefab=V.Load(category,name);if(prefab==null){var path=AssetDatabase.FindAssets(name+" t:Prefab",new[]{"Assets/02_Prefabs"}).Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault(p=>Path.GetFileNameWithoutExtension(p)==name);if(path!=null)prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);}if(prefab==null)throw new InvalidOperationException("Missing public prefab "+name);return V.Place(prefab,parent,at,yaw);}
        private static void Floor(Vector3 at,Vector3 size)
        {
            var node=GameObject.CreatePrimitive(PrimitiveType.Cube);node.name="Forest physical floor";node.transform.SetParent(world);node.transform.position=at;node.transform.localScale=size;node.GetComponent<Renderer>().enabled=false;
        }
        private static void Road(string name,Vector3[] points,float halfWidth)
        {
            var mesh=Ribbon(points,halfWidth);AssetDatabase.CreateAsset(mesh,Root+"/"+name.Replace(" ","")+".asset");
            var node=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));node.transform.SetParent(world);node.GetComponent<MeshFilter>().sharedMesh=mesh;
            node.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/13_AlphaCandidate/Materials/Earth_Alpha.mat");
        }
        private static void FlattenBackdrops()
        {
            foreach(var filter in Object.FindObjectsByType<MeshFilter>().Where(f=>f.name=="Grass"&&f.GetComponent<Renderer>()!=null&&f.GetComponent<Renderer>().bounds.size.x>60))
            {
                var mesh=Object.Instantiate(filter.sharedMesh);var v=mesh.vertices;
                for(var i=0;i<v.Length;i++)
                {
                    var p=filter.transform.TransformPoint(v[i]);
                    var weight=Mathf.Min(Mathf.InverseLerp(-58,-54,p.x),Mathf.InverseLerp(-8,-13,p.x),Mathf.InverseLerp(-25,-21,p.z),Mathf.InverseLerp(9,5,p.z));
                    p.y=Mathf.Lerp(p.y,-.04f,Mathf.SmoothStep(0,1,weight));v[i]=filter.transform.InverseTransformPoint(p);
                }
                mesh.vertices=v;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Root+"/ForestGround"+filter.GetComponent<Renderer>().bounds.size.x.ToString("0")+".asset");filter.sharedMesh=mesh;
            }
        }
        private static void Item(string id,string name,WorldInteractionKind kind,Vector3 at,string text,string action,string prefab)
        {
            var node=new GameObject(name);node.transform.SetParent(world);node.transform.position=at;
            Place("Props",prefab,node.transform,Vector3.zero);
            var trigger=node.AddComponent<SphereCollider>();trigger.radius=.65f;trigger.center=Vector3.up*.7f;trigger.isTrigger=true;
            var point=node.AddComponent<InteractionPoint>();point.Configure(id,name);
            var item=node.AddComponent<WorldInteractable>();item.Kind=kind;item.Description=text;item.Action=action;interactions.Add(item);
        }
        private static GameObject NpcVisual(Transform parent,string cls)
        {
            if(!CharacterVisualFactory.TryCreate(flow.Front.Catalog,cls,parent,out var model,out _,out var reason))throw new InvalidOperationException(reason);
            model.ApplyEquipment(new PrototypeState{Weapon=cls=="warrior"?"training-sword":"",Armour="cloth"});return model.gameObject;
        }
        private static void Npc(string id,string name,WorldInteractionKind kind,Vector3 at,string cls,Vector3[] route)
        {
            var node=(GameObject)PrefabUtility.InstantiatePrefab(flow.Director.Rts.UnitTemplate,world);node.name=name;node.transform.position=at;
            var actor=node.GetComponent<ExplorationUnit>();actor.Select(false);
            foreach(var r in node.GetComponentsInChildren<Renderer>())if(actor.SelectionIndicator==null||!r.transform.IsChildOf(actor.SelectionIndicator.transform))r.enabled=false;
            var visual=NpcVisual(node.transform,cls);visual.GetComponent<CharacterVisualModel>().BindActor(actor);
            var point=node.AddComponent<InteractionPoint>();point.Configure(id,name);
            var item=node.AddComponent<WorldInteractable>();item.Kind=kind;item.Action="Hablar";interactions.Add(item);
            var activity=node.AddComponent<VillageNpcActivity>();activity.Flow=flow;activity.Actor=actor;activity.Route=route;
        }
        private static void HostileDressing()
        {
            var parent=new GameObject("Patrol approach and enemy position").transform;parent.SetParent(world);
            foreach(var at in new[]{new Vector3(-30,0,9),new Vector3(-26,0,9)})Place("Props","Crate_80cm",parent,at);
            Place("Architecture","OpenWoodshed_3x3m",parent,new Vector3(-26,0,11));
            Place("Props","Barrel_100cm",parent,new Vector3(-31,0,10));
            foreach(var at in new[]{new Vector3(-49,0,-9),new Vector3(-44,0,-9)})Place("Props","SupplySack_75cm",parent,at);
        }
        private static void BakeNavigation()
        {
            var surface=GameObject.Find("Baked Navigation").GetComponent<NavMeshSurface>();surface.RemoveData();surface.navMeshData=null;
            surface.collectObjects=CollectObjects.Volume;surface.center=new Vector3(-1,0,-1);surface.size=new Vector3(130,12,66);surface.BuildNavMesh();AssetDatabase.CreateAsset(surface.navMeshData,Root+"/SurfaceNavigation.asset");
        }
        public static void Build()
        {
            Directory.CreateDirectory("Builds/Ux02Windows");Directory.CreateDirectory("Logs/UX02");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/Ux02Windows/Aethernia.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
            if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new InvalidOperationException("UX02 build failed");
            File.WriteAllText("Logs/UX02/build.txt","PASS Unity "+Application.unityVersion+" bytes="+report.summary.totalSize+" seconds="+report.summary.totalTime.TotalSeconds);
        }
        public static void Refine()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);flow=Object.FindAnyObjectByType<DemoFlow>();
            var content=flow.Content;content.Intro=content.Intro.Select(RepairText).ToArray();EditorUtility.SetDirty(content);
            foreach(var item in flow.GetComponent<WorldExploration>().Interactions)
            {var point=item.GetComponent<InteractionPoint>();point.Configure(point.FactId,RepairText(point.DisplayName));item.name=RepairText(item.name);item.Description=RepairText(item.Description);EditorUtility.SetDirty(point);EditorUtility.SetDirty(item);}
            var worldRoot=GameObject.Find("UX02 connected frontier world").transform;
            foreach(var renderer in worldRoot.GetComponentsInChildren<MeshRenderer>())
            {
                var materials=renderer.sharedMaterials;
                for(var i=0;i<materials.Length;i++)
                {
                    if(materials[i]==null)continue;
                    var shared=AssetDatabase.LoadAssetAtPath<Material>("Assets/13_AlphaCandidate/Materials/"+materials[i].name+"_Alpha.mat");
                    if(shared!=null)materials[i]=shared;
                    if(materials[i].HasProperty("_WindStrength")&&materials[i].GetFloat("_WindStrength")>0)
                    {var bounds=renderer.GetComponent<VisualWindBounds>()??renderer.gameObject.AddComponent<VisualWindBounds>();bounds.Margin=.12f;}
                }
                renderer.sharedMaterials=materials;PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
            }
            // Estas cintas tienen UV en metros. El shader Alpha exige UV normalizados para su máscara;
            // reutilizar Earth Lit evita convertir la mayor parte del camino en hierba o estirar el albedo.
            foreach(var name in new[]{"Forest main road","Ruins branch","Village return loop"})worldRoot.Find(name).GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/06_Environment/VillageE07/Materials/Earth.mat");
            RepairRoad(worldRoot,"Forest main road",new[]{new Vector3(-12,0,1.5f),new Vector3(-19,0,1),new Vector3(-25,0,-2),new Vector3(-33,0,-3),new Vector3(-41,0,-2),new Vector3(-47,0,-.5f)},1.9f);
            RepairRoad(worldRoot,"Ruins branch",new[]{new Vector3(-25,0,-2),new Vector3(-26,0,-8),new Vector3(-22,0,-13),new Vector3(-16,0,-16)},1.75f);
            RepairRoad(worldRoot,"Village return loop",new[]{new Vector3(-16,0,-16),new Vector3(-10,0,-17),new Vector3(-9,0,-11)},1.75f);
            foreach(var tree in Object.FindObjectsByType<RoofCutaway>())
            {
                var p=tree.transform.position;
                if(Mathf.Abs(p.x+18)<.1f&&Mathf.Abs(p.z-1)<.1f)tree.transform.position=new Vector3(-23,p.y,12);
                if(Mathf.Abs(p.x+16.5f)<.1f&&Mathf.Abs(p.z+16)<.1f)tree.transform.position=new Vector3(-20,p.y,-21.5f);
                PrefabUtility.RecordPrefabInstancePropertyModifications(tree.transform);
            }
            var cargo=Object.FindAnyObjectByType<LostCargoController>();
            foreach(var point in new[]{cargo.Npc,flow.Patrol})
            {
                var models=point.GetComponentsInChildren<CharacterVisualModel>(true);var keep=models.LastOrDefault();
                if(keep==null)continue;
                foreach(var model in models)if(model!=keep)model.gameObject.SetActive(false);
                foreach(var renderer in point.GetComponentsInChildren<Renderer>(true))
                    if(!renderer.transform.IsChildOf(keep.transform))renderer.enabled=false;
                foreach(var c in point.GetComponentsInChildren<Component>(true))PrefabUtility.RecordPrefabInstancePropertyModifications(c);
            }
            var surface=GameObject.Find("Baked Navigation").GetComponent<NavMeshSurface>();
            var saved=AssetDatabase.LoadAssetAtPath<NavMeshData>(Root+"/SurfaceNavigation.asset");surface.RemoveData();surface.navMeshData=null;surface.BuildNavMesh();var generated=surface.navMeshData;surface.RemoveData();EditorUtility.CopySerialized(generated,saved);surface.navMeshData=saved;surface.AddData();Object.DestroyImmediate(generated);EditorUtility.SetDirty(saved);PrefabUtility.RecordPrefabInstancePropertyModifications(surface);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Debug.Log("UX02_REFINED existing logical NPC keep one shared humanoid presentation; collider unchanged.");
        }
        private static string RepairText(string value)
        {
            if(string.IsNullOrEmpty(value)||(!value.Contains("\u00c3")&&!value.Contains("\u00e2")&&!value.Contains("\u00c2")))return value;
            try{return new System.Text.UTF8Encoding(false,true).GetString(System.Text.Encoding.GetEncoding(1252).GetBytes(value));}catch(System.Text.DecoderFallbackException){return value;}
        }
        private static void RepairRoad(Transform root,string name,Vector3[] points,float halfWidth)
        {
            var mesh=root.Find(name).GetComponent<MeshFilter>().sharedMesh;var fresh=Ribbon(points,halfWidth);
            EditorUtility.CopySerialized(fresh,mesh);Object.DestroyImmediate(fresh);EditorUtility.SetDirty(mesh);
        }
        private static Mesh Ribbon(Vector3[] points,float halfWidth)
        {
            var centres=new List<Vector3>();
            for(var s=0;s<points.Length-1;s++)
            {
                var a=points[Mathf.Max(0,s-1)];var b=points[s];var c=points[s+1];var d=points[Mathf.Min(points.Length-1,s+2)];
                var steps=Mathf.CeilToInt(Vector3.Distance(b,c)/.4f);
                for(var j=0;j<steps;j++)
                {var t=(float)j/steps;centres.Add(.5f*((2*b)+(-a+c)*t+(2*a-5*b+4*c-d)*t*t+(-a+3*b-3*c+d)*t*t*t));}
            }
            centres.Add(points.Last());var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();var distance=0f;
            for(var i=0;i<centres.Count;i++)
            {
                if(i>0)distance+=Vector3.Distance(centres[i],centres[i-1]);
                var tangent=(centres[Mathf.Min(centres.Count-1,i+1)]-centres[Mathf.Max(0,i-1)]).normalized;var side=Vector3.Cross(Vector3.up,tangent);
                var width=halfWidth*(.94f+Mathf.PerlinNoise(centres[i].x*.9f+13,centres[i].z*.9f+11)*.12f);
                vertices.Add(centres[i]-side*width+Vector3.up*.023f);vertices.Add(centres[i]+side*width+Vector3.up*.023f);
                uv.Add(new Vector2(0,distance*.5f));uv.Add(new Vector2(width,distance*.5f));var n=i*2;
                if(i>0)triangles.AddRange(new[]{n-2,n,n-1,n-1,n,n+1});
            }
            return V.Mesh("UX02 continuous metric road",vertices,triangles,uv);
        }
    }
}
