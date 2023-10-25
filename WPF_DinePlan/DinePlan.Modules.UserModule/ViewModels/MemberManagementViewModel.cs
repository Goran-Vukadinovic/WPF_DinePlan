using System;
using DinePlan.Common.Model.User;
using DinePlan.Common.POS;
using DinePlan.Domain.Models.Entities;
using DinePlan.Domain.Models.SapCrm;
using DinePlan.Modules.PosModule;
using DinePlan.Persistance.Data;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using DinePlan.Common.Model.Sync;
using DinePlan.Modules.UserModule.ViewModels.Dtos;
using DinePlan.Modules.UserModule.Views;
using DinePlan.Presentation.Common;

namespace DinePlan.Modules.UserModule.ViewModels
{
    [Export]
    public class MemberManagementViewModel : GenericViewModelbase
    {
        #region [ Constructors ]

        [ImportingConstructor]
        public MemberManagementViewModel()
        {
        }

        #endregion

        #region [ Public Properties ]

        public bool KeepData { get; set; }

        private ConnectMemberDto _selectedMember;
        public ConnectMemberDto SelectedMember
        {
            get => _selectedMember;

            set
            {
                _selectedMember = value;
                RaisePropertyChanged(nameof(SelectedMember));
                SelectCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ConnectMemberDto> Members { get; set; } = new ObservableCollection<ConnectMemberDto>();

        private string _filter;
        public string Filter
        {
            get => _filter;

            set
            {
                if (value == string.Empty || _filter != value)
                {
                    _filter = value;
                    RaisePropertyChanged(nameof(Filter));
                    SearchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private CreateMemberViewModel _createMemberViewModel;
        public CreateMemberViewModel CreateMemberViewModel
        {
            get
            {
                if (_createMemberViewModel == null)
                    _createMemberViewModel = ServiceLocator.Current.GetInstance<CreateMemberViewModel>();

                return _createMemberViewModel;
            }
        }
        private DelegateCommand searchCommand;
        public DelegateCommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                    searchCommand = new DelegateCommand(ExecuteSearchCommand, () => !string.IsNullOrWhiteSpace(Filter));

                return searchCommand;
            }
        }

        private DelegateCommand _createCommand;
        public DelegateCommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                    _createCommand = new DelegateCommand(ExecuteCreateCommand);

                return _createCommand;
            }
        }

        private void ExecuteCreateCommand()
        {
            RegionManager.ActivateRegion(RegionNames.MainRegion, CreateMemberView);
        }

        private DelegateCommand _editCommand;
        public DelegateCommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                    _editCommand = new DelegateCommand(ExecuteEditCommand,() => SelectedMember != null);

                return _editCommand;
            }
        }
        private EditMemberView editMemberView;
        protected EditMemberView EditMemberView
        {
            get
            {
                if (editMemberView == null)
                    editMemberView = ServiceLocator.Current.GetInstance<EditMemberView>();

                return editMemberView;
            }
        }
        private EditMemberViewModel _editMemberViewModel;
        public EditMemberViewModel EditMemberViewModel
        {
            get
            {
                if (_editMemberViewModel == null)
                    _editMemberViewModel = ServiceLocator.Current.GetInstance<EditMemberViewModel>();

                return _editMemberViewModel;
            }
        }

        private CreateMemberView _createMemberView;
        protected CreateMemberView CreateMemberView
        {
            get
            {
                if (_createMemberView == null)
                    _createMemberView = ServiceLocator.Current.GetInstance<CreateMemberView>();

                return _createMemberView;
            }
        }
        private CreateMemberViewModel _createMemberViewModel2;
        public CreateMemberViewModel CreateMemberViewModel2
        {
            get
            {
                if (_createMemberViewModel2 == null)
                    _createMemberViewModel2 = ServiceLocator.Current.GetInstance<CreateMemberViewModel>();

                return _createMemberViewModel2;
            }
        }

        private void ExecuteEditCommand()
        {
            EditMemberViewModel.SelectedMember = SelectedMember;
            RegionManager.ActivateRegion(RegionNames.MainRegion, EditMemberView);
            ExecuteSearchCommand();
        }
        private DelegateCommand _selectCommand;
        public DelegateCommand SelectCommand
        {
            get
            {
                if (_selectCommand == null)
                    _selectCommand = new DelegateCommand(ExecuteSelectCommand, () => SelectedMember != null);

                return _selectCommand;
            }
        }
        private DelegateCommand _deleteCommand;
        public DelegateCommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                    _deleteCommand = new DelegateCommand(ExecuteDeleteCommand, () => SelectedMember != null);

