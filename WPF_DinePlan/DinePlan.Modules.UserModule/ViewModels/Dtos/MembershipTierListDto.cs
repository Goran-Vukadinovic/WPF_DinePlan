using DinePlan.Common.Model;
using System.Collections.Generic;

namespace DinePlan.Modules.UserModule.ViewModels.Dtos
{
    public class MembershipTierDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
    public class MembershipTierListDto : IOutputDto
    {
        public List<MembershipTierDto> Items { get; set; }
        public MembershipTierListDto()
        {
            Items = new List<MembershipTierDto>();
        }
    }
}
