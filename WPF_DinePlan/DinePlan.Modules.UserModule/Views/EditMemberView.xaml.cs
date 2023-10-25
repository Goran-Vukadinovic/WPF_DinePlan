using DinePlan.Modules.UserModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DinePlan.Modules.UserModule.Views
{
    [Export]
    public partial class EditMemberView : UserControl
    {
        private readonly EditMemberViewModel viewModel;

        [ImportingConstructor]
        public EditMemberView(EditMemberViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }
    }
}