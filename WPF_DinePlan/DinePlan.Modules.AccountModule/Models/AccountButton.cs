using DinePlan.Domain.Models.Accounts;
using DinePlan.Services;
using System.Collections.Generic;
using System.Linq;

namespace DinePlan.Modules.AccountModule
{
    public class AccountButton
    {
        private readonly ICacheService _cacheService;

        public AccountButton(AccountScreen accountScreen, ICacheService cacheService)
        {
            Model = accountScreen;
            _cacheService = cacheService;
        }

        public string Caption => Model.Name;
        public AccountScreen Model { get; }

        public string ButtonColor => "Gainsboro";

        public IEnumerable<AccountType> AccountTypes
        {
            get
            {
                return _cacheService.GetAccountTypesByName(Model.AccountScreenValues.Select(x => x.AccountTypeName));
            }
        }
    }
}