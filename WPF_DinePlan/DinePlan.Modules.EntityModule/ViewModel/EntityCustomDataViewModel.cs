using DinePlan.Domain.Models.Entities;
using DinePlan.Infrastructure.Helpers;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.ModelBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace DinePlan.Modules.EntityModule
{
    public class CustomDataValueViewModel : GenericViewModelbase, INameValue<string>
    {
        private DateTime? _date;

        public CustomDataValueViewModel(CustomDataValue model, Func<EntityCustomField, string, string, bool> action)
        {
            Model = model;
            SetValueAction = action;
        }

        public CustomDataValue Model { get; set; }

        public DateTime? Date
        {
            get => _date;
            set
            {
                if (value != _date)
                {
                    _date = value;
                    RaisePropertyChanged(nameof(Date));
                }
            }
        }

        public EntityCustomField CustomField
        {
            get => Model.CustomField;
            set
            {
                Model.CustomField = value;
                RaisePropertyChanged(nameof(CustomField));
            }
        }

        public Func<EntityCustomField, string, string, bool> SetValueAction { get; set; }
        public bool Mandatory { get; set; }
        public Visibility Visibility { get; set; }
        public string DisplayName => Mandatory ? $"{Name} (*)" : Name;

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        public string Value
        {
            get => Model.Value;
            set
            {
                if (value != Model.Value) UpdateValue(value);
            }
        }

        public void UpdateValue(string value)
        {
            var actionResult = SetValueAction(CustomField, Model.Value, value);
            if (!actionResult)
                Model.Value = value;
            RaisePropertyChanged(nameof(Value));
        }

        public void SetValue(string value)
        {
            Model.Value = value;
            RaisePropertyChanged(nameof(Value));
        }
    }

    public class EntityCustomDataViewModel : GenericViewModelbase
    {
        private ObservableCollection<CustomDataValueViewModel> _customData;

        public EntityCustomDataViewModel(Entity model, EntityType template)
        {
            EntityType = template;
            Model = model;
        }

        public Entity Model { get; set; }
        public EntityType EntityType { get; set; }
        public string PrimaryFieldName => GetPrimaryFieldName();
        public string PrimaryFieldFormat => EntityType != null ? EntityType.PrimaryFieldFormat : "";

        public bool IsTextBoxVisible => EntityType != null && string.IsNullOrWhiteSpace(EntityType.PrimaryFieldFormat);
        public bool IsMaskedTextBoxVisible => !IsTextBoxVisible;

        public ObservableCollection<CustomDataValueViewModel> CustomData
        {
            get
            {
                if (_customData == null) _customData = GetCustomData(Model.CustomData);
                return _customData;
            }
        }

        public EntityScreen SelectedEntityScreen => ApplicationState.SelectedEntityScreen;

        private ObservableCollection<CustomDataValueViewModel> GetCustomData(string customData)
        {
            var data = new ObservableCollection<CustomDataValueViewModel>();
            try
            {
                if (!string.IsNullOrWhiteSpace(customData))
                    data =
                        new ObservableCollection<CustomDataValueViewModel>(
                            JsonHelper.Deserialize<List<CustomDataValue>>(customData)
                                .Where(x => EntityType == null ||
                                            EntityType.EntityCustomFields.Any(y => y.Name == x.Name))
                                .Select(x => new CustomDataValueViewModel(x, CustomDataValueUpdating)));
            }
            finally
            {
                GenerateFields(data);
            }

            return data;
        }

        private string GetPrimaryFieldName()
        {
            if (EntityType == null) return "";
            return !string.IsNullOrEmpty(EntityType.PrimaryFieldName)
                ? EntityType.PrimaryFieldName
                : LoOv.G(o => Resources.Name);
        }

        public string GetValue(string name)
        {
            return CustomData.Any(x => x.Name == name)
                ? CustomData.Single(x => x.Name == name).Value
                : string.Empty;
        }

        private void GenerateFields(ICollection<CustomDataValueViewModel> data)
        {
            if (EntityType == null) return;

            //data.Where(x => EntityType.EntityCustomFields.All(y => y.Name != x.Name)).ForEach(x => data.Remove(x));

            foreach (var cf in EntityType.EntityCustomFields)
            {
                var customField = cf;
                var d = data.FirstOrDefault(x => x.Name == customField.Name);
                if (d == null)
                {
                    var customDataValue = new CustomDataValue { Name = cf.Name, CustomField = cf };
                    var cdvVM = new CustomDataValueViewModel(customDataValue, CustomDataValueUpdating);
                    cdvVM.Mandatory = cf.Mandatory;
                    cdvVM.Visibility = cf.Hidden ? Visibility.Collapsed : Visibility.Visible;
                    data.Add(cdvVM);
                }
                else
                {
                    d.CustomField = cf;
                    d.Mandatory = cf.Mandatory;
                    d.Visibility = cf.Hidden ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private bool CustomDataValueUpdating(EntityCustomField entityCustomField, string oldValue, string newValue)
        {
            var handled = false;
            if (entityCustomField.IsQuery && !string.IsNullOrEmpty(entityCustomField.EditingFormat) &&
                entityCustomField.EditingFormat.Contains('='))
            {
                var value = entityCustomField.Values.FirstOrDefault(x =>
                    x.Value.Contains(string.Format("\"{0}\"", newValue)));
                if (value != null)
                {
                    var valueParts = ParseCsv(value.Value);
                    var format = entityCustomField.EditingFormat;

                    for (var i = 0; i < valueParts.Count; i++)
                        format = format.Replace("$" + (i + 1), valueParts[i]);

                    var index = valueParts.Count;
                    while (valueParts.Contains("$") && index < 20)
                    {
                        format = format.Replace("$" + index, "");
                        index++;
                    }

                    format = format.Replace("\r", Environment.NewLine);
                    var formatParts = format.Split(';');

                    foreach (var fieldParts in formatParts.Where(x => x.Contains('='))
                        .Select(formatPart => formatPart.Split(new[] { '=' }, 2)))
                    {
                        var field = CustomData.FirstOrDefault(x => x.Name == fieldParts[0]);
                        if (field == null) continue;
                        field.SetValue(fieldParts[1]);
                        handled = true;
                    }
                }
            }

            return handled;
        }

        private static List<string> ParseCsv(string input)
        {
            var sv = new List<string>();
            var regexObj = new Regex(@"""[^""\r\n]*""|'[^'\r\n]*'|[^,\r\n]*");
            var matchResults = regexObj.Match(input);
            while (matchResults.Success)
            {
                var v = matchResults.Value.Trim('"', ' ');
                if (!string.IsNullOrEmpty(v))
                    sv.Add(v);
                matchResults = matchResults.NextMatch();
            }

            return sv;
        }

        public void Update()
        {
            if (_customData != null)
            {
                Model.CustomData = JsonHelper.Serialize(_customData.Select(x => x.Model).ToList());
                RaisePropertyChanged(nameof(IsMaskedTextBoxVisible));
                RaisePropertyChanged(nameof(IsTextBoxVisible));
                RaisePropertyChanged(nameof(PrimaryFieldName));
                RaisePropertyChanged(nameof(PrimaryFieldFormat));
            }
        }

        public void UpdateNewEntityQueryFields()
        {
            if (Model.Id == 0 && CustomData.Any())
            {
                CustomData.Where(x => x.CustomField.IsQuery).ToList().ForEach(x => x.UpdateValue(x.Model.Value));
                Update();
            }
        }

        public void SaveScreenLayout(string layoutString)
        {
            if (!string.IsNullOrEmpty(layoutString) && SelectedEntityScreen != null)
            {
                EntityService.SaveEntityScreenLayout(SelectedEntityScreen.Id, layoutString);
                SelectedEntityScreen.Layout = layoutString;
            }
        }
    }
}