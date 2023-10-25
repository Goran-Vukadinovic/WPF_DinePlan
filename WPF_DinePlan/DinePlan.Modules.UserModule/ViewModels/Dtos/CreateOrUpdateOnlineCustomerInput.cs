using DinePlan.Common.Model;
using DinePlan.Common.Model.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinePlan.Modules.UserModule.ViewModels.Dtos
{
    public class FrontSignupUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int TenantId { get; set; }
        public string WhatsappNumber { get; set; }
        public string ExtraData { get; set; }
    }

    public class FrontSignupComplete: FrontSignupUser
    {
        public int Id { get; set; }
        public int? GradeId { get; set; }
        public int? SectionId { get; set; }
        public string Code { get; set; }
        public string AlternateCode { get; set; }
    }

    public class FrontSignupUserOutput
    {
        public int UserId { get; set; }
        public int MemberId { get; set; }
    }
    public enum OnlineCustomerGender
    {
        Male = 0,
        Female = 1,
        Other = 2
    }
    public enum OnlineOnboardType
    {
        Wheel = 0,
        Clique = 1,
        Tiffin = 2
    }
    public class ParentListDto : IOutputDto
    {
        public List<ComboboxItemObjectDto> Items { get; set; }
        public ParentListDto()
        {
            Items = new List<ComboboxItemObjectDto>();
        }
    }
    public class ExtraDataDto
    {
        public string FatherName { get; set; }
        public string FatherMobile { get; set; }
        public string MotherMobile { get; set; }
        public string MotherName { get; set; }
    }
}
