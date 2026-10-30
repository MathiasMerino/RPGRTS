using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    public static class PrototypeSaveStore
    {
        public static bool Valid(PrototypeState s, PrototypeRules rules)
        {
            if (s == null || s.Version != 1 || rules.Find(s.ClassId) == null || string.IsNullOrWhiteSpace(s.Name) || s.Name.Length > 32) return false;
            if (s.Gold < 0 || s.Wood < 0 || s.Food < 0 || s.Gold > 1000000 || s.Wood > 1000000 || s.Food > 1000000 || s.Health < 0 || s.Health > rules.Find(s.ClassId).Health) return false;
            if (!Finite(s.HeroPosition.x) || !Finite(s.HeroPosition.y) || !Finite(s.HeroPosition.z) || s.HeroPosition.sqrMagnitude > 1000000) return false;
            if (!Finite(s.WorldSeconds) || s.WorldSeconds < 0 || s.ProductionCycle < 0 || s.ProductionCycle > s.WorldSeconds / 10 + 1) return false;
            if (s.Facts == null || s.Items == null || s.Buildings == null || s.Facts.Count != s.Facts.Distinct().Count()) return false;
            if (s.Facts.Any(f => string.IsNullOrWhiteSpace(f) || f.Length > 128)) return false;
            if (s.Items.Any(i => i == null || !WorldOperations.KnownItem(i.Id) || i.Count <= 0) || s.Items.Sum(i => i.Count) > 8) return false;
            if (s.Items.Select(i => i.Id).Distinct().Count() != s.Items.Count) return false;
            if (s.Weapon != "" && (s.Weapon != "training-sword" || !s.Items.Any(i => i.Id == s.Weapon))) return false;
            if (s.Armour != "" && (s.Armour != "cloth" || !s.Items.Any(i => i.Id == s.Armour))) return false;
            return s.Buildings.Count <= 32 && s.Buildings.All(b => b != null && b.Id == "workshop" && Finite(b.Position.x) && Finite(b.Position.y) && Finite(b.Position.z) && Mathf.Abs(b.Position.y) <= 0.2f && Mathf.Abs(b.Position.x) <= 16 && Mathf.Abs(b.Position.z) <= 16);
        }
        private static bool Finite(float f) => !float.IsNaN(f) && !float.IsInfinity(f);
        public static bool Save(string path, PrototypeState state, PrototypeRules rules)
        {
            if (!Valid(state, rules)) return false;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
                var temp = path + ".tmp";
                File.WriteAllText(temp, JsonUtility.ToJson(state, true));
                if (File.Exists(path)) File.Replace(temp, path, null); else File.Move(temp, path);
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException) { return false; }
        }
        public static bool Load(string path, PrototypeRules rules, out PrototypeState state)
        {
            state = null;
            try
            {
                if (!File.Exists(path)) return false;
                var json = File.ReadAllText(path);
                if (!json.Contains("\"Version\"") || !json.Contains("\"ClassId\"")) return false;
                var candidate = JsonUtility.FromJson<PrototypeState>(json);
                if (!Valid(candidate, rules)) return false;
                state = candidate; return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException) { return false; }
        }
    }
}
