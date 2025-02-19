using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using WinePOSFinal.ServicesLayer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using System.Security.Policy;
using System.Web.Util;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for FlashReport.xaml
    /// </summary>
    public partial class FlashReport : Window
    {

        private readonly WinePOSService objService = new WinePOSService();
        //private string selectedInvoiceCode
        DataTable _dtInvoice;
        DataTable _dtPayment;

        DateTime? _FromDate;
        DateTime? _ToDate;

        public FlashReport(DataTable dtInvoice, DataTable dtPayment, DateTime? FromDate, DateTime? ToDate)
        {
            InitializeComponent();

            _dtInvoice = dtInvoice;
            _dtPayment = dtPayment;

            _FromDate = FromDate;
            _ToDate = ToDate;

            PopulateReport(FromDate, ToDate);
        }

        private void PopulateReport(DateTime? FromDate, DateTime? ToDate)
        {
            var sumTaxZero = _dtInvoice.AsEnumerable()
                          .Where(row => row.Field<decimal>("Tax") == 0)
                          .Sum(row => row.Field<decimal>("TotalPrice"));


            var sumTaxNonZero = _dtInvoice.AsEnumerable()
                             .Where(row => row.Field<decimal>("Tax") > 0)
                             .Sum(row => (row.Field<decimal>("TotalPrice")));



            var TaxAmt = _dtInvoice.AsEnumerable()
                      .Sum(row => row.Field<decimal>("Tax") * row.Field<int>("Quantity"));


            //var TaxAmtCash = dtInvoice.AsEnumerable()
            //          .Where(row => row.Field<string>("PaymentType") == "CASH")
            //          .Sum(row => row.Field<decimal>("Tax") * row.Field<int>("Quantity"));


            //var TaxAmtCheck = dtInvoice.AsEnumerable()
            //          .Where(row => row.Field<string>("PaymentType") == "CHECK")
            //          .Sum(row => row.Field<decimal>("Tax") * row.Field<int>("Quantity"));

            decimal NetSalesAmt = sumTaxZero + sumTaxNonZero - TaxAmt;


            decimal GrossSalesAmt = sumTaxZero + sumTaxNonZero;

            var Cash = (_dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "CASH")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount")));

            var Checks = (_dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "CHECK")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount")));

            var Credit = _dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "CREDIT")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount"));

            var PalmPay = _dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "PALMPAY")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount"));

            string QuantitySold = Convert.ToString(_dtInvoice.Compute("SUM(Quantity)", string.Empty));

            string Transactions = Convert.ToString(_dtInvoice.AsEnumerable()
                                            .Select(row => row.Field<int>("InvoiceCode"))
                                            .Distinct()
                                            .Count());


            DateTime? fromDate = FromDate;
            DateTime? toDate = ToDate;

            txtDateFrom.Text = FromDate.Value.Date.AddHours(0).AddMinutes(0).AddSeconds(0).ToString("dd-MMM-yyyy HH:mm:ss");
            txtDateTo.Text = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd-MMM-yyyy HH:mm:ss");

            NetSales.Text = Convert.ToString(Math.Round(NetSalesAmt,2));
            NetSalesT.Text = Convert.ToString(Math.Round(sumTaxNonZero - TaxAmt, 2));
            NetSalesNT.Text = Convert.ToString(Math.Round(sumTaxZero, 2));
            Tax.Text = Convert.ToString(Math.Round(TaxAmt, 2));
            GrossSales.Text = Convert.ToString(Math.Round(GrossSalesAmt, 2));

            txtCash.Text = Convert.ToString(Math.Round(Cash, 2));
            txtChecks.Text = Convert.ToString(Math.Round(Checks, 2));
            txtCredit.Text = Convert.ToString(Math.Round(Credit, 2));
            txtPalmPay.Text = Convert.ToString(Math.Round(PalmPay, 2));

            txtTransactions.Text = Convert.ToString(Transactions);
            txtAvgTransactions.Text = Convert.ToInt32(Transactions) != 0 ? Convert.ToString(Math.Round(Convert.ToDecimal(GrossSalesAmt) / Convert.ToDecimal(Transactions),2)) : "0.00";

        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get Date Range
                string reportDateFrom = _FromDate.Value.Date.AddHours(0).AddMinutes(0).AddSeconds(0).ToString("dd-MMM-yyyy HH:mm:ss");
                string reportDateTo = _ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd-MMM-yyyy HH:mm:ss");
                string reportDate = $"From: {reportDateFrom}  To: {reportDateTo}";


                // Financial Data Extraction from Labels
                string netSales = NetSales.Text;
                string netSalesT = NetSalesT.Text;  // Taxable Net Sales
                string netSalesNT = NetSalesNT.Text; // Non-Taxable Net Sales
                string totalTax = Tax.Text;
                string totalSales = GrossSales.Text;

                // Payment Breakdown from Labels
                string cashSales = txtCash.Text;
                string checkSales = txtChecks.Text;
                string cardSales = txtCredit.Text;
                string palmPaySales = txtPalmPay.Text;

                // Transaction Details from Labels
                string transactions = txtTransactions.Text;
                string avgTransaction = txtAvgTransactions.Text;

                // Call the Print Function with extracted values
                PrintInvoice(reportDate, totalSales, totalTax, netSalesT, netSales, netSalesNT, cashSales, cardSales, checkSales, palmPaySales, transactions, avgTransaction);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current window
        }



        private void PrintInvoice(string reportDate, string totalSales, string totalTax, string netSalesT, string netSales, string netSalesNT, string cashSales, string cardSales, string checkSales, string palmPaySales, string transactions, string avgTransaction)
        {

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            PosPrinter m_Printer = mainWindow.m_Printer;

            try
            {
                // Open Printer
                m_Printer.Open();
                m_Printer.Claim(1000);
                m_Printer.DeviceEnabled = true;

                if (m_Printer.CapRecPresent)
                {
                    // Start Batch Printing
                    m_Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Transaction);

                    // Print Header
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|1B");
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|cA" + "FLASH REPORT\n");
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|cA" + reportDate + "\n");
                    m_Printer.PrintNormal(PrinterStation.Receipt, "----------------------------------------\n");

                    // Print Sales Summary

                    string strPrintData;

                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Net Sales:", "$" + netSales);
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + strPrintData + "\n");

                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Net Sales - Taxed:", "$" + netSalesT);
                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Net Sales - NOT Taxed:", "$" + netSalesNT);
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|bC" + strPrintData + "\n");

                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Taxes:", "$" + totalTax);
                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Gross Sales:", "$" + totalSales);
                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                    // Payment Breakdown
                    m_Printer.PrintNormal(PrinterStation.Receipt, "----------------------------------------\n");

                    if (!string.IsNullOrWhiteSpace(cashSales))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars, "Cash:", "$" + cashSales);
                        m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");
                    }

                    if (!string.IsNullOrWhiteSpace(cardSales))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars, "Credit/Debit\r\n:", "$" + cardSales);
                        m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");
                    }

                    if (!string.IsNullOrWhiteSpace(checkSales))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars, "Checks:", "$" + checkSales);
                        m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");
                    }

                    if (!string.IsNullOrWhiteSpace(palmPaySales))
                    {
                        strPrintData = MakePrintString(m_Printer.RecLineChars, "PalmPAY:", "$" + palmPaySales);
                        m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");
                    }

                    // Transaction Info
                    m_Printer.PrintNormal(PrinterStation.Receipt, "----------------------------------------\n");
                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Transactions:", transactions);
                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                    strPrintData = MakePrintString(m_Printer.RecLineChars, "Average Transaction\r\n:", "$" + avgTransaction);
                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");

                    // Footer
                    m_Printer.PrintNormal(PrinterStation.Receipt, "----------------------------------------\n");
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|cA" + "End of Report\n");

                    // Cut Paper
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");

                    // End Batch Printing
                    m_Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Normal);
                }
            }
            catch (PosControlException ex)
            {
                MessageBox.Show("Error while printing flash report. Exception: " + ex.ToString(), "Flash Report", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                if (m_Printer != null)
                {
                    m_Printer.DeviceEnabled = false;
                    m_Printer.Release();
                    m_Printer.Close();
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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Define the custom folder name
                string customFileName = "FlashReport//" + GetTimestampedFileName("xlsx");

                // Combine the app directory with the custom folder name to get the full folder path
                string fullFolderPath = System.IO.Path.Combine(appDirectory, customFileName);

                // Group by ItemName and sum TotalPrice & Quantity
                var groupedData = _dtInvoice.AsEnumerable()
                    .GroupBy(row => row["Name"].ToString()) // Group by ItemName
                    .Select(g => new
                    {
                        ItemName = g.Key,
                        TotalPrice = g.Sum(row => Convert.ToDecimal(row["TotalPrice"])),
                        TotalQuantity = g.Sum(row => Convert.ToInt32(row["Quantity"]))
                    })
                    .ToList();

                // Create a new Excel workbook
                using (var workbook = new XLWorkbook())
                {
                    //var worksheet = workbook.Worksheets.Add("Invoices");

                    //// Add headers
                    //worksheet.Cell(1, 1).Value = "ItemName";
                    //worksheet.Cell(1, 2).Value = "Amount";
                    //worksheet.Cell(1, 3).Value = "#OfItem";

                    //// Apply styling (bold header)
                    //worksheet.Range("A1:C1").Style.Font.Bold = true;

                    //// Populate data from grouped results
                    //int row = 2;
                    //foreach (var item in groupedData)
                    //{
                    //    worksheet.Cell(row, 1).Value = item.ItemName;
                    //    worksheet.Cell(row, 2).Value = item.TotalPrice;
                    //    worksheet.Cell(row, 3).Value = item.TotalQuantity;
                    //    row++;
                    //}

                    //// Auto-fit columns for better visibility
                    //worksheet.Columns().AdjustToContents();

                    //// Save the workbook
                    //workbook.SaveAs(fullFolderPath);

                    var worksheet = workbook.Worksheets.Add("Flash Report");

                    int row = 1; // Start at first row

                    // Title
                    worksheet.Cell(row, 1).Value = "FLASH REPORT";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Cell(row, 1).Style.Font.FontSize = 16;
                    worksheet.Range(row, 1, row, 3).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    row += 2; // Move to next section

                    // Date Range
                    worksheet.Cell(row, 1).Value = "From: " + txtDateFrom.Text;
                    worksheet.Cell(row, 3).Value = "To: " + txtDateTo.Text;
                    worksheet.Row(row).Style.Font.Bold = true;
                    row += 2;

                    // **Sales Totals**
                    worksheet.Cell(row, 1).Value = "SALES TOTALS";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;

                    worksheet.Cell(row, 1).Value = "Net Sales";
                    worksheet.Cell(row, 2).Value = NetSales.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "Net Sales - Taxed";
                    worksheet.Cell(row, 2).Value = NetSalesT.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "Net Sales - NOT Taxed";
                    worksheet.Cell(row, 2).Value = NetSalesNT.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "Taxes";
                    worksheet.Cell(row, 2).Value = Tax.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "Gross Sales";
                    worksheet.Cell(row, 2).Value = GrossSales.Text;
                    row += 2;

                    // **Payment Breakdown**
                    worksheet.Cell(row, 1).Value = "PAYMENT TYPE BREAKDOWN";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;

                    worksheet.Cell(row, 1).Value = "Cash";
                    worksheet.Cell(row, 2).Value = txtCash.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "Checks";
                    worksheet.Cell(row, 2).Value = txtChecks.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "Credit/Debit";
                    worksheet.Cell(row, 2).Value = txtCredit.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "PalmPAY";
                    worksheet.Cell(row, 2).Value = txtPalmPay.Text;
                    row += 2;

                    // **Transaction Statistics**
                    worksheet.Cell(row, 1).Value = "TRANSACTION STATISTICS";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;

                    worksheet.Cell(row, 1).Value = "Transactions";
                    worksheet.Cell(row, 2).Value = txtTransactions.Text;
                    row++;

                    worksheet.Cell(row, 1).Value = "Average Transaction";
                    worksheet.Cell(row, 2).Value = txtAvgTransactions.Text;
                    row++;

                    // Auto-fit columns for better visibility
                    worksheet.Columns().AdjustToContents();

                    // Save the workbook
                    workbook.SaveAs(fullFolderPath);

                    // Open the file after saving
                    Process.Start(new ProcessStartInfo(fullFolderPath) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string GetTimestampedFileName(string fileExtension)
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Format the timestamp (e.g., "2024-12-21_14-23-45")
            string timestamp = now.ToString("yyyy_MM_dd_HH_mm_ss");

            // Append the file extension
            return $"{timestamp}.{fileExtension}";
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
