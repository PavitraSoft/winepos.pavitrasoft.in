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
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Data.SqlClient;
using WinePOSFinal.UserControls;
using System.IO;
using Path = System.IO.Path;

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


        private Stopwatch stopwatch = new Stopwatch();
        private string inputBuffer = string.Empty;
        private bool isScanning = false;
        private int invoiceNumber = 0;

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
            ReloadBillingData();

        }

        public void ReloadBillingData()
        {
            txtQuantity.Text = "1";
            objBillingItems.CollectionChanged += (s, e) => CalculateTotals();

            DataContext = this;

            FetchAndPopulateDataTable();

            string currentRole = AccessRightsManager.GetUserRole();

            if (currentRole.ToUpper() != "ADMIN")
            {
                // Toggle visibility of the header textbox
                textDiscount.Visibility = Visibility.Collapsed;
                txtDiscountValue.Visibility = Visibility.Collapsed;
                btnApplyDiscount.Visibility = Visibility.Collapsed;

                // Toggle visibility of the Discount column
                var discountColumn = dgBilling.Columns.FirstOrDefault(c => c.Header.ToString() == "Discount (%)");
                if (discountColumn != null)
                {
                    discountColumn.Visibility = Visibility.Collapsed;
                }
            }

        }

        private void FetchAndPopulateDataTable()
        {
            dtAllItems = objService.GetInventoryData(string.Empty, string.Empty);
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
            int CurrentQuantity = 0;
            string strTotalPrice = string.Empty;
            string strDiscount = string.Empty;

            DataRow[] dr = dtAllItems.Select(" UPC = '" + strUPC + "'");


            if (dr != null && dr.Count() > 0)
            {
                strName = Convert.ToString(dr[0]["Description"]);
                strPrice = Convert.ToString(dr[0]["ChargedCost"]);
                CurrentQuantity = Convert.ToInt32(dr[0]["Stock"]);
                strQuantity = txtQuantity.Text;

                // Calculate total price (for this example, assuming price and quantity are numeric)
                if (decimal.TryParse(strPrice, out decimal parsedPrice) && int.TryParse(strQuantity, out int parsedQuantity))
                {
                    // Check if the item already exists in the ObservableCollection
                    var existingItem = objBillingItems.FirstOrDefault(item => item.UPC == strUPC);
                    if (existingItem != null)
                    {
                        // Update the quantity of the existing item
                        int newQuantity = Convert.ToInt32(existingItem.Quantity) + parsedQuantity;

                        if (CurrentQuantity >= newQuantity)
                        {
                            decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
                            decimal taxedPrice = parsedPrice + tax;
                            existingItem.Quantity = Convert.ToString(newQuantity);
                            existingItem.TotalPrice = (taxedPrice * newQuantity).ToString("F2");

                            // Clear the TextBox controls for new input
                            txtUPC.Clear();
                            txtName.Clear();
                            txtQuantity.Text = "1";
                        }
                        else
                        {
                            MessageBox.Show($"Asked Quantity: {newQuantity} Current Quantity: {CurrentQuantity}.");
                        }
                    }
                    else
                    {
                        if (CurrentQuantity >= parsedQuantity)
                        {
                            decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
                            decimal taxedPrice = parsedPrice + tax;
                            decimal totalPrice = taxedPrice * parsedQuantity;

                            // Create a new BillingItem
                            BillingItem newItem = new BillingItem
                            {
                                UPC = strUPC,
                                Name = strName,
                                Price = strPrice,
                                Quantity = Convert.ToString(parsedQuantity),
                                Tax = tax.ToString("F2"), // Format total price as a string with 2 decimals
                                Discount = "0",
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
                        else
                        {
                            MessageBox.Show($"Asked Quantity: {parsedQuantity} Current Quantity: {CurrentQuantity}.");
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Please enter valid UPC and quantity.");
                }
                dgBilling.ItemsSource = null;
                dgBilling.ItemsSource = objBillingItems;
            }
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
                if (SaveInvoice(objBillingItems, false, "PALMPAY"))
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
                if (SaveInvoice(objBillingItems, true, "CASH"))
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
            if (invoiceNumber != 0)
            {
                try
                {
                    // Create a new report document
                    ReportDocument report = new ReportDocument();

                    // Load the report (winebill.rpt)
                    //string reportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Reports\winebill.rpt");
                    //string reportPath = System.IO.Path.Combine(@"D:\Study\Dotnet\WinePOSGIT\winepos.pavitrasoft.in\WinePOSAppSolution\WinePOSFinal\Reports\winebill.rpt");

                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    // Target file
                    string targetFile = Path.Combine("Reports", "winebill.rpt");

                    // Combine base directory with the relative path
                    string reportPath = Path.Combine(baseDirectory, targetFile);
                    report.Load(reportPath);

                    // Create and populate the DataTable
                    //DataTable dt = objService.GetInventoryData(string.Empty, string.Empty);

                    // Set the DataTable as the data source for the report
                    //report.SetDataSource(dt);

                    // Set database logon credentials (if required)
                    SetDatabaseLogin(report);

                    // Dynamically set the InvoiceCode parameter for the report
                    report.SetParameterValue("InvoiceCode", invoiceNumber);

                    // Export the report to a PDF file
                    string exportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "WineBill.pdf");
                    report.ExportToDisk(ExportFormatType.PortableDocFormat, exportPath);

                    // Display the PDF in the WebBrowser control
                    //pdfWebViewer.Navigate(exportPath); // Navigate to the generated PDF file


                    // Optionally, open the generated report in a PDF viewer
                    System.Diagnostics.Process.Start(exportPath);

                    //MessageBox.Show("Report generated and displayed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please make payment first to print invoice.", "Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


        private void SetDatabaseLogin(ReportDocument report)
        {
            // Set the database login credentials
            try
            {
                // Retrieve the connection string from the app.config file
                string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;

                // Create an instance of SqlConnectionStringBuilder to parse the connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);

                // Extract the individual components from the connection string
                string server = builder.DataSource;
                string database = builder.InitialCatalog;
                string username = builder.UserID;
                string password = builder.Password;

                // Get the database logon info from the report's database
                ConnectionInfo connectionInfo = new ConnectionInfo
                {
                    ServerName = server,
                    DatabaseName = database,
                    UserID = username,
                    Password = password
                };

                // Apply the connection info to the report
                ApplyLogonToSubreports(report, connectionInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting database logon: {ex.Message}");
            }
        }

        private void ApplyLogonToSubreports(ReportDocument report, ConnectionInfo connectionInfo)
        {
            // Set the connection information for the main report
            report.DataSourceConnections[0].SetConnection(connectionInfo.ServerName, connectionInfo.DatabaseName, false);
            report.DataSourceConnections[0].SetLogon(connectionInfo.UserID, connectionInfo.Password);

            // Apply the connection info to any subreports as well
            foreach (ReportDocument subReport in report.Subreports)
            {
                subReport.DataSourceConnections[0].SetConnection(connectionInfo.ServerName, connectionInfo.DatabaseName, false);
                subReport.DataSourceConnections[0].SetLogon(connectionInfo.UserID, connectionInfo.Password);
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

        private bool SaveInvoice(ObservableCollection<BillingItem> objBilling, bool IsVoidInvoice, string PaymentType)
        {
            try
            {
                invoiceNumber = 0;
                foreach (BillingItem bi in objBilling)
                {
                    objService.SaveInvoice(bi, IsVoidInvoice, PaymentType, ref invoiceNumber);
                }

                SendLowQuantityMail();

                dtAllItems = objService.GetInventoryData(string.Empty, string.Empty);


                return true;
            }
            catch
            {
                return false;
            }

        }

        private void SendLowQuantityMail()
        {
            try
            {
                DataTable dtEmailDetails = objService.GetLowQuentityEmailDetails();

                if (dtEmailDetails != null && dtEmailDetails.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtEmailDetails.Rows)
                    {
                        SendEmail(Convert.ToString(dr["smtpUser"]), Convert.ToString(dr["smtpPassword"]), Convert.ToString(dr["ToMail"]), Convert.ToString(dr["Subject"]), Convert.ToString(dr["Body"]));

                        objService.UpdateSentEmailDetail(Convert.ToInt32(dr["ID"]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while sending low quantity email: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SendEmail(string smtpUser, string smtpPassword, string toEmail, string subject, string body)
        {
            string smtpHost = "smtp.gmail.com"; // e.g., "smtp.gmail.com"
            int smtpPort = 587; // Usually 587 for TLS or 465 for SSL

            // Create the email
            MailMessage mailMessage = new MailMessage(smtpUser, toEmail, subject, body);
            mailMessage.IsBodyHtml = false; // Set to true if you include HTML in the email body

            // Configure the SMTP client
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true // Use SSL/TLS encryption
            };

            // Send the email
            smtpClient.Send(mailMessage);
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
                if (SaveInvoice(objBillingItems, true, "CHECK"))
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
                if (SaveInvoice(objBillingItems, true, "CREDIT"))
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


        private void txtUPC_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtUPC.Text;

            // Check if scanner prefix or suffix exists
            if (text.StartsWith("@") && text.EndsWith("\r"))
            {
                isScanning = true;
                text = text.Trim('@', '\r', '\n');
                HandleScannedInput(text);
                return;
            }

            // Use a timer to detect bulk input
            if (isScanning && stopwatch.ElapsedMilliseconds < 500) // Adjust bulk input threshold
            {
                HandleScannedInput(text);
            }
            else
            {
                HandleManualInput(text);
            }
        }

        private void HandleScannedInput(string barcode)
        {
            string strItemName = GetMatchedItem(txtUPC.Text);

            txtName.Text = strItemName;


            btnAdd_Click(null, null);
        }

        private void HandleManualInput(string text)
        {
            // Logic for manual input

            string strItemName = GetMatchedItem(txtUPC.Text);

            txtName.Text = strItemName;
        }

        private void txtUPC_KeyDown(object sender, KeyEventArgs e)
        {
            string text = txtUPC.Text;

            // Check if scanner prefix or suffix exists
            if (text.StartsWith("@") && text.EndsWith("\r"))
            {
                isScanning = true;
                text = text.Trim('@', '\r', '\n');
                HandleScannedInput(text);
                return;
            }

            // Use a timer to detect bulk input
            if (isScanning && stopwatch.ElapsedMilliseconds < 500) // Adjust bulk input threshold
            {
                HandleScannedInput(text);
            }
            else
            {
                HandleManualInput(text);
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = txtName.Text;

            if (!string.IsNullOrWhiteSpace(query))
            {
                var filteredSuggestions = dtAllItems.AsEnumerable()
                    .Where(row => row.Field<string>("Description").StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    .Select(row => $"{row.Field<string>("Description")} - {row.Field<string>("UPC")}")
                    .ToList();

                if (filteredSuggestions.Any())
                {
                    lstNameSuggestions.ItemsSource = filteredSuggestions;
                    NameSuggestionsPopup.IsOpen = true;
                }
                else
                {
                    NameSuggestionsPopup.IsOpen = false;
                }
            }
            else
            {
                NameSuggestionsPopup.IsOpen = false;
            }
        }

        private void lstNameSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstNameSuggestions.SelectedItem is string selectedItem)
            {
                // Extract only the Name part
                var namePart = selectedItem.Split('-')[0].Trim();
                var upcPart = selectedItem.Split('-')[1].Trim();
                txtName.Text = namePart;
                txtUPC.Text = upcPart;

                // Close Popup and clear selection
                NameSuggestionsPopup.IsOpen = false;
                lstNameSuggestions.SelectedItem = null;
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (NameSuggestionsPopup.IsOpen)
            {
                if (e.Key == Key.Down)
                {
                    lstNameSuggestions.Focus();
                    lstNameSuggestions.SelectedIndex = 0;
                }
                else if (e.Key == Key.Escape)
                {
                    NameSuggestionsPopup.IsOpen = false;
                }
            }
        }

        private void btnApplyDiscount_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtDiscountValue.Text, out decimal discount))
            {
                if (discount < 0 || discount >= 100)
                {
                    MessageBox.Show("Discount must be a number less than 100.", "Invalid Discount", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDiscountValue.Text = "0"; // Reset to 0
                    return;
                }

                foreach (BillingItem billingItem in objBillingItems)
                {
                    if (Convert.ToDecimal(billingItem.Discount) <= 0)
                    {
                        billingItem.Discount = Convert.ToString(discount);

                        // Recalculate the Total Price
                        billingItem.TotalPrice = Convert.ToString(Convert.ToDecimal(billingItem.Price) * Convert.ToInt32(billingItem.Quantity) * (1 - Convert.ToDecimal(billingItem.Discount) / 100));
                    }
                }

                // Refresh the DataGrid to reflect changes
                CalculateTotals();
                dgBilling.ItemsSource = null;
                dgBilling.ItemsSource = objBillingItems;
                //dgBilling.Items.Refresh();
            }
        }

        private void txtDiscountValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            // Check if the input text is numeric
            return int.TryParse(text, out _);
        }

        private void dgBilling_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Check if the edited item is of type BillingItem
            if (e.Row.Item is BillingItem editedItem)
            {
                // Check if the edited column is the Discount column
                if (e.Column.Header.ToString() == "Discount (%)")
                {
                    // Extract the editing value from the TextBox
                    var editingElement = e.EditingElement as TextBox;
                    if (editingElement != null && decimal.TryParse(editingElement.Text, out decimal discount))
                    {
                        if (discount < 0 || discount >= 100)
                        {
                            // Show a validation message and reset the discount
                            //MessageBox.Show("Discount must be a number less than 100.", "Invalid Discount", MessageBoxButton.OK, MessageBoxImage.Warning);
                            editingElement.Text = editedItem.Discount.ToString(); // Reset to the original value
                            return;
                        }

                        // Update the item's Discount and recalculate TotalPrice
                        editedItem.Discount = Convert.ToString(discount);
                        decimal originalPrice = Convert.ToDecimal(editedItem.Price) * Convert.ToInt32(editedItem.Quantity);
                        editedItem.TotalPrice = Convert.ToString(originalPrice * (1 - discount / 100));

                        // Refresh the grid (not strictly necessary if binding is set up correctly
                        CalculateTotals();
                        dgBilling.ItemsSource = null;
                        dgBilling.ItemsSource = objBillingItems;
                    }
                }
            }

            //// Get the edited row's data
            //var editedItem = e.Row.Item as BillingItem;
            //if (editedItem == null) return;

            //// Get the new value from the editor
            //var editingElement = e.EditingElement as TextBox;
            //if (editingElement != null && decimal.TryParse(editingElement.Text, out decimal discount))
            //{
            //    if (discount < 0 || discount >= 100)
            //    {
            //        MessageBox.Show("Discount must be a number less than 100.", "Invalid Discount", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        editingElement.Text = "0"; // Reset to default value
            //        return;
            //    }

            //    // Update TotalPrice based on discount
            //    decimal originalPrice = Convert.ToDecimal(editedItem.Price) * Convert.ToInt32(editedItem.Quantity);
            //    editedItem.TotalPrice = Convert.ToString(originalPrice - (originalPrice * discount / 100));

            //    // Refresh the DataGrid to display the updated value
            //    dgBilling.Items.Refresh();
            //}
        }

        private void dgBilling_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers and control keys
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
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
