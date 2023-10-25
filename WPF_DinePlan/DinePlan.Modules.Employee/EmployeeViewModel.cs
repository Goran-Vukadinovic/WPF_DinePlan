using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Biometric.Interfaces;
using DinePlan.Presentation.Common.Biometric.Persona;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Common.Services;
using DinePlan.Presentation.Services.Common;
using DPUruNet;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DinePlan.Modules.Employee
{
    /// <summary>
    ///     The view model for Employee View.
    /// </summary>
    /// <seealso cref="DinePlan.Presentation.Common.ObservableObject" />
    [Export]
    public class EmployeeViewModel : GenericViewModelbase
    {
        #region [ Constructors ]

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmployeeViewModel" /> class.
        /// </summary>
        [ImportingConstructor]
        public EmployeeViewModel()
        {
            ShowKeyboardCommand = new DelegateCommand(ShowKeyboardExecute);
        }

        #endregion

        #region [ Protected Methods ]

        /// <summary>
        ///     Called when [loaded].
        /// </summary>
        protected override void OnLoaded()
        {
            Logo = new BitmapImage(
                new Uri("pack://application:,,,/DinePlan.Infrastructure;component/Files/DinePlan.png"));

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

            ControlVisibility = Visibility.Hidden;
            EmployeeCodeVisibility = Visibility.Visible;
            EmployeeCode = string.Empty;
            RescanVisibility = Visibility.Collapsed;
            WaitForFingerScan();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        ///     The back up value for <see cref="Message" /> property.
        /// </summary>
        private string message;

        /// <summary>
        ///     The back up value for <see cref="EmployeeCode" /> property.
        /// </summary>
        private string employeeCode;

        /// <summary>
        ///     The back up value for <see cref="CurrentEmployee" /> property.
        /// </summary>
        private Domain.Models.Employee.Employee currentEmployee;

        /// <summary>
        ///     The back up value for <see cref="BreakInCommand" /> property.
        /// </summary>
        private ICommand breakInCommand;

        /// <summary>
        ///     The back up value for <see cref="BreakOutCommand" /> property.
        /// </summary>
        private ICommand breakOutCommand;

        /// <summary>
        ///     The back up value for <see cref="TimeInCommand" /> property.
        /// </summary>
        private ICommand timeInCommand;

        /// <summary>
        ///     The back up value for <see cref="TimeOutCommand" /> property.
        /// </summary>
        private ICommand timeOutCommand;

        /// <summary>
        ///     The back up value for <see cref="CancelCommand" /> property.
        /// </summary>
        private ICommand cancelCommand;

        /// <summary>
        ///     The back up value for <see cref="ExitCommand" /> property.
        /// </summary>
        private ICommand exitCommand;

        /// <summary>
        ///     The back up value for <see cref="EnterCodeCommand" /> property.
        /// </summary>
        private ICommand enterCodeCommand;

        /// <summary>
        ///     The back up value for <see cref="PullEmployeeCommand" /> property.
        /// </summary>
        private ICommand pullEmployeeCommand;

        /// <summary>
        ///     The back up value for <see cref="RescanCommand" /> property.
        /// </summary>
        private ICommand rescanCommand;

        /// <summary>
        ///     The adapter
        /// </summary>
        private IFingerPrintScannerAdapter adapter;

        /// <summary>
        ///     The back up value for <see cref="ControlVisibility" /> property.
        /// </summary>
        private Visibility cotnrolVisibility;

        /// <summary>
        ///     The back up value for <see cref="EmployeeCodeVisibility" /> property.
        /// </summary>
        private Visibility employeeCodeVisibility;

        /// <summary>
        ///     The back up value for <see cref="RescanVisibility" /> property.
        /// </summary>
        private Visibility rescanVisibility;

        /// <summary>
        ///     The back up value for <see cref="Logo" /> property.
        /// </summary>
        private ImageSource logo;

        #endregion

        #region [ Public Properties ]

        /// <summary>
        ///     Gets or sets the Message.
        /// </summary>
        public string Message
        {
            get => message;

            set
            {
                if (message != value)
                {
                    message = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the EmployeeCode.
        /// </summary>
        public string EmployeeCode
        {
            get => employeeCode;

            set
            {
                if (employeeCode != value)
                {
                    employeeCode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the ControllVisibility.
        /// </summary>
        public Visibility ControlVisibility
        {
            get => cotnrolVisibility;

            set
            {
                if (cotnrolVisibility != value)
                {
                    cotnrolVisibility = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the EmployeeCodeVisibility.
        /// </summary>
        public Visibility EmployeeCodeVisibility
        {
            get => employeeCodeVisibility;

            set
            {
                if (employeeCodeVisibility != value)
                {
                    employeeCodeVisibility = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the RescanVisibility.
        /// </summary>
        public Visibility RescanVisibility
        {
            get => rescanVisibility;

            set
            {
                if (rescanVisibility != value)
                {
                    rescanVisibility = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the CurrentEmployee.
        /// </summary>
        public Domain.Models.Employee.Employee CurrentEmployee
        {
            get => currentEmployee;

            set
            {
                if (currentEmployee != value)
                {
                    currentEmployee = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string CurrentEmployeeStr
        {
            get
            {
                if (CurrentEmployee != null) return CurrentEmployee.Code + "  " + CurrentEmployee.Name;

                return "";
            }
        }

        public string EmployeeText
        {
            get
            {
                if (currentEmployee != null) return currentEmployee.Code + " / " + currentEmployee.Name;
                return "";
            }
        }

        /// <summary>
        ///     Gets the breakIn command.
        /// </summary>
        public ICommand BreakInCommand
        {
            get
            {
                if (breakInCommand == null) breakInCommand = new DelegateCommand(ExecuteBreakInCommand);

                return breakInCommand;
            }
        }

        /// <summary>
        ///     Gets the breakOut command.
        /// </summary>
        public ICommand BreakOutCommand
        {
            get
            {
                if (breakOutCommand == null) breakOutCommand = new DelegateCommand(ExecuteBreakOutCommand);

                return breakOutCommand;
            }
        }

        /// <summary>
        ///     Gets the timeIn command.
        /// </summary>
        public ICommand TimeInCommand
        {
            get
            {
                if (timeInCommand == null) timeInCommand = new DelegateCommand(ExecuteTimeInCommand);

                return timeInCommand;
            }
        }

        /// <summary>
        ///     Gets the timeOut command.
        /// </summary>
        public ICommand TimeOutCommand
        {
            get
            {
                if (timeOutCommand == null) timeOutCommand = new DelegateCommand(ExecuteTimeOutCommand);

                return timeOutCommand;
            }
        }

        /// <summary>
        ///     Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null) cancelCommand = new DelegateCommand(ExecuteCancelCommand);

                return cancelCommand;
            }
        }

        /// <summary>
        ///     Gets the enterCode command.
        /// </summary>
        public ICommand EnterCodeCommand
        {
            get
            {
                if (enterCodeCommand == null) enterCodeCommand = new DelegateCommand(ExecuteEnterCodeCommand);

                return enterCodeCommand;
            }
        }

        public ICommand PullEmployeeCommand
        {
            get
            {
                if (pullEmployeeCommand == null) pullEmployeeCommand = new DelegateCommand(ExecutePullEmployeeCommand);

                return pullEmployeeCommand;
            }
        }


        /// <summary>
        ///     Gets the rescan command.
        /// </summary>
        public ICommand RescanCommand
        {
            get
            {
                if (rescanCommand == null) rescanCommand = new DelegateCommand(ExecuteRescanCommand);

                return rescanCommand;
            }
        }

        /// <summary>
        ///     Gets the exit command.
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null) exitCommand = new DelegateCommand(ExecuteExitCommand);

                return exitCommand;
            }
        }

        /// <summary>
        ///     Gets or sets the Logo.
        /// </summary>
        public ImageSource Logo
        {
            get => logo;

            set
            {
                if (logo != value)
                {
                    logo = value;
                    RaisePropertyChanged();
                }
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

        #region [ Private Methods ]

        /// <summary>
        ///     Waits for finger scan.
        /// </summary>
        private void WaitForFingerScan()
        {
            //#region [ Dummy Test Methods ]
            //// This code is used for simulate the Finger Print, just wait for 10 seconds.
            ////================================================
            //DispatcherTimer timer = new DispatcherTimer()
            //{
            //    Interval = TimeSpan.FromSeconds(3)
            //};

            //timer.Tick += ((s, e) =>
            //{
            //    this.ScanFinish("12345");
            //    timer.Stop();
            //});

            //timer.Start();
            ////================================================
            //#endregion

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

                                var employees = EmployeeService.GetAllEmployees().ToList();

                                //Perform indentification of fmd of captured sample against enrolledFmds for userid 
                                var iResult1 = Comparison.Identify(fmd, 0,
                                    employees.Select(c => new Fmd(c.IdenitityData, int.Parse(c.Identity.Split(':')[1]),
                                        c.Identity.Split(':')[0])), 21474, 1);

                                //If Identify was successfull
                                if (iResult1.ResultCode == Constants.ResultCode.DP_SUCCESS)
                                    //If number of matches were greater than 0
                                    if (iResult1.Indexes.Length > 0)
                                    {
                                        //for each matched fmd get its index and map that index to userid index
                                        var index = iResult1.Indexes[0][0];
                                        ScanFinish(employees[index]);
                                        return;
                                    }
                            }

                            // Cannot found employee. Ask for Code then download from server.
                            AskForEmployeeCode();
                        }
                        catch (Exception ex)
                        {
                            UpdateMessage(ex.Message);
                        }
                    });

                else
                    AskUserPassword();
            }
            catch (Exception ex)
            {
                UpdateMessage(ex.Message);
            }
        }

        private void AskUserPassword()
        {
            var userPin = DialogService.AskUserPin();
            if (userPin != null)
            {
                var myUser = UserService.GetUserByPinCode(userPin);
                if (myUser != null && myUser.Id > 0)
                {
                    var myEmployee = EmployeeService.GetEmployeeByUserId(myUser.Id);
                    if (myEmployee != null)
                        ScanFinish(myEmployee);
                    else
                        UpdateMessage(LoOv.G(o => Resources.NotFoundEmployee));
                }
                else
                {
                    UpdateMessage(LoOv.G(o => Resources.NotFoundEmployee));
                }
            }

            RescanVisibility = Visibility.Visible;
        }

        /// <summary>
        ///     Asks for employee code.
        /// </summary>
        private void AskForEmployeeCode()
        {
            UpdateMessage(LoOv.G(o => Resources.NotFoundEmployee));
            EmployeeCodeVisibility = Visibility.Visible;
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
        ///     Executes the <see cref="BreakInCommand"> command.
        /// </summary>
        private void ExecuteBreakInCommand()
        {
            if (CurrentEmployee != null) EmployeeService.BreakIn(CurrentEmployee);

            ShowDoneMessage(LoOv.G(o => Resources.BreakInMessage));
        }

        /// <summary>
        ///     Executes the <see cref="BreakOutCommand"> command.
        /// </summary>
        private void ExecuteBreakOutCommand()
        {
            if (CurrentEmployee != null) EmployeeService.BreakOut(CurrentEmployee);

            ShowDoneMessage(LoOv.G(o => Resources.BreakOutMessage));
        }

        /// <summary>
        ///     Executes the <see cref="TimeInCommand"> command.
        /// </summary>
        private void ExecuteTimeInCommand()
        {
            if (CurrentEmployee != null) EmployeeService.TimeIn(CurrentEmployee);

            ShowDoneMessage(LoOv.G(o => Resources.TimeInMessage));
        }

        /// <summary>
        ///     Executes the <see cref="TimeOutCommand"> command.
        /// </summary>
        private void ExecuteTimeOutCommand()
        {
            if (CurrentEmployee != null) EmployeeService.TimeOut(CurrentEmployee);

            ShowDoneMessage(LoOv.G(o => Resources.TimeOutMessage));
        }

        /// <summary>
        ///     Executes the <see cref="CancelCommand"> command.
        /// </summary>
        private void ExecuteCancelCommand()
        {
            Close();
        }

        /// <summary>
        ///     Executes the <see cref="EnterCodeCommand"> command.
        /// </summary>
        private void ExecuteEnterCodeCommand()
        {
            UpdateMessage(LoOv.G(o => Resources.DownloadEmployeeMessage));
            var result = SyncService.PullEmployee(EmployeeCode);

            if (result.Error)
            {
                UpdateMessage(result.ErrorDescription);
                return;
            }

            WaitForFingerScan();
            EmployeeCode = string.Empty;
            EmployeeCodeVisibility = Visibility.Visible;
        }

        private void ExecutePullEmployeeCommand()
        {
            UpdateMessage(LoOv.G(o => Resources.DownloadEmployeeMessage));
            var result = SyncService.PullEmployee(CurrentEmployee.Code);

            if (result.Error) UpdateMessage(result.ErrorDescription);
        }

        /// <summary>
        ///     Executes the <see cref="RescanCommand"> command.
        /// </summary>
        private void ExecuteRescanCommand()
        {
            Loaded();
        }

        /// <summary>
        ///     Executes the <see cref="ExitCommand"> command.
        /// </summary>
        private void ExecuteExitCommand()
        {
            DeviceService.FinalizeDevices();
            NancyService.Stop();

            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Shows the done message.
        /// </summary>
        private void ShowDoneMessage(string message)
        {
            UpdateMessage(message);
            RescanVisibility = Visibility.Visible;
            ControlVisibility = Visibility.Hidden;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };

            timer.Tick += (s, e) =>
            {
                Close();
                timer.Stop();
            };

            timer.Start();
        }

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        private void Close()
        {
            EventServiceFactory.EventService.GetEvent<GenericEvent<EventAggregator>>()
                .Publish(new EventParameters<EventAggregator> { Topic = EventTopicNames.ShellInitialized });
        }

        /// <summary>
        ///     Finish the finger print scan.
        /// </summary>
        /// <param name="identity">The identity.</param>
        private void ScanFinish(Domain.Models.Employee.Employee employee)
        {
            UpdateMessage(LoOv.G(o => Resources.FingerScanFinishMessage));

            CurrentEmployee = employee;
            ControlVisibility = Visibility.Visible;
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

        #endregion
    }
}