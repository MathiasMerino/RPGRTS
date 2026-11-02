using UnityEngine;
using UnityEngine.AI;
namespace Aethernia
{
    public static class CharacterVisualContract
    {
        public static bool CanUse(GameObject prefab,bool requireAnimation,out string reason,bool inspectController=false)
        {
            reason=null;
            if(prefab==null||prefab.GetComponent<CharacterVisualModel>()==null){reason="Missing CharacterVisualModel";return false;}
            if((prefab.transform.localScale-Vector3.one).sqrMagnitude>.0001f){reason="Root must retain metre scale";return false;}
            if(prefab.GetComponentInChildren<Collider>(true)!=null||prefab.GetComponentInChildren<Rigidbody>(true)!=null||prefab.GetComponentInChildren<NavMeshAgent>(true)!=null||prefab.GetComponentInChildren<ExplorationUnit>(true)!=null||prefab.GetComponentInChildren<GameSession>(true)!=null){reason="Visual contains gameplay or physics";return false;}
            var renderer=prefab.GetComponentInChildren<Renderer>(true);
            if(renderer==null||renderer.sharedMaterial==null){reason="Missing renderer/material";return false;}
            var skin=renderer as SkinnedMeshRenderer;var mesh=renderer.GetComponent<MeshFilter>();
            if((skin==null||skin.sharedMesh==null)&&(mesh==null||mesh.sharedMesh==null)){reason="Missing mesh";return false;}
            var driver=prefab.GetComponent<HumanoidAnimationDriver>();
            if(requireAnimation||driver!=null)
            {
                var rig=prefab.GetComponent<HumanoidVisualRig>();var profile=driver==null?null:driver.Profile;
                if(rig==null||!rig.IsUsable||profile==null||profile.Controller==null||!profile.HasClip(CharacterMotion.Idle)||!profile.HasClip(CharacterMotion.Walk)||!profile.HasClip(CharacterMotion.Attack)||!profile.HasClip(CharacterMotion.Defend)||!profile.HasClip(CharacterMotion.Hit)||!profile.HasClip(CharacterMotion.Defeat)){reason="Rig or animation contract incomplete";return false;}
            }
            if(inspectController&&driver!=null)
            {
                var profile=driver.Profile;var clips=profile.Controller.animationClips;
                foreach(var required in new[]{profile.Idle,profile.Walk,profile.Attack,profile.Defend,profile.Hit,profile.Defeat})
                {var found=false;foreach(var clip in clips)if(clip==required){found=true;break;}if(!found){reason="Profile clips not bound to controller";return false;}}
                foreach(var behaviour in prefab.GetComponentsInChildren<MonoBehaviour>(true))if(behaviour==null){reason="Missing behaviour script";return false;}
            }
            return true;
        }
    }
}
