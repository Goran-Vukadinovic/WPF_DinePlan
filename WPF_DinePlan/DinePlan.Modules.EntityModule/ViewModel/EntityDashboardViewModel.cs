using DinePlan.Domain.Models.Entities;
using DinePlan.Infrastructure.Messaging;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Common.Services;
using DinePlan.Presentation.Common.Widgets;
using DinePlan.Presentation.Services.Common;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.EntityModule
{
    [Export]
    public class EntityDashboardViewModel : GenericViewModelbase
    {
        private EntityScreen _currentEntityScreen;
        private bool _isDesignModeActive;

        [ImportingConstructor]
        public EntityDashboardViewModel()
        {
            EventServiceFactory.EventService.GetEvent<GenericEvent<Message>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.MessageReceivedEvent &&
                        x.Value.Command == Messages.WidgetRefreshMessage)
                        Widgets.Where(y => y.IsVisible && y.CreatorName == x.Value.Data)
                            .ToList()
                            .ForEach(y => y.Refresh());
                });
        }

        public EntityScreen SelectedEntityScreen => ApplicationState.SelectedEntityScreen;

        public bool CanDesignEntityScreenItems => ApplicationState.CurrentLoggedInUser.UserRole.IsAdmin;

        public bool IsDesignModeActive
        {
            get => _isDesignModeActive;
            set
            {
                _isDesignModeActive = value;
                RaisePropertyChanged(nameof(IsDesignModeActive));
            }
        }

        public ObservableCollection<IDiagram> Widgets { get; set; }

        public void RemoveWidget(IDiagram viewModel)
        {
            if (viewModel != null && InteractionService.UserIntraction.AskQuestion("Delete Widget?"))
            {
                Widgets.Remove(viewModel);
                EntityService.RemoveWidget(viewModel.GetWidget());
                RaisePropertyChanged(nameof(Widgets));
                SaveTrackableEntityScreenItems();
                LoadTrackableEntityScreenItems();
            }
        }

        public void Refresh(EntityScreen entityScreen)
        {
            if (entityScreen == null) return;
            EntityService.UpdateEntityScreen(entityScreen);
            EntityService.UpdateEntityScreenItemBadges(entityScreen.ScreenItems);
            if (_currentEntityScreen != entityScreen || Widgets == null)
            {
                _currentEntityScreen = entityScreen;
                Widgets?.Cast<ObservableObject>().ToList().ForEach(x => x.DisposeObject());
                Widgets = new ObservableCollection<IDiagram>(
                    from x in entityScreen.Widgets
                    select WidgetCreatorRegistry.CreateWidgetViewModel(x, ApplicationState));
            }

            Widgets.Where(x => x.AutoRefresh).ToList().ForEach(x => x.Refresh());
            RaisePropertyChanged(nameof(Widgets));
            RaisePropertyChanged(nameof(SelectedEntityScreen));
        }

        public void Refresh(EntityScreen entityScreen, OperationRequest<Entity> currentOperationRequest)
        {
            EntityService.UpdateEntityScreen(entityScreen);
            if (_currentEntityScreen != entityScreen || Widgets == null)
            {
                _currentEntityScreen = entityScreen;
                Widgets =
                    new ObservableCollection<IDiagram>(
                        entityScreen.Widgets.Select(
                            x => WidgetCreatorRegistry.CreateWidgetViewModel(x, ApplicationState)));
            }

            Widgets.Where(x => x.AutoRefresh).ToList().ForEach(x => x.Refresh());
            RaisePropertyChanged(nameof(Widgets));
            RaisePropertyChanged(nameof(SelectedEntityScreen));
        }

        internal void AddWidget(Widget widget)
        {
            EntityService.AddWidgetToEntityScreen(SelectedEntityScreen.Name, widget);
            Widgets.Add(WidgetCreatorRegistry.CreateWidgetViewModel(widget, ApplicationState));
            Refresh(SelectedEntityScreen);
        }

        private void refreshWidgets_Tick(object sender, EventArgs e)
        {
            if (ApplicationState.ActiveAppScreen == AppScreens.EntityView)
            {
                RefreshEntityScreenItems();
                RaisePropertyChanged(nameof(Widgets));
                RaisePropertyChanged(nameof(SelectedEntityScreen));
            }
        }

        public void RefreshEntityScreenItems()
        {
            Widgets.Where(x => x.AutoRefresh).ToList().ForEach(x => x.Refresh());
        }

        public void AddWidget(string creatorName)
        {
            var widget = WidgetCreatorRegistry.CreateWidgetFor(creatorName);
            EntityService.AddWidgetToEntityScreen(SelectedEntityScreen.Name, widget);
            widget.Height = 100;
            widget.Width = 100;
            widget.AutoRefresh = true;
            Widgets.Add(WidgetCreatorRegistry.CreateWidgetViewModel(widget, ApplicationState));
        }

        public void LoadTrackableEntityScreenItems()
        {
            IsDesignModeActive = true;
            Widgets =
                new ObservableCollection<IDiagram>(
                    EntityService.LoadWidgets(SelectedEntityScreen.Name)
                        .Select(x => WidgetCreatorRegistry.CreateWidgetViewModel(x, ApplicationState)));
            Widgets.ToList().ForEach(x => x.DesignMode = true);
            RaisePropertyChanged(nameof(Widgets));
        }

        public void SaveTrackableEntityScreenItems()
        {
            ApplicationState.ResetState();
            EventServiceFactory.EventService.PublishEvent(EventTopicNames.ResetCache, true);
            IsDesignModeActive = false;
            Widgets.ToList().ForEach(x => x.SaveSettings());
            Widgets.ToList().ForEach(x => x.DesignMode = false);
            EntityService.SaveEntityScreenItems();
            Widgets.ToList().ForEach(x => x.Refresh());
            RaisePropertyChanged(nameof(Widgets));
        }
    }
}