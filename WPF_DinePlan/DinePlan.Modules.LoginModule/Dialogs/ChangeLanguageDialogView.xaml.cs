#region using

using System;
using System.ComponentModel.Composition;
using System.Windows;

#endregion

namespace DinePlan.Modules.LoginModule.Dialog
{
    [Export]
    public partial class ChangeLanguageDialogView : Window, IDisposable
    {
        private readonly ChangeLanguageDialogViewModel viewModel;

        public ChangeLanguageDialogView(ChangeLanguageDialogViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }

        public void Dispose()
        {
            Close();
        }
    }
}