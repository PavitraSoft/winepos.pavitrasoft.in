using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinePOSFinal.Classes;
using WinePOSFinal.ServicesLayer;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for InventoryMaintenance.xaml
    /// </summary>
    public partial class InventoryMaintenance : UserControl
    {
        WinePOSService objService = new WinePOSService();
        int intItemID = 0;
        DataTable dtTax = new DataTable();
        public List<BulkPricingItem> BulkPricingItems { get; set; }
        public InventoryMaintenance()
        {
            InitializeComponent();
            BulkPricingItems = new List<BulkPricingItem>();

            ReloadInventoryMaintenanceData();

            BindDropdown();
        }


        public void ReloadInventoryMaintenanceData()
        {
            dtTax = objService.GetTaxData();
            // Bind the DataGrid to the list

            ClearFields();
        }

        public void PopulateData(int ItemID)
        {
            InitializeComponent();

            BindDropdown();

            ClearFields();

            intItemID = ItemID;

            FetchAndPopulateItemData();
        }

        private void BindDropdown()
        {
            DataTable dtData = objService.GetIMDropdownData();

            List<ComboBoxItem> cbItems = ConvertDataTableToComboBoxItems(dtData);

            // Add a new item programmatically
            //cbItems.Add(new ComboBoxItem("New Item", ""));

            cbCategory.ItemsSource = cbItems;

            cbCategory.SelectedIndex = 0;
        }

        private void ClearFields()
        {
            cbCategory.SelectedIndex = 0;
            txtUPC.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtADescription.Text = string.Empty;

            txtItemCost.Text = "";
            txtChargePrice.Text = "";
            txtPriceWithTax.Text = "";
            txtStock.Text = "";

            txtVendorName.Text = "";
            txtCase.Text = "0";
            txtCaseCost.Text = "";

            txtDroppedItem.Text = string.Empty;
            txtDroppedItem.IsEnabled = false;
            txtchkST.IsChecked = false;
            txtQuickAdd.IsChecked = false;
            intItemID = 0;

            dgBulkPricing.ItemsSource = null;

        }

        private void FetchAndPopulateItemData()
        {
            Items objItem =  objService.FetchItemDataByID(intItemID);


            ComboBoxItem selectedItem = cbCategory.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Value == objItem.Category); ;
            
            cbCategory.SelectedItem = selectedItem;
            txtUPC.Text = !string.IsNullOrWhiteSpace(objItem.UPC) ? objItem.UPC : string.Empty;

            txtDroppedItem.Text = !string.IsNullOrWhiteSpace(objItem.DroppedItem) ? objItem.DroppedItem : string.Empty;



            txtDescription.Text = !string.IsNullOrWhiteSpace(objItem.Name) ? objItem.Name : string.Empty;
            txtADescription.Text = !string.IsNullOrWhiteSpace(objItem.Additional_Description) ? objItem.Name : string.Empty;

            txtItemCost.Text = Convert.ToString(objItem.itemcost);
            txtChargePrice.Text = Convert.ToString(objItem.ChargedCost);
            txtStock.Text = Convert.ToString(objItem.InStock);

            txtVendorName.Text = Convert.ToString(objItem.VendorName);
            txtCase.Text = Convert.ToString(objItem.CaseCost);
            txtCaseCost.Text = Convert.ToString(objItem.InCase);
            txtPriceWithTax.Text = Convert.ToString(objItem.SalesTaxAmt);

            txtchkST.IsChecked = objItem.Sales_Tax;
            //txtchkST2.IsChecked = objItem.Sales_Tax_2;
            //txtchkST3.IsChecked = objItem.Sales_Tax_3;
            //txtchkST4.IsChecked = objItem.Sales_Tax_4;
            //txtchkST5.IsChecked = objItem.Sales_Tax_5;
            //txtchkST6.IsChecked = objItem.Sales_Tax_6;
            //txtchkBT.IsChecked = objItem.Bar_Tax;

            txtQuickAdd.IsChecked = objItem.QuickADD;

            BulkPricingItems = objItem.BulkPricingItems;
            dgBulkPricing.ItemsSource = null;
            dgBulkPricing.ItemsSource = BulkPricingItems;
        }

        List<ComboBoxItem> ConvertDataTableToComboBoxItems(DataTable dt)
        {
            List<ComboBoxItem> comboBoxItems = new List<ComboBoxItem>();

            foreach (DataRow row in dt.Rows)
            {
                // Create a new ComboBoxItem with the Code and Description from DataTable
                ComboBoxItem item = new ComboBoxItem(Convert.ToString(row["Description"]), Convert.ToString(row["Code"]));
                comboBoxItems.Add(item);
            }

            return comboBoxItems;
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Items objItem = new Items();

            // Ensure that the text in the textbox is a valid decimal number
            if (decimal.TryParse(txtChargePrice.Text, out decimal chargePrice))
            {
                // Recalculate the tax and price when the value changes
                CalculateTax(chargePrice, dtTax);
            }
            else
            {
                // Handle invalid input (e.g., show an error message or revert to previous value)
                txtChargePrice.Text = "";  // Example default value if input is invalid
            }

            objItem.ItemID = intItemID;
            ComboBoxItem selectedItem = (ComboBoxItem)cbCategory.SelectedItem;

            objItem.Category = selectedItem.Value;
            objItem.UPC = txtUPC.Text;
            objItem.Name = txtDescription.Text;
            objItem.Additional_Description = txtADescription.Text;

            objItem.ItemCost = !string.IsNullOrWhiteSpace(txtItemCost.Text) ? Convert.ToDecimal(txtItemCost.Text) : 0;
            objItem.ChargedCost = !string.IsNullOrWhiteSpace(txtChargePrice.Text) ? Convert.ToDecimal(txtChargePrice.Text) : 0;
            objItem.InStock = !string.IsNullOrWhiteSpace(txtStock.Text) ? Convert.ToInt32(txtStock.Text) : 0;

            objItem.VendorName = txtVendorName.Text;

            objItem.InCase = !string.IsNullOrWhiteSpace(txtCase.Text) ? Convert.ToInt32(Convert.ToDecimal(txtCase.Text)) : 0;
            objItem.CaseCost = !string.IsNullOrWhiteSpace(txtCaseCost.Text) ? Convert.ToDecimal(txtCaseCost.Text) : 0;
            objItem.SalesTaxAmt = !string.IsNullOrWhiteSpace(txtPriceWithTax.Text) ? Convert.ToDecimal(txtPriceWithTax.Text) : 0;

            objItem.Sales_Tax = txtchkST.IsChecked == true;
            //objItem.Sales_Tax_2 = txtchkST2.IsChecked == true;
            //objItem.Sales_Tax_3 = txtchkST3.IsChecked == true;
            //objItem.Sales_Tax_4 = txtchkST4.IsChecked == true;
            //objItem.Sales_Tax_5 = txtchkST5.IsChecked == true;
            //objItem.Sales_Tax_6 = txtchkST5.IsChecked == true;
            //objItem.Bar_Tax = txtchkBT.IsChecked == true;
            objItem.QuickADD = txtQuickAdd.IsChecked == true;

            objItem.BulkPricingItems = BulkPricingItems;

            if (objService.SaveItem(objItem))
            {
                MessageBox.Show("Item Saved Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                var mainWindow = (MainWindow)Application.Current.MainWindow;

                if (mainWindow != null)
                {
                    // Get the content inside the "Billing" TabItem (assuming it's a UserControl)
                    var billingControl = mainWindow.Billing.Content as Billing;

                    if (billingControl != null)
                    {
                        // Call the method inside Billing user control
                        billingControl.ReloadBillingData();
                    }
                    else
                    {
                        MessageBox.Show("Billing UserControl is not properly loaded.");
                    }
                }

                ClearFields();
            }
            else
            {
                MessageBox.Show("Some error occurred while saving this Item.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

        public void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }


        private void CalculateTax(decimal chargePrice, DataTable dtTax)
        {
            decimal baseAmount = chargePrice;  // Example base amount
            decimal totalTax = 0;
            decimal finalPrice = baseAmount;

            // Check each checkbox and add or subtract the corresponding tax
            if (txtchkST.IsChecked == true)
            {
                totalTax += GetTaxRate("Sales_Tax", dtTax);
            }
            // Check each checkbox and add or subtract the corresponding tax
            //if (txtchkST2.IsChecked == true)
            //{
            //    totalTax += GetTaxRate("Sales_Tax_2", dtTax);
            //}
            //// Check each checkbox and add or subtract the corresponding tax
            //if (txtchkST3.IsChecked == true)
            //{
            //    totalTax += GetTaxRate("Sales_Tax_3", dtTax);
            //}
            //// Check each checkbox and add or subtract the corresponding tax
            //if (txtchkST4.IsChecked == true)
            //{
            //    totalTax += GetTaxRate("Sales_Tax_4", dtTax);
            //}
            //// Check each checkbox and add or subtract the corresponding tax
            //if (txtchkST5.IsChecked == true)
            //{
            //    totalTax += GetTaxRate("Sales_Tax_5", dtTax);
            //}
            //// Check each checkbox and add or subtract the corresponding tax
            //if (txtchkST6.IsChecked == true)
            //{
            //    totalTax += GetTaxRate("Sales_Tax_6", dtTax);
            //}
            //// Check each checkbox and add or subtract the corresponding tax
            //if (txtchkBT.IsChecked == true)
            //{
            //    totalTax += GetTaxRate("Bar_Tax", dtTax);
            //}

            // Calculate final price after adding or subtracting tax
            finalPrice = baseAmount + (baseAmount * totalTax / 100);

            txtPriceWithTax.Text = finalPrice.ToString("F2");
        }

        private void txtchkST_Checked(object sender, RoutedEventArgs e)
        {
            // Ensure that the text in the textbox is a valid decimal number
            if (decimal.TryParse(txtChargePrice.Text, out decimal chargePrice))
            {
                // Recalculate the tax and price when the value changes
                CalculateTax(chargePrice, dtTax);
            }
            else
            {
                // Handle invalid input (e.g., show an error message or revert to previous value)
                txtChargePrice.Text = "0.00";  // Example default value if input is invalid
            }
        }

        // Function to get the tax rate from the DataTable by tax name
        private decimal GetTaxRate(string taxName, DataTable dtTax)
        {
            DataRow[] taxRows = dtTax.Select($"Type = '{taxName}'");
            if (taxRows.Length > 0)
            {
                return Convert.ToDecimal(taxRows[0]["Percentage"]);
            }
            return 0;
        }

        private void txtChargePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ensure that the text in the textbox is a valid decimal number
            if (decimal.TryParse(txtChargePrice.Text, out decimal chargePrice))
            {
                // Recalculate the tax and price when the value changes
                CalculateTax(chargePrice, dtTax);
            }
            else
            {
                // Handle invalid input (e.g., show an error message or revert to previous value)save
                txtChargePrice.Text = "";  // Example default value if input is invalid
            }

            CalculateProfitAndMargin();
        }

        private void btnCopyItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Item Copied.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            intItemID = 0;
        }

        private void UpdateProfitAndMargin(decimal profitPercentage, decimal grossMarginPercentage)
        {
            lblProfit.Content = $"{profitPercentage:F2}%";
            lblGrossMargin.Content = $"{grossMarginPercentage:F2}%";
        }

        // Example usage:
        private void CalculateProfitAndMargin()
        {
            decimal costPrice = decimal.TryParse(txtItemCost.Text, out decimal cost) ? cost : 0;
            decimal sellingPrice = decimal.TryParse(txtChargePrice.Text, out decimal price) ? price : 0;

            if (costPrice > 0 && sellingPrice > 0)
            {
                decimal profitPercentage = ((sellingPrice - costPrice) / costPrice) * 100;
                decimal grossMarginPercentage = ((sellingPrice - costPrice) / sellingPrice) * 100;

                UpdateProfitAndMargin(profitPercentage, grossMarginPercentage);
            }
            else
            {
                UpdateProfitAndMargin(0, 0);
            }
        }

        private void btnAddBulk_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new BulkPricing(BulkPricingItems, intItemID);
            if (addWindow.ShowDialog() == true)
            {
                dgBulkPricing.ItemsSource = null;
                dgBulkPricing.ItemsSource = BulkPricingItems;
            }
        }

        private void btnRemoveBulk_Click(object sender, RoutedEventArgs e)
        {
            // Remove the selected item from the list
            if (dgBulkPricing.SelectedItem is BulkPricingItem selectedItem)
            {
                BulkPricingItems.Remove(selectedItem);

                dgBulkPricing.ItemsSource = null;
                dgBulkPricing.ItemsSource = BulkPricingItems;
            }
        }

        private void btnSaveBulk_Click(object sender, RoutedEventArgs e)
        {
            // Save the bulk pricing data
        }

        // Quantity: Allow only whole numbers
        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void Price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"[\d.]");
        }
    }
    public class ComboBoxItem
    {
        public string Description { get; set; }
        public string Value { get; set; }

        // Constructor
        public ComboBoxItem(string description, string value)
        {
            Description = description;
            Value = value;
        }

        // Override ToString to display Description in ComboBox
        public override string ToString()
        {
            return Description;  // The Description is what will be shown in the ComboBox
        }
    }

}
