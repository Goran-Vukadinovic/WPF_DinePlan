using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using DinePlan.Common.Model.Permission;
using DinePlan.Common.QD;
using DinePlan.Domain.Models.Entities;
using DinePlan.Domain.Models.Tickets;
using DinePlan.Infrastructure.Messaging;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DinePlan.Presentation.Services.Implementations.Promotion.Extension;
using DinePlan.Services.Common;
using Prism.Commands;

namespace DinePlan.Modules.EntityModule.ViewModel
{
    [Export]
    public class EntitySelectorViewModel : GenericViewModelbase
    {
        private OperationRequest<Entity> _currentOperationRequest;
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly DispatcherTimer _updateTotal;


        [ImportingConstructor]
        public EntitySelectorViewModel()
        {
            EntitySelectionCommand = new DelegateCommand<EntityScreenItemViewModel>(OnSelectEntityExecuted);
            IncPageNumberCommand = new CaptionCommand<string>(LoOv.G(o => Resources.NextPage) + " >>", OnIncPageNumber,
                CanIncPageNumber);
            DecPageNumberCommand = new CaptionCommand<string>("<< " + LoOv.G(o => Resources.PreviousPage),
                OnDecPageNumber,
                CanDecPageNumber);

            EventServiceFactory.EventService.GetEvent<GenericEvent<Message>>().Subscribe(
                x =>
                {
                    if (ApplicationState.ActiveAppScreen == AppScreens.EntityView
                        && x.Topic == EventTopicNames.MessageReceivedEvent
                        && x.Value.Command == Messages.TicketRefreshMessage)
                        RefreshEntityScreenItems();
                });
            Totals = new Dictionary<string, string>();

            _dispatcherTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, LocalSettings.RefreshEntityInSeconds);
            _dispatcherTimer.Start();

            _updateTotal = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            _updateTotal.Tick += RefreshTotals;
            _updateTotal.Interval = new TimeSpan(0, 0, LocalSettings.RefreshEntityInSeconds + 5);
            _updateTotal.Start();
        }

        public DelegateCommand<EntityScreenItemViewModel> EntitySelectionCommand { get; set; }
        public ICaptionCommand IncPageNumberCommand { get; set; }
        public ICaptionCommand DecPageNumberCommand { get; set; }
        public ObservableCollection<EntityScreenItemViewModel> EntityScreenItems { get; set; }

        public Visibility TotalVisibility =>
            SelectedEntityScreen.RefreshEntity ? Visibility.Visible : Visibility.Hidden;

        public EntityScreen SelectedEntityScreen => ApplicationState.SelectedEntityScreen;

        public int CurrentPageNo { get; set; }

        public bool IsPageNavigatorVisible => SelectedEntityScreen != null && SelectedEntityScreen.PageCount > 1;

        public VerticalAlignment ScreenVerticalAlignment =>
            SelectedEntityScreen != null && SelectedEntityScreen.ButtonHeight > 0
                ? VerticalAlignment.Top
                : VerticalAlignment.Stretch;

        public string AutomationCommandName { get; set; }
        public string AutomationCommandValue { get; set; }
        public string StateFilter { get; set; }
        public Dictionary<string, string> Totals { get; set; }

        public void RefreshEntityScreenItems()
        {
            if (SelectedEntityScreen != null) UpdateEntityScreenItems(SelectedEntityScreen);
        }

        private bool CanDecPageNumber(string arg)
        {
            return SelectedEntityScreen != null && CurrentPageNo > 0;
        }

        private void OnDecPageNumber(string obj)
        {
            CurrentPageNo--;
            RefreshEntityScreenItems();
        }

        private bool CanIncPageNumber(string arg)
        {
            return SelectedEntityScreen != null && CurrentPageNo < SelectedEntityScreen.PageCount - 1;
        }

        private void OnIncPageNumber(string obj)
        {
            CurrentPageNo++;
            RefreshEntityScreenItems();
        }

