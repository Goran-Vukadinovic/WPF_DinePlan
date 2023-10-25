using DinePlan.Domain.Models.Entities;
using DinePlan.Infrastructure;
using DinePlan.Infrastructure.Settings;
using DinePlan.Presentation.Common.ModelBase;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace DinePlan.Modules.EntityModule
{
    public class EntityScreenItemViewModel : GenericViewModelbase
    {
        private readonly bool _isTicketSelected;

        private readonly EntityScreen _screen;
        private readonly bool _userPermittedToMerge;


        private string _buttonColor;

        private bool _isEnabled;
        private string _lastUser;

        private EntityScreenItem _model;
        private string _name;
        private string _total;

        public EntityScreenItemViewModel(EntityScreenItem model, EntityScreen screen,
            ICommand actionCommand, bool isTicketSelected, bool userPermittedToMerge)
        {
            Command = actionCommand;
            _screen = screen;
            _isTicketSelected = isTicketSelected;
            _userPermittedToMerge = userPermittedToMerge;
            Model = model;
        }

        public string EntityState => Model.EntityState;

        public EntityScreenItem Model
        {
            get => _model;
            set
            {
                _model = value;
                RefreshDisplayAsync();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public string FormattedName => CacheService.GetFormattedEntityNameByState(EntityState, Model.Name);

        public string Background => !string.IsNullOrEmpty(Total) ? "LightPink" : "White";

        public string Total
        {
            get => _total;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    RaisePropertyChanged(nameof(Total));
                }
            }
        }

        public string LastUser
        {
            get => _lastUser;
            set
            {
                if (_lastUser != value)
                {
                    _lastUser = value;
                    RaisePropertyChanged(nameof(LastUser));
                }
            }
        }

        public string ButtonColor
        {
            get => _buttonColor;
            set
            {
                if (_buttonColor != value)
                {
                    _buttonColor = value;
                    RaisePropertyChanged(nameof(ButtonColor));
                }
            }
        }

        public int FontSize => _screen.FontSize;

        public double ButtonHeight => _screen.ButtonHeight > 0 ? _screen.ButtonHeight : double.NaN;

        public ICommand Command { get; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        public int EntityId => Model.EntityId;

        private void UpdateName()
        {
            LocalSettings.UpdateThreadLanguage();
            Name = Model.Name;
            ButtonColor = EntityState != null ? CacheService.GetStateColor(EntityState, 0) : DinePlanColor.Boro;
        }

        private string Traverse(string executeFunctions)
        {
            if (executeFunctions != null && executeFunctions.ToLower().Contains("<br/>"))
                executeFunctions = executeFunctions.ToLower().Replace("<br/>", "\n");

            return executeFunctions;
        }

        public void UpdateButtonColor()
        {
            IsEnabled = true;
            ButtonColor = EntityState != null ? CacheService.GetStateColor(EntityState) : "Gainsboro";
        }

        private void RefreshDisplayAsync()
        {
            IsEnabled = true;
            if (!_screen.UseStateDisplayFormat)
            {
                Name = Model.Name;
                ButtonColor = EntityState != null ? CacheService.GetStateColor(EntityState, 0) : DinePlanColor.Boro;
                return;
            }

            if (string.IsNullOrEmpty(Name)) Name = Model.Name;
            if (string.IsNullOrEmpty(ButtonColor))
                ButtonColor = EntityState != null ? CacheService.GetStateColor(EntityState, 0) : DinePlanColor.Boro;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(UpdateName));
        }
    }
}