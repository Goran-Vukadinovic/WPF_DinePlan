using DinePlan.Domain.Models.Users;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Services.Common;
using System.Linq;

namespace DinePlan.Modules.UserModule
{
    public class PermissionViewModel
    {
        private readonly Permission _permission;

        public PermissionViewModel(Permission permission)
        {
            _permission = permission;
        }

        public string Title => PermissionRegistry.PermissionNames[_permission.Name][1];

        public string Category => PermissionRegistry.PermissionNames[_permission.Name][0];

        public static string[] Values { get; } = { LoOv.G(o => Resources.Yes), LoOv.G(o => Resources.No) };

        public string Value
        {
            get => Values[_permission.Value];
            set => _permission.Value = Values.ToList().IndexOf(value);
        }

        public bool IsPermitted
        {
            get => _permission.Value == (int)PermissionValue.Enabled;
            set
            {
                if (value)
                    _permission.Value = (int)PermissionValue.Enabled;
                else
                    _permission.Value = (int)PermissionValue.Disabled;
            }
        }
    }
}