                return _deleteCommand;
            }
        }

        private void ExecuteDeleteCommand()
        {
            var input = new ApiDeleteUserInput
            {
                Id = SelectedMember.Id.Value,
                TenantId = SettingService.ProgramSettings.SyncTenantId
            };

            var response = TokenService.GetResponse(ConnectRequests.DELETEMEMBERENGAGE, input);
            if (response == null)
            {
                DialogService.ShowFeedback("Delete member successful!");
                if(!string.IsNullOrWhiteSpace(Filter))
                {
                    ExecuteSearchCommand();
                }                
            }

            else
                DialogService.ShowFeedback("Delete member failed!");
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

        #region [ Private Methods ]

        public void ExecuteSearchCommand()
        {
            try
            {
                KeepData = false;
                if (string.IsNullOrEmpty(Filter))
                    return;
                EventServiceFactory.EventService.PublishEvent(EventTopicNames.ShowLoadingIndicator);
                ApplicationExtensions.DoEvents();
                var input = new SearchUserInput
                {
                    Filter = Filter,
                    TenantId = SettingService.ProgramSettings.SyncTenantId
                };
                var response = TokenService.GetResponse(ConnectRequests.SEARCHMEMBER, input);
                if (response != null)
                {
                    var output = JsonConvert.DeserializeObject<ConnectMemberListDto>(response.ToString());
                    Members = new ObservableCollection<ConnectMemberDto>(output.Items.ToArray());
                    RaisePropertyChanged(nameof(Members));
                    EventServiceFactory.EventService.PublishEvent(EventTopicNames.HideLoadingIndicator);
                    ApplicationExtensions.DoEvents();
                    if (Members.Count == 0)
                    {
                        NotifyPopupService.Notify("SearchMember", "Cannot find member!", true, new TimeSpan(0, 0, 5));
                    }    
                }
                else
                {
                    EventServiceFactory.EventService.PublishEvent(EventTopicNames.HideLoadingIndicator);
                    ApplicationExtensions.DoEvents();
                    NotifyPopupService.Notify("SearchMember", "Cannot find member!", true, new TimeSpan(0, 0, 5));
                }    
            }
            catch (Exception e)
            {
                NotifyPopupService.Notify("SearchMember", e.Message, true, new TimeSpan(0, 0, 5));
            }
            
                
        }

        private void ExecuteSelectCommand()
        {
            KeepData = false;
            if (SelectedMember != null)
            {
                var id = $"{SelectedMember.Id}-{SelectedMember.Name}";
                using (var w = WorkspaceFactory.Create())
                {
                    var mb = w.Query<MemberInformation>(x => x.MemberShipId == id).LastOrDefault();

                    if (mb == null)
                    {
                        mb = new MemberInformation
                        {
                            MemberShipId = id,
                            Data = JsonConvert.SerializeObject(SelectedMember)
                        };
                        w.Add(mb);
                        w.CommitChanges();
                    }
                    else
                    {
                        mb.Data = JsonConvert.SerializeObject(SelectedMember);
                        w.Update(mb);
                        w.CommitChanges();
                    }
                }

                var vm = ServiceLocator.Current.GetInstance<TicketViewModel>();
                vm.SelectedTicket.SetTagValue(1, PosConsts.DineConnectMember, id);
                OperationRequest<Entity>.Publish(null, EventTopicNames.EntitySelected, EventTopicNames.EntitySelected, "");
                vm.RefreshSelectedTicketTitle();
            }
        }
        private void ExecuteCloseCommand()
        {
            KeepData = false;
            Reset();
            OperationRequest<Entity>.Publish(null, EventTopicNames.EntitySelected, EventTopicNames.EntitySelected, "");
        }

        public void Reset()
        {
            KeepData = false;
            Filter = null;
            Members.Clear();
        }
        public void CheckClearData()
        {
            if (KeepData) return;
            var vm = ServiceLocator.Current.GetInstance<TicketViewModel>();

            if (string.IsNullOrEmpty(vm.SelectedTicket.TicketTags))
            {
                Reset();
                return;
            }
            var tag = vm.SelectedTicket.GetTagValue(PosConsts.DineConnectMember);
            if (string.IsNullOrEmpty(tag))
            {
                Reset();
                return;
            }
        }
        #endregion
    }
}