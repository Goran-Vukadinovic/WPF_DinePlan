using DinePlan.Domain.Models;
using DinePlan.Domain.Models.Entities;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Prism.Mef.Modularity;
using Prism.Modularity; //using Prism.Regions;
using System.ComponentModel.Composition;
using DinePlan.Modules.EntityModule.ViewModel;

namespace DinePlan.Modules.EntityModule
{
    [ModuleExport(typeof(EntityModule), InitializationMode = InitializationMode.OnDemand)]
    internal class EntityModule : ModuleBase
    {
        /// <summary>
        ///     The back up value for <see cref="EntityEditorView" /> property.
        /// </summary>
        private EntityEditorView entityEditorView;

        /// <summary>
        ///     The back up value for <see cref="EntitySwitcherView" /> property.
        /// </summary>
        private Views.EntitySwitcherView entitySwitcherView;

        [ImportingConstructor]
        public EntityModule()
        {
            AddDashboardCommand<EntityCollectionViewModelBase<EntityTypeViewModel, EntityType>>(
                LoOv.K(o => Resources.EntityType), LoOv.K(o => Resources.Entities), 40, true);
            AddDashboardCommand<EntityCollectionViewModelBase<EntityViewModel, Entity>>(LoOv.K(o => Resources.Entity),
                LoOv.K(o => Resources.Entities), 40, true);
            AddDashboardCommand<EntityCollectionViewModelBase<EntityScreenViewModel, EntityScreen>>(
                LoOv.K(o => Resources.EntityScreen), LoOv.K(o => Resources.Entities), 41, true);
            AddDashboardCommand<BatchEntityEditorViewModel>(LoOv.K(o => Resources.BatchEntityEditor),
                LoOv.K(o => Resources.Entities), 40);
            
          

        }
        /// <summary>
        ///     Gets or sets the EntitySwitcherView service.
        /// </summary>
        protected Views.EntitySwitcherView EntitySwitcherView
        {
            get
            {
                if (entitySwitcherView == null)
                    entitySwitcherView = ServiceLocator.Current.GetInstance<Views.EntitySwitcherView>();

                return entitySwitcherView;
            }
        }

        /// <summary>
        ///     Gets or sets the EntityEditorView service.
        /// </summary>
        protected EntityEditorView EntityEditorView
        {
            get
            {
                if (entityEditorView == null) entityEditorView = ServiceLocator.Current.GetInstance<EntityEditorView>();

                return entityEditorView;
            }
        }

        protected override void OnInitialization()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(Views.EntitySwitcherView));
            RegionManager.RegisterViewWithRegion(RegionNames.EntityScreenRegion, typeof(Views.EntitySelectorView));
            RegionManager.RegisterViewWithRegion(RegionNames.EntityScreenRegion, typeof(EntitySearchView));
            RegionManager.RegisterViewWithRegion(RegionNames.EntityScreenRegion, typeof(EntityEditorView));
            RegionManager.RegisterViewWithRegion(RegionNames.EntityScreenRegion, typeof(EntityDashboardView));

            EventServiceFactory.EventService.GetEvent<GenericEvent<OperationRequest<Entity>>>().Subscribe(x =>
            {
                if (x.Topic == EventTopicNames.SelectEntity) ActivateEntitySwitcher();
                if (x.Topic == EventTopicNames.EditEntityDetails) ActivateEntityEditor();
            });

            EventServiceFactory.EventService.GetEvent<GenericEvent<OperationRequest<AccountData>>>().Subscribe(x =>
            {
                if (x.Topic == EventTopicNames.SelectEntity) ActivateEntitySwitcher();
            });
        }

        private void ActivateEntityEditor()
        {
            ApplicationStateSetter.SetCurrentApplicationScreen(AppScreens.EntityView);
            RegionManager.Regions[RegionNames.EntityScreenRegion].Activate(EntityEditorView);
        }

        private void ActivateEntitySwitcher()
        {
            ApplicationStateSetter.SetCurrentApplicationScreen(AppScreens.EntityView);
            RegionManager.Regions[RegionNames.MainRegion].Activate(EntitySwitcherView);
        }
    }
}