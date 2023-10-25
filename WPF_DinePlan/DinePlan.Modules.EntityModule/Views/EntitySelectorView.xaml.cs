using System.ComponentModel.Composition;
using System.Windows.Controls;
using DinePlan.Modules.EntityModule.ViewModel;

namespace DinePlan.Modules.EntityModule.Views
{
    /// <summary>
    ///     Interaction logic for LocationSelectorView.xaml
    /// </summary>
    [Export]
    public partial class EntitySelectorView : UserControl
    {
        [ImportingConstructor]
        public EntitySelectorView(EntitySelectorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}