namespace DinePlan.Common.POS
{
    public class PosConsts
    {
        public const string WorkPeriodReport = "Work Day Report";
        public const string CompleteWorkPeriodReport = "Complete Work Day Report";
        public const string OrderStateReport = "Order State Report";
        public const string VoidReport = "Void Report";
        public const string TicketTagSalesReport = "Ticket Tag Sales Report";
        public const string ExchangeReport = "Exchange Report";
        public const string ItemDiscountReport = "Items Discount Report";
        public const string PaymentSummaryByTerminalReport = "Payment Summary By Terminal Report";
        public const string CompReport = "Comp Report";
        public const string RefundReport = "Refund Report";
        public const string GiftReport = "Gift Report";
        public const string TicketsTaxReport = "Tickets Tax Report";
        public const string MemberReport = "Member Report";
        public const string ConsolidatedSummaryReport = "Consolidated Summary Report";
        public const string MemberOpenTicketsReport = "Member Open Tickets Report";
        public const string MemberSummaryOpenTicketsReportViewModel = "Member Summary Open Tickets Report";

        public const string CustomerTotalSummaryReport = "Customer Total Summary Report";
        public const string MemberTotalSummaryReport = "Member Total Summary Report";
        public const string MemberSummaryReport = "Member Summary Report";
        public const string DelivererReport = "Deliverer Report";
        public const string DelivererSummaryReport = "Deliverer Summary Report";
        public const string DelivererTagSummaryReport = "Deliverer Tag Summary Report";
        public const string TicketsSummaryReport = "Tickets Report";
        public const string TicketTagReport = "Tickets Tag Report";
        public const string TicketTagDetailReport = "Tickets Tag Detail Report";
        public const string TicketsHourReport = "Tickets Hourly Sales Report";
        public const string TicketTableReport = "Ticket Table Report";
        public const string MealTimeSalesReport = "Meal Time Sales Report";
        public const string TopDownSellReport = "Top Down Sell Report";
        public const string TopSellByQuantityReport = "Top Sell By Quantity Report";
        public const string TopSellByAmountReport = "Top Sell By Amount Report";
        public const string DownSellByQuantityReport = "Down Sell By Quantity Report";
        public const string DownSellByAmountReport = "Down Sell By Amount Report";
        public const string SalesByPeriodReport = "Sales By Period Report";
        public const string TicketOrderReport = "Ticket and Orders Report";
        public const string WorkTimeReport = "Work Shift Report";
        public const string ProductSummaryReport = "Product List Report";
        public const string ActivePromotionsReportViewModel = "Active Promotions Report";
        public const string SyncMenuListReport = "Menu Items Sync Report";
        public const string TillAccountSummaryReport = "Till Transactions Report";
        public const string LogReport = "Log Report";
        public const string ItemHourReport = "Items Hourly Sales Report";
        public const string ItemDepartmentReport = "Items Department Sales Report";
        public const string DailySalesSummaryReport = "Daily Sales Summary Report";
        public const string DailyTerminalSalesSummaryReport = "Daily Terminal Sales Summary Report";
        public const string CreditSalesReport = "Credit Sales Report";
        public const string AbnormalEndDayReport = "Abnormal End Day Report";
        public const string SalesTaxReport = "Sales Tax Report";
        public const string NonSalesTaxReport = "Non-Sales Tax Report";
        public const string SpeedOfServiceReport = "Speed Of Service Report";


        public const string ItemSalesReport = "Items Sales Report";
        public const string ItemOrderSalesReport = "Items Order Sales Report";
        public const string InternalSalesMeetingReport = "Internal Sales Meeting Report";
        public const string PettyCashReport = "Petty Cash Report";
        public const string ReimbursementExpenseReport = "Reimbursement Expense Report";
        public const string ItemUserSales = "Items User Sales Report";
        public const string ItemCategorySalesReport = "Items Category Sales Report";
        public const string ItemCategorySalesByDate = "Item Category Sales By Date";
        public const string ItemSalesByCategoryReport = "Item Sales By Category Report";

        public const string ItemTagSalesReport = "Items Tag Sales Report";
        public const string PaymentReport = "Payment Summary Report";
        public const string TicketTagItemSales = "Ticket Tag Items Sales Report";
        public const string TicketTagGroupSaleSummary = "Ticket Tag Group Sales Report";
        public const string ItemPortionSalesReport = "Items Portion Sales Report";
        public const string ItemGroupSalesByDate = "Items Category Date Sales Report";
        public const string DailyReconciliationReport = "Daily Reconciliation Report";

