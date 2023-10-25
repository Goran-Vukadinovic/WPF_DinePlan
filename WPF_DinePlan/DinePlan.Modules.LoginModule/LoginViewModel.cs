using DinePlan.Common.License;
using DinePlan.Domain.Models.Employee;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Persistance.Data;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Biometric.Interfaces;
using DinePlan.Presentation.Common.Biometric.Persona;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DPUruNet;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DinePlan.Domain.Models.Users;
using DinePlan.Presentation.Services.Common.DataGeneration;
using DinePlan.Services.Implementations;

namespace DinePlan.Modules.LoginModule
{
    /// <summary>
    ///     The view model for login.
    /// </summary>
    /// <seealso cref="DinePlan.Presentation.Common.ModelBase.GenericViewModelbase" />
    [Export]
    public class LoginViewModel : GenericViewModelbase
    {
        #region [ Constructors ]

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginViewModel" /> class.
        /// </summary>
        [ImportingConstructor]
        public LoginViewModel(IModuleManager moduleManager)
        {
            _mModuleManager = moduleManager;
            EmployeeVisibility = LicenseSettings.DineConnect ? Visibility.Visible : Visibility.Collapsed;
            EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>().Subscribe(OnEvent);

            EventServiceFactory.EventService.GetEvent<GenericEvent<User>>().Subscribe(
                x =>
                {
                    if (x.Topic == EventTopicNames.UserLoggedOut)
                    {
                        ShowRefreshMultipleDatabaseSelector = LocalSettings.MultipleDatabases;                        
                    }
                });
        }

        #endregion [ Constructors ]

        #region [ Fields ]

        /// <summary>
        ///     The adapter
        /// </summary>
        private IFingerPrintScannerAdapter _adapter;

        /// <summary>
        ///     The back up value for <see cref="Message" /> property.
        /// </summary>
        private string _message;

        /// <summary>
        ///     The back up value for <see cref="NumPadCommand" /> property.
        /// </summary>
        private ICommand _numPadCommand;

        /// <summary>
        ///     The back up value for <see cref="SubmitPinCommand" /> property.
        /// </summary>
        private ICommand _submitPinCommand;

        /// <summary>
        ///     The back up value for <see cref="ExitCommand" /> property.
        /// </summary>
        private ICommand _exitCommand;

        /// <summary>
        ///     The back up value for <see cref="_shutdownCommand" /> property.
        /// </summary>
        private ICommand _shutdownCommand;

        /// <summary>
        ///     The back up value for <see cref="EmployeeCommand" /> property.
        /// </summary>
        private ICommand _employeeCommand;

        /// <summary>
        ///     The back up value for <see cref="ClearPinCommand" /> property.
        /// </summary>
        private ICommand _clearPinCommand;

        private ICommand _databaseConnectCommand;

        private ICommand _changeDatabaseCommand;

        /// <summary>
        ///     The back up value for <see cref="PinValue" /> property.
        /// </summary>
        private string _pinValue = string.Empty;

        /// <summary>
        ///     The back up value for <see cref="HiddenPinText" /> property.
        /// </summary>
        private string _hiddenPinText = "";

        /// <summary>
        ///     The back up value for <see cref="EmployeeVisibility" /> property.
        /// </summary>
        private Visibility _employeeVisibility;

        /// <summary>
        ///     The back up value for <see cref="IsLandscape" /> property.
        /// </summary>
        private bool _isLandscape;

        private bool _showMultipleDatabaseSelector = LocalSettings.MultipleDatabases;

        private bool _showRefreshMultipleDatabaseSelector = false;

        private ObservableCollection<string> _databaseNames;

        /// <summary>
        ///     The back up value for <see cref="Logo" /> property.
        /// </summary>
        private ImageSource _logo;

        private readonly IModuleManager _mModuleManager;

        #endregion [ Fields ]

        #region [ Public Properties ]   

        public bool ShowMultipleDatabaseSelector
        {
            get => _showMultipleDatabaseSelector;
            set
            {
                _showMultipleDatabaseSelector = value;
                RaisePropertyChanged(nameof(ShowMultipleDatabaseSelector));
            }
        }

        public bool ShowRefreshMultipleDatabaseSelector
        {
            get => _showRefreshMultipleDatabaseSelector;
            set
            {
                _showRefreshMultipleDatabaseSelector = value;
                RaisePropertyChanged(nameof(ShowRefreshMultipleDatabaseSelector));
            }
        }

        public ObservableCollection<string> DatabaseNames
        {
            get
            {
                if(_databaseNames == null)
                {
                    _databaseNames = new ObservableCollection<string>();
                    GetDatabases();
                }
                return _databaseNames;
            }
            set
            {
                _databaseNames = value;
                RaisePropertyChanged(nameof(DatabaseNames));
            }
        }

