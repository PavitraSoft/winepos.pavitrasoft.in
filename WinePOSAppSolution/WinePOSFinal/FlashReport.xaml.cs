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

        public FlashReport(DataTable dtInvoice, DataTable dtPayment, DateTime? FromDate, DateTime? ToDate)
        {
            InitializeComponent();

            _dtInvoice = dtInvoice;
            _dtPayment = dtPayment;

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

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current window
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    // Group by ItemName and sum TotalPrice & Quantity
                //    var groupedData = _dtInvoice.AsEnumerable()
                //        .GroupBy(row => row["Name"].ToString()) // Group by ItemName
                //        .Select(g => new
                //        {
                //            ItemName = g.Key,
                //            TotalPrice = g.Sum(row => Convert.ToDecimal(row["TotalPrice"])),
                //            TotalQuantity = g.Sum(row => Convert.ToInt32(row["Quantity"]))
                //        })
                //        .ToList();

                //    // Create a new PDF document
                //    Document document = new Document(PageSize.A4);
                //    PdfWriter.GetInstance(document, memoryStream);
                //    document.Open();

                //    // Add Title
                //    Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                //    Paragraph title = new Paragraph("Flash Report\n\n", titleFont);
                //    title.Alignment = Element.ALIGN_CENTER;
                //    document.Add(title);

                //    // Create a table with 3 columns
                //    PdfPTable table = new PdfPTable(3);
                //    table.WidthPercentage = 100;
                //    table.SetWidths(new float[] { 40f, 30f, 30f }); // Column widths

                //    // Add table headers
                //    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                //    table.AddCell(new PdfPCell(new Phrase("Item Name", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                //    table.AddCell(new PdfPCell(new Phrase("Amount ($)", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                //    table.AddCell(new PdfPCell(new Phrase("#OfItem", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                //    // Add Data Rows
                //    Font rowFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                //    foreach (var item in groupedData)
                //    {
                //        table.AddCell(new PdfPCell(new Phrase(item.ItemName, rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                //        table.AddCell(new PdfPCell(new Phrase(item.TotalPrice.ToString("0.00"), rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                //        table.AddCell(new PdfPCell(new Phrase(item.TotalQuantity.ToString(), rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                //    }

                //    // Add table to document
                //    document.Add(table);
                //    document.Close();

                //    // Convert MemoryStream to byte array
                //    byte[] pdfBytes = memoryStream.ToArray();

                //    // Open PDF directly from memory (use a temporary file)
                //    string tempFilePath = Path.Combine(Path.GetTempPath(), "FlashReport.pdf");
                //    File.WriteAllBytes(tempFilePath, pdfBytes);
                //    Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });

                //    //MessageBox.Show("PDF report generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //}

                DataTable InvoiceData = _dtInvoice;
                DataTable PaymentData = _dtPayment;

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

                PrintInvoice(name, price, quantity, tax, totalPrice, discount, strCashAmt, strCheckAmt, strCreditAmt, strPalmPayAmt, string.Empty);

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

            string strStoreName = objService.GetValueFromConfig("StoreName");
            string strAddress = objService.GetValueFromConfig("Address");
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
                    if (string.IsNullOrWhiteSpace(strbcData))
                    {
                        if (m_Printer.CapRecBarCode == true)
                        {
                            string barcodeData = ConvertInvoiceToEAN13(Convert.ToInt32(strbcData));

                            //Barcode printing
                            m_Printer.PrintBarCode(PrinterStation.Receipt, barcodeData,
                                BarCodeSymbology.EanJan13, 1000,
                                m_Printer.RecLineWidth, PosPrinter.PrinterBarCodeLeft,
                                BarCodeTextPosition.Below);
                        }
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
                    var worksheet = workbook.Worksheets.Add("Invoices");

                    // Add headers
                    worksheet.Cell(1, 1).Value = "ItemName";
                    worksheet.Cell(1, 2).Value = "Amount";
                    worksheet.Cell(1, 3).Value = "#OfItem";

                    // Apply styling (bold header)
                    worksheet.Range("A1:C1").Style.Font.Bold = true;

                    // Populate data from grouped results
                    int row = 2;
                    foreach (var item in groupedData)
                    {
                        worksheet.Cell(row, 1).Value = item.ItemName;
                        worksheet.Cell(row, 2).Value = item.TotalPrice;
                        worksheet.Cell(row, 3).Value = item.TotalQuantity;
                        row++;
                    }

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
