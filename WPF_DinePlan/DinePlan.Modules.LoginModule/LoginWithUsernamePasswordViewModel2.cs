using DinePlan.Domain.Models.Users;
using DinePlan.Presentation.Common.ModelBase;
using Prism.Commands;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using DinePlan.Common.Model.Sync;
using DinePlan.Localization;
using DinePlan.Localization.Engine;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using System.Windows.Threading;
using DinePlan.Localization.Properties;
using DinePlan.Domain.Models.Employee;
using DinePlan.Presentation.Common.Biometric.Persona;
using System;
using System.Linq;
using DinePlan.Presentation.Common.Biometric.Interfaces;
using DPUruNet;

namespace DinePlan.Modules.LoginModule
{
    /// <summary>
    ///     The view model for Login with Username and Password view.
    /// </summary>
    /// <seealso cref="DinePlan.Presentation.Common.ModelBase.GenericViewModelbase" />
    [Export]
    public class LoginWithUsernamePasswordViewModel2 : GenericViewModelbase
    {
        [ImportingConstructor]
        public LoginWithUsernamePasswordViewModel2()
        {
            InitialSetting();
        }

        private void InitialSetting()
        {
            UsernameVisibility = Visibility.Visible;
            PasswordVisibility = Visibility.Hidden;
            NameVisibility = Visibility.Collapsed;
            Title = "Enter Username";
            Username = "";
            Password = "";
            ErrorMessage = "";
        }

        #region IsExistedUsername

        private bool IsExistedUsername()
        {
            return UserService.ConfirmUserName(Username);
            ;
        }

        #endregion

        #region Properties
        /// <summary>
        ///     The adapter
        /// </summary>
        private IFingerPrintScannerAdapter adapter;

        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string username;

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string password;

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private Visibility usernameVisibility;

        public Visibility UsernameVisibility
        {
            get => usernameVisibility;
            set
            {
                SetProperty(ref usernameVisibility, value);
                if (value == Visibility.Visible)
                {
                    Title = "Enter Username";
                    PasswordVisibility = Visibility.Hidden;
                    ErrorMessage = "";
                }
                else
                {
                    Title = "Enter Password";
                    PasswordVisibility = Visibility.Visible;
                    ErrorMessage = "";
                }
            }
        }

        private Visibility passwordVisibility;

        public Visibility PasswordVisibility
        {
            get => passwordVisibility;
            set => SetProperty(ref passwordVisibility, value);
        }

        private Visibility nameVisibility;

