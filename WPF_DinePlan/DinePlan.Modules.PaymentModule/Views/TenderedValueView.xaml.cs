using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace DinePlan.Modules.PaymentModule
{
    /// <summary>
    ///     Interaction logic for TenderedValueView.xaml
    /// </summary>
    [Export]
    public partial class TenderedValueView : UserControl
    {
        [ImportingConstructor]
        public TenderedValueView(TenderedValueViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}