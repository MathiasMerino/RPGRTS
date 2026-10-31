namespace Aethernia
{
    public enum CargoQuestStage { Available, Accepted, Recovered, Completed }
    // One provisional quest over the existing Facts/inventory; no new save schema or quest manager.
    public static class LostCargoQuest
    {
        public const string Title="El cargamento perdido";
        public const string Accepted="lost-cargo-accepted",Recovered="lost-cargo-recovered",Completed="lost-cargo-completed";
        public const string NpcId="lost-cargo-npc",CargoId="lost-cargo-crate";
        public const int RewardGold=20;
        public static CargoQuestStage Stage(PrototypeState s)=>s.Facts.Contains(Completed)?CargoQuestStage.Completed:s.Facts.Contains(Recovered)&&s.Facts.Contains(Accepted)?CargoQuestStage.Recovered:s.Facts.Contains(Accepted)?CargoQuestStage.Accepted:CargoQuestStage.Available;
        public static bool Accept(PrototypeState s)
        {if(Stage(s)!=CargoQuestStage.Available)return false;s.Facts.Add(Accepted);return true;}
        public static bool Secure(PrototypeState s)
        {if(Stage(s)!=CargoQuestStage.Accepted)return false;s.Facts.Add(Recovered);return true;}
        public static bool Claim(PrototypeState s,out bool rationAdded)
        {
            rationAdded=false;if(Stage(s)!=CargoQuestStage.Recovered||s.Gold>1000000-RewardGold)return false;
            rationAdded=WorldOperations.AddItem(s,"ration",1);s.Gold+=RewardGold;s.Facts.Add(Completed);return true;
        }
        public static string Objective(PrototypeState s)
        {
            var stage=Stage(s);return stage==CargoQuestStage.Available?"Habla con el habitante junto al camino.":stage==CargoQuestStage.Accepted?"Busca el arcón al oeste de la casa y recupera la carga.":stage==CargoQuestStage.Recovered?"Regresa al habitante para entregar la carga.":"Completada · recompensa entregada.";
        }
    }
}