        public const string DetailDiscountReport = "Detail Discount Report";
        public const string ComboSalesReport = "Combo Sales Report";
        public const string TenderReport = "Tender Report";
        public const string CashAuditReport = "Cash Audit Report";
        public const string ZReport = "Z Report";
        public const string DaySummary = "Day Summary Report";

        public const string Discount = "Discount";
        public const string FixedDiscount = "Fixed Discount";

        public const string CreditCardReport = "Credit Card Report";
        public const string ReturnProductReport = "Return Product Report";
        public const string SoldOutHistoryReport = "Sold Out History Report";
        public const string FullTaxInvoiceReport = "Full Tax Invoice Report";
        public const string AABReprintReceiptReport = "Reprint Receipt Report";
        public const string CashSummaryReport = "Cash Summary Report";

        public const string SalesPromotionReport = "Sales Promotion Report";

        #region TicketLogs

        public const string PRINTDUPLICATERECEIPT = "PRINT DUPLICATE RECEIPT";

        #endregion

        public const string DisplayLastTicket = "Display Last Ticket";

        public const string Table = "Table";
        public const string Vacant = "Vacant";

        public const string PinLogin = "Pin Login";
        public const string UserLogin = "User Login";

        public const string OldEntity = "OldEntityName";
        public const string Promotion = "DISCOUNT";
        public const string Voucher = "VOUCHER";

        public const string ReturnedType = "ReturnedType";
        public const string ReturnedBy = "ReturnedBy";

        public const string OpenTickets = "OpenTickets";


        /*Payment Processors*/

        /*FOLLOW IN DINECONNECT AS WELL  - PaymentProcessors.Processors */


        public static string[] IgnoreTags = { "ChangeDue", "Tendered" };

        //Normal
        public static string[] ModulesAfterLogin =
            {"TicketModule", "PosModule", "EntityModule", "PaymentModule", "ModifierModule", "AutomationModule"};

        #region Entity Type Name

        public const string CrmMemberEntityType = "CrmMember";

        #endregion

        #region

        public static readonly string Comp = "Comp";
        public static readonly string Void = "Void";
        public static readonly string VoidAll = "VoidAll";
        public static readonly string Gift = "Gift";
        public static readonly string GiftAll = "GiftAll";
        public static readonly string Exchange = "Exchange";
        public static readonly string CancelExchange = "CancelExchange";
        public static readonly string All = "All";

        public static readonly string Refund = "Refund";
        public static readonly string Returned = "Returned";
        public static readonly string Available = "Available";
        public static readonly string TableNotReset = "TableNotReset";

        public static readonly string New = "New";
        public static readonly string Submitted = "Submitted";
        public static readonly string NewOrders = "New Orders";
        public static readonly string Preparing = "Preparing";
        public static readonly string Partial = "Partial";
        public static readonly string Prepared = "Prepared";
        public static readonly string Unpaid = "Unpaid";
        public static readonly string ChangePayment = "ChangePayment";
        public static readonly string Locked = "Locked";
        public static readonly string BillRequested = "Bill Requested";
        public static readonly string Status = "Status";
        public static readonly string PreOrder = "PreOrder";
        public static readonly string NonPreorder = "NonPreorder";
        public static readonly string ServeNow = "Serve Now";
        public static readonly string ServeLater = "Serve Later";
        public static readonly string Paid = "Paid";
        public static readonly string Delivery = "Delivery";
        public static readonly string Waiting = "Waiting";
        public static readonly string Delivering = "Delivering";
        public static readonly string Delivered = "Delivered";

        public static string Payment = "Payment";


        public static readonly string LocalWarehouse = "HQ";


        public static readonly string Tables = "Tables";
        public static readonly string Ticket = "Ticket";
        public static readonly string Members = "Members";
        public static readonly string Member = "Member";

        public static readonly string Customers = "Customers";
        public static readonly string Customer = "Customer";

        public static readonly string Phone = "Phone";
        public static readonly string Code = "Code";

        public static readonly string Name = "Name";
        public static readonly string Address = "Address";
        public static readonly string Deliverers = "Deliverers";
        public static readonly string Deliverer = "Deliverer";
        public static readonly string Locations = "Locations";
        public static readonly string Location = "Location";
        public static readonly string Url = "Url";

