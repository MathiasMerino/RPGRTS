using UnityEngine;

namespace Aethernia
{
    public sealed class InteractionPoint : MonoBehaviour
    {
        [SerializeField] private string factId = "reference-chest";
        [SerializeField] private string displayName = "Cofre de prueba";
        [SerializeField] private float reach = 2.2f;
        public float Reach=>reach;
        public bool Available(GameSession game)
        {
            if(!game.Rules.UxExperience)return true;
            if(factId==LostCargoQuest.CargoId)return LostCargoQuest.Stage(game.State)==CargoQuestStage.Accepted;
            if(factId=="demo-patrol")return DemoFlow.CanVisitPatrol(game);
            if(factId==LostCargoQuest.NpcId)return true;
            if(factId=="reference-chest"||factId=="dungeon-objective"||factId=="umbral-cache"||factId=="umbral-objective"||factId.StartsWith("umbral-battle-"))return !game.HasFact(factId);
            return true; // Repeatable trade/travel/recruitment is decided by its existing controller.
        }
        public string DisplayName => displayName;
        public string FactId => factId;
        public void Configure(string id, string title) { factId = id; displayName = title; }
        public bool TryInteract(ExplorationUnit actor, GameSession session)
        {
            if (!isActiveAndEnabled || actor == null || session == null) return false;
            if (Vector3.Distance(actor.transform.position, transform.position) > reach)
            { session.Say("Acércate al objeto para interactuar."); return false; }
            return session.CompleteInteraction(factId, displayName);
        }
    }
}
