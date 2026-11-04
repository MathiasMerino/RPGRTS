using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Aethernia.Editor.Environment
{
    // Mallas de autoría estáticas. Los submeshes comparten una biblioteca de materiales.
    internal sealed class FrontierArtMesh
    {
        private readonly List<Vector3> vertices=new();
        private readonly List<Vector2> uv=new();
        private readonly Dictionary<Material,List<int>> indices=new();

        public void Face(Material material,Vector3 a,Vector3 b,Vector3 c,Vector3? d=null)
        {
            if(!indices.TryGetValue(material,out var triangles))indices.Add(material,triangles=new List<int>());
            var start=vertices.Count;
            var u=(b-a).normalized;
            var normal=Vector3.Cross(b-a,c-a).normalized;
            var v=Vector3.Cross(normal,u);
            Add(a);Add(b);Add(c);
            triangles.AddRange(new[]{start,start+1,start+2});
            if(d.HasValue){Add(d.Value);triangles.AddRange(new[]{start,start+2,start+3});}
            void Add(Vector3 point){vertices.Add(point);uv.Add(new Vector2(Vector3.Dot(point,u),Vector3.Dot(point,v))*.65f);}
        }

        public void Block(Material material,Vector3 center,Vector3 size,float bevel,Quaternion rotation)
        {
            var x=size.x*.5f;var y=size.y*.5f;var z=size.z*.5f;
            var r=Mathf.Min(bevel,x*.5f,z*.5f);
            var rim=new[]{new Vector2(-x+r,-z),new Vector2(x-r,-z),new Vector2(x,-z+r),new Vector2(x,z-r),
                new Vector2(x-r,z),new Vector2(-x+r,z),new Vector2(-x,z-r),new Vector2(-x,-z+r)};
            Vector3 Point(int i,float h)=>center+rotation*new Vector3(rim[i].x,h,rim[i].y);
            for(var i=0;i<rim.Length;i++)
            {
                var j=(i+1)%rim.Length;
                Face(material,Point(i,-y),Point(i,y),Point(j,y),Point(j,-y));
                if(i>=1&&j!=0){Face(material,Point(0,y),Point(j,y),Point(i,y));Face(material,Point(0,-y),Point(i,-y),Point(j,-y));}
            }
        }

        public void Beam(Material material,Vector3 a,Vector3 b,float width,float depth)
        {
            if((b-a).sqrMagnitude<.00001f)return;
            Block(material,(a+b)*.5f,new Vector3(width,Vector3.Distance(a,b),depth),width*.15f,Quaternion.FromToRotation(Vector3.up,b-a));
        }

        public void Arch(Material material,Vector3 center,float radius,float thickness,float depth,int blocks=9)
        {
            for(var i=0;i<blocks;i++)
            {
                var first=(i+.045f)*Mathf.PI/blocks;var last=(i+.955f)*Mathf.PI/blocks;
                Vector3 At(float a,float r,float z)=>center+new Vector3(Mathf.Cos(a)*r,Mathf.Sin(a)*r,z);
                var a=At(first,radius,-depth/2);var b=At(first,radius+thickness,-depth/2);
                var c=At(last,radius+thickness,-depth/2);var d=At(last,radius,-depth/2);
                var back=Vector3.forward*depth;
                Face(material,d,c,b,a);Face(material,a+back,b+back,c+back,d+back);
                Face(material,a,b,b+back,a+back);Face(material,b,c,c+back,b+back);
                Face(material,c,d,d+back,c+back);Face(material,d,a,a+back,d+back);
            }
        }

        public void Leaf(Material material,Vector3 a,Vector3 b,float width,float lift)
        {
            var direction=(b-a).normalized;
            var across=Vector3.Cross(direction,Vector3.up);
            if(across.sqrMagnitude<.001f)across=Vector3.right;
            var mid=Vector3.Lerp(a,b,.42f);var ridge=mid+Vector3.up*lift;
            var l=mid-across.normalized*width;var r=mid+across.normalized*width;
            Face(material,a,ridge,l);Face(material,ridge,b,l);
            Face(material,a,r,ridge);Face(material,r,b,ridge);
        }

        public void Save(Transform root,string name,string path,bool shadows=true)
        {
            var mesh=Create(name);
            AssetDatabase.CreateAsset(mesh,path);
            root.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=root.gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterials=indices.Keys.ToArray();
            if(!shadows)renderer.shadowCastingMode=ShadowCastingMode.Off;
            GameObjectUtility.SetStaticEditorFlags(root.gameObject,StaticEditorFlags.BatchingStatic);
        }

        public void UpdateAsset(string path)
        {
            var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            var updated=Create(existing.name);
            EditorUtility.CopySerialized(updated,existing);
            Object.DestroyImmediate(updated);
            EditorUtility.SetDirty(existing);
        }

        private Mesh Create(string name)
        {
            var mesh=new Mesh{name=name,indexFormat=vertices.Count>65535?IndexFormat.UInt32:IndexFormat.UInt16};
            mesh.SetVertices(vertices);mesh.SetUVs(0,uv);
            mesh.subMeshCount=indices.Count;
            var slot=0;foreach(var list in indices.Values)mesh.SetTriangles(list,slot++);
            mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();
            return mesh;
        }
    }
}
