using DinePlan.Common.License;
using DinePlan.Common.POS;
using DinePlan.Domain.Models.Tickets;
using DinePlan.Domain.Models.Users;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Engine;
using DinePlan.Localization.Properties;
using DinePlan.Modules.LoginModule.Dialog;
using DinePlan.Persistance.Data;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Mef.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using DinePlan.Common.Model.Sync;

namespace DinePlan.Modules.LoginModule
{
    [ModuleExport(typeof(LoginModule))]
    public class LoginModule : VisibleModuleBase
    {
        private readonly IEnumerable<LanguageCommandButton> _supportedLanguages;

        private LoginWithUsernamePasswordView2 loginUserNamePasswordView;

        /// <summary>
        ///     The back up value for <see cref="LoginView" /> property.
        /// </summary>
        private LoginView loginView;

        private LoginWithUsernamePasswordView3 loginWithUsernamePasswordView3;

        [ImportingConstructor]
        public LoginModule()
            : base(AppScreens.LoginScreen)
        {
            SetNavigationCommand(LoOv.K(o => Resources.Logout), LoOv.K(o => Resources.Common), "logout.png", 80);
        }

        /// <summary>
        ///     Gets or sets the LoginView service.
        /// </summary>
        protected LoginView LoginView => loginView ?? (loginView = ServiceLocator.Current.GetInstance<LoginView>());

        protected LoginWithUsernamePasswordView3 LoginWithUsernamePasswordView3 => loginWithUsernamePasswordView3
            ?? (loginWithUsernamePasswordView3 = ServiceLocator.Current.GetInstance<LoginWithUsernamePasswordView3>());

        protected LoginWithUsernamePasswordView2 LoginUserNamePasswordView => loginUserNamePasswordView
                                                                              ?? (loginUserNamePasswordView =
                                                                                  ServiceLocator.Current
                                                                                      .GetInstance<
                                                                                          LoginWithUsernamePasswordView2
                                                                                      >());

        protected override bool CanNavigate(string arg)
        {
            return true;
        }

        protected override void OnNavigate(string obj)
        {
            UserService.LogoutUser();
        }

        protected override void OnInitialization()
        {
            RegionManager.RegisterViewWithRegion("MainRegion", typeof(LoginView));
            RegionManager.RegisterViewWithRegion("MainRegion", typeof(LoginWithUsernamePasswordView3));


            EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.ShellInitialized)
                        Activate();

                    else if (x.Topic == EventTopicNames.ChangeUserLanguage) ChangeLanguage();
                });

            EventServiceFactory.EventService.GetEvent<GenericEvent<string>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.PinSubmitted)
                    {
                        try
                        {
                            File.Copy(LocalSettings.SettingsFileName, LocalSettings.BackupSettingsFileName,
                                true);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        var open = true;
                        if (!LicenseSettings.IsTrial)
                        {
                            var validator = DineLicenseManager.GetLicenseManager().GetValidator();
                            var localContent = validator?.LocalSuretyContent;
                            if (localContent != null)
                            {
                                var totalDay = localContent.TrialExpires - DateTime.Now;
                                if (totalDay.Days < 30 && totalDay.Days >= 0)
                                {
                                    UserInteraction.ShowMessage(string.Format(LoOv.G(o => Resources.LicenseDays),
                                        totalDay.Days));
                                }
                                else if (totalDay.Days < 0)
                                {
                                    UserInteraction.ShowMessage(LoOv.G(o => Resources.LicenseExpired));
                                    open = false;
                                }
                            }
                        }
                        else
                        {
                            var ticket = Dao.Count<Ticket>();
                            if (ticket > 10000000)
                            {
                                UserInteraction.ShowMessage(LoOv.G(o => Resources.LicenseExpired));
                                open = false;
                            }
                        }
                        if (open)
                            if (!string.IsNullOrEmpty(x.Value))
                                PinEntered(x.Value);
                    }
                });
        }

        public void PinEntered(string pin)
        {
            var myUser = new User("", pin);
            var user = UserService.LoginUser(myUser);

            //if (!string.IsNullOrEmpty(user?.Language))
            //    ChangeToLanguage(user.Language);

            if (!string.IsNullOrEmpty(user?.AlternateLanguage))
                SyncConstants.AlternateLanguage = user.AlternateLanguage;
            
        }

        private static void ChangeToLanguage(string language)
        {
            LocalizeDictionary.ChangeLanguage(language);

            LoOv.Refresh();

            var cul = CultureInfo.GetCultureInfo(language);
            ServiceLocator.Current.GetInstance<ILocalizationService>().InitializeAsync(language);
            Thread.CurrentThread.CurrentUICulture = cul;

            EventServiceFactory.EventService.PublishEvent(EventTopicNames.RefreshNavigationCommands);
        }

        public void ChangeLanguage()
        {
            var vm = ServiceLocator.Current.GetInstance<ChangeLanguageDialogViewModel>();
            vm.LanguageCommands = GetSupportedLanguages();

            var button = vm.ShowDialog();

            if (button != null && GetSupportedLanguages().Any(t => t.Name == button.Name))
            {
                ChangeToLanguage(button.Name);

                ApplicationState.CurrentLoggedInUser.Language = button.Name;

                UserService.ChangeUserLanguage(ApplicationState.CurrentLoggedInUser.Id,
                    ApplicationState.CurrentLoggedInUser.Language);
            }
        }

        private IEnumerable<LanguageCommandButton> GetSupportedLanguages()
        {
            return LocalSettings.SupportedLanguages.Select(CultureInfo.GetCultureInfo)
                .ToList()
                .OrderBy(x => x.NativeName)
                .Where(t => t.Name != ApplicationState.CurrentLoggedInUser.Language)
                .Select(t => new LanguageCommandButton
                {
                    NativeName = t.NativeName,
                    Name = t.Name
                });
        }

        public override object GetVisibleView()
        {
            if (SettingService.ProgramSettings.LoginType != null &&
                SettingService.ProgramSettings.LoginType.Equals(PosConsts.UserLogin))
            {
                LoginWithUsernamePasswordView3.viewModel.IsClockOut = false;
                LoginWithUsernamePasswordView3.viewModel.IsFromClockInOut = false;
                return LoginWithUsernamePasswordView3;
            }

            LoginView.IsClockOut = false;
            return LoginView;
        }
    }
}