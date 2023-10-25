using DinePlan.Domain.Models.Employee;
using DinePlan.Domain.Models.Users;
using DinePlan.Localization;
using DinePlan.Localization.Engine;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Biometric.Interfaces;
using DinePlan.Presentation.Common.Biometric.Persona;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Common.Services;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services;
using DPUruNet;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DinePlan.Modules.LoginModule
{
    /// <summary>
    ///     The view model for login.
    /// </summary>
    /// <seealso cref="GenericViewModelbase" />
    [Export]
    public class LoginWithUsernamePasswordViewModel3 : GenericViewModelbase
    {
        #region [Constructors]

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginViewModel" /> class.
        /// </summary>
        [ImportingConstructor]
        public LoginWithUsernamePasswordViewModel3(IModuleManager moduleManager)
        {
            _mModuleManager = moduleManager;
            ShowHidePasswordCommand = new DelegateCommand(ShowHidePasswordExecute);
            ShowKeyboardCommand = new DelegateCommand(ShowKeyboardExecute);
            SubmitCommand = new DelegateCommand(SubmitExecute);
            EmployeeCommand = new DelegateCommand(EmployeeExecute);
            ExitCommand = new DelegateCommand(ExitExecute);
            InitialSetting(null);
        }

        #endregion [Constructors]

        #region IsExistedUsername

        private bool IsExistedUsername()
        {
            return UserService.ConfirmUserName(Username);
            ;
        }

        #endregion

        #region [Properties]
        /// <summary>
        ///     The adapter
        /// </summary>
        private IFingerPrintScannerAdapter adapter;

        DispatcherTimer timer;

        private string errorMessage;

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        private readonly IModuleManager _mModuleManager;


        private Visibility visibleVisibility;

        public Visibility VisibleVisibility
        {
            get => visibleVisibility;
            set
            {
                SetProperty(ref visibleVisibility, value);
                if (value == Visibility.Visible)
                    InvisibleVisibility = Visibility.Collapsed;
                else
                    InvisibleVisibility = Visibility.Visible;
            }
        }

        private Visibility invisibleVisibility;

        public Visibility InvisibleVisibility
        {
            get => invisibleVisibility;
            set => SetProperty(ref invisibleVisibility, value);
        }

        public bool ShowShutDownButton => SettingService.ProgramSettings.ShutdownOnLogin;

        public bool IsClockOut { get; set; } = false;
        public bool IsFromClockInOut { get; set; } = false;
        public bool IsExecuteTimer { get; set; }

        private bool isShowPassword;

        public bool IsShowPassword
        {
            get => isShowPassword;
            set => SetProperty(ref isShowPassword, value);
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
            set
            {
                if (!IsShowPassword)
                {
                    if (value.Length == 0)
                    {
                        passwordValue = "";
                        SetProperty(ref password, value);
                        return;
                    }

                    if (passwordValue.Length > value.Length)
                    {
                        passwordValue = passwordValue.Substring(0, value.Length);
                        for (var i = 0; i < value.Length; i++)
                            if (value[i] != '●')
                            {
                                passwordValue = passwordValue.Replace(passwordValue[i], value[i]);
                                value = value.Replace(value[i], '●');
                            }

                        SetProperty(ref password, value);
                    }
                    else
                    {
                        for (var i = 0; i < value.Length; i++)
                            if (value[i] != '●')
                            {
                                if (passwordValue.Length < value.Length)
                                    passwordValue = passwordValue.Insert(i, value[i].ToString());
                                else
                                    passwordValue = passwordValue.Replace(passwordValue[i], value[i]);
                                SetProperty(ref password, value.Replace(value[i], '●'));
                            }
                    }
                }
                else
                {
                    passwordValue = value;
                    SetProperty(ref password, value);
                }
            }
        }

        private string passwordValue = "";

        #endregion

        #region [ShowHidePasswordCommand]

        public ICommand ShowHidePasswordCommand { get; }

        private void ShowHidePasswordExecute()
        {
            if (IsShowPassword)
            {
                IsShowPassword = false;
                VisibleVisibility = Visibility.Collapsed;
                for (var i = 0; i < Password.Length; i++) Password = Password.Replace(Password[i], '●');
            }
            else
            {
                IsShowPassword = true;
                VisibleVisibility = Visibility.Visible;
                Password = passwordValue;
            }
        }

        #endregion

        #region [ShowKeyboardCommand]

        public ICommand ShowKeyboardCommand { get; }

        private void ShowKeyboardExecute()
        {
            InteractionService.ShowKeyboard();
        }

        #endregion

        #region [SubmitCommand]

        public ICommand SubmitCommand { get; }

        public void SubmitExecute()
        {
            var isExistedUsername = IsExistedUsername();
            if (!isExistedUsername)
            {
                ErrorMessage = LoOv.G(o => Resources.UsernameNotFound);
                return;
            }

            if (string.IsNullOrEmpty(passwordValue))
            {
                ErrorMessage = LoOv.G(o => Resources.PasswordIsIncorrect);
                return;
            }

            var loginUser = new User
            {
                ConnectUser = Username,
                UnhasedPassword = passwordValue
            };
            User user = null;
            if (this.IsFromClockInOut)
            {
                user = UserService.VerifyUser(loginUser);
                if (user != null)
                {
                    ProcessClockInOut(user);
                }
            }
            else
            {
                user = UserService.LoginUser(loginUser);
            }
            if (user != null)
            {
                InitialSetting(user);

            }
            else
            {
                ErrorMessage = LoOv.G(o => Resources.PasswordIsIncorrect);
            }
        }

        private void InitialSetting(User user)
        {
            VisibleVisibility = Visibility.Hidden;
            Username = "";
            Password = "";
            passwordValue = "";
            ErrorMessage = "";
            IsShowPassword = false;

            if (!string.IsNullOrEmpty(user?.Language))
                ChangeToLanguage(user.Language);
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

        #region [EmployeeCommand]

        public ICommand EmployeeCommand { get; }

        private void EmployeeExecute()
        {
            _mModuleManager.InitModule("EmployeeModule", true);
            EventServiceFactory.EventService.GetEvent<GenericEvent<Employee>>().Publish(null);
        }

        #endregion

        #region [ExitCommand]

        public ICommand ExitCommand { get; }

        private void ExitExecute()
        {
            var goAhead = true;
            if (SettingService.ProgramSettings.PinOnExitDinePlan)
            {
                var str = DialogService.AskUserPin();
                if (string.IsNullOrEmpty(str)) goAhead = false;
                var status = UserService.CanConfirmAdminPin(str);
                goAhead = status;
            }

            if (goAhead)
            {
                DeviceService.FinalizeDevices();
                NancyService.Stop();
                Application.Current.Shutdown();
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            EnableClockInOutTimer();
            WaitForFingerScan();

        }

        private void EnableClockInOutTimer()
        {
            if (this.IsFromClockInOut)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                }
                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(30)
                };

                timer.Tick += (s, ee) =>
                {
                    timer.Stop();
                    if (IsExecuteTimer)
                    {
                        EventServiceFactory.EventService.PublishEvent(EventTopicNames.ActivateNavigation);
                        EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>()
                                    .Publish(new EventParameters<EventAggregator> { Topic = EventTopicNames.ClockInDialogOpen });
                    }

                };
                this.IsExecuteTimer = true;
                timer.Start();
            }
        }
        protected override void OnUnloaded()
        {
            base.OnUnloaded();
            this.IsExecuteTimer = false;
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

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
            if (this.IsFromClockInOut)
            {
                ProcessClockInOut(user);
            }
            else
            {
                user.PinCode.PublishEvent(EventTopicNames.PinSubmitted);
            }
        }

        private void ProcessClockInOut(User user)
        {
            this.IsExecuteTimer = false;
            var errorMessage = string.Empty;
            if (this.IsClockOut == false)
            {
                errorMessage = EnterRecords(user, CheckType.TimeIn);
            }
            else
            {
                errorMessage = EnterRecords(user, CheckType.TimeOut);
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => { DialogService.ShowFeedback(errorMessage); }));
            }
            EventServiceFactory.EventService.PublishEvent(EventTopicNames.ActivateNavigation);
        }
        /// <summary>
        /// This method will return an error message in case of error otherwise empty string
        /// </summary>
        /// <param name="status"></param>
        /// <param name="recordType"></param>
        /// <param name="showDialog"></param>
        /// <returns></returns>
        private string EnterRecords(User status, CheckType recordType)
        {
            var employee = EmployeeService.GetEmployeeByUserId(status.Id);
            if (employee == null)
            {
                return LoOv.G(o => Localization.Properties.Resources.NotFoundEmployee);
            }
            var latestClock = EmployeeService.GetLatestClock(employee.Id);
            if (latestClock != null && latestClock.CheckType == CheckType.TimeIn && recordType == CheckType.TimeIn)
            {
                return LoOv.G(o => Localization.Properties.Resources.ClockInIsAlreadyExisted);
            }
            if (latestClock != null && latestClock.CheckType == CheckType.TimeOut &&
                recordType == CheckType.TimeOut)
            {
                return LoOv.G(o => Localization.Properties.Resources.ClockOutIsAlreadyExisted);
            }
            if (latestClock == null && recordType == CheckType.TimeOut)
            {
                return LoOv.G(o =>
                        Localization.Properties.Resources.ClockInCouldNotBeFound);
            }

            UserService.SetTimeAttendance(status, recordType == CheckType.TimeOut);
            return string.Empty;
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