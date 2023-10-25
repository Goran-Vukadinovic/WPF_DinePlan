using DinePlan.Domain.Models.Promotion;
using DinePlan.Domain.Models.Users;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services;
using DinePlan.Presentation.Services.Common;
using Prism.Commands;
using System.ComponentModel.Composition;
using System.Timers;
using System.Windows;
using DinePlan.Common.Model.Permission;

namespace DinePlan.Modules.UserModule
{
    [Export]
    public class LoggedInUserViewModel : GenericViewModelbase
    {
        private System.Timers.Timer aTimer;
        private readonly ITicketService _ticketService;

        [ImportingConstructor]
        public LoggedInUserViewModel(IApplicationState applicationState
            , IUserService userService
            , ITicketService ticketService)
        {
            _ticketService = ticketService;
            EventServiceFactory.EventService.GetEvent<GenericEvent<User>>().Subscribe(x =>
            {
                if (x.Topic == EventTopicNames.UserLoggedIn) UserLoggedIn(x.Value);
                if (x.Topic == EventTopicNames.UserLoggedOut) UserLoggedOut(x.Value);
            });

            LoggedInUser = applicationState.CurrentLoggedInUser;

            LogoutUserCommand = new DelegateCommand<User>(x =>
            {
                if (!ApplicationState.IsLocked)
                {
                    if (UserService.IsUserPermittedFor(PermissionNames.OpenNavigation))
                        EventServiceFactory.EventService.PublishEvent(EventTopicNames.ActivateNavigation);
                    else
                        UserService.LogoutUser();
                }
            });

            ClockInDialogOpenCommand = new DelegateCommand(() =>
            {
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.ClockInDialogOpen);
            });

            DeviceStateDialogOpenCommand = new DelegateCommand(() =>
            {
                ExtensionMethods.PublishIdEvent(0, EventTopicNames.DeviceStateViewActivate);
            });

            EdineDialogOpenCommand = new DelegateCommand(() =>
            {
                ExtensionMethods.PublishIdEvent(0, EventTopicNames.EdineViewActivate);
            });

            SetTimer();

            IsEdineSync = LocalSettings.EDineSync;

            AutoeDineReceivedTicket = LocalSettings.AutoeDineReceivedTicket;
        }

        public bool IsEdineSync { get; set; }

        public bool AutoeDineReceivedTicket { get; set; }

        public string LoggedInUserName => LoggedInUser != null ? LoggedInUser.Name : "";

        private string _dineConnect = LoOv.G("Offline");
        public string DineConnectOnline
        {
            get
            {
                return _dineConnect;
            }
            set
            {
                _dineConnect = value;
                RaisePropertyChanged("DineConnectOnline");
            }
        }

        public User LoggedInUser { get; set; }
        public DelegateCommand<User> LogoutUserCommand { get; set; }
        public DelegateCommand ClockInDialogOpenCommand { get; set; }
        public DelegateCommand DeviceStateDialogOpenCommand { get; set; }
        public DelegateCommand EdineDialogOpenCommand { get; set; }

        private string _edinePending = "M27,22.000013C25.346001,22.000013 24,23.345991 24,25.000013 24,26.654005 25.346001,28.000013 27,28.000013 28.653999,28.000013 30,26.654005 30,25.000013 30,23.345991 28.653999,22.000013 27,22.000013z M5,22C3.3460007,22 2,23.346 2,25 2,26.654 3.3460007,28 5,28 6.6540003,28 8,26.654 8,25 8,23.346 6.6540003,22 5,22z M16,2C14.346,2 13,3.3460007 13,5 13,6.6540003 14.346,8 16,8 17.654,8 19,6.6540003 19,5 19,3.3460007 17.654,2 16,2z M16,0C18.757,0 21,2.243 21,5 21,6.2061872 20.570676,7.3139925 19.856814,8.178627L19.749571,8.3023939 26.702677,20.009615 26.74309,20.006532C26.828189,20.002204 26.913844,20.000013 27,20.000013 29.757004,20.000013 32,22.242994 32,25.000013 32,27.757002 29.757004,30.000013 27,30.000013 24.242996,30.000013 22,27.757002 22,25.000013 22,23.104563 23.060166,21.452075 24.618662,20.604373L24.703606,20.560808 18.143555,9.5162544 17.944359,9.6064529C17.346375,9.8598127 16.68925,10 16,10 15.224594,10 14.489846,9.8225756 13.834221,9.5061913L13.749143,9.4625559 7.9581623,20.972196 7.9896793,20.994624C9.2092552,21.907268 10,23.363032 10,25 10,27.757 7.757,30 5,30 2.243,30 0,27.757 0,25 0,22.243 2.243,20 5,20 5.3446255,20 5.6812191,20.035047 6.0064049,20.101764L6.1420031,20.133139 12.151809,8.1885796 12.143186,8.178627C11.429324,7.3139925 11,6.2061872 11,5 11,2.243 13.243,0 16,0z";
        public string EdinePending
        {
            get { return _edinePending; }
            set
            {
                _edinePending = value;
                RaisePropertyChanged("EdinePending");
            }
        }


        private void UserLoggedIn(User user)
        {
            LoggedInUser = user;
            RaisePropertyChanged(nameof(LoggedInUserName));
            DialogService.ShowSuggestPromotion(Visibility.Visible, Visibility.Collapsed,
                (int)DisplayType.LoginOfDinePlan, null);
        }

