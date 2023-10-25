using DinePlan.Common.Sync;
using DinePlan.Domain.Models.Users;
using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.ModelBase;
using DinePlan.Presentation.Services.Common;
using DinePlan.Services.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace DinePlan.Modules.UserModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserViewModel : EntityViewModelBase<User>
    {
        private bool _edited;
        private IEnumerable<CultureInfo> _supportedLanguages;

        public UserViewModel()
        {
            EventServiceFactory.EventService.GetEvent<GenericEvent<UserRole>>()
                .Subscribe(x => RaisePropertyChanged(nameof(Roles)));
        }

        public string PinCode
        {
            get
            {
                if (_edited) return Model.PinCode;
                return !string.IsNullOrEmpty(Model.PinCode) ? "********" : "";
            }
            set
            {
                if (Model.PinCode == null || !Model.PinCode.Contains("*") && !string.IsNullOrEmpty(value))
                {
                    _edited = true;
                    Model.PinCode = value;
                    ApplicationState.NotifyEvent(RuleEventNames.TrackUser,
                        new
                        {
                            EventName = DinePlanLogTypes.USERPASSWORDCHANGED,
                            Content = "Screen=" + GetHeaderInfo() + ";Model=" + Model.Id + ";Name=" + Model.Name
                        });
                    RaisePropertyChanged(nameof(PinCode));
                }
            }
        }

        public string Language
        {
            get => Model.Language;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Model.Language = "";
                }
                else if (LocalSettings.SupportedLanguages.Contains(value))
                {
                    Model.Language = value;
                }
                else
                {
                    var ci = CultureInfo.GetCultureInfo(value);
                    Model.Language = LocalSettings.SupportedLanguages.Contains(ci.TwoLetterISOLanguageName)
                        ? ci.TwoLetterISOLanguageName
                        : "";
                }

                if (!string.IsNullOrEmpty(Model.Language))
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Model.Language);
            }
        }

        public string AlternateLanguage
        {
            get => Model.AlternateLanguage;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Model.AlternateLanguage = "";
                }
                else if (LocalSettings.SupportedLanguages.Contains(value))
                {
                    Model.AlternateLanguage = value;
                }
                else
                {
                    var ci = CultureInfo.GetCultureInfo(value);
                    Model.AlternateLanguage = LocalSettings.SupportedLanguages.Contains(ci.TwoLetterISOLanguageName)
                        ? ci.TwoLetterISOLanguageName
                        : "";
                }

                if (!string.IsNullOrEmpty(Model.Language))
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Model.Language);
            }
        }

        public IEnumerable<CultureInfo> SupportedLanguages
        {
            get
            {
                return _supportedLanguages ?? (_supportedLanguages =
                    LocalSettings.SupportedLanguages.Select(CultureInfo.GetCultureInfo)
                        .ToList()
                        .OrderBy(x => x.NativeName));
            }
        }

        public string SecurityCode
        {
            get
            {
                if (_edited) return Model.SecurityCode;
                return !string.IsNullOrEmpty(Model.SecurityCode) ? "********" : "";
            }
            set
            {
                if (Model.SecurityCode == null || !Model.SecurityCode.Contains("*") && !string.IsNullOrEmpty(value))
                {
                    _edited = true;
                    Model.SecurityCode = value;
                    ApplicationState.NotifyEvent(RuleEventNames.TrackUser,
                        new
                        {
                            EventName = DinePlanLogTypes.SECURITYCODECHANGED,
                            Content = "Screen=" + GetHeaderInfo() + ";Model=" + Model.Id + ";Name=" + Model.Name
                        });
                    RaisePropertyChanged(nameof(SecurityCode));
                }
            }
        }

        public UserRole Role
        {
            get => Model.UserRole;
            set => Model.UserRole = value;
        }


        public Guid UGuid
        {
            get
            {
                if (Model.UGuid == null) return Guid.NewGuid();
                return Model.UGuid.Value;
            }
        }

        public IEnumerable<UserRole> Roles { get; private set; }

        protected override void OnSave(string value)
        {
            Model.UGuid = UGuid;
            base.OnSave(value);
        }

        public override Type GetViewType()
        {
            return typeof(UserView);
        }

        public override string GetModelTypeString()
        {
            return LoOv.G(o => Resources.User);
        }

        protected override void Initialize()
        {
            Roles = Workspace.All<UserRole>();
        }

        protected override AbstractValidator<User> GetValidator()
        {
            return new UserValidator();
        }
    }

    internal class UserValidator : EntityValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.PinCode).Length(4, 20);
            RuleFor(x => x.UserRole).NotNull();
            RuleFor(x => x.Language).NotNull();
            RuleFor(x => x.AlternateLanguage).NotEqual(x=>x.Language).When(x => !string.IsNullOrEmpty(x.AlternateLanguage));


        }
    }
}