using DinePlan.Domain.Models.Entities;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DinePlan.Modules.EntityModule.ViewModel;

namespace DinePlan.Modules.EntityModule
{
    [Export]
    public class EntitySwitcherViewModel : GenericViewModelbase
    {
        private OperationRequest<Entity> _currentOperationRequest;

        private IEnumerable<EntityScreen> _entityScreens;

        private List<EntitySwitcherButtonViewModel> _entitySwitcherButtons;

        /// <summary>
        ///     The back up value for <see cref="EntityDashboardView" /> property.
        /// </summary>
        private EntityDashboardView entityDashboardView;

        /// <summary>
        ///     The back up value for <see cref="EntityDashboardViewModel" /> property.
        /// </summary>
        private EntityDashboardViewModel entityDashboardViewModel;

        /// <summary>
        ///     The back up value for <see cref="EntitySearchView" /> property.
        /// </summary>
        private EntitySearchView entitySearchView;

        /// <summary>
        ///     The back up value for <see cref="EntitySearchViewModel" /> property.
        /// </summary>
        private EntitySearchViewModel entitySearchViewModel;

        /// <summary>
        ///     The back up value for <see cref="EntitySelectorView" /> property.
        /// </summary>
        private Views.EntitySelectorView entitySelectorView;

        /// <summary>
        ///     The back up value for <see cref="EntitySelectorViewModel" /> property.
        /// </summary>
        private EntitySelectorViewModel entitySelectorViewModel;

        /// <summary>
        ///     The back up value for <see cref="SelectScreen" /> property.
        /// </summary>
        private EntityScreen selectedScreen;

