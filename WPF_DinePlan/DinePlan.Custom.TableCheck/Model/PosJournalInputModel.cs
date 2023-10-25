using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DinePlan.Custom.TableCheck.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class PosDiscount
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("order_at")]
        public DateTime OrderAt { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("amt")]
        public string Amt { get; set; }
    }

    public class PosOrder
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("order_at")]
        public DateTime OrderAt { get; set; }

        [JsonProperty("menu_category_ref")]
        public string MenuCategoryRef { get; set; }

        [JsonProperty("menu_category_name")]
        public string MenuCategoryName { get; set; }

        [JsonProperty("menu_item_ref")]
        public string MenuItemRef { get; set; }

        [JsonProperty("menu_item_name")]
        public string MenuItemName { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("qty")]
        public decimal Qty { get; set; }

        [JsonProperty("unit_price")]
        public string UnitPrice { get; set; }
    }

    public class PosPayment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("payment_at")]
        public DateTime PaymentAt { get; set; }

        [JsonProperty("amt")]
        public string Amt { get; set; }

        [JsonProperty("tender")]
        public string Tender { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }
    }

    public class PosJournalInputModel
    {
        [JsonProperty("receipt_num")]
        public string ReceiptNum { get; set; }

        [JsonProperty("reservation_status")]
        public string ReservationStatus { get; set; }

        [JsonProperty("reservation_ref")]
        public string ReservationRef { get; set; }

        [JsonProperty("original_receipt_num")]
        public string OriginalReceiptNum { get; set; }

        [JsonProperty("batch_date")]
        public string BatchDate { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("room_name")]
        public string RoomName { get; set; }

        [JsonProperty("membership_code")]
        public string MembershipCode { get; set; }

        [JsonProperty("pax")]
        public int Pax { get; set; }

        [JsonProperty("site_name")]
        public string SiteName { get; set; }

        [JsonProperty("revenue_center")]
        public string RevenueCenter { get; set; }

        [JsonProperty("staff_name")]
        public string StaffName { get; set; }

        [JsonProperty("staff_ref")]
        public string StaffRef { get; set; }

        [JsonProperty("terminal_name")]
        public string TerminalName { get; set; }

        [JsonProperty("table_names")]
        public List<string> TableNames { get; set; }

        [JsonProperty("order_at")]
        public DateTime OrderAt { get; set; }

        [JsonProperty("payment_at")]
        public DateTime PaymentAt { get; set; }

        [JsonProperty("subtotal_amt")]
        public string SubtotalAmt { get; set; }

        [JsonProperty("service_fee_amt")]
        public string ServiceFeeAmt { get; set; }

        [JsonProperty("service_fee_rate")]
        public string ServiceFeeRate { get; set; }

        [JsonProperty("coupon_amt")]
        public string CouponAmt { get; set; }

        [JsonProperty("discount_amt")]
        public string DiscountAmt { get; set; }

        [JsonProperty("tax_rate")]
        public string TaxRate { get; set; }

        [JsonProperty("tax_amt")]
        public string TaxAmt { get; set; }

        [JsonProperty("tax_included_amt")]
        public string TaxIncludedAmt { get; set; }

        [JsonProperty("total_amt")]
        public string TotalAmt { get; set; }

        [JsonProperty("settle_amt")]
        public string SettleAmt { get; set; }

        [JsonProperty("change_amt")]
        public string ChangeAmt { get; set; }

        [JsonProperty("system_api_provider")]
        public string SystemApiProvider { get; set; }

        [JsonProperty("system_maker")]
        public string SystemMaker { get; set; }

        [JsonProperty("system_model")]
        public string SystemModel { get; set; }

        [JsonProperty("system_version")]
        public string SystemVersion { get; set; }

        [JsonProperty("pos_orders")]
        public List<PosOrder> PosOrders { get; set; }

        [JsonProperty("pos_payments")]
        public List<PosPayment> PosPayments { get; set; }

        [JsonProperty("pos_discounts")]
        public List<PosDiscount> PosDiscounts { get; set; }
    }

    public class PosJournal
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("transaction_at")]
        public DateTime TransactionAt { get; set; }

        [JsonProperty("franchise_id")]
        public string FranchiseId { get; set; }

        [JsonProperty("shop_id")]
        public string ShopId { get; set; }

        [JsonProperty("reservation_id")]
        public string ReservationId { get; set; }

        [JsonProperty("original_receipt_num")]
        public string OriginalReceiptNum { get; set; }

        [JsonProperty("batch_date")]
        public string BatchDate { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("room_name")]
        public string RoomName { get; set; }

        [JsonProperty("membership_code")]
        public string MembershipCode { get; set; }

        [JsonProperty("pax")]
        public int Pax { get; set; }

        [JsonProperty("reservation_ref")]
        public string ReservationRef { get; set; }

        [JsonProperty("site_name")]
        public string SiteName { get; set; }

        [JsonProperty("revenue_center")]
        public string RevenueCenter { get; set; }

        [JsonProperty("staff_name")]
        public string StaffName { get; set; }

        [JsonProperty("staff_ref")]
        public string StaffRef { get; set; }

        [JsonProperty("terminal_name")]
        public string TerminalName { get; set; }

        [JsonProperty("table_names")]
        public List<string> TableNames { get; set; }

        [JsonProperty("order_at")]
        public DateTime OrderAt { get; set; }

        [JsonProperty("payment_at")]
        public DateTime PaymentAt { get; set; }

        [JsonProperty("subtotal_amt")]
        public string SubtotalAmt { get; set; }

        [JsonProperty("service_fee_amt")]
        public string ServiceFeeAmt { get; set; }

        [JsonProperty("service_fee_rate")]
        public string ServiceFeeRate { get; set; }

        [JsonProperty("coupon_amt")]
        public string CouponAmt { get; set; }

        [JsonProperty("discount_amt")]
        public string DiscountAmt { get; set; }

        [JsonProperty("tax_rate")]
        public string TaxRate { get; set; }

        [JsonProperty("tax_amt")]
        public string TaxAmt { get; set; }

        [JsonProperty("tax_included_amt")]
        public string TaxIncludedAmt { get; set; }

        [JsonProperty("total_amt")]
        public string TotalAmt { get; set; }

        [JsonProperty("settle_amt")]
        public string SettleAmt { get; set; }

        [JsonProperty("change_amt")]
        public string ChangeAmt { get; set; }

        [JsonProperty("system_api_provider")]
        public string SystemApiProvider { get; set; }

        [JsonProperty("system_maker")]
        public string SystemMaker { get; set; }

        [JsonProperty("system_model")]
        public string SystemModel { get; set; }

        [JsonProperty("system_version")]
        public string SystemVersion { get; set; }

        [JsonProperty("pos_orders")]
        public List<PosOrder> PosOrders { get; set; }

        [JsonProperty("pos_payments")]
        public List<PosPayment> PosPayments { get; set; }

        [JsonProperty("pos_discounts")]
        public List<PosDiscount> PosDiscounts { get; set; }
    }

    public class PosJournalOutputModel
    {
        [JsonProperty("pos_journal")]
        public PosJournal PosJournal { get; set; }

        [JsonProperty("reservations")]
        public List<Reservation> Reservations { get; set; }
    }

    public class PosJournalOutputListModel
    {
        [JsonProperty("pos_journals")]
        public List<PosJournal> PosJournals { get; set; }

        [JsonProperty("reservations")]
        public List<Reservation> Reservations { get; set; }
    }



}
