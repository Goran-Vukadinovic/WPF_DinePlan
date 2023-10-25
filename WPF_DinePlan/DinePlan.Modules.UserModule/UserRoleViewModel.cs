using DinePlan.Domain.Models.Tickets;
using DinePlan.Domain.Models.Users;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinePlan.Modules.UserModule
{
    public class UserRoleViewModel : EntityViewModelBase<UserRole>
    {
        private IEnumerable<Department> _departments;
        private IEnumerable<PermissionViewModel> _permissions;

        public IEnumerable<PermissionViewModel> Permissions => _permissions ?? (_permissions = GetPermissions());

        public IEnumerable<Department> Departments => _departments ?? (_departments = Workspace.All<Department>());

        public int DepartmentId
        {
            get => Model.DepartmentId;
            set => Model.DepartmentId = value;
        }

        public bool IsAdmin
        {
            get => Model.IsAdmin;
            set => Model.IsAdmin = value || Model.Id == 1;
        }

        private IEnumerable<PermissionViewModel> GetPermissions()
        {
            try
            {
                var missingPermissions =
                    Model.Permissions.Where(x => PermissionRegistry.PermissionNames.All(y => y.Key != x.Name));

                missingPermissions.ToList().ForEach(x =>
                {
                    Model.Permissions.Remove(x);
                    Workspace.Delete(x);
                });

                if (Model.Permissions.Count() < PermissionRegistry.PermissionNames.Count)
                    foreach (
                        var pName in
                        PermissionRegistry.PermissionNames.Keys.Where(
                            pName => Model.Permissions.SingleOrDefault(x => x.Name == pName) == null).ToList())
                        Model.Permissions.Add(new Permission { Name = pName, Value = 0 });
                return Model.Permissions.OrderBy(a=>a.Name).Select(x => new PermissionViewModel(x));
            }
            catch
            {
                return null;
            }
        }

        public override Type GetViewType()
        {
            return typeof(UserRoleView);
        }

        public override string GetModelTypeString()
        {
            return LoOv.G(o => Resources.UserRole);
        }

        protected override AbstractValidator<UserRole> GetValidator()
        {
            return new UserRoleValidator();
        }
    }

    internal class UserRoleValidator : EntityValidator<UserRole>
    {
        public UserRoleValidator()
        {
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }
}