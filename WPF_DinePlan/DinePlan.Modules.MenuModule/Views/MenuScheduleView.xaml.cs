using DinePlan.Presentation.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DinePlan.Modules.MenuModule
{
    public partial class MenuScheduleView : UserControl
    {
        public MenuScheduleView()
        {
            InitializeComponent();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditingElement is TextBox)
                ((TextBox)e.EditingElement).Text = ((TextBox)e.EditingElement).Text.Replace("\b", "");
        }

        private void DataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg != null && dg.CurrentColumn is DataGridTemplateColumn)
                if (!dg.IsEditing())
                    dg.BeginEdit();
        }

        private void DataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var ec = ExtensionServices.GetVisualChild<TextBox>(e.EditingElement as ContentPresenter);
            if (ec != null)
                ec.SelectAll();
        }

        private void AddLink_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MenuScheduleViewModel;
            if (vm == null) return;

            vm.AddMenuSchedulerCommand.Execute();

            var index = vm.MenuItemSchedulesToView.Count - 1;
            var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null) grid.ScrollIntoView(grid.Items[index]);
        }
    }
}