using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Aethernia.Editor.Characters
{
    // Autoría de un graph nativo editable. Reflection aislada al editor/version17.5;
    // la build consume el shader generado y no depende de estas APIs internas.
    public static class AlphaSurfaceGraph
    {
        public const string Path="Assets/13_AlphaCandidate/Shaders/MedievalSurface.shadergraph";
        private const string SG="UnityEditor.ShaderGraph.";
        private const BindingFlags Flags=BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic;
        private static Type Type(string name){foreach(var a in AppDomain.CurrentDomain.GetAssemblies()){var t=a.GetType(name);if(t!=null)return t;}throw new InvalidOperationException("ShaderGraph API type missing: "+name);}
        private static object New(string name)=>Activator.CreateInstance(Type(name),true);
        private static object Get(object node,string name){var p=node.GetType().GetProperty(name,Flags);return p!=null?p.GetValue(node):node.GetType().GetField(name,Flags).GetValue(node);}
        private static void Set(object node,string name,object value)
        {
            var p=node.GetType().GetProperty(name,Flags);if(p!=null&&p.GetSetMethod(true)!=null){if(p.PropertyType.IsEnum&&value is string)value=Enum.Parse(p.PropertyType,(string)value);p.SetValue(node,value);return;}
            var fieldName=p==null?name:"m_"+char.ToUpperInvariant(name[0])+name.Substring(1);
            for(var type=node.GetType();type!=null;type=type.BaseType){var field=type.GetField(fieldName,Flags|BindingFlags.DeclaredOnly);if(field==null)continue;if(field.FieldType.IsEnum&&value is string)value=Enum.Parse(field.FieldType,(string)value);field.SetValue(node,value);return;}
            throw new MissingFieldException(node.GetType().FullName,fieldName);
        }
        private static object Call(object node,string name,params object[] args)
        {
            var type=node as Type??node.GetType();foreach(var method in type.GetMethods(Flags).Where(m=>m.Name==name&&!m.IsGenericMethodDefinition))
            {
                var ps=method.GetParameters();if(args.Length>ps.Length)continue;var values=new object[ps.Length];var ok=true;
                for(var n=0;n<ps.Length;n++){if(n>=args.Length){if(!ps[n].IsOptional){ok=false;break;}values[n]=ps[n].DefaultValue;}else if(args[n]==null||ps[n].ParameterType.IsInstanceOfType(args[n]))values[n]=args[n];else{ok=false;break;}}
                if(ok)return method.Invoke(node is Type?null:node,values);
            }
            throw new MissingMethodException(type.FullName,name);
        }
        private static object Construct(string name,params object[] args)
        {
            var type=Type(name);foreach(var ctor in type.GetConstructors(Flags))
            {
                var ps=ctor.GetParameters();if(ps.Length<args.Length)continue;var values=new object[ps.Length];var ok=true;
                for(var n=0;n<ps.Length;n++){if(n<args.Length){if(ps[n].ParameterType.IsEnum&&args[n] is string)values[n]=Enum.Parse(ps[n].ParameterType,(string)args[n]);else if(args[n]!=null&&!ps[n].ParameterType.IsInstanceOfType(args[n])){ok=false;break;}else values[n]=args[n];}else if(ps[n].IsOptional)values[n]=ps[n].DefaultValue;else{ok=false;break;}}
                if(ok)return ctor.Invoke(values);
            }
            throw new MissingMethodException("ShaderGraph constructor "+name);
        }
        private static object Node(object graph,string name,float x,float y){var node=New(SG+name);Call(node,"UpdateNodeAfterDeserialization");Call(graph,"AddNode",node,false);var draw=Get(node,"drawState");Set(draw,"position",new Rect(x,y,220,170));Set(node,"drawState",draw);return node;}
        private static object Property(object graph,string type,string name,object value,float y)
        {var property=New(SG+"Internal."+type);Set(property,"displayName",name);Set(property,"overrideReferenceName",name);if(value!=null)Set(property,"value",value);Call(graph,"AddGraphInput",property);var node=Node(graph,"PropertyNode",0,y);Set(node,"property",property);return node;}
        private static void Link(object graph,object from,int output,object to,int input)=>Call(graph,"Connect",Call(from,"GetSlotReference",output),Call(to,"GetSlotReference",input));
        private static object Function(object graph,string name,string guid,float y)
        {var node=Node(graph,"CustomFunctionNode",650,y);Set(node,"functionName",name);Set(node,"sourceType","File");Set(node,"functionSource",guid);return node;}
        private static void Slot(object node,int id,string name,string vector,bool output=false)
        {
            object value=vector=="1"?(object)0f:vector=="2"?(object)Vector2.zero:vector=="3"?(object)Vector3.zero:Vector4.zero;
            var slot=Construct(SG+"Vector"+vector+"MaterialSlot",id,name,name,output?"Output":"Input",value);Call(node,"AddSlot",slot);
        }
        public static Shader CreateOrLoad()
        {
            if(File.Exists(Path)){var previous=AssetDatabase.LoadAssetAtPath<Shader>(Path);if(previous!=null&&!ShaderUtil.ShaderHasError(previous))return previous;throw new InvalidOperationException("Existing graph needs targeted repair; do not overwrite blindly.");}
            AssetDatabase.ImportAsset("Assets/13_AlphaCandidate/Shaders/MedievalSurface.hlsl",ImportAssetOptions.ForceSynchronousImport);
            var graph=New(SG+"GraphData");Call(graph,"AddContexts");Set(graph,"path","Aethernia/Alpha");var target=New("UnityEditor.Rendering.Universal.ShaderGraph.UniversalTarget");Set(target,"allowMaterialOverride",true);Call(target,"TrySetActiveSubTarget",Type("UnityEditor.Rendering.Universal.ShaderGraph.UniversalLitSubTarget"));
            var init=graph.GetType().GetMethod("InitializeOutputs",Flags);var targetType=init.GetParameters()[0].ParameterType.GetElementType();var descriptorType=init.GetParameters()[1].ParameterType.GetElementType();var targets=Array.CreateInstance(targetType,1);targets.SetValue(target,0);
            var names=new[]{"VertexDescription.Position","VertexDescription.Normal","VertexDescription.Tangent","SurfaceDescription.BaseColor","SurfaceDescription.NormalTS","SurfaceDescription.Metallic","SurfaceDescription.Smoothness","SurfaceDescription.Emission","SurfaceDescription.Occlusion","SurfaceDescription.Alpha"};var descriptors=Array.CreateInstance(descriptorType,names.Length);
            for(var i=0;i<names.Length;i++){var part=names[i].Split('.');var t=Type(SG+"BlockFields+"+part[0]);descriptors.SetValue(t.GetField(part[1],Flags).GetValue(null),i);}Call(graph,"InitializeOutputs",targets,descriptors);
            var getNodes=graph.GetType().GetMethods(Flags).First(m=>m.Name=="GetNodes"&&m.IsGenericMethodDefinition&&m.GetParameters().Length==0).MakeGenericMethod(Type(SG+"BlockNode"));var blocks=((IEnumerable)getNodes.Invoke(graph,null)).Cast<object>().ToArray();
            object Block(string name)=>blocks.Single(b=>(string)Get(Get(b,"descriptor"),"name")==name);
            var albedo=Property(graph,"Texture2DShaderProperty","_BaseMap",null,0);var underlay=Property(graph,"Texture2DShaderProperty","_BlendMap",null,160);var normalMap=Property(graph,"Texture2DShaderProperty","_BumpMap",null,320);Set(Get(normalMap,"property"),"defaultType","NormalMap");
            var tint=Property(graph,"ColorShaderProperty","_BaseColor",Color.white,480);var variation=Property(graph,"Vector1ShaderProperty","_SurfaceVariation",.2f,640);var edge=Property(graph,"Vector1ShaderProperty","_EdgeBlend",0f,780);var scale=Property(graph,"Vector1ShaderProperty","_NoiseScale",.35f,920);var smooth=Property(graph,"Vector1ShaderProperty","_Smoothness",.12f,1060);var metal=Property(graph,"Vector1ShaderProperty","_Metallic",0f,1200);var emission=Property(graph,"ColorShaderProperty","_EmissionColor",Color.black,1340);var bump=Property(graph,"Vector1ShaderProperty","_BumpScale",1f,1480);var wind=Property(graph,"Vector1ShaderProperty","_WindStrength",0f,1620);
            var s=Node(graph,"SampleTexture2DNode",300,0);var u=Node(graph,"SampleTexture2DNode",300,190);var n=Node(graph,"SampleTexture2DNode",300,400);Set(n,"textureType","Normal");Link(graph,albedo,0,s,1);Link(graph,underlay,0,u,1);Link(graph,normalMap,0,n,1);
            var world=Node(graph,"PositionNode",300,650);Set(world,"space","World");var uv=Node(graph,"UVNode",300,840);var guid=AssetDatabase.AssetPathToGUID("Assets/13_AlphaCandidate/Shaders/MedievalSurface.hlsl");var surface=Function(graph,"AetherniaSurface",guid,0);
            Slot(surface,0,"WorldPosition","3");Slot(surface,1,"Surface","4");Slot(surface,2,"Underlay","4");Slot(surface,3,"Tint","4");Slot(surface,4,"UV","2");Slot(surface,5,"Variation","1");Slot(surface,6,"EdgeBlend","1");Slot(surface,7,"NoiseScale","1");Slot(surface,8,"Smoothness","1");Slot(surface,9,"Color","3",true);Slot(surface,10,"Alpha","1",true);Slot(surface,11,"SurfaceSmoothness","1",true);
            Link(graph,world,0,surface,0);Link(graph,s,0,surface,1);Link(graph,u,0,surface,2);Link(graph,tint,0,surface,3);Link(graph,uv,0,surface,4);Link(graph,variation,0,surface,5);Link(graph,edge,0,surface,6);Link(graph,scale,0,surface,7);Link(graph,smooth,0,surface,8);Link(graph,surface,9,Block("BaseColor"),0);Link(graph,surface,10,Block("Alpha"),0);Link(graph,surface,11,Block("Smoothness"),0);Link(graph,metal,0,Block("Metallic"),0);Link(graph,emission,0,Block("Emission"),0);
            var strength=Node(graph,"NormalStrengthNode",650,550);Link(graph,n,0,strength,0);Link(graph,bump,0,strength,1);Link(graph,strength,2,Block("NormalTS"),0);
            var pos=Node(graph,"PositionNode",300,1150);Set(pos,"space","Object");var time=Node(graph,"TimeNode",300,1400);var motion=Function(graph,"AetherniaWind",guid,1000);Slot(motion,0,"Position","3");Slot(motion,1,"Strength","1");Slot(motion,2,"Time","1");Slot(motion,3,"WorldPosition","3");Slot(motion,4,"Out","3",true);Link(graph,pos,0,motion,0);Link(graph,wind,0,motion,1);Link(graph,time,0,motion,2);Link(graph,world,0,motion,3);Link(graph,motion,4,Block("Position"),0);
            Call(graph,"ValidateGraph");var json=(string)Call(Type(SG+"Serialization.MultiJson"),"Serialize",graph);File.WriteAllText(Path,json);AssetDatabase.ImportAsset(Path,ImportAssetOptions.ForceSynchronousImport);var shader=AssetDatabase.LoadAssetAtPath<Shader>(Path);
            if(shader==null||ShaderUtil.ShaderHasError(shader))throw new InvalidOperationException("Generated graph did not compile.");return shader;
        }
    }
}