        /// <summary>
        ///     Gets or sets the Message.
        /// </summary>
        private string Message
        {
            get => _message;

            set
            {
                if (_message == value) return;
                _message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }

        public bool ShowShutDownButton => SettingService.ProgramSettings.ShutdownOnLogin;

        /// <summary>
        ///     Gets or sets the EmployeeVisibility.
        /// </summary>
        public Visibility EmployeeVisibility
        {
            get => _employeeVisibility;

            set
            {
                if (_employeeVisibility != value)
                {
                    _employeeVisibility = value;
                    RaisePropertyChanged(nameof(EmployeeVisibility));
                }
            }
        }

        /// <summary>
        ///     Gets the admin password hint.
        /// </summary>
        /// <value>
        ///     The admin password hint.
        /// </value>
        public string AdminPasswordHint => GetAdminPasswordHint();

        /// <summary>
        ///     Gets the SQL hint.
        /// </summary>
        /// <value>
        ///     The SQL hint.
        /// </value>
        public string SqlHint => GetSqlHint();

        public ICommand DatabaseConnectCommand =>
            _databaseConnectCommand ?? (_databaseConnectCommand = new DelegateCommand<object>(UpdateWorkspaceFactoryConnectToSelectDatabase));

        /// <summary>
        ///     Gets the myProperty command.
        /// </summary>
        public ICommand NumPadCommand =>
            _numPadCommand ?? (_numPadCommand = new DelegateCommand<string>(ExecuteNumPadCommand));

        /// <summary>
        ///     Gets the SubmitPin command.
        /// </summary>
        public ICommand SubmitPinCommand =>
            _submitPinCommand ?? (_submitPinCommand = new DelegateCommand(ExecuteSubmitPinCommand));

        /// <summary>
        ///     Gets the exit command.
        /// </summary>
        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new DelegateCommand(ExecuteExitCommand));

        public ICommand ShutdownCommand =>
            _shutdownCommand ?? (_shutdownCommand = new DelegateCommand(ExecuteShutdownCommand));

        /// <summary>
        ///     Gets the employee command.
        /// </summary>
        public ICommand EmployeeCommand =>
            _employeeCommand ?? (_employeeCommand = new DelegateCommand(ExecuteEmployeeCommand));

        /// <summary>
        ///     Gets the clearPin command.
        /// </summary>
        public ICommand ClearPinCommand =>
            _clearPinCommand ?? (_clearPinCommand = new DelegateCommand(ExecuteClearPinCommand));

        public ICommand ChangeDatabaseCommand =>
            _changeDatabaseCommand ?? (_changeDatabaseCommand = new DelegateCommand(() => {
                Process.Start(Application.ResourceAssembly.Location);
                Process.GetCurrentProcess().Kill();
            }));