        public static readonly string NonCash = "NonCash";
        public static readonly string Cash = "Cash";
        public static readonly string InternalSale = "Internal Sale";
        public static readonly string Credit = "Credit";

        #endregion

        #region Job Name

        public static readonly string PrintTicket = "Print Ticket";
        public static readonly string PrintRefundTicket = "Print Refund Ticket";

        public static readonly string PrintInvoice = "Print Invoice";
        public static readonly string PrintInvoiceSlip = "Print Invoice Slip";
        public static readonly string CopyInvoice = "Copy Invoice";
        public static readonly string CopyInvoiceSlip = "Copy Invoice Slip";
        public static readonly string RePrintInvoice = "RePrint Invoice";
        public static readonly string RePrintInvoiceSlip = "RePrint Invoice Slip";
        public static readonly string PrintBill = "Print Bill";
        public static readonly string PrintKitchen = "Print Kitchen";
        public static readonly string PrintKitchenDisplay = "Print Kitchen Display";
        public static readonly string PrintMeeting = "Print Meeting";
        public static readonly string PrintPayInSlip = "Print Pay-In Slip";

        public static readonly string PrintCreditNote = "Print CreditNote";
        public static readonly string CopyCreditNote = "Copy CreditNote";
        public static readonly string ReprintCreditNote = "Reprint CreditNote";

        public static readonly string PrintTableMovement = "Print Table Movement";

        public static readonly string OpenTill = "Open Till";

        #endregion

        #region Action Name

        public const string AbortTicket = "Abort Ticket";
        public const string ActivatePos = "Activate POS";
        public const string AddLineToTextFile = "AddLine ToText File";
        public const string AddOrder = "Add Order";
        public const string AddTask = "Add Task";
        public const string AddCalculationAndPayment = "AddCalculationAndPayment";

