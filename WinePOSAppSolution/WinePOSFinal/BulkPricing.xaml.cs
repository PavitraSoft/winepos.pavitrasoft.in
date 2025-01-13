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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using WinePOSFinal.ServicesLayer;
using WinePOSFinal.Classes;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for BulkPricing.xaml
    /// </summary>
    public partial class BulkPricing : Window
    {
        private readonly List<BulkPricingItem> _bulkPricingList;
        int ItemID = 0;

        public BulkPricing(List<BulkPricingItem> bulkPricingList, int intItemID)
        {
            InitializeComponent();
            ItemID = intItemID;
            _bulkPricingList = bulkPricingList;
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

            // Add data to the list
            _bulkPricingList.Add(new BulkPricingItem
            {
                BuilkPricingID = 0,
                ItemID = ItemID,
                Quantity = quantity,
                Price = price
            });                

            // Close the window and return success
            this.DialogResult = true;

            //if (objService.SaveBulkPricing(itemID, quantity, price))
            //{
            //    MessageBox.Show("Pricing saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
    }
}
