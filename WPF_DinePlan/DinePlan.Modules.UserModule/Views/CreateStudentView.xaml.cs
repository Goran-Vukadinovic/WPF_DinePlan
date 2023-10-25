using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Modules.UserModule.ViewModels;

namespace DinePlan.Modules.UserModule.Views
{
    /// <summary>
    /// Interaction logic for CreateStudentView.xaml
    /// </summary>
    [Export]
    public partial class CreateStudentView : UserControl
    {
        private readonly CreateStudentViewModel viewModel;

        [ImportingConstructor]
        public CreateStudentView(CreateStudentViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            Loaded += CreateStudentView_Loaded;
        }

        private void CreateStudentView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.GetData();
            viewModel.RefreshExecute();
        }
    }
}
