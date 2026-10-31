using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using G=Aethernia.Editor.Environment.EnvironmentGeometry;
using M=Aethernia.Editor.Environment.VillageE07Materials;
using Object=UnityEngine.Object;

namespace Aethernia.Editor.Environment
{
    // Authoring helpers reuse D01 geometry; only retained, merged E07 meshes are persisted in E07.
    public static class VillageE07Geometry
    {
        public static Transform Group(Transform parent,string name)
        {var t=new GameObject(name).transform;t.SetParent(parent,false);return t;}
        public static GameObject Box(Transform p,string name,Vector3 pos,Vector3 size,string mat)
        {var g=G.Box(p,name,pos,size,"Oak");g.GetComponent<Renderer>().sharedMaterial=M.Load(mat);return g;}
        public static GameObject Part(Transform p,string name,Mesh mesh,string mat,Vector3 at)
        {var g=G.Part(p,name,mesh,"Oak",at);g.GetComponent<Renderer>().sharedMaterial=M.Load(mat);return g;}
        public static void Beam(Transform p,Vector3 a,Vector3 b,float width,string mat)
        {var g=Box(p,"Timber brace",(a+b)*.5f,new Vector3(width,Vector3.Distance(a,b),width),mat);g.transform.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);}
        public static void Pole(Transform p,Vector3 a,Vector3 b,float r0,float r1,string mat)
        {
            var length=Vector3.Distance(a,b);var mesh=G.Lathe("E07Pole_"+length.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"_"+r0.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"_"+r1.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),new[]{0f,length},new[]{r0,r1});
            var node=Part(p,"Tapered timber",mesh,mat,a);node.transform.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);
        }
        public static Mesh Mesh(string name,List<Vector3> vertices,List<int> triangles,List<Vector2> uv)
        {var mesh=new Mesh{name="E07_"+name};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();return mesh;}
        public static void Merge(Transform root,string name)
        {if(root.GetComponentsInChildren<MeshFilter>().Length>0)G.Consolidate(root.gameObject,"E07_"+name,M.Root);}
        public static GameObject Save(string folder,string name,Action<Transform> build,bool merge=true)
        {
            EnvironmentMaterials.Folder(M.Prefabs+"/"+folder);var path=M.Prefabs+"/"+folder+"/"+name+".prefab";
            if(AssetDatabase.LoadAssetAtPath<GameObject>(path)!=null)throw new InvalidOperationException("Existing E07 prefab: "+path);
            var node=new GameObject(name);try{build(node.transform);if(merge)Merge(node.transform,name);return PrefabUtility.SaveAsPrefabAsset(node,path);}finally{Object.DestroyImmediate(node);}
        }
        public static GameObject Load(string folder,string name)=>AssetDatabase.LoadAssetAtPath<GameObject>(M.Prefabs+"/"+folder+"/"+name+".prefab");
        public static GameObject Place(GameObject prefab,Transform parent,Vector3 at,float yaw=0)
        {var g=(GameObject)PrefabUtility.InstantiatePrefab(prefab,parent);g.transform.localPosition=at;g.transform.localRotation=Quaternion.Euler(0,yaw,0);return g;}
    }
}
