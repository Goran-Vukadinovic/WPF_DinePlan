using DinePlan.Domain.Models.Accounts;
using DinePlan.Presentation.Common.ModelBase;
using System.Collections.Generic;
using System.Linq;

namespace DinePlan.Modules.AccountModule
{
    public class AccountRowViewModel : GenericViewModelbase
    {
        private decimal _amount;

        private string _description;

        private bool _isSelected;

        public AccountRowViewModel(Account account, AccountTransactionDocumentType documentType)
        {
            Account = account;
            Amount = AccountService.GetDefaultAmount(documentType, account);
            Description = AccountService.GetDescription(documentType, account);
            TargetAccounts = GetAccountSelectors(documentType, account).ToList();
        }

        public AccountSelectViewModel this[int accountTypeId]
        {
            get { return TargetAccounts.Single(x => x.AccountType.Id == accountTypeId); }
        }

        public Account Account { get; }

        public IList<AccountSelectViewModel> TargetAccounts { get; set; }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                IsSelected = _amount != 0;
                RaisePropertyChanged(nameof(Amount));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        private IEnumerable<AccountSelectViewModel> GetAccountSelectors(AccountTransactionDocumentType documentType,
            Account selectedAccount)
        {
            var accountMap =
                documentType.AccountTransactionDocumentAccountMaps.FirstOrDefault(
                    x => x.AccountId == selectedAccount.Id);
            return accountMap != null
                ? documentType.GetNeededAccountTypes().Select(x =>
                    new AccountSelectViewModel(CacheService.GetAccountTypeById(x), accountMap.MappedAccountId,
                        accountMap.MappedAccountName))
                : documentType.GetNeededAccountTypes()
                    .Select(x => new AccountSelectViewModel(CacheService.GetAccountTypeById(x)));
        }
    }
}