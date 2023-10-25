using DinePlan.Presentation.Services.Common;
using Prism.Events;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DinePlan.Modules.LoginModule
{
    /// <summary>
    ///     Interaction logic for LoginWithUsernamePasswordView2.xaml
    /// </summary>
    [Export]
    public partial class LoginWithUsernamePasswordView2 : UserControl
    {
        public LoginWithUsernamePasswordViewModel2 viewModel;
        DispatcherTimer timer;

        [ImportingConstructor]
        public LoginWithUsernamePasswordView2(LoginWithUsernamePasswordViewModel2 viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            this.viewModel = viewModel;
            Loaded += LoginWithUsernamePasswordView2_Loaded;
            Unloaded += LoginWithUsernamePasswordView2_Unloaded;
        }

        private void LoginWithUsernamePasswordView2_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void LoginWithUsernamePasswordView2_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Loaded();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };

            timer.Tick += (s, ee) =>
            {
                    timer.Stop();
                if (IsLoaded)
                {
                    EventServiceFactory.EventService.PublishEvent(EventTopicNames.ActivateNavigation);
                    EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>()
                                .Publish(new EventParameters<EventAggregator> { Topic = EventTopicNames.ClockInDialogOpen });
                }

            };
            timer.Start();
        }
    }
}