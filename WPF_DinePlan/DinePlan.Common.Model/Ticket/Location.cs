using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DinePlan.Common.Model.Ticket
{
    public class Location
    {

        public Location()
        {
            LocationSchedules = new List<LocationSchedule>();
        }
        public string Code { get; set; }        
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public String AddOn { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public List<LocationSchedule> LocationSchedules { get; set; }

        public string Mid
        {
            get
            {
                if (!String.IsNullOrEmpty(AddOn))
                {
                    try
                    {
                        var ao = JsonConvert.DeserializeObject<AddOn>(AddOn);
                        return ao?.Mid;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

                return "";
            }
        }

        public string ShopId
        {
            get
            {
                if (!String.IsNullOrEmpty(AddOn))
                {
                    try
                    {
                        var ao = JsonConvert.DeserializeObject<AddOn>(AddOn);
                        return ao?.ShopId;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

                return "";
            }
        }

        public string TaxReg
        {
            get
            {
                if (!String.IsNullOrEmpty(AddOn))
                {
                    try
                    {
                        var ao = JsonConvert.DeserializeObject<AddOn>(AddOn);
                        return ao?.VatReg;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

                return "";
            }
        }

        public string SoldTo
        {
            get
            {
                if (!String.IsNullOrEmpty(AddOn))
                {
                    try
                    {
                        var ao = JsonConvert.DeserializeObject<AddOn>(AddOn);
                        return ao?.SoldTo;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

                return "";
            }
        }

        public string FullTaxName
        {
            get
            {
                if (!String.IsNullOrEmpty(AddOn))
                {
                    try
                    {
                        var ao = JsonConvert.DeserializeObject<AddOn>(AddOn);
                        return ao?.FullTaxName;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

                return "";
            }
        }

        public string PlantProf
        {
            get
            {
                if (!String.IsNullOrEmpty(AddOn))
                {
                    try
                    {
                        var ao = JsonConvert.DeserializeObject<AddOn>(AddOn);
                        return ao?.PlantProf;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

                return "";
            }
        }
    }
    public class AddOn
    {
        public string Mid { get; set; }
        public string ShopId { get; set; }
        public string VatReg { get; set; }
        public string SoldTo { get; set; }
        public string FullTaxName { get; set; }
        public string PlantProf { get; set; }
    }

    public class LocationSchedule
    {
        public string Name { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public int LocationId { get; set; }
    }
}