using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace DinePlan.Modules.PaymentModule
{
    /// <summary>
    ///     Interaction logic for ReturningAmountView.xaml
    /// </summary>
    [Export]
    public partial class ReturningAmountView : UserControl
    {
        [ImportingConstructor]
        public ReturningAmountView(ReturningAmountViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}