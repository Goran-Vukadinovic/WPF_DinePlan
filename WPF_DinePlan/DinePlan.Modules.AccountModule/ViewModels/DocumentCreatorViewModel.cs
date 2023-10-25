using DinePlan.Domain.Models;
using DinePlan.Domain.Models.Accounts;
using DinePlan.Domain.Models.Settings;
using DinePlan.Infrastructure.Helpers;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.AccountModule
{
    [Export]
    public class DocumentCreatorViewModel : GenericViewModelbase
    {
        private IEnumerable<AccountSelectViewModel> _accountSelectors;
        private string _amountStr;
        private string _description;

        [ImportingConstructor]
        public DocumentCreatorViewModel()
        {
            SaveCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Save), OnSave);
            PrintCommand = new CaptionCommand<string>("Print", OnPrint);
            CancelCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Cancel), OnCancel);
            EventServiceFactory.EventService.GetEvent<GenericEvent<DocumentCreationData>>()
                .Subscribe(OnDocumentCreation);
        }

        public string AccountName
        {
            get
            {
                return SelectedAccount == null
                    ? ""
                    : string.Format("{0} {1}: {2}", SelectedAccount.Name, LoOv.G(o => Resources.Balance),
                        GetAccountBalance());
            }
        }

        public Account SelectedAccount { get; set; }
        public AccountTransactionDocumentType DocumentType { get; set; }
        public string Description { get; set; }

        public decimal Amount
        {
            get => Utility.ToDecimal(AmountStr, new decimal(0));
            set => AmountStr = value.ToString(LocalSettings.CurrencyFormat);
        }

        public string AmountStr
        {
            get => _amountStr ?? "0";
            set
            {
                _amountStr = value;
                RaisePropertyChanged(nameof(AmountStr));
            }
        }

        public bool IsPrintCommandVisible => DocumentType != null && DocumentType.PrinterTemplateId > 0;
        public ICaptionCommand SaveCommand { get; set; }
        public ICaptionCommand PrintCommand { get; set; }
        public ICaptionCommand CancelCommand { get; set; }
        public Printer SelectedPrinter => ApplicationState.GetTransactionPrinter();

        public PrinterTemplate SelectedPrinterTemplate
        {
            get { return CacheService.GetPrinterTemplates().First(x => x.Id == DocumentType.PrinterTemplateId); }
        }

        public IEnumerable<AccountSelectViewModel> AccountSelectors
        {
            get => _accountSelectors;
            set
            {
                _accountSelectors = value;
                RaisePropertyChanged(nameof(AccountSelectors));
            }
        }

        private void OnDocumentCreation(EventParameters<DocumentCreationData> obj)
        {
            _description = AccountService.GetDescription(obj.Value.DocumentType, obj.Value.Account);
            SelectedAccount = obj.Value.Account;
            DocumentType = obj.Value.DocumentType;
            Description = _description;
            Amount = AccountService.GetDefaultAmount(obj.Value.DocumentType, obj.Value.Account);
            AccountSelectors = GetAccountSelectors();

            RaisePropertyChanged(nameof(Description));
            RaisePropertyChanged(nameof(Amount));
            RaisePropertyChanged(nameof(AccountName));
            RaisePropertyChanged(nameof(IsPrintCommandVisible));
        }

        private IEnumerable<AccountSelectViewModel> GetAccountSelectors()
        {
            var map = DocumentType.AccountTransactionDocumentAccountMaps.FirstOrDefault(x =>
                x.AccountId == SelectedAccount.Id);
            return map != null
                ? DocumentType.GetNeededAccountTypes().Select(x =>
                    new AccountSelectViewModel(CacheService.GetAccountTypeById(x), map.MappedAccountId,
                        map.MappedAccountName))
                : DocumentType.GetNeededAccountTypes()
                    .Select(x => new AccountSelectViewModel(CacheService.GetAccountTypeById(x)));
        }

        private string GetAccountBalance()
        {
            if (SelectedAccount.ForeignCurrencyId > 0)
                return
                    AccountService.GetAccountExchangeBalance(SelectedAccount.Id)
                        .ToString(LocalSettings.ReportCurrencyFormat);
            return AccountService.GetAccountBalance(SelectedAccount.Id).ToString(LocalSettings.ReportCurrencyFormat);
        }

        private void OnCancel(string obj)
        {
            CommonEventPublisher.PublishEntityOperation(new AccountData(SelectedAccount),
                EventTopicNames.DisplayAccountTransactions);
        }

        private void OnSave(string obj)
        {
            Action(CreateDocument);
        }

        private void OnPrint(string obj)
        {
            Action(PrintDocument);
        }

        public void Action(Func<AccountTransactionDocument> action)
        {
            var document = action();
            if (document != null)
            {
                ApplicationState.NotifyEvent(RuleEventNames.AccountTransactionDocumentCreated,
                    new { AccountTransactionDocumentName = DocumentType.Name, DocumentId = document.Id });
                CommonEventPublisher.PublishEntityOperation(new AccountData(SelectedAccount),
                    EventTopicNames.DisplayAccountTransactions);
            }
        }

        public AccountTransactionDocument CreateDocument()
        {
            var description = Description;
            if (Description != _description) description = string.Format("{0}  [{1}]", _description, Description);
            if (AccountSelectors.Any(x => x.SelectedAccountId == 0)) return null;
            return AccountService.CreateTransactionDocument(SelectedAccount, DocumentType, description, Amount,
                AccountSelectors.Select(x => new Account { Id = x.SelectedAccountId, AccountTypeId = x.AccountType.Id }));
        }

        public AccountTransactionDocument PrintDocument()
        {
            if (SelectedPrinter == null) return null;
            if (SelectedPrinterTemplate == null) return null;
            var document = CreateDocument();
            if (document == null) return null;
            PrinterService.PrintObject(document, SelectedPrinter, SelectedPrinterTemplate);
            return document;
        }
    }
}