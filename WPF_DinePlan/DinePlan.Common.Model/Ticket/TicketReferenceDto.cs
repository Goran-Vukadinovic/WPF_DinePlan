namespace DinePlan.Common.Model.Ticket
{
    public class TicketReferenceDto
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public bool NewInvoice { get; set; }
        public string InvoiceNumber { get; set; }
        public string OldNumber { get; set; }
    }
}