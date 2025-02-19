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
using System.Windows.Shapes;
using WinePOSFinal.ServicesLayer;
using System.Data;
using ClosedXML.Excel;
using Microsoft.PointOfService;
using System.Diagnostics;
using System.Globalization;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for HourlyReport.xaml
    /// </summary>
    public partial class HourlyReport : Window
    {

        private readonly WinePOSService objService = new WinePOSService();
        //private string selectedInvoiceCode
        DataTable _dtInvoice;
        DataTable _dtPayment;

        public HourlyReport()
        {
            InitializeComponent();
        }

        public HourlyReport(DataTable dtInvoice, DataTable dtPayment, DateTime? FromDate, DateTime? ToDate)
        {
            InitializeComponent();

            _dtInvoice = dtInvoice;
            _dtPayment = dtPayment;

            PopulateReport(FromDate, ToDate);
        }

        private void PopulateReport(DateTime? FromDate, DateTime? ToDate)
        {

            string strReportType = "VOID"; // Example: Can be dynamic from user input

            // Grouping data into a new DataTable
            DataTable dtGrouped = new DataTable();
            dtGrouped.Columns.Add("ItemName", typeof(string));
            dtGrouped.Columns.Add("TotalPrice", typeof(decimal));
            dtGrouped.Columns.Add("TotalQuantity", typeof(int));

            var groupedData = _dtInvoice.AsEnumerable()
                .GroupBy(row => row["Name"].ToString()) // Group by ItemName
                .Select(g => new
                {
                    ItemName = g.Key,
                    TotalPrice = g.Sum(row => Convert.ToDecimal(row["TotalPrice"])),
                    TotalQuantity = g.Sum(row => Convert.ToInt32(row["Quantity"]))
                });

            // Populate the DataTable with grouped results
            foreach (var item in groupedData)
            {
                dtGrouped.Rows.Add(item.ItemName, item.TotalPrice, item.TotalQuantity);
            }

            // Bind to DataGrid
            dataGrid.ItemsSource = dtGrouped.DefaultView;

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current window
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DataTable dtGrouped = new DataTable();
                dtGrouped.Columns.Add("ItemName", typeof(string));
                dtGrouped.Columns.Add("TotalPrice", typeof(decimal));
                dtGrouped.Columns.Add("TotalQuantity", typeof(int));

                var groupedData = _dtInvoice.AsEnumerable()
                .GroupBy(row => row["Name"].ToString()) // Group by ItemName
                .Select(g => new
                {
                    ItemName = g.Key,
                    TotalPrice = g.Sum(row => Convert.ToDecimal(row["TotalPrice"])),
                    TotalQuantity = g.Sum(row => Convert.ToInt32(row["Quantity"]))
                });

                // Populate the DataTable with grouped results
                foreach (var item in groupedData)
                {
                    dtGrouped.Rows.Add(item.ItemName, item.TotalPrice, item.TotalQuantity);
                }



                PrintInvoice(dtGrouped, string.Empty);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void PrintInvoice(DataTable dtData, string invoiceNumber)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            PosPrinter m_Printer = mainWindow.m_Printer;


            try
            {
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

                m_Printer.Open();
                m_Printer.Claim(1000);
                m_Printer.DeviceEnabled = true;
                if (m_Printer.CapRecPresent)
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
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|20uF");
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
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|50uF");

                    //Print buying goods
                    string strPrintData = "";

                    foreach (DataRow dr in dtData.Rows)
                    {

                        string strItemName = Convert.ToString(dr["ItemName"]);
                        string strItemAmount = Convert.ToString(dr["TotalPrice"]);
                        string strTotalQuantity = Convert.ToString(dr["TotalQuantity"]);


                        strPrintData = MakePrintString(m_Printer.RecLineChars, strItemName, "   " + "$" + strItemAmount + " " + strTotalQuantity);

                        m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");
                    }

                    //Make 2mm speces
                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|50uF");



                    strPrintData = "Thank you for shopping at Crown Liquor!";

                    m_Printer.PrintNormal(PrinterStation.Receipt, strPrintData + "\n");


                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|50uF");

                    //<<<step5>>>--End


                    m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|fP");
                    //<<<step2>>>--End

                    //print all the buffer data. and exit the batch processing mode.
                    m_Printer.TransactionPrint(PrinterStation.Receipt
                        , PrinterTransactionControl.Normal);
                    //<<<step6>>>--End
                }
            }
            catch (PosControlException ex)
            {
                MessageBox.Show("Error while printing invoice. Exception:" + ex.ToString(), "Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            finally
            {
                if (m_Printer != null)
                {
                    m_Printer.DeviceEnabled = false;
                    m_Printer.Release();
                    m_Printer.Close();
                    Console.WriteLine("✅ Printer Released.");
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
