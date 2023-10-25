using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using DinePlan.Common.Model.Sync;
using DinePlan.Domain.Models.Entities;
using DinePlan.Infrastructure.ExceptionReporter;
using DinePlan.Modules.UserModule.ViewModels.Dtos;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services;
using Newtonsoft.Json;
using Prism.Commands;

namespace DinePlan.Modules.UserModule.ViewModels
{
    [Export]
    public class CreateStudentViewModel : GenericViewModelbase
    {
        private DelegateCommand refreshCommand;

        [ImportingConstructor]
        public CreateStudentViewModel(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public DelegateCommand RefreshCommand =>
            refreshCommand ?? (refreshCommand = new DelegateCommand(RefreshExecute));

        private void ExecuteCloseCommand()
        {
            OperationRequest<Entity>.Publish(null, EventTopicNames.EntitySelected, EventTopicNames.EntitySelected, "");
        }

        public void GetData()
        {
            GetGrade();
        }

        public void RefreshExecute()
        {
            Name = "";
            Code = "";
            GradeSelected = null;
            PhoneNumber = "";
            Email = "";
            FatherMobile = "";
            FatherName = "";
            MotherMobile = "";
            MotherName = "";
        }

        private void OnSubmitCommand()
        {
            try
            {
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.ShowLoadingIndicator);
                var extraData = new ExtraDataDto
                {
                    FatherMobile = FatherMobile,
                    FatherName = FatherName,
                    MotherName = MotherName,
                    MotherMobile = MotherMobile,
                };
                var frontSignUp = new FrontSignupComplete
                {
                    Id = 0,
                    Name = Name,
                    Code = Code,
                    GradeId =  GradeSelected != null ? Convert.ToInt32(GradeSelected.Id) : (int?)null,
                    PhoneNumber = PhoneNumber,
                    Email = string.IsNullOrEmpty(Email)? GenerateEmail(Name) :Email,
                    TenantId = _settingService.ProgramSettings.SyncTenantId,
                    WhatsappNumber = PhoneNumber,
                    Password = "123qwe",        
                    ExtraData = JsonConvert.SerializeObject(extraData),
                };
                var input = JsonConvert.SerializeObject(frontSignUp);
                var response = TokenService.GetResponse(ConnectRequests.CREATE_ONLINECUSTOMER, input);
                bool resultSuccess = false;

                if (response != null)
                {
                    var myResult = JsonConvert.DeserializeObject<FrontSignupUserOutput>(response.ToString());
                    if (myResult != null && myResult.UserId > 0)
                    {
                        resultSuccess = true;
                    }
                }

                NotifyPopupService.Notify("Student",
                    resultSuccess ? "Student Creation Successful and Code is : " + frontSignUp.Code : "Student Creation failed", true,
                    new TimeSpan(0, 0, 5));
                if (resultSuccess)
                {
                    RefreshExecute();
                }
            }
            catch (Exception exception)
            {
                DinePlanLogger.Log(exception);
                NotifyPopupService.Notify("Student", exception.Message, true, new TimeSpan(0, 0, 5));
            }
            finally
            {
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.HideLoadingIndicator);
            }
        }

        private string GenerateEmail(string name)
        {
            return Guid.NewGuid().ToString("N") + "@test.com";
        }

        private void GetGrade()
        {
            var response = TokenService.GetResponse(ConnectRequests.GET_GRADELIST, null);
            if (response != null)
            {
                var output = JsonConvert.DeserializeObject<List<ComboboxItemObjectDto>>(response.ToString());
                GradeList = new ObservableCollection<ComboboxItemObjectDto>(output);
                RaisePropertyChanged(nameof(GradeList));
            }
        }

       
       
        #region [ Public Properties ]

        private string _name;

        public string Name
        {
            get => _name;

            set
            {
                if (value == string.Empty || _name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _code;

        public string Code
        {
            get => _code;

            set
            {
                if (value == string.Empty || _code != value)
                {
                    _code = value;
                    RaisePropertyChanged();
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;

            set
            {
                if (value == string.Empty || _phoneNumber != value)
                {
                    _phoneNumber = value;
                    RaisePropertyChanged();
                }
            }
        }


        private string _email;

        public string Email
        {
            get => _email;

            set
            {
                if (value == string.Empty || _email != value)
                {
                    _email = value;
                    RaisePropertyChanged();
                }
            }
        }


        private string _fatherName;

        public string FatherName
        {
            get => _fatherName;

            set
            {
                if (value == string.Empty || _fatherName != value)
                {
                    _fatherName = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _fatherMobile;

        public string FatherMobile
        {
            get => _fatherMobile;

            set
            {
                if (value == string.Empty || _fatherMobile != value)
                {
                    _fatherMobile = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _motherName;

        public string MotherName
        {
            get => _motherName;

            set
            {
                if (value == string.Empty || _motherName != value)
                {
                    _motherName = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _motherMobile;

        public string MotherMobile
        {
            get => _motherMobile;

            set
            {
                if (value == string.Empty || _motherMobile != value)
                {
                    _motherMobile = value;
                    RaisePropertyChanged();
                }
            }
        }


        private ComboboxItemObjectDto _gradeSelected;

        public ComboboxItemObjectDto GradeSelected
        {
            get => _gradeSelected;

            set
            {
                if (value == null || _gradeSelected != value)
                {
                    _gradeSelected = value;
                    RaisePropertyChanged();
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }

      

        private ComboboxItemObjectDto _parentSelected;

        public ObservableCollection<ComboboxItemObjectDto> GradeList { get; set; } =
            new ObservableCollection<ComboboxItemObjectDto>();


        private DelegateCommand submitCommand;

        public DelegateCommand SubmitCommand
        {
            get
            {
                if (submitCommand == null)
                    submitCommand = new DelegateCommand(OnSubmitCommand, () => !string.IsNullOrWhiteSpace(Name) &&
                                                                               !string.IsNullOrWhiteSpace(Code) &&
                                                                               GradeSelected != null &&
                                                                               !string.IsNullOrWhiteSpace(GradeSelected.Id));

                return submitCommand;
            }
        }

        private DelegateCommand _closeCommand;

        public DelegateCommand CloseCommand
        {
            get
            {
                if (_closeCommand == null) _closeCommand = new DelegateCommand(ExecuteCloseCommand);

                return _closeCommand;
            }
        }

        #endregion
    }
}