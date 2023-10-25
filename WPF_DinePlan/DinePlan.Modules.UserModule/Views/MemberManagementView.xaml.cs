using DinePlan.Modules.UserModule.ViewModels;
using DinePlan.Presentation.Common;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DinePlan.Modules.UserModule.Views
{
    [Export]
    public partial class MemberManagementView : UserControl
    {
        private readonly MemberManagementViewModel viewModel;

        [ImportingConstructor]
        public MemberManagementView(MemberManagementViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.CheckClearData();
            Filter.Focus();
        }

        private void GridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KeyboardRow.Height = new GridLength(1, GridUnitType.Star);
            ContentRow.Height = new GridLength(1, GridUnitType.Star);
        }

        private void FlexButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.Reset();
            Filter.BackgroundFocus();
        }
    }
}