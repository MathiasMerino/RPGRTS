#ifndef AETHERNIA_ALPHA_SURFACE_INCLUDED
#define AETHERNIA_ALPHA_SURFACE_INCLUDED
float AetherniaHash(float2 p){p=frac(p*float2(.1031,.11369));p+=dot(p,p.yx+19.19);return frac(p.x*p.y);}
float AetherniaNoise(float2 p){float2 i=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(AetherniaHash(i),AetherniaHash(i+float2(1,0)),f.x),lerp(AetherniaHash(i+float2(0,1)),AetherniaHash(i+1),f.x),f.y);}
void AetherniaSurface_float(float3 WorldPosition,float4 Surface,float4 Underlay,float4 Tint,float2 UV,float Variation,float EdgeBlend,float NoiseScale,float Smoothness,out float3 Color,out float Alpha,out float SurfaceSmoothness)
{
 float n=AetherniaNoise(float2(WorldPosition.x+WorldPosition.y*.37,WorldPosition.z+WorldPosition.y*.73)*NoiseScale);
 float edge=min(min(UV.x,1-UV.x),min(UV.y,1-UV.y));float mask=lerp(1,smoothstep(0,.12,edge+(n-.5)*.035),saturate(EdgeBlend));
 float3 base=lerp(Underlay.rgb,Surface.rgb*Tint.rgb,mask);Color=base*lerp(1,lerp(.78,1.13,n),saturate(Variation));Alpha=Surface.a*Tint.a;SurfaceSmoothness=saturate(Smoothness*lerp(.85,1.08,n));
}
void AetherniaSurface_half(half3 p,half4 s,half4 u,half4 t,half2 uv,half v,half e,half n,half sm,out half3 c,out half a,out half r){float3 cf;float af,rf;AetherniaSurface_float(p,s,u,t,uv,v,e,n,sm,cf,af,rf);c=cf;a=af;r=rf;}
void AetherniaWind_float(float3 Position,float Strength,float Time,float3 WorldPosition,out float3 Out)
{
 Out=Position;if(Strength<=0)return;float weight=saturate(Position.y+.4)*Strength;float phase=WorldPosition.x*.23+WorldPosition.z*.17;
 Out.x+=sin(Time*1.35+phase)*weight;Out.z+=cos(Time*1.08+phase)*weight*.4;
}
void AetherniaWind_half(half3 p,half s,half t,half3 w,out half3 o){float3 result;AetherniaWind_float(p,s,t,w,result);o=result;}
#endif
