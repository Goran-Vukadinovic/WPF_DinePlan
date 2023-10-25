using DinePlan.Domain.Models.Accounts;
using DinePlan.Domain.Models.Automation;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Services;
using System.Linq;

namespace DinePlan.Modules.AccountModule
{
    public class AccountScreenAutmationCommandMapViewModel
    {
        private static string[] _commandValueTypes;
        private readonly ICacheService _cacheService;

        private readonly AccountScreenAutmationCommandMap _model;

        public AccountScreenAutmationCommandMapViewModel(AccountScreenAutmationCommandMap model)
        {
            _model = model;
        }

        public AccountScreenAutmationCommandMapViewModel(AccountScreenAutmationCommandMap model,
            ICacheService cacheService)
            : this(model)
        {
            _cacheService = cacheService;
            AutomationCommand = _cacheService.GetAutomationCommandByName(model.AutomationCommandName);
        }

        public static string[] CommandValueTypes
        {
            get
            {
                return _commandValueTypes ?? (_commandValueTypes =
                    new[] { LoOv.G(o => Resources.Account), LoOv.G(o => Resources.Entity) });
            }
        }

        public AutomationCommand AutomationCommand { get; set; }

        public string AutomationCommandName
        {
            get => _model.AutomationCommandName;
            set => _model.AutomationCommandName = value;
        }

        public string ButtonHeader =>
            AutomationCommand != null ? AutomationCommand.ButtonCaption : AutomationCommandName;

        public string AutomationCommandValueType
        {
            get => CommandValueTypes[_model.AutomationCommandValueType];
            set => _model.AutomationCommandValueType = CommandValueTypes.ToList().IndexOf(value);
        }
    }
}