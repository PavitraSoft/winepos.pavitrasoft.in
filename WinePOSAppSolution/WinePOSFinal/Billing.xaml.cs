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
using System.Data;
using WinePOSFinal.ServicesLayer;
using System.ComponentModel;
using WinePOSFinal.Classes;
using System.Collections.ObjectModel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace WinePOSFinal
{

    /// <summary>
    /// Interaction logic for Billing.xaml
    /// </summary>
    public partial class Billing : UserControl, INotifyPropertyChanged
    {
        DataTable dtAllItems = new DataTable();
        DataTable dtTax = new DataTable();

        WinePOSService objService = new WinePOSService();
        ObservableCollection<BillingItem> objBillingItems = new ObservableCollection<BillingItem>();

        private decimal _subTotal;
        private decimal _tax;
        private decimal _grandTotal;

        public decimal SubTotal
        {
            get => _subTotal;
            set
            {
                _subTotal = value;
                OnPropertyChanged(nameof(SubTotal));
            }
        }

        public decimal Tax
        {
            get => _tax;
            set
            {
                _tax = value;
                OnPropertyChanged(nameof(Tax));
            }
        }

        public decimal GrandTotal
        {
            get => _grandTotal;
            set
            {
                _grandTotal = value;
                OnPropertyChanged(nameof(GrandTotal));
            }
        }

        public Billing()
        {
            InitializeComponent();
            txtQuantity.Text = "1"; 
            objBillingItems.CollectionChanged += (s, e) => CalculateTotals();

            DataContext = this;

            FetchAndPopulateDataTable();
        }

        private void FetchAndPopulateDataTable()
        {
            dtAllItems = objService.GetInventoryData(string.Empty);
            if (dtAllItems.Rows.Count > 0)
            {
                DataRow[] dr = dtAllItems.Select(" QuickADD = 1");

                cbQuickADD.SelectedIndex = 0;
                if (dr.Count() > 0)
                {
                    DataTable dtData = objService.GetIMDropdownData();

                    List<ComboBoxItem> cbItems = ConvertDataTableToComboBoxItems(dr.CopyToDataTable());


                    cbQuickADD.ItemsSource = cbItems;
                }
            }
            dtTax = objService.GetTaxData();
        }

        List<ComboBoxItem> ConvertDataTableToComboBoxItems(DataTable dt)
        {
            List<ComboBoxItem> comboBoxItems = new List<ComboBoxItem>();

            foreach (DataRow row in dt.Rows)
            {
                // Create a new ComboBoxItem with the Code and Description from DataTable
                ComboBoxItem item = new ComboBoxItem(Convert.ToString(row["Description"]), Convert.ToString(row["UPC"]));
                comboBoxItems.Add(item);
            }

            return comboBoxItems;
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // Check if the input is a positive integer
            if (!IsPositiveInteger(textBox.Text))
            {
                // If the input is not valid, remove the last character (undo the invalid input)
                textBox.Text = string.Join("", textBox.Text.Where(c => Char.IsDigit(c)));

                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "1";
                }

                // Reset the caret position to the end of the text
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private bool IsPositiveInteger(string input)
        {
            // Try to parse the input as an integer and check if it's positive
            return int.TryParse(input, out int result) && result > 0;
        }

        private void txtUPC_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strItemName = GetMatchedItem(txtUPC.Text);

            txtName.Text = strItemName;
        }

        private string GetMatchedItem(string UPC)
        {
            string strItemName = string.Empty;
            if (!string.IsNullOrWhiteSpace(UPC))
            {
                DataRow[] dr = dtAllItems.Select(" UPC = '" + UPC + "'");

                if (dr != null && dr.Count() > 0)
                {
                    strItemName = Convert.ToString(dr[0]["Description"]);
                }
            }

            return strItemName;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string strUPC = txtUPC.Text;

            string strName = string.Empty;
            string strPrice = string.Empty;
            string strQuantity = string.Empty;
            string strTotalPrice = string.Empty;

            DataRow[] dr = dtAllItems.Select(" UPC = '" + strUPC + "'");


            if (dr != null && dr.Count() > 0)
            {
                strName = Convert.ToString(dr[0]["Description"]);
                strPrice = Convert.ToString(dr[0]["ChargedCost"]);
                strQuantity = txtQuantity.Text;

                // Calculate total price (for this example, assuming price and quantity are numeric)
                if (decimal.TryParse(strPrice, out decimal parsedPrice) && int.TryParse(strQuantity, out int parsedQuantity))
                {
                    decimal tax = CalculatePriceAfterTax(Convert.ToDecimal(strPrice), dr[0], dtTax);
                    decimal taxedPrice = parsedPrice + tax;
                    decimal totalPrice = (taxedPrice * parsedQuantity);

                    // Create a new BillingItem
                    BillingItem newItem = new BillingItem
                    {
                        UPC = strUPC,
                        Name = strName,
                        Price = strPrice,
                        Quantity = strQuantity,
                        Tax = tax.ToString("F2"), // Format total price as a string with 2 decimals
                        TotalPrice = totalPrice.ToString("F2"), // Format total price as a string with 2 decimals
                        UserName = AccessRightsManager.GetUserName()
                    };

                    // Add the new item to the ObservableCollection
                    objBillingItems.Add(newItem);

                    // Clear the TextBox controls for new input
                    txtUPC.Clear();
                    txtName.Clear();
                    txtQuantity.Text = "1";
                }
            }
            else
            {
                MessageBox.Show("Please enter valid UPC and quantity.");
            }

            dgBilling.ItemsSource = objBillingItems;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strItemName = GetMatchedItem(txtUPC.Text);

            if (!string.IsNullOrWhiteSpace(strItemName))
            {
                txtName.Text = strItemName;
            }
            else
            {
                MessageBox.Show("Please enter valid UPC.");
            }

        }

        private void btnPalmPay_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               $"Are you sure you want to Pay the Current Billing Invoice?",
               "Confirm Payment",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // Handle user response
            if (result == MessageBoxResult.Yes)
            {
                if (SaveInvoice(objBillingItems, false))
                {
                    MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Optionally, clear the DataGrid after payment
                    objBillingItems.Clear();
                }
                else
                {
                    MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCash_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               $"Are you sure you want to Cash the Current Billing Invoice?",
               "Confirm Payment",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // Handle user response
            if (result == MessageBoxResult.Yes)
            {
                if (SaveInvoice(objBillingItems, true))
                {
                    MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Optionally, clear the DataGrid after payment
                    objBillingItems.Clear();
                }
                else
                {
                    MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnVoidInvoice_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               $"Are you sure you want to void current Billing Invoice?",
               "Confirm Payment",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // Handle user response
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Optionally, clear the DataGrid after payment
                objBillingItems.Clear();
            }
            else
            {
                MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnPrintInvoice_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               $"The total payment amount is . Do you want to proceed?",
               "Confirm Payment",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // Handle user response
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Optionally, clear the DataGrid after payment
                objBillingItems.Clear();
            }
            else
            {
                MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            var selectedItem = dgBilling.SelectedItem as BillingItem;

            if (selectedItem != null)
            {
                // Remove the selected item from the collection
                objBillingItems.Remove(selectedItem);
            }
            else
            {
                MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CalculateTotals()
        {
            if (objBillingItems.Count > 0)
            {
                SubTotal = objBillingItems.Sum(item => decimal.TryParse(item.TotalPrice, out var totalPrice) ? totalPrice : 0);
                Tax = SubTotal * 0.10m; // Assuming 10% tax
                GrandTotal = SubTotal + Tax;
            }
            else
            {
                SubTotal = 0;
                Tax = 0; // Assuming 10% tax
                GrandTotal = 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SaveInvoice(ObservableCollection<BillingItem> objBilling, bool IsVoidInvoice)
        {
            try
            {
                foreach (BillingItem bi in objBilling)
                {
                    objService.SaveInvoice(bi, IsVoidInvoice);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static decimal CalculatePriceAfterTax(decimal amount, DataRow taxColumns, DataTable taxRates)
        {
            decimal finalAmount = amount;

            // Iterate through the tax columns to check which taxes are applicable
            foreach (DataColumn column in taxColumns.Table.Columns)
            {
                if (taxColumns[column.ColumnName] is bool isTaxApplicable && isTaxApplicable)
                {
                    // Find the corresponding tax rate in the taxRates table
                    DataRow[] taxRateRow = taxRates.Select($"Type = '{column.ColumnName}'");
                    if (taxRateRow.Length > 0 && decimal.TryParse(taxRateRow[0]["Percentage"].ToString(), out decimal taxRate))
                    {
                        // Apply the tax rate
                        finalAmount += (amount * taxRate / 100);
                    }
                }
            }

            return finalAmount;
        }

        private void btnQuickAdd_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)cbQuickADD.SelectedItem;
            if (selectedItem != null)
            {
                txtUPC.Text = selectedItem.Value;
                btnAdd_Click(null, null);
            }
            else
            {
                MessageBox.Show("Please select an item to add.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               $"Are you sure you want to Check the Current Billing Invoice?",
               "Confirm Payment",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // Handle user response
            if (result == MessageBoxResult.Yes)
            {
                if (SaveInvoice(objBillingItems, true))
                {
                    MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Optionally, clear the DataGrid after payment
                    objBillingItems.Clear();
                }
                else
                {
                    MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCredit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               $"Are you sure you want to Credit the Current Billing Invoice?",
               "Confirm Payment",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // Handle user response
            if (result == MessageBoxResult.Yes)
            {
                if (SaveInvoice(objBillingItems, true))
                {
                    MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Optionally, clear the DataGrid after payment
                    objBillingItems.Clear();
                }
                else
                {
                    MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    //public class ComboBoxItem
    //{
    //    public string Description { get; set; }
    //    public string Value { get; set; }

    //    // Constructor
    //    public ComboBoxItem(string description, string value)
    //    {
    //        Description = description;
    //        Value = value;
    //    }

    //    // Override ToString to display Description in ComboBox
    //    public override string ToString()
    //    {
    //        return Description;  // The Description is what will be shown in the ComboBox
    //    }
    //}
}
