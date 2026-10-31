using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using V=Aethernia.Editor.Environment.VillageE07Geometry;
using M=Aethernia.Editor.Environment.VillageE07Materials;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Environment
{
    public static class VillageE07SceneBuilder
    {
        public const string ScenePath="Assets/01_Scenes/AetherniaVillageDemo.unity";
        public const string RootName="E07 Medieval Village";
        private static Camera camera;
        private static Transform hero;
        public static void Apply()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath);
            if(GameObject.Find(RootName)!=null)throw new InvalidOperationException("E07 scene already authored; use a targeted fix, do not regenerate.");
            if(V.Load("Props","Handcart_240cm")==null)throw new InvalidOperationException("Author and validate E07 library/nature first.");
            var before=EnvironmentPhysicsSnapshot.Capture(scene);var nav=MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath);
            var root=new GameObject(RootName).transform;camera=Camera.main;hero=Object.FindAnyObjectByType<PrototypeDirector>().Hero.transform;
            // Recovery remains in the same scene, disabled only for presentation; original colliders stay active.
            GameObject.Find(MedievalQuarterBuilder.RootName).SetActive(false);
            Hide("ReferenceGround_40m");Hide("ReferenceDoor_120x220cm");Hide("ReferenceWindow_100x120cm");Hide("Narrow Gate - 0.40m clearance");Hide("Faction B Marker");Hide("Dungeon Entrance");
            var terrain=V.Group(root,"Terrain and roads");V.Place(V.Load("Ground","Backdrop_80m"),terrain,Vector3.zero);
            Patch(terrain,"Forest clearing",new Vector3(-15,0,8),4.5f,10,"ForestFloor");Patch(terrain,"Garden earth",new Vector3(12,0,12),5,4,"ForestFloor");
            Path(terrain,"Village entry",new[]{new Vector3(-9,0,-20),new Vector3(-10,0,-14),new Vector3(-9,0,-9),new Vector3(-5,0,-4)},1.65f,.008f);
            Path(terrain,"House approach",new[]{new Vector3(-4,0,-4),new Vector3(1,0,-3),new Vector3(6,0,0),new Vector3(6,0,2)},1.65f,.010f);
            Path(terrain,"Lost cargo trail",new[]{new Vector3(-7.5f,0,-3.8f),new Vector3(-8.7f,0,-.4f),new Vector3(-10,0,2),new Vector3(-12.7f,0,6.8f),new Vector3(-15,0,8.8f)},1.25f,.012f);
            Path(terrain,"Inn approach",new[]{new Vector3(-3,0,-1),new Vector3(.8f,0,5),new Vector3(-1,0,9)},1.0f,.013f);
            Path(terrain,"Storage branch",new[]{new Vector3(6,0,0),new Vector3(12.5f,0,0),new Vector3(14,0,1.5f)},1.05f,.014f);
            Path(terrain,"Stone dwelling approach",new[]{new Vector3(-9,0,-10),new Vector3(-15,0,-10)},.9f,.011f);
            V.Place(V.Load("Ground","CobbledSquare_10x8m"),terrain,new Vector3(-3,0,-3));
            var buildings=V.Group(root,"Architecture");
            Building(buildings,"TimberHouse_6x8m",new Vector3(6,0,6));
            Building(buildings,"VillageInn_55x6m",new Vector3(-1,0,12));
            Building(buildings,"StoneCottage_45x45m",new Vector3(-15,0,-7));
            Building(buildings,"Storehouse_42x5m",new Vector3(14,0,4));
            var shed=V.Place(V.Load("Architecture","OpenWoodshed_3x3m"),buildings,new Vector3(-2,0,5));Cut(shed,shed.GetComponentsInChildren<Renderer>());
            foreach(var x in new[]{-1.3f,1.3f})foreach(var z in new[]{-1.3f,1.3f})
            {var post=V.Group(root,"Shed post collision");post.position=new Vector3(-2+x,0,5+z);VillageE07Nature.SolidBox(post,new Vector3(0,1.2f,0),new Vector3(.16f,2.4f,.16f));}
            var skins=V.Group(root,"Skins over original logical objects");
            OldProp(skins,"Barrel_100cm",new Vector3(-8,0,-4));OldProp(skins,"Chest_80x80x60cm",new Vector3(-2,0,-5));OldProp(skins,"CrateStack_3x2x2m",new Vector3(-2,0,5));
            V.Place(V.Load("Modules","Door_140x240cm"),skins,new Vector3(-2,0,-3));
            var window=V.Place(V.Load("Modules","Window_100x120cm"),skins,new Vector3(-8,.9f,3));
            var support=V.Group(skins,"Window ruin support");V.Box(support,"Low stonework",new Vector3(-8,.4f,3.02f),new Vector3(1.25f,.8f,.24f),"GreyStone");V.Merge(support,"WindowRuinSupport");
            var gate=V.Group(skins,"Old narrow gate posts");foreach(var x in new[]{-6.3f,-5.7f}){V.Box(gate,"Worn post",new Vector3(x,1.2f,-9),new Vector3(.2f,2.4f,.25f),"AgedOak");V.Pole(gate,new Vector3(x,2.4f,-9),new Vector3(x,2.56f,-9),.12f,0,"DarkOak");}V.Merge(gate,"NarrowGateSkin");
            V.Place(V.Load("Props","Runestone_80cm"),skins,new Vector3(-10,0,5));
            Banner(skins);Market(skins);
            var props=V.Group(root,"Village activity and quest dressing");
            PlaceProp(props,"VillageWell_2m",new Vector3(-.5f,0,-.4f),0,true);
            PlaceProp(props,"Bench_180cm",new Vector3(-4.5f,0,.6f),180);PlaceProp(props,"Bench_180cm",new Vector3(1.4f,0,8.8f),0);
            PlaceProp(props,"Table_180cm",new Vector3(-3.7f,0,8.0f),90);PlaceProp(props,"WickerBasket_60cm",new Vector3(-4.1f,0,8.6f));
            PlaceProp(props,"Handcart_240cm",new Vector3(-12.7f,0,6.2f),-25);
            PlaceProp(props,"SupplySack_75cm",new Vector3(-11.3f,0,4.3f));PlaceProp(props,"SupplySack_75cm",new Vector3(-12.1f,0,4.1f),25);
            PlaceProp(props,"Woodpile_2m",new Vector3(10.3f,0,6.2f),0);PlaceProp(props,"ToolRack_140cm",new Vector3(11.5f,0,5.6f),90);
            foreach(var at in new[]{new Vector3(15.8f,0,.5f),new Vector3(16.7f,0,1.1f),new Vector3(15.8f,0,-.5f)})PlaceProp(props,"SupplySack_75cm",at);
            OldProp(props,"Barrel_100cm",new Vector3(11.5f,0,2));Solid(props.GetChild(props.childCount-1),new Vector3(0,.5f,0),new Vector3(.96f,1,.96f));
            OldProp(props,"Crate_80cm",new Vector3(15,0,0));Solid(props.GetChild(props.childCount-1),new Vector3(0,.4f,0),Vector3.one*.8f);
            // Dedicated clear strips are retained for the existing 6v6 RTS and construction smoke locations.
            foreach(var x in new[]{4.0f,6.0f,8.0f})Fence(props,new Vector3(x,0,-8.8f));
            foreach(var x in new[]{-16.6f,-14.6f})Fence(props,new Vector3(x,0,-11));
            Entry(props);Sign(props,new Vector3(-8.4f,0,6.8f),"AL BOSQUE");Sign(props,new Vector3(-5.3f,0,-1.4f),"SUMINISTROS");
            Lamp(props,new Vector3(-8.8f,0,-1.8f));Lamp(props,new Vector3(1.3f,0,8.8f));Lamp(props,new Vector3(-8.2f,0,-15.4f));
            var forest=V.Group(root,"Temperate woodland and ground cover");Forest(forest);
            var q=Object.FindAnyObjectByType<LostCargoController>();Remap(q.Cargo.gameObject);
            Lighting(root);
            var rig=camera.GetComponent<IsometricCameraRig>();using(var so=new SerializedObject(rig)){so.FindProperty("focus").vector3Value=new Vector3(-1,0,1);so.FindProperty("size").floatValue=13;so.ApplyModifiedPropertiesWithoutUndo();}PrefabUtility.RecordPrefabInstancePropertyModifications(rig);
            var after=EnvironmentPhysicsSnapshot.Capture(scene);var originalLines=string.Join("\n",after.Split('\n').Where(line=>!line.StartsWith(RootName+"/",StringComparison.Ordinal)));
            if(originalLines!=before||MedievalQuarterBuilder.Hash(MedievalQuarterBuilder.NavPath)!=nav)throw new InvalidOperationException("E07 unexpectedly changed original physical objects or baked navigation.");
            File.WriteAllText("Logs/E07-scene-physics.txt",after);File.WriteAllText("Logs/E07-navigation-hash.txt",nav);
            EditorSceneManager.MarkSceneDirty(scene);AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene);
            Debug.Log("E07_SCENE: existing village transformed, original physical objects/NavMesh and quest coordinates preserved; new solid obstacles carve at runtime. Human/art approval pending.");
        }
        private static void Hide(string name)
        {var go=GameObject.Find(name);if(go==null)throw new InvalidOperationException("Original object missing: "+name);foreach(var r in go.GetComponentsInChildren<Renderer>()){r.enabled=false;PrefabUtility.RecordPrefabInstancePropertyModifications(r);}}
        private static void Building(Transform p,string name,Vector3 at)
        {var go=V.Place(V.Load("Architecture",name),p,at);Cut(go,go.transform.Find("Contextual cutaway").GetComponentsInChildren<Renderer>());}
        private static void Cut(GameObject go,Renderer[] renderers)
        {var cut=go.AddComponent<RoofCutaway>();cut.Configure(camera,hero,renderers);}
        private static void Solid(Transform t,Vector3 centre,Vector3 size)=>VillageE07Nature.SolidBox(t,centre,size);
        private static GameObject PlaceProp(Transform p,string name,Vector3 at,float yaw=0,bool cut=false)
        {var go=V.Place(V.Load("Props",name),p,at,yaw);if(cut)Cut(go,go.GetComponentsInChildren<Renderer>().Where(r=>r.bounds.max.y>1.8f).ToArray());return go;}
        private static void OldProp(Transform p,string name,Vector3 at)
        {var go=V.Place(MedievalPrefabs.Load("Props",name),p,at);Remap(go);}
        private static void Remap(GameObject go)
        {
            foreach(var r in go.GetComponentsInChildren<Renderer>())
            {if(r.sharedMaterial==EnvironmentMaterials.Load("Oak"))r.sharedMaterial=M.Load("AgedOak");PrefabUtility.RecordPrefabInstancePropertyModifications(r);}
        }
        private static void Fence(Transform p,Vector3 at)
        {var go=V.Place(V.Load("Modules","Fence_2m"),p,at);Solid(go.transform,new Vector3(0,.62f,0),new Vector3(2,1.24f,.16f));}
        private static void Path(Transform p,string name,Vector3[] points,float width,float height)
        {var node=V.Group(p,name);V.Part(node,name,VillageE07Nature.Road(points,width,height),"Earth",Vector3.zero);V.Merge(node,"Path_"+name.Replace(" ",""));}
        private static void Patch(Transform p,string name,Vector3 centre,float rx,float rz,string mat)
        {
            var v=new List<Vector3>{centre+Vector3.up*.004f};var uv=new List<Vector2>{new Vector2(centre.x/2,centre.z/2)};var t=new List<int>();const int n=48;
            for(var i=0;i<=n;i++){var a=(i%n)*Mathf.PI*2/n;var f=.94f+.06f*Mathf.Sin(a*7+rx);var point=centre+new Vector3(Mathf.Cos(a)*rx*f,.004f,Mathf.Sin(a)*rz*f);v.Add(point);uv.Add(new Vector2(point.x/2,point.z/2));if(i<n)t.AddRange(new[]{0,i+2,i+1});}
            var group=V.Group(p,name);V.Part(group,name,V.Mesh(name,v,t,uv),mat,Vector3.zero);V.Merge(group,name.Replace(" ",""));
        }
        private static void Entry(Transform p)
        {
            var entry=V.Group(p,"Village entrance");entry.position=new Vector3(-10,0,-16);
            foreach(var x in new[]{-1.9f,1.9f})
            {var post=V.Group(entry,"Gate pier");post.localPosition=new Vector3(x,0,0);V.Box(post,"Stone base",Vector3.up*.48f,new Vector3(.58f,.96f,.58f),"GreyStone");V.Box(post,"Timber pier",Vector3.up*1.6f,new Vector3(.26f,3.2f,.26f),"DarkOak");V.Merge(post,x<0?"EntryLeft":"EntryRight");Solid(post,Vector3.up*1.6f,new Vector3(.6f,3.2f,.6f));}
            var beam=V.Group(entry,"Gate lintel");V.Box(beam,"Weathered lintel",Vector3.up*3,new Vector3(4.3f,.24f,.3f),"AgedOak");V.Merge(beam,"EntryLintel");
        }
        private static void Banner(Transform p)
        {
            var b=V.Group(p,"Faction standard on original marker");b.position=new Vector3(-15,0,5);
            V.Box(b,"Stone marker base",Vector3.up*.4f,Vector3.one*.8f,"GreyStone");V.Pole(b,Vector3.up*.8f,Vector3.up*2.7f,.045f,.035f,"DarkOak");
            V.Box(b,"Cloth standard",new Vector3(.37f,2.2f,0),new Vector3(.66f,.75f,.028f),"Linen");V.Box(b,"Bronze emblem",new Vector3(.37f,2.2f,-.018f),new Vector3(.14f,.25f,.02f),"Bronze");V.Merge(b,"FactionStandard");
        }
        private static void Market(Transform p)
        {
            var node=V.Group(p,"Market over original interaction");node.position=new Vector3(1,0,-8);
            var low=V.Group(node,"Counter");V.Box(low,"Counter",Vector3.up*.48f,new Vector3(.8f,.96f,.8f),"AgedOak");V.Merge(low,"MarketCounter");
            var high=V.Group(node,"Context canopy");foreach(var x in new[]{-.34f,.34f})V.Box(high,"Post",new Vector3(x,1.25f,.32f),new Vector3(.07f,2.5f,.08f),"DarkOak");
            V.Box(high,"Canvas awning",new Vector3(0,2.45f,0),new Vector3(2.3f,.07f,1.65f),"Linen");V.Box(high,"Canopy edge",new Vector3(0,2.3f,-.8f),new Vector3(2.3f,.27f,.04f),"Linen");V.Merge(high,"MarketCanopy");Cut(node.gameObject,high.GetComponentsInChildren<Renderer>());
        }
        private static void Lamp(Transform p,Vector3 at)
        {
            var lamp=PlaceProp(p,"VillageLantern_250cm",at);Solid(lamp.transform,Vector3.up*1.25f,new Vector3(.15f,2.5f,.15f));
            var light=new GameObject("Warm lantern light").AddComponent<Light>();light.transform.SetParent(lamp.transform,false);light.transform.localPosition=new Vector3(.5f,2.08f,-.12f);light.type=LightType.Point;light.color=new Color(1,.66f,.29f);light.intensity=1.5f;light.range=3.4f;light.shadows=LightShadows.None;
        }
        private static void Sign(Transform p,Vector3 at,string caption)
        {
            var go=PlaceProp(p,"WayfindingSign_190cm",at);Solid(go.transform,Vector3.up*.95f,new Vector3(.16f,1.9f,.16f));
            var label=new GameObject("Wayfinding text",typeof(RectTransform),typeof(Canvas));label.transform.SetParent(go.transform,false);label.transform.localPosition=new Vector3(0,1.55f,-.09f)-camera.transform.forward*.8f;label.transform.rotation=camera.transform.rotation;label.transform.localScale=Vector3.one*.006f;
            label.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;label.GetComponent<Canvas>().sortingOrder=-20;label.GetComponent<RectTransform>().sizeDelta=new Vector2(200,48);
            var text=new GameObject("Caption",typeof(RectTransform),typeof(Text));text.transform.SetParent(label.transform,false);var rt=text.GetComponent<RectTransform>();rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;
            var t=text.GetComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=22;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleCenter;t.color=new Color(.89f,.82f,.62f);t.text=caption;t.raycastTarget=false;
        }
        private static void Forest(Transform p)
        {
            var positions=new[]{new Vector3(-18,0,1),new Vector3(-18.5f,0,6.5f),new Vector3(-18.5f,0,13),new Vector3(-18,0,19),new Vector3(-12,0,20.5f),new Vector3(-6.5f,0,21),new Vector3(1,0,20),new Vector3(8,0,20),new Vector3(15,0,18.5f),new Vector3(20,0,13),new Vector3(21,0,5),new Vector3(20,0,-3),new Vector3(18.5f,0,-14),new Vector3(10,0,-19),new Vector3(1,0,-20.5f),new Vector3(-16.5f,0,-16),new Vector3(-22,0,21),new Vector3(-19,0,25),new Vector3(1,0,25),new Vector3(14,0,24),new Vector3(25,0,10)};
            for(var i=0;i<positions.Length;i++)
            {
                var at=positions[i];at.y=VillageE07Nature.BackdropHeight(at.x,at.z);var go=V.Place(V.Load("Vegetation",i%3==0?"SlenderTree_8m":"Broadleaf_7m"),p,at,i*47);
                Cut(go,go.GetComponentsInChildren<Renderer>());
            }
            foreach(var at in new[]{new Vector3(-16.5f,0,7.5f),new Vector3(-17,0,11),new Vector3(17,0,9),new Vector3(11,0,17),new Vector3(-16,0,-3.5f)})V.Place(V.Load("Vegetation","Boulder_120cm"),p,at,at.x*19);
            V.Place(V.Load("Vegetation","FallenLog_220cm"),p,new Vector3(-16.5f,0,3),32);
            var random=new System.Random(1707);
            for(var i=0;i<160;i++)
            {
                var x=(float)random.NextDouble()*38-19;var z=(float)random.NextDouble()*38-19;
                // Intentional zones: woods/perimeters/gardens, never a random spread through the playable corridors.
                if(!((x< -16&&z> -2)||(z>17)||(x>17)||(x>10&&z>10)||(x< -11&&z< -12)))continue;
                var go=V.Place(V.Load("Vegetation",i%3==0?"Shrub_90cm":"Understory_35cm"),p,new Vector3(x,0,z),i*37);go.transform.localScale=Vector3.one*(.75f+(float)random.NextDouble()*.45f);
            }
        }
        private static void Lighting(Transform p)
        {
            foreach(var volume in Object.FindObjectsByType<Volume>(FindObjectsSortMode.None))volume.enabled=false;
            var sun=GameObject.Find("Directional Light").GetComponent<Light>();sun.color=new Color(1,.89f,.73f);sun.intensity=2;sun.shadows=LightShadows.Soft;sun.shadowStrength=.87f;sun.shadowBias=.06f;sun.shadowNormalBias=.25f;sun.transform.rotation=Quaternion.Euler(48,-32,0);
            RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.37f,.45f,.56f);RenderSettings.ambientEquatorColor=new Color(.33f,.35f,.34f);RenderSettings.ambientGroundColor=new Color(.14f,.18f,.13f);
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Exponential;RenderSettings.fogColor=new Color(.30f,.36f,.39f);RenderSettings.fogDensity=.0035f;camera.backgroundColor=RenderSettings.fogColor;
            var path=M.Root+"/E07Atmosphere.asset";var profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,path);
            var tone=profile.Add<Tonemapping>(true);tone.mode.Override(TonemappingMode.ACES);AssetDatabase.AddObjectToAsset(tone,profile);
            var color=profile.Add<ColorAdjustments>(true);color.postExposure.Override(.18f);color.contrast.Override(8);color.saturation.Override(-7);AssetDatabase.AddObjectToAsset(color,profile);
            var bloom=profile.Add<Bloom>(true);bloom.intensity.Override(.10f);bloom.threshold.Override(1.2f);AssetDatabase.AddObjectToAsset(bloom,profile);
            var vignette=profile.Add<Vignette>(true);vignette.intensity.Override(.08f);vignette.smoothness.Override(.45f);AssetDatabase.AddObjectToAsset(vignette,profile);
            var node=new GameObject("E07 late afternoon atmosphere");node.transform.SetParent(p,false);var v=node.AddComponent<Volume>();v.isGlobal=true;v.priority=20;v.sharedProfile=profile;
            var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;data.antialiasing=AntialiasingMode.SubpixelMorphologicalAntiAliasing;data.antialiasingQuality=AntialiasingQuality.High;PrefabUtility.RecordPrefabInstancePropertyModifications(data);
        }
    }
}
