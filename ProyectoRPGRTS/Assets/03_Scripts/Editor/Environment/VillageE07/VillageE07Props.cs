using UnityEngine;
using System.Collections.Generic;
using G=Aethernia.Editor.Environment.EnvironmentGeometry;
using V=Aethernia.Editor.Environment.VillageE07Geometry;

namespace Aethernia.Editor.Environment
{
    public static class VillageE07Props
    {
        public static void Create()
        {
            V.Save("Props","Bench_180cm",p=>
            {
                V.Box(p,"Seat",new Vector3(0,.46f,0),new Vector3(1.8f,.09f,.42f),"AgedOak");V.Box(p,"Backrest",new Vector3(0,.78f,.2f),new Vector3(1.8f,.26f,.07f),"AgedOak");
                foreach(var x in new[]{-.67f,.67f}){V.Box(p,"Trestle",new Vector3(x,.22f,0),new Vector3(.13f,.44f,.38f),"DarkOak");V.Box(p,"Back support",new Vector3(x,.66f,.2f),new Vector3(.1f,.5f,.1f),"DarkOak");}
                VillageE07Nature.SolidBox(p,new Vector3(0,.45f,0),new Vector3(1.8f,.90f,.5f));
            });
            V.Save("Props","Table_180cm",p=>
            {
                V.Box(p,"Tabletop",new Vector3(0,.82f,0),new Vector3(1.8f,.10f,.85f),"AgedOak");
                foreach(var x in new[]{-.65f,.65f})foreach(var z in new[]{-.28f,.28f})V.Box(p,"Leg",new Vector3(x,.385f,z),new Vector3(.12f,.77f,.12f),"DarkOak");
                V.Box(p,"Stretcher",new Vector3(0,.27f,0),new Vector3(1.45f,.1f,.12f),"DarkOak");VillageE07Nature.SolidBox(p,new Vector3(0,.44f,0),new Vector3(1.8f,.88f,.85f));
            });
            V.Save("Props","SupplySack_75cm",p=>
            {V.Part(p,"Linen sack",G.Lathe("E07Sack",new[]{0f,.06f,.27f,.50f,.63f,.69f,.75f},new[]{.22f,.30f,.34f,.29f,.14f,.105f,.12f}),"Linen",Vector3.zero);V.Part(p,"Tie",G.Lathe("E07SackTie",new[]{.66f,.69f},new[]{.116f,.116f}),"Leather",Vector3.zero);VillageE07Nature.SolidBox(p,new Vector3(0,.36f,0),new Vector3(.62f,.72f,.62f));});
            V.Save("Props","Woodpile_2m",p=>
            {for(var row=0;row<3;row++)for(var col=0;col<4-row;col++){var x=(col-(3-row)*.5f)*.38f;V.Pole(p,new Vector3(x,.18f+row*.31f,-.85f),new Vector3(x,.18f+row*.31f,.85f),.18f,.16f,"AgedOak");}VillageE07Nature.SolidBox(p,new Vector3(0,.50f,0),new Vector3(1.5f,1,1.8f));});
            V.Save("Props","VillageLantern_250cm",Lantern);
            V.Save("Props","Torch_170cm",p=>{V.Pole(p,Vector3.zero,Vector3.up*1.48f,.055f,.045f,"DarkOak");V.Part(p,"Iron basket",G.Lathe("E07Brazier",new[]{1.35f,1.5f},new[]{.08f,.13f}),"Iron",Vector3.zero);V.Part(p,"Small flame",G.Lathe("E07Flame",new[]{1.48f,1.60f,1.79f},new[]{.085f,.12f,0}),"EmberGlass",Vector3.zero);});
            V.Save("Props","WayfindingSign_190cm",p=>{V.Box(p,"Sign post",new Vector3(0,.95f,0),new Vector3(.14f,1.9f,.14f),"DarkOak");V.Box(p,"Board",new Vector3(0,1.55f,-.09f),new Vector3(1.25f,.50f,.10f),"AgedOak");foreach(var x in new[]{-.48f,.48f})V.Box(p,"Rivet",new Vector3(x,1.55f,-.15f),new Vector3(.04f,.04f,.02f),"Iron");});
            V.Save("Props","ToolRack_140cm",p=>
            {
                foreach(var x in new[]{-.6f,.6f})V.Box(p,"Rack leg",new Vector3(x,.75f,0),new Vector3(.1f,1.5f,.12f),"DarkOak");V.Box(p,"Rack rail",new Vector3(0,1.15f,0),new Vector3(1.4f,.11f,.12f),"AgedOak");
                for(var i=0;i<3;i++){var x=(i-1)*.4f;V.Pole(p,new Vector3(x,.15f,-.12f),new Vector3(x,1.37f,-.12f),.025f,.025f,"AgedOak");if(i==0){for(var k=0;k<3;k++)V.Box(p,"Fork tine",new Vector3(x+(k-1)*.08f,1.48f,-.12f),new Vector3(.025f,.26f,.03f),"Iron");}else V.Box(p,"Tool blade",new Vector3(x,1.45f,-.12f),new Vector3(.22f,.27f,.05f),"Iron");}
                VillageE07Nature.SolidBox(p,new Vector3(0,.80f,0),new Vector3(1.4f,1.6f,.35f));
            });
            V.Save("Props","WickerBasket_60cm",p=>
            {
                V.Part(p,"Wicker body",G.Lathe("E07Basket",new[]{0f,.08f,.35f,.42f},new[]{.23f,.28f,.31f,.32f}),"Linen",Vector3.zero);
                for(var i=0;i<5;i++)V.Part(p,"Woven ring",G.Lathe("E07BasketRing"+i,new[]{.06f+i*.075f,.085f+i*.075f},new[]{.25f+i*.016f,.25f+i*.016f}),"AgedOak",Vector3.zero);
                VillageE07Nature.SolidBox(p,new Vector3(0,.22f,0),new Vector3(.64f,.44f,.64f));
            });
            V.Save("Props","VillageWell_2m",p=>
            {
                Ring(p,Vector3.zero,.72f,.52f,.82f,"GreyStone");Ring(p,Vector3.up*.82f,.79f,.49f,.12f,"WarmStone");
                V.Part(p,"Dark water",G.Lathe("E07WellWater",new[]{.37f,.39f},new[]{.50f,.50f}),"Iron",Vector3.zero);
                foreach(var x in new[]{-.96f,.96f})V.Box(p,"Well upright",new Vector3(x,1.15f,0),new Vector3(.14f,2.3f,.16f),"DarkOak");
                V.Pole(p,new Vector3(-1,1.55f,0),new Vector3(1,1.55f,0),.07f,.07f,"AgedOak");V.Pole(p,new Vector3(0,.65f,0),new Vector3(0,1.7f,0),.018f,.018f,"Leather");
                var roof=V.Group(p,"Well roof");roof.localPosition=Vector3.up*2.3f;VillageE07Architecture.Roof(roof,2.4f,1.8f,.65f);
                VillageE07Nature.SolidBox(p,new Vector3(0,.55f,0),new Vector3(1.7f,1.1f,1.7f));
            });
            V.Save("Props","Handcart_240cm",Cart);
            V.Save("Props","Runestone_80cm",p=>{V.Box(p,"Stone plinth",new Vector3(0,.16f,0),new Vector3(.8f,.32f,.8f),"GreyStone");V.Part(p,"Worn standing stone",G.Lathe("E07StandingStone",new[]{.27f,.5f,1.5f,1.7f},new[]{.34f,.35f,.29f,.12f}),"GreyStone",Vector3.zero);V.Box(p,"Bronze inset",new Vector3(0,1,-.31f),new Vector3(.12f,.42f,.025f),"Bronze");});
        }
        private static void Lantern(Transform p)
        {
            V.Box(p,"Lamp post",new Vector3(0,1.25f,0),new Vector3(.13f,2.5f,.13f),"DarkOak");V.Beam(p,new Vector3(0,2.4f,0),new Vector3(.58f,2.4f,0),.09f,"Iron");
            V.Box(p,"Lantern glass",new Vector3(.5f,2.1f,0),new Vector3(.23f,.35f,.23f),"EmberGlass");
            foreach(var y in new[]{1.89f,2.31f})V.Box(p,"Iron hood",new Vector3(.5f,y,0),new Vector3(.34f,.07f,.34f),"Iron");
            foreach(var x in new[]{.355f,.645f})foreach(var z in new[]{-.145f,.145f})V.Box(p,"Lamp frame",new Vector3(x,2.1f,z),new Vector3(.035f,.4f,.035f),"Iron");
        }
        private static void Cart(Transform p)
        {
            V.Box(p,"Bed",new Vector3(0,.70f,0),new Vector3(1.12f,.12f,1.9f),"AgedOak");
            foreach(var x in new[]{-.55f,.55f})for(var row=0;row<3;row++)V.Box(p,"Side board",new Vector3(x,.86f+row*.19f,0),new Vector3(.09f,.16f,1.9f),"AgedOak");
            foreach(var z in new[]{-.94f,.94f})for(var row=0;row<3;row++)V.Box(p,"End board",new Vector3(0,.86f+row*.19f,z),new Vector3(1.1f,.16f,.09f),"AgedOak");
            V.Pole(p,new Vector3(-.88f,.47f,.15f),new Vector3(.88f,.47f,.15f),.06f,.06f,"Iron");
            foreach(var x in new[]{-.79f,.79f})
            {
                var wheel=V.Group(p,"Spoked wheel");wheel.localPosition=new Vector3(x,.47f,.15f);wheel.localRotation=Quaternion.Euler(0,0,90);Ring(wheel,Vector3.zero,.46f,.35f,.10f,"DarkOak");Ring(wheel,new Vector3(0,-.01f,0),.475f,.45f,.12f,"Iron");
                for(var i=0;i<8;i++){var a=i*Mathf.PI/4;V.Beam(wheel,new Vector3(0,.05f,0),new Vector3(Mathf.Cos(a)*.36f,.05f,Mathf.Sin(a)*.36f),.055f,"AgedOak");}
                V.Part(wheel,"Wheel hub",G.Lathe("E07WheelHub",new[]{-.04f,.15f},new[]{.10f,.10f}),"Iron",Vector3.zero);
            }
            foreach(var x in new[]{-.4f,.4f})V.Beam(p,new Vector3(x,.68f,-.8f),new Vector3(x,.48f,-2),.07f,"DarkOak");
            VillageE07Nature.SolidBox(p,new Vector3(0,.65f,-.4f),new Vector3(1.8f,1.3f,3.2f));
        }
        private static void Ring(Transform p,Vector3 centre,float outer,float inner,float height,string mat)
        {
            var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();const int n=24;
            for(var i=0;i<n;i++)
            {
                var a=i*Mathf.PI*2/n;var b=(i+1)*Mathf.PI*2/n;var ao=new Vector3(Mathf.Cos(a)*outer,0,Mathf.Sin(a)*outer);var bo=new Vector3(Mathf.Cos(b)*outer,0,Mathf.Sin(b)*outer);var ai=ao*(inner/outer);var bi=bo*(inner/outer);var up=Vector3.up*height;
                G.Face(v,uv,t,ao,ao+up,bo+up,bo);G.Face(v,uv,t,bi,bi+up,ai+up,ai);G.Face(v,uv,t,ao+up,ai+up,bi+up,bo+up);
            }
            V.Part(p,"Hollow rim",V.Mesh("Ring",v,t,uv),mat,centre);
        }
    }
}
