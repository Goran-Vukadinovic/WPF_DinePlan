using DinePlan.Common.Model.Sync;
using DinePlan.Common.Model.User;
using DinePlan.Infrastructure.ExceptionReporter;
using DinePlan.Modules.UserModule.ViewModels.Dtos;
using DinePlan.Modules.UserModule.Views;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.UserModule.ViewModels
{
    [Export]
    public class EditMemberViewModel : GenericViewModelbase
    {
        #region [ Constructors ]

        [ImportingConstructor]
        public EditMemberViewModel()
        {
        }
        #endregion

        #region [ Fields ]
        public ObservableCollection<RoleDto> Roles { get; set; } = new ObservableCollection<RoleDto>();
        private DelegateCommand submitCommand;

        #endregion

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
                    RaisePropertyChanged(nameof(Name));
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private string _mobile;
        public string Mobile
        {
            get => _mobile;

            set
            {
                if (value == string.Empty || _mobile != value)
                {
                    _mobile = value;
                    RaisePropertyChanged(nameof(Mobile));
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private string _lastName;
        public string LastName
        {
            get => _lastName;

            set
            {
                if (value == string.Empty || _lastName != value)
                {
                    _lastName = value;
                    RaisePropertyChanged(nameof(LastName));
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private DateTime? _birthDay;
        [Browsable(false)]
        public DateTime? BirthDay
        {
            get => _birthDay;
            set
            {
                _birthDay = value;
                RaisePropertyChanged(nameof(BirthDay));
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private string _memberCode;
        public string MemberCode
        {
            get => _memberCode;

            set
            {
                if (value == string.Empty || _memberCode != value)
                {
                    _memberCode = value;
                    RaisePropertyChanged(nameof(MemberCode));
                    SubmitCommand.RaiseCanExecuteChanged();
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
                    RaisePropertyChanged(nameof(Email));
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private ConnectMemberDto _selectedMember;
        public ConnectMemberDto SelectedMember
        {
            get => _selectedMember;

            set
            {
                _selectedMember = value;
                Name = SelectedMember.Name;
                LastName = SelectedMember.LastName;
                MemberCode = SelectedMember.MemberCode;
                Mobile = SelectedMember.PhoneNumber;
                Email = SelectedMember.EmailId;

                if (SelectedMember.BirthDay.HasValue)
                {
                    BirthDay = SelectedMember.BirthDay.Value;
                }
                else
                {
                    BirthDay = DateTime.Now.AddYears(-18);
                }
                if (SelectedMember.MembershipTierId.HasValue)
                {
                    MembershipTier = new MembershipTierDto { Id = SelectedMember.MembershipTierId.Value };
                }
                else
                {
                    MembershipTier = new MembershipTierDto();
                }

                RaisePropertyChanged(nameof(SelectedMember));
                RaisePropertyChanged(nameof(Name));
                RaisePropertyChanged(nameof(LastName));
                RaisePropertyChanged(nameof(MemberCode));
                RaisePropertyChanged(nameof(BirthDay));
                RaisePropertyChanged(nameof(MembershipTier));
                RaisePropertyChanged(nameof(Mobile));

            }
        }
        private MembershipTierDto _membershipTier;
        public MembershipTierDto MembershipTier
        {
            get => _membershipTier;

            set
            {
                if (value == null || _membershipTier != value)
                {
                    _membershipTier = value;
                    RaisePropertyChanged(nameof(MembershipTier));
                    SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public DelegateCommand SubmitCommand
        {
            get
            {
                if (submitCommand == null)
                    submitCommand = new DelegateCommand(OnSubmitCommand, () => !string.IsNullOrWhiteSpace(Name) &&
                                                                               !string.IsNullOrWhiteSpace(LastName) &&
                                                                               !string.IsNullOrWhiteSpace(Mobile) &&
                                                                               !string.IsNullOrWhiteSpace(Email));

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
        private MemberManagementView _memberManagementView;
        protected MemberManagementView MemberManagementView
        {
            get
            {
                if (_memberManagementView == null)
                    _memberManagementView = ServiceLocator.Current.GetInstance<MemberManagementView>();

                return _memberManagementView;
            }
        }
        private MemberManagementViewModel _memberManagementViewModel;
        public MemberManagementViewModel MemberManagementViewModel
        {
            get
            {
                if (_memberManagementViewModel == null)
                    _memberManagementViewModel = ServiceLocator.Current.GetInstance<MemberManagementViewModel>();

                return _memberManagementViewModel;
            }
        }
        private void ExecuteCloseCommand()
        {          
            RegionManager.ActivateRegion(RegionNames.MainRegion, MemberManagementView);
        }
        #endregion

        #region [ Private Methods ]
        private void OnSubmitCommand()
        {
            try
            {
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.ShowLoadingIndicator);
                SelectedMember.Name = Name;
                SelectedMember.MemberCode = MemberCode;
                SelectedMember.BirthDay = BirthDay;
                SelectedMember.PhoneNumber = Mobile;
                SelectedMember.LastName = LastName;
                SelectedMember.EmailId = Email;
                var input = new CreateMemberInput
                {
                    Member = SelectedMember,
                    TenantId = SettingService.ProgramSettings.SyncTenantId,
                };
                var response = TokenService.GetResponse(ConnectRequests.CREATEORUPDATEMEMBERENGAGE, input);
                if (response != null)
                {
                    var user = JsonConvert.DeserializeObject<MessageOutputDto>(response.ToString());
                    if (user != null && user.Id > 0)
                    {
                        NotifyPopupService.Notify("Member", "Update Member Successful", true, new TimeSpan(0, 0, 5));
                        ExecuteCloseCommand();

                        var mb = MemberManagementViewModel.Members.FirstOrDefault(x => x.Id == user.Id);
                        if (mb != null)
                        {
                            mb.Name = Name;
                            mb.MemberCode = MemberCode;
                            mb.BirthDay = BirthDay;
                            mb.PhoneNumber = Mobile;
                            mb.LastName = LastName;
                            mb.EmailId = Email;

                            mb.MembershipTierId = MembershipTier.Id;

                            MemberManagementViewModel.SelectedMember = mb;
                            
                        }
                        MemberManagementViewModel.KeepData = true;
                    }
                    else if (user != null && user.Id == -1 && user.Exists)
                    {
                        NotifyPopupService.Notify("Member", user.ErrorMessage, true, new TimeSpan(0, 0, 5));

                    }
                    else
                        NotifyPopupService.Notify("Member", "Member Updation failed", true, new TimeSpan(0, 0, 5));
                }
                else
                    NotifyPopupService.Notify("Member", "Member Updation failed", true, new TimeSpan(0, 0, 5));

            }
            catch (Exception exception)
            {
                DinePlanLogger.Log(exception);
                NotifyPopupService.Notify("Member", exception.Message, true, new TimeSpan(0, 0, 5));
                throw;
            }
            finally
            {
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.HideLoadingIndicator);
            }
          
        }
       
        #endregion
    }
}