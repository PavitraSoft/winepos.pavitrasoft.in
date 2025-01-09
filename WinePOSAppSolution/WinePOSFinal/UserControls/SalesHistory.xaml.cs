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
                dtInvoice = objService.FetchAndPopulateInvoice(isAdmin, null, null, string.Empty);

                // Bind to DataGrid
                SalesInventoryDataGrid.ItemsSource = dtInvoice.DefaultView;


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

        // Placeholder for the print logic
        private void PrintInvoice(string invoiceCode)
        {
            // Implement your actual print logic here
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
                report.SetParameterValue("InvoiceCode", invoiceCode);

                ReportViewerWindow viewer = new ReportViewerWindow();
                viewer.SetReport(report);
                viewer.Show();

                // Export the report to a PDF file
                //string exportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "WineBill.pdf");
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

        private void FlashReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (dtInvoice.Rows.Count > 0)
            {
                try
                {
                    // Create a new report document
                    ReportDocument report = new ReportDocument();


                    // path for development
                    //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    //string projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
                    //string targetFile = Path.Combine("Reports", "flashReport.rpt");
                    //string reportPath = Path.Combine(projectRoot, targetFile);

                    //path for static file
                    //string reportPath = System.IO.Path.Combine(@"H:\SOFTWARES\winepos.pavitrasoft.in-main\winepos.pavitrasoft.in-main\WinePOSAppSolution\WinePOSFinal\Reports\flashReport.rpt");
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
                                              .Where(row => row.Field<string>("PaymentType") == "CHECKS")
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

                    DateTime dateFrom = fromDate ?? DateTime.Now;
                    DateTime dateTo = toDate ?? DateTime.Now;

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
        }

        private void VoidInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SalesInventoryDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("No entries selected. Please select rows to void.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Collect all selected rows
            var selectedRows = SalesInventoryDataGrid.SelectedItems.Cast<DataRowView>().ToList();
            var invoiceCodes = selectedRows.Select(row => row["InvoiceCode"].ToString()).Distinct();

            foreach (var invoiceCode in invoiceCodes)
            {
                //MessageBox.Show($"Voiding all entries with InvoiceCode: {invoiceCode}");
                objService.VoidInvoice(Convert.ToInt32(invoiceCode));
            }

            FetchAndPopulateInvoice();
        }
    }
}
