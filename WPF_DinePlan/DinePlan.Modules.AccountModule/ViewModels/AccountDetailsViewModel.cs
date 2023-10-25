using DinePlan.Domain.Models;
using DinePlan.Domain.Models.Accounts;
using DinePlan.Domain.Models.Tickets;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Persistance.Data;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services.Common;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.AccountModule
{
    [Export]
    public class AccountDetailsViewModel : GenericViewModelbase
    {
        private OperationRequest<AccountData> _currentOperationRequest;

        private DateTime? _end;

        private string _filterType;

        private Account _selectedAccount;

        private DateTime? _start;

        [ImportingConstructor]
        public AccountDetailsViewModel()
        {
            CloseAccountScreenCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Close), OnCloseAccountScreen);
            DisplayTicketCommand =
                new CaptionCommand<string>(LoOv.G(o => Resources.FindTicket).Replace(" ", "\r").ToUpper(),
                    OnDisplayTicket);
            DisplayTransaction =
                new CaptionCommand<string>(LoOv.G(o => Resources.FindTransaction).Replace(" ", "\r").ToUpper(),
                    OnDisplayTicket);
            PrintAccountCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Print), OnPrintAccount);
            RefreshCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Refresh), OnRefreshCommand);
            AccountDetails = new ObservableCollection<AccountDetailData>();
            DocumentTypes = new ObservableCollection<DocumentTypeButtonViewModel>();
            AccountSummaries = new ObservableCollection<AccountSummaryData>();
            EventServiceFactory.EventService.GetEvent<GenericEvent<OperationRequest<AccountData>>>()
                .Subscribe(OnDisplayAccountTransactions);
        }

        public ICaptionCommand RefreshCommand { get; set; }
        public ICaptionCommand CloseAccountScreenCommand { get; set; }
        public ICaptionCommand DisplayTicketCommand { get; set; }
        public ICaptionCommand DisplayTransaction { get; set; }

        public ICaptionCommand PrintAccountCommand { get; set; }

        public DateTime? Start
        {
            get => _start;
            set
            {
                _start = value;
                RaisePropertyChanged(nameof(StartDateString));
            }
        }

        public DateTime? End
        {
            get => _end;
            set
            {
                _end = value;
                RaisePropertyChanged(nameof(EndDateString));
            }
        }

        public string StartDateString
        {
            get => Start.HasValue ? Start.GetValueOrDefault().ToString("dd MM yyyy") : "";
            set => SetStartDate(value);
        }

        public string EndDateString
        {
            get => End.HasValue ? End.GetValueOrDefault().ToString("dd MM yyyy") : "";
            set => SetEndDate(value);
        }

        public AccountType SelectedAccountType { get; set; }
        public AccountDetailData FocusedAccountTransaction { get; set; }

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                if (SelectedAccountType == null || SelectedAccountType.Id != _selectedAccount.AccountTypeId)
                    SelectedAccountType = CacheService.GetAccountTypeById(value.AccountTypeId);
                RaisePropertyChanged(nameof(SelectedAccount));
                UpdateTemplates();
                FilterType = LoOv.G(o => Resources.Default);
            }
        }

        public ObservableCollection<DocumentTypeButtonViewModel> DocumentTypes { get; set; }
        public ObservableCollection<AccountDetailData> AccountDetails { get; set; }
        public ObservableCollection<AccountSummaryData> AccountSummaries { get; set; }

        public string[] FilterTypes
        {
            get
            {
                return new[]
                {
                    LoOv.G(o => Resources.Default), LoOv.G(o => Resources.All), LoOv.G(o => Resources.ThisMonth),
                    LoOv.G(o => Resources.PastMonth), LoOv.G(o => Resources.ThisWeek), LoOv.G(o => Resources.PastWeek),
                    LoOv.G(o => Resources.WorkPeriod)
                };
            }
        }

        public string FilterType
        {
            get => _filterType;
            set
            {
                _filterType = value;
                Start = null;
                End = null;
                DisplayTransactions();
                RaisePropertyChanged(nameof(FilterType));
            }
        }

        public string TotalBalance
        {
            get { return AccountDetails.Sum(x => x.Debit - x.Credit).ToString(LocalSettings.ReportCurrencyFormat); }
        }

        private void SetStartDate(string value)
        {
            Start = StrToDate(value);
            if (!Start.HasValue) End = Start;
        }

        private void SetEndDate(string value)
        {
            End = StrToDate(value);
            if (Start.HasValue && End == Start.GetValueOrDefault())
                End = Start.GetValueOrDefault().AddDays(1).AddSeconds(-1);
            if (End.HasValue && End < Start) End = null;
        }

        private static DateTime? StrToDate(string value)
        {
            if (string.IsNullOrEmpty(value.Trim())) return null;
            var vals = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x))
                .ToList();
            if (vals.Count == 1) vals.Add(DateTime.Now.Month);
            if (vals.Count == 2) vals.Add(DateTime.Now.Year);

            if (vals[2] < 1) vals[2] = DateTime.Now.Year;
            if (vals[2] < 1000) vals[2] += 2000;

            if (vals[1] < 1) vals[1] = 1;
            if (vals[1] > 12) vals[1] = 12;

            var dim = DateTime.DaysInMonth(vals[0], vals[1]);
            if (vals[0] < 1) vals[0] = 1;
            if (vals[0] > dim) vals[0] = dim;
            return new DateTime(vals[2], vals[1], vals[0]);
        }

        private void UpdateTemplates()
        {
            DocumentTypes.Clear();
            if (SelectedAccount != null)
            {
                var templates = ApplicationState.GetAccountTransactionDocumentTypes(SelectedAccount.AccountTypeId)
                    .Where(x => !string.IsNullOrEmpty(x.ButtonHeader) && x.CanMakeAccountTransaction(SelectedAccount));
                DocumentTypes.AddRange(templates.Select(x => new DocumentTypeButtonViewModel(x, SelectedAccount)));
            }
        }

        private void DisplayTransactions()
        {
            if (FilterType != LoOv.G(o => Resources.Default))
            {
                var dateRange = AccountService.GetDateRange(FilterType, ApplicationState.CurrentWorkPeriod);
                Start = dateRange.Start;
                End = dateRange.End;
            }

            var transactionData =
                AccountService.GetAccountTransactionSummary(SelectedAccount, ApplicationState.CurrentWorkPeriod, Start,
                    End);
            Start = transactionData.Start;
            End = transactionData.End != transactionData.Start ? transactionData.End : null;

            AccountDetails.Clear();
            AccountDetails.AddRange(transactionData.Transactions);
            AccountSummaries.Clear();
            AccountSummaries.AddRange(transactionData.Summaries);

            RaisePropertyChanged(nameof(TotalBalance));
        }

        private void OnDisplayAccountTransactions(EventParameters<OperationRequest<AccountData>> obj)
        {
            if (obj.Topic == EventTopicNames.DisplayAccountTransactions)
            {
                var account = AccountService.GetAccountById(obj.Value.SelectedItem.AccountId);
                if (obj.Value != null && !string.IsNullOrEmpty(obj.Value.GetExpectedEvent()))
                    _currentOperationRequest = obj.Value;
                SelectedAccount = account;
            }
        }

        private void OnRefreshCommand(string obj)
        {
            DisplayTransactions();
        }

        private void OnCloseAccountScreen(string obj)
        {
            AccountDetails.Clear();
            if (_currentOperationRequest != null)
                _currentOperationRequest.Publish(new AccountData(SelectedAccount));
        }

        private void OnDisplayTicket(string obj)
        {
            if (FocusedAccountTransaction != null)
            {
                var did = FocusedAccountTransaction.Model.AccountTransactionDocumentId;
                var ticket = Dao.Single<Ticket>(a => a.TransactionDocument_Id == did);
                if (ticket != null)
                {
                    var expectedEvent = _currentOperationRequest != null
                        ? _currentOperationRequest.GetExpectedEvent()
                        : EventTopicNames.DisplayAccountTransactions;

                    ExtensionMethods.PublishIdEvent(ticket.Id,
                        EventTopicNames.DisplayTicket,
                        () => CommonEventPublisher.PublishEntityOperation(new AccountData(SelectedAccount),
                            EventTopicNames.DisplayAccountTransactions, expectedEvent));
                }
            }
        }

        private void OnPrintAccount(string obj)
        {
            ReportService.PrintAccountTransactions(SelectedAccount, ApplicationState.CurrentWorkPeriod,
                ApplicationState.GetReportPrinter(), FilterType);
        }
    }
}