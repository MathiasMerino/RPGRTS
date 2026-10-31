using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aethernia.Editor.Characters
{
    // Small authoring mesh library. Bakes directly into material groups; no runtime primitive hierarchy.
    internal sealed class MonkProxyGeometry
    {
        private sealed class Surface
        {
            public readonly List<Vector3> Vertices=new List<Vector3>(), Normals=new List<Vector3>();
            public readonly List<Vector2> Uv=new List<Vector2>();
            public readonly List<int> Indices=new List<int>();
        }
        private readonly Dictionary<Material,Surface> groups=new Dictionary<Material,Surface>();
        private Surface For(Material material)
        { if(!groups.TryGetValue(material,out var surface)) groups[material]=surface=new Surface(); return surface; }
        public void Ellipsoid(Material material,Vector3 centre,Vector3 size)
        {
            const int rings=8,sides=12; var s=For(material); var start=s.Vertices.Count; var half=size*.5f;
            for(var r=0;r<=rings;r++) for(var i=0;i<=sides;i++)
            {
                var phi=i*Mathf.PI*2/sides; var theta=r*Mathf.PI/rings;
                var n=new Vector3(Mathf.Sin(theta)*Mathf.Cos(phi),Mathf.Cos(theta),Mathf.Sin(theta)*Mathf.Sin(phi));
                s.Vertices.Add(centre+Vector3.Scale(n,half));
                s.Normals.Add(new Vector3(n.x/half.x,n.y/half.y,n.z/half.z).normalized); s.Uv.Add(new Vector2((float)i/sides,(float)r/rings));
                if(r<rings&&i<sides)
                { var k=start+r*(sides+1)+i; s.Indices.AddRange(new[]{k,k+1,k+sides+1,k+1,k+sides+2,k+sides+1}); }
            }
        }
        public void Tube(Material material,float x,float z,float[] heights,float[] radii)
        {
            const int sides=12; var s=For(material); var start=s.Vertices.Count;
            for(var r=0;r<heights.Length;r++) for(var i=0;i<=sides;i++)
            {
                var angle=i*Mathf.PI*2/sides; var normal=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
                s.Vertices.Add(new Vector3(x,heights[r],z)+normal*radii[r]); s.Normals.Add(normal); s.Uv.Add(new Vector2((float)i/sides,heights[r]*2));
                if(r>0&&i<sides) { var k=start+r*(sides+1)+i; s.Indices.AddRange(new[]{k,k+1,k-sides-1,k+1,k-sides,k-sides-1}); }
            }
            for(var cap=0;cap<2;cap++)
            {
                var row=cap==0?0:heights.Length-1; var sign=cap==0?-1:1; var k=s.Vertices.Count;
                s.Vertices.Add(new Vector3(x,heights[row],z));s.Normals.Add(Vector3.up*sign);s.Uv.Add(Vector2.one*.5f);
                for(var i=0;i<=sides;i++)
                {
                    var angle=i*Mathf.PI*2/sides;
                    s.Vertices.Add(new Vector3(x+Mathf.Cos(angle)*radii[row],heights[row],z+Mathf.Sin(angle)*radii[row]));
                    s.Normals.Add(Vector3.up*sign);s.Uv.Add(new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*.5f+Vector2.one*.5f);
                    if(i<sides) s.Indices.AddRange(cap==0?new[]{k,k+i+1,k+i+2}:new[]{k,k+i+2,k+i+1});
                }
            }
        }
        public void Quad(Material material,Vector3 a,Vector3 b,Vector3 c,Vector3 d,bool doubleSided=false)
        {
            var s=For(material);var k=s.Vertices.Count;var n=Vector3.Cross(b-a,c-a).normalized;
            s.Vertices.AddRange(new[]{a,b,c,d});s.Normals.AddRange(new[]{n,n,n,n});
            s.Uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up});s.Indices.AddRange(new[]{k,k+1,k+2,k,k+2,k+3});
            if(doubleSided) Quad(material,d,c,b,a);
        }
        public void Bake(Transform parent,string folder)
        {
            foreach(var group in groups)
            {
                var data=group.Value;var mesh=new Mesh {name="MonkProxy_"+group.Key.name};
                mesh.SetVertices(data.Vertices);mesh.SetNormals(data.Normals);mesh.SetUVs(0,data.Uv);mesh.SetTriangles(data.Indices,0);mesh.RecalculateBounds();mesh.RecalculateTangents();
                AssetDatabase.CreateAsset(mesh,folder+"/"+mesh.name+".asset");
                var go=new GameObject(group.Key.name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);
                go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=group.Key;
            }
        }
        public void Box(Material material,Vector3 centre,Vector3 size)
        {
            var h=size*.5f;var a=centre+new Vector3(-h.x,-h.y,-h.z);var b=centre+new Vector3(h.x,-h.y,-h.z);
            var c=centre+new Vector3(h.x,h.y,-h.z);var d=centre+new Vector3(-h.x,h.y,-h.z);var z=Vector3.forward*size.z;
            Quad(material,a,d,c,b);Quad(material,a+z,b+z,c+z,d+z);Quad(material,a,a+z,d+z,d);Quad(material,b,c,c+z,b+z);Quad(material,d,d+z,c+z,c);Quad(material,a,b,b+z,a+z);
        }
    }
}
