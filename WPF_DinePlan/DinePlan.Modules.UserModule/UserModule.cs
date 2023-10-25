using DinePlan.Domain.Models.Users;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Modules.UserModule.Views;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Prism.Mef.Modularity;
using Prism.Modularity;
using System.ComponentModel.Composition;

namespace DinePlan.Modules.UserModule
{
    [ModuleExport(typeof(UserModule), InitializationMode = InitializationMode.OnDemand)]
    public class UserModule : ModuleBase
    {
        public MemberManagementView _memberManagementView;
        private CreateStudentView _createStudentView;

        [ImportingConstructor]
        public UserModule()
        {
            _memberManagementView = ServiceLocator.Current.GetInstance<MemberManagementView>();
            AddDashboardCommand<EntityCollectionViewModelBase<UserRoleViewModel, UserRole>>(
                LoOv.K(o => Resources.UserRoleList),
                LoOv.K(o => Resources.Users), 50);
            AddDashboardCommand<EntityCollectionViewModelBase<UserViewModel, User>>(LoOv.K(o => Resources.UserList),
                LoOv.K(o => Resources.Users));
        }

        protected override void OnInitialization()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.RightUserRegion, typeof(LoggedInUserView));
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MemberManagementView));
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(EditMemberView));
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(CreateStudentView));
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(CreateMemberView));
            EventServiceFactory.EventService.GetEvent<GenericEvent<string>>().Subscribe(OnSelectSapMember);
        }

        private void OnSelectSapMember(EventParameters<string> obj)
        {
            if (ApplicationState.CurrentLoggedInUser.Id > 0 && obj.Topic == EventTopicNames.ConnectMember)
                RegionManager.ActivateRegion(RegionNames.MainRegion, _memberManagementView);
            if (ApplicationState.CurrentLoggedInUser.Id > 0 && obj.Topic == EventTopicNames.CreateStudent)
            {
                _createStudentView = ServiceLocator.Current.GetInstance<CreateStudentView>();
                RegionManager.ActivateRegion(RegionNames.MainRegion, _createStudentView);
            }
        }
    }
}