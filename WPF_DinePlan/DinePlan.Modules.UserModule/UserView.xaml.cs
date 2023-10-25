using System.Windows;
using System.Windows.Controls;

namespace DinePlan.Modules.UserModule
{
    /// <summary>
    ///     Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
            PasswordTextBox.GotFocus += PasswordTextBoxGotFocus;
            SecurityTextBox.GotFocus += SecurityTextBoxGotFocus;
        }

        private void PasswordTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Text.Contains("*"))
                PasswordTextBox.Clear();
        }

        private void SecurityTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (SecurityTextBox.Text.Contains("*"))
                SecurityTextBox.Clear();
        }
    }
}