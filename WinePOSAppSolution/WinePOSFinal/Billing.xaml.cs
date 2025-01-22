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
using System.IO.Ports;
using Microsoft.PointOfService;
using CrystalDecisions.Windows.Forms;

namespace WinePOSFinal
{

    /// <summary>
    /// Interaction logic for Billing.xaml
    /// </summary>
    public partial class Billing : UserControl, INotifyPropertyChanged
    {
        DataTable dtAllItems = new DataTable();
        DataTable dtTax = new DataTable();
        DataTable dtBulkPricing = new DataTable();


        private PosExplorer explorer;

        private CashDrawer cashDrawer;

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

            InitializeCashDrawer();
            ReloadBillingData();

        }

        private void InitializeCashDrawer()

        {

            try

            {

                explorer = new PosExplorer();

                DeviceInfo deviceInfo = explorer.GetDevice(DeviceType.CashDrawer, "Tera");

                cashDrawer = (CashDrawer)explorer.CreateInstance(deviceInfo);

                cashDrawer.Open();

                cashDrawer.Claim(1000);

                cashDrawer.DeviceEnabled = true;

            }

            catch (Exception ex)

            {

                MessageBox.Show("Error initializing cash drawer: " + ex.Message);

            }

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
            //if (dtAllItems.Rows.Count > 0)
            //{
            //    DataRow[] dr = dtAllItems.Select(" QuickADD = 1");

            //    //cbQuickADD.SelectedIndex = 0;
            //    if (dr.Count() > 0)
            //    {
            //        DataTable dtData = objService.GetIMDropdownData();

            //        List<ComboBoxItem> cbItems = ConvertDataTableToComboBoxItems(dr.CopyToDataTable());


            //        cbQuickADD.ItemsSource = cbItems;
            //    }
            //}
            dtTax = objService.GetTaxData();
            dtBulkPricing = objService.GetBulkPricingData();
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
            int ItemID = 0;
            string strTotalPrice = string.Empty;
            string strDiscount = string.Empty;

            DataRow[] dr = dtAllItems.Select(" UPC = '" + strUPC + "'");


            if (dr != null && dr.Count() > 0)
            {
                strName = Convert.ToString(dr[0]["Description"]);
                strPrice = Convert.ToString(dr[0]["ChargedCost"]);
                CurrentQuantity = Convert.ToInt32(dr[0]["Stock"]);
                ItemID = Convert.ToInt32(dr[0]["ItemID"]);
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

                        DataRow[] dataRow = dtBulkPricing.Select(" UPC = " + strUPC + " AND " + newQuantity + " % Quantity = 0");
                        string strNote = string.Empty;

                        if (dataRow.Any())
                        {
                            parsedPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                            string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                            strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);

                            parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                        }


                         if (CurrentQuantity >= newQuantity)
                        //if (true)
                        {

                            decimal discount = Convert.ToDecimal(existingItem.Discount);

                            parsedPrice = parsedPrice * (1 - discount / 100);

                            decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
                            //decimal taxedPrice = parsedPrice + tax;
                            decimal taxedPrice = tax;
                            existingItem.Price = Convert.ToString(parsedPrice);
                            existingItem.Tax = (tax - parsedPrice).ToString();
                            existingItem.Quantity = Convert.ToString(newQuantity);
                            existingItem.Discount = Convert.ToString(discount);
                            existingItem.TotalPrice = (taxedPrice * newQuantity).ToString("F2");
                            existingItem.Note = strNote;
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
                        //if (true)
                        {
                            DataRow[] dataRow = dtBulkPricing.Select(" UPC = " + strUPC + " AND " + parsedQuantity + " % Quantity = 0");
                            string strNote = string.Empty;

                            if (dataRow.Any())
                            {
                                parsedPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                                string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                                strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);


                                parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                            }

                            decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
                            //decimal taxedPrice = parsedPrice + tax;
                            decimal taxedPrice = tax;
                            decimal totalPrice = taxedPrice * parsedQuantity;




                            // Create a new BillingItem
                            BillingItem newItem = new BillingItem
                            {
                                UPC = strUPC,
                                Name = strName,
                                Price = Convert.ToString(parsedPrice),
                                Quantity = Convert.ToString(parsedQuantity),
                                Tax = (tax- parsedPrice).ToString("F2"), // Format total price as a string with 2 decimals
                                Discount = "0",
                                TotalPrice = totalPrice.ToString("F2"), // Format total price as a string with 2 decimals
                                UserName = AccessRightsManager.GetUserName(),
                                Note = strNote,
                                ItemID = Convert.ToString(ItemID),
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

                    CalculateTotals();

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

                    btnPrintInvoice_Click(null, null);
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
            try
            {


                btnTenderWindow_Click(null, null);
                //Open cash drawer
                //OpenCashDrawer();
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
                        // Optionally, clear the  after paymentDataGrid
                        objBillingItems.Clear();


                        btnPrintInvoice_Click(null, null);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error 1: {ex.Message}");
            }

            

        }

        private void btnVoidInvoice_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               $"Are you sure you want to clear current Billing Invoice?",
               "Confirm Payment",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // Handle user response
            if (result == MessageBoxResult.Yes)
            {
                Remaining.Visibility = Visibility.Collapsed;
                txtAmtRemaining.Visibility = Visibility.Collapsed;
                Change.Visibility = Visibility.Collapsed;
                txtAmtChange.Visibility = Visibility.Collapsed;

                // Optionally, clear the DataGrid after payment
                objBillingItems.Clear();

                //btnPrintInvoice_Click(null, null);

                MessageBox.Show("Invoice has been cleared. Thank you!", "Clear", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    string reportPath = System.IO.Path.Combine(@"D:\Study\Dotnet\WinePOSGIT\winepos.pavitrasoft.in\WinePOSAppSolution\WinePOSFinal\Reports\winebill.rpt");

                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    // Target file
                    string targetFile = Path.Combine("Reports", "winebill.rpt");

                    // Combine base directory with the relative path
                    //string reportPath = Path.Combine(baseDirectory, targetFile);
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
                SubTotal = objBillingItems.Sum(item => (decimal.TryParse(item.Price, out var totalPrice) ? totalPrice : 0) * Convert.ToInt32(item.Quantity));
                Tax = objBillingItems.Sum(item => decimal.TryParse(item.Tax, out var tax) ? (tax * Convert.ToInt32(item.Quantity)) : 0); // Assuming 10% tax
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

        //private void btnQuickAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    ComboBoxItem selectedItem = (ComboBoxItem)cbQuickADD.SelectedItem;
        //    if (selectedItem != null)
        //    {
        //        txtUPC.Text = selectedItem.Value;
        //        btnAdd_Click(null, null);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select an item to add.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

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

                    btnPrintInvoice_Click(null, null);
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

                    btnPrintInvoice_Click(null, null);
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
            if (text.Length == 10 || text.Length == 9 || text.Length == 6)
            {
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
            if (text.Length == 10 || text.Length == 9 || text.Length == 6)
            {
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

                        DataRow[] dr = dtAllItems.Select(" UPC = '" + billingItem.UPC + "'");

                        decimal parsedPrice = Convert.ToDecimal(dr[0]["ChargedCost"]);
                        int iQuantity = Convert.ToInt32(billingItem.Quantity);

                        parsedPrice = parsedPrice * (1 - discount / 100);

                        decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);

                        //decimal taxedPrice = parsedPrice + tax;
                        decimal taxedPrice = tax;
                        billingItem.Price = Convert.ToString(parsedPrice);
                        billingItem.Tax = (tax - parsedPrice).ToString();
                        billingItem.Discount = Convert.ToString(discount);
                        billingItem.TotalPrice = (taxedPrice * iQuantity).ToString("F2");
                    }
                }

                // Refresh the DataGrid to reflect changes
                dgBilling.ItemsSource = null;
                dgBilling.ItemsSource = objBillingItems;
                CalculateTotals();
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

                        DataRow[] dr = dtAllItems.Select(" UPC = '" + editedItem.UPC + "'");

                        decimal parsedPrice = Convert.ToDecimal(dr[0]["ChargedCost"]);
                        int iQuantity = Convert.ToInt32(editedItem.Quantity);

                        parsedPrice = parsedPrice * (1 - discount / 100);

                        decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);

                        //decimal taxedPrice = parsedPrice + tax;
                        decimal taxedPrice = tax;
                        editedItem.Price = Convert.ToString(parsedPrice);
                        editedItem.Tax = (tax - parsedPrice).ToString();
                        editedItem.Discount =Convert.ToString(discount);
                        editedItem.TotalPrice = (taxedPrice * iQuantity).ToString("F2");

                        // Refresh the grid (not strictly necessary if binding is set up correctly
                        dgBilling.ItemsSource = null;
                        dgBilling.ItemsSource = objBillingItems;
                        CalculateTotals();
                    }
                }
                else if (e.Column.Header.ToString() == "Price")
                {
                    // Extract the editing value from the TextBox
                    var editingElement = e.EditingElement as TextBox;
                    if (editingElement != null && decimal.TryParse(editingElement.Text, out decimal price))
                    {
                        if (price < 0)
                        {
                            editingElement.Text = editedItem.Price.ToString(); // Reset to the original value
                            return;
                        }

                        // Update the item's Discount and recalculate TotalPrice
                        editedItem.Price = Convert.ToString(price);
                        decimal originalPrice = Convert.ToDecimal(editedItem.Price) * Convert.ToInt32(editedItem.Quantity);
                        editedItem.TotalPrice = Convert.ToString(originalPrice * (1 - Convert.ToDecimal(editedItem.Discount) / 100));

                        // Refresh the grid (not strictly necessary if binding is set up correctly
                        CalculateTotals();
                        dgBilling.ItemsSource = null;
                        dgBilling.ItemsSource = objBillingItems;
                    }
                }
                //else if (e.Column.Header.ToString() == "Quantity")
                //{
                //    // Extract the editing value from the TextBox
                //    var editingElement = e.EditingElement as TextBox;
                //    if (editingElement != null && int.TryParse(editingElement.Text, out int quantity))
                //    {
                //        if (quantity > 0)
                //        {
                //            editingElement.Text = editedItem.Quantity.ToString(); // Reset to the original value
                //            return;
                //        }

