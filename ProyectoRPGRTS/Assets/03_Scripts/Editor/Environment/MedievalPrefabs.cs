using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using G = Aethernia.Editor.Environment.EnvironmentGeometry;

namespace Aethernia.Editor.Environment
{
    public static class MedievalPrefabs
    {
        public const string Root = "Assets/02_Prefabs/Environment/Medieval";
        public static GameObject Load(string folder, string name) => AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/"+folder+"/"+name+".prefab");
        private static GameObject Save(string folder, string name, Action<Transform> build, bool consolidate = true)
        {
            EnvironmentMaterials.Folder(Root+"/"+folder);
            var root = new GameObject(name);
            try
            {
                build(root.transform); if(consolidate) G.Consolidate(root,name); G.Persist(root);
                return PrefabUtility.SaveAsPrefabAsset(root,Root+"/"+folder+"/"+name+".prefab");
            }
            finally { Object.DestroyImmediate(root); }
        }
        public static GameObject Place(GameObject prefab, Transform parent, Vector3 position, float yaw = 0)
        {
            var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab,parent);
            go.transform.localPosition=position; go.transform.localRotation=Quaternion.Euler(0,yaw,0); return go;
        }
        public static void Create()
        {
            Save("Architecture","WallBay_2m", p =>
            {
                G.Box(p,"Infill",new Vector3(0,1.5f,0),new Vector3(2,3,.2f),"Plaster");
                G.Box(p,"Stone footing",new Vector3(0,.3f,-.015f),new Vector3(2,.6f,.23f),"Stone");
                foreach(var x in new[]{-.92f,.92f}) G.Box(p,"Post",new Vector3(x,1.5f,-.035f),new Vector3(.16f,3,.22f),"Oak");
                foreach(var y in new[]{.65f,2.75f}) G.Box(p,"Rail",new Vector3(0,y,-.035f),new Vector3(2,.15f,.22f),"Oak");
                var brace=G.Box(p,"Diagonal",new Vector3(0,1.7f,-.12f),new Vector3(.11f,2.5f,.08f),"Oak"); brace.transform.localRotation=Quaternion.Euler(0,0,-34);
            });
            Save("Architecture","OpenDoor_120x220cm", p =>
            {
                foreach(var x in new[]{-.7f,.7f})
                {
                    G.Box(p,"Jamb",new Vector3(x,1.2f,0),new Vector3(.2f,2.4f,.25f),"Oak");
                    foreach(var y in new[]{.35f,1.8f}) G.Box(p,"Iron strap",new Vector3(x,y,-.13f),new Vector3(.21f,.07f,.025f),"Iron");
                }
                G.Box(p,"Lintel",new Vector3(0,2.3f,0),new Vector3(1.2f,.2f,.25f),"Oak");
            });
            Save("Architecture","Window_100x120cm", p =>
            {
                foreach(var x in new[]{-.6f,.6f}) G.Box(p,"Jamb",new Vector3(x,.8f,0),new Vector3(.2f,1.6f,.2f),"Oak");
                foreach(var y in new[]{.1f,1.5f}) G.Box(p,"Sill",new Vector3(0,y,0),new Vector3(1,.2f,.2f),"Oak");
                G.Box(p,"Recess",new Vector3(0,.8f,.025f),new Vector3(1,1.2f,.025f),"Iron");
                G.Box(p,"Vertical mullion",new Vector3(0,.8f,-.035f),new Vector3(.045f,1.2f,.06f),"Oak");
                G.Box(p,"Horizontal mullion",new Vector3(0,.8f,-.035f),new Vector3(1,.045f,.06f),"Oak");
            });
            Save("Architecture","GableRoof_660x860cm", p =>
            {
                var vertices=new List<Vector3>(); var uv=new List<Vector2>(); var triangles=new List<int>();
                G.Face(vertices,uv,triangles,new Vector3(-3.3f,0,-4.3f),new Vector3(-3.3f,0,4.3f),new Vector3(0,2,4.3f),new Vector3(0,2,-4.3f));
                G.Face(vertices,uv,triangles,new Vector3(0,2,-4.3f),new Vector3(0,2,4.3f),new Vector3(3.3f,0,4.3f),new Vector3(3.3f,0,-4.3f));
                G.Part(p,"Slates",G.Save("GableRoof",vertices.ToArray(),triangles.ToArray(),uv.ToArray()),"Slate",Vector3.zero);
                foreach(var z in new[]{-4.3f,4.3f})
                {
                    var gable=G.Save("Gable",new[]{new Vector3(-3.3f,0,0),new Vector3(0,2,0),new Vector3(3.3f,0,0),new Vector3(-3.3f,0,0),new Vector3(0,2,0),new Vector3(3.3f,0,0)},new[]{0,1,2,5,4,3},new[]{Vector2.zero,new Vector2(1.65f,1),new Vector2(3.3f,0),Vector2.zero,new Vector2(1.65f,1),new Vector2(3.3f,0)});
                    G.Part(p,"Gable plaster",gable,"Plaster",new Vector3(0,0,z));
                    foreach(var side in new[]{-1,1})
                    {
                        var beam=G.Box(p,"Barge board",new Vector3(side*1.65f,1,z),new Vector3(.16f,3.86f,.16f),"Oak");
                        beam.transform.localRotation=Quaternion.Euler(0,0,side*58.78f);
                    }
                    G.Box(p,"Gable centre",new Vector3(0,.88f,z),new Vector3(.14f,1.8f,.15f),"Oak");
                    G.Box(p,"Gable sill",new Vector3(0,0,z),new Vector3(6.6f,.16f,.18f),"Oak");
                }
                G.Box(p,"Ridge",new Vector3(0,2.02f,0),new Vector3(.19f,.15f,8.7f),"Slate");
                foreach(var x in new[]{-3.3f,3.3f}) G.Box(p,"Eave",new Vector3(x,-.025f,0),new Vector3(.16f,.2f,8.7f),"Oak");
            });
            Save("Props","Crate_80cm", p => Crate(p,.8f));
            Save("Props","CrateStack_3x2x2m", p =>
            {
                for(var x=0;x<3;x++) for(var y=0;y<2;y++) for(var z=0;z<2;z++)
                {
                    var node=new GameObject("Crate").transform; node.SetParent(p,false); node.localPosition=new Vector3(x-1,y,z-.5f);
                    Crate(node,1);
                }
            });
            Save("Props","Barrel_100cm", p =>
            {
                G.Part(p,"Staves",G.Lathe("Barrel",new[]{0f,.07f,.25f,.5f,.75f,.93f,1f},new[]{.41f,.43f,.47f,.48f,.47f,.43f,.41f}),"Oak",Vector3.zero);
                foreach(var y in new[]{.12f,.48f,.84f})
                {
                    var radius=y<.2f||y>.8f?.45f:.485f;
                    G.Part(p,"Hoop",G.Lathe("Hoop_"+y,new[]{y,y+.055f},new[]{radius,radius}),"Iron",Vector3.zero);
                }
                G.Box(p,"Lid brace",new Vector3(0,1.007f,0),new Vector3(.72f,.025f,.07f),"Oak");
            });
            Save("Props","Chest_80x80x60cm", p =>
            {
                G.Box(p,"Body",new Vector3(0,.29f,0),new Vector3(.8f,.58f,.6f),"Oak");
                G.Box(p,"Lid",new Vector3(0,.68f,0),new Vector3(.8f,.24f,.6f),"Oak");
                foreach(var x in new[]{-.27f,.27f})
                {
                    G.Box(p,"Strap top",new Vector3(x,.805f,0),new Vector3(.045f,.015f,.6f),"Iron");
                    foreach(var z in new[]{-.305f,.305f}) G.Box(p,"Strap front",new Vector3(x,.40f,z),new Vector3(.045f,.8f,.015f),"Iron");
                }
                G.Box(p,"Lock",new Vector3(0,.59f,-.317f),new Vector3(.10f,.15f,.03f),"Iron");
            });
            Save("Architecture","TimberHouse_6x8m", House, false);
            Save("Vegetation","GroundFern", p => Plants(p,false));
            Save("Vegetation","GrassClump", p => Plants(p,true));
        }
        private static void Crate(Transform p,float size)
        {
            G.Box(p,"Planks",Vector3.up*size*.5f,new Vector3(size,size,size)*.94f,"Oak");
            foreach(var x in new[]{-1,1}) foreach(var z in new[]{-1,1})
                G.Box(p,"Corner rail",new Vector3(x*size*.44f,size*.5f,z*size*.44f),new Vector3(size*.12f,size,size*.12f),"Oak");
            foreach(var y in new[]{.07f,.93f}) foreach(var z in new[]{-1,1})
                G.Box(p,"Rail",new Vector3(0,size*y,z*size*.47f),new Vector3(size,size*.12f,size*.06f),"Oak");
            foreach(var z in new[]{-1,1})
            {
                var brace=G.Box(p,"Brace",new Vector3(0,size*.5f,z*size*.48f),new Vector3(size*.11f,size*1.13f,size*.035f),"Oak");
                brace.transform.localRotation=Quaternion.Euler(0,0,44);
            }
        }
        private static void House(Transform p)
        {
            var bay=Load("Architecture","WallBay_2m");
            for(var i=0;i<3;i++) Place(bay,p,new Vector3(-2+i*2,0,3.9f),180);
            for(var i=0;i<4;i++)
            { Place(bay,p,new Vector3(-2.9f,0,-3+i*2),90); Place(bay,p,new Vector3(2.9f,0,-3+i*2),-90); }
            foreach(var x in new[]{-1.9f,1.9f})
            {
                var panel=Place(bay,p,new Vector3(x,0,-3.9f)); panel.transform.localScale=new Vector3(1.1f,1,1);
                Place(Load("Architecture","Window_100x120cm"),p,new Vector3(x,.8f,-4.04f));
            }
            Place(Load("Architecture","OpenDoor_120x220cm"),p,new Vector3(0,0,-3.875f));
            Place(Load("Architecture","GableRoof_660x860cm"),p,new Vector3(0,3.1f,0));
            foreach(var z in new[]{-2f,1f}) Place(Load("Architecture","Window_100x120cm"),p,new Vector3(-3.03f,.8f,z),90);
            G.Box(p,"Chimney",new Vector3(1.5f,4.3f,1.8f),new Vector3(.6f,2.3f,.65f),"Stone");
            G.Box(p,"Chimney cap",new Vector3(1.5f,5.5f,1.8f),new Vector3(.78f,.16f,.82f),"Stone");
            G.Box(p,"Chimney flue",new Vector3(1.5f,5.587f,1.8f),new Vector3(.44f,.015f,.48f),"Iron");
        }
        private static void Plants(Transform p,bool grass)
        {
            var vertices=new List<Vector3>(); var uv=new List<Vector2>(); var tris=new List<int>();
            var count=grass?19:13;
            for(var i=0;i<count;i++)
            {
                var angle=i*2.39996f; var dir=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
                var side=Vector3.Cross(Vector3.up,dir)*(grass?.028f:.07f);
                var a=dir*.03f; var b=dir*(grass?.14f:.23f)+Vector3.up*(grass?.23f:.26f); var tip=dir*(grass?.20f:.43f)+Vector3.up*(grass?.19f:.15f);
                var start=vertices.Count; vertices.AddRange(new[]{a-side,b-side*.65f,tip,b+side*.65f,a+side});
                uv.AddRange(new[]{Vector2.zero,new Vector2(0,.5f),new Vector2(.5f,1),new Vector2(1,.5f),Vector2.right});
                tris.AddRange(new[]{start,start+1,start+2,start,start+2,start+3,start,start+3,start+4});
                for(var k=0;k<5;k++) { vertices.Add(vertices[start+k]); uv.Add(uv[start+k]); }
                tris.AddRange(new[]{start+7,start+6,start+5,start+8,start+7,start+5,start+9,start+8,start+5});
            }
            G.Part(p,"Leaves",G.Save(grass?"Grass":"Fern",vertices.ToArray(),tris.ToArray(),uv.ToArray()),"Foliage",Vector3.zero);
        }
    }
}
