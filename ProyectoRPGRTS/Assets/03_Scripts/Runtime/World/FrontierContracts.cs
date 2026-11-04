using System;
using System.Linq;

namespace Aethernia
{
    public enum ContractStatus { Locked, Available, Accepted, Ready, Completed }

    public sealed class FrontierContract
    {
        public readonly string Id, Title, Objective, Requirement, Prerequisite;
        public readonly int Gold, Wood, Food, Experience;
        public string Accepted => "e22-contract-accepted:" + Id;
        public string Completed => "e22-contract-completed:" + Id;

        internal FrontierContract(string id, string title, string objective, string requirement,
            string prerequisite, int gold, int wood, int food, int experience)
        {
            Id = id; Title = title; Objective = objective; Requirement = requirement;
            Prerequisite = prerequisite; Gold = gold; Wood = wood; Food = food; Experience = experience;
        }
    }

    public static class FrontierContracts
    {
        public const string Board = "e22-board";
        public const string Cairn = "e22-boundary-stone";
        public const string Materials = "e22-recovery-crate";
        public const string Watch = "e22-ruins-watch";
        public const string WatchContract = "watch";
        private static readonly FrontierContract[] entries =
        {
            new FrontierContract("survey", "El mojón del bosque",
                "Sigue el camino occidental y examina el mojón al sur de la bifurcación.",
                Cairn, null, 7, 0, 0, 4),
            new FrontierContract("recovery", "Reserva del sendero",
                "Recupera el cajón de madera junto al alijo del sendero, al oeste del cruce.",
                Materials, null, 5, 5, 0, 4),
            new FrontierContract(WatchContract, "Última guardia del Umbral",
                "Cierra el capítulo con Tarek y derrota al centinela opcional dentro de las ruinas.",
                Watch, FrontierChapter.Finished, 10, 0, 6, 6)
        };

        public static int Count => entries.Length;
        public static FrontierContract At(int index) => entries[index];
        public static ContractStatus Status(PrototypeState state, FrontierContract contract)
        {
            if (state.Facts.Contains(contract.Completed)) return ContractStatus.Completed;
            if (state.Facts.Contains(contract.Accepted))
                return state.Facts.Contains(contract.Requirement) ? ContractStatus.Ready : ContractStatus.Accepted;
            return contract.Prerequisite != null && !state.Facts.Contains(contract.Prerequisite)
                ? ContractStatus.Locked : ContractStatus.Available;
        }

        public static bool Accept(PrototypeState state, FrontierContract contract)
        {
            if (Status(state, contract) != ContractStatus.Available) return false;
            state.Facts.Add(contract.Accepted);
            return true;
        }

        public static bool Claim(PrototypeState state, FrontierContract contract)
        {
            if (Status(state, contract) != ContractStatus.Ready || state.Gold > 1000000 - contract.Gold ||
                state.Wood > 1000000 - contract.Wood || state.Food > 1000000 - contract.Food) return false;
            state.Gold += contract.Gold;
            state.Wood += contract.Wood;
            state.Food += contract.Food;
            state.Facts.Add(contract.Completed);
            return true;
        }

        public static bool WatchAvailable(PrototypeState state) =>
            Status(state, entries[2]) == ContractStatus.Accepted;

        public static int OpenCount(PrototypeState state) =>
            entries.Count(c => Status(state, c) == ContractStatus.Accepted || Status(state, c) == ContractStatus.Ready);

        internal static bool Valid(PrototypeState state)
        {
            foreach (var contract in entries)
            {
                var accepted = state.Facts.Contains(contract.Accepted);
                if (accepted && contract.Prerequisite != null && !state.Facts.Contains(contract.Prerequisite)) return false;
                if (state.Facts.Contains(contract.Completed) && (!accepted || !state.Facts.Contains(contract.Requirement))) return false;
            }
            return (!state.Facts.Contains(Watch) || state.Facts.Contains(entries[2].Accepted)) &&
                (!state.Facts.Contains(Materials) || state.Facts.Contains(entries[1].Accepted));
        }
    }
}
