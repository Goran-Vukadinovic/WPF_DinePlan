using System.Collections.Generic;

namespace DinePlan.Common.Combo
{
    public class ComboWindowInputButton
    {
        public ComboWindowInputButton()
        {
            Count = 1;
        }

        public string Name { get; set; }

        public decimal Price { get; set; }
        public string CurrencySymbol { get; set; }

        public int ID { get; set; }

        public string ImagePath { get; set; }

        public bool IsSelected { get; set; }
        public int Maximum { get; set; }
        public int Count { get; set; }
        public int MaxItemCount { get; set; }
        public string ButtonColor { get; set; }
        public string GroupTag { get; set; }
    }

    public class ComboWindowInput
    {
        public ComboWindowInput()
        {
            MainButtons = new List<ComboWindowInputButton>();
            AlternateButtons = new List<ComboWindowInputButton>();
        }

        public string Question { get; set; }
        public string MainText { get; set; }
        public List<ComboWindowInputButton> MainButtons { get; set; }
        public List<ComboWindowInputButton> AlternateButtons { get; set; }
        public string Background { get; set; }
        public int MainItemCount { get; set; }
        public int AddOnCount { get; set; }

        public int Minimum { get; set; }
        public int Maximum { get; set; }
    }
}