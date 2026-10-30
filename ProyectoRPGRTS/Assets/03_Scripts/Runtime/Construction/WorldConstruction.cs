using System.Collections.Generic;
using UnityEngine;

namespace Aethernia
{
    public sealed class WorldConstruction : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private ExplorationUnit hero;
        [SerializeField] private GameObject prefab;
        private readonly List<GameObject> placed = new List<GameObject>();
        public void Configure(GameSession game, ExplorationUnit actor, GameObject workshop) { session = game; hero = actor; prefab = workshop; }
        public bool CanPlace(Vector3 point, bool ignorePlaced = false)
        {
            if (Mathf.Abs(point.x) > 16 || Mathf.Abs(point.z) > 16 || Mathf.Abs(point.y) > 0.2f) return false;
            if (!ignorePlaced && Vector3.Distance(point, hero.transform.position) < 2) return false;
            foreach (var hit in Physics.OverlapBox(point + Vector3.up * 1.1f, new Vector3(1.55f, 1.05f, 1.55f), Quaternion.identity, ~0, QueryTriggerInteraction.Ignore))
            {
                if (ignorePlaced && hit.GetComponentInParent<ExplorationUnit>() != null) continue;
                if (ignorePlaced && placed.Exists(p => p != null && hit.transform.IsChildOf(p.transform))) continue;
                return false;
            }
            return true;
        }
        public bool Place(Vector3 point)
        {
            if (prefab == null || !CanPlace(point) || !WorldOperations.BuildWorkshop(session.State, point))
            { session.Say("Colocación inválida o recursos insuficientes. Coste: 10 oro y 5 madera."); return false; }
            placed.Add(Instantiate(prefab, point, Quaternion.identity));
            session.Say("Taller construido. Cada ciclo consume 1 alimento y produce 1 madera."); return true;
        }
        public bool CanRestore(List<BuildingRecord> buildings)
        {
            for (var i = 0; i < buildings.Count; i++)
            {
                if (!CanPlace(buildings[i].Position, true)) return false;
                for (var j = 0; j < i; j++)
                    if (Mathf.Abs(buildings[i].Position.x - buildings[j].Position.x) < 3.1f && Mathf.Abs(buildings[i].Position.z - buildings[j].Position.z) < 3.1f) return false;
            }
            return true;
        }
        public void Restore()
        {
            foreach (var go in placed) if (go != null) { go.SetActive(false); Destroy(go); }
            placed.Clear();
            foreach (var record in session.State.Buildings) placed.Add(Instantiate(prefab, record.Position, Quaternion.identity));
        }
    }
}
