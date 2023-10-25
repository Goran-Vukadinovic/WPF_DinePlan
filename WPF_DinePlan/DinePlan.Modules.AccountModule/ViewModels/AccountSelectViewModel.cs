using DinePlan.Domain.Models.Accounts;
using DinePlan.Presentation.Common.ModelBase;
using System;
using System.Collections.Generic;

namespace DinePlan.Modules.AccountModule
{
    public class AccountSelectViewModel : GenericViewModelbase
    {
        private string _accountName;

        private AccountType _accountType;
        private int _selectedAccountId;

        public AccountSelectViewModel(AccountType accountType)
        {
            AccountType = accountType;
        }

        public AccountSelectViewModel(AccountType accountType, int accountId, string accountName)
            : this(accountType)
        {
            _accountName = accountName;
            _selectedAccountId = accountId;
        }

        public AccountSelectViewModel(AccountType accountType, string accountName, Action<string, int> updateAction)
            : this(accountType)
        {
            _accountName = accountName;
            UpdateAction = updateAction;
        }

        protected Action<string, int> UpdateAction { get; set; }

        public string AccountName
        {
            get => _accountName ?? (_accountName = AccountService.GetAccountNameById(SelectedAccountId));
            set
            {
                _accountName = value;
                SelectedAccountId = AccountService.GetAccountIdByName(value);
                if (SelectedAccountId == 0)
                    RaisePropertyChanged(nameof(AccountNames));
                _accountName = null;
                RaisePropertyChanged(nameof(AccountName));
            }
        }

        public IEnumerable<string> AccountNames
        {
            get
            {
                if (AccountType == null) return null;
                return AccountService.GetCompletingAccountNames(AccountType.Id, AccountName);
            }
        }

        public AccountType AccountType
        {
            get => _accountType;
            set
            {
                _accountType = value;
                RaisePropertyChanged(nameof(AccountType));
                RaisePropertyChanged(nameof(TemplateName));
            }
        }

        public int SelectedAccountId
        {
            get => _selectedAccountId;
            set
            {
                _selectedAccountId = value;
                if (UpdateAction != null)
                    UpdateAction.Invoke(AccountName, value);
            }
        }

        public string TemplateName => AccountType == null ? "" : string.Format("{0}:", AccountType.Name);
    }
}