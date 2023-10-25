#region using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using DinePlan.Domain.Models.Accounts;
using DinePlan.Domain.Models.Menus;
using DinePlan.Infrastructure.Data;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Persistance.Data;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common.DataGeneration;
using FluentValidation;
using FluentValidation.Results;

#endregion

namespace DinePlan.Modules.MenuModule.Menu
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MenuItemViewModel : EntityViewModelBase<MenuItem>, IEntityCreator<MenuItem>
    {
        private readonly MenuItemTagCache _menuItemTagCache;
        private IEnumerable<AccountTransactionType> _accountTransactionType;
        private IEnumerable<Course> _courses;

        private IEnumerable<string> _groupCodes;
        private IEnumerable<MenuItemTagViewModel> _menuItemTags;
        private ObservableCollection<PortionViewModel> _portions;
        private IEnumerable<string> _tags;

        [ImportingConstructor]
        public MenuItemViewModel(MenuItemTagCache menuItemTagCache)
        {
            AddPortionCommand = new CaptionCommand<string>(
                string.Format(LoOv.G(o => Resources.Add_f), LoOv.G(o => Resources.Portion)),
                OnAddPortion);
            DeletePortionCommand = new CaptionCommand<string>(
                string.Format(LoOv.G(o => Resources.Delete_f), LoOv.G(o => Resources.Portion)),
                OnDeletePortion, CanDeletePortion);
            _menuItemTagCache = menuItemTagCache;
        }

        public ICaptionCommand ExportMenuCommand { get; set; }

        public IEnumerable<string> GroupCodes => _groupCodes ?? (_groupCodes = MenuService.GetMenuItemGroupCodes());

        public IEnumerable<Course> Courses => _courses ?? (_courses = MenuService.GetCourses());

        public int? TransactionId
        {
            get => Model.TransactionId;
            set => Model.TransactionId = value.GetValueOrDefault(0);
        }

        public IEnumerable<AccountTransactionType> AccountTransactions => _accountTransactionType ??
                                                                          (_accountTransactionType =
                                                                              CacheService
                                                                                  .GetAccountTransactionTypes());

        public int? CourseId
        {
            get => Model.CourseId;
            set => Model.CourseId = value.GetValueOrDefault(0);
        }

        public bool IsCombo
        {
            get => Model.MenuItemType.Equals(1);
            set => Model.MenuItemType = value ? 1 : 0;
        }

        public bool ForceQuantity
        {
            get => Model.ForceQuantity;
            set => Model.ForceQuantity = value;
        }

        public bool ForceChangePrice
        {
            get => Model.ForceChangePrice;
            set => Model.ForceChangePrice = value;
        }

        public string ItemDesc
        {
            get => Model.ItemDesc;
            set => Model.ItemDesc = value;
        }

        public IEnumerable<string> Tags => _tags ?? (_tags = MenuService.GetMenuItemTags());

        public ObservableCollection<PortionViewModel> Portions =>
            _portions ?? (_portions = new ObservableCollection<PortionViewModel>(GetPortions(Model)));

        public PortionViewModel SelectedPortion { get; set; }
        public ICaptionCommand AddPortionCommand { get; set; }
        public ICaptionCommand DeletePortionCommand { get; set; }

        public string GroupCode
        {
            get => Model.GroupCode;
            set => Model.GroupCode = value;
        }

        public string AliasName
        {
            get => Model.AliasName ?? "";
            set => Model.AliasName = value;
        }

        public string AliasCode
        {
            get => Model.AliasCode ?? "";
            set => Model.AliasCode = value;
        }

        public string Tag
        {
            get => Model.Tag ?? "";
            set => Model.Tag = value;
        }

        public string Barcode
        {
            get => Model.Barcode ?? "";
            set => Model.Barcode = value;
        }

        public string GroupValue => Model.GroupCode;

        public bool IsCustomTagEditorVisible => MenuItemTags.Any();

        public IEnumerable<MenuItemTagViewModel> MenuItemTags
        {
            get
            {
                if (_menuItemTags == null) _menuItemTags = CreateMenuItemTags();
                return _menuItemTags;
            }
        }

        public string RefImagePath
        {
            get => Model.RefImagePath;
            set => Model.RefImagePath = value;
        }

        public IEnumerable<MenuItem> CreateItems(IEnumerable<string> data)
        {
            return new DataCreationService().BatchCreateMenuItems(data.ToArray(), Workspace);
        }

        private void OnAddPortion(string value)
        {
            var portion = MenuItem.AddDefaultMenuPortion(Model);
            Portions.Add(new PortionViewModel(portion));
            Workspace.Add(portion);
        }

        private IEnumerable<MenuItemTagViewModel> CreateMenuItemTags()
        {
            if (string.IsNullOrWhiteSpace(SettingService.ProgramSettings.ProductTagCaptions))
                return Enumerable.Empty<MenuItemTagViewModel>();
            var productTagCaptions = SettingService.ProgramSettings.ProductTagCaptions;
            char[] chrArray = { ',' };

            var strs =
                from x in productTagCaptions.Split(chrArray, StringSplitOptions.RemoveEmptyEntries).Distinct()
                select x.Trim();
            return (
                from x in strs
                select new MenuItemTagViewModel(_menuItemTagCache)
                {
                    TagName = x,
                    TagValue = Model.GetTagValue(x)
                }).ToList();
        }

        private void OnDeletePortion(string value)
        {
            if (SelectedPortion != null)
            {
                Workspace.Delete(SelectedPortion.Model);
                Model.Portions.Remove(SelectedPortion.Model);
                Portions.Remove(SelectedPortion);
            }
        }

        private bool CanDeletePortion(string value)
        {
            return SelectedPortion != null;
        }

        public override string GetModelTypeString()
        {
            return LoOv.G(o => Resources.Product);
        }

        public override Type GetViewType()
        {
            return typeof(MenuItemView);
        }

        private static IEnumerable<PortionViewModel> GetPortions(MenuItem baseModel)
        {
            return baseModel.Portions.Select(item => new PortionViewModel(item));
        }

        protected override void OnSave(string value)
        {
            foreach (var portion in Model.Portions) portion.Price = Math.Abs(portion.Price);

            Model.MenuItemType = IsCombo ? 1 : 0;

            foreach (var menuItemtag in MenuItemTags) Model.SetTagValue(menuItemtag.TagName, menuItemtag.TagValue);
            _menuItemTagCache.ResetCacheIfNeeded(Model.GetCustomTagValues());
            _menuItemTags = null;
            base.OnSave(value);

            using (var a = WorkspaceFactory.Create())
            {
                var portions = a.First<MenuItem>(1).First().Portions;
            }
        }

        protected override AbstractValidator<MenuItem> GetValidator()
        {
            return new MenuItemValidator();
        }
    }

    public class MenuItemValidator : EntityValidator<MenuItem>
    {
        public MenuItemValidator()
        {
            RuleFor(x => x.Portions.Count).GreaterThan(0);
            RuleFor(x => x.GroupCode).NotNull();

            Custom(x =>
            {
                if (x.IsComboItem && x.Portions.Count > 1)
                    return new ValidationFailure("Menu Items", "Combo Product should have only one Portion");
                return null;
            });
        }
    }
}