using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace DinePlan.Modules.PaymentModule
{
    /// <summary>
    ///     Interaction logic for ChangeTemplatesView.xaml
    /// </summary>
    [Export]
    public partial class ChangeTemplatesView : UserControl
    {
        [ImportingConstructor]
        public ChangeTemplatesView(ChangeTemplatesViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}