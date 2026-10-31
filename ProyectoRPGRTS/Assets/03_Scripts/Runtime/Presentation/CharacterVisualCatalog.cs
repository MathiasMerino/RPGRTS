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
        }
        public Entry[] Entries = Array.Empty<Entry>();
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
