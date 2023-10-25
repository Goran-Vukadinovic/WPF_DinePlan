using DinePlan.Modules.UserModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DinePlan.Modules.UserModule.Views
{
    [Export]
    public partial class CreateMemberView : UserControl
    {
        private readonly CreateMemberViewModel viewModel;

        [ImportingConstructor]
        public CreateMemberView(CreateMemberViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }
    }
}