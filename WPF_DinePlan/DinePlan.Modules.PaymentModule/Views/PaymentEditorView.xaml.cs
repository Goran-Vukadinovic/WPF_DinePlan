using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Modules.PaymentModule.ViewModels;

namespace DinePlan.Modules.PaymentModule.Views
{
    /// <summary>
    ///     Interaction logic for PaymentView.xaml
    /// </summary>
    [Export]
    public partial class PaymentEditorView : UserControl
    {
        [ImportingConstructor]
        public PaymentEditorView(PaymentEditorViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            Loaded += PaymentEditorView_Loaded;
        }

        private void PaymentEditorView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
         
        }
    }
}