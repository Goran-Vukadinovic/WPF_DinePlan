using DinePlan.Domain.Models.Promotion;
using DinePlan.Domain.Models.Tickets;
using DinePlan.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using DinePlan.Modules.PaymentModule.Models;

namespace DinePlan.Modules.PaymentModule
{
    public class OrderSelector
    {
        private const string Keyformat = "{0}_{1}";

        public OrderSelector()
        {
            Selectors = new List<Selector>();
            ExchangeRate = 1;
            SelectedTicket = Ticket.Empty;
        }

        public decimal ExchangeRate { get; set; }
        public Ticket SelectedTicket { get; set; }
        public IList<Selector> Selectors { get; set; }

        public decimal SelectedTotal
        {
            get
            {
                return decimal.Round(Selectors.Sum(x => x.SelectedQuantity * x.Price), LocalSettings.Decimals,
                    MidpointRounding.AwayFromZero);
            }
        }

        public decimal RemainingTotal
        {
            get
            {
                return decimal.Round(Selectors.Sum(x => x.RemainingQuantity * x.Price), LocalSettings.Decimals,
                    MidpointRounding.AwayFromZero);
            }
        }

        protected decimal AutoRoundValue { get; set; }

        public void UpdateTicket(Ticket ticket)
        {
            SelectedTicket = ticket;
            Selectors.Clear();
            if (SelectedTicket.RemainingAmount != 0)
            {
                UpdateSelectors();
                foreach (var paidItem in SelectedTicket.PaidItems)
                {
                    var mi = Selectors.SingleOrDefault(x => x.Key == paidItem.Key);
                    mi?.AddPaidItem(paidItem);
                }
            }
        }

        private void UpdateSelector(Order order)
        {
            var promotionDetails = order.GetPromotionDetailValues();
            var key = GetKey(order, promotionDetails);
            var selector = Selectors.FirstOrDefault(x => x.Key == key);
            if (selector == null)
            {
                selector = new Selector
                {
                    Key = key,
                    Description = order.MenuItemName + order.GetPortionDesc(),
                    Model = order,
                };
                Selectors.Add(selector);
            }
            selector.Quantity += order.Quantity;
            if (promotionDetails.Any())
            {
                selector.Price += order.GetValue();
                selector.IsPromotion = true;
            }
            else
            {
                selector.Price = order.GetVisiblePrice();
                selector.IsPromotion = false;
            }
        }

        private string GetKey(Order order, IEnumerable<PromotionDetailValue> details)
        {
            if(details.Any())
                return string.Format(Keyformat, order.MenuItemId, order.GetValue());
            return string.Format(Keyformat, order.MenuItemId, order.GetPrice());
        }

        public void Select(Selector selector)
        {
            selector.Select();
        }

        public void PersistSelectedItems()
        {
            foreach (var selector in Selectors) selector.PersistSelected();
        }

        public void UpdateSelectedTicketPaidItems()
        {
            SelectedTicket.PaidItems.Clear();
            Selectors.SelectMany(x => x.GetSelectedItems()).ToList().ForEach(x => SelectedTicket.PaidItems.Add(x));
        }

        public void PersistTicket()
        {
            SelectedTicket.PaidItems.Clear();
            Selectors.SelectMany(x => x.GetPaidItems()).ToList().ForEach(x => SelectedTicket.PaidItems.Add(x));
        }

        public void UpdateExchangeRate(decimal exchangeRate)
        {
            ExchangeRate = exchangeRate;
            Selectors.ToList().ForEach(x => x.Quantity = 0);
            UpdateSelectors();
        }

        private void UpdateSelectors()
        {
            foreach (var order in SelectedTicket.Orders.Where(x => x.CalculatePrice))
                UpdateSelector(order);

            RoundSelectors();
        }

        public void ClearSelection()
        {
            foreach (var selector in Selectors) selector.ClearSelection();
        }

        public void UpdateAutoRoundValue(decimal d)
        {
            AutoRoundValue = d;
            RoundSelectors();
        }

        private void RoundSelectors()
        {
            if (AutoRoundValue != 0 && RemainingTotal > 0)
            {
                var amount = 0m;
                foreach (var selector in Selectors)
                {
                    var price = selector.Price;
                    var newPrice =
                        decimal.Round(price / AutoRoundValue, LocalSettings.Decimals, MidpointRounding.AwayFromZero) *
                        AutoRoundValue;
                    selector.Price = newPrice;
                    amount += newPrice * selector.RemainingQuantity;
                }

                var mLast = Selectors.Where(x => x.RemainingQuantity > 0).OrderBy(x => x.RemainingPrice).First();
                mLast.Price += (SelectedTicket.GetSum() / ExchangeRate - amount) / mLast.RemainingQuantity;
            }
        }

        public decimal GetRemainingAmount()
        {
            return SelectedTicket.GetRemainingAmount();
        }

        public IEnumerable<PaidItem> GetSelectedItems()
        {
            return Selectors.SelectMany(x => x.GetSelectedItems());
        }

        public decimal GetSelectedAmount()
        {
            var result = SelectedTotal;
            var remaining = GetRemainingAmount();
            if (result > remaining) result = remaining;
            return result;
        }
    }
}