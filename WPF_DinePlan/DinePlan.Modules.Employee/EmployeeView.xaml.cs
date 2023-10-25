using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace DinePlan.Modules.Employee
{
    /// <summary>
    ///     Interaction logic for EmployeeView.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    [Export]
    public partial class EmployeeView : UserControl
    {
        /// <summary>
        ///     The view model
        /// </summary>
        private readonly EmployeeViewModel viewModel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmployeeView" /> class.
        /// </summary>
        [ImportingConstructor]
        public EmployeeView(EmployeeViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            DataContext = viewModel;

            Loaded += EmployeeView_Loaded;
        }

        /// <summary>
        ///     Handles the Loaded event of the EmployeeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void EmployeeView_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Loaded();
            login.CornerRadius = new CornerRadius(20, 0, 0, 0);
            exit.CornerRadius = new CornerRadius(0, 0, 0, 20);
        }
    }
}