using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using DinePlan.Common.Excel;
using DinePlan.Common.File;
using DinePlan.Common.Model.Export;
using DinePlan.Common.Model.Menu;
using DinePlan.Domain.Models.Menus;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Services;

namespace DinePlan.Modules.MenuModule
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class MenuItemListViewModel : EntityCollectionViewModelBase<MenuItemViewModel, MenuItem>
    {
        private readonly IMenuService _menuService;

        [ImportingConstructor]
        public MenuItemListViewModel(IMenuService menuService)
        {
            _menuService = menuService;
            ExportMenu = new CaptionCommand<string>(Resources.ExportTemplate, OnExportMenu);
            LocateTemplate = new CaptionCommand<string>(Resources.LocateTemplate, OnLocateTemplate);

            CustomCommands.Add(ExportMenu);
            CustomCommands.Add(LocateTemplate);

        }
        public ICaptionCommand LocateTemplate { get; set; }

        public ICaptionCommand ExportMenu { get; set; }

        private void OnLocateTemplate(string obj)
        {
            var prc = new Process {StartInfo = {FileName = Path.Combine(LocalSettings.AppPath, "Imports")}};
            prc.Start();
        }

        
        private void OnExportMenu(string obj)
        {
            var list = (from item in _menuService.GetMenuItemsWithPrices()
                from portion in item.Portions
                select new ImportMenu()
                {
                    Category = item.GroupCode ?? "",
                    AliasCode = item.AliasCode ?? "",
                    MenuName = item.Name ?? "",
                    AliasName = item.AliasName ?? "",
                    Portion = portion.Name ?? "",
                    Price = portion.Price,
                    Barcode = item.Barcode,
                    Description = item.ItemDesc,
                    ForceQuantity = item.ForceQuantity?1:0,
                    ForceChangePrice = item.ForceChangePrice?1:0

                }).ToList();

            var baseExport = new BaseExcelExportObject
            {
                ExportObject = list,
                ColumnNames = new[]
                {
                    "Category",
                    "AliasCode",
                    "MenuName",
                    "AliasName",
                    "Portion",
                    "Price",
                    "Barcode",
                    "Description",
                    "ForceQuantity",
                    "ForceChangePrice"
                }
            };

            var fn = FileDialogHelper.SaveFileName("MENU_EXPORT", ".xlsx");
            if (!string.IsNullOrEmpty(fn))
            {
                try
                {
                    ExcelExport.SaveAsExcel("MENU", fn, baseExport, true);
                }
                catch (Exception exception)
                {
                    var myException = exception.Message;
                    //ignore
                }
            }
        }
    }
}