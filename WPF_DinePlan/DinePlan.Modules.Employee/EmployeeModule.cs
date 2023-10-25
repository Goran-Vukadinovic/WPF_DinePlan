using DinePlan.Presentation.Common;
using DinePlan.Presentation.Services.Common;
using Microsoft.Practices.ServiceLocation;
using Prism.Mef.Modularity;
using System.ComponentModel.Composition;

namespace DinePlan.Modules.Employee
{
    [ModuleExport(typeof(EmployeeModule))]
    public class EmployeeModule : ModuleBase
    {
        #region [ Fields ]

        /// <summary>
        ///     The back up value for <see cref="EmployeeView" /> property.
        /// </summary>
        private EmployeeView employeeView;

        #endregion

        #region [ Constructors ]

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmployeeModule" /> class.
        /// </summary>
        /// <param name="this.RegionManager">The region manager.</param>
        /// <param name="appScreen">The application screen.</param>
        [ImportingConstructor]
        public EmployeeModule()
        {
        }

        #endregion

        #region [ Public Properties ]

        /// <summary>
        ///     Gets or sets the EmployeeView service.
        /// </summary>
        protected EmployeeView EmployeeView
        {
            get
            {
                if (employeeView == null) employeeView = ServiceLocator.Current.GetInstance<EmployeeView>();

                return employeeView;
            }
        }

        #endregion

        #region [ Protected Methods ]

        /// <summary>
        ///     Called when [initialization].
        /// </summary>
        protected override void OnInitialization()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(EmployeeView));
            EventServiceFactory.EventService.GetEvent<GenericEvent<Domain.Models.Employee.Employee>>()
                .Subscribe(ActivateEmployeeView);
            base.OnInitialization();
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        ///     Activates the employee view.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ActivateEmployeeView(EventParameters<Domain.Models.Employee.Employee> obj)
        {
            RegionManager.ActivateRegion(RegionNames.MainRegion, EmployeeView);
        }

        #endregion

        #region [ Public Methods ]

        #endregion
    }
}