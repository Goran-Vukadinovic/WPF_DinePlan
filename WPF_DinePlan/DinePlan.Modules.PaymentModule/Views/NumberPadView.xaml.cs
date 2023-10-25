using DinePlan.Common.License;
using DinePlan.Services;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace DinePlan.Modules.PaymentModule
{
    /// <summary>
    ///     Interaction logic for NumberPadView.xaml
    /// </summary>
    [Export]
    public partial class NumberPadView : UserControl
    {
        [ImportingConstructor]
        public NumberPadView(NumberPadViewModel viewModel, ISettingService settingService)
        {
            DataContext = viewModel;
            InitializeComponent();

            if (LicenseSettings.CountryCode.Equals("IA"))
            {
                SeriousButton.Content = "000";
                SeriousButton.CommandParameter = "000";
            }
            else
            {
                SeriousButton.Content = ".";
                SeriousButton.CommandParameter = ".";
            }

            Loaded += NumberPadView_Loaded;
        }

        private void NumberPadView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as NumberPadViewModel;
            if (vm.PaymentScreenValues == null || vm.PaymentScreenValues.Length == 0)
            {
                col1.Width = new GridLength(0);
                psValues.Visibility = Visibility.Collapsed;
            }
            else
            {
                col1.Width = new GridLength(17, GridUnitType.Star);
                psValues.Visibility = Visibility.Visible;
            }
        }
    }
}