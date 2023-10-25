using System;
using DinePlan.Domain.Models.Tickets;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Common.Services;
using DinePlan.Presentation.Services.Common;
using DinePlan.Presentation.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using System.ComponentModel.Composition;
using System.Linq;
using DinePlan.Common.Enum;
using DinePlan.Common.Model.Point;
using DinePlan.Common.POS;
using DinePlan.Domain.Models.Entities;
using DinePlan.Infrastructure.Settings;

namespace DinePlan.Modules.PaymentModule
{
    [Export]
    public class PaymentEditor : GenericViewModelbase
    {
        private Ticket _selectedTicket;

        /// <summary>
        ///     The back up value for <see cref="AccountBalances" /> property.
        /// </summary>
        private AccountBalances _accountBalances;

        [ImportingConstructor]
        public PaymentEditor()
        {
            _selectedTicket = Ticket.Empty;

            EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>().Subscribe(x =>
            {
                if (SelectedTicket != Ticket.Empty && x.Topic == EventTopicNames.CloseTicketRequested)
                    SelectedTicket = Ticket.Empty;
            });
        }

        /// <summary>
        ///     Gets or sets the AccountBalances service.
        /// </summary>
        protected AccountBalances AccountBalances
        {
            get
            {
                if (_accountBalances == null) _accountBalances = ServiceLocator.Current.GetInstance<AccountBalances>();

                return _accountBalances;
            }
        }

