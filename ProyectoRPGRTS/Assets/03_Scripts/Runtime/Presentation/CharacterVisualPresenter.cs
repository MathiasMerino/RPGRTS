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
        private GameObject currentPrefab,rejectedPrefab;
        public CharacterVisualCatalog Catalog=>catalog;
        public Renderer LegacyBody=>legacyBody;
        public string LastFailureReason {get;private set;}
        public CharacterVisualModel Model { get; private set; }
        public string DisplayedClass { get; private set; }
        public Transform VisualRoot => visualRoot;
        public void Configure(GameSession game, CharacterVisualCatalog visuals, Transform root, Renderer legacy)
        {
            var active = enabled; enabled = false;
            rejectedPrefab=null;session = game; catalog = visuals; visualRoot = root; legacyBody = legacy;
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
        private bool KeepCurrent()
        {
            if(Model==null||DisplayedClass!=session.State.ClassId||!CharacterVisualContract.CanUse(Model.gameObject,false,out _))return false;
            visualRoot.gameObject.SetActive(true);if(legacyBody!=null)legacyBody.enabled=false;Model.ApplyEquipment(session.State);RaceVisualVariant.Apply(Model.transform,session.Rules.Civilizations,session.State.RaceId);return true;
        }
        private void Refresh()
        {
            if (session == null || catalog == null || visualRoot == null) return;
            var prefab = catalog.Find(session.State.ClassId);
            if(prefab==null)
            {
                if(KeepCurrent())return;
                visualRoot.gameObject.SetActive(false);if(legacyBody!=null)legacyBody.enabled=legacyEnabled;DisplayedClass=null;return;
            }
            if((prefab!=currentPrefab&&prefab!=rejectedPrefab)||Model==null)
            {
                if(!CharacterVisualFactory.TryCreate(catalog,session.State.ClassId,visualRoot,out var next,out var source,out var failure))
                {
                    LastFailureReason=failure;rejectedPrefab=prefab;
                    if(KeepCurrent())return;
                    visualRoot.gameObject.SetActive(false);if(legacyBody!=null)legacyBody.enabled=legacyEnabled;DisplayedClass=null;return;
                }
                next.BindActor(GetComponent<ExplorationUnit>());
                var old=Model;Model=next;currentPrefab=source;rejectedPrefab=source==prefab?null:prefab;LastFailureReason=failure;
                if(old!=null){old.gameObject.SetActive(false);Destroy(old.gameObject);}
            }
            visualRoot.gameObject.SetActive(true);
            if (legacyBody != null) legacyBody.enabled = false;
            DisplayedClass = session.State.ClassId;
            if (Model != null) {Model.ApplyEquipment(session.State);RaceVisualVariant.Apply(Model.transform,session.Rules.Civilizations,session.State.RaceId);}
        }
    }
}
