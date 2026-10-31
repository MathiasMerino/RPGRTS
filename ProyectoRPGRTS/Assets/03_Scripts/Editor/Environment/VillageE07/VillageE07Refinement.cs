using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using V=Aethernia.Editor.Environment.VillageE07Geometry;
using M=Aethernia.Editor.Environment.VillageE07Materials;

namespace Aethernia.Editor.Environment
{
    public static class VillageE07Refinement
    {
        public static void BindTreeVisibility()
        {
            var scene=EditorSceneManager.OpenScene(VillageE07SceneBuilder.ScenePath);var root=GameObject.Find(VillageE07SceneBuilder.RootName);
            var physics=EnvironmentPhysicsSnapshot.Capture(scene);var actor=UnityEngine.Object.FindAnyObjectByType<PrototypeDirector>().Hero.transform;
            foreach(var c in root.transform.Find("Temperate woodland and ground cover").GetComponentsInChildren<CapsuleCollider>())
                c.GetComponent<RoofCutaway>().Configure(Camera.main,actor,c.GetComponentsInChildren<Renderer>());
            foreach(var canvas in root.GetComponentsInChildren<Canvas>())if(canvas.name=="Wayfinding text")
                canvas.transform.localPosition=new Vector3(0,1.55f,-.09f)-Camera.main.transform.forward*.8f;
            if(root.transform.Find("E07 measured refinements/Readability revision")==null)PolishGrassEdges(root.transform);
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics)throw new InvalidOperationException("Tree visibility changed physics.");
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);Debug.Log("E07_VISIBILITY: tree trunks and crowns use the same reversible actor visibility; physics intact.");
        }
        private static void PolishGrassEdges(Transform root)
        {
            var parent=root.Find("E07 measured refinements");var source=MedievalPrefabs.Load("Vegetation","GrassClump").GetComponentInChildren<MeshFilter>().sharedMesh;
            var groups=new Dictionary<Vector2Int,List<CombineInstance>>();var random=new System.Random(7049);
            foreach(var filter in root.Find("Terrain and roads").GetComponentsInChildren<MeshFilter>())
            {
                if(!filter.sharedMesh.name.Contains("Path_"))continue;var vertices=filter.sharedMesh.vertices;
                for(var i=0;i+1<vertices.Length;i+=4)for(var side=0;side<2;side++)
                {
                    if(random.NextDouble()<.62)continue;
                    var edge=filter.transform.TransformPoint(vertices[i+side]);var across=(vertices[i+1]-vertices[i]).normalized;var along=Vector3.Cross(across,Vector3.up);
                    for(var patch=0;patch<1+random.Next(3);patch++)
                    {
                        var at=edge+across*((float)random.NextDouble()*.6f-.3f)+along*((float)random.NextDouble()*.9f-.45f);at.y=0;
                        if((at.x+3)*(at.x+3)/25+(at.z+3)*(at.z+3)/16<1.18f||Physics.CheckSphere(at+Vector3.up*.25f,.21f,~0,QueryTriggerInteraction.Ignore))continue;
                        var key=new Vector2Int(Mathf.FloorToInt(at.x/8),Mathf.FloorToInt(at.z/8));if(!groups.TryGetValue(key,out var list))groups[key]=list=new List<CombineInstance>();
                        list.Add(new CombineInstance{mesh=source,transform=Matrix4x4.TRS(at,Quaternion.Euler(0,(float)random.NextDouble()*360,0),Vector3.one*(.45f+(float)random.NextDouble()*.6f))});
                    }
                }
            }
            foreach(Transform child in parent)if(child.name.StartsWith("Grass edge cell ",StringComparison.Ordinal))foreach(var r in child.GetComponentsInChildren<Renderer>())r.enabled=false;
            foreach(var pair in groups)
            {
                var node=parent.Find("Grass edge cell "+pair.Key.x+"_"+pair.Key.y);MeshFilter filter;
                if(node!=null)filter=node.GetComponentInChildren<MeshFilter>();
                else{node=V.Group(parent,"Grass edge cell "+pair.Key.x+"_"+pair.Key.y);filter=new GameObject("LeafLight",typeof(MeshFilter),typeof(MeshRenderer)).GetComponent<MeshFilter>();filter.transform.SetParent(node,false);}
                var mesh=new Mesh{name="E07_EdgePolish_"+pair.Key.x+"_"+pair.Key.y};mesh.CombineMeshes(pair.Value.ToArray());
                if(filter.sharedMesh!=null){EditorUtility.CopySerialized(mesh,filter.sharedMesh);EditorUtility.SetDirty(filter.sharedMesh);UnityEngine.Object.DestroyImmediate(mesh);}
                else{AssetDatabase.CreateAsset(mesh,M.Root+"/Meshes/"+mesh.name+".asset");filter.sharedMesh=mesh;}
                var renderer=filter.GetComponent<MeshRenderer>();renderer.enabled=true;renderer.sharedMaterial=M.Load("LeafLight");renderer.shadowCastingMode=ShadowCastingMode.Off;
            }
            V.Group(parent,"Readability revision");
        }
        // Targeted edit of the authored scene, not a repeat of any existing generator.
        public static void Apply()
        {
            var scene=EditorSceneManager.OpenScene(VillageE07SceneBuilder.ScenePath);var root=GameObject.Find(VillageE07SceneBuilder.RootName).transform;
            if(root.Find("E07 measured refinements")!=null)throw new InvalidOperationException("E07 refinements already applied.");
            var physics=EnvironmentPhysicsSnapshot.Capture(scene);var group=V.Group(root,"E07 measured refinements");
            ExtendBackdrop(group);DressRoadEdges(root,group);BatchSoftPlants(root,group);
            foreach(var r in root.Find("Terrain and roads").GetComponentsInChildren<Renderer>()){r.shadowCastingMode=ShadowCastingMode.Off;PrefabUtility.RecordPrefabInstancePropertyModifications(r);}
            foreach(var c in root.Find("Temperate woodland and ground cover").GetComponentsInChildren<CapsuleCollider>())
                if(Mathf.Abs(c.transform.position.x)>20||c.transform.position.z>20)foreach(var r in c.GetComponentsInChildren<Renderer>()){r.shadowCastingMode=ShadowCastingMode.Off;PrefabUtility.RecordPrefabInstancePropertyModifications(r);}
            RenderSettings.ambientSkyColor=new Color(.47f,.55f,.67f);RenderSettings.ambientEquatorColor=new Color(.40f,.44f,.47f);
            if(EnvironmentPhysicsSnapshot.Capture(scene)!=physics)throw new InvalidOperationException("Visual refinement changed physics.");
            EditorSceneManager.MarkSceneDirty(scene);UnityEditor.AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene);
            Debug.Log("E07_REFINED: measured foliage chunks, grass-frayed road edges, visual backdrop extension, fewer irrelevant shadows, readable cool fill; no physics/gameplay changes.");
        }
        private static void ExtendBackdrop(Transform p)
        {
            var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
            // Extra framing surface leaves x>=44 untouched, preserving the dungeon at x=45..55.
            for(var z=-60;z<60;z+=2)for(var x=-60;x<44;x+=2)
            {
                if(x>=-40&&x<40&&z>=-40&&z<40)continue;
                var a=new Vector3(x,VillageE07Nature.BackdropHeight(x,z),z);var b=new Vector3(x,VillageE07Nature.BackdropHeight(x,z+2),z+2);var c=new Vector3(x+2,VillageE07Nature.BackdropHeight(x+2,z+2),z+2);var d=new Vector3(x+2,VillageE07Nature.BackdropHeight(x+2,z),z);
                var n=v.Count;v.AddRange(new[]{a,b,c,d});foreach(var q in new[]{a,b,c,d})uv.Add(new Vector2(q.x/2,q.z/2));t.AddRange(new[]{n,n+1,n+2,n,n+2,n+3});
            }
            var node=V.Group(p,"Distant framing ground");V.Part(node,"Outer backdrop",V.Mesh("OuterBackdrop",v,t,uv),"Grass",Vector3.zero);V.Merge(node,"OuterBackdrop");node.GetComponentInChildren<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
        }
        private static void DressRoadEdges(Transform root,Transform p)
        {
            var cells=new Dictionary<Vector2Int,Transform>();var source=MedievalPrefabs.Load("Vegetation","GrassClump").GetComponentInChildren<MeshFilter>().sharedMesh;var random=new System.Random(7041);
            foreach(var f in root.Find("Terrain and roads").GetComponentsInChildren<MeshFilter>())
            {
                if(!f.sharedMesh.name.Contains("Path_"))continue;var verts=f.sharedMesh.vertices;
                for(var i=0;i+1<verts.Length;i+=4)for(var side=0;side<2;side++)
                {
                    var at=f.transform.TransformPoint(verts[i+side]);var plaza=(at.x+3)*(at.x+3)/25+(at.z+3)*(at.z+3)/16;
                    if(plaza<1.18f||Physics.CheckSphere(at+Vector3.up*.25f,.21f,~0,QueryTriggerInteraction.Ignore))continue;
                    at.y=0;var key=new Vector2Int(Mathf.FloorToInt(at.x/8),Mathf.FloorToInt(at.z/8));
                    if(!cells.TryGetValue(key,out var cell))cells[key]=cell=V.Group(p,"Grass edge cell "+key.x+"_"+key.y);
                    var g=V.Part(cell,"Edge grass",source,"LeafLight",at);g.transform.localRotation=Quaternion.Euler(0,(float)random.NextDouble()*360,0);g.transform.localScale=Vector3.one*(.5f+(float)random.NextDouble()*.5f);
                }
            }
            foreach(var pair in cells){V.Merge(pair.Value,"RoadEdge_"+pair.Key.x+"_"+pair.Key.y);foreach(var r in pair.Value.GetComponentsInChildren<Renderer>())r.shadowCastingMode=ShadowCastingMode.Off;}
        }
        private static void BatchSoftPlants(Transform root,Transform p)
        {
            var groups=new Dictionary<string,List<CombineInstance>>();var materials=new Dictionary<string,Material>();var forest=root.Find("Temperate woodland and ground cover");
            foreach(var f in forest.GetComponentsInChildren<MeshFilter>())
            {
                if(f.GetComponentInParent<Collider>()!=null)continue;var r=f.GetComponent<MeshRenderer>();if(r==null||!r.enabled)continue;
                var position=f.transform.position;var key=Mathf.FloorToInt(position.x/8)+"_"+Mathf.FloorToInt(position.z/8)+"_"+r.sharedMaterial.name;
                if(!groups.TryGetValue(key,out var list))groups[key]=list=new List<CombineInstance>();materials[key]=r.sharedMaterial;
                list.Add(new CombineInstance{mesh=f.sharedMesh,transform=p.worldToLocalMatrix*f.transform.localToWorldMatrix});
                // Retain source instances for editing/recovery; the compiled chunks alone render.
                r.enabled=false;PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            foreach(var pair in groups)
            {
                var mesh=new Mesh{name="E07_SoftCover_"+pair.Key,indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(pair.Value.ToArray());AssetDatabase.CreateAsset(mesh,M.Root+"/Meshes/"+mesh.name+".asset");
                var g=new GameObject("Ground cover "+pair.Key,typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(p,false);g.GetComponent<MeshFilter>().sharedMesh=mesh;var renderer=g.GetComponent<MeshRenderer>();renderer.sharedMaterial=materials[pair.Key];renderer.shadowCastingMode=ShadowCastingMode.Off;
            }
        }
    }
}