        [ImportingConstructor]
        public EntitySwitcherViewModel()
        {
            SelectEntityCategoryCommand = new DelegateCommand<EntityScreen>(OnSelectEntityCategoryExecuted);
            EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.ResetCache)
                    {
                        _entityScreens = null;
                        _entitySwitcherButtons = null;
                        RaisePropertyChanged(nameof(EntitySwitcherButtons));
                    }
                });

            EventServiceFactory.EventService.GetEvent<GenericEvent<OperationRequest<Entity>>>().Subscribe(x =>
            {
                if (x.Topic == EventTopicNames.SelectEntity)
                {
                    var ss = UpdateEntityScreens(x.Value);
                    _currentOperationRequest = x.Value;
                    ActivateEntityScreen(ss);
                    if (ss != null && ss.DisplayMode == 1)
                        EntitySearchViewModel.SearchString = x.Value.Data;
                }
            });
        }

        /// <summary>
        ///     Gets or sets the SelectScreen.
        /// </summary>
        public EntityScreen SelectScreen
        {
            get => selectedScreen;

            set
            {
                if (selectedScreen != value)
                {
                    selectedScreen = value;
                    RaisePropertyChanged(nameof(SelectScreen));
                }
            }
        }

        public DelegateCommand<EntityScreen> SelectEntityCategoryCommand { get; set; }

        public IEnumerable<EntityScreen> EntityScreens
        {
            get
            {
                if (ApplicationState.CurrentDepartment == null) return new List<EntityScreen>();
                return _entityScreens ??
                       (_entityScreens = ApplicationState.GetEntityScreens().OrderBy(x => x.SortOrder));
            }
        }

        public List<EntitySwitcherButtonViewModel> EntitySwitcherButtons
        {
            get
            {
                return _entitySwitcherButtons ?? (_entitySwitcherButtons = EntityScreens.Select(
                    x => new EntitySwitcherButtonViewModel(x, EntityScreens.Count() > 1)).ToList());
            }
        }

        /// <summary>
        ///     Gets or sets the EntitySelectorView service.
        /// </summary>
        protected Views.EntitySelectorView EntitySelectorView
        {
            get
            {
                if (entitySelectorView == null)
                    entitySelectorView = ServiceLocator.Current.GetInstance<Views.EntitySelectorView>();

                return entitySelectorView;
            }
        }

        /// <summary>
        ///     Gets or sets the EntitySelectorViewModel service.
        /// </summary>
        protected EntitySelectorViewModel EntitySelectorViewModel
        {
            get
            {
                if (entitySelectorViewModel == null)
                    entitySelectorViewModel = ServiceLocator.Current.GetInstance<EntitySelectorViewModel>();

                return entitySelectorViewModel;
            }
        }

        /// <summary>
        ///     Gets or sets the EntitySearchView service.
        /// </summary>
        protected EntitySearchView EntitySearchView
        {
            get
            {
                if (entitySearchView == null) entitySearchView = ServiceLocator.Current.GetInstance<EntitySearchView>();

                return entitySearchView;
            }
        }

        /// <summary>
        ///     Gets or sets the EntitySearchViewModel service.
        /// </summary>
        protected EntitySearchViewModel EntitySearchViewModel
        {
            get
            {
                if (entitySearchViewModel == null)
                    entitySearchViewModel = ServiceLocator.Current.GetInstance<EntitySearchViewModel>();

                return entitySearchViewModel;
            }
        }

        /// <summary>
        ///     Gets or sets the EntityDashboardView service.
        /// </summary>
        protected EntityDashboardView EntityDashboardView
        {
            get
            {
                if (entityDashboardView == null)
                    entityDashboardView = ServiceLocator.Current.GetInstance<EntityDashboardView>();

                return entityDashboardView;
            }
        }

        /// <summary>
        ///     Gets or sets the EntityDashboardViewModel service.
        /// </summary>
        protected EntityDashboardViewModel EntityDashboardViewModel
        {
            get
            {
                if (entityDashboardViewModel == null)
                    entityDashboardViewModel = ServiceLocator.Current.GetInstance<EntityDashboardViewModel>();

                return entityDashboardViewModel;
            }
        }

        private EntityScreen UpdateEntityScreens(OperationRequest<Entity> value)
        {
            var entityScreens =
                ApplicationState.IsLocked
                    ? ApplicationState.GetTicketEntityScreens().ToList()
                    : ApplicationState.GetEntityScreens().ToList();
            if (!entityScreens.Any()) return null;
            _entityScreens = entityScreens.OrderBy(x => x.SortOrder).ToList();
            _entitySwitcherButtons = null;
            var selectedScreen = ApplicationState.SelectedEntityScreen;
            if (value != null && value.SelectedItem != null && ApplicationState.CurrentDepartment != null)
            {
                if (ApplicationState.IsLocked || ApplicationState.CurrentDepartment.TicketCreationMethod == 1)
                    _entityScreens = _entityScreens.Where(x => x.EntityTypeId == value.SelectedItem.EntityTypeId)
                        .OrderBy(x => x.SortOrder);
                if (!_entityScreens.Any())
                {
                    var screens = CacheService.GetEntityScreensByEntityType(value.SelectedItem.EntityTypeId);
                    return screens.FirstOrDefault() ?? entityScreens.ElementAt(0);
                }

                if (selectedScreen == null || selectedScreen.EntityTypeId != value.SelectedItem.EntityTypeId)
                {
                    selectedScreen = null;
                    if (!string.IsNullOrEmpty(value.Data))
                        selectedScreen = _entityScreens.Where(x => x.DisplayMode == 1)
                            .FirstOrDefault(x => x.EntityTypeId == value.SelectedItem.EntityTypeId);
                    if (selectedScreen == null)
                        selectedScreen =
                            _entityScreens.FirstOrDefault(x => x.EntityTypeId == value.SelectedItem.EntityTypeId);
                }

                if (selectedScreen == null)
                {
                    var screens = CacheService.GetEntityScreensByEntityType(value.SelectedItem.EntityTypeId);
                    selectedScreen = screens.FirstOrDefault() ?? _entityScreens.ElementAt(0);
                }
            }

            return selectedScreen ?? EntityScreens.ElementAt(0);
        }

        private void OnSelectEntityCategoryExecuted(EntityScreen obj)
        {
            ActivateEntityScreen(obj);
        }

        private void ActivateEntityScreen(EntityScreen entityScreen)
        {
            entityScreen = ApplicationStateSetter.SetSelectedEntityScreen(entityScreen);
            ApplicationStateSetter.SetCurrentTicketType(entityScreen != null
                ? CacheService.GetTicketTypeById(entityScreen.TicketTypeId)
                : null);

            SelectScreen = entityScreen;

            if (entityScreen != null)
            {
                if (entityScreen.DisplayMode == 1)
                    ActivateEntitySearcher(entityScreen);
                else if (entityScreen.DisplayMode == 2)
                    ActivateDashboard(entityScreen);
                else
                    ActivateButtonSelector(entityScreen);
            }

            RaisePropertyChanged(nameof(EntitySwitcherButtons));
            EntitySwitcherButtons.ForEach(x => x.Refresh());
        }

        private void ActivateDashboard(EntityScreen entityScreen)
        {
            EntityDashboardViewModel.Refresh(entityScreen, _currentOperationRequest);
            RegionManager.ActivateRegion(RegionNames.EntityScreenRegion, EntityDashboardView);
        }

        private void ActivateEntitySearcher(EntityScreen entityScreen)
        {
            EntitySearchViewModel.Refresh(entityScreen.EntityTypeId, entityScreen.StateFilter,
                _currentOperationRequest);
            RegionManager.ActivateRegion(RegionNames.EntityScreenRegion, EntitySearchView);
        }

        private void ActivateButtonSelector(EntityScreen entityScreen)
        {
            EntitySelectorViewModel.Refresh(entityScreen, entityScreen.StateFilter, _currentOperationRequest);
            RegionManager.ActivateRegion(RegionNames.EntityScreenRegion, EntitySelectorView);
        }
    }
}