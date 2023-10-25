using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace DinePlan.Modules.PaymentModule
{
    /// <summary>
    ///     Interaction logic for CommandButtonsView.xaml
    /// </summary>
    [Export]
    public partial class CommandButtonsView : UserControl
    {
        [ImportingConstructor]
        public CommandButtonsView(CommandButtonsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}