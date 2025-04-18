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

// MainWindow.xaml.cs
using System.Collections.ObjectModel;

namespace NewBankApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            var transactions = new ObservableCollection<Transaction>
            {
                new Transaction { Date = "Today", Merchant = "Southeast Cafe", Category = "Caringmenet", Type = "Real", Amount = "-18.00 $" },
                new Transaction { Date = "Today", Merchant = "Off White Orbital Street €", Category = "Caringmenet", Type = "Others", Amount = "-260.00 $" },
                new Transaction { Date = "10:05", Merchant = "Spotify Premium", Category = "Fair", Type = "Entertainment", Amount = "-19.00 $" },
                new Transaction { Date = "10:05", Merchant = "Google Inc.", Category = "Trouble", Type = "Salary", Amount = "+9.500 $" }
            };

            dataGrid.ItemsSource = transactions;
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            // Handle transfer button click
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            // Handle link account button click
        }
    }

    public class Transaction
    {
        public string Date { get; set; }
        public string Merchant { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Amount { get; set; }
    }
}