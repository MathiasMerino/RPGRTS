using UnityEngine;

namespace Aethernia
{
    public static class SupportVisualFactory
    {
        public static CharacterVisualModel Attach(ExplorationUnit unit,RtsRoleProfile role,CharacterVisualCatalog catalog,Material effect,CivilizationCatalog civilizations=null,string raceId=null)
        {
            var physical=unit.GetComponentsInChildren<Renderer>();
            if(!CharacterVisualFactory.TryCreate(catalog,role.VisualClass,unit.transform,out var model,out _,out _))return null;var node=model.gameObject;model.BindActor(unit);
            foreach(var r in physical)if(unit.SelectionIndicator==null||!r.transform.IsChildOf(unit.SelectionIndicator.transform))r.enabled=false;model.ApplyEquipment(new PrototypeState{Weapon=role.Id=="swordsman"||role.Id=="frontier-guard"||role.Id=="stonehall-striker"?"training-sword":"",Armour="cloth"});
            if(role.Id=="archer"&&model.WeaponAnchor!=null&&effect!=null)
            {
                Bow(model.WeaponAnchor,new[]{new Vector3(0,-.45f,0),new Vector3(.16f,-.25f,0),new Vector3(.21f,0,0),new Vector3(.16f,.25f,0),new Vector3(0,.45f,0)},.025f,new Color(.48f,.30f,.13f),effect);
                Bow(model.WeaponAnchor,new[]{new Vector3(0,-.45f,0),new Vector3(0,.45f,0)},.008f,new Color(.78f,.74f,.62f),effect);
                var magePrefab=catalog.Find("mage");var mage=magePrefab==null?null:magePrefab.GetComponent<HumanoidAnimationDriver>();var driver=node.GetComponent<HumanoidAnimationDriver>();if(driver!=null&&mage!=null)driver.Configure(node.GetComponent<HumanoidVisualRig>(),mage.Profile);
            }
            RaceVisualVariant.Apply(model.transform,civilizations,raceId??civilizations?.Unit(role.Id)?.RaceId);return model;
        }
        private static void Bow(Transform parent,Vector3[] points,float width,Color color,Material material)
        {var node=new GameObject("Provisional bow");node.transform.SetParent(parent,false);var line=node.AddComponent<LineRenderer>();line.sharedMaterial=material;line.useWorldSpace=false;line.positionCount=points.Length;line.SetPositions(points);line.startWidth=line.endWidth=width;line.startColor=line.endColor=color;}
    }
}
