using System.Collections.Generic;

namespace DinePlan.Common.Location
{
    public class LocationJson
    {
        public string AddOn { get; set; }
        public string Address1 { get; set; }
        public object Address2 { get; set; }
        public object Address3 { get; set; }
        public bool ChangeTax { get; set; }
        public string City { get; set; }
        public string Code { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyAddress3 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyCountry { get; set; }
        public object CompanyEmail { get; set; }
        public string CompanyName { get; set; }
        public object CompanyPhoneNumber { get; set; }
        public object CompanyState { get; set; }
        public string CompanyTaxCode { get; set; }
        public object CompanyWebsite { get; set; }
        public string Country { get; set; }
        public object Email { get; set; }
        public int Id { get; set; }
        public List<object> LocationSchedules { get; set; }
        public object LocationTaxCode { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public object State { get; set; }
        public bool TaxInclusive { get; set; }
        public object Website { get; set; }
    }
}
