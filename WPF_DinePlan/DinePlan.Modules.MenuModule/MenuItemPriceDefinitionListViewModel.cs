using System.ComponentModel.Composition;
using DinePlan.Domain.Models.Menus;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Services;

namespace DinePlan.Modules.MenuModule
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class MenuItemPriceDefinitionListViewModel :
        EntityCollectionViewModelBase<MenuItemPriceDefinitionViewModel, MenuItemPriceDefinition>
    {
        private readonly IPriceListService _priceListService;

        [ImportingConstructor]
        public MenuItemPriceDefinitionListViewModel(IPriceListService priceListService)
        {
            _priceListService = priceListService;
        }

        protected override void BeforeDeleteItem(MenuItemPriceDefinition item)
        {
            _priceListService.DeleteMenuItemPricesByPriceTag(item.PriceTag);
        }
    }
}