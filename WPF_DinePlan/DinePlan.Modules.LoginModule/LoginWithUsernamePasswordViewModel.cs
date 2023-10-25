using DinePlan.Presentation.Common.ModelBase;
using Prism.Commands;
using Prism.Modularity;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace DinePlan.Modules.LoginModule
{
    /// <summary>
    ///     The view model for Login with Username and Password view.
    /// </summary>
    /// <seealso cref="DinePlan.Presentation.Common.ModelBase.GenericViewModelbase" />
    [Export]
    public class LoginWithUsernamePasswordViewModel : GenericViewModelbase
    {
        private string password;

        private string username;

        [ImportingConstructor]
        public LoginWithUsernamePasswordViewModel(IModuleManager moduleManager)
        {
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        #region LoginCommand

        public ICommand LoginCommand
        {
            get { return new DelegateCommand<object>(args => { LoginExecute((PasswordBox)args); }); }
        }

        private void LoginExecute(PasswordBox passwordBox)
        {
            Password = passwordBox.Password;
        }

        #endregion

        #region CancelCommand

        public ICommand CancelCommand
        {
            get { return new DelegateCommand(() => { CancelExecute(); }); }
        }

        private void CancelExecute()
        {
        }

        #endregion
    }
}