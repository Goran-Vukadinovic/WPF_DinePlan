using DinePlan.Common.Excel;
using DinePlan.Common.File;
using DinePlan.Common.Model.Export;
using DinePlan.Common.Model.Menu;
using DinePlan.Domain.Models.Menus;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Modules.MenuModule.Views;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DinePlan.Modules.MenuModule.Menu
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MenuItemListViewModel : EntityCollectionViewModelBase<MenuItemViewModel, MenuItem>
    {
        [ImportingConstructor]
        public MenuItemListViewModel()
        {
            ExportMenu = new CaptionCommand<string>(LoOv.G(o => Resources.ExportTemplate), OnExportMenu);
            LocateTemplate = new CaptionCommand<string>(LoOv.G(o => Resources.LocateTemplate), OnLocateTemplate);
            UrbanPiperItemUpdate = new CaptionCommand<string>(LoOv.G(o => Resources.UrbanPiperItemUpdate), OnUrbanPiperItemUpdate);
            CustomCommands.Add(ExportMenu);
            CustomCommands.Add(LocateTemplate);
            CustomCommands.Add(UrbanPiperItemUpdate);
        }

        public ICaptionCommand LocateTemplate { get; set; }

        public ICaptionCommand ExportMenu { get; set; }

        public ICaptionCommand UrbanPiperItemUpdate { get; set; }

        private void OnUrbanPiperItemUpdate(string obj)
        {
            var urbanPiperItemUpdateView = new UrbanPiperItemUpdateView();
            urbanPiperItemUpdateView.ShowDialog();
        }

        private void OnLocateTemplate(string obj)
        {
            var prc = new Process { StartInfo = { FileName = Path.Combine(LocalSettings.AppPath, "Imports") } };
            prc.Start();
        }


        private void OnExportMenu(string obj)
        {
            var list = (from item in MenuService.GetMenuItemsWithPrices()
                        from portion in item.Portions
                        select new ImportMenu
                        {
                            Category = item.GroupCode ?? "",
                            AliasCode = item.AliasCode ?? "",
                            MenuName = item.Name ?? "",
                            AliasName = item.AliasName ?? "",
                            Portion = portion.Name ?? "",
                            Price = portion.Price,
                            Barcode = item.Barcode,
                            Description = item.ItemDesc,
                            ForceQuantity = item.ForceQuantity ? 1 : 0,
                            ForceChangePrice = item.ForceChangePrice ? 1 : 0,
                            Tag = item.Tag,
                            RestrictPromotion = item.RestrictPromotion ? 1 : 0,
                            NoTax = item.NoTax ? 1 : 0
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
                    "ForceChangePrice",
                    "Tag",
                    "RestrictPromotion",
                    "NoTax"
                }
            };

            var fn = FileDialogHelper.SaveFileName("MENU_EXPORT", ".xlsx");
            if (!string.IsNullOrEmpty(fn))
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