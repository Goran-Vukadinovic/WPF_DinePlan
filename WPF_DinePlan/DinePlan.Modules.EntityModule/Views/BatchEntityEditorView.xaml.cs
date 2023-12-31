﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DinePlan.Modules.EntityModule.ViewModel;

namespace DinePlan.Modules.EntityModule
{
    /// <summary>
    ///     Interaction logic for EntityListerView.xaml
    /// </summary>
    public partial class BatchEntityEditorView : UserControl
    {
        public BatchEntityEditorView()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null) return;

            grid.Columns.Where(x => x.DisplayIndex > 1).ToList().ForEach(x => grid.Columns.Remove(x));

            var i = 0;
            var d = DataContext as BatchEntityEditorViewModel;
            if (d == null) return;

            var entityTypes = d.SelectedEntityType.EntityCustomFields.Select(cf => new DataGridTextColumn
            {
                Header = cf.Name,
                Binding = new Binding("[" + cf.Name + "]"),
                IsReadOnly = false,
                MinWidth = 60
            });

            foreach (var dgtc in entityTypes)
            {
                grid.Columns.Insert(i + 2, dgtc);
                i++;
            }
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement_OnDataContextChanged(sender, new DependencyPropertyChangedEventArgs());
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((BatchEntityEditorViewModel)DataContext).RefreshItems();
                (sender as TextBox).SelectAll();
            }
        }
    }
}