namespace Aethernia
{
    public enum FrontierStep { Arrival, Trail, Supply, Patrol, Ruins, Debrief, Conflict, Return, Free }
    public enum RoadChoice { None, Report, Repair }

    // Proyección del diario y transacciones de una misión sobre la partida compartida.
    public static class FrontierChapter
    {
        public const string Title = "Las rutas de la frontera";
        public const string Sign = "ux02-crossroads", Merchant = "ux02-merchant", Liaison = "ux02-recruiter";
        public const string Clue = "e21-road-clue", Report = "e21-road-report", Repair = "e21-road-repair";
        public const string Dossier = "e21-dossier-delivered", Finished = "e21-chapter-complete", Independent = "e21-independent";
        public const int RepairCost = 5, RepairWood = 6, ReportFood = 3, ReportReputation = 5;
        private const int ResourceLimit = 1000000;

        public static RoadChoice Decision(PrototypeState s) =>
            s.Facts.Contains(Report) ? RoadChoice.Report : s.Facts.Contains(Repair) ? RoadChoice.Repair : RoadChoice.None;

        private static bool CanInvestigate(PrototypeState s) =>
            s.Facts.Contains(LostCargoQuest.Completed) || ExpeditionOperations.Ready(s);

        public static bool ReadClue(PrototypeState s)
        {
            if (!CanInvestigate(s) || s.Facts.Contains(Clue)) return false;
            s.Facts.Add(Clue);
            return true;
        }

        public static bool ChooseRoad(PrototypeState s, RoadChoice choice)
        {
            if (!CanInvestigate(s) || !s.Facts.Contains(Clue) || Decision(s) != RoadChoice.None) return false;
            switch (choice)
            {
                case RoadChoice.Report:
                    if (s.Food > ResourceLimit - ReportFood || s.ReputationA > ResourceLimit - ReportReputation) return false;
                    s.Food += ReportFood;
                    s.ReputationA += ReportReputation;
                    s.Facts.Add(Report);
                    return true;
                case RoadChoice.Repair:
                    if (s.Gold < RepairCost || s.Wood > ResourceLimit - RepairWood) return false;
                    s.Gold -= RepairCost;
                    s.Wood += RepairWood;
                    s.Facts.Add(Repair);
                    return true;
                default: return false;
            }
        }

        public static bool Debrief(PrototypeState s)
        {
            if (!ExpeditionOperations.Ready(s) || !s.Facts.Contains("umbral-reward") || s.Facts.Contains(Dossier))
                return false;
            s.Facts.Add(Dossier);
            return true;
        }

        public static bool ConflictResolved(PrototypeState s) =>
            s.Facts.Contains("umbral-outpost-conquered") || s.Facts.Contains(CivilizationOperations.Pact);

        public static bool Finish(PrototypeState s)
        {
            if (!s.Facts.Contains(Dossier) || s.Facts.Contains(Finished)) return false;
            if (!ConflictResolved(s)) s.Facts.Add(Independent);
            s.Facts.Add(Finished);
            return true;
        }

        public static FrontierStep Step(PrototypeState s)
        {
            if (s.Facts.Contains(Finished)) return FrontierStep.Free;
            if (!ExpeditionOperations.Ready(s))
            {
                if (!CanInvestigate(s)) return FrontierStep.Arrival;
                if (s.Demo?.Chapter != DemoChapter.FirstRts)
                {
                    if (!s.Facts.Contains(Clue)) return FrontierStep.Trail;
                    if (Decision(s) == RoadChoice.None) return FrontierStep.Supply;
                }
                return FrontierStep.Patrol;
            }
            if (!s.Facts.Contains("umbral-reward")) return FrontierStep.Ruins;
            if (!s.Facts.Contains(Dossier)) return FrontierStep.Debrief;
            return ConflictResolved(s) ? FrontierStep.Return : FrontierStep.Conflict;
        }

        public static string Heading(FrontierStep step)
        {
            switch (step)
            {
                case FrontierStep.Arrival: return "I · Llegada";
                case FrontierStep.Trail:
                case FrontierStep.Supply:
                case FrontierStep.Patrol: return "II · El camino interrumpido";
                case FrontierStep.Ruins:
                case FrontierStep.Debrief: return "III · Las ruinas";
                case FrontierStep.Conflict: return "IV · El conflicto militar";
                default: return "V · Nuevos caminos";
            }
        }

        public static string Objective(PrototypeState s, bool inside = false)
        {
            switch (Step(s))
            {
                case FrontierStep.Arrival: return LostCargoQuest.Objective(s);
                case FrontierStep.Trail: return "Lee y anota la señal del cruce.";
                case FrontierStep.Supply: return "Habla con Elia y decide cómo ayudar a la ruta.";
                case FrontierStep.Patrol: return "Habla con el capitán y prepara la patrulla.";
                case FrontierStep.Ruins:
                    if (!inside) return "Ve a las ruinas. Puedes reclutar apoyo en el campamento.";
                    if (!s.Facts.Contains("umbral-battle-1")) return "Despeja la primera guardianía.";
                    if (!s.Facts.Contains("umbral-battle-2")) return "Despeja la segunda guardianía.";
                    return "Examina el objetivo de las ruinas.";
                case FrontierStep.Debrief: return inside ? "Sal de las ruinas con el hallazgo." : "Lleva el hallazgo a Tarek, en el campamento.";
                case FrontierStep.Conflict: return "Resuelve el puesto, pacta o vuelve con Tarek para seguir tu camino.";
                case FrontierStep.Return: return "Vuelve con Tarek para cerrar el capítulo.";
                default: return "Explora, comercia, recluta o desarrolla el asentamiento.";
            }
        }

        public static string Journal(PrototypeState s, bool inside)
        {
            var decision = Decision(s);
            var path = decision == RoadChoice.Report ? "Informe entregado · víveres y reputación" :
                decision == RoadChoice.Repair ? "Reparación financiada · madera" : "Decisión de ruta disponible tras investigar";
            var conflict = s.Facts.Contains("umbral-outpost-conquered") ? "Puesto conquistado" :
                s.Facts.Contains(CivilizationOperations.Pact) ? "Pacto vigente" : "Puesto hostil";
            var ruins = s.Facts.Contains("umbral-reward") ? "Ruinas completadas" :
                ExpeditionOperations.Ready(s) ? "Ruinas disponibles" : "Ruinas bloqueadas hasta asegurar el campamento";
            return Heading(Step(s)) + " · " + Title + "\n" +
                "Principal · " + Objective(s, inside) + "\n\n" +
                "Cargamento · " + LostCargoQuest.Objective(s) + "\n" +
                "Cruce · " + path + "\n" + ruins + " · " + conflict + "\n" +
                (s.Facts.Contains(Independent) ? "Capítulo cerrado por cuenta propia. " : "") +
                "Secundarias · alijos, materiales, comercio, grupo y asentamiento.";
        }

        internal static bool Valid(PrototypeState s)
        {
            var report = s.Facts.Contains(Report);
            var repair = s.Facts.Contains(Repair);
            var dossier = s.Facts.Contains(Dossier);
            var finished = s.Facts.Contains(Finished);
            return !(report && repair) &&
                (!(report || repair) || s.Facts.Contains(Clue)) &&
                (!s.Facts.Contains(Clue) || CanInvestigate(s)) &&
                (!dossier || (ExpeditionOperations.Ready(s) && s.Facts.Contains("umbral-reward"))) &&
                (!finished || (dossier && (ConflictResolved(s) || s.Facts.Contains(Independent)))) &&
                (!s.Facts.Contains(Independent) || finished);
        }
    }
}
