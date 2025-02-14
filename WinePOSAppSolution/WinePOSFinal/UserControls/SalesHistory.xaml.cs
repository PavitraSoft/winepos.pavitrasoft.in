using CrystalDecisions.Shared;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WinePOSFinal.Classes;
using WinePOSFinal.ServicesLayer;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.PointOfService;
using System.Globalization;
using static WinePOSFinal.TenderWindow;

namespace WinePOSFinal.UserControls
{
    public partial class SalesHistory : UserControl
    {
        private readonly WinePOSService objService = new WinePOSService();
        private string selectedInvoiceCode;
        private bool isAdmin = false;
        DataTable dtInvoice = new DataTable();
        DataTable dtPayment = new DataTable();

        public SalesHistory()
        {
            InitializeComponent();
            ReloadSalesHistoryData();
        }


        public void ReloadSalesHistoryData()
        {
            FromDatePicker.SelectedDate = DateTime.Today;
            ToDatePicker.SelectedDate = DateTime.Today;
            FetchAndPopulateInvoice();
        }

        private void FetchAndPopulateInvoice()
        {
            try
            {
                string currentRole = AccessRightsManager.GetUserRole();

                if (currentRole.ToUpper() == "ADMIN" || currentRole.ToUpper() == "MANAGER")
                {
                    isAdmin = true;
                }

                // Fetch invoice data
                SearchButton_Click(null, null);

                // Bind to DataGrid
                //SalesInventoryDataGrid.ItemsSource = dtInvoice.DefaultView;


                string userRole = AccessRightsManager.GetUserRole(); // This is a placeholder method. Replace it with your actual role-fetching logic.

                if (userRole.ToLower() == "admin" || userRole.ToLower() == "manager")
                {
                    // Show the IsVoided column for admin users
                    var isVoidedColumn = SalesInventoryDataGrid.Columns.FirstOrDefault(col => col.Header.ToString() == "Voided");
                    if (isVoidedColumn != null)
                    {
                        isVoidedColumn.Visibility = Visibility.Visible;
                    }
                    FlashReportButton.Visibility = Visibility.Visible;
                    VoidInvoice.Visibility = Visibility.Visible;
                }
                else
                {
                    // Hide the IsVoided column for non-admin users
                    var isVoidedColumn = SalesInventoryDataGrid.Columns.FirstOrDefault(col => col.Header.ToString() == "Voided");
                    if (isVoidedColumn != null)
                    {
                        isVoidedColumn.Visibility = Visibility.Collapsed;
                    }
                    FlashReportButton.Visibility = Visibility.Collapsed;
                    VoidInvoice.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching invoice data: {ex.Message}");
            }
        }

        // Handle Row Selection
        //private void SalesInventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (SalesInventoryDataGrid.SelectedItem is DataRowView selectedRow)
        //    {
        //        try
        //        {
        //            selectedInvoiceCode = selectedRow["InvoiceCode"]?.ToString();
        //        }
        //        catch
        //        {
        //            MessageBox.Show("Error retrieving the InvoiceCode from the selected row. Ensure the data context is correct.");
        //            selectedInvoiceCode = null;
        //        }
        //    }
        //}

        // Handle Print Invoice Button Click
        private void PrintInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedInvoiceCode))
            {
                // Call the method to generate and display the Crystal Report
                PrintInvoice(selectedInvoiceCode);
            }
            else
            {
                MessageBox.Show("Please select a row before printing the invoice.");
            }
        }



        private void PrintInvoice(string invoiceNumber)
        {
            try
            {

                //DataTable InvoiceData = objService.FetchAndPopulateInvoice(true, null, null, Convert.ToString(invoiceNumber));
                DataSet dsInvoiceData = objService.FetchAndPopulateInvoice(true, null, null, Convert.ToString(invoiceNumber));

                DataTable InvoiceData = dsInvoiceData.Tables[0];
                DataTable PaymentData = dsInvoiceData.Tables[1];

                string[] name = InvoiceData.AsEnumerable()
                             .Select(row => row.Field<string>("Name").ToString())
                             .ToArray();

                string[] price = InvoiceData.AsEnumerable()
                             .Select(row => row.Field<decimal>("Price").ToString())
                             .ToArray();

                string[] quantity = InvoiceData.AsEnumerable()
                             .Select(row => row.Field<int>("Quantity").ToString())
                             .ToArray();

                string[] tax = InvoiceData.AsEnumerable()
                             .Select(row => row.Field<decimal>("Tax").ToString())
                             .ToArray();

                string[] totalPrice = InvoiceData.AsEnumerable()
                             .Select(row => row.Field<decimal>("TotalPrice").ToString())
                             .ToArray();

                string[] discount = InvoiceData.AsEnumerable()
                             .Select(row => row.Field<decimal>("Discount").ToString())
                             .ToArray();

                string paymentType = Convert.ToString(InvoiceData.Rows[0]["PaymentType"]);

                string strCashAmt = string.Empty;
                string strCheckAmt = string.Empty;
                string strCreditAmt = string.Empty;
                string strPalmPayAmt = string.Empty;

                foreach (DataRow dataRow in PaymentData.Rows)
                {
                    string strPaymentType = Convert.ToString(dataRow["PaymentType"]).ToUpper();
                    decimal Amount = Convert.ToDecimal(dataRow["Amount"]);

                    if (Amount > 0)
                    {
                        if (strPaymentType == "CASH")
                            strCashAmt = Amount.ToString("G29");
                        else if (strPaymentType == "CHECK")
                            strCheckAmt = Amount.ToString("G29");
                        else if (strPaymentType == "CREDIT")
                            strCreditAmt = Amount.ToString("G29");
                        else if (strPaymentType == "PALMPAY")
                            strPalmPayAmt = Amount.ToString("G29");
                    }
                }

                PrintInvoice(name, price, quantity, tax, totalPrice, discount, strCashAmt, strCheckAmt, strCreditAmt, strPalmPayAmt, Convert.ToString(invoiceNumber));

                //// Create a new report document
                //ReportDocument report = new ReportDocument();


                //// Load the report (winebill.rpt)
                ////string reportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Reports\winebill.rpt");
                //string reportPath = System.IO.Path.Combine(@"D:\Study\Dotnet\WinePOSGIT\winepos.pavitrasoft.in\WinePOSAppSolution\WinePOSFinal\Reports\winebill.rpt");

                //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //// Target file
                //string targetFile = Path.Combine("Reports", "winebill.rpt");

                //// Combine base directory with the relative path
                ////string reportPath = Path.Combine(baseDirectory, targetFile);
                //report.Load(reportPath);

                //// Create and populate the DataTable
                ////DataTable dt = objService.GetInventoryData(string.Empty, string.Empty);

                //// Set the DataTable as the data source for the report
                ////report.SetDataSource(dt);

                //// Set database logon credentials (if required)
                //SetDatabaseLogin(report);

                //// Dynamically set the InvoiceCode parameter for the report
                //report.SetParameterValue("InvoiceCode", invoiceNumber);

                //// Export the report to a PDF file
                //string exportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "WineBill.pdf");
                //report.ExportToDisk(ExportFormatType.PortableDocFormat, exportPath);

                //// Display the PDF in the WebBrowser control
                ////pdfWebViewer.Navigate(exportPath); // Navigate to the generated PDF file


                //// Optionally, open the generated report in a PDF viewer
                //System.Diagnostics.Process.Start(exportPath);

                //MessageBox.Show("Report generated and displayed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintInvoice(string[] name, string[] price, string[] quantity, string[] tax, string[] totalPrice, string[] discount, string strCashAmt, string strCheckAmt, string strCreditAmt, string strPalmPayAmt, string invoiceNumber)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            PosPrinter m_Printer = mainWindow.m_Printer;
            //<<<step2>>>--Start
            //Initialization
            DateTime nowDate = DateTime.Now;                            //System date
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   //Date Format
            dateFormat.MonthDayPattern = "MMMM";
            string strDate = nowDate.ToString("MMMM,dd,yyyy  HH:mm", dateFormat);
            string strbcData = invoiceNumber;


            string strAddress = objService.GetValueFromConfig("Address");
            string strStoreName = objService.GetValueFromConfig("StoreName");
            string strPhone = objService.GetValueFromConfig("Phone");

            //String[] astritem = { "apples", "grapes", "bananas", "lemons", "oranges" };
            //String[] astrprice = { "10.00", "20.00", "30.00", "40.00", "50.00" };

            if (m_Printer.CapRecPresent)
            {

                try
                {
                    //<<<step6>>>--Start
                    //Batch processing mode
                    m_Printer.TransactionPrint(PrinterStation.Receipt
                        , PrinterTransactionControl.Transaction);

                    //<<<step3>>>--Start
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|1B");
                    //<<<step3>>>--End

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|N"
                        + strStoreName + "\n");

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|N"
                        + strAddress + "\n");

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|rA"
                        + "TEL " + strPhone + "\n");

                    //<<<step5>>--Start
                    //Make 2mm speces
                    //ESC|#uF = Line Feed
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|200uF");
                    //<<<step5>>>-End

                    int iRecLineCharsCount = m_Printer.RecLineCharsList.Length;
                    if (iRecLineCharsCount >= 2)
                    {
                        m_Printer.RecLineChars = m_Printer.RecLineCharsList[1];
                        m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|cA" + strDate + "\n");
                        m_Printer.RecLineChars = m_Printer.RecLineCharsList[0];
                    }
                    else
                    {
                        m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|cA" + strDate + "\n");
                    }

                    //<<<step5>>>--Start
                    //Make 5mm speces
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|500uF");

                    //Print buying goods
                    double total = 0.0;
                    string strPrintData = "";
                    for (int i = 0; i < name.Length; i++)
                    {
                        decimal itemTotal = Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(price[i]);

                        string strDiscount = (Convert.ToDecimal(discount[i]) != 0) ? "* (" + Convert.ToString(discount[i]) + "%)" : string.Empty;

                        strPrintData = MakePrintString(m_Printer.RecLineChars, name[i] + strDiscount, "   " + quantity[i] + " @ $" + price[i] + " $"
                            + (Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(price[i])));

                        m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                        total += Convert.ToDouble(itemTotal);

                    }

                    //Make 2mm speces
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|200uF");

                    //Print the total cost
                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Tax excluded."
                        , "$" + total.ToString("F"));

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + strPrintData + "\n");

                    decimal totaltax = tax.Select(item => Convert.ToDecimal(item)).Sum();
                    decimal totalPriceAfterTax = totalPrice.Select(item => Convert.ToDecimal(item)).Sum();

                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Tax ", "$"
                        + (totaltax).ToString("F"));

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|uC" + strPrintData + "\n");

                    strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "Total", "$"
                        + (totalPriceAfterTax).ToString("F"));

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
                        + strPrintData + "\n");

                    //strPrintData = MakePrintString(m_Printer.RecLineChars, "Customer's payment", "$200.00");

                    m_Printer.PrintNormal(PrinterStation.Receipt
                        , strPrintData + "\n");

                    //strPrintData = MakePrintString(m_Printer.RecLineChars, "Change", "$" + (200.00 - (total * 1.05)).ToString("F"));

                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                    if (!string.IsNullOrWhiteSpace(strCashAmt))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "CASH", "$"
                            + strCashAmt);

                        m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
                            + strPrintData + "\n");
                    }
                    if (!string.IsNullOrWhiteSpace(strCheckAmt))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "CHECK", "$"
                            + strCheckAmt);

                        m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
                            + strPrintData + "\n");
                    }
                    if (!string.IsNullOrWhiteSpace(strPalmPayAmt))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "PALM PAY", "$"
                            + strPalmPayAmt);

                        m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
                            + strPrintData + "\n");
                    }
                    if (!string.IsNullOrWhiteSpace(strCreditAmt))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "CREDIT", "$"
                            + strCreditAmt);

                        m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
                            + strPrintData + "\n");
                    }

                    //strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "Payment Type", "$"
                    //    + paymentType);

                    //m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
                    //    + strPrintData + "\n");

                    //Make 5mm speces
                    //m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|500uF");

                    //<<<step4>>>--Start
                    if (m_Printer.CapRecBarCode == true)
                    {
                        string barcodeData = ConvertInvoiceToEAN13(Convert.ToInt32(strbcData));

                        //Barcode printing
                        m_Printer.PrintBarCode(PrinterStation.Receipt, barcodeData,
                            BarCodeSymbology.EanJan13, 1000,
                            m_Printer.RecLineWidth, PosPrinter.PrinterBarCodeLeft,
                            BarCodeTextPosition.Below);
                    }
                    //<<<step4>>>--End


                    //Make 5mm speces
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|500uF");

                    strPrintData = "Thank you for shopping at Crown Liquor!";

                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                    //<<<step5>>>--End


                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");
                    //<<<step2>>>--End

                    //print all the buffer data. and exit the batch processing mode.
                    m_Printer.TransactionPrint(PrinterStation.Receipt
                        , PrinterTransactionControl.Normal);
                    //<<<step6>>>--End
                }
                catch (PosControlException ex)
                {
                    MessageBox.Show("Error while printing invoice. Exception:" + ex.ToString(), "Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }

        public static string ConvertInvoiceToEAN13(int invoiceNumber)
        {
            // Convert invoice number to string
            string base12Digits = invoiceNumber.ToString();

            // Ensure it's at least 12 digits by padding with leading zeros
            base12Digits = base12Digits.PadLeft(12, '0');

            // Calculate EAN-13 checksum
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = base12Digits[i] - '0'; // Convert char to integer
                sum += (i % 2 == 0) ? digit : digit * 3; // Odd position: digit * 1, Even position: digit * 3
            }

            int checksum = (10 - (sum % 10)) % 10; // Compute the checksum
            return base12Digits + checksum; // Return valid 13-digit barcode
        }

        // Placeholder for the print logic
        //private void PrintInvoice(string invoiceCode)
        //{
        //    // Implement your actual print logic here
        //    try
        //    {
        //        // Create a new report document
        //        ReportDocument report = new ReportDocument();

        //        // Load the report (winebill.rpt)
        //        //string reportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Reports\winebill.rpt");
        //        //string reportPath = System.IO.Path.Combine(@"D:\Study\Dotnet\WinePOSGIT\winepos.pavitrasoft.in\WinePOSAppSolution\WinePOSFinal\Reports\winebill.rpt");

        //        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //        // Target file
        //        string targetFile = Path.Combine("Reports", "winebill.rpt");

        //        // Combine base directory with the relative path
        //        string reportPath = Path.Combine(baseDirectory, targetFile);
        //        report.Load(reportPath);

        //        // Create and populate the DataTable
        //        //DataTable dt = objService.GetInventoryData(string.Empty, string.Empty);

        //        // Set the DataTable as the data source for the report
        //        //report.SetDataSource(dt);

        //        // Set database logon credentials (if required)
        //        SetDatabaseLogin(report);

        //        // Dynamically set the InvoiceCode parameter for the report
        //        report.SetParameterValue("InvoiceCode", invoiceCode);

        //        ReportViewerWindow viewer = new ReportViewerWindow();
        //        viewer.SetReport(report);
        //        viewer.Show();

        //        // Export the report to a PDF file
        //        //string exportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "WineBill.pdf");
        //        //report.ExportToDisk(ExportFormatType.PortableDocFormat, exportPath);

        //        // Display the PDF in the WebBrowser control
        //        //pdfWebViewer.Navigate(exportPath); // Navigate to the generated PDF file


        //        // Optionally, open the generated report in a PDF viewer
        //        //System.Diagnostics.Process.Start(exportPath);

        //        //MessageBox.Show("Report generated and displayed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void FlashReportButton_Click(object sender, RoutedEventArgs e)
        {
            //if (dtInvoice.Rows.Count > 0)
            //{
            //    try
            //    { 

                    SearchButton_Click(null, null);

                    var addWindow = new FlashReport(dtInvoice, dtPayment, FromDatePicker.SelectedDate, ToDatePicker.SelectedDate);
                    addWindow.ShowDialog();


                    // Create a new report document
                    //ReportDocument report = new ReportDocument();

                    //// path for development
                    ////string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    ////string projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
                    ////string targetFile = Path.Combine("Reports", "flashReport.rpt");
                    ////string reportPath = Path.Combine(projectRoot, targetFile);

                    ////path for static file
                    ////string reportPath = System.IO.Path.Combine(@"D:\Study\Dotnet\WinePOSGIT\winepos.pavitrasoft.in\WinePOSAppSolution\WinePOSFinal\Reports\flashReport.rpt");
                    ////report.Load(reportPath);

                    //// path fo live report
                    //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    //string targetFile = Path.Combine("Reports", "flashReport.rpt");


                    //string reportPath = Path.Combine(baseDirectory, targetFile);

                    //report.Load(reportPath);


                    //decimal GrossSales = Convert.ToDecimal(dtInvoice.Compute("SUM(TotalPrice)", string.Empty));
                    //decimal Tax = Convert.ToDecimal(dtInvoice.Compute("SUM(Tax)", string.Empty));

                    //decimal NetSales = GrossSales - Tax;

                    //var Cash = dtInvoice.AsEnumerable()
                    //                          .Where(row => row.Field<string>("PaymentType") == "CASH")
                    //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    //var Checks = dtInvoice.AsEnumerable()
                    //                          .Where(row => row.Field<string>("PaymentType") == "CHECK")
                    //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    //var Credit = dtInvoice.AsEnumerable()
                    //                          .Where(row => row.Field<string>("PaymentType") == "CREDIT")
                    //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    //var PalmPay = dtInvoice.AsEnumerable()
                    //                          .Where(row => row.Field<string>("PaymentType") == "PALMPAY")
                    //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    //string QuantitySold = Convert.ToString(dtInvoice.Compute("SUM(Quantity)", string.Empty));

                    //string Transactions = Convert.ToString(dtInvoice.AsEnumerable()
                    //                                .Select(row => row.Field<int>("InvoiceCode"))
                    //                                .Distinct()
                    //                                .Count());


                    //DateTime? fromDate = FromDatePicker.SelectedDate;
                    //DateTime? toDate = ToDatePicker.SelectedDate;

                    //DateTime dateFrom = fromDate ?? dtInvoice.AsEnumerable().Min(row => row.Field<DateTime>("CreatedDateTime"));
                    //DateTime dateTo = toDate ?? dtInvoice.AsEnumerable().Max(row => row.Field<DateTime>("CreatedDateTime"));

                    //// Set database logon credentials (if required)
                    //SetDatabaseLogin(report);

                    //// Dynamically set the InvoiceCode parameter for the report
                    //report.SetParameterValue("NetSales", "$" + Convert.ToString(NetSales));
                    //report.SetParameterValue("Tax", "$" + Convert.ToString(Tax));
                    //report.SetParameterValue("GrossSales", "$" + Convert.ToString(GrossSales));
                    //report.SetParameterValue("QuantitySold", Convert.ToString(QuantitySold));
                    //report.SetParameterValue("Cash", "$" + Convert.ToString(Cash));
                    //report.SetParameterValue("Checks", "$" + Convert.ToString(Checks));
                    //report.SetParameterValue("Credit", "$" + Convert.ToString(Credit));
                    //report.SetParameterValue("PalmPay", "$" + Convert.ToString(PalmPay));
                    //report.SetParameterValue("Transactions", Convert.ToString(Transactions));
                    //report.SetParameterValue("DateFrom", dateFrom);
                    //report.SetParameterValue("DateTo", dateTo);

                    //ReportViewerWindow viewer = new ReportViewerWindow();
                    //viewer.SetReport(report);
                    //viewer.Show();

                    // Export the report to a PDF file
                    //string exportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "flashReport.pdf");
                    //report.ExportToDisk(ExportFormatType.PortableDocFormat, exportPath);

                    // Display the PDF in the WebBrowser control
                    //pdfWebViewer.Navigate(exportPath); // Navigate to the generated PDF file


                    // Optionally, open the generated report in a PDF viewer
                    //System.Diagnostics.Process.Start(exportPath);

                    //MessageBox.Show("Report generated and displayed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    catch (Exception ex)
            //    {
            //        //MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("No Data To Show.", "Flash Report", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Get values from UI controls
            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;
            string invoiceNumber = InvoiceNumberTextBox.Text;

            //dtInvoice = objService.FetchAndPopulateInvoice(isAdmin, fromDate, toDate, invoiceNumber);

            DataSet dsInvoice = objService.FetchAndPopulateInvoice(isAdmin, fromDate, toDate, invoiceNumber);

            dtInvoice = dsInvoice.Tables[0];
            dtPayment = dsInvoice.Tables[1];

            // Bind to DataGrid
            SalesInventoryDataGrid.ItemsSource = dtInvoice.DefaultView;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear filters (if applicable)
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            InvoiceNumberTextBox.Text = string.Empty;

            // Clear DataGrid selection
            SalesInventoryDataGrid.SelectedItems.Clear();

            // Reset total price label
            TotalPriceLabel.Content = "Total Price: $0.00";

            SearchButton_Click(null, null);
        }

        private void SalesInventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get selected rows and filter only cash payment invoices that are not voided
            var selectedRows = SalesInventoryDataGrid.SelectedItems.Cast<DataRowView>()
                //.Where(row => row["PaymentType"]?.ToString().Equals("Cash", StringComparison.OrdinalIgnoreCase) == true
                //           && row["IsVoided"]?.ToString().Equals("No", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            // If no valid cash payment invoice is selected, show $0.00
            if (!selectedRows.Any())
            {
                TotalPriceLabel.Content = "Total Price: $0.00";
                return;
            }

            // Calculate total price for selected cash invoices
            decimal totalPrice = selectedRows.Sum(row => Convert.ToDecimal(row["TotalPrice"]));

            // Update the label to show the total price
            TotalPriceLabel.Content = $"Total Price: ${totalPrice:0.00}";

            // Set the selected invoice code
            if (SalesInventoryDataGrid.SelectedItem is DataRowView selectedRow)
            {
                try
                {
                    selectedInvoiceCode = selectedRow["InvoiceCode"]?.ToString();
                }
                catch
                {
                    MessageBox.Show("Error retrieving the InvoiceCode from the selected row. Ensure the data context is correct.");
                    selectedInvoiceCode = null;
                }
            }
        }


        private void VoidInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SalesInventoryDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("No entries selected. Please select rows to void.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string strSelectedInvoiceCodes = string.Empty;

            // Collect selected rows and filter only those with PaymentType = "Cash"
            var selectedRows = SalesInventoryDataGrid.SelectedItems.Cast<DataRowView>()
                .Where(row => row["PaymentType"]?.ToString().Equals("Cash", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            if (selectedRows.Count == 0)
            {
                MessageBox.Show("No valid cash payment invoice selected to void.", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var invoiceCodes = selectedRows.Select(row => row["InvoiceCode"].ToString()).Distinct();

            var result = MessageBox.Show($"Are you sure you want to void selected cash invoices?", "Confirm Change", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var invoiceCode in invoiceCodes)
                {
                    strSelectedInvoiceCodes += invoiceCode + ",";
                }

                if (!string.IsNullOrWhiteSpace(strSelectedInvoiceCodes))
                {
                    DataTable dtVoid = objService.VoidInvoicesByCodes(strSelectedInvoiceCodes.TrimEnd(','), AccessRightsManager.GetUserName());

                    if (dtVoid != null && dtVoid.Rows.Count > 0)
                    {
                        DataRow drVoid = dtVoid.Rows[0];

                        string IsAllowed = Convert.ToString(drVoid["IsAllowed"]);
                        string TotalInvoices = Convert.ToString(drVoid["TotalInvoices"]);
                        string VoidedInvoices = Convert.ToString(drVoid["VoidedInvoices"]);
                        string ToBeVoided = Convert.ToString(drVoid["ToBeVoided"]);

                        if (IsAllowed == "False")
                        {
                            if (Convert.ToInt32(ToBeVoided) > 0)
                            {
                                MessageBox.Show($"Could not void invoice (Limit Exhausted).\n Total Invoices: {TotalInvoices}\n Voided Invoices: {VoidedInvoices}\n Invoices To Void: {ToBeVoided}", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("No Invoice found to Void.", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cash invoices voided successfully.", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Some error occurred while voiding invoices.", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    FetchAndPopulateInvoice();
                }
                else
                {
                    MessageBox.Show("No valid cash invoice to void.", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
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

        private void EditInvoice_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(selectedInvoiceCode))
            {
                // Get reference to the MainWindow
                var mainWindow = (MainWindow)Application.Current.MainWindow;

                if (mainWindow != null)
                {
                    // Get the content inside the "Billing" TabItem (assuming it's a UserControl)
                    var billingControl = mainWindow.Billing.Content as Billing;

                    if (billingControl != null)
                    {
                        // Call the method inside Billing user control
                        billingControl.PopulateInvoiceData(Convert.ToInt32(selectedInvoiceCode));
                        // Switch to the Billing tab
                        mainWindow.MainTabControl.SelectedItem = mainWindow.Billing;
                    }
                    else
                    {
                        MessageBox.Show("Billing UserControl is not properly loaded.");
                    }
                }

                // Call the method to generate and display the Crystal Report
                //PrintInvoice(selectedInvoiceCode);
            }
            else
            {
                MessageBox.Show("Please select a row before editing the invoice.");
            }

        }
    }
}
