using System;

namespace DinePlan.Modules.UserModule.ViewModels.Dtos
{
    public class ConnectMemberDto
    {
        public int? Id { get; set; }
        public virtual string MemberCode { get; set; }
        public virtual string Name { get; set; }
        public virtual string Locality { get; set; }
        public virtual DateTime? BirthDay { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Country { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string EmailId { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string LastName { get; set; }
        public virtual int TenantId { get; set; }
        public virtual decimal TotalPoints { get; set; }
        public virtual DateTime? LastVisitDate { get; set; }
        public int? LocationRefId { get; set; }
        public virtual int? DefaultAddressRefId { get; set; }
        public virtual int EditAddressRefId { get; set; }
        public virtual int? MembershipTierId { get; set; }

    }
}
