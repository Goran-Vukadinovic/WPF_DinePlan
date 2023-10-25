using DinePlan.Domain.Models.Accounts;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Modules.AccountModule.Dashboard;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.ModelBase;
using Prism.Mef.Modularity;
using Prism.Modularity;
using System.ComponentModel.Composition;

namespace DinePlan.Modules.AccountModule
{
    [ModuleExport(typeof(AccountModule), InitializationMode = InitializationMode.OnDemand)]
    public class AccountModule : ModuleBase
    {
        [ImportingConstructor]
        public AccountModule()

        {
            AddDashboardCommand<EntityCollectionViewModelBase<AccountTypeViewModel, AccountType>>(
                LoOv.K(o => Resources.AccountType), LoOv.K(o => Resources.Accounts), 40, true);
            AddDashboardCommand<EntityCollectionViewModelBase<AccountViewModel, Account>>(
                LoOv.K(o => Resources.Account), LoOv.K(o => Resources.Accounts), 40, true);
            AddDashboardCommand<EntityCollectionViewModelBase<AccountTransactionTypeViewModel, AccountTransactionType>>(
                LoOv.K(o => Resources.TransactionType), LoOv.K(o => Resources.Accounts), 40, true);
        }
    }
}
