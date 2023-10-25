using DinePlan.Domain.Models.Entities;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.EntityModule
{
    [Export]
    public class EntityEditorViewModel : GenericViewModelbase
    {
        private OperationRequest<Entity> _operationRequest;

        private EntitySearchResultViewModel _selectedEntity;

        [ImportingConstructor]
        public EntityEditorViewModel()
        {
            SaveEntityCommand =
                new CaptionCommand<string>(LoOv.G(o => Resources.Save).ToUpper(), OnSaveEntity, CanSelectEntity);
            SelectEntityCommand = new CaptionCommand<string>(
                string.Format(LoOv.G(o => Resources.Select_f), LoOv.G(o => Resources.Entity)).ToUpper(), OnSelectEntity,
                CanSelectEntity);
            CreateAccountCommand = new CaptionCommand<string>(
                string.Format(LoOv.G(o => Resources.Create_f), LoOv.G(o => Resources.Account)).ToUpper(),
                OnCreateAccount, CanCreateAccount);
            EventServiceFactory.EventService.GetEvent<GenericEvent<OperationRequest<Entity>>>().Subscribe(OnEditEntity);
        }

        public ICaptionCommand SaveEntityCommand { get; set; }
        public ICaptionCommand SelectEntityCommand { get; set; }
        public ICaptionCommand CreateAccountCommand { get; set; }

        public bool IsEntitySelectorVisible
        {
            get
            {
                if (ApplicationState.SelectedEntityScreen != null)
                {
                    var ticketType = CacheService.GetTicketTypeById(ApplicationState.SelectedEntityScreen.TicketTypeId);
                    return ticketType.EntityTypeAssignments.Any(x => x.EntityTypeId == SelectedEntity.EntityType.Id);
                }

                return false;
            }
        }

        public string SelectEntityCommandCaption =>
            string.Format(LoOv.G(o => Resources.Select_f), SelectedEntityName()).ToUpper();

        public EntitySearchResultViewModel SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                _selectedEntity = value;
                SelectedEntity.AccountCustomDataViewModel.UpdateNewEntityQueryFields();
                RaisePropertyChanged(nameof(SelectedEntity));
                RaisePropertyChanged(nameof(SelectEntityCommandCaption));
            }
        }

        public EntityCustomDataViewModel CustomDataViewModel { get; set; }

        private bool CanCreateAccount(string arg)
        {
            if (CustomDataViewModel == null) return false;
            CustomDataViewModel.Update();
            return SelectedEntity != null && SelectedEntity.Model.AccountId == 0 &&
                   SelectedEntity.EntityType.AccountTypeId > 0 &&
                   !string.IsNullOrEmpty(SelectedEntity.EntityType.GenerateAccountName(SelectedEntity.Model));
        }

        private void OnCreateAccount(string obj)
        {
            if (SelectedEntity.Model.Id == 0 && !SaveSelectedEntity()) return;
            CreateAccount();
            CommonEventPublisher.PublishEntityOperation(SelectedEntity.Model, EventTopicNames.SelectEntity,
                EventTopicNames.EntitySelected);
        }

        private void CreateAccount()
        {
            var str = SelectedEntity.EntityType.GenerateAccountName(SelectedEntity.Model);
            var num = AccountService.CreateAccount(SelectedEntity.EntityType.AccountTypeId, str);
            SelectedEntity.Model.AccountId = num;
            SaveSelectedEntity();
            TicketServiceBase.UpdateAccountOfOpenTickets(SelectedEntity.Model, ApplicationState.LastTicketId);
        }

        private bool CanSelectEntity(string arg)
        {
            return SelectedEntity != null && !string.IsNullOrEmpty(SelectedEntity.Name);
        }

        private void OnSelectEntity(string obj)
        {
            if (SaveSelectedEntity()) _operationRequest.Publish(SelectedEntity.Model);
        }

        private void OnSaveEntity(string obj)
        {
            if (SaveSelectedEntity())
            {
                var targetEvent = _operationRequest != null
                    ? _operationRequest.GetExpectedEvent()
                    : EventTopicNames.EntitySelected;

                CommonEventPublisher.PublishEntityOperation(SelectedEntity.Model, EventTopicNames.SelectEntity,
                    targetEvent);
            }
        }

        private bool SaveSelectedEntity()
        {
            if (!EntityService.EntityExists(SelectedEntity.Model))
            {
                CustomDataViewModel.Update();
                EntityService.SaveEntity(SelectedEntity.Model);
                return true;
            }

            UserInteraction.ShowMessage(string.Format(LoOv.G(o => Resources.EntityNameAlreadyExists_f),
                SelectedEntity.EntityType.EntityName, SelectedEntity.Model.Name));

            return false;
        }

        private void OnEditEntity(EventParameters<OperationRequest<Entity>> obj)
        {
            if (obj.Topic == EventTopicNames.EditEntityDetails)
            {
                _operationRequest = obj.Value;
                var entityType = CacheService.GetEntityTypeById(obj.Value.SelectedItem.EntityTypeId);
                SelectedEntity = new EntitySearchResultViewModel(obj.Value.SelectedItem, entityType);
                CustomDataViewModel = new EntityCustomDataViewModel(obj.Value.SelectedItem, entityType);
                RaisePropertyChanged(nameof(CustomDataViewModel));
                RaisePropertyChanged(nameof(IsEntitySelectorVisible));
            }
        }

        private string SelectedEntityName()
        {
            return SelectedEntity != null ? SelectedEntity.EntityType.EntityName : LoOv.G(o => Resources.Entity);
        }
    }
}