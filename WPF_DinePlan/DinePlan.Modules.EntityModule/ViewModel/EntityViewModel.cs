using DinePlan.Domain.Models.Entities;
using DinePlan.Domain.Models.Inventory;
using DinePlan.Infrastructure.Data;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common.DataGeneration;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.EntityModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EntityViewModel : EntityViewModelBase<Entity>, IEntityCreator<Entity>
    {
        private string _accountName;

        private EntityCustomDataViewModel _customDataViewModel;

        private EntityType _entityType;

        private IEnumerable<EntityType> _entityTypes;

        [ImportingConstructor]
        public EntityViewModel()
        {
        }

        public IEnumerable<EntityType> EntityTypes => _entityTypes ?? (_entityTypes = CacheService.GetEntityTypes());

        public EntityType EntityType
        {
            get => _entityType ?? (_entityType = CacheService.GetEntityTypeById(Model.EntityTypeId));
            set
            {
                Model.EntityTypeId = value.Id;
                _entityType = null;
                _customDataViewModel = null;
                RaisePropertyChanged(nameof(CustomDataViewModel));
                RaisePropertyChanged(nameof(EntityType));
            }
        }

        public EntityCustomDataViewModel CustomDataViewModel => _customDataViewModel ??
                                                                (_customDataViewModel = Model != null
                                                                    ? new EntityCustomDataViewModel(Model, EntityType)
                                                                    : null);

        public string AccountName
        {
            get => _accountName ?? (_accountName = AccountService.GetAccountNameById(Model.AccountId));
            set
            {
                _accountName = value;
                Model.AccountId = AccountService.GetAccountIdByName(value);
                if (Model.AccountId == 0)
                    RaisePropertyChanged(nameof(AccountNames));
                _accountName = null;
                RaisePropertyChanged(nameof(AccountName));
            }
        }

        public IEnumerable<string> AccountNames
        {
            get
            {
                if (EntityType == null) return null;
                return AccountService.GetCompletingAccountNames(EntityType.AccountTypeId, AccountName);
            }
        }

        public Warehouse Warehouse
        {
            get { return Warehouses.SingleOrDefault(x => x.Id == Model.WarehouseId); }
            set => Model.WarehouseId = value != null ? value.Id : 0;
        }

        public IEnumerable<Warehouse> Warehouses
        {
            get
            {
                var whId = EntityType != null ? EntityType.WarehouseTypeId : 0;
                return Workspace.Query<Warehouse>(x => x.WarehouseTypeId == whId);
            }
        }

        public string GroupValue => NameCache.GetName<EntityType>(Model.EntityTypeId);

        public IEnumerable<Entity> CreateItems(IEnumerable<string> data)
        {
            return new DataCreationService().BatchCreateEntities(data.ToArray(), Workspace);
        }

        public override Type GetViewType()
        {
            return typeof(EntityView);
        }

        public override string GetModelTypeString()
        {
            return LoOv.G(o => Resources.Entity);
        }

        protected override AbstractValidator<Entity> GetValidator()
        {
            return new EntityValidator();
        }

        protected override void OnSave(string value)
        {
            CustomDataViewModel.Update();
            if (Model.Id > 0)
            {
                var screenItems = Workspace.All<EntityScreenItem>(x => x.EntityId == Model.Id);
                foreach (var entityScreenItem in screenItems) entityScreenItem.Name = Model.Name;
            }

            base.OnSave(value);
        }
    }

    internal class EntityValidator : EntityValidator<Entity>
    {
        public EntityValidator()
        {
            RuleFor(x => x.EntityTypeId).GreaterThan(0);
        }
    }
}