using DinePlan.Domain.Models;
using DinePlan.Domain.Models.Entities;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;

namespace DinePlan.Modules.EntityModule
{
    [Export]
    public class EntitySearchViewModel : GenericViewModelbase
    {
        private readonly Timer _updateTimer;

        private bool _canCreateNewEntity;

        private OperationRequest<Entity> _currentEntitySelectionRequest;

        private EntitySearchResultViewModel _focusedEntity;

        private bool _isCreateEntityCommandVisible;

        private bool _isDisplayAccountCommandVisible;

        private bool _isEditEntityCommandVisible;

        private bool _isKeyboardVisible;

        private string _searchLabel;

        private string _searchString;

        private EntityType _selectedEntityType;

        [ImportingConstructor]
        public EntitySearchViewModel()
        {
            _updateTimer = new Timer(250);
            _updateTimer.Elapsed += UpdateTimerElapsed;

            IsKeyboardVisible = true;
            IsEditEntityCommandVisible = true;
            IsCreateEntityCommandVisible = true;
            IsDisplayAccountCommandVisible = true;
            SearchLabel = LoOv.G(o => Resources.Search);
            FoundEntities = new ObservableCollection<EntitySearchResultViewModel>();

            SelectEntityCommand = new CaptionCommand<string>("", OnSelectEntity, CanSelectEntity);
            EditEntityCommand = new CaptionCommand<string>("", OnEditEntity, CanEditEntity);
            CreateEntityCommand = new CaptionCommand<string>("", OnCreateEntity, CanCreateEntity);
            DisplayAccountCommand = new CaptionCommand<string>(
                LoOv.G(o => Resources.AccountDetails).Replace(" ", "\r").ToUpper(), OnDisplayAccount,
                CanDisplayAccount);
            DisplayInventoryCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Inventory).ToUpper(),
                OnDisplayInventory, CanDisplayInventory);
            RemoveEntityCommand = new CaptionCommand<string>("", OnRemoveEntity, CanRemoveEntity);

            DisplayTicketCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Tickets).ToUpper(),
                OnClickTicketCommand, CanTicketCommand);
        }

        public DelegateCommand<string> SelectEntityCommand { get; set; }
        public DelegateCommand<string> CreateEntityCommand { get; set; }
        public DelegateCommand<string> EditEntityCommand { get; set; }
        public DelegateCommand<string> RemoveEntityCommand { get; set; }
        public DelegateCommand<string> DisplayTicketCommand { get; set; }

        public ICaptionCommand DisplayAccountCommand { get; set; }
        public ICaptionCommand DisplayInventoryCommand { get; set; }

        public string SelectEntityCommandCaption =>
            string.Format(LoOv.G(o => Resources.Select_f), SelectedEntityName()).ToUpper();

        public string CreateEntityCommandCaption =>
            string.Format(LoOv.G(o => Resources.New_f), SelectedEntityName()).ToUpper();

        public string EditEntityCommandCaption =>
            string.Format(LoOv.G(o => Resources.Edit_f), SelectedEntityName()).ToUpper();

        public string RemoveEntityCommandCaption =>
            string.Format(LoOv.G(o => Resources.No_f), SelectedEntityName()).ToUpper();

        public bool CanCreateNewEntity
        {
            get => _canCreateNewEntity;
            set
            {
                _canCreateNewEntity = value;
                RaisePropertyChanged(nameof(CanCreateNewEntity));
            }
        }

        public string SearchString
        {
            get => string.IsNullOrEmpty(_searchString) ? null : _searchString;
            set
            {
                if (value != _searchString)
                {
                    _searchString = value;
                    if (ApplicationState.SelectedEntityScreen != null &&
                        !string.IsNullOrEmpty(ApplicationState.SelectedEntityScreen.SearchValueReplacePattern))
                        _searchString = Regex.Replace(_searchString,
                            ApplicationState.SelectedEntityScreen.SearchValueReplacePattern,
                            match => match.Groups[1].Value);
                    RaisePropertyChanged(nameof(SearchString));
                    ResetTimer();
                }
            }
        }

        public string SearchLabel
        {
            get => _searchLabel;
            set
            {
                _searchLabel = value;
                RaisePropertyChanged(nameof(SearchLabel));
            }
        }

        public bool IsEditEntityCommandVisible
        {
            get => _isEditEntityCommandVisible;
            set
            {
                _isEditEntityCommandVisible = value;
                RaisePropertyChanged(nameof(IsEditEntityCommandVisible));
            }
        }

        public bool IsCreateEntityCommandVisible
        {
            get => _isCreateEntityCommandVisible;
            set
            {
                _isCreateEntityCommandVisible = value;
                RaisePropertyChanged(nameof(IsCreateEntityCommandVisible));
            }
        }

        public bool IsDisplayAccountCommandVisible
        {
            get => _isDisplayAccountCommandVisible;
            set
            {
                _isDisplayAccountCommandVisible = value;
                RaisePropertyChanged(nameof(IsDisplayAccountCommandVisible));
            }
        }

        protected string StateFilter { get; set; }

        public bool IsKeyboardVisible
        {
            get => _isKeyboardVisible;
            set
            {
                _isKeyboardVisible = value;
                RaisePropertyChanged(nameof(IsKeyboardVisible));
            }
        }

        public string PrimaryFieldName
        {
            get
            {
                return SelectedEntityType != null
                    ? SelectedEntityType.PrimaryFieldName ?? LoOv.G(o => Resources.Name)
                    : "";
            }
        }

        public string PrimaryFieldFormat => SelectedEntityType != null ? SelectedEntityType.PrimaryFieldFormat : null;

        public IEnumerable<EntityType> EntityTypes => CacheService.GetEntityTypes();

        public EntityType SelectedEntityType
        {
            get => _selectedEntityType;
            set
            {
                _selectedEntityType = value;
                ClearSearchValues();
                RaisePropertyChanged(nameof(SelectedEntityType));
                RaisePropertyChanged(nameof(SelectEntityCommandCaption));
                RaisePropertyChanged(nameof(CreateEntityCommandCaption));
                RaisePropertyChanged(nameof(EditEntityCommandCaption));
                RaisePropertyChanged(nameof(RemoveEntityCommandCaption));
                RaisePropertyChanged(nameof(IsEntitySelectorVisible));
                RaisePropertyChanged(nameof(IsEntitySelectVisible));
                RaisePropertyChanged(nameof(IsInventorySelectorVisible));
                RaisePropertyChanged(nameof(PrimaryFieldName));
                RaisePropertyChanged(nameof(PrimaryFieldFormat));
                InvokeSelectedEntityTypeChanged(EventArgs.Empty);
            }
        }

        public ObservableCollection<EntitySearchResultViewModel> FoundEntities { get; set; }

        public EntitySearchResultViewModel SelectedEntity =>
            FoundEntities.Count == 1 ? FoundEntities[0] : FocusedEntity;

        public EntitySearchResultViewModel FocusedEntity
        {
            get => _focusedEntity;
            set
            {
                _focusedEntity = value;
                RaisePropertyChanged(nameof(FocusedEntity));
                RaisePropertyChanged(nameof(SelectedEntity));
            }
        }

        public bool IsEntitySelectorVisible
        {
            get
            {
                if (ApplicationState.SelectedEntityScreen != null)
                {
                    var ticketType = CacheService.GetTicketTypeById(ApplicationState.SelectedEntityScreen.TicketTypeId);
                    var result = SelectedEntityType != null &&
                                 ticketType.EntityTypeAssignments.Any(x => x.EntityTypeId == SelectedEntityType.Id);
                    if (result)
                    {
                        var entityTypeAssi =
                            ticketType.EntityTypeAssignments.FirstOrDefault(
                                a => a.EntityTypeId == SelectedEntityType.Id);
                        if (entityTypeAssi != null) return !entityTypeAssi.AskBeforeCreatingTicket;
                    }
                }

                return false;
            }
        }

        public bool IsEntitySelectVisible
        {
            get
            {
                if (ApplicationState.SelectedEntityScreen != null)
                {
                    var ticketType = CacheService.GetTicketTypeById(ApplicationState.SelectedEntityScreen.TicketTypeId);
                    var result = SelectedEntityType != null &&
                                 ticketType.EntityTypeAssignments.Any(x => x.EntityTypeId == SelectedEntityType.Id);
                    return result;
                }

                return false;
            }
        }

        public bool IsInventorySelectorVisible => SelectedEntityType != null && SelectedEntityType.WarehouseTypeId > 0;

        public event EventHandler SelectedEntityTypeChanged;

        private void InvokeSelectedEntityTypeChanged(EventArgs e)
        {
            var handler = SelectedEntityTypeChanged;
            if (handler != null) handler(this, e);
        }

        private bool CanDisplayInventory(string arg)
        {
            return SelectedEntity != null && SelectedEntity.Model.WarehouseId > 0;
        }

        private void OnDisplayInventory(string obj)
        {
            SelectedEntity.Model.PublishEvent(EventTopicNames.DisplayInventory);
        }

        private string SelectedEntityName()
        {
            return SelectedEntityType != null ? SelectedEntityType.EntityName : LoOv.G(o => Resources.Entity);
        }

        private void OnDisplayAccount(string obj)
        {
            CommonEventPublisher.PublishEntityOperation(new AccountData(SelectedEntity.Model.AccountId),
                EventTopicNames.DisplayAccountTransactions, EventTopicNames.SelectEntity);
        }

        private bool CanDisplayAccount(string arg)
        {
            return SelectedEntity != null && SelectedEntity.Model.AccountId > 0;
        }

        private bool CanEditEntity(string arg)
        {
            return SelectedEntity != null;
        }

        private void OnEditEntity(string obj)
        {
            var targetEvent = _currentEntitySelectionRequest != null
                ? _currentEntitySelectionRequest.GetExpectedEvent()
                : EventTopicNames.SelectEntity;

            CommonEventPublisher.PublishEntityOperation(SelectedEntity.Model,
                EventTopicNames.EditEntityDetails, targetEvent);
        }

        private void OnRemoveEntity(string obj)
        {
            _currentEntitySelectionRequest.Publish(Entity.GetNullEntity(SelectedEntityType.Id));
        }

        private bool CanRemoveEntity(string arg)
        {
            return ApplicationState.IsLocked && _currentEntitySelectionRequest != null &&
                   _currentEntitySelectionRequest.SelectedItem != null &&
                   (SelectedEntity != null && _currentEntitySelectionRequest.SelectedItem.Id == SelectedEntity.Id ||
                    _currentEntitySelectionRequest.SelectedItem.Id == 0 ||
                    _currentEntitySelectionRequest.SelectedItem == null);
        }

        private bool CanCreateEntity(string arg)
        {
            return SelectedEntityType != null && CanCreateNewEntity;
        }

        private void OnCreateEntity(string obj)
        {
            var targetEvent = _currentEntitySelectionRequest != null
                ? _currentEntitySelectionRequest.GetExpectedEvent()
                : EventTopicNames.SelectEntity;
            var newEntity = new Entity { EntityTypeId = SelectedEntityType.Id };
            newEntity.SetDefaultValues(SearchString);
            ClearSearchValues();
            CommonEventPublisher.PublishEntityOperation(newEntity, EventTopicNames.EditEntityDetails, targetEvent);
        }

        private bool CanSelectEntity(string arg)
        {
            return
                SelectedEntity != null
                && ApplicationState.IsCurrentWorkPeriodOpen
                && ApplicationState.CurrentDepartment != null
                && !string.IsNullOrEmpty(SelectedEntity.Name);
        }

        private void OnSelectEntity(string obj)
        {
            if (_currentEntitySelectionRequest != null) _currentEntitySelectionRequest.Publish(SelectedEntity.Model);
            ClearSearchValues();
        }

        private void OnClickTicketCommand(string obj)
        {
            CommonEventPublisher.PublishEntityOperation(SelectedEntity.Model, EventTopicNames.DisplayTicketExplorer,
                EventTopicNames.SelectEntity);
        }

        private bool CanTicketCommand(string arg)
        {
            return SelectedEntity != null && SelectedEntity.Model.Id > 0;
        }


        public void RefreshSelectedEntity(OperationRequest<Entity> value)
        {
            ClearSearchValues();
            _currentEntitySelectionRequest = value;

            if (_currentEntitySelectionRequest != null && _currentEntitySelectionRequest.SelectedItem != null &&
                !string.IsNullOrEmpty(_currentEntitySelectionRequest.SelectedItem.Name))
            {
                ClearSearchValues();
                if (SelectedEntityType != null)
                    if (_currentEntitySelectionRequest.SelectedItem.Name != "*" &&
                        _currentEntitySelectionRequest.SelectedItem.EntityTypeId == SelectedEntityType.Id)
                        FoundEntities.Add(new EntitySearchResultViewModel(_currentEntitySelectionRequest.SelectedItem,
                            SelectedEntityType));
            }

            RaisePropertyChanged(nameof(SelectedEntityType));
            RaisePropertyChanged(nameof(SelectedEntity));
            RaisePropertyChanged(nameof(EntityTypes));
        }

        public void ResetSearch()
        {
            if (_currentEntitySelectionRequest != null)
                _currentEntitySelectionRequest.SelectedItem = Entity.GetNullEntity(SelectedEntityType.Id);
            RefreshSelectedEntity(_currentEntitySelectionRequest);
        }

        internal void ClearSearchValues()
        {
            FoundEntities.Clear();
            SearchString = "";
        }

        private void ResetTimer()
        {
            CanCreateNewEntity = true;
            _updateTimer.Stop();
            if (!string.IsNullOrEmpty(SearchString))
            {
                CanCreateNewEntity = false;
                _updateTimer.Start();
            }
            else
            {
                FoundEntities.Clear();
            }
        }

        private void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _updateTimer.Stop();
            UpdateFoundEntities();
        }

        private void UpdateFoundEntities()
        {
            CanCreateNewEntity = true;
            var result = new List<Entity>();

            using (var worker = new BackgroundWorker())
            {
                worker.DoWork += delegate
                {
                    LocalSettings.UpdateThreadLanguage();
                    result = SearchEntities(SelectedEntityType, SearchString, StateFilter);
                };

                worker.RunWorkerCompleted +=
                    delegate
                    {
                        ApplicationState.MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                            delegate
                            {
                                FoundEntities.Clear();
                                FoundEntities.AddRange(result.Select(x =>
                                    new EntitySearchResultViewModel(x, SelectedEntityType)));
                                RaisePropertyChanged(nameof(SelectedEntity));
                                CommandManager.InvalidateRequerySuggested();
                            }));
                    };

                worker.RunWorkerAsync();
            }
        }

        /// <summary>
        ///     Searches the entity.
        /// </summary>
        /// <returns></returns>
        protected virtual List<Entity> SearchEntities(EntityType entityType, string searchString, string state)
        {
            return EntityService.SearchEntities(entityType, searchString, state);
        }

        public void Refresh(int entityType, string stateFilter, OperationRequest<Entity> currentOperationRequest)
        {
            StateFilter = stateFilter;
            SelectedEntityType = CacheService.GetEntityTypeById(entityType);
            RefreshSelectedEntity(currentOperationRequest);
        }

        public void SelectFullMatch()
        {
            if (_updateTimer.Enabled)
            {
                _updateTimer.Stop();
                UpdateFoundEntities();
            }

            if (FoundEntities.Count > 1 && FoundEntities.Any(x => x.Name == SearchString))
            {
                var f = FoundEntities.First(x => x.Name == SearchString);
                FoundEntities.Clear();
                FoundEntities.Add(f);
            }

            if (SelectEntityCommand.CanExecute(""))
                SelectEntityCommand.Execute("");
        }
    }
}