        private void OnSelectEntityExecuted(EntityScreenItemViewModel obj)
        {
            ExtensionMethods.PublishIdEvent(100, EventTopicNames.ClearNumberPadValue);

            var permission = SettingService.ProgramSettings.OpenEntityAuth;
            if (permission)
            {
                var myText = CommandExecutionService.ConfirmUser();
                if (!myText)
                {
                    DialogService.ShowFeedback(LoOv.G(o => Resources.NotAllowed));
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(AutomationCommandName))
            {
                var commandValue = obj.Model.EntityState;
                if (!string.IsNullOrWhiteSpace(AutomationCommandValue))
                {
                    commandValue = AutomationCommandValue;
                    if (commandValue.Contains("{"))
                    {
                        var entity = CacheService.GetEntityById(obj.Model.EntityId);
                        commandValue = PrinterService.ExecuteFunctions(commandValue, entity);
                    }
                }

                ApplicationState.NotifyEvent(RuleEventNames.AutomationCommandExecuted,
                    new
                    {
                        Ticket = Ticket.Empty,
                        obj.Model.EntityId,
                        SelectedEntityScreen.EntityTypeId,
                        AutomationCommandName,
                        CommandValue = commandValue
                    });
                return;
            }

            var myEntity = CacheService.GetEntityById(obj.Model.EntityId);

            var moveNext = true;
            try
            {
                if (LocalSettings.EDine && myEntity != null)
                {

                    int myQuestion = GetQrQuestion();
                    DineUrlObject myCode = null;
                    switch (myQuestion)
                    {
                        case 0:
                            moveNext = false;
                            break;

                        case 1: 
                            myCode = EdineApiService.VacantTable(obj.Model.EntityId);
                            if (myCode != null)
                            {
                                var pTemplate =
                                    CacheService.GetPrinterTemplateByName(SettingService.ProgramSettings
                                        .EDinePrintTemplate);

                                if (pTemplate != null && !string.IsNullOrEmpty(pTemplate.Template))
                                {
                                    var replaceText = pTemplate.Template.Replace("{URL}", myCode.Url);
                                    replaceText = replaceText.Replace("{PinCode}", myCode.Pincode);
                                    replaceText = replaceText.Replace("{EntityName}", myEntity.Name);
                                    replaceText = replaceText.Replace("{Image}", myCode.ImagePath);

                                    var content = replaceText.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                                    PrinterService.PrintReport(content, ApplicationState.GetTransactionPrinter());
                                }
                            }
                            else
                            {
                                DialogService.ShowFeedback("Unable to Receive");
                            }
                            moveNext = false;

                            break;

                        case 2:
                            myCode = EdineApiService.EdineTableUrl(obj.Model.EntityId);
                            if (myCode != null)
                            {
                                var pTemplate =
                                    CacheService.GetPrinterTemplateByName(SettingService.ProgramSettings
                                        .EDinePrintTemplate);

                                if (pTemplate != null && !string.IsNullOrEmpty(pTemplate.Template))
                                {
                                    var replaceText = pTemplate.Template.Replace("{URL}", myCode.Url);
                                    replaceText = replaceText.Replace("{PinCode}", myCode.Pincode);
                                    replaceText = replaceText.Replace("{EntityName}", myEntity.Name);
                                    replaceText = replaceText.Replace("{Image}", myCode.ImagePath);

                                    var content = replaceText.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                                    PrinterService.PrintReport(content, ApplicationState.GetTransactionPrinter());
                                }
                            }
                            else
                            {
                                DialogService.ShowFeedback("Unable to Receive");
                            }
                            moveNext = false;
                            break;

                        default:
                            moveNext = true;
                            break;

                    }

                    
                }
            }
            catch (Exception)
            {
                // ignored
            }


            if (moveNext)
            {
                if (obj.Model.EntityId > 0 && obj.Model.ItemId == 0)
                    _currentOperationRequest.Publish(CacheService.GetEntityById(obj.Model.EntityId));
                else if (obj.Model.ItemId > 0)
                    ExtensionMethods.PublishIdEvent(obj.Model.ItemId, EventTopicNames.DisplayTicket);
            }
        }

        private int GetQrQuestion()
        {
            var builder = new StringBuilder();

            builder.Append("No QR");
            builder.Append("|");
            builder.Append("3");
            builder.Append("|");
            builder.Append(PromotionValueTypeExtensions.GetPromotionGroupName(103));
            builder.Append(",");

            builder.Append("New QR");
            builder.Append("|");
            builder.Append("1");
            builder.Append("|");
            builder.Append(PromotionValueTypeExtensions.GetPromotionGroupName(103));
            builder.Append(",");


            builder.Append("Existing QR");
            builder.Append("|");
            builder.Append("2");
            builder.Append("|");
            builder.Append(PromotionValueTypeExtensions.GetPromotionGroupName(103));
            builder.Append(",");

            builder.Append("Cancel");
            builder.Append("|");
            builder.Append("0");
            builder.Append("|");
            builder.Append(PromotionValueTypeExtensions.GetPromotionGroupName(103));

            var allQuestion =  builder.ToString();

            return Convert.ToInt32(DialogService.AskPromotionQuestion("eDine - QR Ordering", allQuestion,
                "White",
                true,
                false,
                "Magenta"));
        }


        private void UpdateEntityScreenItems(EntityScreen entityScreen)
        {
            var entityData = GetEntityScreenItems(entityScreen, StateFilter);
            if (EntityScreenItems != null &&
                (!EntityScreenItems.Any() || EntityScreenItems.Count != entityData.Count() ||
                 EntityScreenItems.First().Name != entityData.First().Name)) EntityScreenItems = null;

            UpdateEntityButtons(entityData);

            RaisePropertyChanged(nameof(EntityScreenItems));
            RaisePropertyChanged(nameof(SelectedEntityScreen));
            RaisePropertyChanged(nameof(IsPageNavigatorVisible));
            RaisePropertyChanged(nameof(ScreenVerticalAlignment));
        }

        private List<EntityScreenItem> GetEntityScreenItems(EntityScreen entityScreen, string stateFilter)
        {
            if (entityScreen.ScreenItems.Count > 0)
                return
                    EntityService.GetCurrentEntityScreenItems(entityScreen, CurrentPageNo, stateFilter)
                        .OrderBy(x => x.SortOrder)
                        .ToList();
            return
                EntityService.GetEntitiesByState(stateFilter, entityScreen.EntityTypeId)
                    .Select(
                        x =>
                            new EntityScreenItem(CacheService.GetEntityTypeById(entityScreen.EntityTypeId), x,
                                stateFilter)).ToList();
        }

        private void UpdateEntityButtons(ICollection<EntityScreenItem> entityData)
        {
            if (EntityScreenItems == null)
            {
                if (SelectedEntityScreen.RowCount > 0 && SelectedEntityScreen.ColumnCount > 0 &&
                    entityData.Count > SelectedEntityScreen.ColumnCount * SelectedEntityScreen.RowCount)
                    SelectedEntityScreen.RowCount = 0;
                EntityScreenItems = new ObservableCollection<EntityScreenItemViewModel>();
                EntityScreenItems.AddRange(entityData.Select(x => new
                    EntityScreenItemViewModel(x, SelectedEntityScreen,
                        EntitySelectionCommand,
                        _currentOperationRequest != null && _currentOperationRequest.SelectedItem != null,
                        UserService.IsUserPermittedFor(PermissionNames.MergeTickets))));
            }
            else
            {
                for (var i = 0; i < entityData.Count(); i++) EntityScreenItems[i].Model = entityData.ElementAt(i);
            }
        }

        public void Refresh(EntityScreen entityScreen, string stateFilter,
            OperationRequest<Entity> currentOperationRequest)
        {
            StateFilter = stateFilter;
            _currentOperationRequest = currentOperationRequest;
            RefreshEntityScreenItems();
        }

        private void RefreshTotals(object sender, EventArgs e)
        {
            if (ApplicationState.ActiveAppScreen == AppScreens.EntityView && SelectedEntityScreen.RefreshEntity)
                UpdateTotal(EntityScreenItems);
        }

        private void UpdateTotal(ObservableCollection<EntityScreenItemViewModel> entityScreenItems)
        {
            if (entityScreenItems != null)
                foreach (var item in entityScreenItems)
                {
                    var allValue = item.FormattedName.Contains("{")
                        ? PrinterService.ExecuteFunctions(item.FormattedName)
                        : "";
                    if (!string.IsNullOrEmpty(allValue) && allValue.Contains("\\n"))
                        allValue = allValue.Replace(@"\n", Environment.NewLine);
                    Totals[item.Name] = allValue;
                    item.Total = allValue ?? "";
                }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (ApplicationState.ActiveAppScreen == AppScreens.EntityView)
                try
                {
                    RefreshEntityScreenItems();
                }
                catch (Exception)
                {
                    // ignored
                }
        }
    }
}