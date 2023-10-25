#region using

using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion using

namespace DinePlan.Modules.LoginModule
{
    /// <summary>
    ///     Interaction logic for LoginView.xaml
    /// </summary>
    [Export]
    public partial class LoginWithUsernamePasswordView3 : UserControl
    {
        public LoginWithUsernamePasswordViewModel3 viewModel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginView" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        [ImportingConstructor]
        public LoginWithUsernamePasswordView3(LoginWithUsernamePasswordViewModel3 viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            this.viewModel = viewModel;
            Loaded += LoginView_Loaded;
        }

        public bool IsClockOut { get; set; } = false;

        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown" /> attached event reaches an
        ///     element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        /// <summary>
        ///     Handles the Loaded event of the LoginView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Loaded();

            Keyboard.Focus(this);
            Unloaded += LoginView_Unloaded;
            if (viewModel.ShowShutDownButton)
            {
                exit.CornerRadius = new CornerRadius(0);
                exit.BorderThickness = new Thickness(0, 0, 0, 0.5);
                shutdown.CornerRadius = new CornerRadius(0, 0, 0, 20);
                shutdown.Visibility = Visibility.Visible;
            }
            else
            {
                exit.CornerRadius = new CornerRadius(0, 0, 0, 20);
                exit.BorderThickness = new Thickness(0, 0, 0, 0);
                shutdown.Visibility = Visibility.Collapsed;
            }

            login.CornerRadius = new CornerRadius(20, 0, 0, 0);
            login.BorderThickness = new Thickness(0, 0, 0, 0.5);
            employee.BorderThickness = new Thickness(0, 0, 0, 0.5);
        }

        /// <summary>
        ///     Handles the Unloaded event of the LoginView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void LoginView_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Unloaded();
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) viewModel.SubmitExecute();
        }
    }
}