using DinePlan.Domain.Models.Accounts;
using DinePlan.Domain.Models.Entities;
using DinePlan.Domain.Models.Inventory;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.EntityModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EntityTypeViewModel : EntityViewModelBase<EntityType>
    {
        private IEnumerable<AccountType> _accountTypes;

        private ObservableCollection<EntityCustomFieldViewModel> _entityCustomFields;

        private IEnumerable<WarehouseType> _warehouseTypes;

        [ImportingConstructor]
        public EntityTypeViewModel()
        {
            AddCustomFieldCommand = new CaptionCommand<string>(
                string.Format(LoOv.G(o => Resources.Add_f), LoOv.G(o => Resources.CustomField)), OnAddCustomField);
            DeleteCustomFieldCommand = new CaptionCommand<EntityCustomFieldViewModel>(
                string.Format(LoOv.G(o => Resources.Delete_f), LoOv.G(o => Resources.CustomField)), OnDeleteCustomField,
                CanDeleteCustomField);
        }

        public string AccountNameTemplate
        {
            get => Model.AccountNameTemplate;
            set => Model.AccountNameTemplate = value;
        }

        public string EntityName
        {
            get => Model.EntityName;
            set => Model.EntityName = value;
        }

        public IEnumerable<AccountType> AccountTypes => _accountTypes ?? (_accountTypes = Workspace.All<AccountType>());

        public AccountType AccountType
        {
            get { return AccountTypes.SingleOrDefault(x => x.Id == Model.AccountTypeId); }
            set => Model.AccountTypeId = value != null ? value.Id : 0;
        }

        public IEnumerable<WarehouseType> WarehouseTypes =>
            _warehouseTypes ?? (_warehouseTypes = Workspace.All<WarehouseType>());

        public WarehouseType WarehouseType
        {
            get { return WarehouseTypes.SingleOrDefault(x => x.Id == Model.WarehouseTypeId); }
            set => Model.WarehouseTypeId = value != null ? value.Id : 0;
        }

        public string PrimaryFieldName
        {
            get => Model.PrimaryFieldName;
            set => Model.PrimaryFieldName = value;
        }

        public string PrimaryFieldFormat
        {
            get => Model.PrimaryFieldFormat;
            set => Model.PrimaryFieldFormat = value;
        }

        public string AccountBalanceDisplayFormat
        {
            get => Model.AccountBalanceDisplayFormat;
            set => Model.AccountBalanceDisplayFormat = value;
        }

        public string DisplayFormat
        {
            get => Model.DisplayFormat;
            set => Model.DisplayFormat = value;
        }

        public ICaptionCommand AddCustomFieldCommand { get; set; }
        public ICaptionCommand DeleteCustomFieldCommand { get; set; }

        public EntityCustomFieldViewModel SelectedCustomField { get; set; }

        public ObservableCollection<EntityCustomFieldViewModel> EntityCustomFields
        {
            get
            {
                return _entityCustomFields ?? (_entityCustomFields =
                    new ObservableCollection<EntityCustomFieldViewModel>(
                        Model.EntityCustomFields.Select(x => new EntityCustomFieldViewModel(x))));
            }
        }

        private bool CanDeleteCustomField(EntityCustomFieldViewModel arg)
        {
            return SelectedCustomField != null;
        }

        private void OnDeleteCustomField(EntityCustomFieldViewModel accountCustomFieldViewModel)
        {
            if (SelectedCustomField != null)
            {
                Model.EntityCustomFields.Remove(SelectedCustomField.Model);
                if (SelectedCustomField.Model.Id > 0)
                    Workspace.Delete(SelectedCustomField.Model);
                EntityCustomFields.Remove(SelectedCustomField);
            }
        }

        private void OnAddCustomField(string s)
        {
            EntityCustomFields.Add(new EntityCustomFieldViewModel(
                Model.AddCustomField(LoOv.G(o => Resources.New), 0)));
        }

        public override string GetModelTypeString()
        {
            return LoOv.G(o => Resources.EntityType);
        }

        public override Type GetViewType()
        {
            return typeof(Views.EntityTypeView);
        }
    }
}