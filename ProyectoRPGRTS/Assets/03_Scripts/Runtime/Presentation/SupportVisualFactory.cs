using UnityEngine;

namespace Aethernia
{
    public static class SupportVisualFactory
    {
        public static CharacterVisualModel Attach(ExplorationUnit unit,RtsRoleProfile role,CharacterVisualCatalog catalog,Material effect)
        {
            var prefab=catalog==null?null:catalog.Find(role.VisualClass);if(prefab==null)return null;
            var physical=unit.GetComponentsInChildren<Renderer>();var node=Object.Instantiate(prefab,unit.transform,false);var model=node.GetComponent<CharacterVisualModel>();model.BindActor(unit);
            foreach(var r in physical)r.enabled=false;model.ApplyEquipment(new PrototypeState{Weapon=role.Id=="swordsman"?"training-sword":"",Armour="cloth"});
            if(role.Id=="archer"&&model.WeaponAnchor!=null&&effect!=null)
            {
                Bow(model.WeaponAnchor,new[]{new Vector3(0,-.45f,0),new Vector3(.16f,-.25f,0),new Vector3(.21f,0,0),new Vector3(.16f,.25f,0),new Vector3(0,.45f,0)},.025f,new Color(.48f,.30f,.13f),effect);
                Bow(model.WeaponAnchor,new[]{new Vector3(0,-.45f,0),new Vector3(0,.45f,0)},.008f,new Color(.78f,.74f,.62f),effect);
                var mage=catalog.Find("mage").GetComponent<HumanoidAnimationDriver>();node.GetComponent<HumanoidAnimationDriver>().Configure(node.GetComponent<HumanoidVisualRig>(),mage.Profile);
            }
            return model;
        }
        private static void Bow(Transform parent,Vector3[] points,float width,Color color,Material material)
        {var node=new GameObject("Provisional bow");node.transform.SetParent(parent,false);var line=node.AddComponent<LineRenderer>();line.sharedMaterial=material;line.useWorldSpace=false;line.positionCount=points.Length;line.SetPositions(points);line.startWidth=line.endWidth=width;line.startColor=line.endColor=color;}
    }
}
