using DinePlan.Domain.Models.Accounts;
using DinePlan.Domain.Models.Settings;
using DinePlan.Infrastructure.Helpers;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.AccountModule
{
    [Export]
    public class BatchDocumentCreatorViewModel : GenericViewModelbase
    {
        private AccountTransactionDocumentType _selectedDocumentType;

        [ImportingConstructor]
        public BatchDocumentCreatorViewModel()
        {
            Accounts = new ObservableCollection<AccountRowViewModel>();
            CreateDocuments = new CaptionCommand<string>(string.Format(LoOv.G(o => Resources.Create_f), "").Trim(),
                OnCreateDocuments, CanCreateDocument);
            GoBack = new CaptionCommand<string>(LoOv.G(o => Resources.Back), OnGoBack);
            Print = new CaptionCommand<string>(LoOv.G(o => Resources.Print), OnPrint);
        }

        public CaptionCommand<string> CreateDocuments { get; set; }
        public CaptionCommand<string> GoBack { get; set; }
        public CaptionCommand<string> Print { get; set; }

        public string Title => SelectedDocumentType != null ? SelectedDocumentType.Name : "";
        public string Description { get; set; }
        public ObservableCollection<AccountRowViewModel> Accounts { get; set; }

        public bool IsPrintButtonVisible =>
            SelectedDocumentType != null && SelectedPrinterTemplate != null && SelectedPrinter != null;

        public AccountTransactionDocumentType SelectedDocumentType
        {
            get => _selectedDocumentType;
            set
            {
                if (Equals(value, _selectedDocumentType)) return;
                _selectedDocumentType = value;
                RaisePropertyChanged(nameof(SelectedDocumentType));
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(IsPrintButtonVisible));
            }
        }

        public Printer SelectedPrinter => ApplicationState.GetTransactionPrinter();

        public PrinterTemplate SelectedPrinterTemplate
        {
            get
            {
                try
                {
                    return CacheService.GetPrinterTemplates()
                        .FirstOrDefault(x => x.Id == SelectedDocumentType.PrinterTemplateId);
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public event EventHandler OnUpdate;

        private void OnGoBack(string obj)
        {
            SelectedDocumentType.PublishEvent(EventTopicNames.BatchDocumentsCreated);
        }

        public IEnumerable<AccountType> GetNeededAccountTypes()
        {
            return SelectedDocumentType.GetNeededAccountTypes().Select(x => CacheService.GetAccountTypeById(x));
        }

        private void OnOnUpdate(EventArgs e)
        {
            var handler = OnUpdate;
            if (handler != null) handler(this, e);
        }

        private void OnPrint(string obj)
        {
            BatchCreate(CreatePrintDocument);
        }

        private void OnCreateDocuments(string obj)
        {
            BatchCreate(CreateDocument);
        }

        private void BatchCreate(Func<AccountRowViewModel, AccountTransactionDocument> action)
        {
            if (Accounts.Where(x => x.IsSelected).Any(x => x.TargetAccounts.Any(y => y.SelectedAccountId == 0))) return;
            Accounts
                .Where(x => x.IsSelected && x.Amount != 0)
                .ForEach(x => action(x));

            SelectedDocumentType.PublishEvent(EventTopicNames.BatchDocumentsCreated);
        }

        private AccountTransactionDocument CreateDocument(AccountRowViewModel accountRowViewModel)
        {
            var document = AccountService.CreateTransactionDocument(accountRowViewModel.Account,
                SelectedDocumentType, accountRowViewModel.Description,
                accountRowViewModel.Amount,
                accountRowViewModel.TargetAccounts.Select(
                    y =>
                        new Account
                        {
                            Id = y.SelectedAccountId,
                            AccountTypeId = y.AccountType.Id
                        }));
            ApplicationState.NotifyEvent(RuleEventNames.AccountTransactionDocumentCreated, new
            {
                AccountTransactionDocumentName = SelectedDocumentType.Name,
                DocumentId = document.Id
            });

            return document;
        }

        private AccountTransactionDocument CreatePrintDocument(AccountRowViewModel accountRowViewModel)
        {
            if (SelectedPrinterTemplate == null) return null;
            if (SelectedPrinter == null) return null;
            var document = CreateDocument(accountRowViewModel);
            PrinterService.PrintObject(document, SelectedPrinter, SelectedPrinterTemplate);
            return document;
        }

        private bool CanCreateDocument(string arg)
        {
            return Accounts.Any(x => x.IsSelected);
        }

        public void Update(AccountTransactionDocumentType value)
        {
            SelectedDocumentType = value;
            Accounts.Clear();
            var accounts = GetAccounts(value);
            Accounts.AddRange(accounts
                .AsParallel()
                .SetCulture()
                .Select(x => new AccountRowViewModel(x, value)));
            OnOnUpdate(EventArgs.Empty);
        }

        private IEnumerable<Account> GetAccounts(AccountTransactionDocumentType documentType)
        {
            return AccountService.GetDocumentAccounts(documentType).Where(documentType.CanMakeAccountTransaction);
        }
    }
}