        public Visibility NameVisibility
        {
            get => nameVisibility;
            set => SetProperty(ref nameVisibility, value);
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsClockOut { get; set; } = false;
        public bool VerifyUser { get; set; } = false;

        #endregion

        #region NextCommmand

        public ICommand NextCommmand
        {
            get { return new DelegateCommand<object>(args => { NextExecute(); }); }
        }

        private void NextExecute()
        {
            if (UsernameVisibility == Visibility.Visible)
            {
                var isExistedUsername = IsExistedUsername();
                if (isExistedUsername)
                {
                    UsernameVisibility = Visibility.Hidden;
                    NameVisibility = Visibility.Visible;
                }
                else
                {
                    ErrorMessage = "Username not found";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Password))
                {
                    ErrorMessage = "Password is incorrect";
                    return;
                }

                var loginUser = new User
                {
                    ConnectUser = Username,
                    UnhasedPassword = Password
                };
                User user = null;
                user = VerifyUser ? UserService.VerifyUser(loginUser) 
                    : UserService.LoginUser(loginUser);

                if (user != null)
                {
                    if (VerifyUser)
                    {
                        EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>()
                            .Publish(new EventParameters<EventAggregator> { Topic = EventTopicNames.ShellInitialized });



                        if (!string.IsNullOrEmpty(user?.Language))
                            ChangeToLanguage(user.Language);

                        if (!string.IsNullOrEmpty(user?.AlternateLanguage))
                            SyncConstants.AlternateLanguage = user.AlternateLanguage;
                    }
                    else
                    {
                        InitialSetting();
                    }
                }
                else
                    ErrorMessage = "Password is incorrect";

               
            }
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

       
        #endregion

        #region BackCommmand

        public ICommand BackCommmand
        {
            get { return new DelegateCommand(() => { BackExecute(); }); }
        }

        private void BackExecute()
        {
            UsernameVisibility = Visibility.Visible;
            NameVisibility = Visibility.Collapsed;
            Password = "";
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            WaitForFingerScan();

        }
        
        private void WaitForFingerScan()
        {

            UpdateMessage(LoOv.G(o => Resources.WaitingForFingerPrint));

            try
            {
                adapter = GetScannerAdapter();

                if (adapter != null)
                    adapter.OnCaptured(captureResult =>
                    {
                        try
                        {
                            var fmdResult = FeatureExtraction.CreateFmdFromFid(captureResult.Data,
                                Constants.Formats.Fmd.DP_VERIFICATION);
                            //If successfully extracted fmd then assign fmd
                            if (fmdResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                            {
                                var fmd = fmdResult.Data;

                                var employees = EmployeeService.GetAllEmployeeWithIdentity().ToList();

                                //Perform indentification of fmd of captured sample against enrolledFmds for userid 
                                var iResult1 = Comparison.Identify(fmd, 0,
                                    employees.Select(c => new Fmd(c.IdenitityData, int.Parse(c.Identity.Split(':')[1]),
                                        c.Identity.Split(':')[0])), 21474, 1);

                                //If Identify was successfull
                                if (iResult1.ResultCode == Constants.ResultCode.DP_SUCCESS)
                                {
                                    //If number of matches were greater than 0
                                    if (iResult1.Indexes.Length > 0)
                                    {
                                        //for each matched fmd get its index and map that index to userid index
                                        var index = iResult1.Indexes[0][0];
                                        ScanFinish(employees[index]);
                                        return;
                                    }
                                }
                                else
                                {
                                    DialogService.ShowFeedback(LoOv.G(o => Localization.Properties.Resources.UsernameNotFound));
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            UpdateMessage(ex.Message);
                        }
                    });

            }
            catch (Exception ex)
            {
                UpdateMessage(ex.Message);
            }
        }


        /// <summary>
        ///     Gets the scanner adapter.
        /// </summary>
        /// <returns>The scanner adapter.</returns>
        private IFingerPrintScannerAdapter GetScannerAdapter()
        {
            var myDevice = DeviceService.GetBioMetricDevice();
            if (myDevice != null)
                if (myDevice.Name.Equals("Persona"))
                {
                    var settings = (PersonaDeviceSettings)myDevice.GetSettingsObject();
                    if (settings != null)
                    {
                        int myDeviceValue = 0;
                        if (!string.IsNullOrEmpty(settings.DeviceName))
                        {
                            myDeviceValue = Convert.ToInt32(settings.DeviceName);
                        }
                        else
                        {
                            if (settings.DeviceNameValue != null
                                && settings.DeviceNameValue.Values != null && settings.DeviceNameValue.Values.Any())
                            {
                                var myText = settings.DeviceNameValue.Values.Last();
                                myDeviceValue = Convert.ToInt32(myText);
                            }
                        }
                        if (myDeviceValue > 0)
                        {
                            return new UruScannerAdapter(Convert.ToInt32(myDeviceValue));
                        }
                        UpdateMessage(LoOv.G(o => Resources.DeviceIDIsNotSet));
                    }
                }


            return null;
        }
        /// <summary>
        ///     Finish the finger print scan.
        /// </summary>
        /// <param name="identity">The identity.</param>
        private void ScanFinish(Employee employee)
        {
            if (employee.DinePlanUserId <= 0 || !UserService.ContainsUser(employee.DinePlanUserId))
            {
                UpdateMessage(LoOv.G(o => Resources.EmployeeIsNotUser));
                return;
            }

            var user = UserService.GetUserById(employee.DinePlanUserId);

            if (user == null)
            {
                UpdateMessage(LoOv.G(o => Resources.EmployeeIsNotUser));
                return;
            }
            Application.Current.Dispatcher.BeginInvoke((Action)(() => 
            {
                EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>()
                            .Publish(new EventParameters<EventAggregator> { Topic = EventTopicNames.ShellInitialized });



                if (!string.IsNullOrEmpty(user?.Language))
                    ChangeToLanguage(user.Language);

                if (!string.IsNullOrEmpty(user?.AlternateLanguage))
                    SyncConstants.AlternateLanguage = user.AlternateLanguage;
            }));
            
        }
        /// <summary>
        ///     Updates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void UpdateMessage(string message)
        {
            //Message = message;
            Dispatcher.CurrentDispatcher.DoEvents();
        }
        #endregion
    }
}