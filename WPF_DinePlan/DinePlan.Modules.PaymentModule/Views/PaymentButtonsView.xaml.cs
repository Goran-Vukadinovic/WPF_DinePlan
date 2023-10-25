using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Modules.PaymentModule.ViewModels;

namespace DinePlan.Modules.PaymentModule.Views
{
    /// <summary>
    ///     Interaction logic for PaymentButtonsView.xaml
    /// </summary>
    [Export]
    public partial class PaymentButtonsView : UserControl
    {
        [ImportingConstructor]
        public PaymentButtonsView(PaymentButtonsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}