using DinePlan.Domain.Models.Entities;
using DinePlan.Presentation.Common.ModelBase;

namespace DinePlan.Modules.EntityModule
{
    public class EntitySwitcherButtonViewModel : GenericViewModelbase
    {
        private readonly bool _displayActiveScreen;

        public EntitySwitcherButtonViewModel(EntityScreen model, bool displayActiveScreen)
        {
            Model = model;
            _displayActiveScreen = displayActiveScreen;
        }

        public EntityScreen Model { get; set; }

        public string Caption =>
            ApplicationState.IsLandscape ? Model.Name.ToUpper() : Model.Name.Replace(" ", "\r").ToUpper();

        public string ButtonColor => Model != ApplicationState.SelectedEntityScreen || !_displayActiveScreen
            ? "#F16767"
            : "Brown";

        public void Refresh()
        {
            RaisePropertyChanged(nameof(ButtonColor));
        }
    }
}