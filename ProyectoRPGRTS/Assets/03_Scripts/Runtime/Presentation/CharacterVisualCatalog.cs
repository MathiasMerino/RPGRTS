using System;
using UnityEngine;

namespace Aethernia
{
    [CreateAssetMenu(menuName = "Aethernia/Character Visual Catalog")]
    public sealed class CharacterVisualCatalog : ScriptableObject
    {
        [Serializable] public sealed class Entry
        {
            public string ClassId;
            public GameObject Prefab;
            public GameObject HumanoidPrefab;
            [Tooltip("Referencia revisada; no activa automáticamente FBX experimentales.")]
            public GameObject ReplacementPrefab;
            public bool UseReplacement;
            public bool ReplacementRequiresAnimation=true;
            public Texture2D ReferenceArt;
            public Rect ReferenceUv = new Rect(0, 0, 1, 1);
            public string ReferenceCaption = "Referencia 2D provisional";
            public string DisplayTitle;
            public Sprite PortraitSprite;
            public Texture2D ConceptSheet;
            public GameObject PreviousPrefab;
            public Material[] SharedMaterials = Array.Empty<Material>();
            public float ReferenceHeightMeters = 1.8f;
            public bool IsProvisional = true;
            public string EquipmentPolicy;
        }
        public Entry[] Entries = Array.Empty<Entry>();
        public Entry FindEntry(string id)
        { foreach (var entry in Entries) if (entry.ClassId == id) return entry; return null; }
        public GameObject Candidate(Entry entry,int index,out bool requiresAnimation)
        {
            requiresAnimation=index==1||(index==0&&entry.ReplacementRequiresAnimation);
            switch(index){case 0:return entry.UseReplacement?entry.ReplacementPrefab:null;case 1:return entry.HumanoidPrefab;case 2:return entry.Prefab;case 3:return entry.PreviousPrefab;default:return null;}
        }
        public GameObject Find(string id)
        {
            var entry=FindEntry(id);if(entry==null)return null;
            for(var i=0;i<4;i++){var prefab=Candidate(entry,i,out var animated);if(CharacterVisualContract.CanUse(prefab,animated,out _))return prefab;}
            return null;
        }
    }
}
