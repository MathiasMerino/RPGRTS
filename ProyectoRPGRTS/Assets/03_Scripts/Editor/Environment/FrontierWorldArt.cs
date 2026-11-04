using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using V=Aethernia.Editor.Environment.VillageE07Geometry;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Environment
{
    public static class FrontierWorldArt
    {
        public const string Source="Assets/01_Scenes/AetherniaE23IntegratedAlpha.unity";
        public const string Candidate="Assets/01_Scenes/AetherniaArtEnv01.unity";
        public const string Library="Assets/06_Environment/ArtEnv01";
        public const string Study="Assets/01_Scenes/AetherniaArtEnv01Study.unity";
        private static readonly Quaternion Q=Quaternion.identity;
        private static Material oak,lightOak,stone,lightStone,plaster,iron,glass;
        private static Material[] slate,clay,canvas,plants;

        public static void Author()
        {
            if(File.Exists(Candidate)||AssetDatabase.IsValidFolder(Library))
                throw new InvalidOperationException("ART-ENV-01 is already authored. Modify its candidate assets explicitly.");
            foreach(var folder in new[]{"","/Meshes","/Materials","/Prefabs"})EnvironmentMaterials.Folder(Library+folder);
            LoadPalette();
            var scene=EditorSceneManager.OpenScene(Source);
            var flow=Object.FindAnyObjectByType<DemoFlow>();
            var root=new GameObject("ART-ENV-01 · frontera");
            var roofs=new[]{
                ("TimberHouse_6x8m",6f,8f,2f,clay),
                ("VillageInn_55x6m",5.5f,6f,2.2f,slate),
                ("StoneCottage_45x45m",4.5f,4.5f,1.65f,slate)
            };
            foreach(var (name,width,length,rise,tiles) in roofs)
            {
                var house=GameObject.Find(name);
                if(house==null)throw new InvalidOperationException("Missing integrated building "+name);
                var pitched=house.transform.Find("Contextual cutaway/Pitched roof");
                if(pitched==null)throw new InvalidOperationException("Missing integrated roof "+name);
                var cover=Prefab(name+"_Tejas",draft=>Roof(draft,width+.6f,length+.6f,rise,tiles));
                var newRoof=V.Place(cover,pitched,Vector3.zero);
                var walls=house.transform.Find("Contextual cutaway/Upper walls");
                var facade=Prefab(name+"_Fachada",draft=>Facade(draft,width,length,rise,tiles==clay,name.StartsWith("VillageInn")));
                var newFacade=V.Place(facade,pitched,Vector3.zero);
                var cut=house.GetComponent<RoofCutaway>();
                if(cut!=null)
                {
                    var existing=new SerializedObject(cut).FindProperty("roof");
                    var renderers=new List<Renderer>();
                    for(var i=0;i<existing.arraySize;i++)
                        if(existing.GetArrayElementAtIndex(i).objectReferenceValue is Renderer r)renderers.Add(r);
                    renderers.AddRange(newRoof.GetComponentsInChildren<Renderer>());
                    renderers.AddRange(newFacade.GetComponentsInChildren<Renderer>());
                    cut.Configure(flow.CameraRig.GetComponent<Camera>(),flow.Director.Hero.transform,renderers.ToArray());
                    PrefabUtility.RecordPrefabInstancePropertyModifications(cut);
                }
                if(walls==null)throw new InvalidOperationException("Missing wall anchor "+name);
            }

            var market=Prefab("Mercado_Toldo_Lona",Market);
            var marketAnchor=GameObject.Find("Market over original interaction").transform;
            var oldCanopy=marketAnchor.Find("Context canopy");
            if(oldCanopy==null)oldCanopy=marketAnchor.Cast<Transform>().FirstOrDefault(t=>t.name.Contains("Canopy"));
            if(oldCanopy!=null)
                foreach(var renderer in oldCanopy.GetComponentsInChildren<Renderer>())renderer.enabled=false;
            var canopy=V.Place(market,marketAnchor,Vector3.zero);
            var marketCut=marketAnchor.GetComponent<RoofCutaway>();
            if(marketCut!=null)marketCut.Configure(flow.CameraRig.GetComponent<Camera>(),flow.Director.Hero.transform,canopy.GetComponentsInChildren<Renderer>());

            var set=Prefab("Mercado_Cajas_Y_Cestos",MarketStock);
            Place(set,root.transform,new Vector3(-.1f,0,-8.4f),22);
            Place(set,root.transform,new Vector3(1.9f,0,-7.6f),-20);
            Place(set,root.transform,new Vector3(3.1f,0,3.8f),-10);
            var step=Prefab("Sendero_Adoquin_Desgastado",draft=>Cobbles(draft,33),false);
            foreach(var p in new[]{new Vector3(-9,0,-3.2f),new Vector3(-5,0,-3.7f),new Vector3(-2.6f,0,-2.4f),
                new Vector3(2.4f,0,-4.2f),new Vector3(-17,0,.9f),new Vector3(-24,0,-3.1f),new Vector3(-25.5f,0,-7.8f),
                new Vector3(-21.5f,0,-13.5f),new Vector3(-16,0,-16),new Vector3(-37,0,-2.1f)})
                Place(step,root.transform,p,Mathf.Abs(p.x*12));

            var foliage=Prefab("Bosque_Borde_Helechos_Y_Carex",Foliage,false);
            foreach(var p in new[]{new Vector3(-8,0,4),new Vector3(-7,0,-6),new Vector3(-13,0,-5),
                new Vector3(-19,0,2.6f),new Vector3(-22,0,-6.3f),new Vector3(-26,0,1.3f),
                new Vector3(-30,0,-8.6f),new Vector3(-19.5f,0,-15.3f),new Vector3(-32,0,3.3f),
                new Vector3(-36,0,-5.9f),new Vector3(-43,0,3.6f),new Vector3(-40,0,-8.1f)})
                Place(foliage,root.transform,p,p.x*37);
            var ruin=Prefab("Ruinas_Portada_Dovelas",Ruin);
            Place(ruin,root.transform,new Vector3(-16,0,-18.7f),0);
            Place(ruin,root.transform,new Vector3(89,0,3.4f),-90);
            var palisade=Prefab("Campamento_Empalizada_Roble",Palisade);
            Place(palisade,root.transform,new Vector3(-46,0,-8.4f),0);
            Place(palisade,root.transform,new Vector3(-29,0,9.8f),12);
            Place(palisade,root.transform,new Vector3(-25,0,10.5f),12);
            Place(set,root.transform,new Vector3(-43.7f,0,-8.8f),-45);

            var elf=Prefab("Piloto_Elfico_Arco_Vivo",Elven);
            var dwarf=Prefab("Piloto_Enano_Portico_Forja",Dwarven);
            var orc=Prefab("Piloto_Orco_Refugio_De_Piel",Orcish);
            EditorSceneManager.MarkSceneDirty(scene);
            if(!EditorSceneManager.SaveScene(scene,Candidate,true))throw new InvalidOperationException(Candidate);
            CreateStudy(elf,dwarf,orc,palisade,ruin);
            AssetDatabase.SaveAssets();
        }

        public static void Build()
        {
            const string destination="Builds/ArtEnv01Windows/Aethernia.exe";
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes=new[]{Candidate},locationPathName=destination,
                target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development
            });
            if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new InvalidOperationException("ART-ENV-01 Windows build failed.");
            Directory.CreateDirectory("Logs/ARTENV01");
            File.WriteAllText("Logs/ARTENV01/build.txt","PASS "+Application.unityVersion+" "+report.summary.totalSize+" bytes");
        }

        private static void LoadPalette()
        {
            oak=VillageE07Materials.Load("DarkOak");lightOak=VillageE07Materials.Load("AgedOak");
            stone=VillageE07Materials.Load("GreyStone");lightStone=VillageE07Materials.Load("WarmStone");
            plaster=VillageE07Materials.Load("Plaster");iron=VillageE07Materials.Load("Iron");glass=VillageE07Materials.Load("EmberGlass");
            var baseSlate=VillageE07Materials.Load("Slate");
            slate=new[]{Tint(baseSlate,"Pizarra húmeda",new Color(.41f,.47f,.52f)),Tint(baseSlate,"Pizarra gris",new Color(.51f,.56f,.59f))};
            clay=new[]{Tint(baseSlate,"Teja de arcilla",new Color(.64f,.39f,.29f)),Tint(baseSlate,"Teja cocida oscura",new Color(.47f,.29f,.22f))};
            var linen=VillageE07Materials.Load("Linen");
            canvas=new[]{Tint(linen,"Lona cáñamo",new Color(.66f,.51f,.32f),true),Tint(linen,"Lona roja de almagre",new Color(.47f,.22f,.15f),true)};
            plants=new[]{Tint(linen,"Helecho verde",new Color(.23f,.31f,.15f),true),Tint(linen,"Carex oliva",new Color(.36f,.39f,.17f),true),
                Tint(linen,"Brote ocre",new Color(.52f,.41f,.19f),true)};
        }

        private static Material Tint(Material template,string title,Color color,bool doubleSided=false)
        {
            var material=new Material(template){name=title};
            material.SetColor("_BaseColor",color);
            material.SetFloat("_Smoothness",.18f);
            if(doubleSided)material.SetFloat("_Cull",0);
            AssetDatabase.CreateAsset(material,Library+"/Materials/"+title+".mat");
            return material;
        }

        private static GameObject Prefab(string name,Action<FrontierArtMesh> sculpt,bool shadows=true)
        {
            var node=new GameObject(name);
            try
            {
                var draft=new FrontierArtMesh();
                sculpt(draft);
                draft.Save(node.transform,name,Library+"/Meshes/"+name+".asset",shadows);
                return PrefabUtility.SaveAsPrefabAsset(node,Library+"/Prefabs/"+name+".prefab");
            }
            finally{Object.DestroyImmediate(node);}
        }

        private static void Place(GameObject prefab,Transform parent,Vector3 at,float yaw)=>V.Place(prefab,parent,at,yaw);

        private static void Roof(FrontierArtMesh mesh,float width,float length,float rise,Material[] tiles)
        {
            var random=new System.Random(71);
            var rows=Mathf.CeilToInt(width/.74f);
            var columns=Mathf.CeilToInt(length/.42f);
            var slope=rise/(width*.5f);
            foreach(var side in new[]{-1,1})
            {
                var rotation=Quaternion.Euler(0,0,-side*Mathf.Atan(slope)*Mathf.Rad2Deg);
                var step=width*.5f/rows;
                for(var row=0;row<rows;row++)
                    for(var col=0;col<columns;col++)
                    {
                        var x=(row+.5f)*step;
                        var z=-length*.5f+(col+.5f)*length/columns+(row%2==0?-.065f:.065f);
                        var relief=.07f+(float)random.NextDouble()*.025f;
                        mesh.Block(tiles[random.NextDouble()>.72?1:0],new Vector3(side*x,rise-x*slope+relief,z),
                            new Vector3(step*Mathf.Sqrt(1+slope*slope)*1.12f,.074f,length/columns-.013f),.028f,rotation);
                    }
                for(var i=0;i<columns;i+=2)
                {
                    var z=-length*.5f+i*length/columns;
                    mesh.Beam(oak,new Vector3(side*(width*.5f+.045f),-.13f,z),new Vector3(side*(width*.5f-.35f),-.23f,z),.105f,.11f);
                }
            }
            for(var i=0;i<columns;i++)
            {
                var z=-length*.5f+(i+.5f)*length/columns;
                mesh.Block(tiles[i%3==0?1:0],new Vector3(0,rise+.125f,z),new Vector3(.30f,.12f,length/columns-.012f),.07f,Q);
            }
        }

        private static void Facade(FrontierArtMesh mesh,float width,float length,float rise,bool dwelling,bool inn)
        {
            var z=-length*.5f-.31f;
            var rear=-z;
            foreach(var face in new[]{z,rear})
            {
                var offset=Mathf.Sign(face)*.035f;
                mesh.Beam(lightOak,new Vector3(-width*.48f,-.31f,face),new Vector3(width*.48f,-.31f,face),.17f,.16f);
                mesh.Beam(oak,new Vector3(-width*.39f,.10f,face),new Vector3(0,rise*.73f,face),.145f,.14f);
                mesh.Beam(oak,new Vector3(width*.39f,.10f,face),new Vector3(0,rise*.73f,face),.145f,.14f);
                mesh.Block(oak,new Vector3(0,rise*.40f,face+offset),new Vector3(.53f,.63f,.08f),.07f,Q);
                mesh.Block(glass,new Vector3(0,rise*.40f,face+offset*2),new Vector3(.36f,.44f,.04f),.025f,Q);
                mesh.Beam(oak,new Vector3(-.18f,rise*.40f,face+offset*3),new Vector3(.18f,rise*.40f,face+offset*3),.045f,.055f);
                foreach(var side in new[]{-1,1})
                    for(var i=0;i<4;i++)
                    {
                        var x=side*(width*.21f+i*.23f);
                        mesh.Block(i%3==0?lightStone:stone,new Vector3(x,inn?-3.87f:dwelling?-2.86f:-2.41f,face+.11f),
                            new Vector3(.218f,.19f,.18f),.031f,Q);
                    }
            }
            if(inn)
            {
                mesh.Beam(iron,new Vector3(-1.10f,-1.14f,z),new Vector3(-1.10f,-1.14f,z-.60f),.075f,.08f);
                mesh.Block(oak,new Vector3(-1.10f,-1.49f,z-.56f),new Vector3(.86f,.44f,.12f),.06f,Q);
                mesh.Arch(lightOak,new Vector3(-1.10f,-1.52f,z-.63f),.15f,.04f,.03f,7);
            }
        }

        private static void Market(FrontierArtMesh mesh)
        {
            const int strips=9;
            const int samples=10;
            Vector3 At(float x,float t)=>new(x,2.28f+.37f*t+.27f*Mathf.Sin(t*Mathf.PI)-.075f*Mathf.Cos(x*16f)*Mathf.Sin(t*Mathf.PI),-1.03f+t*2.04f);
            for(var stripe=0;stripe<strips;stripe++)
            {
                var x=-1.4f+stripe*2.8f/strips;
                var end=x+2.8f/strips;
                for(var t=0;t<samples;t++)mesh.Face(canvas[stripe%2],At(x,(float)t/samples),At(x,(float)(t+1)/samples),At(end,(float)(t+1)/samples),At(end,(float)t/samples));
                var a=At(x,0);var b=At(end,0);
                mesh.Face(canvas[stripe%2],a,b,b-Vector3.up*.16f,a-Vector3.up*.18f);
            }
            foreach(var x in new[]{-1.30f,1.30f})
            {
                mesh.Beam(oak,new Vector3(x,.15f,-.82f),new Vector3(x,2.36f,-.82f),.14f,.14f);
                mesh.Beam(oak,new Vector3(x,.15f,.86f),new Vector3(x,2.66f,.86f),.14f,.14f);
                mesh.Beam(lightOak,new Vector3(x,1.81f,-.82f),new Vector3(x,2.34f,-.34f),.085f,.09f);
            }
        }

        private static void MarketStock(FrontierArtMesh mesh)
        {
            foreach(var x in new[]{-.28f,.27f})
                for(var i=0;i<4;i++)
                {
                    var y=.10f+i*.105f;
                    mesh.Block(i%2==0?lightOak:oak,new Vector3(x,y,-.20f),new Vector3(.48f,.093f,.085f),.012f,Q);
                    mesh.Block(lightOak,new Vector3(x,y,.19f),new Vector3(.48f,.093f,.085f),.012f,Q);
                }
            foreach(var x in new[]{-.51f,-.04f,.04f,.51f})
            {
                mesh.Block(oak,new Vector3(x,.23f,0),new Vector3(.075f,.45f,.48f),.015f,Q);
                mesh.Block(lightOak,new Vector3(x,.07f,0),new Vector3(.082f,.12f,.48f),.012f,Q);
            }
            var random=new System.Random(18);
            for(var i=0;i<30;i++)
            {
                var x=(float)random.NextDouble()-.5f;
                var z=(float)random.NextDouble()*.32f-.16f;
                mesh.Block(i<18?plants[2]:plants[0],new Vector3(x,.39f+(float)random.NextDouble()*.06f,z),
                    new Vector3(.095f,.09f,.10f),.024f,Quaternion.Euler(13,i*37,5));
            }
        }

        private static void Cobbles(FrontierArtMesh mesh,int seed)
        {
            var random=new System.Random(seed);
            for(var row=0;row<7;row++)
                for(var col=0;col<8;col++)
                {
                    if((row==0||row==6||col==0||col==7)&&random.NextDouble()<.54)continue;
                    var p=new Vector3((col-3.5f)*.31f+(row%2)*.12f,.012f+(float)random.NextDouble()*.012f,(row-3)*.29f);
                    mesh.Block(random.Next(4)==0?lightStone:stone,p,new Vector3(.27f,.048f,.24f),.06f,Quaternion.Euler(0,random.Next(-13,13),0));
                }
        }

        private static void Foliage(FrontierArtMesh mesh)
        {
            var random=new System.Random(82);
            for(var clump=0;clump<10;clump++)
            {
                var root=new Vector3((float)random.NextDouble()*2.3f-1.15f,0,(float)random.NextDouble()*1.75f-.875f);
                for(var branch=0;branch<7;branch++)
                {
                    var angle=branch*.9f+(float)random.NextDouble()*.4f;
                    var direction=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
                    var tip=root+direction*(.24f+(float)random.NextDouble()*.29f)+Vector3.up*(.23f+(float)random.NextDouble()*.28f);
                    var leaf=plants[(clump+branch)%plants.Length];
                    mesh.Leaf(leaf,root,tip,.023f,.018f);
                    for(var pair=1;pair<6;pair++)
                    {
                        var basePoint=Vector3.Lerp(root,tip,pair/7f);
                        var lateral=Vector3.Cross(direction,Vector3.up)*(.14f-pair*.014f);
                        mesh.Leaf(leaf,basePoint,basePoint+lateral+direction*.07f+Vector3.up*.04f,.028f,.014f);
                        mesh.Leaf(leaf,basePoint,basePoint-lateral+direction*.07f+Vector3.up*.04f,.028f,.014f);
                    }
                }
            }
        }

        private static void Ruin(FrontierArtMesh mesh)
        {
            foreach(var side in new[]{-1,1})
                for(var row=0;row<5;row++)
                    for(var col=0;col<3-(row>2?1:0);col++)
                    {
                        var x=side*(1.25f+col*.43f+(row%2)*.14f);
                        mesh.Block((row+col)%3==0?lightStone:stone,new Vector3(x,.17f+row*.34f,0),
                            new Vector3(.40f,.317f,.54f),.058f,Quaternion.Euler(0,(row-col)*2.5f,0));
                    }
            mesh.Arch(lightStone,new Vector3(0,1.68f,0),1.10f,.34f,.55f,13);
            mesh.Block(stone,new Vector3(0,3.03f,0),new Vector3(.38f,.43f,.67f),.065f,Q);
        }

        private static void Palisade(FrontierArtMesh mesh)
        {
            var random=new System.Random(111);
            for(var i=0;i<12;i++)
            {
                var x=(i-5.5f)*.27f;
                var height=1.19f+(float)random.NextDouble()*.38f;
                mesh.Block(i%4==0?lightOak:oak,new Vector3(x,height*.5f,0),new Vector3(.252f,height,.22f),.065f,Quaternion.Euler(0,i*37,random.Next(-4,5)));
                mesh.Face(lightOak,new Vector3(x-.12f,height,-.11f),new Vector3(x+.12f,height,-.11f),new Vector3(x+.02f,height+.21f,-.01f));
            }
            foreach(var y in new[]{.53f,.96f})mesh.Beam(lightOak,new Vector3(-1.57f,y,-.135f),new Vector3(1.57f,y,-.135f),.12f,.14f);
        }

        private static void Elven(FrontierArtMesh mesh)
        {
            foreach(var side in new[]{-1,1})
            {
                var prev=new Vector3(side*1.65f,0,0);
                for(var i=1;i<=10;i++)
                {
                    var t=i/10f;var next=new Vector3(side*1.65f*Mathf.Pow(1-t,.78f),t*3.8f,Mathf.Sin(t*Mathf.PI)*.21f);
                    mesh.Beam(i%3==0?lightOak:oak,prev,next,.28f-i*.012f,.28f-i*.013f);
                    mesh.Leaf(plants[i%2],next,next+new Vector3(side*.62f,.36f,.05f),.17f,.10f);
                    prev=next;
                }
            }
            for(var i=0;i<10;i++)
            {
                var a=i*Mathf.PI*2/10;var b=(i+1)*Mathf.PI*2/10;
                mesh.Beam(lightOak,new Vector3(Mathf.Cos(a)*.28f,2.54f+Mathf.Sin(a)*.28f,-.17f),
                    new Vector3(Mathf.Cos(b)*.28f,2.54f+Mathf.Sin(b)*.28f,-.17f),.044f,.06f);
            }
        }
        private static void Dwarven(FrontierArtMesh mesh)
        {
            foreach(var side in new[]{-1,1})
                for(var i=0;i<6;i++)mesh.Block(i%2==0?stone:lightStone,new Vector3(side*(1.06f+i*.035f),.21f+i*.35f,0),
                    new Vector3(.64f-i*.027f,.326f,.88f),.085f,Q);
            mesh.Arch(stone,new Vector3(0,1.76f,0),.94f,.36f,.88f,11);
            mesh.Block(lightStone,new Vector3(0,2.96f,0),new Vector3(2.96f,.25f,.94f),.09f,Q);
            foreach(var side in new[]{-1,1})mesh.Block(iron,new Vector3(side*.76f,1.37f,-.48f),new Vector3(.17f,.59f,.09f),.028f,Q);
        }
        private static void Orcish(FrontierArtMesh mesh)
        {
            foreach(var side in new[]{-1,1})
            {
                mesh.Beam(oak,new Vector3(side*1.8f,0,-1.25f),new Vector3(0,2.95f,-1.25f),.17f,.17f);
                mesh.Beam(oak,new Vector3(side*1.8f,0,1.25f),new Vector3(0,2.95f,1.25f),.17f,.17f);
                for(var i=0;i<7;i++)
                {
                    var a=-1.3f+i*.38f;var b=a+.39f;
                    mesh.Face(canvas[i%3==0?1:0],new Vector3(side*1.68f,.43f,a),new Vector3(0,2.78f,a),new Vector3(0,2.78f,b),new Vector3(side*1.68f,.43f,b));
                    mesh.Beam(lightOak,new Vector3(side*1.65f,.46f,a),new Vector3(0,2.79f,a),.031f,.043f);
                }
            }
            mesh.Beam(oak,new Vector3(0,2.87f,-1.57f),new Vector3(0,2.87f,1.58f),.16f,.17f);
        }

        private static void CreateStudy(params GameObject[] prefabs)
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var light=new GameObject("Study sunlight").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.3f;light.shadows=LightShadows.Soft;
            light.transform.rotation=Quaternion.Euler(43,-32,0);
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight=new Color(.38f,.43f,.48f);
            var camera=new GameObject("Study camera").AddComponent<Camera>();camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=8;
            camera.transform.rotation=Quaternion.Euler(36,30,0);camera.transform.position=new Vector3(0,0,1)-camera.transform.forward*32;
            var floor=GameObject.CreatePrimitive(PrimitiveType.Plane);floor.name="Shared reference plane";floor.transform.localScale=new Vector3(2.2f,1,1.5f);
            floor.GetComponent<Renderer>().sharedMaterial=VillageE07Materials.Load("Earth");Object.DestroyImmediate(floor.GetComponent<Collider>());
            for(var i=0;i<prefabs.Length;i++)Place(prefabs[i],null,new Vector3((i%3-1)*5.4f,0,i/3*4.6f),0);
            EditorSceneManager.SaveScene(scene,Study);
        }
    }
}
