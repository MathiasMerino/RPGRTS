using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    public static class PrototypeSaveStore
    {
        public static string LastWriteFailure { get; private set; } = "";
        public static bool Valid(PrototypeState s, PrototypeRules rules)
        {
            if (s == null || s.Version != 1 || rules.Find(s.ClassId) == null || string.IsNullOrWhiteSpace(s.Name) || s.Name.Length > 32) return false;
            if (!string.IsNullOrEmpty(s.RaceId) && !DemoContent.KnownLineage(s.RaceId)) return false;
            if (s.Demo != null && (!Enum.IsDefined(typeof(DemoChapter),s.Demo.Chapter) || s.Demo.Produced<0 || s.Demo.Produced>1000000)) return false;
            if (s.Gold < 0 || s.Wood < 0 || s.Food < 0 || s.Gold > 1000000 || s.Wood > 1000000 || s.Food > 1000000 || s.Health < 0 || s.Health > rules.Find(s.ClassId).Health) return false;
            if (!Finite(s.HeroPosition.x) || !Finite(s.HeroPosition.y) || !Finite(s.HeroPosition.z) || s.HeroPosition.sqrMagnitude > 1000000) return false;
            if (!Finite(s.WorldSeconds) || s.WorldSeconds < 0 || s.WorldSeconds / 10 >= int.MaxValue || s.ProductionCycle != Mathf.FloorToInt(s.WorldSeconds / 10)) return false;
            if (s.Facts == null || s.Items == null || s.Buildings == null || s.Facts.Count != s.Facts.Distinct().Count()) return false;
            if (s.Facts.Any(f => string.IsNullOrWhiteSpace(f) || f.Length > 128)) return false;
            if (s.Items.Any(i => i == null || !WorldOperations.KnownItem(i.Id) || i.Count <= 0 || i.Count > 8) || s.Items.Sum(i => (long)i.Count) > 8) return false;
            if (s.Items.Select(i => i.Id).Distinct().Count() != s.Items.Count) return false;
            if (s.Weapon != "" && (s.Weapon != "training-sword" || !s.Items.Any(i => i.Id == s.Weapon))) return false;
            if (s.Armour != "" && (s.Armour != "cloth" || !s.Items.Any(i => i.Id == s.Armour))) return false;
            if(s.Army==null||s.Army.Count>6||s.Army.Any(r=>r==null||string.IsNullOrEmpty(r.Id)||r.Id.Length!=32||!r.Id.All(Uri.IsHexDigit)||!SettlementRules.KnownRole(r.Role)||r.Health<=0||r.Health>10)||s.Army.Select(r=>r.Id).Distinct().Count()!=s.Army.Count)return false;
            return s.Buildings.Count <= 32 && s.Buildings.All(b => b != null && SettlementRules.KnownBuilding(b.Id) && (s.Demo!=null||b.Id=="workshop") && b.Level>=1&&b.Level<=2 && Finite(b.Position.x) && Finite(b.Position.y) && Finite(b.Position.z) && Mathf.Abs(b.Position.y) <= 0.2f && Mathf.Abs(b.Position.x) <= 16 && Mathf.Abs(b.Position.z) <= 16);
        }
        private static bool Finite(float f) => !float.IsNaN(f) && !float.IsInfinity(f);
        public static bool Save(string path, PrototypeState state, PrototypeRules rules)
        {
            LastWriteFailure = "";
            if (!Valid(state, rules)) { LastWriteFailure = "Invalid state"; return false; }
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
                var temp = path + ".tmp";
                File.WriteAllText(temp, JsonUtility.ToJson(state, true));
                if (File.Exists(path)) File.Replace(temp, path, null); else File.Move(temp, path);
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException)
            { LastWriteFailure = e.GetType().Name + " 0x" + e.HResult.ToString("X8") + ": " + e.Message; return false; }
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
                if (candidate != null && string.IsNullOrEmpty(candidate.RaceId)) candidate.RaceId="human";
                if(candidate!=null)
                {
                    if(candidate.Army==null)candidate.Army=new System.Collections.Generic.List<RecruitRecord>();
                    if(!json.Contains("\"Level\"")&&candidate.Buildings!=null)foreach(var b in candidate.Buildings)if(b!=null)b.Level=1;
                }
                if (!Valid(candidate, rules)) return false;
                state = candidate; return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException) { return false; }
        }
    }
}
