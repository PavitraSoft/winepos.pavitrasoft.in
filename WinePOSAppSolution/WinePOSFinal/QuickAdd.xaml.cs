using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinePOSFinal.Classes;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for QuickAdd.xaml
    /// </summary>
    public partial class QuickAdd : Window
    {

        ObservableCollection<BillingItem> _objBillingItems = new ObservableCollection<BillingItem>();
        string _Name = string.Empty;
        public QuickAdd(ObservableCollection<BillingItem> BillingItems, int Quantity, decimal Price, string Name)
        {
            InitializeComponent();
            _objBillingItems = BillingItems;
            txtQuantity.Text = Quantity.ToString();
            txtPrice.Text = Price.ToString();
            _Name = Name;
        }

        // Allow only numbers for Quantity
        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$"); // Allow only numeric characters
        }

        // Allow decimal values (12,3 format) for Price
        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text); // Simulate new text
            e.Handled = !Regex.IsMatch(fullText, @"^\d{0,9}(\.\d{0,3})?$"); // Validate decimal 12,3
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtQuantity.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Please enter both Quantity and Price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity))
            {
                MessageBox.Show("Quantity must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Price must be a valid decimal number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
            decimal tax = 0;
            decimal taxedPrice = price + tax;
            decimal totalPrice = taxedPrice * quantity;

            // Create a new BillingItem
            BillingItem newItem = new BillingItem
            {
                UPC = string.Empty,
                Name = _Name,
                Price = Convert.ToString(price),
                Quantity = Convert.ToString(quantity),
                Tax = tax.ToString("F2"), // Format total price as a string with 2 decimals
                Discount = "0",
                TotalPrice = totalPrice.ToString("F2"), // Format total price as a string with 2 decimals
                UserName = AccessRightsManager.GetUserName(),
                Note = string.Empty
            };

            // Add the new item to the ObservableCollection
            _objBillingItems.Add(newItem);

            // Close the window and return success
            this.DialogResult = true;

            //if (objService.SaveBulkPricing(itemID, quantity, price))
            //{
            //    MessageBox.Show("Pricing saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
    }
}
