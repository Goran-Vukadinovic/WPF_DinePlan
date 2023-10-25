#region using

using DinePlan.Domain.Models.Menus;
using DinePlan.Services;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

#endregion

namespace DinePlan.Modules.MenuModule
{
    [Export]
    public class MenuItemTagCache
    {
        private readonly IDictionary<string, IEnumerable<string>> _cache;
        private readonly IMenuService _menuService;

        [ImportingConstructor]
        public MenuItemTagCache(IMenuService menuService)
        {
            _menuService = menuService;
            _cache = new Dictionary<string, IEnumerable<string>>();
        }

        public IEnumerable<string> GetTagValues(string tagName)
        {
            if (_cache.ContainsKey(tagName)) return _cache[tagName];
            var list = _menuService.GetMenuItemTags(tagName).ToList();
            _cache.Add(tagName, list);
            return list;
        }

        public void ResetCacheIfNeeded(IEnumerable<MenuItemTagValue> currentTagValues)
        {
            if (currentTagValues.All(x =>
            {
                if (!_cache.ContainsKey(x.TagName)) return false;
                return _cache[x.TagName].Contains(x.TagValue);
            }))
                return;
            _cache.Clear();
        }
    }
}