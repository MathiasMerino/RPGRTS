using UnityEngine;

namespace Aethernia
{
    public sealed class CharacterVisualPresenter : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private CharacterVisualCatalog catalog;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Renderer legacyBody;
        private bool legacyEnabled;
        private GameObject currentPrefab;
        public CharacterVisualModel Model { get; private set; }
        public string DisplayedClass { get; private set; }
        public Transform VisualRoot => visualRoot;
        public void Configure(GameSession game, CharacterVisualCatalog visuals, Transform root, Renderer legacy)
        {
            var active = enabled; enabled = false;
            session = game; catalog = visuals; visualRoot = root; legacyBody = legacy;
            enabled = active;
        }
        private void OnEnable()
        {
            if (legacyBody != null) legacyEnabled = legacyBody.enabled;
            if (session != null) { session.Changed += Refresh; Refresh(); }
        }
        private void OnDisable()
        {
            if (session != null) session.Changed -= Refresh;
            if (visualRoot != null) visualRoot.gameObject.SetActive(false);
            if (legacyBody != null) legacyBody.enabled = legacyEnabled;
        }
        private void Refresh()
        {
            if (session == null || catalog == null || visualRoot == null) return;
            var prefab = catalog.Find(session.State.ClassId);
            if (prefab == null || prefab.GetComponent<CharacterVisualModel>() == null)
            {
                visualRoot.gameObject.SetActive(false);
                if (legacyBody != null) legacyBody.enabled = legacyEnabled;
                DisplayedClass = null; return;
            }
            if (prefab != currentPrefab || Model == null)
            {
                if (Model != null) { Model.gameObject.SetActive(false); Destroy(Model.gameObject); }
                Model = Instantiate(prefab, visualRoot, false).GetComponent<CharacterVisualModel>();
                Model.BindActor(GetComponent<ExplorationUnit>());
                currentPrefab = prefab;
            }
            visualRoot.gameObject.SetActive(true);
            if (legacyBody != null) legacyBody.enabled = false;
            DisplayedClass = session.State.ClassId;
            if (Model != null) Model.ApplyEquipment(session.State);
        }
    }
}
