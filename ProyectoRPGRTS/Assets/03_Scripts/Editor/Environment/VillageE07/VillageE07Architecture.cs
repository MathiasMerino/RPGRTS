using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using G=Aethernia.Editor.Environment.EnvironmentGeometry;
using V=Aethernia.Editor.Environment.VillageE07Geometry;
using M=Aethernia.Editor.Environment.VillageE07Materials;

namespace Aethernia.Editor.Environment
{
    public static class VillageE07Architecture
    {
        public static void Create()
        {
            M.Create();
            V.Save("Modules","TimberBay_2m",p=>Wall(p,2,3,false));
            V.Save("Modules","StoneBay_2m",p=>Wall(p,2,2.6f,true));
            V.Save("Modules","Door_140x240cm",p=>Door(p,false));
            V.Save("Modules","Window_100x120cm",Window);
            V.Save("Modules","Roof_6x8m",p=>Roof(p,6,8,2));
            V.Save("Modules","Fence_2m",p=>Fence(p));
            V.Save("Modules","StoneWall_2m",p=>{V.Box(p,"Wall",new Vector3(0,.48f,0),new Vector3(2,.96f,.4f),"GreyStone");V.Box(p,"Capstone",new Vector3(0,1,0),new Vector3(2.08f,.12f,.48f),"WarmStone");});
            Building("TimberHouse_6x8m",6,8,3.1f,2,false,true,false);
            Building("VillageInn_55x6m",5.5f,6,4.2f,2.2f,false,false,true);
            Building("StoneCottage_45x45m",4.5f,4.5f,2.65f,1.65f,true,false,false);
            Building("Storehouse_42x5m",4.2f,5,2.8f,1.6f,true,false,false);
            V.Save("Architecture","OpenWoodshed_3x3m",p=>
            {
                foreach(var x in new[]{-1.3f,1.3f})foreach(var z in new[]{-1.3f,1.3f})V.Box(p,"Post",new Vector3(x,1.2f,z),new Vector3(.16f,2.4f,.16f),"DarkOak");
                V.Box(p,"Rear timber wall",new Vector3(0,1.15f,1.3f),new Vector3(2.6f,2.3f,.12f),"AgedOak");
                var roof=V.Group(p,"Roof");roof.localPosition=Vector3.up*2.35f;Roof(roof,3,3,.7f);
            });
            AssetDatabase.SaveAssets();Debug.Log("E07_LIBRARY: five original albedos, shared URP materials, seven modules and five architecture prefabs; originals preserved.");
        }
        private static void Wall(Transform p,float width,float height,bool stone)
        {
            V.Box(p,"Infill",new Vector3(0,height*.5f,0),new Vector3(width,height,.22f),stone?"GreyStone":"Plaster");
            V.Box(p,"Stone foundation",new Vector3(0,.36f,-.015f),new Vector3(width,.72f,.27f),"WarmStone");
            if(!stone)
            {
                foreach(var x in new[]{-width*.5f+.08f,width*.5f-.08f})V.Box(p,"Post",new Vector3(x,height*.5f,-.06f),new Vector3(.16f,height,.22f),"DarkOak");
                foreach(var y in new[]{.76f,height-.1f})V.Box(p,"Rail",new Vector3(0,y,-.1f),new Vector3(width,.16f,.20f),"DarkOak");
                V.Beam(p,new Vector3(-width*.4f,.88f,-.16f),new Vector3(width*.38f,height-.24f,-.16f),.105f,"DarkOak");
            }
        }
        private static void Door(Transform p,bool closed)
        {
            foreach(var x in new[]{-.78f,.78f})V.Box(p,"Door jamb",new Vector3(x,1.2f,0),new Vector3(.18f,2.4f,.28f),"DarkOak");
            V.Box(p,"Lintel",new Vector3(0,2.31f,0),new Vector3(1.4f,.18f,.28f),"DarkOak");
            if(closed)
            {
                V.Box(p,"Oak door leaf",new Vector3(0,1.09f,.035f),new Vector3(1.36f,2.18f,.10f),"AgedOak");
                foreach(var y in new[]{.35f,1.7f})V.Box(p,"Strap hinge",new Vector3(0,y,-.028f),new Vector3(1.23f,.07f,.025f),"Iron");
                V.Box(p,"Latch",new Vector3(.4f,1,-.06f),new Vector3(.14f,.08f,.06f),"Bronze");
            }
        }
        private static void Window(Transform p)
        {
            V.Box(p,"Dark recess",new Vector3(0,.65f,.02f),new Vector3(1,1.2f,.025f),"DarkOak");
            V.Box(p,"Warm glazed panes",new Vector3(0,.65f,-.005f),new Vector3(.85f,1.05f,.018f),"EmberGlass");
            foreach(var x in new[]{-.55f,.55f})V.Box(p,"Jamb",new Vector3(x,.65f,-.04f),new Vector3(.12f,1.42f,.18f),"DarkOak");
            foreach(var y in new[]{-.06f,1.36f})V.Box(p,"Sill",new Vector3(0,y,-.05f),new Vector3(1.22f,.14f,.27f),"AgedOak");
            V.Box(p,"Mullion",new Vector3(0,.65f,-.04f),new Vector3(.07f,1.2f,.05f),"DarkOak");V.Box(p,"Crossbar",new Vector3(0,.65f,-.04f),new Vector3(1,.07f,.05f),"DarkOak");
            foreach(var x in new[]{-.73f,.73f})V.Box(p,"Open shutter",new Vector3(x,.65f,-.02f),new Vector3(.24f,1.2f,.07f),"AgedOak");
        }
        private static void Building(string name,float w,float l,float h,float rise,bool stone,bool open,bool inn)
        {
            V.Save("Architecture",name,p=>
            {
                var baseNode=V.Group(p,"Low foundation");
                // The original house's entry stays open. New closed buildings are solid and carve navigation.
                foreach(var side in new[]{-1,1})
                {
                    V.Box(baseNode,"Side plinth",new Vector3(side*(w*.5f-.1f),.33f,0),new Vector3(.28f,.66f,l),"WarmStone");
                    V.Box(baseNode,"Front plinth",new Vector3(side*(w*.25f+.4f),.33f,-l*.5f+.1f),new Vector3(w*.5f-.8f,.66f,.28f),"WarmStone");
                }
                V.Box(baseNode,"Back plinth",new Vector3(0,.33f,l*.5f-.1f),new Vector3(w,.66f,.28f),"WarmStone");
                if(open)V.Box(baseNode,"Floor",new Vector3(0,.006f,0),new Vector3(w-.4f,.012f,l-.4f),"AgedOak");
                V.Merge(baseNode,name+"_Foundation");
                var upper=V.Group(p,"Contextual cutaway");var walls=V.Group(upper,"Upper walls");
                var count=Mathf.CeilToInt(l/2);var bay=l/count;
                for(var i=0;i<count;i++)foreach(var side in new[]{-1,1})
                {var node=V.Group(walls,"Side bay");node.localPosition=new Vector3(side*(w*.5f-.1f),0,-l*.5f+bay*(i+.5f));node.localRotation=Quaternion.Euler(0,side==1?-90:90,0);Wall(node,bay,h,stone);}
                var back=V.Group(walls,"Rear facade");back.localPosition=new Vector3(0,0,l*.5f-.1f);back.localRotation=Quaternion.Euler(0,180,0);Wall(back,w,h,stone);
                foreach(var side in new[]{-1,1})
                {var front=V.Group(walls,"Front bay");front.localPosition=new Vector3(side*(w*.25f+.4f),0,-l*.5f+.1f);Wall(front,w*.5f-.8f,h,stone);}
                V.Box(walls,"Door head infill",new Vector3(0,2.4f+(h-2.4f)*.5f,-l*.5f+.1f),new Vector3(1.6f,h-2.4f,.22f),stone?"GreyStone":"Plaster");
                var door=V.Group(walls,"Entry");door.localPosition=new Vector3(0,0,-l*.5f-.055f);Door(door,!open);
                foreach(var side in new[]{-1,1})
                {
                    var win=V.Group(walls,"Front window");win.localPosition=new Vector3(side*w*.32f,1.05f,-l*.5f-.035f);Window(win);
                    var flank=V.Group(walls,"Side window");flank.localPosition=new Vector3(side*(w*.5f+.035f),1.05f,0);flank.localRotation=Quaternion.Euler(0,side==1?-90:90,0);Window(flank);
                    if(inn){var loft=V.Group(walls,"Upper window");loft.localPosition=new Vector3(side*w*.31f,2.85f,-l*.5f-.04f);loft.localScale=Vector3.one*.72f;Window(loft);}
                }
                V.Merge(walls,name+"_Walls");
                var roof=V.Group(upper,"Pitched roof");roof.localPosition=Vector3.up*(h+.08f);Roof(roof,w+.6f,l+.6f,rise);V.Merge(roof,name+"_Roof");
                var chimney=V.Group(upper,"Chimney");var cx=w*.25f;var cz=l*.2f;V.Box(chimney,"Chimney masonry",new Vector3(cx,h+rise*.65f,cz),new Vector3(.58f,rise+1,.66f),"GreyStone");
                var top=h+rise*1.15f+.5f;V.Box(chimney,"Chimney crown",new Vector3(cx,top,cz),new Vector3(.78f,.17f,.84f),"WarmStone");V.Box(chimney,"Flue",new Vector3(cx,top+.091f,cz),new Vector3(.43f,.014f,.49f),"Iron");V.Merge(chimney,name+"_Chimney");
                if(!open)
                {
                    var collider=p.gameObject.AddComponent<BoxCollider>();collider.center=Vector3.up*h*.5f;collider.size=new Vector3(w,h,l);
                    var obstacle=p.gameObject.AddComponent<NavMeshObstacle>();obstacle.carving=true;obstacle.center=collider.center;obstacle.size=collider.size;
                }
            },false);
        }
        public static void Roof(Transform p,float w,float l,float rise)
        {
            var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
            foreach(var side in new[]{-1,1})
            {
                var a=new Vector3(side*w*.5f,0,-l*.5f);var b=new Vector3(side*w*.5f,0,l*.5f);var c=new Vector3(0,rise,l*.5f);var d=new Vector3(0,rise,-l*.5f);
                var n=vertices.Count;vertices.AddRange(new[]{a,b,c,d});var slope=Vector3.Distance(b,c)/2;
                uv.AddRange(new[]{Vector2.zero,new Vector2(l/2,0),new Vector2(l/2,slope),new Vector2(0,slope)});
                triangles.AddRange(side<0?new[]{n,n+1,n+2,n,n+2,n+3}:new[]{n+2,n+1,n,n+3,n+2,n});
            }
            V.Part(p,"Slate slopes",V.Mesh("RoofSlopes",vertices,triangles,uv),"Slate",Vector3.zero);
            foreach(var z in new[]{-l*.5f,l*.5f})
            {
                var v=new List<Vector3>{new Vector3(-w*.5f,0,z),new Vector3(0,rise,z),new Vector3(w*.5f,0,z)};
                var t=z<0?new List<int>{0,1,2}:new List<int>{2,1,0};V.Part(p,"Gable",V.Mesh("Gable",v,t,new List<Vector2>{Vector2.zero,new Vector2(w/4,rise/2),new Vector2(w/2,0)}),"Plaster",Vector3.zero);
                foreach(var side in new[]{-1,1})V.Beam(p,new Vector3(side*w*.5f,-.06f,z),new Vector3(0,rise+.06f,z),.18f,"DarkOak");
                V.Beam(p,new Vector3(0,0,z-.018f),new Vector3(0,rise,z-.018f),.15f,"DarkOak");V.Box(p,"Gable tie",new Vector3(0,0,z),new Vector3(w,.19f,.2f),"DarkOak");
            }
            foreach(var x in new[]{-w*.5f,w*.5f})V.Box(p,"Eaves and fascia",new Vector3(x,-.075f,0),new Vector3(.2f,.22f,l+.12f),"DarkOak");
            for(var i=0;i<Mathf.CeilToInt(l/.5f);i++)V.Part(p,"Ridge cap",G.Lathe("E07Ridge",new[]{0f,.49f},new[]{.14f,.14f}),"Slate",new Vector3(0,rise,-l*.5f+i*.5f)).transform.localRotation=Quaternion.Euler(90,0,0);
        }
        public static void Fence(Transform p)
        {
            foreach(var x in new[]{-.94f,.94f}){V.Box(p,"Fence post",new Vector3(x,.6f,0),new Vector3(.13f,1.2f,.13f),"DarkOak");V.Pole(p,new Vector3(x,1.2f,0),new Vector3(x,1.32f,0),.09f,0,"DarkOak");}
            foreach(var y in new[]{.45f,.9f})V.Box(p,"Rough rail",new Vector3(0,y,0),new Vector3(2,.12f,.08f),"AgedOak");
        }
    }
}