        /// <summary>
        ///     Gets or sets the HiddenPinText.
        /// </summary>
        public string HiddenPinText
        {
            get => _hiddenPinText;

            set
            {
                if (_hiddenPinText != value)
                {
                    _hiddenPinText = value;
                    RaisePropertyChanged(nameof(HiddenPinText));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the IsLandscape.
        /// </summary>
        public bool IsLandscape
        {
            get => _isLandscape;

            set
            {
                if (_isLandscape != value)
                {
                    _isLandscape = value;
                    RaisePropertyChanged(nameof(IsLandscape));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the PinValue.
        /// </summary>
        private string PinValue
        {
            get => _pinValue;

            set
            {
                if (_pinValue != value)
                {
                    _pinValue = value;
                    RaisePropertyChanged(nameof(PinValue));
                    HiddenPinText = string.Empty.PadLeft(value.Length, '*');
                }
            }
        }

        /// <summary>
        ///     Gets or sets the Logo.
        /// </summary>
        public ImageSource Logo
        {
            get => _logo;

            set
            {
                if (_logo != value)
                {
                    _logo = value;
                    RaisePropertyChanged(nameof(Logo));
                }
            }
        }

        #endregion [ Public Properties ]

        #region [ Protected Methods ]

        /// <summary>
        ///     Called when [loaded].
        /// </summary>
        protected override void OnLoaded()
        {
            WaitForFingerScan();
            Logo =
                new BitmapImage(new Uri("pack://application:,,,/DinePlan.Infrastructure;component/Files/DinePlan.png"));

            if (LocalSettings.LogoPath != null && File.Exists(LocalSettings.LogoPath))
            {
                var uriSource = new Uri(LocalSettings.LogoPath, UriKind.Absolute);
                try
                {
                    Logo = new BitmapImage(uriSource);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///     Called when [unloaded].
        /// </summary>
        protected override void OnUnloaded()
        {
            if (_adapter != null) _adapter.CancelCurrentOperation();
        }

        #endregion [ Protected Methods ]

        #region [ Private Methods ]

        /// <summary>
        ///     Gets the admin password hint.
        /// </summary>
        /// <returns></returns>
        private string GetAdminPasswordHint()
        {
            if ((GetDatabaseLabel() == "TX" || GetDatabaseLabel() == "CE") && UserService.IsDefaultUserConfigured)
                return LoOv.G(o => Resources.AdminPasswordHint);

            return "";
        }

        /// <summary>
        ///     Gets the database label.
        /// </summary>
        /// <returns></returns>
        private static string GetDatabaseLabel()
        {
            return LocalSettings.DatabaseLabel;
        }

        /// <summary>
        ///     Gets the SQL hint.
        /// </summary>
        /// <returns></returns>
        private string GetSqlHint()
        {
            return !string.IsNullOrEmpty(GetAdminPasswordHint())
                ? LoOv.G(o => Resources.SqlHint)
                : "";
        }

        /// <summary>
        ///     Waits for finger scan.
        /// </summary>
        private void WaitForFingerScan()
        {
            try
            {
                _adapter = GetScannerAdapter();

                if (_adapter != null)
                    _adapter.OnCaptured(captureResult =>
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
                                    employees.Select(
                                        c =>
                                            new Fmd(c.IdenitityData, int.Parse(c.Identity.Split(':')[1]),
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
            catch
            {
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

            user.PinCode.PublishEvent(EventTopicNames.PinSubmitted);
        }

        /// <summary>
        ///     Updates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void UpdateMessage(string message)
        {
            Message = message;
            Dispatcher.CurrentDispatcher.DoEvents();
        }

        /// <summary>
        ///     Executes the <see cref="NumPadCommand"> command.
        /// </summary>
        private void ExecuteNumPadCommand(string parameter)
        {
            if (CheckPinValue()) PinValue += parameter;
        }

        private void UpdateWorkspaceFactoryConnectToSelectDatabase(object item)
        {
            if(item != null && item is string databaseName)
            {
                var newConnectionString = LocalSettings.ConnectionString.Replace(LocalSettings.CSDatabase, databaseName);
                WorkspaceFactory.UpdateConnection(newConnectionString);
                LocalSettings.ConnectionString = newConnectionString;

                var creationService = new DataCreationService();
                creationService.CreateData();

                EventServiceFactory2.EventService.GetEvent<PubSubEvent<string>>().Publish(EventTopicNames.DatabaseChanged);

                if (VerifyDatabaseSchema())
                {
                    ShowMultipleDatabaseSelector = false;
                }
                else
                {
                    DialogService.ShowFeedback(LoOv.G(o=>Resources.WrongDatabase));
                    ShowMultipleDatabaseSelector = true;
                }
            }
        }

        private bool VerifyDatabaseSchema()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(LocalSettings.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("select top(1) Version from VersionInfo order by version desc", con))
                    {
                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            var myRead = dr.Read();
                            return myRead;
                        }
                    }
                }
            }
            catch (Exception){}
            return false;
        }

        private void GetDatabases()
        {
            if (ShowMultipleDatabaseSelector)
            {
                using (SqlConnection con = new SqlConnection(LocalSettings.ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                    {
                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                if (dr[0].ToString().StartsWith(LocalSettings.AppName+"_", StringComparison.OrdinalIgnoreCase))
                                {
                                    DatabaseNames.Add(dr[0].ToString());
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Executes the <see cref="SubmitPinCommand"> command.
        /// </summary>
        private void ExecuteSubmitPinCommand()
        {
            EventServiceFactory.EventService.PublishEvent(EventTopicNames.ShowLoadingIndicator);
            ApplicationExtensions.DoEvents();

            if (AppServices.CanStartApplication())
                PinValue.PublishEvent(EventTopicNames.PinSubmitted);
            else
                MessageBox.Show(LoOv.G(o => Resources.CheckDBVersion));

            PinValue = string.Empty;
            EventServiceFactory.EventService.PublishEvent(EventTopicNames.HideLoadingIndicator);
        }

        /// <summary>
        ///     Executes the <see cref="ExitCommand"> command.
        /// </summary>
        private void ExecuteExitCommand()
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

        /// <summary>
        ///     Executes the <see cref="ShutdownCommand"> command.
        /// </summary>
        private void ExecuteShutdownCommand()
        {
            Process.Start("shutdown", "/s /t 0");
        }

        /// <summary>
        ///     Executes the <see cref="EmployeeCommand"> command.
        /// </summary>
        private void ExecuteEmployeeCommand()
        {
            _mModuleManager.InitModule("EmployeeModule", true);
            EventServiceFactory.EventService.GetEvent<GenericEvent<Employee>>().Publish(null);
        }

        /// <summary>
        ///     Executes the <see cref="ClearPinCommand"> command.
        /// </summary>
        private void ExecuteClearPinCommand()
        {
            PinValue = string.Empty;
        }

        /// <summary>
        ///     Checks the pin value.
        /// </summary>
        /// <returns></returns>
        private bool CheckPinValue()
        {
            return PinValue.Length < 19;
        }

        /// <summary>
        ///     Called when [event].
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OnEvent(EventParameters<EventAggregator> obj)
        {
            switch (obj.Topic)
            {
                case EventTopicNames.DisableLandscape:
                    IsLandscape = false;
                    break;

                case EventTopicNames.EnableLandscape:
                    IsLandscape = true;
                    break;                
            }
        }

        #endregion [ Private Methods ]
    }
}