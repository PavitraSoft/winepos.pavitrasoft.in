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

namespace WinePOSFinal.UserControls
{
    public partial class SalesHistory : UserControl
    {
        private readonly WinePOSService objService = new WinePOSService();
        private string selectedInvoiceCode;
        private bool isAdmin = false;
        DataTable dtInvoice = new DataTable();

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

                if (currentRole.ToUpper() == "ADMIN")
                {
                    isAdmin = true;
                }

                // Fetch invoice data
                SearchButton_Click(null, null);

                // Bind to DataGrid
                //SalesInventoryDataGrid.ItemsSource = dtInvoice.DefaultView;


                string userRole = AccessRightsManager.GetUserRole(); // This is a placeholder method. Replace it with your actual role-fetching logic.

                if (userRole.ToLower() != "admin")
                {
                    // Hide the IsVoided column for non-admin users
                    var isVoidedColumn = SalesInventoryDataGrid.Columns.FirstOrDefault(col => col.Header.ToString() == "Voided");
                    if (isVoidedColumn != null)
                    {
                        isVoidedColumn.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    // Show the IsVoided column for admin users
                    var isVoidedColumn = SalesInventoryDataGrid.Columns.FirstOrDefault(col => col.Header.ToString() == "Voided");
                    if (isVoidedColumn != null)
                    {
                        isVoidedColumn.Visibility = Visibility.Visible;
                    }
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

                DataTable InvoiceData = objService.FetchAndPopulateInvoice(true, null, null, Convert.ToString(invoiceNumber));

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

                PrintInvoice(name, price, quantity, tax, totalPrice, discount, paymentType);

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

        private void PrintInvoice(string[] name, string[] price, string[] quantity, string[] tax, string[] totalPrice, string[] discount, string paymentType)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            PosPrinter m_Printer = mainWindow.m_Printer;
            //<<<step2>>>--Start
            //Initialization
            DateTime nowDate = DateTime.Now;                            //System date
            DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();   //Date Format
            dateFormat.MonthDayPattern = "MMMM";
            string strDate = nowDate.ToString("MMMM,dd,yyyy  HH:mm", dateFormat);
            string strbcData = "4902720005074";
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
                        + "123xxstreet,xxxcity,xxxxstate\n");

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|rA"
                        + "TEL 9999-99-9999   C#2\n");

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


                    strPrintData = MakePrintString(m_Printer.RecLineChars / 2, "Payment Type", "$"
                        + paymentType);

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + "\u001b|2C"
                        + strPrintData + "\n");

                    //Make 5mm speces
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|500uF");

                    //<<<step4>>>--Start
                    if (m_Printer.CapRecBarCode == true)
                    {
                        //Barcode printing
                        m_Printer.PrintBarCode(PrinterStation.Receipt, strbcData,
                            BarCodeSymbology.EanJan13, 1000,
                            m_Printer.RecLineWidth, PosPrinter.PrinterBarCodeLeft,
                            BarCodeTextPosition.Below);
                    }
                    //<<<step4>>>--End
                    //<<<step5>>>--End

                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");

                    strPrintData = "Thank you for shopping at Crown Liquor!";

                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");


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

