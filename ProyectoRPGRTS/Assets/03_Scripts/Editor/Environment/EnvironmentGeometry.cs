using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace Aethernia.Editor.Environment
{
    public static class EnvironmentGeometry
    {
        private static readonly Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();
        public static Mesh Save(string name, Vector3[] vertices, int[] triangles, Vector2[] uv)
        {
            if (meshes.TryGetValue(name, out var cached)) return cached;
            var path = EnvironmentMaterials.Root + "/Meshes/" + name + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null) { meshes[name] = existing; return existing; }
            var mesh = new Mesh { name = name, vertices = vertices, triangles = triangles, uv = uv };
            mesh.RecalculateNormals(); mesh.RecalculateTangents(); mesh.RecalculateBounds();
            meshes[name] = mesh; return mesh;
        }
        public static void Persist(GameObject root)
        {
            // Only meshes actually retained by a prefab/scene become assets, not intermediate boxes.
            foreach(var filter in root.GetComponentsInChildren<MeshFilter>(true))
                if(!AssetDatabase.Contains(filter.sharedMesh))
                    AssetDatabase.CreateAsset(filter.sharedMesh,EnvironmentMaterials.Root+"/Meshes/"+filter.sharedMesh.name+".asset");
        }
        public static GameObject Part(Transform parent, string name, Mesh mesh, string material, Vector3 position)
        {
            var part = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            part.transform.SetParent(parent, false); part.transform.localPosition = position;
            part.GetComponent<MeshFilter>().sharedMesh = mesh;
            part.GetComponent<MeshRenderer>().sharedMaterial = EnvironmentMaterials.Load(material); return part;
        }
        public static GameObject Box(Transform parent, string name, Vector3 centre, Vector3 size, string material)
        {
            var key = string.Format(CultureInfo.InvariantCulture, "Box_{0:F3}_{1:F3}_{2:F3}", size.x, size.y, size.z);
            if (!meshes.TryGetValue(key, out var mesh))
            {
                var h = size * .5f;
                var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var tris = new List<int>();
                Face(vertices, uv, tris, new Vector3(-h.x,-h.y,-h.z), new Vector3(-h.x,h.y,-h.z), new Vector3(h.x,h.y,-h.z), new Vector3(h.x,-h.y,-h.z));
                Face(vertices, uv, tris, new Vector3(h.x,-h.y,h.z), new Vector3(h.x,h.y,h.z), new Vector3(-h.x,h.y,h.z), new Vector3(-h.x,-h.y,h.z));
                Face(vertices, uv, tris, new Vector3(-h.x,-h.y,h.z), new Vector3(-h.x,h.y,h.z), new Vector3(-h.x,h.y,-h.z), new Vector3(-h.x,-h.y,-h.z));
                Face(vertices, uv, tris, new Vector3(h.x,-h.y,-h.z), new Vector3(h.x,h.y,-h.z), new Vector3(h.x,h.y,h.z), new Vector3(h.x,-h.y,h.z));
                Face(vertices, uv, tris, new Vector3(-h.x,h.y,-h.z), new Vector3(-h.x,h.y,h.z), new Vector3(h.x,h.y,h.z), new Vector3(h.x,h.y,-h.z));
                Face(vertices, uv, tris, new Vector3(-h.x,-h.y,h.z), new Vector3(-h.x,-h.y,-h.z), new Vector3(h.x,-h.y,-h.z), new Vector3(h.x,-h.y,h.z));
                mesh = Save(key, vertices.ToArray(), tris.ToArray(), uv.ToArray());
            }
            return Part(parent, name, mesh, material, centre);
        }
        public static void Face(List<Vector3> vertices, List<Vector2> uv, List<int> tris, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            var i = vertices.Count; vertices.AddRange(new[] { a,b,c,d });
            uv.AddRange(new[] { Vector2.zero, new Vector2(0,Vector3.Distance(a,b)/2), new Vector2(Vector3.Distance(b,c)/2,Vector3.Distance(a,b)/2), new Vector2(Vector3.Distance(b,c)/2,0) });
            tris.AddRange(new[] { i,i+1,i+2,i,i+2,i+3 });
        }
        public static Mesh Lathe(string name, float[] ys, float[] radii)
        {
            const int sides = 24;
            var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var tris = new List<int>();
            for (var row = 0; row < ys.Length; row++) for (var i = 0; i <= sides; i++)
            {
                var angle = i * Mathf.PI * 2 / sides;
                vertices.Add(new Vector3(Mathf.Cos(angle)*radii[row], ys[row], Mathf.Sin(angle)*radii[row]));
                uv.Add(new Vector2((float)i/sides*2,ys[row]));
                if (row > 0 && i < sides)
                { var k = row*(sides+1)+i; tris.AddRange(new[] { k,k+1,k-sides-1,k+1,k-sides,k-sides-1 }); }
            }
            // Separate cap vertices preserve hard rims and smooth side normals.
            for (var cap = 0; cap < 2; cap++)
            {
                var row = cap == 0 ? 0 : ys.Length-1; var start = vertices.Count;
                vertices.Add(Vector3.up*ys[row]); uv.Add(Vector2.one*.5f);
                for (var i = 0; i <= sides; i++)
                {
                    var angle = i*Mathf.PI*2/sides;
                    vertices.Add(new Vector3(Mathf.Cos(angle)*radii[row],ys[row],Mathf.Sin(angle)*radii[row]));
                    uv.Add(new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*.5f+Vector2.one*.5f);
                    if (i < sides) tris.AddRange(cap == 0 ? new[] { start,start+i+1,start+i+2 } : new[] { start,start+i+2,start+i+1 });
                }
            }
            return Save(name,vertices.ToArray(),tris.ToArray(),uv.ToArray());
        }
        public static void Consolidate(GameObject root, string name)
        {
            var groups = new Dictionary<Material,List<CombineInstance>>();
            var children = root.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in children)
            {
                var mat = filter.GetComponent<MeshRenderer>().sharedMaterial;
                if (!groups.TryGetValue(mat,out var list)) groups[mat] = list = new List<CombineInstance>();
                list.Add(new CombineInstance { mesh = filter.sharedMesh, transform = root.transform.worldToLocalMatrix * filter.transform.localToWorldMatrix });
            }
            foreach (var group in groups)
            {
                var mesh = new Mesh { name = name+"_"+group.Key.name }; mesh.CombineMeshes(group.Value.ToArray());
                AssetDatabase.CreateAsset(mesh,EnvironmentMaterials.Root+"/Meshes/"+mesh.name+".asset");
                var node = new GameObject(group.Key.name,typeof(MeshFilter),typeof(MeshRenderer)); node.transform.SetParent(root.transform,false);
                node.GetComponent<MeshFilter>().sharedMesh=mesh; node.GetComponent<MeshRenderer>().sharedMaterial=group.Key;
            }
            foreach(var filter in children) UnityEngine.Object.DestroyImmediate(filter.gameObject);
        }
    }
}
