namespace Aethernia
{
    public sealed partial class WorldExploration
    {
        private bool TryLivingDialogue(InteractionPoint point, out bool handled)
        {
            var state = Flow.Session.State;
            switch (point.FactId)
            {
                case FrontierContracts.Board:
                    handled = ShowContract(point, 0);
                    return true;
                case FrontierContracts.Cairn:
                    handled = Show(point, "Mojón del bosque",
                        "La marca indica el regreso a la aldea por el sendero sur. Puedes registrar el hallazgo en tu mapa.",
                        state.Facts.Contains(FrontierContracts.Cairn) ? null : "Anotar hallazgo",
                        () => RecordSite(FrontierContracts.Cairn, "Mojón del bosque registrado. Consulta el tablón."), "mage");
                    return true;
                case FrontierContracts.Materials:
                    var contract = FrontierContracts.At(1);
                    var status = FrontierContracts.Status(state, contract);
                    handled = Show(point, "Cajón de la reserva",
                        status == ContractStatus.Available ? "El precinto de la guardia sigue intacto. Busca el encargo en el tablón de la aldea." :
                        status == ContractStatus.Accepted ? "La marca coincide con el encargo. Recupera la reserva y vuelve al tablón." :
                        "Ya recuperaste la reserva de este cajón.",
                        status == ContractStatus.Accepted ? "Recuperar reserva" : null,
                        () => RecordSite(FrontierContracts.Materials, "Reserva recuperada. Vuelve al tablón para entregarla."), "warrior");
                    return true;
                default:
                    handled = false;
                    return false;
            }
        }

        private void RecordSite(string fact, string message)
        {
            var game = Flow.Session;
            if (game.HasFact(fact)) return;
            game.State.Facts.Add(fact);
            ReportAndSave(message);
        }

        private bool ShowContract(InteractionPoint point, int index, string notice = "")
        {
            var game = Flow.Session;
            var contract = FrontierContracts.At(index);
            var status = FrontierContracts.Status(game.State, contract);
            var detail = status == ContractStatus.Completed ? "Entregado" : status == ContractStatus.Ready ? "Listo para entregar" :
                status == ContractStatus.Accepted ? "En curso" : status == ContractStatus.Locked ? "Requiere cerrar el capítulo" : "Disponible";
            var reward = contract.Gold + " oro" + (contract.Wood > 0 ? " · " + contract.Wood + " madera" : "") +
                (contract.Food > 0 ? " · " + contract.Food + " alimento" : "") + " · hasta " + contract.Experience + " XP";
            return Show(point, "Tablón de la aldea · " + (index + 1) + "/" + FrontierContracts.Count,
                contract.Title + " · " + detail + "\n" + contract.Objective + "\nRecompensa: " + reward + "\n" + notice,
                status == ContractStatus.Available ? "Aceptar encargo" : status == ContractStatus.Ready ? "Entregar encargo" : null,
                () =>
                {
                    string message;
                    if (status == ContractStatus.Available)
                        message = FrontierContracts.Accept(game.State, contract) ? "Encargo aceptado." : "El encargo ya cambió de estado.";
                    else if (FrontierContracts.Claim(game.State, contract))
                    {
                        var xp = game.AwardForFact(contract.Completed);
                        message = "Recompensa entregada · +" + xp + " XP.";
                    }
                    else
                    {
                        ShowContract(point, index, "No se entregó la recompensa. Comprueba tus reservas.");
                        return;
                    }
                    if(game.Rules.IntegratedAlpha)Flow.Front.Audio?.Play(FeedbackSound.Quest,UnityEngine.Vector3.zero,true);
                    ReportAndSave(message);
                    ShowContract(point, index, message);
                }, "warrior", "Siguiente", () => ShowContract(point, (index + 1) % FrontierContracts.Count));
        }

        private bool ShowMarket(InteractionPoint point, int page, string notice = "")
        {
            var game = Flow.Session;
            var state = game.State;
            string product, button;
            System.Func<bool> trade;
            switch (page)
            {
                case 0:
                    product = "Madera para construcción · 2 unidades por 5 oro.";
                    button = "Comprar · 5 oro"; trade = () => WorldOperations.Trade(state);
                    break;
                case 1:
                    product = "Ración · recupera hasta 4 PV al usarla. Precio " + WorldOperations.RationPrice + " oro.";
                    button = "Comprar ración"; trade = () => WorldOperations.BuyRation(state);
                    break;
                case 2:
                    var price = ItemCatalog.Find("ration", game.Rules.Progression?.Items).SellGold;
                    product = "Elia compra una ración de tu bolsa por " + price + " oro.";
                    button = "Vender ración"; trade = () => EquipmentOperations.Sell(state, "ration", game.Rules);
                    break;
                default:
                    product = "Chaleco reforzado · 8 oro y 4 madera. Requiere un taller construido.";
                    button = "Comprar chaleco"; trade = () => EquipmentOperations.BuyVest(state);
                    break;
            }
            var stock = state.Items.Find(i => i.Id == "ration")?.Count ?? 0;
            return Show(point, "Elia · comercio " + (page + 1) + "/4",
                product + "\nBolsa: " + state.Gold + " oro · " + state.Wood + " madera · " + stock + " raciones\n" + notice,
                button, () =>
                {
                    var message = trade() ? "Intercambio realizado." : "Sin intercambio. Revisa precio, requisitos y espacio.";
                    if (message == "Intercambio realizado.")
                    {
                        if(game.Rules.IntegratedAlpha)Flow.Front.Audio?.Play(FeedbackSound.Confirm,UnityEngine.Vector3.zero,true);
                        ReportAndSave(message);
                    }
                    ShowMarket(point, page, message);
                }, "monk", "Siguiente", () => ShowMarket(point, (page + 1) % 4));
        }

        private void ReportAndSave(string message)
        {
            var saved = Flow.Director.Save(Flow.SlotPath);
            Flow.Session.Say(saved ? message : message + " Guardado pendiente.");
        }

        public bool TryContractGuidance(bool inside,out string objective,out UnityEngine.Vector3 position)
        {
            var state=Flow.Session.State;
            foreach(var priority in guidancePriority)
                for(var i=0;i<FrontierContracts.Count;i++)
                {
                    var contract=FrontierContracts.At(i);
                    if(FrontierContracts.Status(state,contract)!=priority)continue;
                    var ready=priority==ContractStatus.Ready;
                    var id=ready?(inside?"umbral-exit":FrontierContracts.Board):
                        contract.Requirement==FrontierContracts.Watch&&!inside?"umbral-entry":
                        inside&&contract.Requirement!=FrontierContracts.Watch?"umbral-exit":contract.Requirement;
                    if(!landmarks.TryGetValue(id,out var point))continue;
                    position=point.transform.position;
                    objective=contract.Title+"\n"+(id=="umbral-exit"?"Sal de las ruinas.":ready?"Entrega el encargo en el tablón.":point.DisplayName);
                    return true;
                }
            position=default;
            objective=null;
            return false;
        }
        private static readonly ContractStatus[] guidancePriority={ContractStatus.Ready,ContractStatus.Accepted};
    }
}
