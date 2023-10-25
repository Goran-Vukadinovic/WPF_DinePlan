using DinePlan.Infrastructure.Settings;
using DinePlan.Localization;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Common.Commands;
using DinePlan.Presentation.Common.ModelBase;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DinePlan.Modules.LoginModule.Dialog
{
    [Export]
    public class ChangeLanguageDialogViewModel : DialogViewModelBase<LanguageCommandButton>
    {
        [ImportingConstructor]
        public ChangeLanguageDialogViewModel()
        {
            CloseCommand = new CaptionCommand<string>(LoOv.G(o => Resources.Close), OnClose);
            ExecuteAutomationCommnand = new DelegateCommand<LanguageCommandButton>(OnExecuteAutomationCommand);
        }

        public Func<LanguageCommandButton, bool> CanExecuteCommand { get; set; }

        public ICaptionCommand CloseCommand { get; set; }

        public IEnumerable<LanguageCommandButton> LanguageCommands { get; set; }

        public DelegateCommand<LanguageCommandButton> ExecuteAutomationCommnand { get; set; }

        protected override Window OnCreateDialog()
        {
            return new ChangeLanguageDialogView(this);
        }

        protected override LanguageCommandButton OnShowDialog()
        {
            if (LanguageCommands != null && LanguageCommands.Any())
            {
                RaisePropertyChanged(nameof(LanguageCommands));

                if (LocalSettings.WindowScale > 0)
                {
                    var layoutTransform = Dialog.LayoutTransform as ScaleTransform;
                    if (layoutTransform != null)
                    {
                        layoutTransform.ScaleX = LocalSettings.WindowScale;
                        layoutTransform.ScaleY = LocalSettings.WindowScale;
                    }
                }

                Dialog.ShowDialog();
            }

            return DialogResult;
        }

        private void OnExecuteAutomationCommand(LanguageCommandButton obj)
        {
            DialogResult = obj;
            Dialog.Hide();
        }

        private void OnClose(string obj)
        {
            DialogResult = null;
            Dialog.Hide();
        }
    }

    public class LanguageCommandButton : ObservableObject
    {
        public string NativeName { get; set; }

        public string Name { get; set; }
    }
}