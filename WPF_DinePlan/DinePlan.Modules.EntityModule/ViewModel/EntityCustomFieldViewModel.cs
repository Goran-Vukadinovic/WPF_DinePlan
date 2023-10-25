using DinePlan.Domain.Models.Entities;
using DinePlan.Presentation.Common.ModelBase;
using System.Linq;

namespace DinePlan.Modules.EntityModule
{
    public class EntityCustomFieldViewModel : GenericViewModelbase
    {
        private string[] _fieldTypes;

        public EntityCustomFieldViewModel(EntityCustomField model)
        {
            Model = model;
        }

        public EntityCustomField Model { get; set; }

        public string[] FieldTypes
        {
            get
            {
                if (_fieldTypes == null)
                    _fieldTypes = new[] { "String", "WideString", "Number", "Query", "Date", "ReadOnly" };
                return _fieldTypes;
            }
        }

        public string FieldType
        {
            get => FieldTypes[Model.FieldType];
            set => Model.FieldType = FieldTypes.ToList().IndexOf(value);
        }

        public string Name
        {
            get => Model.Name;
            set
            {
                if (value.Contains(",") || value.Any(x => char.IsWhiteSpace(x))) return;
                Model.Name = value;
            }
        }

        public bool Hidden
        {
            get => Model.Hidden;
            set => Model.Hidden = value;
        }

        public string EditingFormat
        {
            get => Model.EditingFormat;
            set => Model.EditingFormat = value;
        }

        public string ValueSource
        {
            get => Model.ValueSource;
            set => Model.ValueSource = value;
        }
    }
}