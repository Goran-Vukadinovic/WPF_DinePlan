using DinePlan.Domain.Models.Tickets;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Prism.Mef.Modularity;
using Prism.Modularity;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Modules.PaymentModule.ViewModels;
using DinePlan.Modules.PaymentModule.Views;

namespace DinePlan.Modules.PaymentModule
{
    [ModuleExport(typeof(PaymentModule), InitializationMode = InitializationMode.OnDemand)]
    internal class PaymentModule : VisibleModuleBase
    {
        /// <summary>
        ///     The back up value for <see cref="ChangeTemplatesView" /> property.
        /// </summary>
        private ChangeTemplatesView changeTemplatesView;

        /// <summary>
        ///     The back up value for <see cref="PaymentEditorView" /> property.
        /// </summary>
        private UserControl paymentEditorView;

        /// <summary>
        ///     The back up value for <see cref="ReturningAmountView" /> property.
        /// </summary>
        private ReturningAmountView returningAmountView;

        /// <summary>
        ///     The back up value for <see cref="TenderedValueView" /> property.
        /// </summary>
        private TenderedValueView tenderedValueView;

        [ImportingConstructor]
        public PaymentModule(NumberPadViewModel numberPadViewModel)
            : base(AppScreens.PaymentView)
        {
            numberPadViewModel.TypedValueChanged += NumberPadViewModelTypedValueChanged;
        }


        /// <summary>
        ///     Gets or sets the TenderedValueView.
        /// </summary>
        protected TenderedValueView TenderedValueView
        {
            get
            {
                if (tenderedValueView == null)
                    tenderedValueView = ServiceLocator.Current.GetInstance<TenderedValueView>();

                return tenderedValueView;
            }
        }

        /// <summary>
        ///     Gets or sets the ReturningAmountView.
        /// </summary>
        protected ReturningAmountView ReturningAmountView
        {
            get
            {
                if (returningAmountView == null)
                    returningAmountView = ServiceLocator.Current.GetInstance<ReturningAmountView>();

                return returningAmountView;
            }
        }

        /// <summary>
        ///     Gets or sets the PaymentEditorView.
        /// </summary>
        protected UserControl PaymentEditorView =>
            paymentEditorView ??
            (paymentEditorView = ServiceLocator.Current.GetInstance<PaymentEditorView>());

        /// <summary>
        ///     Gets or sets the ChangeTemplatesView.
        /// </summary>
        protected ChangeTemplatesView ChangeTemplatesView
        {
            get
            {
                if (changeTemplatesView == null)
                    changeTemplatesView = ServiceLocator.Current.GetInstance<ChangeTemplatesView>();

                return changeTemplatesView;
            }
        }

        private void NumberPadViewModelTypedValueChanged(object sender, EventArgs e)
        {
            ActivateTenderedAmount();
        }

        public override object GetVisibleView()
        {
            return PaymentEditorView;
        }

        protected override void OnInitialization()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(PaymentEditorView));

            RegionManager.RegisterViewWithRegion(RegionNames.PaymentNumberPadRegion, typeof(NumberPadView));
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentOrderSelectorRegion, typeof(OrderSelectorView));
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentForeignCurrencyRegion,
                typeof(ForeignCurrencyButtonsView));
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentButtonsRegion, GetTypeOfPaymentButtonsView());
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentCommandButtonsRegion, typeof(CommandButtonsView));
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentTotalsRegion, typeof(PaymentTotalsView));
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentTenderedValueRegion, typeof(TenderedValueView));
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentTenderedValueRegion, typeof(ReturningAmountView));
            RegionManager.RegisterViewWithRegion(RegionNames.PaymentTenderedValueRegion, typeof(ChangeTemplatesView));

            EventServiceFactory.EventService.GetEvent<GenericEvent<ReturningAmountViewModel>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.Activate)
                        ActivateReturningAmount();
                });

            EventServiceFactory.EventService.GetEvent<GenericEvent<ChangeTemplatesViewModel>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.Activate)
                        ActivateChangeTemplates();
                });

            EventServiceFactory.EventService.GetEvent<GenericEvent<Ticket>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.MakePayment)
                    {
                        Activate();
                        ActivateTenderedAmount();
                        ((PaymentEditorViewModel)PaymentEditorView.DataContext).Prepare(x.Value);
                    }
                });
        }

        public void ActivateReturningAmount()
        {
            RegionManager.ActivateRegion(RegionNames.PaymentTenderedValueRegion, ReturningAmountView);
        }

        public void ActivateTenderedAmount()
        {
            RegionManager.ActivateRegion(RegionNames.PaymentTenderedValueRegion, TenderedValueView);
        }

        public void ActivateChangeTemplates()
        {
            RegionManager.ActivateRegion(RegionNames.PaymentTenderedValueRegion, ChangeTemplatesView);
        }

        public Type GetTypeOfPaymentButtonsView()
        {
            return typeof(PaymentButtonsView);
        }
    }
}