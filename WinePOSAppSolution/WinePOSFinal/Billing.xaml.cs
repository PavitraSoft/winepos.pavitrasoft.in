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
using System.Net.NetworkInformation;
using System.Globalization;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Bibliography;
using System.Security.Policy;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.IO;
using DocumentFormat.OpenXml.Spreadsheet;

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


        WinePOSService objService = new WinePOSService();
        ObservableCollection<BillingItem> objBillingItems = new ObservableCollection<BillingItem>();

        private decimal _subTotal;
        private decimal _tax;
        private decimal _grandTotal;


        private Stopwatch stopwatch = new Stopwatch();
        private string inputBuffer = string.Empty;
        private bool isScanning = false;
        private int invoiceNumber = 0;
        private int editinvoiceNumber = 0;

        MainWindow mainWindow = null;
        bool isAdminLoggedIn = false; //For discount

        private List<Payments> paymentList = new List<Payments>();

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

            //InitializeCashDrawer(true);
            ReloadBillingData();

        }



        public void ReloadBillingData()
        {
            txtQuantity.Text = "1";
            objBillingItems.CollectionChanged += (s, e) => CalculateTotals();

            DataContext = this;

            FetchAndPopulateDataTable();

            string currentRole = AccessRightsManager.GetUserRole();

            //// Toggle visibility of the header textbox
            //textDiscount.Visibility = Visibility.Collapsed;
            //txtDiscountValue.Visibility = Visibility.Collapsed;
            //btnApplyDiscount.Visibility = Visibility.Collapsed;

            //// Toggle visibility of the Discount column
            //var discountColumn = dgBilling.Columns.FirstOrDefault(c => c.Header.ToString() == "Discount (%)");
            //if (discountColumn != null)
            //{
            //    discountColumn.Visibility = Visibility.Collapsed;
            //}

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

                //if (string.IsNullOrWhiteSpace(textBox.Text))
                //{
                //    textBox.Text = "1";
                //}

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
            try
            {
                string strUPC = txtUPC.Text;

                string strName = string.Empty;
                string strPrice = string.Empty;
                string strQuantity = string.Empty;
                int CurrentQuantity = 0;
                int ItemID = 0;
                string strTotalPrice = string.Empty;
                string strDiscount = string.Empty;

                DataRow[] dr;

                strName = txtName.Text.Split('-')[0].Trim();

                if (strName == "NUTS" || strName == "ICE BAG")
                {
                    dr = dtAllItems.Select(" Description = '" + strName + "'");
                }
                else
                {
                    dr = dtAllItems.Select(" UPC = '" + strUPC + "'");
                }

                //DataRow[] dr = dtAllItems.Select(" UPC = '" + strUPC + "'");


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
                        if (parsedQuantity > 0)
                        {
                            // Check if the item already exists in the ObservableCollection
                            var existingItem = objBillingItems.FirstOrDefault(item => item.ItemID == Convert.ToString(ItemID));

                            if (existingItem != null)
                            {
                                // Update the quantity of the existing item
                                int newQuantity = Convert.ToInt32(existingItem.Quantity) + parsedQuantity;

                                //DataRow[] dataRow = dtBulkPricing.Select(" UPC = " + strUPC + " AND " + newQuantity + " % Quantity = 0");
                                DataRow[] dataRow = dtBulkPricing.Select(" ItemID = " + Convert.ToString(ItemID));

                                //DataRow[] dataRow = dtBulkPricing.Select(" UPC = '" + strUPC + "'");
                                string strNote = string.Empty;
                                decimal totalPrice = parsedPrice * newQuantity;

                                if (dataRow.Any())
                                {
                                    // Extract all bulk pricing configurations and sort them in descending order based on quantity
                                    var bulkPricingList = dataRow
                                        .Select(row => new
                                        {
                                            Quantity = Convert.ToInt32(row["Quantity"]),
                                            Price = Convert.ToDecimal(row["Pricing"])
                                        })
                                        .OrderByDescending(x => x.Quantity) // Sort in descending order
                                        .ToList();

                                    int selectedBulkQuantity = 1;  // Default to normal price
                                    decimal selectedBulkPrice = parsedPrice; // Default normal price

                                    // Determine the best bulk pricing tier applicable
                                    foreach (var bulk in bulkPricingList)
                                    {
                                        if (newQuantity >= bulk.Quantity)
                                        {
                                            selectedBulkQuantity = bulk.Quantity;
                                            selectedBulkPrice = bulk.Price;

                                            strNote = $"Bulk Pricing @{selectedBulkQuantity} for ${selectedBulkPrice}";

                                            break; // Use the highest applicable bulk pricing
                                        }
                                    }


                                    // Calculate total price
                                    totalPrice = (Convert.ToInt32(newQuantity / selectedBulkQuantity) * selectedBulkPrice) +
                                                 ((newQuantity % selectedBulkQuantity) * parsedPrice);
                                }

                                //if (dataRow.Any())
                                //{
                                //    decimal bulkPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                                //    string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                                //    if (Convert.ToInt32(strQuan) <= Convert.ToInt32(newQuantity))
                                //    {
                                //        strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(bulkPrice);
                                //    }

                                //    totalPrice = (Convert.ToInt32(Convert.ToInt32(newQuantity) / Convert.ToInt32(strQuan)) * bulkPrice) + ((Convert.ToInt32(newQuantity) % Convert.ToInt32(strQuan)) * parsedPrice);

                                //    //parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                                //}


                                //if (CurrentQuantity >= newQuantity)
                                if (true)
                                {

                                    decimal discount = Convert.ToDecimal(existingItem.Discount);

                                    totalPrice = totalPrice * (1 - discount / 100);

                                    decimal tax = CalculatePriceAfterTax(totalPrice, dr[0], dtTax);
                                    //decimal taxedPrice = parsedPrice + tax;
                                    decimal taxedPrice = tax;
                                    existingItem.Price = Convert.ToString(Math.Round(parsedPrice, 2));
                                    existingItem.Tax = Convert.ToString(Math.Round((tax - totalPrice) / newQuantity, 3));
                                    existingItem.Quantity = Convert.ToString(newQuantity);
                                    existingItem.Discount = Convert.ToString(Math.Round(discount));
                                    existingItem.TotalPrice = Convert.ToString(Math.Round(taxedPrice, 2));
                                    existingItem.Note = strNote;
                                    // Clear the TextBox controls for new input
                                    txtUPC.Clear();
                                    txtName.Clear();
                                    txtQuantity.Text = "1";

                                    ShowTextOnDisplay(strName, existingItem.Quantity, existingItem.TotalPrice);
                                }
                                else
                                {
                                    MessageBox.Show($"Asked Quantity: {newQuantity} Current Quantity: {CurrentQuantity}.");
                                }
                            }
                            else
                            {
                                //if (CurrentQuantity >= parsedQuantity)
                                if (true)
                                {
                                    //DataRow[] dataRow = dtBulkPricing.Select(" UPC = " + strUPC + " AND " + parsedQuantity + " % Quantity = 0");
                                    DataRow[] dataRow = dtBulkPricing.Select(" ItemID = '" + ItemID + "'");
                                    string strNote = string.Empty;
                                    decimal totalPrice = parsedPrice * parsedQuantity;

                                    if (dataRow.Any())
                                    {
                                        // Extract all bulk pricing configurations and sort them in descending order based on quantity
                                        var bulkPricingList = dataRow
                                            .Select(row => new
                                            {
                                                Quantity = Convert.ToInt32(row["Quantity"]),
                                                Price = Convert.ToDecimal(row["Pricing"])
                                            })
                                            .OrderByDescending(x => x.Quantity) // Sort in descending order
                                            .ToList();

                                        int selectedBulkQuantity = 1;  // Default to normal price
                                        decimal selectedBulkPrice = parsedPrice; // Default normal price

                                        // Determine the best bulk pricing tier applicable
                                        foreach (var bulk in bulkPricingList)
                                        {
                                            if (parsedQuantity >= bulk.Quantity)
                                            {
                                                selectedBulkQuantity = bulk.Quantity;
                                                selectedBulkPrice = bulk.Price;

                                                strNote = $"Bulk Pricing @{selectedBulkQuantity} for ${selectedBulkPrice}";

                                                break; // Use the highest applicable bulk pricing
                                            }
                                        }


                                        // Calculate total price
                                        totalPrice = (Convert.ToInt32(parsedQuantity / selectedBulkQuantity) * selectedBulkPrice) +
                                                     ((parsedQuantity % selectedBulkQuantity) * parsedPrice);
                                    }

                                    //if (dataRow.Any())
                                    //{
                                    //    decimal bulkPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                                    //    string strQuan = Convert.ToString(dataRow[0]["Quantity"]);

                                    //    if (Convert.ToInt32(strQuan) <= Convert.ToInt32(parsedQuantity))
                                    //    {
                                    //        strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(bulkPrice);
                                    //    }

                                    //    totalPrice = (Convert.ToInt32(Convert.ToInt32(parsedQuantity) / Convert.ToInt32(strQuan)) * bulkPrice) + ((Convert.ToInt32(parsedQuantity) % Convert.ToInt32(strQuan)) * parsedPrice);


                                    //    //parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                                    //}

                                    decimal tax = CalculatePriceAfterTax(totalPrice, dr[0], dtTax);
                                    //decimal taxedPrice = parsedPrice + tax;
                                    decimal taxedPrice = tax;




                                    // Create a new BillingItem
                                    BillingItem newItem = new BillingItem
                                    {
                                        UPC = strUPC,
                                        Name = strName,
                                        Price = Convert.ToString(Math.Round(parsedPrice, 2)),
                                        Quantity = Convert.ToString(parsedQuantity),
                                        Tax = Convert.ToString(Math.Round((tax - totalPrice) / parsedQuantity, 3)), // Format total price as a string with 2 decimals
                                        Discount = "0",
                                        TotalPrice = Convert.ToString(Math.Round(taxedPrice, 2)), // Format total price as a string with 2 decimals
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


                                    ShowTextOnDisplay(strName, newItem.Quantity, newItem.TotalPrice);
                                }
                                else
                                {
                                    MessageBox.Show($"Asked Quantity: {parsedQuantity} Current Quantity: {CurrentQuantity}.");
                                }
                            }

                            CalculateTotals();
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
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.ToString());
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
            //MessageBoxResult result = MessageBox.Show(
            //   $"Are you sure you want to Pay the Current Billing Invoice?",
            //   "Confirm Payment",
            //   MessageBoxButton.YesNo,
            //   MessageBoxImage.Question);

            // Handle user response
            //if (result == MessageBoxResult.Yes)
            //{
            decimal totalPrice = objBillingItems.Sum(item => Convert.ToDecimal(item.TotalPrice));
            paymentList.Add(new Payments("PALMPAY", totalPrice));
            if (SaveInvoice(objBillingItems, false, "PALMPAY", paymentList, editinvoiceNumber))
            {
                //MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Optionally, clear the DataGrid after payment
                objBillingItems.Clear();
                paymentList.Clear();
                //MessageBoxResult result = MessageBox.Show(
                //    $"Payment confirmed. Thank you! Do you want to print invoice?",
                //    "Print Invoice",
                //    MessageBoxButton.YesNo,
                //    MessageBoxImage.Question);
                //if (result == MessageBoxResult.Yes)
                //    btnPrintInvoice_Click(null, null);
            }
            else
            {
                MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //}
            //else
            //{
            //    MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}

        }

        private void btnCash_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                //btnTenderWindow_Click(null, null);
                ////Open cash drawer
                //MessageBoxResult result = MessageBox.Show(
                //$"Are you sure you want to Cash the Current Billing Invoice?",
                //"Confirm Payment",
                //MessageBoxButton.YesNo,
                //MessageBoxImage.Question);

                //// Handle user response
                //if (result == MessageBoxResult.Yes)
                //{


                decimal totalPrice = objBillingItems.Sum(item => Convert.ToDecimal(item.TotalPrice));
                paymentList.Add(new Payments("CASH", totalPrice));


                btnTenderWindow_Click(null, null);


                if (SaveInvoice(objBillingItems, false, "CASH", paymentList, editinvoiceNumber))
                {
                    objBillingItems.Clear();
                    paymentList.Clear();
                }
                else
                {
                    MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                //}
                //else
                //{
                //    MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
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
                paymentList.Clear();
                //btnPrintInvoice_Click(null, null);

                MessageBox.Show("Invoice has been cleared. Thank you!", "Clear", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        //private void btnPrintInvoice_Click(object sender, RoutedEventArgs e)
        //{
        //    if (invoiceNumber != 0)
        //    {
        //        try
        //        {

        //            //DataTable InvoiceData = objService.FetchAndPopulateInvoice(true, null, null, Convert.ToString(invoiceNumber));
        //            DataSet dsInvoiceData = objService.FetchAndPopulateInvoice(true, null, null, Convert.ToString(invoiceNumber));

        //            DataTable InvoiceData = dsInvoiceData.Tables[0];
        //            DataTable PaymentData = dsInvoiceData.Tables[1];

        //            string[] name = InvoiceData.AsEnumerable()
        //                         .Select(row => row.Field<string>("Name").ToString())
        //                         .ToArray();

        //            string[] price = InvoiceData.AsEnumerable()
        //                         .Select(row => row.Field<decimal>("Price").ToString())
        //                         .ToArray();

        //            string[] quantity = InvoiceData.AsEnumerable()
        //                         .Select(row => row.Field<int>("Quantity").ToString())
        //                         .ToArray();

        //            string[] tax = InvoiceData.AsEnumerable()
        //                         .Select(row => row.Field<decimal>("Tax").ToString())
        //                         .ToArray();

        //            string[] totalPrice = InvoiceData.AsEnumerable()
        //                         .Select(row => row.Field<decimal>("TotalPrice").ToString())
        //                         .ToArray();

        //            string[] discount = InvoiceData.AsEnumerable()
        //                         .Select(row => row.Field<decimal>("Discount").ToString())
        //                         .ToArray();

        //            string paymentType = Convert.ToString(InvoiceData.Rows[0]["PaymentType"]);

        //            string strCashAmt = string.Empty;
        //            string strCheckAmt = string.Empty;
        //            string strCreditAmt = string.Empty;
        //            string strPalmPayAmt = string.Empty;

        //            foreach(DataRow dataRow in PaymentData.Rows)
        //            {
        //                string strPaymentType = Convert.ToString(dataRow["PaymentType"]).ToUpper();
        //                decimal Amount = Convert.ToDecimal(dataRow["Amount"]);

        //                if (Amount > 0)
        //                {
        //                    if (strPaymentType == "CASH")
        //                        strCashAmt = Amount.ToString("G29");
        //                    else if (strPaymentType == "CHECK")
        //                        strCheckAmt = Amount.ToString("G29");
        //                    else if (strPaymentType == "CREDIT")
        //                        strCreditAmt = Amount.ToString("G29");
        //                    else if (strPaymentType == "PALMPAY")
        //                        strPalmPayAmt = Amount.ToString("G29");
        //                }
        //            }

        //            PrintInvoice(name, price, quantity, tax, totalPrice, discount, strCashAmt, strCheckAmt, strCreditAmt, strPalmPayAmt, Convert.ToString(invoiceNumber));

        //            //// Create a new report document
        //            //ReportDocument report = new ReportDocument();


        //            //// Load the report (winebill.rpt)
        //            ////string reportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Reports\winebill.rpt");
        //            //string reportPath = System.IO.Path.Combine(@"D:\Study\Dotnet\WinePOSGIT\winepos.pavitrasoft.in\WinePOSAppSolution\WinePOSFinal\Reports\winebill.rpt");

        //            //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //            //// Target file
        //            //string targetFile = Path.Combine("Reports", "winebill.rpt");

        //            //// Combine base directory with the relative path
        //            ////string reportPath = Path.Combine(baseDirectory, targetFile);
        //            //report.Load(reportPath);

        //            //// Create and populate the DataTable
        //            ////DataTable dt = objService.GetInventoryData(string.Empty, string.Empty);

        //            //// Set the DataTable as the data source for the report
        //            ////report.SetDataSource(dt);

        //            //// Set database logon credentials (if required)
        //            //SetDatabaseLogin(report);

        //            //// Dynamically set the InvoiceCode parameter for the report
        //            //report.SetParameterValue("InvoiceCode", invoiceNumber);

        //            //// Export the report to a PDF file
        //            //string exportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "WineBill.pdf");
        //            //report.ExportToDisk(ExportFormatType.PortableDocFormat, exportPath);

        //            //// Display the PDF in the WebBrowser control
        //            ////pdfWebViewer.Navigate(exportPath); // Navigate to the generated PDF file


        //            //// Optionally, open the generated report in a PDF viewer
        //            //System.Diagnostics.Process.Start(exportPath);

        //            //MessageBox.Show("Report generated and displayed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please make payment first to print invoice.", "Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        //private void PrintInvoice(string[] name, string[] price, string[] quantity, string[] tax, string[] totalPrice, string[] discount, string strCashAmt, string strCheckAmt, string strCreditAmt, string strPalmPayAmt, string invoiceNumber)
        //{
        //    var mainWindow = (MainWindow)Application.Current.MainWindow;
        //    PosPrinter m_Printer = mainWindow.m_Printer;
        //    //<<<step2>>>--Start
        //    //Initialization
        //    DateTime nowDate = DateTime.Now;                            //System date
        //    DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   //Date Format
        //    dateFormat.MonthDayPattern = "MMMM";
        //    string strDate = nowDate.ToString("MMMM,dd,yyyy  HH:mm", dateFormat);
        //    string strbcData = invoiceNumber;
        //    //String[] astritem = { "apples", "grapes", "bananas", "lemons", "oranges" };
        //    //String[] astrprice = { "10.00", "20.00", "30.00", "40.00", "50.00" };

        //    if (m_Printer.CapRecPresent)
        //    {

        //        try
        //        {
        //            //<<<step6>>>--Start
        //            //Batch processing mode
        //            m_Printer.TransactionPrint(PrinterStation.Receipt
        //                , PrinterTransactionControl.Transaction);

        //            //<<<step3>>>--Start
        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|1B");
        //            //<<<step3>>>--End

        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|N"
        //                + "123xxstreet,xxxcity,xxxxstate\n");

        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|rA"
        //                + "TEL 9999-99-9999   C#2\n");

        //            //<<<step5>>--Start
        //            //Make 2mm speces
        //            //ESC|#uF = Line Feed
        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|200uF");
        //            //<<<step5>>>-End

        //            int iRecLineCharsCount = m_Printer.RecLineCharsList.Length;
        //            if (iRecLineCharsCount >= 2)
        //            {
        //                m_Printer.RecLineChars = m_Printer.RecLineCharsList[1];
        //                m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|cA" + strDate + "\n");
        //                m_Printer.RecLineChars = m_Printer.RecLineCharsList[0];
        //            }
        //            else
        //            {
        //                m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|cA" + strDate + "\n");
        //            }

        //            //<<<step5>>>--Start
        //            //Make 5mm speces
        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|500uF");

        //            //Print buying goods
        //            double total = 0.0;
        //            string strPrintData = "";
        //            for (int i = 0; i < name.Length; i++)
        //            {
        //                decimal itemTotal = Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(price[i]);

        //                string strDiscount = (Convert.ToDecimal(discount[i]) != 0) ? "* (" + Convert.ToString(discount[i]) + "%)" : string.Empty;

        //                strPrintData = MakePrintString(m_Printer.RecLineChars, name[i] + strDiscount, "   " + quantity[i] + " @ $" + price[i] + " $"
        //                    + (Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(price[i])));

        //                m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

        //                total += Convert.ToDouble(itemTotal);

        //            }

        //            //Make 2mm speces
        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|200uF");

        //            //Print the total cost
        //            strPrintData = MakePrintString(m_Printer.RecLineChars, "Tax excluded."
        //                , "$" + total.ToString("F"));

        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + strPrintData + "\n");

        //            decimal totaltax = tax.Select(item => Convert.ToDecimal(item)).Sum();
        //            decimal totalPriceAfterTax = totalPrice.Select(item => Convert.ToDecimal(item)).Sum();

        //            strPrintData = MakePrintString(m_Printer.RecLineChars, "Tax ", "$"
        //                + (totaltax).ToString("F"));

        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|uC" + strPrintData + "\n");

        //            strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "Total", "$"
        //                + (totalPriceAfterTax).ToString("F"));

        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
        //                + strPrintData + "\n");

        //            //strPrintData = MakePrintString(m_Printer.RecLineChars, "Customer's payment", "$200.00");

        //            m_Printer.PrintNormal(PrinterStation.Receipt
        //                , strPrintData + "\n");

        //            //strPrintData = MakePrintString(m_Printer.RecLineChars, "Change", "$" + (200.00 - (total * 1.05)).ToString("F"));

        //            m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

        //            if (!string.IsNullOrWhiteSpace(strCashAmt))
        //            {
        //                strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "CASH", "$"
        //                    + strCashAmt);

        //                m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
        //                    + strPrintData + "\n");
        //            }
        //            if (!string.IsNullOrWhiteSpace(strCheckAmt))
        //            {
        //                strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "CHECK", "$"
        //                    + strCheckAmt);

        //                m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
        //                    + strPrintData + "\n");
        //            }
        //            if (!string.IsNullOrWhiteSpace(strPalmPayAmt))
        //            {
        //                strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "PALM PAY", "$"
        //                    + strPalmPayAmt);

        //                m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
        //                    + strPrintData + "\n");
        //            }
        //            if (!string.IsNullOrWhiteSpace(strCreditAmt))
        //            {
        //                strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "CASH", "$"
        //                    + strCreditAmt);

        //                m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
        //                    + strPrintData + "\n");
        //            }

        //            //strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "Payment Type", "$"
        //            //    + paymentType);

        //            //m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
        //            //    + strPrintData + "\n");

        //            //Make 5mm speces
        //            //m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|500uF");

        //            //<<<step4>>>--Start
        //            if (m_Printer.CapRecBarCode == true)
        //            {
        //                string barcodeData = ConvertInvoiceToEAN13(Convert.ToInt32(strbcData));

        //                //Barcode printing
        //                m_Printer.PrintBarCode(PrinterStation.Receipt, barcodeData,
        //                    BarCodeSymbology.EanJan13, 1000,
        //                    m_Printer.RecLineWidth, PosPrinter.PrinterBarCodeLeft,
        //                    BarCodeTextPosition.Below);
        //            }
        //            //<<<step4>>>--End


        //            //Make 5mm speces
        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|500uF");

        //            strPrintData = "Thank you for shopping at Crown Liquor!";

        //            m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

        //            //<<<step5>>>--End


        //            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");
        //            //<<<step2>>>--End

        //            //print all the buffer data. and exit the batch processing mode.
        //            m_Printer.TransactionPrint(PrinterStation.Receipt
        //                , PrinterTransactionControl.Normal);
        //            //<<<step6>>>--End
        //        }
        //        catch (PosControlException ex)
        //        {
        //            MessageBox.Show("Error while printing invoice. Exception:" + ex.ToString(), "Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        }
        //    }

        //    //<<<step6>>>--Start
        //    // When a cursor is back to its default shape, it means the process ends
        //    //Cursor.Current = Cursors.Default;
        //    //<<<step6>>>--End

        //}

        //public static string ConvertInvoiceToEAN13(int invoiceNumber)
        //{
        //    // Convert invoice number to string
        //    string base12Digits = invoiceNumber.ToString();

        //    // Ensure it's at least 12 digits by padding with leading zeros
        //    base12Digits = base12Digits.PadLeft(12, '0');

        //    // Calculate EAN-13 checksum
        //    int sum = 0;
        //    for (int i = 0; i < 12; i++)
        //    {
        //        int digit = base12Digits[i] - '0'; // Convert char to integer
        //        sum += (i % 2 == 0) ? digit : digit * 3; // Odd position: digit * 1, Even position: digit * 3
        //    }

        //    int checksum = (10 - (sum % 10)) % 10; // Compute the checksum
        //    return base12Digits + checksum; // Return valid 13-digit barcode
        //}


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
                decimal TotalPrice = objBillingItems.Sum(item => (decimal.TryParse(item.TotalPrice, out var dectotalPrice) ? dectotalPrice : 0));
                decimal TaxPrice = objBillingItems.Sum(item => (decimal.TryParse(item.Tax, out var decTax) ? decTax * Convert.ToInt32(item.Quantity) : 0));

                Tax = TaxPrice;
                SubTotal = TotalPrice - Tax;
                GrandTotal = SubTotal + Tax;

                if (objBillingItems.Count == 1)
                {
                    Change.Visibility = Visibility.Collapsed;
                    txtAmtChange.Text = "0";
                    txtAmtChange.Visibility = Visibility.Collapsed;
                }
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

        private bool SaveInvoice(ObservableCollection<BillingItem> objBilling, bool IsVoidInvoice, string PaymentType, List<Payments> objPayments, int iNumber)
        {
            try
            {
                objService.DeleteInvoiceByNumber(iNumber);
                if (iNumber != 0)
                {
                    invoiceNumber = iNumber;
                }
                editinvoiceNumber = 0;
                foreach (BillingItem bi in objBilling)
                {
                    objService.SaveInvoice(bi, IsVoidInvoice, PaymentType, ref invoiceNumber, objPayments);
                    objPayments = new List<Payments>();
                }
                invoiceNumber = 0;
                SendLowQuantityMail();

                dtAllItems = objService.GetInventoryData(string.Empty, string.Empty);

                isAdminLoggedIn = false;

                var mainWindow = (MainWindow)Application.Current.MainWindow;

                if (mainWindow != null)
                {
                    // Get the content inside the "Billing" TabItem (assuming it's a UserControl)
                    var salesHistory = mainWindow.SalesHistory.Content as SalesHistory;

                    if (salesHistory != null)
                    {
                        // Call the method inside Billing user control
                        salesHistory.ReloadSalesHistoryData();
                    }

                }

                ShowTextOnDisplay("Thank you for shopping.", "", "");

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
            //MessageBoxResult result = MessageBox.Show(
            //   $"Are you sure you want to Check the Current Billing Invoice?",
            //   "Confirm Payment",
            //   MessageBoxButton.YesNo,
            //   MessageBoxImage.Question);

            //// Handle user response
            //if (result == MessageBoxResult.Yes)
            //{

            decimal totalPrice = objBillingItems.Sum(item => Convert.ToDecimal(item.TotalPrice));
            paymentList.Add(new Payments("CHECK", totalPrice));
            if (SaveInvoice(objBillingItems, false, "CHECK", paymentList, editinvoiceNumber))
            {
                //MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Optionally, clear the DataGrid after payment
                objBillingItems.Clear();
                paymentList.Clear();

                //MessageBoxResult result = MessageBox.Show(
                //    $"Payment confirmed. Thank you! Do you want to print invoice?",
                //    "Print Invoice",
                //    MessageBoxButton.YesNo,
                //    MessageBoxImage.Question);
                //if (result == MessageBoxResult.Yes)
                //    btnPrintInvoice_Click(null, null);
            }
            else
            {
                MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //}
            //else
            //{
            //    MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }

        private void btnCredit_Click(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show(
            //   $"Are you sure you want to Credit the Current Billing Invoice?",
            //   "Confirm Payment",
            //   MessageBoxButton.YesNo,
            //   MessageBoxImage.Question);

            // Handle user response
            //if (result == MessageBoxResult.Yes)
            //{

            decimal totalPrice = objBillingItems.Sum(item => Convert.ToDecimal(item.TotalPrice));
            paymentList.Add(new Payments("CREDIT", totalPrice));
            if (SaveInvoice(objBillingItems, false, "CREDIT", paymentList, editinvoiceNumber))
            {
                //MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Optionally, clear the DataGrid after payment
                objBillingItems.Clear();
                paymentList.Clear();

                //MessageBoxResult result = MessageBox.Show(
                //    $"Payment confirmed. Thank you! Do you want to print invoice?",
                //    "Print Invoice",
                //    MessageBoxButton.YesNo,
                //    MessageBoxImage.Question);
                //if (result == MessageBoxResult.Yes)
                //    btnPrintInvoice_Click(null, null);
            }
            else
            {
                MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //}
            //else
            //{
            //    MessageBox.Show("Payment canceled.", "Payment Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
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

        private bool PromptAdminLogin()
        {
            Login loginWindow = new Login(true);
            bool? result = loginWindow.ShowDialog();

            return result == true; // Only proceed if authentication succeeds
        }

        private void btnApplyDiscount_Click(object sender, RoutedEventArgs e)
        {

            string userRole = AccessRightsManager.GetUserRole();
            if (!isAdminLoggedIn)
            {
                if (!(userRole == "ADMIN" || userRole == "MANAGER"))
                {
                    if (!PromptAdminLogin()) // Prompt for admin credentials
                    {
                        MessageBox.Show("Admin authentication failed. Discount cannot be applied.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            isAdminLoggedIn = true;

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
                    string userRole = AccessRightsManager.GetUserRole();
                    if (!isAdminLoggedIn)
                    {
                        if (!(userRole == "ADMIN" || userRole == "MANAGER"))
                        {
                            if (!PromptAdminLogin()) // Prompt for admin credentials
                            {
                                MessageBox.Show("Admin authentication failed. Discount cannot be applied.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                                editedItem.Discount = "";
                                return;
                            }
                        }
                    }

                    isAdminLoggedIn = true;

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
                        editedItem.Discount = Convert.ToString(discount);
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
            var addWindow = new QuickAdd(objBillingItems, 1, 0, "NON SCAN NO TAX");
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
                strQuantity = "1";

                // Calculate total price (for this example, assuming price and quantity are numeric)
                if (decimal.TryParse(strPrice, out decimal parsedPrice) && int.TryParse(strQuantity, out int parsedQuantity))
                {
                    // Check if the item already exists in the ObservableCollection
                    var existingItem = objBillingItems.FirstOrDefault(item => item.ItemID == Convert.ToString(ItemID));


                    if (existingItem != null)
                    {
                        // Update the quantity of the existing item
                        int newQuantity = Convert.ToInt32(existingItem.Quantity) + parsedQuantity;


                        //DataRow[] dataRow = dtBulkPricing.Select(" ItemID = " + Convert.ToString(ItemID) + " AND " + newQuantity + " % Quantity = 0");
                        DataRow[] dataRow = dtBulkPricing.Select(" ItemID = " + Convert.ToString(ItemID));
                        string strNote = string.Empty;

                        //if (dataRow.Any())
                        //{
                        //    decimal bulkPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                        //    string strQuan = Convert.ToString(dataRow[0]["Quantity"]);

                        //    if (Convert.ToInt32(strQuan) <= Convert.ToInt32(newQuantity))
                        //    {
                        //        strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(bulkPrice);
                        //    }

                        //    parsedPrice = (Convert.ToInt32(Convert.ToInt32(newQuantity) / Convert.ToInt32(strQuan)) * bulkPrice) + ((Convert.ToInt32(newQuantity) % Convert.ToInt32(strQuan)) * parsedPrice);
                        //}

                        //if (CurrentQuantity >= newQuantity)
                        if (true)
                        {
                            decimal totalPrice = parsedPrice * newQuantity;

                            if (dataRow.Any())
                            {
                                // Extract all bulk pricing configurations and sort them in descending order based on quantity
                                var bulkPricingList = dataRow
                                    .Select(row => new
                                    {
                                        Quantity = Convert.ToInt32(row["Quantity"]),
                                        Price = Convert.ToDecimal(row["Pricing"])
                                    })
                                    .OrderByDescending(x => x.Quantity) // Sort in descending order
                                    .ToList();

                                int selectedBulkQuantity = 1;  // Default to normal price
                                decimal selectedBulkPrice = parsedPrice; // Default normal price

                                // Determine the best bulk pricing tier applicable
                                foreach (var bulk in bulkPricingList)
                                {
                                    if (newQuantity >= bulk.Quantity)
                                    {
                                        selectedBulkQuantity = bulk.Quantity;
                                        selectedBulkPrice = bulk.Price;


                                        strNote = $"Bulk Pricing @{selectedBulkQuantity} for ${selectedBulkPrice}";

                                        break; // Use the highest applicable bulk pricing
                                    }
                                }

                                // Calculate total price
                                totalPrice = (Convert.ToInt32(newQuantity / selectedBulkQuantity) * selectedBulkPrice) +
                                             ((newQuantity % selectedBulkQuantity) * parsedPrice);
                            }


                            //if (dataRow.Any())
                            //{
                            //    decimal bulkPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                            //    string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                            //    //strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);


                            //    if (Convert.ToInt32(strQuan) <= Convert.ToInt32(newQuantity))
                            //    {
                            //        strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(bulkPrice);
                            //    }

                            //    totalPrice = (Convert.ToInt32(Convert.ToInt32(newQuantity) / Convert.ToInt32(strQuan)) * bulkPrice) + ((Convert.ToInt32(newQuantity) % Convert.ToInt32(strQuan)) * parsedPrice);

                            //    //parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                            //}

                            decimal discount = Convert.ToDecimal(existingItem.Discount);

                            totalPrice = totalPrice * (1 - discount / 100);

                            decimal tax = CalculatePriceAfterTax(totalPrice, dr[0], dtTax);
                            //decimal taxedPrice = parsedPrice + tax;
                            decimal taxedPrice = tax;
                            existingItem.Price = Convert.ToString(Math.Round(parsedPrice, 2));
                            existingItem.Tax = Convert.ToString(Math.Round((tax - totalPrice) / newQuantity, 3));
                            existingItem.Quantity = Convert.ToString(newQuantity);
                            existingItem.Discount = Convert.ToString(discount);
                            existingItem.TotalPrice = Convert.ToString(Math.Round(taxedPrice, 2));
                            existingItem.Note = strNote;
                            // Clear the TextBox controls for new input
                            txtUPC.Clear();
                            txtName.Clear();
                            txtQuantity.Text = "1";


                            ShowTextOnDisplay(strName, existingItem.Quantity, existingItem.TotalPrice);
                        }
                        else
                        {
                            MessageBox.Show($"Asked Quantity: {newQuantity} Current Quantity: {CurrentQuantity}.");
                        }
                    }
                    else
                    {
                        //if (CurrentQuantity >= parsedQuantity)
                        if (true)
                        {
                            //DataRow[] dataRow = dtBulkPricing.Select(" ItemID = " + Convert.ToString(ItemID) + " AND " + parsedQuantity + " % Quantity = 0");
                            DataRow[] dataRow = dtBulkPricing.Select(" ItemID = " + Convert.ToString(ItemID));
                            string strNote = string.Empty;

                            decimal totalPrice = parsedPrice * parsedQuantity;
                            if (dataRow.Any())
                            {
                                // Extract all bulk pricing configurations and sort them in descending order based on quantity
                                var bulkPricingList = dataRow
                                    .Select(row => new
                                    {
                                        Quantity = Convert.ToInt32(row["Quantity"]),
                                        Price = Convert.ToDecimal(row["Pricing"])
                                    })
                                    .OrderByDescending(x => x.Quantity) // Sort in descending order
                                    .ToList();

                                int selectedBulkQuantity = 1;  // Default to normal price
                                decimal selectedBulkPrice = parsedPrice; // Default normal price

                                // Determine the best bulk pricing tier applicable
                                foreach (var bulk in bulkPricingList)
                                {
                                    if (parsedQuantity >= bulk.Quantity)
                                    {
                                        selectedBulkQuantity = bulk.Quantity;
                                        selectedBulkPrice = bulk.Price;

                                        strNote = $"Bulk Pricing @{selectedBulkQuantity} for ${selectedBulkPrice}";

                                        break; // Use the highest applicable bulk pricing
                                    }
                                }


                                // Calculate total price
                                totalPrice = (Convert.ToInt32(parsedQuantity / selectedBulkQuantity) * selectedBulkPrice) +
                                             ((parsedQuantity % selectedBulkQuantity) * parsedPrice);
                            }

                            //if (dataRow.Any())
                            //{
                            //    decimal bulkPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                            //    string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                            //    //strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(parsedPrice);

                            //    if (Convert.ToInt32(strQuan) <= Convert.ToInt32(parsedQuantity))
                            //    {
                            //        strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(bulkPrice);
                            //    }

                            //    totalPrice = (Convert.ToInt32(Convert.ToInt32(parsedQuantity) / Convert.ToInt32(strQuan)) * bulkPrice) + ((Convert.ToInt32(parsedQuantity) % Convert.ToInt32(strQuan)) * parsedPrice);

                            //    //parsedPrice = parsedPrice / Convert.ToDecimal(strQuan);
                            //}

                            decimal tax = CalculatePriceAfterTax(totalPrice, dr[0], dtTax);
                            //decimal taxedPrice = parsedPrice + tax;
                            decimal taxedPrice = tax;

                            // Create a new BillingItem
                            BillingItem newItem = new BillingItem
                            {
                                UPC = strUPC,
                                Name = strName,
                                Price = Convert.ToString(Math.Round(parsedPrice, 2)),
                                Quantity = Convert.ToString(parsedQuantity),
                                Tax = Convert.ToString(Math.Round((tax - totalPrice) / parsedQuantity, 3)), // Format total price as a string with 2 decimals
                                Discount = "0",
                                TotalPrice = Convert.ToString(Math.Round(taxedPrice, 2)), // Format total price as a string with 2 decimals
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


                            ShowTextOnDisplay(strName, newItem.Quantity, newItem.TotalPrice);
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
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            CashDrawer cashDrawer = mainWindow.cashDrawer;
            try
            {
                cashDrawer.Open();
                cashDrawer.Claim(5000);
                cashDrawer.DeviceEnabled = true;
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
            finally
            {
                // ✅ Ensure the device is properly released before initializing the printer
                if (cashDrawer != null)
                {
                    cashDrawer.DeviceEnabled = false;
                    cashDrawer.Release();
                    cashDrawer.Close();
                }
            }
        }

        private void dgBilling_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dgBilling.SelectedItem is BillingItem selectedItem)
            {
                // Store the current selection
                int selectedIndex = dgBilling.SelectedIndex;

                if (e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Up || e.Key == Key.Down)
                {
                    string keyPressed = e.Key == Key.OemPeriod ? ">" :
                                        e.Key == Key.OemComma ? "<" :
                                        e.Key == Key.Up ? ">" : "<"; // Up Arrow acts as '>', Down Arrow acts as '<'

                    DataRow[] dr = dtAllItems.Select("ItemID = '" + selectedItem.ItemID + "'");
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

                    DataRow[] dataRow = dtBulkPricing.Select("ItemID = " + selectedItem.ItemID);
                    string strNote = string.Empty;
                    decimal parsedPrice = Convert.ToDecimal(dr[0]["ChargedCost"]);
                    decimal totalPrice = parsedPrice * iQuantity;

                    if (dataRow.Any())
                    {
                        var bulkPricingList = dataRow
                            .Select(row => new
                            {
                                Quantity = Convert.ToInt32(row["Quantity"]),
                                Price = Convert.ToDecimal(row["Pricing"])
                            })
                            .OrderByDescending(x => x.Quantity)
                            .ToList();

                        int selectedBulkQuantity = 1;
                        decimal selectedBulkPrice = parsedPrice;

                        foreach (var bulk in bulkPricingList)
                        {
                            if (iQuantity >= bulk.Quantity)
                            {
                                selectedBulkQuantity = bulk.Quantity;
                                selectedBulkPrice = bulk.Price;
                                strNote = $"Bulk Pricing @{selectedBulkQuantity} for ${selectedBulkPrice}";
                                break;
                            }
                        }

                        totalPrice = (Convert.ToInt32(iQuantity / selectedBulkQuantity) * selectedBulkPrice) +
                                     ((iQuantity % selectedBulkQuantity) * parsedPrice);
                    }

                    totalPrice = totalPrice * (1 - discount / 100);
                    decimal tax = CalculatePriceAfterTax(totalPrice, dr[0], dtTax);
                    decimal taxedPrice = tax;

                    selectedItem.Price = Convert.ToString(Math.Round(parsedPrice, 2));
                    selectedItem.Tax = Convert.ToString(Math.Round((tax - totalPrice) / iQuantity, 3));
                    selectedItem.Discount = Convert.ToString(discount);
                    selectedItem.Quantity = Convert.ToString(iQuantity);
                    selectedItem.TotalPrice = Convert.ToString(Math.Round(taxedPrice, 2));
                    selectedItem.Note = strNote;

                    // Notify UI without full refresh
                    CollectionViewSource.GetDefaultView(dgBilling.ItemsSource).Refresh();

                    // Restore the selection explicitly
                    dgBilling.SelectedIndex = selectedIndex;
                    dgBilling.SelectedItem = selectedItem;
                    dgBilling.ScrollIntoView(selectedItem);
                    dgBilling.Focus();

                    // Recalculate totals
                    CalculateTotals();



                    ShowTextOnDisplay(selectedItem.Name, selectedItem.Quantity, selectedItem.TotalPrice);

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

        public String MakePrintString(int iLineChars, String strBuf, String strPrice)
        {
            int iSpaces = 0;
            String tab = "";
            try
            {
                iSpaces = iLineChars - (strBuf.Length + strPrice.Length);
                for (int j = 0; j < iSpaces; j++)
                {
                    tab += " ";
                }
            }
            catch (Exception)
            {
            }
            return strBuf + tab + strPrice;
        }

        private void btnSplitPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addWindow = new SplitPayment(paymentList, Math.Round(GrandTotal, 2));
                if (addWindow.ShowDialog() == true)
                {
                    if (SaveInvoice(objBillingItems, false, "SPLIT", paymentList, editinvoiceNumber))
                    {
                        //MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Optionally, clear the  after paymentDataGrid
                        objBillingItems.Clear();
                        paymentList.Clear();
                        //OpenCashDrawer();


                        //MessageBoxResult result = MessageBox.Show(
                        //    $"Payment confirmed. Thank you! Do you want to print invoice?",
                        //    "Print Invoice",
                        //    MessageBoxButton.YesNo,
                        //    MessageBoxImage.Question);
                        //if (result == MessageBoxResult.Yes)
                        //    btnPrintInvoice_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Error while saving the current Invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    //do nothing
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error 1: {ex.Message}");
            }


        }
        public void PopulateInvoiceData(int iNumber)
        {
            editinvoiceNumber = iNumber;

            InitializeComponent();
            //InitializeCashDrawer(true);
            ReloadBillingData();

            objBillingItems.Clear();
            paymentList.Clear();
            DataSet dsInvoiceData = objService.FetchAndPopulateInvoice(true, null, null, Convert.ToString(editinvoiceNumber));

            DataTable dtInvoice = dsInvoiceData.Tables[0];

            foreach (DataRow dr in dtInvoice.Rows)
            {
                string strUPC = Convert.ToString(dr["UPC"]);
                int ItemID = 0;
                // Update the quantity of the existing item
                int newQuantity = Convert.ToInt32(dr["Quantity"]);

                //DataRow[] dataRow = dtBulkPricing.Select(" UPC = " + strUPC + " AND " + newQuantity + " % Quantity = 0");
                DataRow[] dataRow = dtBulkPricing.Select(" UPC = '" + strUPC + "'");

                string strNamer = Convert.ToString(dr["Name"]);

                DataRow[] drAll;
                if (strNamer == "NUTS" || strNamer == "ICE BAG")
                {
                    drAll = dtAllItems.Select(" Description = '" + strNamer + "'");
                }
                else
                {
                    drAll = dtAllItems.Select(" UPC = '" + strUPC + "'");
                }
                ItemID = Convert.ToInt32(drAll[0]["ItemID"]);
                string strNote = string.Empty;

                if (dataRow.Any())
                {
                    decimal bulkPrice = Convert.ToDecimal(dataRow[0]["Pricing"]);
                    string strQuan = Convert.ToString(dataRow[0]["Quantity"]);
                    if (Convert.ToInt32(strQuan) <= Convert.ToInt32(newQuantity))
                    {
                        strNote = "Bulk Pricing @" + strQuan + " for $" + Convert.ToString(bulkPrice);
                    }

                }


                BillingItem newItem = new BillingItem
                {
                    UPC = strUPC,
                    Name = Convert.ToString(dr["Name"]),
                    Price = Convert.ToString(dr["Price"]),
                    Quantity = Convert.ToString(dr["Quantity"]),
                    Tax = Convert.ToString(dr["Tax"]), // Format total price as a string with 2 decimals
                    Discount = Convert.ToString(dr["Discount"]),
                    TotalPrice = Convert.ToString(dr["TotalPrice"]), // Format total price as a string with 2 decimals
                    UserName = AccessRightsManager.GetUserName(),
                    Note = strNote,
                    ItemID = Convert.ToString(ItemID),
                };


                // Add the new item to the ObservableCollection
                objBillingItems.Add(newItem);
            }

            dgBilling.ItemsSource = null;
            dgBilling.ItemsSource = objBillingItems;

            // Clear the TextBox controls for new input
            txtUPC.Clear();
            txtName.Clear();
            txtQuantity.Text = "1";
        }

        public void ShowTextOnDisplay(string ItemName, string ItemQuantity, string ItemPrice)
        {
            //var mainWindow = (MainWindow)Application.Current.MainWindow;
            //mainWindow.DisplayText(ItemName + "     " + ItemQuantity, "$" + ItemPrice);
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
