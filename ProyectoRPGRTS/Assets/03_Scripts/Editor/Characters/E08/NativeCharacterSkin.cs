using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Aethernia.Editor.Characters
{
    // Fuente editable en metros. La superficie corporal es continua; ropa y equipo son capas separadas.
    public sealed class NativeCharacterSkin
    {
        public readonly List<Transform> Bones = new List<Transform>();
        public readonly Dictionary<string, int> Indices = new Dictionary<string, int>();
        public readonly Transform ModelRoot;
        public readonly string ClassId;
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<Vector2> uv = new List<Vector2>();
        private readonly List<BoneWeight> weights = new List<BoneWeight>();
        private readonly List<int>[] triangles = Enumerable.Range(0, 9).Select(_ => new List<int>()).ToArray();
        private readonly Dictionary<Vector3Int, int> welded = new Dictionary<Vector3Int, int>();
        private readonly List<Func<Vector3, float>> shapes = new List<Func<Vector3, float>>();
        private readonly Material[] materials;
        private readonly float bulk;
        public NativeCharacterSkin(Transform model, string id, Material[] shared)
        {
            ModelRoot = model; ClassId = id; materials = shared; bulk = id == "warrior" ? 1.12f : id == "mage" ? .92f : 1;
            Bone("Root", null, Vector3.zero); Bone("Hips", "Root", V(0,.94f,0)); Bone("Spine", "Hips", V(0,1.10f,0));
            Bone("Chest", "Spine", V(0,1.34f,0)); Bone("Neck", "Chest", V(0,1.49f,0)); Bone("Head", "Neck", V(0,1.61f,0));
            foreach (var s in new[] {-1,1})
            {
                var p = s < 0 ? "Left" : "Right";
                Bone(p+"Shoulder", "Chest", V(s*.19f,1.42f,0)); Bone(p+"UpperArm", p+"Shoulder", V(s*.255f,1.42f,0));
                Bone(p+"Forearm", p+"UpperArm", V(s*.50f,1.42f,0)); Bone(p+"Hand", p+"Forearm", V(s*.735f,1.42f,0));
                Bone(p+"Fingers", p+"Hand", V(s*.81f,1.42f,.004f));
                Bone(p+"UpperLeg", "Hips", V(s*.105f,.90f,0)); Bone(p+"LowerLeg", p+"UpperLeg", V(s*.105f,.51f,.005f));
                Bone(p+"Foot", p+"LowerLeg", V(s*.105f,.12f,0)); Bone(p+"Toes", p+"Foot", V(s*.105f,.06f,.13f));
            }
        }
        public Transform Get(string name) => Bones[Indices[name]];
        private void Bone(string name, string parent, Vector3 point)
        {
            var t = new GameObject(name).transform; t.SetParent(parent == null ? ModelRoot : Get(parent), false); t.position = ModelRoot.TransformPoint(point);
            Indices[name] = Bones.Count; Bones.Add(t);
        }
        public string Path(string name) => AnimationUtility.CalculateTransformPath(Get(name), ModelRoot);
        private static Vector3 V(float x,float y,float z) => new Vector3(x,y,z);
        private void EllipsoidField(Vector3 center, Vector3 radius)
        {
            shapes.Add(p => { var q = p-center; var k = new Vector3(q.x/radius.x,q.y/radius.y,q.z/radius.z).magnitude; return (k-1)*Mathf.Min(radius.x,Mathf.Min(radius.y,radius.z)); });
        }
        private void CapsuleField(Vector3 a, Vector3 b, float r)
        { shapes.Add(p => { var d=b-a;return (p-a-d*Mathf.Clamp01(Vector3.Dot(p-a,d)/d.sqrMagnitude)).magnitude-r; }); }
        private float Field(Vector3 p)
        {
            var d=100f;
            foreach(var f in shapes) { var b=f(p);var h=Mathf.Clamp01(.5f+.5f*(b-d)/.035f);d=Mathf.Lerp(b,d,h)-.035f*h*(1-h); }
            return d;
        }
        private Vector3 Gradient(Vector3 p)
        {
            const float e=.001f;
            return V(Field(p+V(e,0,0))-Field(p-V(e,0,0)),Field(p+V(0,e,0))-Field(p-V(0,e,0)),Field(p+V(0,0,e))-Field(p-V(0,0,e))).normalized;
        }
        public void Body()
        {
            EllipsoidField(V(0,1.22f,0),V(.195f*bulk,.27f,.123f)); EllipsoidField(V(0,.96f,0),V(.17f*bulk,.15f,.115f));
            EllipsoidField(V(0,1.385f,0),V(.235f*bulk,.10f,.115f)); CapsuleField(V(0,1.43f,0),V(0,1.59f,0),.061f);
            EllipsoidField(V(0,1.672f,0),V(.087f,.118f,.095f)); EllipsoidField(V(0,1.613f,.027f),V(.073f,.064f,.069f));
            EllipsoidField(V(0,1.659f,.095f),V(.018f,.027f,.025f)); EllipsoidField(V(0,1.619f,.080f),V(.043f,.017f,.015f));
            foreach(var s in new[]{-1,1})
            {
                EllipsoidField(V(s*.086f,1.665f,0),V(.014f,.028f,.022f));
                CapsuleField(V(s*.215f,1.42f,0),V(s*.49f,1.42f,0),.064f*bulk); EllipsoidField(V(s*.34f,1.42f,-.006f),V(.12f,.073f*bulk,.070f));
                CapsuleField(V(s*.49f,1.42f,0),V(s*.73f,1.42f,0),.045f*bulk); EllipsoidField(V(s*.60f,1.42f,.006f),V(.09f,.055f,.050f));
                EllipsoidField(V(s*.778f,1.42f,0),V(.061f,.034f,.046f));
                for(var finger=0;finger<4;finger++) CapsuleField(V(s*.81f,1.42f,-.029f+finger*.019f),V(s*(.872f-Mathf.Abs(finger-1.5f)*.008f),1.418f,-.029f+finger*.019f),.009f);
                CapsuleField(V(s*.75f,1.405f,.037f),V(s*.80f,1.384f,.062f),.016f);
                CapsuleField(V(s*.104f,.91f,0),V(s*.105f,.51f,0),.077f*bulk); EllipsoidField(V(s*.105f,.73f,.002f),V(.095f*bulk,.19f,.089f));
                CapsuleField(V(s*.105f,.51f,0),V(s*.105f,.12f,0),.049f); EllipsoidField(V(s*.105f,.34f,-.017f),V(.060f,.16f,.068f));
                EllipsoidField(V(s*.105f,.065f,.065f),V(.059f,.064f,.126f));
                EllipsoidField(V(s*.085f,1.30f,.105f),V(.093f*bulk,.065f,.033f));
            }
            // Marching tetrahedra: vértices compartidos y normales de gradiente, sin costuras de cilindros.
            const float step=.021f; const int nx=89,ny=88,nz=25;
            var grid=new float[nx*ny*nz];var origin=V(-.924f,-.021f,-.252f);
            int Id(int x,int y,int z)=>(x*ny+y)*nz+z;
            Vector3 Point(int x,int y,int z)=>origin+V(x,y,z)*step;
            for(var x=0;x<nx;x++)for(var y=0;y<ny;y++)for(var z=0;z<nz;z++)grid[Id(x,y,z)]=Field(Point(x,y,z));
            var corners=new[]{new Vector3Int(0,0,0),new Vector3Int(1,0,0),new Vector3Int(1,1,0),new Vector3Int(0,1,0),new Vector3Int(0,0,1),new Vector3Int(1,0,1),new Vector3Int(1,1,1),new Vector3Int(0,1,1)};
            var tetra=new[,]{{0,5,1,6},{0,1,2,6},{0,2,3,6},{0,3,7,6},{0,7,4,6},{0,4,5,6}};
            var ps=new Vector3[8];var ds=new float[8];var points=new List<Vector3>(4);
            for(var x=0;x<nx-1;x++)for(var y=0;y<ny-1;y++)for(var z=0;z<nz-1;z++)
            {
                var min=100f;var max=-100f;
                for(var i=0;i<8;i++){var c=corners[i];ps[i]=Point(x+c.x,y+c.y,z+c.z);ds[i]=grid[Id(x+c.x,y+c.y,z+c.z)];min=Mathf.Min(min,ds[i]);max=Mathf.Max(max,ds[i]);}
                if(min>0||max<0)continue;
                for(var t=0;t<6;t++)
                {
                    points.Clear();for(var a=0;a<4;a++)for(var b=a+1;b<4;b++){var ia=tetra[t,a];var ib=tetra[t,b];if((ds[ia]<0)==(ds[ib]<0))continue;points.Add(Vector3.Lerp(ps[ia],ps[ib],ds[ia]/(ds[ia]-ds[ib])));}
                    if(points.Count<3)continue;
                    var center=points.Aggregate(Vector3.zero,(a,b)=>a+b)/points.Count;var n=Gradient(center);var axis=(points[0]-center).normalized;var other=Vector3.Cross(n,axis);
                    points.Sort((a,b)=>Mathf.Atan2(Vector3.Dot(a-center,other),Vector3.Dot(a-center,axis)).CompareTo(Mathf.Atan2(Vector3.Dot(b-center,other),Vector3.Dot(b-center,axis))));
                    var mat=BodyMaterial(center);for(var j=1;j<points.Count-1;j++)SurfaceFace(Vertex(points[0],Gradient(points[0]),true),Vertex(points[j],Gradient(points[j]),true),Vertex(points[j+1],Gradient(points[j+1]),true),mat);
                }
            }
        }
        private int BodyMaterial(Vector3 p)
        {
            if(p.y>1.49f)return 0;
            if(p.y<.145f)return 2;
            if(p.y<.98f)return 3;
            if(ClassId=="monk")return 0;
            return Mathf.Abs(p.x)>.73f?2:ClassId=="warrior"?2:5;
        }
        private BoneWeight Weight(Vector3 p)
        {
            string a,b;float t;
            var side=p.x<0?"Left":"Right";var x=Mathf.Abs(p.x);
            if(x>.215f&&p.y>1.20f&&p.y<1.53f)
            {
                if(x<.30f){a="Chest";b=side+"UpperArm";t=Mathf.InverseLerp(.215f,.30f,x);}
                else if(x<.56f){a=side+"UpperArm";b=side+"Forearm";t=Mathf.InverseLerp(.43f,.56f,x);}
                else if(x<.76f){a=side+"Forearm";b=side+"Hand";t=Mathf.InverseLerp(.69f,.77f,x);}
                else{a=side+"Hand";b=side+"Fingers";t=Mathf.InverseLerp(.80f,.87f,x);}
            }
            else if(p.y<.91f)
            {
                if(p.y>.80f){a=side+"UpperLeg";b="Hips";t=Mathf.InverseLerp(.80f,.94f,p.y);}
                else if(p.y>.39f){a=side+"LowerLeg";b=side+"UpperLeg";t=Mathf.InverseLerp(.43f,.59f,p.y);}
                else if(p.y>.08f){a=side+"Foot";b=side+"LowerLeg";t=Mathf.InverseLerp(.10f,.23f,p.y);}
                else{a=side+"Foot";b=side+"Toes";t=Mathf.InverseLerp(.08f,.17f,p.z);}
            }
            else if(p.y<1.15f){a="Hips";b="Spine";t=Mathf.InverseLerp(1,1.16f,p.y);}
            else if(p.y<1.44f){a="Spine";b="Chest";t=Mathf.InverseLerp(1.17f,1.32f,p.y);}
            else if(p.y<1.58f){a="Chest";b="Neck";t=Mathf.InverseLerp(1.44f,1.51f,p.y);}
            else{a="Neck";b="Head";t=Mathf.InverseLerp(1.54f,1.60f,p.y);}
            return new BoneWeight{boneIndex0=Indices[a],weight0=1-t,boneIndex1=Indices[b],weight1=t};
        }
        private int Vertex(Vector3 p,Vector3 normal,bool weld=false)
        {
            var key=Vector3Int.RoundToInt(p*1000000);
            if(weld&&welded.TryGetValue(key,out var found))return found;
            var i=vertices.Count;vertices.Add(p);normals.Add(normal);uv.Add(new Vector2(p.x*2,p.y*2));weights.Add(Weight(p));if(weld)welded[key]=i;return i;
        }
        private void Face(int a,int b,int c,int material){triangles[material].Add(a);triangles[material].Add(b);triangles[material].Add(c);}
        private void SurfaceFace(int a,int b,int c,int material)
        {
            if(Vector3.Dot(Vector3.Cross(vertices[b]-vertices[a],vertices[c]-vertices[a]),normals[a]+normals[b]+normals[c])<0)Face(a,c,b,material);
            else Face(a,b,c,material);
        }
        public void Oval(int mat,Vector3 center,Vector3 radius,int rings=12,int sides=20)
        {
            var start=vertices.Count;
            for(var y=0;y<=rings;y++)for(var x=0;x<=sides;x++)
            {var a=x*Mathf.PI*2/sides;var b=y*Mathf.PI/rings;var n=V(Mathf.Sin(b)*Mathf.Cos(a),Mathf.Cos(b),Mathf.Sin(b)*Mathf.Sin(a));Vertex(center+Vector3.Scale(n,radius),new Vector3(n.x/radius.x,n.y/radius.y,n.z/radius.z).normalized);}
            for(var y=0;y<rings;y++)for(var x=0;x<sides;x++){var i=start+y*(sides+1)+x;SurfaceFace(i,i+1,i+sides+1,mat);SurfaceFace(i+1,i+sides+2,i+sides+1,mat);}
        }
        public void Tube(int mat,Vector3 a,Vector3 b,float ra,float rb,int rings=10,int sides=24,float flatten=1)
        {
            var axis=(b-a).normalized;var u=Vector3.Cross(axis,Mathf.Abs(axis.y)>.9f?Vector3.forward:Vector3.up).normalized;var v=Vector3.Cross(axis,u);var start=vertices.Count;
            for(var r=0;r<=rings;r++)for(var s=0;s<=sides;s++)
            {var t=(float)r/rings;var angle=s*Mathf.PI*2/sides;var n=u*Mathf.Cos(angle)+v*Mathf.Sin(angle);var fold=1+.025f*Mathf.Cos(angle*8+t*3);Vertex(Vector3.Lerp(a,b,t)+(u*Mathf.Cos(angle)+v*Mathf.Sin(angle)*flatten)*Mathf.Lerp(ra,rb,t)*fold,n);}
            for(var r=0;r<rings;r++)for(var s=0;s<sides;s++){var i=start+r*(sides+1)+s;SurfaceFace(i,i+1,i+sides+1,mat);SurfaceFace(i+1,i+sides+2,i+sides+1,mat);}
        }
        public void Ribbon(int mat,Vector3 a,Vector3 b,float width)
        {
            var side=Vector3.Cross((b-a).normalized,Vector3.forward)*width*.5f;var start=vertices.Count;const int across=8;
            var normal=a.z+b.z<0?Vector3.back:Vector3.forward;
            for(var i=0;i<=12;i++)for(var j=0;j<=across;j++)
            {var p=Vector3.Lerp(a,b,i/12f)+side*(2f*j/across-1);Vertex(Conform(p),normal);}
            for(var i=0;i<12;i++)for(var j=0;j<across;j++)
            {var n=start+i*(across+1)+j;Face(n,n+1,n+across+1,mat);Face(n+1,n+across+2,n+across+1,mat);Face(n+across+1,n+1,n,mat);Face(n+across+1,n+across+2,n+1,mat);}
        }
        private Vector3 Conform(Vector3 p)
        {
            if(p.y<.95f||Field(V(p.x,p.y,0))>=0)return p;
            var lo=0f;var hi=.35f;var sign=p.z<0?-1:1;
            for(var i=0;i<12;i++){var mid=(lo+hi)*.5f;if(Field(V(p.x,p.y,sign*mid))<0)lo=mid;else hi=mid;}
            p.z=sign*Mathf.Max(Mathf.Abs(p.z),hi+.008f);return p;
        }
        public void Clothing()
        {
            var monk=ClassId=="monk";var warrior=ClassId=="warrior";
            foreach(var s in new[]{-1,1})
            {
                Tube(3,V(s*.105f,.19f,0),V(s*.105f,.91f,0),.063f,monk?.127f:.090f,18,24,.95f);
                Oval(2,V(s*.105f,.066f,.065f),V(.066f,.065f,.132f));Tube(2,V(s*.105f,.08f,0),V(s*.105f,monk?.19f:.36f,0),.065f,.07f);
                if(monk)
                {
                    for(var i=0;i<8;i++)Tube(4,V(s*.105f,.17f+i*.022f,0),V(s*.105f,.191f+i*.022f,0),.076f+i*.0004f,.077f+i*.0004f,1,20);
                    for(var i=0;i<9;i++)Tube(4,V(s*(.57f+i*.018f),1.42f,0),V(s*(.588f+i*.018f),1.42f,0),.075f-i*.0025f,.075f-i*.0025f,1,20);
                }
                else if(warrior)
                {
                    Oval(6,V(s*.255f,1.435f,0),V(.104f,.089f,.12f));Tube(6,V(s*.56f,1.42f,0),V(s*.70f,1.42f,0),.066f,.048f,8,20);
                    Oval(6,V(s*.105f,.51f,.059f),V(.073f,.083f,.040f));
                    for(var layer=0;layer<3;layer++)Ribbon(2,V(s*.17f,.99f,.13f-layer*.045f),V(s*.20f,.67f,.16f-layer*.045f),.11f);
                }
                else
                {
                    Tube(5,V(s*.105f,.22f,0),V(s*.105f,.99f,0),.145f,.139f,24,28,.95f);
                    Tube(7,V(s*.27f,1.42f,0),V(s*.60f,1.42f,0),.092f,.079f,14,24);
                    Tube(4,V(s*.63f,1.42f,0),V(s*.69f,1.42f,0),.066f,.058f,3,24);
                }
                Oval(8,V(s*.035f,1.689f,.086f),V(.018f,.005f,.008f),6,12);Oval(8,V(s*.034f,1.667f,.090f),V(.010f,.004f,.007f),6,12);
            }
            if(monk)
            {
                Ribbon(1,V(-.21f,1.47f,.105f),V(.12f,1.015f,.143f),.19f);Ribbon(1,V(-.21f,1.47f,-.105f),V(.12f,1.015f,-.143f),.19f);
                Oval(1,V(-.19f,1.492f,0),V(.105f,.035f,.13f));
                foreach(var s in new[]{-1,1})Ribbon(1,V(s*.095f,.98f,.14f),V(s*.14f,.59f,.17f),.15f);
                Tube(4,V(0,.95f,0),V(0,1.035f,0),.215f,.215f,5,32,.82f);
                Oval(2,V(.139f,.985f,.162f),V(.018f,.030f,.012f));
            }
            else
            {
                Oval(warrior?6:5,V(0,1.237f,.003f),V(.224f*bulk,.251f,.170f));
                if(!warrior)
                {
                    Oval(7,V(0,1.409f,-.018f),V(.244f,.081f,.147f));Oval(5,V(0,1.48f,-.065f),V(.092f,.080f,.076f));
                    Oval(8,V(0,1.725f,-.013f),V(.092f,.074f,.095f));Oval(8,V(0,1.602f,.041f),V(.060f,.031f,.055f));
                    Oval(2,V(-.205f,.89f,.07f),V(.052f,.091f,.036f));Oval(4,V(-.205f,.89f,.101f),V(.046f,.079f,.009f));
                }
                else{Oval(8,V(0,1.735f,-.015f),V(.092f,.065f,.093f));Oval(8,V(0,1.605f,.044f),V(.058f,.022f,.050f));}
                Tube(2,V(0,.98f,0),V(0,1.035f,0),.220f*bulk,.218f*bulk,3,32,.85f);Oval(6,V(0,1.005f,.208f),V(.027f,.022f,.008f));
                Ribbon(2,V(-.16f,1.44f,.15f),V(.14f,1.045f,.153f),.035f);
            }
        }
        public SkinnedMeshRenderer Bake(string assetPath, string name)
        {
            var mesh=new Mesh{name=name,indexFormat=IndexFormat.UInt32};mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.boneWeights=weights.ToArray();
            mesh.bindposes=Bones.Select(b=>b.worldToLocalMatrix*ModelRoot.localToWorldMatrix).ToArray();mesh.subMeshCount=materials.Length;
            for(var i=0;i<materials.Length;i++)mesh.SetTriangles(triangles[i],i);mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,assetPath);
            var node=new GameObject(name);node.transform.SetParent(ModelRoot,false);var skin=node.AddComponent<SkinnedMeshRenderer>();skin.sharedMesh=mesh;skin.sharedMaterials=materials;skin.bones=Bones.ToArray();skin.rootBone=Get("Hips");skin.quality=SkinQuality.Bone4;
            skin.localBounds=new Bounds(V(0,.9f,0),V(2.5f,2.4f,2.5f));skin.updateWhenOffscreen=false;return skin;
        }
    }
}
