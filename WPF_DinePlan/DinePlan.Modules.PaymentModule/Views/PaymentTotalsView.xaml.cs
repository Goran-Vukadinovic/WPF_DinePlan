using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Presentation.ViewModels;

namespace DinePlan.Modules.PaymentModule.Views
{
    /// <summary>
    ///     Interaction logic for PaymentTotalsView.xaml
    /// </summary>
    [Export]
    public partial class PaymentTotalsView : UserControl
    {
        [ImportingConstructor]
        public PaymentTotalsView(TicketTotalsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}