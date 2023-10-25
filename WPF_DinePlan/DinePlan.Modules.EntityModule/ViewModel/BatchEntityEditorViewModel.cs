using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using DinePlan.Domain.Models.Entities;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Persistance.Data;
using DinePlan.Persistance.Specification;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;

namespace DinePlan.Modules.EntityModule.ViewModel
{
    [Export(typeof(BatchEntityEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class BatchEntityEditorViewModel : VisibleViewModelBase
    {
        private ObservableCollection<EntityListerRow> _entities;
        private IEnumerable<EntityType> _entityTypes;
        private string _searchValue;
        private EntityType _selectedEntityType;

        public BatchEntityEditorViewModel()
        {
            Commands = new List<ICaptionCommand>();
            SaveCommand = new CaptionCommand<string>(LoOv.G(o => Resources.SaveChanges), OnSave, CanSave);
            Commands.Add(SaveCommand);
        }

        public ICaptionCommand SaveCommand { get; set; }

        public IEnumerable<EntityType> EntityTypes
        {
            get
            {
                return _entityTypes ?? (_entityTypes = Dao.Query<EntityType>(a => a.Id > 0, x => x.EntityCustomFields));
            }
        }

        public EntityType SelectedEntityType
        {
            get => _selectedEntityType;
            set
            {
                _selectedEntityType = value;
                SearchValue = "";
                RefreshItems();
                RaisePropertyChanged(nameof(SelectedEntityType));
            }
        }

        public ObservableCollection<EntityListerRow> Entities
        {
            get => _entities;
            set
            {
                _entities = value;
                RaisePropertyChanged(nameof(Entities));
            }
        }

        public bool DisplayLimitWarning { get; set; }

        public string SearchValue
        {
            get => _searchValue;
            set
            {
                _searchValue = value;
                RaisePropertyChanged(nameof(SearchValue));
            }
        }

        public IList<ICaptionCommand> Commands { get; set; }

        private bool CanSave(string arg)
        {
            return Entities.Any(x => x.IsModified);
        }

        private void OnSave(string obj)
        {
            foreach (var entityRow in Entities.Where(x => x.IsModified))
            {
                var entity = entityRow.Model;
                if (Dao.Exists<Entity>(x => x.Name == entity.Name && x.Id != entity.Id)) continue;
                Dao.Save(entity);
            }

            RefreshItems();
        }

        public override void OnShown()
        {
            Entities = new ObservableCollection<EntityListerRow>();
            SelectedEntityType = EntityTypes.FirstOrDefault();
            base.OnShown();
        }

        protected override string GetHeaderInfo()
        {
            return LoOv.G(o => Resources.BatchEntityEditor);
        }

        public override Type GetViewType()
        {
            return typeof(BatchEntityEditorView);
        }

        public void RefreshItems()
        {
            Expression<Func<Entity, bool>> predictate = x => x.EntityTypeId == SelectedEntityType.Id;
            if (!string.IsNullOrWhiteSpace(SearchValue))
                predictate = predictate.And(x => x.Name.Contains(SearchValue));
            var entities = Dao.Query(predictate).Select(x => new EntityListerRow(x));
            Entities = new ObservableCollection<EntityListerRow>(entities);
        }
    }

    internal class EntityListerRow : ObservableObject
    {
        private bool _isModified;

        public EntityListerRow(Entity entity)
        {
            Model = entity;
        }

        public Entity Model { get; set; }

        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                IsModified = true;
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                RaisePropertyChanged(nameof(IsModified));
            }
        }

        public string this[string fieldName]
        {
            get => Model.GetCustomData(fieldName);
            set
            {
                Model.SetCustomData(fieldName, value);
                IsModified = true;
            }
        }
    }
}