                //        // Update the item's Discount and recalculate TotalPrice
                //        editedItem.Quantity = Convert.ToString(quantity);
                //        decimal originalPrice = Convert.ToDecimal(editedItem.Price) * Convert.ToInt32(editedItem.Quantity);
                //        editedItem.TotalPrice = Convert.ToString(originalPrice * (1 - Convert.ToDecimal(editedItem.Discount) / 100));

                //        // Refresh the grid (not strictly necessary if binding is set up correctly
                //        CalculateTotals();
                //        dgBilling.ItemsSource = null;
                //        dgBilling.ItemsSource = objBillingItems;
                //    }
                //}
            }
        }

        private void dgBilling_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers and control keys
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }

        private void btnTenderWindow_Click(object sender, RoutedEventArgs e)
        {
            // Pass the value to the TenderWindow
            TenderWindow tenderWindow = new TenderWindow(GrandTotal, Remaining, txtAmtRemaining, Change, txtAmtChange); // Pass 100 as the initial value
            tenderWindow.ShowDialog(); // Open the window modally
        }

        private void btnTaxConfig_Click(object sender, RoutedEventArgs e)
        {
            TaxWindow taxWindow = new TaxWindow();
            taxWindow.ShowDialog();
        }

        private void NonScanNoTax_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new QuickAdd(objBillingItems, 1,0, "NON SCAN NO TAX");
            if (addWindow.ShowDialog() == true)
            {
                CalculateTotals();
                dgBilling.ItemsSource = null;
                dgBilling.ItemsSource = objBillingItems;
            }
        }

        private void Nuts_Click(object sender, RoutedEventArgs e)
        {
            AddItemByName("NUTS");
            //var addWindow = new QuickAdd(objBillingItems, 1, Convert.ToDecimal(1.99), "NUTS");
            //if (addWindow.ShowDialog() == true)
            //{
            //    CalculateTotals();
            //    dgBilling.ItemsSource = null;
            //    dgBilling.ItemsSource = objBillingItems;
            //}
        }

        private void IceBag_Click(object sender, RoutedEventArgs e)
        {
            AddItemByName("ICE BAG");
            //var addWindow = new QuickAdd(objBillingItems, 1, Convert.ToDecimal(1.00), "ICE BAG");
            //if (addWindow.ShowDialog() == true)
            //{
            //    CalculateTotals();
            //    dgBilling.ItemsSource = null;
            //    dgBilling.ItemsSource = objBillingItems;
            //}
        }

        private void AddItemByName(string strItem)
        {
            string strUPC = string.Empty;

            string strName = string.Empty;
            string strPrice = string.Empty;
            string strQuantity = string.Empty;
            int CurrentQuantity = 0;
            int ItemID = 0;
            string strTotalPrice = string.Empty;
            string strDiscount = string.Empty;

            DataRow[] dr = dtAllItems.Select(" Description = '" + strItem + "'");


            if (dr != null && dr.Count() > 0)
            {
                strName = Convert.ToString(dr[0]["Description"]);
                strPrice = Convert.ToString(dr[0]["ChargedCost"]);
                CurrentQuantity = Convert.ToInt32(dr[0]["Stock"]);
                ItemID = Convert.ToInt32(dr[0]["ItemID"]);
                strUPC = Convert.ToString(dr[0]["UPC"]);
                strQuantity = txtQuantity.Text;

                // Calculate total price (for this example, assuming price and quantity are numeric)
                if (decimal.TryParse(strPrice, out decimal parsedPrice) && int.TryParse(strQuantity, out int parsedQuantity))
                {
                    // Check if the item already exists in the ObservableCollection
                    var existingItem = objBillingItems.FirstOrDefault(item => item.ItemID == Convert.ToString(ItemID));

                   
                    if (existingItem != null)
                    {
                        // Update the quantity of the existing item
                        int newQuantity = Convert.ToInt32(existingItem.Quantity) + parsedQuantity;


                        DataRow[] dataRow = dtBulkPricing.Select(" ItemID = " + Convert.ToString(ItemID) + " AND " + newQuantity + " % Quantity = 0");
                        string strNote = string.Empty;

                        if (dataRow.Any())
                        {
                            parsedPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                            string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                            strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);
                        }

                        if (CurrentQuantity >= newQuantity)
                        //if (true)
                        {
                            if (dataRow.Any())
                            {
                                parsedPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                                string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                                strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);


                                parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                            }

                            decimal discount = Convert.ToDecimal(existingItem.Discount);

                            parsedPrice = parsedPrice * (1 - discount / 100);

                            decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
                            //decimal taxedPrice = parsedPrice + tax;
                            decimal taxedPrice = tax;
                            existingItem.Price = Convert.ToString(parsedPrice);
                            existingItem.Tax = (tax - parsedPrice).ToString();
                            existingItem.Quantity = Convert.ToString(newQuantity);
                            existingItem.Discount = Convert.ToString(discount);
                            existingItem.TotalPrice = (taxedPrice * newQuantity).ToString("F2");
                            existingItem.Note = strNote;
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
                        //if (true)
                        {


                            DataRow[] dataRow = dtBulkPricing.Select(" ItemID = " + Convert.ToString(ItemID) + " AND " + parsedQuantity + " % Quantity = 0");
                            string strNote = string.Empty;

                            if (dataRow.Any())
                            {
                                parsedPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                                string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                                strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);


                                parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                            }

                            decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
                            //decimal taxedPrice = parsedPrice + tax;
                            decimal taxedPrice = tax;
                            decimal totalPrice = taxedPrice * parsedQuantity;

                            // Create a new BillingItem
                            BillingItem newItem = new BillingItem
                            {
                                UPC = strUPC,
                                Name = strName,
                                Price = Convert.ToString(parsedPrice),
                                Quantity = Convert.ToString(parsedQuantity),
                                Tax = (tax - parsedPrice).ToString("F2"), // Format total price as a string with 2 decimals
                                Discount = "0",
                                TotalPrice = totalPrice.ToString("F2"), // Format total price as a string with 2 decimals
                                UserName = AccessRightsManager.GetUserName(),
                                Note = strNote,
                                ItemID = Convert.ToString(ItemID),
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

                    CalculateTotals();

                }
                else
                {
                    MessageBox.Show("There is no item presnt with this name in Inventory.");
                }
                dgBilling.ItemsSource = null;
                dgBilling.ItemsSource = objBillingItems;
            }
        }

        private void OpenCashDrawer()
        {
            try
            {
                if (cashDrawer != null && cashDrawer.DeviceEnabled)
                {
                    cashDrawer.OpenDrawer();
                }
                else
                {
                    MessageBox.Show("Cash drawer not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening cash drawer: " + ex.Message);
            }
        }

        private void dgBilling_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dgBilling.SelectedItem is BillingItem selectedItem)
            {
                // Check if '>' or '<' keys are pressed
                if (e.Key == Key.OemPeriod || e.Key == Key.OemComma)
                {
                    string keyPressed = e.Key == Key.OemPeriod ? ">" : "<";

                    // Preserve the selected item and its index
                    int selectedIndex = dgBilling.SelectedIndex;

                    DataRow[] dr = dtAllItems.Select("UPC = '" + selectedItem.UPC + "'");
                    int iQuantity = Convert.ToInt32(selectedItem.Quantity);
                    decimal discount = Convert.ToDecimal(selectedItem.Discount);

                    if (keyPressed == ">")
                    {
                        iQuantity++;
                    }
                    else
                    {
                        iQuantity--;
                        if (iQuantity <= 0)
                        {
                            MessageBoxResult result = MessageBox.Show(
                                   $"Do you want to remove selected Item?",
                                   "Confirm Edit",
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Question);

                            // Handle user response
                            if (result == MessageBoxResult.Yes)
                            {
                                btnRemoveItem_Click(null, null);
                                return; // Exit if the item is removed
                            }
                            else
                            {
                                return; // Exit without making any changes
                            }
                        }
                    }

                    DataRow[] dataRow = dtBulkPricing.Select("ItemID = " + selectedItem.ItemID + " AND " + Convert.ToString(iQuantity) + " % Quantity = 0");
                    string strNote = string.Empty;

                    decimal parsedPrice = Convert.ToDecimal(dr[0]["ChargedCost"]);

                    if (dataRow.Any())
                    {
                        parsedPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                        string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                        strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);

                        parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                    }

                    parsedPrice = parsedPrice * (1 - discount / 100);
                    decimal tax = CalculatePriceAfterTax(parsedPrice, dr[0], dtTax);
                    decimal taxedPrice = tax;

                    // Update the selected item properties
                    selectedItem.Price = Convert.ToString(parsedPrice);
                    selectedItem.Tax = (tax - parsedPrice).ToString();
                    selectedItem.Discount = Convert.ToString(discount);
                    selectedItem.Quantity = Convert.ToString(iQuantity);
                    selectedItem.TotalPrice = (taxedPrice * iQuantity).ToString("F2");

                    //// Instead of refreshing the entire grid, modify the ObservableCollection
                    //var itemsList = objBillingItems.ToList(); // Convert ObservableCollection to a List
                    //int index = itemsList.FindIndex(item => item.UPC == selectedItem.UPC); // Find the index
                    //if (index != -1)
                    //{
                    //    itemsList[index] = selectedItem; // Update the item in the list
                    //    objBillingItems = new ObservableCollection<BillingItem>(itemsList); // Reassign to the ObservableCollection
                    //}

                    //// Update the DataGrid with the new ItemsSource
                    //dgBilling.ItemsSource = objBillingItems;

                    //// Explicitly reselect the item after refreshing
                    //dgBilling.SelectedIndex = selectedIndex; // Ensure the selected index is set
                    //dgBilling.SelectedItem = selectedItem;  // Set the selected item explicitly
                    //dgBilling.ScrollIntoView(selectedItem); // Ensure the selected item is visible

                    //// Recalculate totals
                    //CalculateTotals();

                    // Avoid full refresh, only update the specific item
                    dgBilling.Items.Refresh();

                    // Re-set the selection explicitly to preserve it after the refresh
                    dgBilling.SelectedIndex = selectedIndex; // Ensure the selected index is set
                    dgBilling.SelectedItem = selectedItem;  // Set the selected item explicitly
                    dgBilling.ScrollIntoView(selectedItem); // Ensure the selected item is visible

                    // Recalculate totals
                    CalculateTotals();

                    // Prevent further processing of the key
                    e.Handled = true;
                }
            }
        }

        private void btnCashDrawer_Click(object sender, RoutedEventArgs e)
        {
            //Open cash drawer
            OpenCashDrawer();
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
