using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using V=Aethernia.Editor.Environment.VillageE07Geometry;
using G=Aethernia.Editor.Environment.EnvironmentGeometry;

namespace Aethernia.Editor.Environment
{
    public static class VillageE07Nature
    {
        public static void Create()
        {
            if(V.Load("Vegetation","Broadleaf_7m")!=null)throw new InvalidOperationException("E07 nature exists; do not regenerate.");
            Tree("Broadleaf_7m",713,1);Tree("SlenderTree_8m",1517,1.15f);
            V.Save("Vegetation","Shrub_90cm",p=>Leaves(p,new Vector3(0,.45f,0),new Vector3(.62f,.35f,.50f),110,.28f,71));
            V.Save("Vegetation","Understory_35cm",p=>Leaves(p,new Vector3(0,.16f,0),new Vector3(.28f,.09f,.24f),32,.18f,17));
            V.Save("Vegetation","Boulder_120cm",p=>{Rock(p,1.2f,.72f,411);SolidBox(p,new Vector3(0,.33f,0),new Vector3(1.1f,.66f,1));});
            V.Save("Vegetation","FallenLog_220cm",p=>{V.Pole(p,new Vector3(-1.1f,.24f,0),new Vector3(1.1f,.28f,0),.24f,.20f,"DarkOak");V.Pole(p,new Vector3(-.1f,.3f,0),new Vector3(.1f,.6f,.15f),.1f,.03f,"DarkOak");SolidBox(p,new Vector3(0,.26f,0),new Vector3(2.2f,.52f,.48f));});
            V.Save("Ground","Backdrop_80m",p=>V.Part(p,"Organic backdrop",Backdrop(),"Grass",Vector3.zero));
            V.Save("Ground","RoadSegment_3x6m",p=>V.Part(p,"Irregular dirt",Road(new[]{new Vector3(0,0,-3),new Vector3(0,0,3)},1.5f),"Earth",Vector3.zero));
            V.Save("Ground","CobbledSquare_10x8m",Plaza);
            VillageE07Props.Create();AssetDatabase.SaveAssets();Debug.Log("E07_NATURE: coherent terrain, reusable foliage/trees, rocks and medieval props; no scene or baked NavMesh changed.");
        }
        private static void Tree(string name,int seed,float scale)
        {
            V.Save("Vegetation",name,p=>
            {
                V.Pole(p,Vector3.zero,new Vector3(.13f,5.5f*scale,.05f),.27f,.07f,"DarkOak");
                for(var i=0;i<7;i++)
                {
                    var a=i*2.39996f;var tip=new Vector3(Mathf.Cos(a)*1.6f,4+(i%3)*.65f,Mathf.Sin(a)*1.65f)*scale;
                    V.Pole(p,new Vector3(0,(2.5f+i*.23f)*scale,0),tip,.10f,.027f,"DarkOak");
                    Leaves(p,tip,new Vector3(1.12f,.98f,1.08f)*scale,92,.48f*scale,seed+i*101);
                }
                Leaves(p,new Vector3(.1f,5.8f,0)*scale,new Vector3(1.05f,.8f,1.1f)*scale,100,.46f*scale,seed+71);
                var c=p.gameObject.AddComponent<CapsuleCollider>();c.height=3;c.radius=.27f;c.center=Vector3.up*1.5f;
                var o=p.gameObject.AddComponent<NavMeshObstacle>();o.shape=NavMeshObstacleShape.Capsule;o.radius=.31f;o.height=3;o.center=c.center;o.carving=true;
            });
        }
        private static float Rand(System.Random random,float lo,float hi)=>Mathf.Lerp(lo,hi,(float)random.NextDouble());
        public static void Leaves(Transform p,Vector3 centre,Vector3 radius,int count,float length,int seed)
        {
            var random=new System.Random(seed);
            for(var shade=0;shade<2;shade++)
            {
                var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
                for(var i=shade;i<count;i+=2)
                {
                    var direction=new Vector3(Rand(random,-1,1),Rand(random,-1,1),Rand(random,-1,1)).normalized;
                    var point=centre+Vector3.Scale(direction,new Vector3(radius.x*Rand(random,.2f,1),radius.y*Rand(random,.1f,1),radius.z*Rand(random,.2f,1)));
                    var l=length*Rand(random,.65f,1.2f);var rotation=Quaternion.Euler(Rand(random,-38,38),Rand(random,0,360),Rand(random,-28,28));
                    var shape=new[]{Vector3.zero,new Vector3(-l*.34f,.015f,l*.34f),new Vector3(-l*.24f,0,l*.72f),new Vector3(0,0,l),new Vector3(l*.24f,0,l*.72f),new Vector3(l*.34f,.015f,l*.34f),new Vector3(0,l*.07f,l*.48f)};
                    var start=vertices.Count;
                    foreach(var local in shape){vertices.Add(point+rotation*local);uv.Add(new Vector2(local.x/l+.5f,local.z/l));}
                    for(var j=0;j<6;j++)triangles.AddRange(new[]{start+6,start+j,start+(j+1)%6});
                    // Independent reverse vertices keep correct two-sided normals without transparent sorting.
                    foreach(var local in shape){vertices.Add(point+rotation*local);uv.Add(new Vector2(local.x/l+.5f,local.z/l));}
                    for(var j=0;j<6;j++)triangles.AddRange(new[]{start+13,start+7+(j+1)%6,start+7+j});
                }
                V.Part(p,"Leaf cluster",V.Mesh("Leaves",vertices,triangles,uv),shade==0?"LeafLight":"LeafDark",Vector3.zero);
            }
        }
        private static void Rock(Transform p,float diameter,float height,int seed)
        {
            var rng=new System.Random(seed);var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();const int sides=11;
            for(var row=0;row<4;row++)for(var i=0;i<=sides;i++)
            {
                var a=(i%sides)*Mathf.PI*2/sides;var r=(row==0?.39f:row==3?.27f:.5f)*diameter*(.9f+.1f*Mathf.Sin(i*3.7f+seed));
                v.Add(new Vector3(Mathf.Cos(a)*r,row*height/3,Mathf.Sin(a)*r*.87f));uv.Add(new Vector2((float)i/sides*2,row/3f));
                if(row>0&&i<sides){var k=row*(sides+1)+i;t.AddRange(new[]{k,k+1,k-sides-1,k+1,k-sides,k-sides-1});}
            }
            var n=v.Count;v.Add(Vector3.up*height*.98f);uv.Add(Vector2.one*.5f);for(var i=0;i<sides;i++)t.AddRange(new[]{n,3*(sides+1)+i+1,3*(sides+1)+i});
            V.Part(p,"Weathered stone",V.Mesh("Boulder",v,t,uv),"GreyStone",Vector3.zero);
        }
        public static void SolidBox(Transform p,Vector3 centre,Vector3 size)
        {
            var c=p.gameObject.AddComponent<BoxCollider>();c.center=centre;c.size=size;
            var o=p.gameObject.AddComponent<NavMeshObstacle>();o.carving=true;o.center=centre;o.size=size;
        }
        public static float BackdropHeight(float x,float z)
        {var edge=Mathf.Max(Mathf.Abs(x),Mathf.Abs(z));return edge<=20?0:Mathf.SmoothStep(0,1,Mathf.Clamp01((edge-20)/12))*(.6f+Mathf.PerlinNoise(x*.13f+71,z*.13f+19)*1.7f);}
        private static Mesh Backdrop()
        {
            var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();const int n=80;
            for(var z=0;z<=n;z++)for(var x=0;x<=n;x++)
            {var px=x-40;var pz=z-40;v.Add(new Vector3(px,BackdropHeight(px,pz),pz));uv.Add(new Vector2(px/2f,pz/2f));if(x<n&&z<n){var k=z*(n+1)+x;t.AddRange(new[]{k,k+n+1,k+1,k+1,k+n+1,k+n+2});}}
            return V.Mesh("Backdrop",v,t,uv);
        }
        public static Mesh Road(Vector3[] points,float halfWidth,float height=.015f)
        {
            var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();var distance=0f;
            for(var s=0;s<points.Length-1;s++)
            {
                var forward=(points[s+1]-points[s]).normalized;var side=Vector3.Cross(Vector3.up,forward);var steps=Mathf.CeilToInt(Vector3.Distance(points[s],points[s+1])/.45f);
                for(var j=0;j<=steps;j++)
                {
                    var centre=Vector3.Lerp(points[s],points[s+1],(float)j/steps);var width=halfWidth*(.91f+Mathf.PerlinNoise(centre.x*.9f+13,centre.z*.9f+11)*.20f);
                    if(j>0)distance+=Vector3.Distance(points[s],points[s+1])/steps;
                    var n=v.Count;v.Add(centre-side*width+Vector3.up*height);v.Add(centre+side*width+Vector3.up*height);uv.Add(new Vector2(0,distance/2));uv.Add(new Vector2(width,distance/2));
                    if(j>0)t.AddRange(new[]{n-2,n,n-1,n-1,n,n+1});
                }
            }
            return V.Mesh("WornPath",v,t,uv);
        }
        private static void Plaza(Transform p)
        {
            for(var row=0;row<19;row++)for(var col=0;col<24;col++)
            {
                var x=(col-11.5f)*.4f+(row%2)*.2f;var z=(row-9)*.4f;
                if(x*x/25+z*z/16>1.05f)continue;
                var y=.012f+Mathf.PerlinNoise(col*1.4f,row*1.7f)*.006f;
                var g=V.Box(p,"Worn cobble",new Vector3(x,y,z),new Vector3(.375f,.02f,.355f),row%4==0?"WarmStone":"GreyStone");g.transform.localRotation=Quaternion.Euler(0,Mathf.Sin(col*row)*4,0);
            }
        }
    }
}