        private void UserLoggedOut(User user)
        {
            LoggedInUser = User.Nobody;
            RaisePropertyChanged(nameof(LoggedInUserName));
            DialogService.ShowSuggestPromotion(Visibility.Visible, Visibility.Collapsed,
                (int)DisplayType.LogoutOfDinePlan, null);
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(1000 * 10);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var token = TokenService.GetToken(false);
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DineConnectOnline = string.IsNullOrEmpty(token) ? LoOv.G("Offline") : LoOv.G("Online");
                });
            }

            if (IsEdineSync)
            {
                var pending = _ticketService.GetAllTicketApi(Common.Model.EDine.TicketApiProcessState.Pending).Count;
                EdinePending = pending > 0 ? $"M13.371006,0C14.806003,0 15.968998,1.3850021 15.968998,3.0940018 15.968998,3.9589996 15.670002,4.7390022 15.190006,5.3000031L19.498007,13.132999C19.797994,12.989002 20.125006,12.905003 20.468008,12.905003 21.903005,12.905003 23.066,14.291 23.066,16 23.066,17.138 22.543997,18.122002 21.774999,18.660004L23.858007,25.854004C23.974005,25.834 24.088003,25.811005 24.207006,25.811005 25.642004,25.811005 26.805,27.196999 26.805,28.904999 26.805,30.614998 25.642004,32 24.207006,32 22.771993,32 21.608999,30.614998 21.608999,28.904999 21.608999,27.767998 22.131002,26.784004 22.899008,26.245003L20.816,19.053001C20.702001,19.070999 20.586996,19.095001 20.468008,19.095001 20.362997,19.095001 20.263006,19.072998 20.162008,19.056999L18.149998,26.370003C18.821997,26.931 19.263998,27.855003 19.263998,28.904999 19.263998,30.614998 18.101002,32 16.666004,32 15.230991,32 14.067005,30.614998 14.067005,28.904999 14.067005,27.196999 15.230991,25.811005 16.666004,25.811005 16.854999,25.811005 17.039005,25.838005 17.215992,25.884003L19.196997,18.683998C18.409,18.151001 17.869999,17.154999 17.869999,16 17.869999,15.135002 18.168995,14.355 18.649008,13.794003L14.340992,5.9600029C14.039997,6.1049995 13.715,6.1879997 13.371006,6.1879997 13.037006,6.1879997 12.720005,6.1070023 12.427996,5.9700012L8.1660021,13.905003C8.6570004,14.468002 8.9669979,15.255005 8.9669979,16.133003 8.9669979,17.258003 8.4570025,18.235001 7.6999992,18.776001L9.8259915,25.857002C9.9499996,25.834999 10.073001,25.811005 10.20299,25.811005 11.638003,25.811005 12.799991,27.196999 12.799991,28.904999 12.799991,30.614998 11.638003,32 10.20299,32 8.7679928,32 7.6039904,30.614998 7.6039904,28.904999 7.6039904,27.779999 8.114992,26.802002 8.8710049,26.262001L6.7460039,19.181999C6.6219958,19.203003 6.4980028,19.227005 6.367998,19.227005 6.2439898,19.227005 6.1250017,19.203003 6.0059984,19.182999L3.9180003,26.253998C4.6799939,26.792999 5.1960004,27.773003 5.1960004,28.904999 5.1960004,30.614998 4.0329908,32 2.597993,32 1.1629948,32 1.6183913E-07,30.614998 0,28.904999 1.6183913E-07,27.196999 1.1629948,25.811005 2.597993,25.811005 2.7220009,25.811005 2.840989,25.834 2.9609996,25.855003L5.0479902,18.783005C4.2870038,18.244003 3.76999,17.264999 3.76999,16.133003 3.76999,14.423004 4.9339917,13.037003 6.367998,13.037003 6.7019976,13.037003 7.0189989,13.118999 7.3119982,13.255001L11.573001,5.321003C11.082003,4.7579994 10.772998,3.9700012 10.772998,3.0940018 10.772998,1.3850021 11.935992,0 13.371006,0z" : "M27,22.000013C25.346001,22.000013 24,23.345991 24,25.000013 24,26.654005 25.346001,28.000013 27,28.000013 28.653999,28.000013 30,26.654005 30,25.000013 30,23.345991 28.653999,22.000013 27,22.000013z M5,22C3.3460007,22 2,23.346 2,25 2,26.654 3.3460007,28 5,28 6.6540003,28 8,26.654 8,25 8,23.346 6.6540003,22 5,22z M16,2C14.346,2 13,3.3460007 13,5 13,6.6540003 14.346,8 16,8 17.654,8 19,6.6540003 19,5 19,3.3460007 17.654,2 16,2z M16,0C18.757,0 21,2.243 21,5 21,6.2061872 20.570676,7.3139925 19.856814,8.178627L19.749571,8.3023939 26.702677,20.009615 26.74309,20.006532C26.828189,20.002204 26.913844,20.000013 27,20.000013 29.757004,20.000013 32,22.242994 32,25.000013 32,27.757002 29.757004,30.000013 27,30.000013 24.242996,30.000013 22,27.757002 22,25.000013 22,23.104563 23.060166,21.452075 24.618662,20.604373L24.703606,20.560808 18.143555,9.5162544 17.944359,9.6064529C17.346375,9.8598127 16.68925,10 16,10 15.224594,10 14.489846,9.8225756 13.834221,9.5061913L13.749143,9.4625559 7.9581623,20.972196 7.9896793,20.994624C9.2092552,21.907268 10,23.363032 10,25 10,27.757 7.757,30 5,30 2.243,30 0,27.757 0,25 0,22.243 2.243,20 5,20 5.3446255,20 5.6812191,20.035047 6.0064049,20.101764L6.1420031,20.133139 12.151809,8.1885796 12.143186,8.178627C11.429324,7.3139925 11,6.2061872 11,5 11,2.243 13.243,0 16,0z";
            }
        }


    }
}