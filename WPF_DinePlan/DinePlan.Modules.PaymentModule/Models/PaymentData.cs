using DinePlan.Domain.Models.Tickets;

namespace DinePlan.Modules.PaymentModule
{
    public class PaymentData
    {
        public ChangePaymentType ChangePaymentType { get; set; }
        public decimal PaymentDueAmount { get; set; }
        public decimal TenderedAmount { get; set; }
    }
}