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
        public InventoryMaintenance()
        {
            InitializeComponent();

            BindDropdown();

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

            txtItemCost.Text = "0";
            txtChargePrice.Text = "0";
            txtPriceWithTax.Text = "0";
            txtStock.Text = "0";

            txtchkST.IsChecked = false;
            txtchkST2.IsChecked = false;
            txtchkST3.IsChecked = false;
            txtchkST4.IsChecked = false;
            txtchkST5.IsChecked = false;
            txtchkST6.IsChecked = false;
            txtchkBT.IsChecked = false;
            intItemID = 0;

        }

        private void FetchAndPopulateItemData()
        {
            Items objItem =  objService.FetchItemDataByID(intItemID);


            ComboBoxItem selectedItem = cbCategory.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Value == objItem.Category); ;
            
            cbCategory.SelectedItem = selectedItem;
            txtUPC.Text = !string.IsNullOrWhiteSpace(objItem.UPC) ? objItem.UPC : string.Empty;


            txtDescription.Text = !string.IsNullOrWhiteSpace(objItem.Name) ? objItem.Name : string.Empty;
            txtADescription.Text = !string.IsNullOrWhiteSpace(objItem.Additional_Description) ? objItem.Name : string.Empty;

            txtItemCost.Text = Convert.ToString(objItem.itemcost);
            txtChargePrice.Text = Convert.ToString(objItem.ChargedCost);
            txtStock.Text = Convert.ToString(objItem.InStock);

            txtchkST.IsChecked = objItem.Sales_Tax;
            txtchkST2.IsChecked = objItem.Sales_Tax_2;
            txtchkST3.IsChecked = objItem.Sales_Tax_3;
            txtchkST4.IsChecked = objItem.Sales_Tax_4;
            txtchkST5.IsChecked = objItem.Sales_Tax_5;
            txtchkST6.IsChecked = objItem.Sales_Tax_6;
            txtchkBT.IsChecked = objItem.Bar_Tax;
        }

        List<ComboBoxItem> ConvertDataTableToComboBoxItems(DataTable dt)
        {
            List<ComboBoxItem> comboBoxItems = new List<ComboBoxItem>();

            foreach (DataRow row in dt.Rows)
            {
                // Create a new ComboBoxItem with the Code and Description from DataTable
                ComboBoxItem item = new ComboBoxItem(Convert.ToString(row["Description"]), Convert.ToInt32(row["Code"]));
                comboBoxItems.Add(item);
            }

            return comboBoxItems;
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Items objItem = new Items();

            objItem.ItemID = intItemID;
            ComboBoxItem selectedItem = (ComboBoxItem)cbCategory.SelectedItem;

            objItem.Category = selectedItem.Value;
            objItem.UPC = txtUPC.Text;
            objItem.Name = txtDescription.Text;
            objItem.Additional_Description = txtADescription.Text;

            objItem.ItemCost = Convert.ToDecimal(txtItemCost.Text);
            objItem.ChargedCost = Convert.ToDecimal(txtChargePrice.Text);
            objItem.InStock = Convert.ToInt32(txtStock.Text);

            objItem.Sales_Tax = txtchkST.IsChecked == true;
            objItem.Sales_Tax_2 = txtchkST2.IsChecked == true;
            objItem.Sales_Tax_3 = txtchkST3.IsChecked == true;
            objItem.Sales_Tax_4 = txtchkST4.IsChecked == true;
            objItem.Sales_Tax_5 = txtchkST5.IsChecked == true;
            objItem.Sales_Tax_6 = txtchkST6.IsChecked == true;
            objItem.Bar_Tax = txtchkBT.IsChecked == true;

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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }
    public class ComboBoxItem
    {
        public string Description { get; set; }
        public int Value { get; set; }

        // Constructor
        public ComboBoxItem(string description, int value)
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