        public const string AddTicketLog = "Add Ticket Log";
        public const string AddTillTransaction = "Add Till Transaction";
        public const string ApplyFreeItemPromotion = "Apply Free Item Promotion";
        public const string AskQuestion = "Ask Question";
        public const string AskQuestionFullScreen = "Ask Question Full Screen";
        public const string BackupDatabase = "Backup Database";
        public const string CallTicket = "Call Ticket";
        public const string CancelLastTicketPayments = "Cancel Last Ticket Payments";
        public const string CancelOrders = "Cancel Orders";
        public const string CancelTicketByKeyPad = "Cancel Ticket By KeyPad";
        public const string CancelTicketPayments = "Cancel Ticket Payments";
        public const string CashDrop = "Cash Drop";
        public const string ChangeItemPrice = "Change Item Price";
        public const string ChangeOrderDepartment = "Change Order Department";
        public const string ChangeTicketDepartment = "Change Ticket Department";
        public const string ChangeOrderDescription = "Change Order Description";
        public const string ChangeOrderPrice = "Change Order Price";
        public const string ChangeOrderPriceOnTag = "Change Order Price On Tag";
        public const string ChangeOrderUser = "Change Order User";
        public const string ChangeScreenMenu = "Change Screen Menu";
        public const string ChangeShiftUser = "Change Shift User";
        public const string ChangeTicketEntity = "Change Ticket Entity";
        public const string ChangeTicketProperties = "Change Ticket Properties";
        public const string ChangeTicketState = "Change Ticket State";
        public const string ChangeTicketToCredit = "Change Ticket To Credit";
        public const string ChangeUserLanguage = "Change User Language";
        public const string ChangeWorkTimeFloat = "Change WorkShift Float";
        public const string CheckServiceStatus = "Check Service Status";
        public const string ClearAllTicketQueue = "Clear All Ticket Queue";
        public const string CloseActiveTicket = "Close Active Ticket";
        public const string CloseSlientTicket = "Close Slient Ticket";
        public const string CreateAccountTransaction = "Create Account Transaction";
        public const string CreateAccountTransactionDocument = "Create Account Transaction Document";
        public const string CreateBatchAccountTransactionDocument = "Create Batch Account Transaction Document";
        public const string CreateEntity = "Create Entity";
        public const string CreateTicket = "Create Ticket";
        public const string CreateTicketFromOldTicket = "Create Ticket From Old Ticket";
        public const string DemandPromotion = "Demand Promotion";
        public const string AllPromotion = "All Promotions";
        public const string DisplayPaymentScreen = "Display Payment Screen";
        public const string DisplayPopup = "Display Popup";
        public const string DisplayPromotion = "Display Promotion";
        public const string DisplayServiceStatus = "Display Service Status";
        public const string DisplayTicket = "Display Ticket";
        public const string DisplayTicketList = "Display Ticket List";
        public const string DisplayTicketLog = "Display Ticket Log";
        public const string DisplayTickMessages = "Display Tick Messages";
        public const string EndWorkDay = "End WorkDay";
        public const string ExecuteAutomationCommand = "Execute Automation Command";
        public const string ExecuteDatabaseTask = "Execute Database Task";
        public const string ExecuteLastTicketJob = "Execute Last Ticket Job";
        public const string ExecuteNetworkPrintJob = "Execute Network Print Job";
        public const string ExecuteOrderCommand = "Execute Order Command";
        public const string ExecutePrintById = "Execute Print By Id";
        public const string ExecutePrintByKeyPad = "Execute Print ByKeyPad";
        public const string ExecutePrintByTicketNo = "Execute Print ByTicketNo";
        public const string ExecutePrintJob = "Execute PrintJob";
        public const string ExecuteScript = "Execute Script";
        public const string ExecuteTicketCommand = "Execute Ticket Command";
        public const string HoldTicket = "Hold Tickets";
        public const string LoadLastOrder = "Load Last Order";
        public const string LoadTicket = "Load Ticket";
        public const string LockTicket = "Lock Ticket";
        public const string LoopValues = "Loop Values";
        public const string MarkTicketAsClosed = "Mark Ticket As Closed";
        public const string MergeTickets = "Merge Tickets";
        public const string ModifyVariable = "Modify Variable";
        public const string MoveTaggedOrders = "Move Tagged Orders";
        public const string OrderNote = "Order Note";
        public const string PayTicket = "Pay Ticket";
        public const string PlaySound = "PlaySound";
        public const string PostTicket = "Post Ticket";
        public const string PrintAccountScreen = "Print Account Screen";
        public const string PrintAccountTransactionDocument = "Print Account Transaction Document";
        public const string PrintAccountTransactions = "Print Account Transactions";
        public const string PrintEntity = "Print Entity";
        public const string PrintReport = "Print Report";
        public const string PullOnlineOrder = "Pull Online Order";
        public const string PulleDineTickets = "PulleDineTickets";
        public const string KitchenBindStatus = "KitchenBindStatus";
        public const string RedeemConnectPoints = "Redeem Connect Points";
        public const string RedeemVoucher = "Redeem Voucher";
        public const string RefreshCache = "Refresh Cache";
        public const string RefundQuota = "Refund Quota";
        public const string RefundTicket = "Refund Ticket";
        public const string ReOpenTicket = "Re Open Ticket";
        public const string ResetNumerator = "Reset Numerator";
        public const string RestoreDatabase = "Restore Database";
        public const string ReturnTicket = "ReturnTicket";
        public const string SaveExcelReport = "Save Excel Report";
        public const string SavePdfReport = "Save PDF Report";
        public const string SaveReportToFile = "Save Report To File";
        public const string SaveTicket = "Save Ticket";
        public const string SelectAutomationCommand = "Select Automation Command";
        public const string SendDataToPort = "Send Data To Port";
        public const string SendEmail = "Send Email";
        public const string SendEmailCsvReport = "Send Email CSV Report";
        public const string SendEmailExcelReport = "Send Email Excel Report";
        public const string SendEmailPdfReport = "Send Email PDF Report";
        public const string SendFtpExcelReport = "Send FTP Excel Report";
        public const string SendMessageToPrinter = "Send Message To Printer";
        public const string SendPushOverMessage = "Send PushOver Message";
        public const string SendReceiptByEmail = "Send Receipt By Email";
        public const string SendTwilioMessage = "Send Twilio Message";
        public const string SendUrlMessage = "Send Url Message";
        public const string SetActiveTicketType = "Set Active TicketType";
        public const string SetCurrentTerminal = "Set Current Terminal";
        public const string SettleTicket = "Settle Ticket";
        public const string SetWidgetValue = "Set Widget Value";
        public const string ShowMessage = "Show Message";
        public const string SplitTicket = "Split Ticket";
        public const string StartProcess = "Start Process";
        public const string StopActiveTimers = "Stop Active Timers";
        public const string SwipeCardBalance = "Swipe Card Balance";
        public const string SwitchScreens = "Switch Screens";
        public const string SyncPull = "Sync Pull";
        public const string SyncTicket = "Sync Ticket";
        public const string SyncTicketById = "Sync Ticket By Id";
        public const string SyncWorkPeriod = "Sync WorkPeriod";
        public const string TagOrder = "Tag Order";
        public const string TicketDelivery = "Ticket Delivery";
        public const string TicketDiscountPromotion = "Ticket Discount Promotion";
        public const string RevertPromotion = "Revert Promotion";
        public const string TicketNote = "Ticket Note";
        public const string TicketSearchProduct = "Ticket Search Product";
        public const string UnlockTicket = "Unlock Ticket";
        public const string LockOrder = "Lock Order";
        public const string UntagOrder = "Untag Order";
        public const string UpdateApplicationSubtitle = "Update Application Subtitle";
        public const string UpdateBindOrderState = "Update Bind Order State";
        public const string UpdateCallTicket = "Update Call Ticket";
        public const string UpdateDemandPromotion = "Update Demand Promotion";
        public const string UpdateEntityData = "Update Entity Data";
        public const string UpdateEntityState = "Update Entity State";
        public const string UpdateLocationTicket = "Update Location Ticket";
        public const string UpdateOrder = "Update Order";
        public const string UpdateOrderNote = "Update Order Note";
        public const string UpdateOrderState = "Update Order State";
        public const string UpdateNewOrderNumber = "Update New Order Number";
        public const string RepeatOrder = "Repeat Order";
        public const string UpdateOrderTransaction = "Update Order Transaction";
        public const string UpdatePriceTag = "Update Price Tag";
        public const string UpdateProgramSetting = "Update Program Setting";
        public const string UpdatePromotion = "Update Promotion";
        public const string UpdateReservationStatus = "Update Reservation Status";
        public const string UpdateServiceEntity = "Update Service Entity";
        public const string UpdateSoldOut = "Update SoldOut";
        public const string UpdateTicketCalculation = "Update Ticket Calculation";
        public const string UpdateTicketDate = "Update Ticket Date";
        public const string UpdateTicketQueueStatus = "Update Ticket Queue Status";
        public const string UpdateTicketState = "Update Ticket State";
        public const string UpdateTicketTag = "Update Ticket Tag";
        public const string UpdateTicketTagByEntity = "UpdateTicketTagByEntity";

