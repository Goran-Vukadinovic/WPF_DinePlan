using System.Collections.Generic;
using System.Linq;
using DinePlan.Domain.Models.Tickets;

namespace DinePlan.Modules.PaymentModule.Models
{
    public class Selector
    {
        private readonly IList<PaidItem> _paidItems = new List<PaidItem>();
        private readonly IList<PaidItem> _selectedItems = new List<PaidItem>();
        public string Key { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

        public decimal RemainingPrice => IsPromotion ? Price : RemainingQuantity * Price;

        public decimal SelectedQuantity
        {
            get { return _selectedItems.Sum(x => x.Quantity); }
        }

        public string Description { get; set; }

        public decimal RemainingQuantity => Quantity - PaidQuantitiy;

        public bool IsSelected => SelectedQuantity > 0;
        public bool IsPromotion { get; set; }

        public decimal PaidQuantitiy
        {
            get { return _paidItems.Sum(x => x.Quantity); }
        }
        public Order Model { get; set; }

        public void Select()
        {
            if (SelectedQuantity < RemainingQuantity)
            {
                _selectedItems.Add(new PaidItem { Key = Key, Quantity = 1 });
            }
            Model.IsSelected = !Model.IsSelected;
        }

        public void PersistSelected()
        {
            foreach (var selectedItem in _selectedItems)
            {
                var pitem = _paidItems.SingleOrDefault(x => x.Key == selectedItem.Key);
                if (pitem != null)
                    pitem.Quantity += selectedItem.Quantity;
                else _paidItems.Add(selectedItem);
            }

            _selectedItems.Clear();
        }

        public void AddPaidItem(PaidItem paidItem)
        {
            _paidItems.Add(paidItem);
        }

        public IEnumerable<PaidItem> GetPaidItems()
        {
            return _paidItems;
        }

        public void ClearSelection()
        {
            _selectedItems.Clear();
        }

        public IEnumerable<PaidItem> GetSelectedItems()
        {
            return _selectedItems;
        }
    }
}