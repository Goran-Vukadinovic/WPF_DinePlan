using DinePlan.Domain.Models;
using DinePlan.Domain.Models.Accounts;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services.Common;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.AccountModule
{
    [Export]
    public class AccountSelectorViewModel : GenericViewModelbase
    {
        private IEnumerable<AccountButton> _accountButtons;

        private IEnumerable<AccountScreenAutmationCommandMapViewModel> _automationCommands;

        private IEnumerable<DocumentTypeButtonViewModel> _batchDocumentButtons;
        private AccountScreen _selectedAccountScreen;

        [ImportingConstructor]
        public AccountSelectorViewModel()
        {
            Accounts = new ObservableCollection<AccountScreenRow>();
            ShowAccountDetailsCommand = new CaptionCommand<string>(
                LoOv.G(o => Resources.AccountDetails).Replace(' ', '\r'), OnShowAccountDetails, CanShowAccountDetails);
            PrintCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Print), OnPrint);
            AccountButtonSelectedCommand = new CaptionCommand<AccountScreen>("", OnAccountScreenSelected);
            AutomationCommandSelectedCommand =
                new CaptionCommand<AccountScreenAutmationCommandMap>("", OnAutomationCommandSelected);

            EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.ResetCache)
                    {
                        _accountButtons = null;
                        _batchDocumentButtons = null;
                        _selectedAccountScreen = null;
                    }
                });
        }

        public ICaptionCommand ShowAccountDetailsCommand { get; set; }
        public ICaptionCommand PrintCommand { get; set; }
        public ICaptionCommand AccountButtonSelectedCommand { get; set; }
        public ICaptionCommand AutomationCommandSelectedCommand { get; set; }

        public IEnumerable<DocumentTypeButtonViewModel> BatchDocumentButtons
        {
            get
            {
                return _batchDocumentButtons ??
                       (_batchDocumentButtons =
                           _selectedAccountScreen != null
                               ? ApplicationState
                                   .GetBatchDocumentTypes(
                                       _selectedAccountScreen.AccountScreenValues.Select(x => x.AccountTypeName))
                                   .Where(x => !string.IsNullOrEmpty(x.ButtonHeader))
                                   .Select(x => new DocumentTypeButtonViewModel(x, null))
                               : null);
            }
        }

        public IEnumerable<AccountScreenAutmationCommandMapViewModel> AutomationCommands
        {
            get
            {
                return _automationCommands ?? (_automationCommands =
                    _selectedAccountScreen != null
                        ? _selectedAccountScreen.AutmationCommandMaps.Select(x =>
                            new AccountScreenAutmationCommandMapViewModel(x, CacheService))
                        : null);
            }
        }

        public IEnumerable<AccountScreen> AccountScreens => CacheService.GetAccountScreens();

        public IEnumerable<AccountButton> AccountButtons
        {
            get
            {
                return _accountButtons ??
                       (_accountButtons = AccountScreens.Select(x => new AccountButton(x, CacheService)));
            }
        }

        public ObservableCollection<AccountScreenRow> Accounts { get; }

        public AccountScreenRow SelectedAccount { get; set; }

        public event EventHandler Refreshed;

        protected virtual void OnRefreshed()
        {
            var handler = Refreshed;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        private void OnAccountScreenSelected(AccountScreen accountScreen)
        {
            UpdateAccountScreen(accountScreen);
        }

        private bool CanShowAccountDetails(string arg)
        {
            return SelectedAccount != null && SelectedAccount.AccountId > 0;
        }

        private void OnShowAccountDetails(object obj)
        {
            CommonEventPublisher.PublishEntityOperation(new AccountData(SelectedAccount.AccountId),
                EventTopicNames.DisplayAccountTransactions, EventTopicNames.ActivateAccountSelector);
        }

        private void OnAutomationCommandSelected(AccountScreenAutmationCommandMap obj)
        {
            object value = null;
            if (obj.AutomationCommandValueType == 0) // Account Id
            {
                var account = AccountService.GetAccountById(SelectedAccount.AccountId);
                if (account == null) return;
                value = account.Id;
            }

            if (obj.AutomationCommandValueType == 1) //Entity Id
            {
                var entities = EntityService.GetEntitiesByAccountId(SelectedAccount.AccountId);
                if (!entities.Any()) return;
                value = entities.Select(x => x.Id).First();
            }

            if (obj.AutomationCommandValueType == 2) //Entity Id List
                value = string.Join(",",
                    EntityService.GetEntitiesByAccountId(SelectedAccount.AccountId).Select(x => x.Id));

            ApplicationState.NotifyEvent(RuleEventNames.AutomationCommandExecuted, new
            {
                obj.AutomationCommandName,
                CommandValue = value
            });
        }

        private void UpdateAccountScreen(AccountScreen accountScreen)
        {
            if (accountScreen == null) return;
            _batchDocumentButtons = null;
            _selectedAccountScreen = accountScreen;
            _automationCommands = null;

            Accounts.Clear();
            Accounts.AddRange(AccountService.GetAccountScreenRows(accountScreen, ApplicationState.CurrentWorkPeriod));

            RaisePropertyChanged(nameof(BatchDocumentButtons));
            RaisePropertyChanged(nameof(AccountButtons));
            RaisePropertyChanged(nameof(AutomationCommands));

            OnRefreshed();
        }

        public void Refresh()
        {
            UpdateAccountScreen(_selectedAccountScreen ?? (_selectedAccountScreen = AccountScreens.FirstOrDefault()));
        }

        private void OnPrint(string obj)
        {
            ReportServiceClient.PrintAccountScreen(_selectedAccountScreen);
        }
    }
}