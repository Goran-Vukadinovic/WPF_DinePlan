using System;
using System.Collections.Generic;

namespace DinePlan.Common.Model.User
{
    public class SearchUserInput : IInputDto
    {
        public int Id { get; set; }
        public string Filter { get; set; }
        public int TenantId { get; set; }
    }

    public class GetUserRolesNameInput : IInputDto
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
    }

    public class DinePlanUserListDto : IOutputDto
    {
        public List<DinePlanUserOutputDto> Items { get; set; }
    }

    public class DinePlanUserOutputDto
    {
        public virtual int Id { get; set; }
        public virtual string Code { get; set; }
        public virtual string PinCode { get; set; }
        public virtual string Name { get; set; }
        public virtual string SecurityCode { get; set; }
        public virtual string ConnectUser { get; set; }
        public virtual string ConnectPassword { get; set; }
        public virtual string LanguageCode { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual int DinePlanUserRoleId { get; set; }
        public virtual string DinePlanUserRoleName { get; set; }
        public string AlternateLanguageCode { get; set; }
    }

    public class GetDinePlanUserRoleOutput : IOutputDto
    {
        public GetDinePlanUserRoleForEditOutput dinePlanUserRole { get; set; }
        public object permissions { get; set; }
        public List<string> grantedPermissionNames { get; set; }
    }

    public class GetDinePlanUserRoleOutputList : IOutputDto
    {
        public List<GetDinePlanUserRoleOutput> items { get; set; }
    }

    public class GetDinePlanUserRoleForEditOutput
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isAdmin { get; set; }
        public int? departmentId { get; set; }
    }

    public class ConnectUserListDto : IOutputDto
    {
        public List<ConnectUserDto> Items { get; set; }
    }
    //public class ConnectMemberListDto : IOutputDto
    //{
    //    public List<ConnectMemberDto> Items { get; set; }
    //    public ConnectMemberListDto()
    //    {
    //        Items = new List<ConnectMemberDto>();
    //    }
    //}
    //public class ConnectMemberDto
    //{
    //    public int? Id { get; set; }
    //    public virtual string MemberCode { get; set; }
    //    public virtual string Name { get; set; }
    //    public virtual string Locality { get; set; }
    //    public virtual DateTime? BirthDay { get; set; }
    //    public virtual string Address { get; set; }
    //    public virtual string City { get; set; }
    //    public virtual string State { get; set; }
    //    public virtual string Country { get; set; }
    //    public virtual string PostalCode { get; set; }
    //    public virtual string EmailId { get; set; }
    //    public virtual string PhoneNumber { get; set; }
    //    public virtual string LastName { get; set; }
    //    public virtual int TenantId { get; set; }
    //    public virtual decimal TotalPoints { get; set; }
    //    public virtual DateTime? LastVisitDate { get; set; }
    //    public int? LocationRefId { get; set; }
    //    public virtual int? DefaultAddressRefId { get; set; }
    //    public virtual int EditAddressRefId { get; set; }
    //    public virtual int? MembershipTierId { get; set; }

    //}
    public class ConnectUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }
        public DateTime CreationTime { get; set; }
        public string Password { get; set; }

        public string EmailAddress { get; set; }
        public List<UserListRoleDto> Roles { get; set; }
        public bool IsActive { get; set; }
    }

    public class RoleListDto : IOutputDto
    {
        public List<RoleDto> Items { get; set; }
    }
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
    public class UserListRoleDto
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
    public class CreateUserInput : IInputDto
    {
        public ConnectUserDto User { get; set; }
        public int TenantId { get; set; }
        public string[] AssignedRoleNames { get; set; }
    }
    //public class CreateMemberInput : IInputDto
    //{
    //    public ConnectMemberDto Member { get; set; }
    //    public int TenantId { get; set; }
    //}
    public class ApiDeleteUserInput : IInputDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }

    }
}