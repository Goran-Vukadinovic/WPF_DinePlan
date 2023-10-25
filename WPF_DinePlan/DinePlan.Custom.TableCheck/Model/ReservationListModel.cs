using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DinePlan.Custom.TableCheck.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Pagination
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }
    }

    public class Reservation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("franchise_id")]
        public string FranchiseId { get; set; }

        [JsonProperty("shop_id")]
        public string ShopId { get; set; }

        [JsonProperty("start_at")]
        public DateTime StartAt { get; set; }

        [JsonProperty("duration")]
        public decimal Duration { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("pax")]
        public int? Pax { get; set; }

        [JsonProperty("purpose")]
        public string Purpose { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("provider_ref")]
        public string ProviderRef { get; set; }

        [JsonProperty("points")]
        public int? Points { get; set; }

        [JsonProperty("seat_types")]
        public List<string> SeatTypes { get; set; }

        [JsonProperty("smoking")]
        public string Smoking { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("party_name")]
        public string PartyName { get; set; }

        [JsonProperty("room_name")]
        public string RoomName { get; set; }

        [JsonProperty("table_names")]
        public List<string> TableNames { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("cancel_reason_type")]
        public string CancelReasonType { get; set; }

        [JsonProperty("cancel_reason")]
        public string CancelReason { get; set; }
    }

    public class ReservationListModel
    {
        [JsonProperty("reservations")]
        public List<Reservation> Reservations { get; set; }

        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }

    public class ReservationModel
    {
        [JsonProperty("reservation")]
        public Reservation Reservation { get; set; }
    }



}
