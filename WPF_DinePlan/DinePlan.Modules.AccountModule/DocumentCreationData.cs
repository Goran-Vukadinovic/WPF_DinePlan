using DinePlan.Domain.Models.Accounts;

namespace DinePlan.Modules.AccountModule
{
    internal class DocumentCreationData
    {
        public DocumentCreationData(Account account, AccountTransactionDocumentType documentType)
        {
            Account = account;
            DocumentType = documentType;
        }

        public Account Account { get; set; }
        public AccountTransactionDocumentType DocumentType { get; set; }
    }
}