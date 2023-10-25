using DinePlan.Modules.PaymentModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DinePlan.Modules.PaymentModule.Views
{
    /// <summary>
    /// Interaction logic for ForeignCurrencyAmountDialog.xaml
    /// </summary>
    public partial class ForeignCurrencyAmountDialog : Window
    {
        public ForeignCurrencyAmountDialog()
        {
            InitializeComponent();
        }

        public ForeignCurrencyAmountDialog(ForeignCurrencyAmountViewModel model)
        {
            DataContext = model;
            InitializeComponent();
        }
    }
}
