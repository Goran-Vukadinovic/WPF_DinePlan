using DinePlan.Domain.Models.Entities;
using DinePlan.Domain.Models.Settings;
using DinePlan.Domain.Models.Tickets;
using DinePlan.Infrastructure.Data;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Common.Services;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace DinePlan.Modules.EntityModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EntityScreenViewModel : EntityViewModelBaseWithMap<EntityScreen, EntityScreenMap,
        AbstractMapViewModel<EntityScreenMap>>
    {
        private IEnumerable<EntityScreenItem> _entityScreenItems;

        private IEnumerable<State> _entityStates;

        private IEnumerable<EntityType> _entityTypes;

        private IEnumerable<TicketType> _ticketTypes;

        [ImportingConstructor]
        public EntityScreenViewModel()
        {
            SelectScreenItemsCommand = new CaptionCommand<string>(
                string.Format(LoOv.G(o => Resources.Select_f), LoOv.G(o => Resources.Entity)), OnSelectScreenItems,
                CanSelectScreenItems);
        }

        public ICaptionCommand SelectScreenItemsCommand { get; set; }

        public string[] DisplayModes
        {
            get
            {
                return new[]
                    {LoOv.G(o => Resources.Automatic), LoOv.G(o => Resources.Search), LoOv.G(o => Resources.Custom)};
            }
        }

        public string DisplayMode
        {
            get => DisplayModes[Model.DisplayMode];
            set => Model.DisplayMode = Array.IndexOf(DisplayModes, value);
        }

        public string BackgroundImage
        {
            get => string.IsNullOrEmpty(Model.BackgroundImage) ? "/Images/empty.png" : Model.BackgroundImage;
            set => Model.BackgroundImage = value;
        }

        public string BackgroundColor
        {
            get => string.IsNullOrEmpty(Model.BackgroundColor) ? "Transparent" : Model.BackgroundColor;
            set => Model.BackgroundColor = value;
        }

        public int FontSize
        {
            get => Model.FontSize;
            set => Model.FontSize = value;
        }

        public int PageCount
        {
            get => Model.PageCount;
            set => Model.PageCount = value;
        }

        public int ColumnCount
        {
            get => Model.ColumnCount;
            set => Model.ColumnCount = value;
        }

        public int RowCount
        {
            get => Model.RowCount;
            set => Model.RowCount = value;
        }

        public int ButtonHeight
        {
            get => Model.ButtonHeight;
            set => Model.ButtonHeight = value;
        }

        public int FlyButtonHeight
        {
            get => Model.FlyButtonHeight;
            set => Model.FlyButtonHeight = value;
        }

        public int TicketTypeId
        {
            get => Model.TicketTypeId;
            set => Model.TicketTypeId = value;
        }

        public int? EntityTypeId
        {
            get => Model.EntityTypeId;
            set => Model.EntityTypeId = value.GetValueOrDefault(0);
        }

        public string StateFilter
        {
            get => Model.StateFilter;
            set => Model.StateFilter = value;
        }

        public string DisplayState
        {
            get => Model.DisplayState;
            set => Model.DisplayState = value;
        }

        public bool AskTicketType
        {
            get => Model.AskTicketType;
            set => Model.AskTicketType = value;
        }

        public bool RefreshEntity
        {
            get => Model.RefreshEntity;
            set => Model.RefreshEntity = value;
        }

        public string SearchValueReplacePattern
        {
            get => Model.SearchValueReplacePattern;
            set => Model.SearchValueReplacePattern = value;
        }

        public bool UseStateDisplayFormat
        {
            get => Model.UseStateDisplayFormat;
            set => Model.UseStateDisplayFormat = value;
        }

        public IEnumerable<TicketType> TicketTypes => _ticketTypes ?? (_ticketTypes = Workspace.All<TicketType>());

        public IEnumerable<EntityScreenItem> EntityScreenItems => _entityScreenItems ??
                                                                  (_entityScreenItems =
                                                                      new List<EntityScreenItem>(Model.ScreenItems));

        public IEnumerable<EntityType> EntityTypes => _entityTypes ?? (_entityTypes = Workspace.All<EntityType>());

        public IEnumerable<State> EntityStates => _entityStates ?? (_entityStates = Workspace.All<State>());

        private bool CanSelectScreenItems(string arg)
        {
            return Model.EntityTypeId > 0;
        }

        private void OnSelectScreenItems(string obj)
        {
            var entityType = EntityDao.GetEntityTypeById(EntityTypeId.GetValueOrDefault(0));
            var items = Model.ScreenItems.ToList();

            IList<IOrderable> values = new List<IOrderable>(Workspace
                .All<Entity>(x => x.EntityTypeId == EntityTypeId)
                .Where(x => items.FirstOrDefault(y => y.EntityId == x.Id) == null)
                .OrderBy(x => x.Name)
                .Select(x => new EntityScreenItem(entityType, x)));

            IList<IOrderable> selectedValues = new List<IOrderable>(items);
            var choosenValues =
                InteractionService.UserIntraction.ChooseValuesFrom(values, selectedValues,
                    string.Format(LoOv.G(o => Resources.List_f), LoOv.G(o => Resources.Entity)),
                    string.Format(LoOv.G(o => Resources.SelectItemsFor_f), LoOv.G(o => Resources.Entities), Model.Name,
                        LoOv.G(o => Resources.EntityScreen)), LoOv.G(o => Resources.Entity),
                    LoOv.G(o => Resources.Entities));

            Model.ScreenItems.Clear();
            foreach (EntityScreenItem choosenValue in choosenValues) Model.AddScreenItem(choosenValue);
            _entityScreenItems = null;
            RaisePropertyChanged(nameof(EntityScreenItems));
        }

        public override Type GetViewType()
        {
            return typeof(Views.EntityScreenView);
        }

        public override string GetModelTypeString()
        {
            return LoOv.G(o => Resources.EntityScreen);
        }

        protected override AbstractValidator<EntityScreen> GetValidator()
        {
            return new EntityScreenValidator();
        }

        protected override void Initialize()
        {
            base.Initialize();
            MapController =
                new MapController<EntityScreenMap, AbstractMapViewModel<EntityScreenMap>>(Model.EntityScreenMaps,
                    Workspace);
        }

        protected override void OnSave(string value)
        {
            if (Model.DisplayMode < 2)
                Model.Layout = "";
            base.OnSave(value);
        }
    }

    internal class EntityScreenValidator : EntityValidator<EntityScreen>
    {
        public EntityScreenValidator()
        {
            RuleFor(x => x.TicketTypeId).GreaterThan(0);
            RuleFor(x => x.EntityTypeId).GreaterThan(0).When(x => x.DisplayMode < 2);
        }
    }
}