        public Ticket SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                AccountBalances.SelectedTicket = value;
            }
        }

        public bool AccountMode { get; set; }
        public decimal ExchangeRate { get; set; }

        public decimal GetRemainingAmount()
        {
            return AccountMode && AccountBalances.ContainsActiveAccount()
                ? SelectedTicket.GetRemainingAmount() + AccountBalances.GetActiveAccountBalance() -
                  SelectedTicket.TransactionDocument.AccountTransactions.Where(
                      x => x.ContainsAccountId(AccountBalances.GetActiveAccountId())).Sum(y => y.Amount)
                : SelectedTicket.GetRemainingAmount();
        }

        public void UpdateTicketPayment(PaymentType paymentType, ChangePaymentType changeTemplate,
            decimal paymentDueAmount, decimal paidAmount, decimal tenderedAmount, decimal tipAmount, decimal otherAmount, 
            decimal conversion, string desc = "")
        {
            var paymentAccount = paymentType.Account ?? TicketService.GetAccountForPayment(SelectedTicket, paymentType);

            if (paymentDueAmount > SelectedTicket.GetRemainingAmount() &&
                paidAmount > SelectedTicket.GetRemainingAmount())
            {
                var account = AccountBalances.GetActiveAccount();
                if (account != null)
                {
                    var ticketAmount = SelectedTicket.GetRemainingAmount();
                    var accountAmount = paidAmount - ticketAmount;
                    var accountBalance = AccountBalances.GetAccountBalance(account.Id);
                    if (accountAmount > accountBalance) accountAmount = accountBalance;
                    if (ticketAmount > 0)
                        TicketService.AddPayment(SelectedTicket, paymentType, paymentAccount, ticketAmount,
                            tenderedAmount - accountAmount, desc, "",0,otherAmount, conversion);
                    if (accountAmount > 0)
                        TicketService.AddAccountTransaction(SelectedTicket, account, paymentAccount, accountAmount,
                            ExchangeRate);
                }

                AccountBalances.Refresh();
            }
            else
            {
                TicketService.AddPayment(SelectedTicket, paymentType, paymentAccount, paidAmount, tenderedAmount, desc, "",tipAmount, otherAmount, conversion);
                if (paidAmount - tipAmount > paymentDueAmount && changeTemplate != null)
                    TicketService.AddChangePayment(SelectedTicket, changeTemplate, changeTemplate.Account,
                        paidAmount - tipAmount - paymentDueAmount);
            }
        }
        
        public void UpdateTicketDiscountPayment(PaymentType paymentType, decimal paymentDueAmount, decimal paidAmount, decimal tenderedAmount)
        {
            var paymentAccount = paymentType.Account ?? TicketService.GetAccountForPayment(SelectedTicket, paymentType);

            if (paymentDueAmount > SelectedTicket.GetRemainingAmount() && paidAmount > SelectedTicket.GetRemainingAmount())
            {
                var account = AccountBalances.GetActiveAccount();
                if (account != null)
                {
                    var ticketAmount = SelectedTicket.GetRemainingAmount();
                    var accountAmount = paidAmount - ticketAmount;
                    var accountBalance = AccountBalances.GetAccountBalance(account.Id);
                    if (accountAmount > accountBalance) accountAmount = accountBalance;
                    if (ticketAmount > 0)
                        TicketService.AddDiscountPayment(SelectedTicket, paymentType, paymentAccount, ticketAmount,
                            tenderedAmount - accountAmount);
                    if (accountAmount > 0)
                        TicketService.AddAccountTransaction(SelectedTicket, account, paymentAccount, accountAmount,
                            ExchangeRate);
                }

                AccountBalances.Refresh();
            }
            else
            {
                TicketService.AddDiscountPayment(SelectedTicket, paymentType, paymentAccount, paidAmount, tenderedAmount);
            }
        }

        public void UpdateCalculations()
        {
            TicketService.RecalculateTicket(SelectedTicket);
            if (GetRemainingAmount() < 0)
            {
                foreach (
                    var cSelector in
                    ApplicationState.GetCalculationSelectors().Where(x => !string.IsNullOrEmpty(x.ButtonHeader)))
                    foreach (var ctemplate in cSelector.CalculationTypes)
                        if (SelectedTicket.Calculations.Any(x => x.CalculationTypeId == ctemplate.Id))
                            SelectedTicket.AddCalculation(ctemplate, 0, removed: false);

                TicketService.RecalculateTicket(SelectedTicket);
                if (GetRemainingAmount() >= 0)
                    InteractionService.UserIntraction.ShowMessage(LoOv.G(o => Resources.AllDiscountsRemoved));
            }
        }

        public void Close()
        {
            if (SelectedTicket.RemainingAmount == 0)
            {
                Entity entity = null;
                if (LocalSettings.EDine)
                {
                    if (SelectedTicket.EDineTicketId != null)
                    {
                        if (SelectedTicket.TicketEntities.Any())
                        {
                            var tableEntity = CacheService.GetEntityTypeByName(PosConsts.Tables);

                            if (tableEntity != null)
                            {
                                var tableId = SelectedTicket.TicketEntities.LastOrDefault(a =>
                                    a.EntityTypeId == tableEntity.Id);
                                if (tableId != null)
                                {
                                    entity = CacheService.GetEntityById(tableId.EntityId);
                                }
                            }
                        }
                    }
                }

                try
                {
                    if (entity != null)
                    {
                        var myTableName = entity.Name;

                        var myCode = EdineApiService.VacantTable(myTableName);
                        if (myCode == null || string.IsNullOrEmpty(myCode.Pincode))
                        {
                            DialogService.ShowFeedback(" Please clear manually or generate key from the Table Screen");
                            EntityService.UpdateEntityState(entity.Id, "Status",
                                FlyEntityStatus.TableNotReset.ToDescription(), "");
                        }
                        
                    }
                }
                catch (Exception)
                {
                    DialogService.ShowFeedback("eDine Table was not Cleared. Please clear manually or generate key from the Table Screen");
                    if (entity != null)
                    {

                        EntityService.UpdateEntityState(entity.Id, "Status",
                            FlyEntityStatus.TableNotReset.ToDescription(), "");
                    }
                }
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.CloseTicketRequested);

            }
            else
            {
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.RefreshSelectedTicket);
            }
        }
    }
}