            //<<<step6>>>--Start
            // When a cursor is back to its default shape, it means the process ends
            //Cursor.Current = Cursors.Default;
            //<<<step6>>>--End

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
            if (dtInvoice.Rows.Count > 0)
            {
                try
                { 

                    SearchButton_Click(null, null);

                    var addWindow = new FlashReport(dtInvoice, FromDatePicker.SelectedDate, ToDatePicker.SelectedDate);
                    addWindow.ShowDialog();


                    // Create a new report document
                    ReportDocument report = new ReportDocument();

                    // path for development
                    //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    //string projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
                    //string targetFile = Path.Combine("Reports", "flashReport.rpt");
                    //string reportPath = Path.Combine(projectRoot, targetFile);

                    //path for static file
                    //string reportPath = System.IO.Path.Combine(@"D:\Study\Dotnet\WinePOSGIT\winepos.pavitrasoft.in\WinePOSAppSolution\WinePOSFinal\Reports\flashReport.rpt");
                    //report.Load(reportPath);

                    // path fo live report
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    string targetFile = Path.Combine("Reports", "flashReport.rpt");


                    string reportPath = Path.Combine(baseDirectory, targetFile);

                    report.Load(reportPath);


                    decimal GrossSales = Convert.ToDecimal(dtInvoice.Compute("SUM(TotalPrice)", string.Empty));
                    decimal Tax = Convert.ToDecimal(dtInvoice.Compute("SUM(Tax)", string.Empty));

                    decimal NetSales = GrossSales - Tax;

                    var Cash = dtInvoice.AsEnumerable()
                                              .Where(row => row.Field<string>("PaymentType") == "CASH")
                                              .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    var Checks = dtInvoice.AsEnumerable()
                                              .Where(row => row.Field<string>("PaymentType") == "CHECK")
                                              .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    var Credit = dtInvoice.AsEnumerable()
                                              .Where(row => row.Field<string>("PaymentType") == "CREDIT")
                                              .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    var PalmPay = dtInvoice.AsEnumerable()
                                              .Where(row => row.Field<string>("PaymentType") == "PALMPAY")
                                              .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

                    string QuantitySold = Convert.ToString(dtInvoice.Compute("SUM(Quantity)", string.Empty));

                    string Transactions = Convert.ToString(dtInvoice.AsEnumerable()
                                                    .Select(row => row.Field<int>("InvoiceCode"))
                                                    .Distinct()
                                                    .Count());


                    DateTime? fromDate = FromDatePicker.SelectedDate;
                    DateTime? toDate = ToDatePicker.SelectedDate;

                    DateTime dateFrom = fromDate ?? dtInvoice.AsEnumerable().Min(row => row.Field<DateTime>("CreatedDateTime"));
                    DateTime dateTo = toDate ?? dtInvoice.AsEnumerable().Max(row => row.Field<DateTime>("CreatedDateTime"));

                    // Set database logon credentials (if required)
                    SetDatabaseLogin(report);

                    // Dynamically set the InvoiceCode parameter for the report
                    report.SetParameterValue("NetSales", "$" + Convert.ToString(NetSales));
                    report.SetParameterValue("Tax", "$" + Convert.ToString(Tax));
                    report.SetParameterValue("GrossSales", "$" + Convert.ToString(GrossSales));
                    report.SetParameterValue("QuantitySold", Convert.ToString(QuantitySold));
                    report.SetParameterValue("Cash", "$" + Convert.ToString(Cash));
                    report.SetParameterValue("Checks", "$" + Convert.ToString(Checks));
                    report.SetParameterValue("Credit", "$" + Convert.ToString(Credit));
                    report.SetParameterValue("PalmPay", "$" + Convert.ToString(PalmPay));
                    report.SetParameterValue("Transactions", Convert.ToString(Transactions));
                    report.SetParameterValue("DateFrom", dateFrom);
                    report.SetParameterValue("DateTo", dateTo);

                    ReportViewerWindow viewer = new ReportViewerWindow();
                    viewer.SetReport(report);
                    viewer.Show();

                    // Export the report to a PDF file
                    //string exportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "flashReport.pdf");
                    //report.ExportToDisk(ExportFormatType.PortableDocFormat, exportPath);

                    // Display the PDF in the WebBrowser control
                    //pdfWebViewer.Navigate(exportPath); // Navigate to the generated PDF file


                    // Optionally, open the generated report in a PDF viewer
                    //System.Diagnostics.Process.Start(exportPath);

                    //MessageBox.Show("Report generated and displayed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No Data To Show.", "Flash Report", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Get values from UI controls
            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;
            string invoiceNumber = InvoiceNumberTextBox.Text;

            dtInvoice = objService.FetchAndPopulateInvoice(isAdmin, fromDate, toDate, invoiceNumber);
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
            // Format the total price information
            // Get selected rows
            var selectedRows = SalesInventoryDataGrid.SelectedItems.Cast<DataRowView>().ToList();
            if (!selectedRows.Any())
            {
                TotalPriceLabel.Content = "Total Price: $0.00";
                return;
            }

            // Calculate total price for all selected rows
            decimal totalPrice = selectedRows.Where(row => row["IsVoided"].ToString().ToLower() == "no").Sum(row => (decimal)row["TotalPrice"]);

            // Update the label to show the total price
            TotalPriceLabel.Content = $"Total Price: ${totalPrice:0.00}";


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

            // Collect all selected rows
            var selectedRows = SalesInventoryDataGrid.SelectedItems.Cast<DataRowView>().ToList();
            var invoiceCodes = selectedRows.Select(row => row["InvoiceCode"].ToString()).Distinct();

            var result = MessageBox.Show($"Are you sure you want to void selected invoices?", "Confirm Change", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var invoiceCode in invoiceCodes)
                {
                    //MessageBox.Show($"Voiding all entries with InvoiceCode: {invoiceCode}");
                    strSelectedInvoiceCodes += invoiceCode.ToString() + ",";

                    //objService.VoidInvoice(Convert.ToInt32(invoiceCode));
                }

                if (!string.IsNullOrWhiteSpace(strSelectedInvoiceCodes))
                {
                    DataTable dtVoid = objService.VoidInvoicesByCodes(strSelectedInvoiceCodes.TrimEnd(','), AccessRightsManager.GetUserName());

                    if (dtVoid != null && dtVoid.Rows.Count > 0)
                    {
                        DataRow drVoid = dtVoid.Rows[0];

                        string IsAllowed = Convert.ToString(drVoid["IsAllowed"]);
                        string DateFrom = Convert.ToString(drVoid["DateFrom"]);
                        string DateTo = Convert.ToString(drVoid["DateTo"]);
                        string TotalInvoices = Convert.ToString(drVoid["TotalInvoices"]);
                        string VoidedInvoices = Convert.ToString(drVoid["VoidedInvoices"]);
                        string ToBeVoided = Convert.ToString(drVoid["ToBeVoided"]);
                        string Percentage = Convert.ToString(drVoid["Percentage"]);
                        string UserRole = Convert.ToString(drVoid["UserRole"]);

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
                            MessageBox.Show("Invoice voided successfully.", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show("No Invoice to Void.", "Void Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                // Cancel the edit
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
    }
}
