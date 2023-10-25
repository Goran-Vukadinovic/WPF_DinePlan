using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Modules.PaymentModule.ViewModels;

namespace DinePlan.Modules.PaymentModule
{
    /// <summary>
    ///     Interaction logic for OrderSelectorView.xaml
    /// </summary>
    [Export]
    public partial class OrderSelectorView : UserControl
    {
        [ImportingConstructor]
        public OrderSelectorView(OrderSelectorViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}