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
        }
        public Entry[] Entries = Array.Empty<Entry>();
        public GameObject Find(string id)
        { foreach (var entry in Entries) if (entry.ClassId == id) return entry.Prefab; return null; }
    }
}
