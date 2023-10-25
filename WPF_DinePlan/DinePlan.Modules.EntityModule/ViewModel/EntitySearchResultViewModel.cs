using DinePlan.Domain.Models.Entities;
using DinePlan.Presentation.Common.ModelBase;

namespace DinePlan.Modules.EntityModule
{
    public class EntitySearchResultViewModel : GenericViewModelbase
    {
        private EntityCustomDataViewModel _accountCustomDataViewModel;

        public EntitySearchResultViewModel(Entity model, EntityType template)
        {
            EntityType = template;
            Model = model;
        }

        public Entity Model { get; set; }
        public EntityType EntityType { get; set; }

        public EntityCustomDataViewModel AccountCustomDataViewModel => _accountCustomDataViewModel ??
                                                                       (_accountCustomDataViewModel =
                                                                           new EntityCustomDataViewModel(Model,
                                                                               EntityType));

        public string this[string index] => AccountCustomDataViewModel.GetValue(index);

        public int Id => Model.Id;

        public string NameDisplay => EntityType.FormatEntityName(Model.Name);

        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
    }
}