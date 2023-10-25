using DinePlan.Infrastructure.Settings;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;

namespace DinePlan.Common.Combo
{
    /// <summary>
    /// </summary>
    public class ComboGroupItem : BindableBase
    {
        /// <summary>
        ///     The back up value for <see cref="BackgroundColor" /> property.
        /// </summary>
        private string backgroundColor;

        private string currencySymbol;

        /// <summary>
        ///     The back up value for <see cref="ID" /> property.
        /// </summary>
        private int id;

        /// <summary>
        ///     The back up value for <see cref="ImagePath" /> property.
        /// </summary>
        private string imagePath;


        private int maxCount;

        /// <summary>
        ///     The back up value for <see cref="Name" /> property.
        /// </summary>
        private string name;

        /// <summary>
        ///     The back up value for <see cref="Price" /> property.
        /// </summary>
        private decimal price;

        public bool IsGroupTag { get; set; }

        public event EventHandler MaxSelectionEvent;


        public int MaximumCount
        {
            get => maxCount;

            set
            {
                if (maxCount != value)
                {
                    maxCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the BackgroundColor.
        /// </summary>
        public string BackgroundColor
        {
            get => backgroundColor;

            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    RaisePropertyChanged(nameof(BackgroundColor));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the ImagePath.
        /// </summary>
        public string ImagePath
        {
            get => imagePath;

            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    RaisePropertyChanged(nameof(ImagePath));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the ID.
        /// </summary>
        public int ID
        {
            get => id;

            set
            {
                if (id != value)
                {
                    id = value;
                    RaisePropertyChanged(nameof(ID));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the Price.
        /// </summary>
        public decimal Price
        {
            get => price;

            set
            {
                if (price != value)
                {
                    price = value;
                    RaisePropertyChanged(nameof(Price));
                }
            }
        }

        public string CurrencySymbol
        {
            get => currencySymbol;
            set
            {
                if (currencySymbol != value)
                {
                    currencySymbol = value;
                    RaisePropertyChanged(nameof(CurrencySymbol));
                }
            }
        }


        public string ComboPrice => $"{LocalSettings.CurrencySymbol}{Price}";

        /// <summary>
        ///     Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get => name;

            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        private int count;
        public int Count
        {
            get => count;
            set
            {
                count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                if(IsGroupTag)
                {
                    return _isSelected && Count > 0;
                }
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        public ICommand AddQuantityCommand { get => new DelegateCommand(AddQuantityCommandHandle); }
        public void AddQuantityCommandHandle()
        {
            if(IsGroupTag)
            {
                MaxSelectionEvent?.Invoke(this, null);
                return;
            }
            int count = Group.MainItems.Where(x => x.IsSelected).Sum(x => x.Count);
            if (count == Group.MaximumCount)
            {
                if (Group.MainItemsSelectedCacheQueue.Count > 1)
                {
                    ComboGroupItem first = Group.MainItemsSelectedCacheQueue.Where(x => x.ID != ID).First();
                    if (first.Count - 1 == 0)
                    {
                        first.IsSelected = false;
                        first.Count = 0;
                        Group.MainItemsSelectedCacheQueue.Remove(first);
                    }
                    else
                    {
                        first.Count--;
                    }
                }

                if (Group.MainItems.Where(x => x.IsSelected).Sum(x => x.Count) + 1 <= Group.MaximumCount)
                {
                    Count++;
                }
            }
            else if (count < Group.MaximumCount || Group.MaximumCount == 0)
            {
                Count++;
            }

            if (Group.MainItems.Where(x => x.IsSelected).Sum(x => x.Count) == Group.MaximumCount)
            {
                MaxSelectionEvent?.Invoke(this, null);
            }
        }

        public ICommand SubtractQuantityCommand { get => new DelegateCommand(SubtractQuantityCommandHandle); }
        public void SubtractQuantityCommandHandle()
        {
            IEnumerable<ComboGroupItem> findCombos = Group.MainItems.Where(x => x.IsSelected);
            if (findCombos.Sum(x => x.Count) - 1 >= Group.MinimumCount)
            {
                if (Count == 1)
                {
                    if (Group.MinimumCount == 0 ||
                        (Group.MinimumCount > 0 && Group.MainItems.Where(x => x.ID != ID && x.IsSelected).Sum(x => x.Count) >= Group.MinimumCount))
                    {
                        IsSelected = false;
                        Group.MainItemsSelectedCacheQueue.Remove(this);
                    }
                }
                Count--;
            }
        }

        public ICommand ComboCommand { get => new DelegateCommand(ComboCommandHandle); }
        public void ComboCommandHandle()
        {
            try
            {
                bool isAdd = false;
                if (IsSelected)
                {
                    isAdd = true;
                    AddQuantityCommandHandle();
                }
                else
                {
                    IsSelected = true;
                    if(IsGroupTag)
                    {
                        MaxSelectionEvent?.Invoke(this, null);
                        return;
                    }
                    Count = 1;
                    int count = Group.MainItems.Where(x => x.IsSelected).Sum(x => x.Count);
                    if (count > Group.MaximumCount && Group.MaximumCount > 0 && Group.MainItemsSelectedCacheQueue.Any())
                    {
                        if (Group.MainItemsSelectedCacheQueue[0].Count > 1)
                        {
                            Group.MainItemsSelectedCacheQueue[0].Count--;
                        }
                        else
                        {
                            Group.MainItemsSelectedCacheQueue[0].IsSelected = false;
                            Group.MainItemsSelectedCacheQueue[0].Count = 0;
                            Group.MainItemsSelectedCacheQueue.RemoveAt(0);
                        }
                    }
                    Group.MainItemsSelectedCacheQueue.Add(this);
                }

                if (Group.MainItems.Where(x => x.IsSelected).Sum(x => x.Count) == Group.MaximumCount && !isAdd)
                {
                    MaxSelectionEvent?.Invoke(this, null);
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        ///     Gets or sets the group.
        /// </summary>
        /// <value>
        ///     The group.
        /// </value>
        public ComboGroup Group { get; set; }

        public string GroupTag { get; set; }

        private string _btnColor;
        public string ButtonColor
        {
            get
            {
                if(string.IsNullOrEmpty(_btnColor))
                {
                    return "#E6E784";
                }
                return _btnColor;
            }
            set
            {
                _btnColor = value;
            }
        }

        public Brush ButtonColorBrushType
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(ButtonColor))
                        return (Brush)new BrushConverter().ConvertFrom(ButtonColor);
                }
                catch (Exception)
                {
                    // ignored
                }

                return Brushes.Transparent;
            }
        }
    }

    /// <summary>
    ///     The struct define the data view in 1 combo group.
    /// </summary>
    public class ComboGroup
    {
        public ComboGroup()
        {
            MainItems = new List<ComboGroupItem>();
        }

        /// <summary>
        ///     Gets or sets the main items.
        /// </summary>
        /// <value>
        ///     The main items.
        /// </value>
        public List<ComboGroupItem> MainItems { get; set; }

        /// <summary>
        ///     Gets or sets the add on items.
        /// </summary>
        /// <value>
        ///     The add on items.
        /// </value>
        public List<ComboGroupItem> AddOnItems { get; set; }


        public List<ComboGroupItem> MainItemsSelectedCacheQueue = new List<ComboGroupItem>();

        public List<ComboGroupItem> AddOnItemsSelectedCacheQueue = new List<ComboGroupItem>();

        /// <summary>
        ///     Gets or sets the color of the background.
        /// </summary>
        /// <value>
        ///     The color of the background.
        /// </value>
        public string BackgroundColor { get; set; }

        public string GroupHeaderBackground { get; set; }

        /// <summary>
        ///     Gets or sets the color of the foreground.
        /// </summary>
        /// <value>
        ///     The color of the foreground.
        /// </value>
        public string ForegroundColor { get; set; }

        /// <summary>
        ///     Gets or sets the condition text.
        /// </summary>
        /// <value>
        ///     The condition text.
        /// </value>
        public string ConditionText { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the minimum count.
        /// </summary>
        /// <value>
        ///     The minimum count.
        /// </value>
        public int MinimumCount { get; set; }

        /// <summary>
        ///     Gets or sets the maximum.
        /// </summary>
        /// <value>
        ///     The maximum.
        /// </value>
        public int MaximumCount { get; set; }

        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
    }

    public class UpSellingItem : BindableBase
    {
        private int count;

        private bool isSelected;
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }

        public int Count
        {
            get => count;

            set
            {
                if (count != value)
                {
                    count = value;
                    RaisePropertyChanged(nameof(Count));
                }
            }
        }

        public bool IsSelected
        {
            get => isSelected;

            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }
    }

    public class ProductComboItemSelectedDoneEvent : PubSubEvent { }
}