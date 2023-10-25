using DinePlan.Common.Model.Export;

namespace DinePlan.Common.Model.Menu
{
    public class ImportMenu : ExcelExportObject
    {
        public string Group { get; set; }
        public string Category { get; set; }
        public string AliasCode { get; set; }
        public string MenuName { get; set; }
        public string AliasName { get; set; }
        public string Tag { get; set; }
        public string Portion { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public string ImageLocation { get; set; }
        public string Description { get; set; }
        public int ForceQuantity { get; set; }
        public int ForceChangePrice { get; set; }
        public int RestrictPromotion { get; set; }
        public int NoTax { get; set; }
        public int Id { get; set; }
        public decimal Count { get; set; }
    }
}