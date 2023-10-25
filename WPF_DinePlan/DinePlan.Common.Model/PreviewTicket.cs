using System;

namespace DinePlan.Common.Model
{
    public class PreviewTicketInput
    {
        public int TenantId { get; set; }
        public int LocationId { get; set; }

        public PreviewTicketInput()
        {
            Page = 0;
            Count = 5;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }


    }
}
