using DinePlan.Common.App;
using DinePlan.Common.POS;
using DinePlan.Custom.TableCheck.Dialog;
using DinePlan.Custom.TableCheck.Model;
using DinePlan.Custom.TableCheck.Service;
using DinePlan.Domain.Models.Reserve;
using DinePlan.Services;
using DinePlan.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinePlan.Custom.TableCheck.Action
{
    [Export(typeof(IActionType))]
    public class TableCheckSendingSaleAction : ActionType
    {
        ITableCheckService _tableCheckService;
        IReserveService _reserveService;

        [ImportingConstructor]
        public TableCheckSendingSaleAction(ITableCheckService tableCheckService,
            IReserveService reserveService)
        {
            _tableCheckService = tableCheckService;
            _reserveService = reserveService;
        }


        private ChooseReservationViewModel _chooseReservationViewModel;
        public override void Process(ActionData actionData)
        {
            var ticket = actionData.GetDataValue<Domain.Models.Tickets.Ticket>("Ticket");
            var status = actionData.GetAsString("ReservationStatus");
            if (string.IsNullOrEmpty(status)) status = TableCheckReservationStatus.occupied.ToString();

            if (ticket == null || string.IsNullOrEmpty(ticket.TicketNumber)) return;

            var reservationId = ticket.GetTicketTagValue(PosConsts.ReservationId);

            Domain.Models.Reserve.Reservation reversation = null;
            if (string.IsNullOrEmpty(reservationId))
            {
                reversation = ChooseReservationViewModel.ShowDialog();
            } else
            {
                reversation = _reserveService.GetReservations(reservationId);
            }
            // Show Reservation dialog
            
            if (reversation != null)
            {
                // Update status
                var posJournal = new Model.PosJournalInputModel()
                {
                    ReceiptNum = ticket.TicketNumber,
                    ReservationRef = reversation.RemoteId,
                    ReservationStatus = status,
                    TableNames = JsonConvert.DeserializeObject<Model.Reservation>(reversation.OtherDetails).TableNames,
                    SettleAmt = ticket.TotalAmount.ToString(),
                    ChangeAmt = ticket.GetChangeAmount().ToString(),
                    SubtotalAmt = ticket.GetSubTotal().ToString(),
                    OrderAt = ticket.LastOrderDate.ToUniversalTime(),
                    PaymentAt = ticket.LastPaymentDate.ToUniversalTime(),
                    Pax = Convert.ToInt32(reversation.Pax),
                };
                //posJournal.PosDiscounts = new List<PosDiscount>();
                //posJournal.PosOrders = new List<PosOrder>();
                //posJournal.PosPayments = new List<PosPayment>();

                //ticket.PromotionDetailValues.ToList().ForEach(x => posJournal.PosDiscounts.Add(new PosDiscount()
                //{
                //    Id = x.Id.ToString(),
                //    Amt = x.PromotionAmount.ToString(),
                //    Name = x.PromotionName
                //}));
                //ticket.Orders.ToList().ForEach(x => posJournal.PosOrders.Add(new PosOrder()
                //{
                //    Id = x.Id.ToString(),
                //    MenuItemName = x.MenuItemName,
                //    Qty = x.Quantity,
                //    UnitPrice = x.Price.ToString(),
                //    OrderAt = x.CreatedDateTime.ToUniversalTime(),
                //}));
                //ticket.Payments.ToList().ForEach(x => posJournal.PosPayments.Add(new PosPayment()
                //{
                //    Id = x.Id.ToString(),
                //    Amt = x.Amount.ToString(),
                //    Tender = x.TenderedAmount.ToString(),
                //    PaymentAt = x.Date.ToUniversalTime()
                //}));
                var postJournal = _tableCheckService.CreatePosJournal(posJournal);         

                // Update reserveration status
                if (postJournal != null)
                {
                    _reserveService.UpdateReservation(reversation.Id, (int)ReservationStatus.Accepted);

                    ticket.SetTagValue(1, PosConsts.ReservationId, reversation.RemoteId);

                }
            }
        }

        public ChooseReservationViewModel ChooseReservationViewModel => _chooseReservationViewModel ?? (_chooseReservationViewModel = ServiceLocator.Current.GetInstance<ChooseReservationViewModel>());

        protected override string GetActionKey()
        {
            return "Table Check Send Sales";
        }

        protected override string GetActionName()
        {
            return "Table Check Send Sales";
        }

        protected override object GetDefaultData()
        {
            return new { ReservationStatus = "paid" };
        }

        private enum TableCheckReservationStatus
        {
            occupied,
            bill_printed,
            paid,
            cleaning,
            vacant
        }
    }
}
