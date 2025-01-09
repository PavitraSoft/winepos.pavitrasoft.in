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
using WinePOSFinal.ServicesLayer;
using System.Data;
using WinePOSFinal.Classes;

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
        public InventoryMaintenance()
        {
            InitializeComponent();

            ReloadInventoryMaintenanceData();

            BindDropdown();
        }


        public void ReloadInventoryMaintenanceData()
        {
            dtTax = objService.GetTaxData();

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
            txtQuickAdd.IsChecked = objItem.QuickADD;
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

            objItem.InCase = !string.IsNullOrWhiteSpace(txtCase.Text) ? Convert.ToInt32(txtCase.Text) : 0;
            objItem.CaseCost = !string.IsNullOrWhiteSpace(txtCaseCost.Text) ? Convert.ToDecimal(txtCaseCost.Text) : 0;
            objItem.SalesTaxAmt = !string.IsNullOrWhiteSpace(txtPriceWithTax.Text) ? Convert.ToDecimal(txtPriceWithTax.Text) : 0;

            objItem.Sales_Tax = txtchkST.IsChecked == true;
            objItem.QuickADD = txtQuickAdd.IsChecked == true;

            if (objService.SaveItem(objItem))
            {
                MessageBox.Show("Item Saved Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);



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
                // Handle invalid input (e.g., show an error message or revert to previous value)
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

            if (costPrice > 0)
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