        public const string UpdateTicketWithTagPrice = "Update Ticket With TagPrice";
        public const string UpdateTillCount = "Update Till Count";
        public const string UploadTicketJournal = "Upload Ticket Journal";
        public const string UserLogOut = "User LogOut";
        public const string UpdateComboGroupOrderState = "Update Combo Group  Order State";
        public const string SyncOpenTable = "SyncOpenTable";
        #endregion

        #region Philipines

        public const string TransactionType = "TransactionType";
        public const string SeniorCitizen = "SENIOR CITIZEN";
        public const string PWD = "PWD";
        public const string Senior = "Senior";

        public const string VatableSales = "VATABLE";
        public const string VatExemptSales = "VATEXEMPT";
        public const string ZeroRatedSales = "ZEROSALES";
        public const string LessVatGroup = "LESSVAT";
        public const string LessVatTag = "(LESS VAT)";

        #endregion


        #region Reports

        public const string TimeAttendanceReport = "Time Attendance Report";
        public const string OrderTagReport = "Order Tag Report";

        #region Abnormal EndDay Report
        public const int StartWorkingHours = 8;
        public const int StartWorkingMinutes = 0;
        public const int EndWorkingHours = 17;
        public const int EndWorkingMinutes = 0;
        #endregion

        #region Out Tax Reports
        #region Sales Tax Report

        public const string OutTaxReports_Description_HardCoded_NormalTicket = "Sell products and services";
        public const string OutTaxReports_Description_HardCoded_Refund = "Cancel sales list";
        public const string OutTaxReports_Description_HardCoded_Return = "Return products or services";

        #endregion
        #endregion

        #endregion


        #region Printers

        public static string TicketPrinter = "Ticket Printer";
        public static string TextPrinter = "Text Printer";
        public static string HtmlPrinter = "Html Printer";
        public static string PortPrinter = "Port Printer";
        public static string DemoPrinter = "Demo Printer";
        public static string WindowsPrinter = "Windows Printer";
        public static string CustomPrinter = "Custom Printer";
        public static string RawPrinter = "Raw Printer";
        public static string NetworkPrinter = "Network Printer";
        public static string PortTicketPrinter = "PortTicket Printer";
        public static string CopyPrinter = "Copy Printer";
        public static string DocumentPrinter = "Document Printer";

