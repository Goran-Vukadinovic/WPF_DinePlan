using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace DinePlan.Modules.LoginModule
{
    /// <summary>
    ///     Interaction logic for LoginWithUsernamePasswordView.xaml
    /// </summary>
    [Export]
    public partial class LoginWithUsernamePasswordView : UserControl
    {
        private readonly LoginWithUsernamePasswordViewModel viewModel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginWithUsernamePasswordView" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        [ImportingConstructor]
        public LoginWithUsernamePasswordView(LoginWithUsernamePasswordViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            this.viewModel = viewModel;
        }

        private void TextBox_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox == null) return;
            textBox.SelectAll();
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox == null) return;
            textBox.SelectAll();
        }
    }
}