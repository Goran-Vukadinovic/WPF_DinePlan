using DinePlan.Presentation.Common;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace DinePlan.Modules.AccountModule
{
    /// <summary>
    ///     Interaction logic for DocumentCreatorView.xaml
    /// </summary>
    [Export]
    public partial class DocumentCreatorView : UserControl
    {
        [ImportingConstructor]
        public DocumentCreatorView(DocumentCreatorViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DescriptionEdit.BackgroundFocus();
        }
    }
}