        public static string KitchenDisplay = "Kitchen Display";
        public static string BrowserPrinter = "Browser Printer";
        public static string SecondMonitor = "Second Monitor";
        public static string SaveFilePrinter = "SaveFile Printer";
        public static string SettingPrinter = "Setting Printer";
        public static string UrlPrinter = "Url Printer";
        public static string DineQueue = "Dine Queue";

        #endregion

        #region Devices

        public const string Generic = "Generic";
        public const string Labau = "Labau";
        public const string CD = "CD";
        public const string Epson = "Epson";

        #endregion

        #region PaymentProcessor

        public const string ConnectVoucherProcessor = "Connect Voucher Processor";
        public const string ConnectVoucherSalesProcessor = "Connect Voucher Sales Processor";

        public const string ConnectMemberProcessor = "Connect Member Processor";
        public const string ConnectMemberDiscountProcessor = "Connect Member Discount Processor";
        public const string PrepaidCardProcessor = "Prepaid Card Processor";
        public const string PrepaidStudentCardProcessor = "Prepaid Student Card Processor";
        public const string CreditProcessor = "Credit Processor";
        public const string ChangeTaxProcessor = "Change Tax Processor";
        public const string ExecuteAutomationCommandProcessor = "Execute Automation Command Processor";
        public const string AddCalculationProcessor = "Add Calculation";
        public const string PaymentDenominationProcessor = "Payment Denomination";
        public const string AddDiscountCalculationProcessor = "Add Discount Calculation";
        public const string AddLimitProcessor = "Add Limit";
        public const string NetsProcessor = "Nets";
        public const string NetsICProcessor = "NetsIC";

        public const string InputTronics = "InputTronics";
        public const string InputTronicsNetwork = "InputTronicsNetwork";
        public const string PaymentAdjustmentProcessor = "Payment Adjustment";
        public const string Pay88Processor = "IPay88";
        public const string PaymentConfirmationProcessor = "Ask Payment Confirmation";
        public const string PaymentDescriptionProcessor = "Ask Payment Description";
        public const string PaymentMultiDescriptionProcessor = "Ask Payment Multi Description";
        public const string PaymentVoucherProcessor = "PaymentVoucher";
        public const string AddServiceCalculationProcessor = "Add Service Calculation";
        public const string CustomProcessor = "CustomProcessor";
        public const string Nets3Processor = "Nets3Processor";
        public const string ReasonPaymentProcessor = "ReasonPaymentProcessor";

        public const string RestrictDiscountPaymentType = "Restrict Discount Payment";
        #endregion


        #region AutomationCommandMaps

        public const string AuTicket = "Ticket";
        public const string AuPayment = "Payment";
        public const string AuTicketAndPayment = "Ticket And Payment";
        public const string AuOrderLine = "Order Line";
        public const string AuTicketList = "Ticket List";
        public const string AuDisplayUnderTicket = "Display Under Ticket";
        public const string AuDisplayUnderTicketSecondRow = "Display Under Ticket Second Row";
        public const string AuDisplayOnCommandSelector = "Display in Command Selector";
        public const string AuBelowOrder = "Below Order";
        public const string AuAboveOrder = "Above Order";
        public const string AuDisplayWorkPeriod = "Display in WorkPeriod";

        #endregion

        public const string Tendered = "Tendered";
        public const string ChangeDue = "ChangeDue";


        public static string AllTickets = "AllTickets";
        public static string OnlyOpenTickets = "OnlyOpenTickets";
        public static string OnlyClosedTickets = "OnlyClosedTickets";
        public static string TicketNo = "TicketNo";
        public static string Invoice = "Invoice";
        public static string Terminal = "Terminal";
        public static string NotSynced = "NotSynced";
        public static string TicketDiscountTag = "-TICKETDISCOUNT-";
        public static string DemandTag = "-DEMAND-";
        public const string UpdateTicketLineSeparator = "Update Ticket Line Separator";

        public static string GeneralModifier = "General Modifier";
        public static string CreditNoteUser = "CreditNoteUser";
        public static string DineConnectMember = "ConnectMember";
        public static string ChangeComboItem = "Change Combo Item";

        public static string ReservationId = "ReservationId";

    }
}