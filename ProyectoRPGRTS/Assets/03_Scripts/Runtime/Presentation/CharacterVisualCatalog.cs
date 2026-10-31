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
        public GameObject Find(string id)
        {
            foreach (var entry in Entries)
                if (entry.ClassId == id)
                {
                    var rig = entry.HumanoidPrefab == null ? null : entry.HumanoidPrefab.GetComponent<HumanoidVisualRig>();
                    return rig != null && rig.IsUsable ? entry.HumanoidPrefab : entry.Prefab;
                }
            return null;
        }
    }
}
