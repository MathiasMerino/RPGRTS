using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aethernia.Editor.Characters
{
    public static class NativeCharacterClips
    {
        public static AnimationClip Create(NativeCharacterSkin rig,string id,CharacterMotion motion,string path)
        {
            var duration=motion==CharacterMotion.Attack?(id=="mage"?1.3f:.95f):motion==CharacterMotion.Hit?.38f:motion==CharacterMotion.Defeat?1.3f:motion==CharacterMotion.Run?.65f:motion==CharacterMotion.Walk?1.05f:2;
            var clip=new AnimationClip{name=id+"_"+motion,frameRate=30,legacy=false};const int samples=40;
            foreach(var pair in rig.Indices)
            {
                var curves=new[]{new AnimationCurve(),new AnimationCurve(),new AnimationCurve(),new AnimationCurve()};
                for(var i=0;i<=samples;i++)
                {
                    var t=(float)i/samples;var rotation=Quaternion.Euler(Pose(pair.Key,id,motion,t));
                    curves[0].AddKey(t*duration,rotation.x);curves[1].AddKey(t*duration,rotation.y);curves[2].AddKey(t*duration,rotation.z);curves[3].AddKey(t*duration,rotation.w);
                }
                for(var axis=0;axis<4;axis++)AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve(rig.Path(pair.Key),typeof(Transform),"m_LocalRotation."+"xyzw"[axis]),curves[axis]);
            }
            var y=new AnimationCurve();var hips=rig.Get("Hips").localPosition;
            for(var i=0;i<=samples;i++)
            {
                var t=(float)i/samples;var bob=(motion==CharacterMotion.Walk?.013f:motion==CharacterMotion.Run?.038f:0)*Mathf.Pow(Mathf.Sin(t*Mathf.PI*2),2);
                var drop=motion==CharacterMotion.Defeat?Mathf.SmoothStep(0,.54f,Mathf.Clamp01(t*1.5f)):0;
                y.AddKey(t*duration,hips.y+bob-drop);
            }
            AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve(rig.Path("Hips"),typeof(Transform),"m_LocalPosition.y"),y);
            clip.EnsureQuaternionContinuity();var settings=AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime=motion==CharacterMotion.Idle||motion==CharacterMotion.Walk||motion==CharacterMotion.Run||motion==CharacterMotion.Defend;
            settings.keepOriginalPositionXZ=true;settings.keepOriginalPositionY=true;settings.keepOriginalOrientation=true;AnimationUtility.SetAnimationClipSettings(clip,settings);
            AssetDatabase.CreateAsset(clip,path);return clip;
        }
        private static Vector3 Pose(string bone,string id,CharacterMotion motion,float t)
        {
            var p=Vector3.zero;var left=bone.StartsWith("Left",StringComparison.Ordinal);var right=bone.StartsWith("Right",StringComparison.Ordinal);var s=right?-1f:1f;
            var arm=bone.EndsWith("UpperArm",StringComparison.Ordinal);var forearm=bone.EndsWith("Forearm",StringComparison.Ordinal);var hand=bone.EndsWith("Hand",StringComparison.Ordinal);
            if(arm)p.z=s*78;
            if(forearm)p.y=s*12;
            if(bone.EndsWith("Fingers",StringComparison.Ordinal))p.z=s*(id=="monk"?45:30);
            if(bone=="Chest")p.x=Mathf.Sin(t*Mathf.PI*2)*.9f;
            if(bone=="Head")p.x=2+Mathf.Sin(t*Mathf.PI*2)*.5f;
            if(motion==CharacterMotion.Idle)
            {
                if(id=="monk")
                {if(arm)p=new Vector3(-8,s*35,s*48);if(forearm)p=new Vector3(0,s*46,-s*72);}
                else if(id=="warrior")
                {if(arm)p=new Vector3(0,s*15,s*76);if(forearm)p=new Vector3(0,s*25,-s*10);}
                else
                {if(bone=="LeftUpperArm")p=new Vector3(0,12,70);if(bone=="LeftForearm")p=new Vector3(0,22,-12);if(bone=="RightHand")p.z=-8;}
            }
            if(motion==CharacterMotion.Walk||motion==CharacterMotion.Run)
            {
                var run=motion==CharacterMotion.Run;var phase=t*Mathf.PI*2+(left?Mathf.PI:0);var stride=Mathf.Sin(phase);var lift=Mathf.Max(0,-stride);
                if(bone.EndsWith("UpperLeg",StringComparison.Ordinal))p.x=stride*(run?39:24);
                if(bone.EndsWith("LowerLeg",StringComparison.Ordinal))p.x=lift*(run?66:36)+5;
                if(bone.EndsWith("Foot",StringComparison.Ordinal))p.x=-stride*12-lift*18;
                if(bone.EndsWith("Toes",StringComparison.Ordinal))p.x=-lift*12;
                if(arm){p.x=-stride*(run?32:20);p.z=s*76;}
                if(forearm){p.x=-lift*(run?45:10);p.y=s*(run?38:16);}
                if(bone=="Hips")p.y=Mathf.Sin(t*Mathf.PI*2)*4;
                if(bone=="Chest")p=new Vector3(run?9:3,-Mathf.Sin(t*Mathf.PI*2)*6,0);
            }
            if(motion==CharacterMotion.Defend || motion==CharacterMotion.Attack)
            {
                if(id=="monk")
                {
                    if(arm)p=new Vector3(-12,s*35,s*48);
                    if(forearm)p=new Vector3(0,s*46,-s*72);
                    if(bone=="Chest")p=new Vector3(3,-12,0);
                    if(bone.EndsWith("UpperLeg",StringComparison.Ordinal))p=new Vector3(left?-12:12,0,s*-7);
                    if(bone.EndsWith("LowerLeg",StringComparison.Ordinal))p.x=22;
                    if(bone.EndsWith("Foot",StringComparison.Ordinal))p.x=-12;
                }
                else if(id=="warrior")
                {
                    if(arm)p=new Vector3(-8,s*38,s*47);
                    if(forearm)p=new Vector3(0,s*30,-s*72);
                    if(bone=="Chest")p.y=-16;
                }
                else
                {
                    if(arm)p=new Vector3(-5,s*35,s*(left?60:40));
                    if(forearm)p=new Vector3(0,s*36,-s*60);
                    if(bone=="Head")p.x=-5;
                }
                if(motion==CharacterMotion.Attack)
                {
                    var strike=Mathf.Sin(Mathf.Clamp01((t-.20f)/.55f)*Mathf.PI);
                    if(id=="monk")
                    {
                        if(bone=="RightUpperArm")p=Vector3.Lerp(p,new Vector3(0,-78,-8),strike);
                        if(bone=="RightForearm")p=Vector3.Lerp(p,new Vector3(0,-8,0),strike);
                        if(bone=="Chest")p.y=-12+strike*28;
                    }
                    else if(id=="warrior")
                    {
                        var sweep=Mathf.SmoothStep(0,1,Mathf.Clamp01((t-.15f)/.52f));var recover=Mathf.SmoothStep(0,1,Mathf.Clamp01((t-.72f)/.28f));
                        if(bone=="RightUpperArm")p=Vector3.Lerp(new Vector3(-20,-20,15),new Vector3(35,-115,-48),sweep);
                        if(bone=="RightUpperArm")p=Vector3.Lerp(p,new Vector3(-8,-38,-47),recover);
                        if(bone=="RightForearm")p=new Vector3(0,-20,35*(1-strike));
                        if(bone=="Chest")p.y=-22+strike*48;
                    }
                    else
                    {
                        if(bone=="RightUpperArm")p=Vector3.Lerp(p,new Vector3(-12,-80,-15),strike);
                        if(bone=="RightForearm")p=Vector3.Lerp(p,new Vector3(0,-10,-8),strike);
                        if(bone=="RightHand")p=new Vector3(0,strike*20,-strike*18);
                        if(bone=="Chest")p.x=-strike*6;
                    }
                }
            }
            if(motion==CharacterMotion.Hit)
            {
                var hit=Mathf.Sin(t*Mathf.PI);if(bone=="Chest")p.x=-hit*20;if(bone=="Head")p.x=-hit*12;if(arm)p.x=hit*15;
            }
            if(motion==CharacterMotion.Defeat)
            {
                var fall=Mathf.SmoothStep(0,1,Mathf.Clamp01(t*1.5f));
                if(bone=="Hips")p.x=fall*15;if(bone=="Spine")p.x=fall*30;if(bone=="Chest")p.x=fall*32;if(bone=="Head")p.x=fall*28;
                if(bone.EndsWith("UpperLeg",StringComparison.Ordinal))p.x=fall*-102;
                if(bone.EndsWith("LowerLeg",StringComparison.Ordinal))p.x=fall*136;
                if(bone.EndsWith("Foot",StringComparison.Ordinal))p.x=fall*-49;
                if(arm)p=new Vector3(fall*-22,0,s*75);if(forearm)p.y=s*20;
            }
            return p;
        }
    }
}
