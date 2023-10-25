using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Modules.PaymentModule.ViewModels;

namespace DinePlan.Modules.PaymentModule
{
    /// <summary>
    ///     Interaction logic for ForeignCurrencyButtonsView.xaml
    /// </summary>
    [Export]
    public partial class ForeignCurrencyButtonsView : UserControl
    {
        [ImportingConstructor]
        public ForeignCurrencyButtonsView(ForeignCurrencyButtonsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}