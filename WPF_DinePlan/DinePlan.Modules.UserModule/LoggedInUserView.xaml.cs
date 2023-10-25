using Prism.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace DinePlan.Modules.UserModule
{
    /// <summary>
    ///     Interaction logic for LoggedInUserView.xaml
    /// </summary>
    [Export]
    public partial class LoggedInUserView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly LoggedInUserViewModel _viewModel;

        [ImportingConstructor]
        public LoggedInUserView(LoggedInUserViewModel viewModel, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            DataContext = viewModel;
            _eventAggregator = eventAggregator;
            _viewModel = viewModel;
        }
    }
}