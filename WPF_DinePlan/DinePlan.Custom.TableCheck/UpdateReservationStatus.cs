using System.ComponentModel.Composition;
using DinePlan.Custom.TableCheck.Model;
using DinePlan.Custom.TableCheck.Service;
using DinePlan.Services.Common;

namespace DinePlan.Custom.TableCheck
{
    [Export(typeof(IActionType))]
    public class UpdateReservationStatus : ActionType
    {
        private readonly ITableCheckService _tableCheckService;

        [ImportingConstructor]
        public UpdateReservationStatus(ITableCheckService tableCheckService)
        {
            _tableCheckService = tableCheckService;
        }

        public override void Process(ActionData actionData)
        {
            var refOrId = actionData.GetDataValue<string>("RefOrId");
            var reservation = actionData.GetDataValue<ReservationListModel>("Reservation");

            if (!string.IsNullOrEmpty(refOrId) && reservation != null)
            {
                _tableCheckService.UpdateReservation(refOrId, reservation);
            }
        }

        protected override object GetDefaultData()
        {
            return new { };
        }

        protected override string GetActionName()
        {
            return "UpdateReservationStatus";
        }

        protected override string GetActionKey()
        {
            return "UpdateReservationStatus";
        }
    }
}
