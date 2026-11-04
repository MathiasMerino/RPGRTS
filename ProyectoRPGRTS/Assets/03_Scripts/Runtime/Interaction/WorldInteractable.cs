using UnityEngine;

namespace Aethernia
{
    public enum WorldInteractionKind { Sign, Chest, Materials, Merchant, Recruiter, Guard, Door, Store }
    [RequireComponent(typeof(InteractionPoint))]
    public sealed class WorldInteractable : MonoBehaviour
    {
        public WorldInteractionKind Kind;
        [TextArea] public string Description;
        public string Action = "Examinar";
        public bool Consumed(GameSession session) =>
            (Kind == WorldInteractionKind.Chest || Kind == WorldInteractionKind.Materials) && session.HasFact(GetComponent<InteractionPoint>().FactId);
    }
}
