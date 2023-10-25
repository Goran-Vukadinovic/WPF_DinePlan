namespace DinePlan.Common.Model.Payment
{
    public class PaymentFrame
    {
        public string ResponseCode { get; set; }
        public string AuthorizationCode { get; set; }
        public string TypeOfCard { get; set; }
        public string AccountNumber { get; set; }
        public string Last4DigitsCard { get; set; }
        public string Description { get; set; }
        public string ApprovalCode { get; set; }
        public string TerminalId { get; set; }
        public string MerchantId { get; set; }
        public string CardIssuerName { get; set; }
        public string CardNo { get; set; }
        public string CardExpiry { get; set; }
        public string BatchNo { get; set; }
        public string ReferenceNo { get; set; }
        public string TransactionNo { get; set; }

        public override string ToString()
        {
            return  "ResponseCode: " + ResponseCode +
                    "; AuthorizationCode: " + AuthorizationCode +
                    "; TypeOfCard: " + TypeOfCard +
                    "; AccountNumber: " + AccountNumber +
                    "; Last4DigitsCard: " + Last4DigitsCard +
                    "; Description: " + Description +
                    "; ApprovalCode: " + ApprovalCode +
                    "; TerminalId: " + TerminalId +
                    "; MerchantId: " + MerchantId +
                    "; CardIssuerName: " + CardIssuerName +
                    "; CardNo: " + CardNo +
                    "; CardExpiry: " + CardExpiry +
                    "; BatchNo: " + BatchNo +
                    "; ReferenceNo: " + ReferenceNo +
                    "; TransactionNo: " + TransactionNo;
        }
    }
}