namespace DinePlan.Common.Model
{
    public class WorkPeriodSalesInformation
    {
        public decimal TotalSales { get; set; }
        public string DepartmentSales { get; set; }
        public int TotalTicketCount { get; set; }
        public decimal AverageSale => TotalSales / TotalTicketCount;
    }
}