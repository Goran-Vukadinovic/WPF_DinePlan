using System.Collections.Generic;

namespace DinePlan.Modules.MenuModule
{
    public class MenuItemTagViewModel
    {
        private readonly MenuItemTagCache _menuItemTagCache;

        public MenuItemTagViewModel(MenuItemTagCache menuItemTagCache)
        {
            _menuItemTagCache = menuItemTagCache;
        }

        public string TagName { get; set; }

        public string TagNameDisplay => string.Format("{0}: ", TagName);

        public string TagValue { get; set; }

        public IEnumerable<string> TagValues => _menuItemTagCache.GetTagValues(TagName);
    }
}