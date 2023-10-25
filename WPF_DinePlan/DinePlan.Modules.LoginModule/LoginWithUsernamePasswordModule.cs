using DinePlan.Common.POS;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Prism.Mef.Modularity;
using System.ComponentModel.Composition;

namespace DinePlan.Modules.LoginModule
{
    [ModuleExport(typeof(LoginWithUsernamePasswordModule))]
    public class LoginWithUsernamePasswordModule : VisibleModuleBase
    {
        private LoginView loginView;

        private LoginWithUsernamePasswordView loginWithUsernamePasswordView;

        private LoginWithUsernamePasswordView2 loginWithUsernamePasswordView2;

        private LoginWithUsernamePasswordView3 loginWithUsernamePasswordView3;

        [ImportingConstructor]
        public LoginWithUsernamePasswordModule()
            : base(AppScreens.LoginScreen)
        {
            //SetNavigationCommand(LoOv.K(o => Resources.Logout), LoOv.K(o => Resources.Common), "logout.png", 80);
        }

        protected LoginView LoginView => loginView
                                         ?? (loginView = ServiceLocator.Current.GetInstance<LoginView>());

        protected LoginWithUsernamePasswordView LoginWithUsernamePasswordView =>
            loginWithUsernamePasswordView ?? (loginWithUsernamePasswordView =
                ServiceLocator.Current.GetInstance<LoginWithUsernamePasswordView>());

        protected LoginWithUsernamePasswordView2 LoginWithUsernamePasswordView2 =>
            loginWithUsernamePasswordView2 ?? (loginWithUsernamePasswordView2 =
                ServiceLocator.Current.GetInstance<LoginWithUsernamePasswordView2>());

        protected LoginWithUsernamePasswordView3 LoginWithUsernamePasswordView3 => loginWithUsernamePasswordView3
            ?? (loginWithUsernamePasswordView3 = ServiceLocator.Current.GetInstance<LoginWithUsernamePasswordView3>());

        protected override void OnInitialization()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(LoginWithUsernamePasswordView));
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(LoginWithUsernamePasswordView2));

            EventServiceFactory.EventService.GetEvent<GenericIdEvent>().Subscribe(OnEvent);
        }

        private void OnEvent(EventParameters<int> obj)
        {
            if (obj.Topic == EventTopicNames.ClockInWithUsernamePassword)
            {
                var loginType = SettingService.ProgramSettings.LoginType;
                if (loginType.Equals(PosConsts.PinLogin))
                    RegionManager.ActivateRegion(RegionNames.MainRegion, LoginView);
                if (loginType.Equals(PosConsts.UserLogin))
                {
                    LoginWithUsernamePasswordView3.viewModel.IsFromClockInOut = true;
                    LoginWithUsernamePasswordView3.viewModel.IsClockOut = false;
                    RegionManager.ActivateRegion(RegionNames.MainRegion, LoginWithUsernamePasswordView3);
                }

            }

            if (obj.Topic == EventTopicNames.ClockOutWithUsernamePassword)
            {
                LoginWithUsernamePasswordView3.viewModel.IsFromClockInOut = true;
                LoginWithUsernamePasswordView3.viewModel.IsClockOut = true;
                RegionManager.ActivateRegion(RegionNames.MainRegion, LoginWithUsernamePasswordView3);
            }


        }

        protected override bool CanNavigate(string arg)
        {
            return true;
        }

        public override object GetVisibleView()
        {
            return LoginWithUsernamePasswordView;
        }
    }
}