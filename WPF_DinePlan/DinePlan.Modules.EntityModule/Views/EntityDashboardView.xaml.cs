using DinePlan.Infrastructure.Helpers;
using DinePlan.Presentation.Common.Widgets;
using DinePlan.Presentation.Controls.UIControls;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DinePlan.Modules.EntityModule
{
    /// <summary>
    ///     Interaction logic for ResourceDashboardView.xaml
    /// </summary>
    [Export]
    public partial class EntityDashboardView : UserControl
    {
        public EntityDashboardView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public EntityDashboardView(EntityDashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DiagramCanvas.EditingMode == InkCanvasEditingMode.None)
            {
                brd.BorderBrush = Brushes.Red;
                miDesignMode.IsChecked = true;
                DiagramCanvas.EditingMode = InkCanvasEditingMode.Select;
                ((EntityDashboardViewModel)DataContext).LoadTrackableEntityScreenItems();
            }
            else
            {
                brd.BorderBrush = Brushes.Transparent;
                miDesignMode.IsChecked = false;
                DiagramCanvas.EditingMode = InkCanvasEditingMode.None;
                ((EntityDashboardViewModel)DataContext).SaveTrackableEntityScreenItems();
            }
        }

        private void DiagramCanvas_WidgetRemoved(object sender, EventArgs e)
        {
            ((EntityDashboardViewModel)DataContext).RemoveWidget(sender as IDiagram);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            miAddWidget.Items.Cast<MenuItem>().ForEach(x => x.Click -= MenuItem_Click);
            miAddWidget.Items.Clear();
            foreach (var creator in WidgetCreatorRegistry.GetCreators())
            {
                var menuItem = new MenuItem();
                menuItem.Tag = creator.GetCreatorName();
                menuItem.Header = creator.GetCreatorDescription();
                menuItem.Click += menuItem_Click;
                miAddWidget.Items.Add(menuItem);
            }
        }


        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi != null) ((EntityDashboardViewModel)DataContext).AddWidget(mi.Tag.ToString());
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            if (WidgetCopier.CanGetWidget) ((EntityDashboardViewModel)DataContext).AddWidget(WidgetCopier.GetWidget());
        }
    }
}