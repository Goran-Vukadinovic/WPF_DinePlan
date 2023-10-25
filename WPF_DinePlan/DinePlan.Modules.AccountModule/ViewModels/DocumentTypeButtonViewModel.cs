using DinePlan.Domain.Models.Accounts;
using DinePlan.Presentation.Common;
using DinePlan.Presentation.Services.Common;
using Prism.Commands;

namespace DinePlan.Modules.AccountModule
{
    public class DocumentTypeButtonViewModel : ObservableObject
    {
        public DocumentTypeButtonViewModel(AccountTransactionDocumentType model, Account account)
        {
            Model = model;
            Account = account;
            SelectDocumentTypeCommand = new DelegateCommand<string>(OnSelectDocumentType);
        }

        public AccountTransactionDocumentType Model { get; set; }
        public Account Account { get; set; }
        public DelegateCommand<string> SelectDocumentTypeCommand { get; set; }

        public string ButtonHeader => Model.ButtonHeader.Replace(" ", "\r");
        public string ButtonColor => Model.ButtonColor;

        private void OnSelectDocumentType(string obj)
        {
            if (Account != null)
            {
                var creationData = new DocumentCreationData(Account, Model);
                creationData.PublishEvent(EventTopicNames.AccountTransactionDocumentSelected);
            }
            else
            {
                Model.PublishEvent(EventTopicNames.BatchCreateDocument);
            }
        }
    }
}