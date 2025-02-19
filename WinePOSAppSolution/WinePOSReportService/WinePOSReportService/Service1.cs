using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Net.Mail;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WinePOSReportService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;


        private readonly Service objService = new Service();
        private string selectedInvoiceCode;
        private bool isAdmin = false;
        DataTable dtInvoice = new DataTable();
        DataTable dtPayment = new DataTable();

        public Service1()
        {
            InitializeComponent();
        }

        public new void OnStart(string[] args)
        {
            timer = new Timer();
            timer.Interval = GetNextInterval(); // Set the interval to the next 1 AM
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            // Perform your scheduled task here
            RunTask();

            // Reset the timer for the next 1 AM execution
            timer.Interval = GetNextInterval();
            timer.Start();
        }

        private double GetNextInterval()
        {
            DateTime now = DateTime.Now;
            DateTime nextRun = now.Date.AddDays(1).AddHours(1); // Default: Tomorrow 1 AM

            // If the current time is BEFORE 1 AM, schedule for today at 1 AM
            if (now.Hour < 1 || (now.Hour == 1 && now.Minute == 0 && now.Second == 0))
            {
                nextRun = now.Date.AddHours(1); // Today at 1 AM
            }

            TimeSpan timeUntilNextRun = nextRun - now;
            return timeUntilNextRun.TotalMilliseconds; // Return milliseconds for the Timer
        }

        public void RunTask()
        {
            string strStatus = string.Empty;
            string strError = string.Empty;
            try
            {

                DateTime fromDate = Convert.ToDateTime(objService.GetValueFromConfig("LastReportGeneratedDate"));
                DateTime toDate = DateTime.Now;
                string invoiceNumber = string.Empty;

                //dtInvoice = objService.FetchAndPopulateInvoice(isAdmin, fromDate, toDate, invoiceNumber);

                DataSet dsInvoice = objService.FetchAndPopulateInvoice(isAdmin, fromDate, toDate, invoiceNumber);

                dtInvoice = dsInvoice.Tables[0];
                dtPayment = dsInvoice.Tables[1];

                ReportGenerator(dtInvoice, dtPayment);

                strStatus = "Success";
                strError = string.Empty;

                objService.UpdateConfigValue("LastReportGeneratedDate", Convert.ToString(DateTime.Now));
            }
            catch (Exception ex)
            {
                strStatus = "Error";
                strStatus = ex.ToString();
            }
            finally
            {
                objService.UpdateConfigValue("LastServiceRunStatus", strStatus);
                objService.UpdateConfigValue("LastServiceRunError", strError);
            }
        }

        public new void OnStop()
        {
            timer?.Stop();
            timer?.Dispose();
        }

        public void ReportGenerator(DataTable dtInvoice, DataTable dtPayment)
        {
            string strReportExcelPath = GenerateExcel(dtInvoice, dtPayment, "REPORT");
            string strVoidExcelPath = GenerateExcel(dtInvoice, dtPayment, "VOID");
            string strReportPDFPath = GeneratePDF(dtInvoice, dtPayment, "REPORT");
            string strVoidPDFPath = GeneratePDF(dtInvoice, dtPayment, "VOID");

            DateTime reportDate = DateTime.Now; // You can replace this with a configurable date
            string subject = $"Daily Report - {GetFormattedDate(reportDate)}";

            SendEmail(strReportExcelPath, strVoidExcelPath, strReportPDFPath, strVoidPDFPath, subject, "Daily Report");
        }

        string GetFormattedDate(DateTime date)
        {
            int day = date.Day;
            string suffix = (day % 10 == 1 && day != 11) ? "st" :
                            (day % 10 == 2 && day != 12) ? "nd" :
                            (day % 10 == 3 && day != 13) ? "rd" : "th";

            string formattedDate = $"{day}{suffix} {date:MMM yyyy}";
            return formattedDate;
        }

        public string GenerateExcel(DataTable dtInvoice, DataTable dtPayment, string strReportType)
        {
            string excelFilePath = Path.Combine(Path.GetTempPath(), $"Report_{strReportType}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");

            // Group by ItemName and sum TotalPrice & Quantity
            var groupedData = dtInvoice.AsEnumerable()
                .Where(row => strReportType.ToUpper() == "VOID" ? row["IsVoided"].ToString().ToUpper() == "YES" : true)
                .GroupBy(row => row["Name"].ToString()) // Group by ItemName
                .Select(g => new
                {
                    ItemName = g.Key,
                    TotalPrice = g.Sum(row => Convert.ToDecimal(row["TotalPrice"])),
                    TotalQuantity = g.Sum(row => Convert.ToInt32(row["Quantity"]))
                })
                .ToList();

            // Calculate total sum
            decimal grandTotalPrice = groupedData.Sum(item => item.TotalPrice);
            int grandTotalQuantity = groupedData.Sum(item => item.TotalQuantity);

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

                // Add total row
                worksheet.Cell(row, 1).Value = "Total";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Value = grandTotalPrice;
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Value = grandTotalQuantity;
                worksheet.Cell(row, 3).Style.Font.Bold = true;

                // Auto-fit columns for better visibility
                worksheet.Columns().AdjustToContents();

                // Save the workbook
                workbook.SaveAs(excelFilePath);
            }

            return excelFilePath;

        }

        public string GeneratePDF(DataTable dtInvoice, DataTable dtPayment, string strReportType)
        {
            string pdfFilePath = Path.Combine(Path.GetTempPath(), $"Report_{strReportType}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            //using (FileStream stream = new FileStream(pdfFilePath, FileMode.Create))
            //{
            //    Document pdfDoc = new Document(PageSize.A4);
            //    PdfWriter.GetInstance(pdfDoc, stream);
            //    pdfDoc.Open();

            //    // Invoice Section
            //    pdfDoc.Add(new Paragraph("Invoice Data"));
            //    pdfDoc.Add(new Paragraph(" "));
            //    AddTableToPDF(pdfDoc, dtInvoice);

            //    pdfDoc.Add(new Paragraph(" "));

            //    // Payment Section
            //    pdfDoc.Add(new Paragraph("Payment Data"));
            //    pdfDoc.Add(new Paragraph(" "));
            //    AddTableToPDF(pdfDoc, dtPayment);
            //    pdfDoc.Close();
            //}

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Group by ItemName and sum TotalPrice & Quantity
                var groupedData = dtInvoice.AsEnumerable()
                    .Where(row => strReportType.ToUpper() == "VOID" ? row["IsVoided"].ToString().ToUpper() == "YES" : true)
                    .GroupBy(row => row["Name"].ToString()) // Group by ItemName
                    .Select(g => new
                    {
                        ItemName = g.Key,
                        TotalPrice = g.Sum(row => Convert.ToDecimal(row["TotalPrice"])),
                        TotalQuantity = g.Sum(row => Convert.ToInt32(row["Quantity"]))
                    })
                    .ToList();

                // Calculate total sum of TotalPrice
                decimal grandTotalPrice = groupedData.Sum(item => item.TotalPrice);
                int grandTotalQuantity = groupedData.Sum(item => item.TotalQuantity);

                // Create a new PDF document
                Document document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add Title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Paragraph title = new Paragraph("Flash Report\n\n", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                // Create a table with 3 columns
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 40f, 30f, 30f }); // Column widths

                // Add table headers
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("Item Name", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Amount ($)", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("#OfItem", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                // Add Data Rows
                Font rowFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                foreach (var item in groupedData)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.ItemName, rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.TotalPrice.ToString("0.00"), rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.TotalQuantity.ToString(), rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                }

                // Add grand total row
                Font totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                PdfPCell totalCell = new PdfPCell(new Phrase("Total", totalFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 1 };
                PdfPCell totalAmountCell = new PdfPCell(new Phrase(grandTotalPrice.ToString("0.00"), totalFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
                PdfPCell totalQuantityCell = new PdfPCell(new Phrase(grandTotalQuantity.ToString(), totalFont)) { HorizontalAlignment = Element.ALIGN_CENTER };

                table.AddCell(totalCell);
                table.AddCell(totalAmountCell);
                table.AddCell(totalQuantityCell);

                // Add table to document
                document.Add(table);
                document.Close();

                // Convert MemoryStream to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Save and open PDF
                File.WriteAllBytes(pdfFilePath, pdfBytes);
            }

            return pdfFilePath;
        }


        public void SendEmail(string strReportExcelPath, string strVoidExcelPath, string strReportPDFPath, string strVoidPDFPath, string subject, string body)
        {
            string smtpUser = Convert.ToString(objService.GetValueFromConfig("LowStockAlertEmail"));
            string smtpPassword = Convert.ToString(objService.GetValueFromConfig("LowStockAlertSMTPPassword"));

            using (MailMessage mail = new MailMessage(smtpUser, smtpUser, subject, body))
            {
                string smtpHost = "smtp.gmail.com"; // e.g., "smtp.gmail.com"
                int smtpPort = 587; // Usually 587 for TLS or 465 for SSL

                // Attach Excel and PDF files
                mail.Attachments.Add(new Attachment(strReportExcelPath));
                mail.Attachments.Add(new Attachment(strReportPDFPath));
                mail.Attachments.Add(new Attachment(strVoidExcelPath));
                mail.Attachments.Add(new Attachment(strVoidPDFPath));
                mail.IsBodyHtml = false;


                // Configure the SMTP client
                SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true // Use SSL/TLS encryption
                };

                // Send the email
                smtpClient.Send(mail);
            }

            // Clean up temporary files
            File.Delete(strReportExcelPath);
            File.Delete(strVoidExcelPath);
            File.Delete(strReportPDFPath);
            File.Delete(strVoidPDFPath);
        }
    }
}
