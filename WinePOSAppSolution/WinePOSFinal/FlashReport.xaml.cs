using System;
using System.Collections.Generic;
using System.Data;
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

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for FlashReport.xaml
    /// </summary>
    public partial class FlashReport : Window
    {
        public FlashReport(DataTable dtInvoice, DataTable dtPayment, DateTime? FromDate, DateTime? ToDate)
        {
            InitializeComponent();

            PopulateReport(dtInvoice, dtPayment, FromDate, ToDate);
        }

        private void PopulateReport(DataTable dtInvoice, DataTable dtPayment, DateTime? FromDate, DateTime? ToDate)
        {
            //decimal GrossSalesAmt = Convert.ToDecimal(dtInvoice.Compute("SUM(TotalPrice)", string.Empty));
            //decimal TaxAmt = Convert.ToDecimal(dtInvoice.Compute("SUM(Tax)", string.Empty));

            // Sum of prices where tax = 0
            //var sumTaxZero = dtInvoice.AsEnumerable()
            //                          .Where(row => row.Field<decimal>("Tax") == 0)
            //                          .Sum(row => row.Field<decimal>("Price") * row.Field<int>("Quantity"));

            var sumTaxZero = dtInvoice.AsEnumerable()
                          .Where(row => (row.Field<decimal>("Tax") == 0 ||
                                       (row.Field<decimal>("Tax") != 0 &&
                                        (row.Field<string>("PaymentType") == "CASH" ||
                                         row.Field<string>("PaymentType") == "CHECK"))))
                          .Sum(row => row.Field<decimal>("Price") * row.Field<int>("Quantity"));

            // Sum of prices where tax > 0
            //var sumTaxNonZero = dtInvoice.AsEnumerable()
            //                             .Where(row => row.Field<decimal>("Tax") > 0)
            //                             .Sum(row => row.Field<decimal>("Price") * row.Field<int>("Quantity"));

            var sumTaxNonZero = dtInvoice.AsEnumerable()
                             .Where(row => row.Field<decimal>("Tax") > 0 &&
                                          row.Field<string>("PaymentType") != "CASH" &&
                                          row.Field<string>("PaymentType") != "CHECK")
                             .Sum(row => (row.Field<decimal>("Price") + row.Field<decimal>("Tax")) * row.Field<int>("Quantity"));


            // Sum of prices where tax > 0
            //var TaxAmt = dtInvoice.AsEnumerable()
            //                             .Sum(row => row.Field<decimal>("Tax") * row.Field<int>("Quantity"));

            var TaxAmt = dtInvoice.AsEnumerable()
                      .Where(row => row.Field<string>("PaymentType") != "CASH" &&
                                    row.Field<string>("PaymentType") != "CHECK")
                      .Sum(row => row.Field<decimal>("Tax") * row.Field<int>("Quantity"));


            var TaxAmtCash = dtInvoice.AsEnumerable()
                      .Where(row => row.Field<string>("PaymentType") == "CASH")
                      .Sum(row => row.Field<decimal>("Tax") * row.Field<int>("Quantity"));


            var TaxAmtCheck = dtInvoice.AsEnumerable()
                      .Where(row => row.Field<string>("PaymentType") == "CHECK")
                      .Sum(row => row.Field<decimal>("Tax") * row.Field<int>("Quantity"));

            decimal NetSalesAmt = sumTaxZero + sumTaxNonZero;

            //var GrossSalesAmt = dtInvoice.AsEnumerable()
            //                             .Sum(row => row.Field<decimal>("TotalPrice"));

            decimal GrossSalesAmt = NetSalesAmt + TaxAmt;


            //var Cash = dtInvoice.AsEnumerable()
            //                          .Where(row => row.Field<string>("PaymentType") == "CASH")
            //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

            //var Cash = dtPayment.AsEnumerable()
            //                          .Where(row => row.Field<string>("PaymentType").ToUpper() == "CASH")
            //                          .Sum(row => row.IsNull("Price") ? 0 : row.Field<decimal>("Amount") * row.Field<int>("Quantity"));
            var Cash = (dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "CASH")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount"))) - TaxAmtCash;

            //var Checks = dtInvoice.AsEnumerable()
            //                          .Where(row => row.Field<string>("PaymentType") == "CHECK")
            //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));

            //var Checks = dtPayment.AsEnumerable()
            //                          .Where(row => row.Field<string>("PaymentType").ToUpper() == "CHECK")
            //                          .Sum(row => row.IsNull("Price") ? 0 : row.Field<decimal>("Price") * row.Field<int>("Quantity"));
            var Checks = (dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "CHECK")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount"))) - TaxAmtCheck;

            //var Credit = dtPayment.AsEnumerable()
            //                          .Where(row => row.Field<string>("PaymentType").ToUpper() == "CREDIT")
            //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));
            var Credit = dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "CREDIT")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount"));

            //var PalmPay = dtPayment.AsEnumerable()
            //                          .Where(row => row.Field<string>("PaymentType").ToUpper() == "PALMPAY")
            //                          .Sum(row => row.IsNull("TotalPrice") ? 0 : row.Field<decimal>("TotalPrice"));
            var PalmPay = dtPayment.AsEnumerable()
                                      .Where(row => row.Field<string>("PaymentType").ToUpper() == "PALMPAY")
                                      .Sum(row => row.IsNull("Amount") ? 0 : row.Field<decimal>("Amount"));

            string QuantitySold = Convert.ToString(dtInvoice.Compute("SUM(Quantity)", string.Empty));

            string Transactions = Convert.ToString(dtInvoice.AsEnumerable()
                                            .Select(row => row.Field<int>("InvoiceCode"))
                                            .Distinct()
                                            .Count());


            DateTime? fromDate = FromDate;
            DateTime? toDate = ToDate;

            //DateTime dateFrom = fromDate ?? dtInvoice.AsEnumerable().Min(row => row.Field<DateTime>("CreatedDateTime"));
            //DateTime dateTo = toDate ?? dtInvoice.AsEnumerable().Max(row => row.Field<DateTime>("CreatedDateTime"));

            txtDateFrom.Text = FromDate.Value.Date.AddHours(0).AddMinutes(0).AddSeconds(1).ToString("dd-MMM-yyyy HH:mm:ss");
            txtDateTo.Text = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd-MMM-yyyy HH:mm:ss");

            NetSales.Text = Convert.ToString(NetSalesAmt);
            NetSalesT.Text = Convert.ToString(sumTaxNonZero);
            NetSalesNT.Text = Convert.ToString(sumTaxZero);
            Tax.Text = Convert.ToString(TaxAmt);
            GrossSales.Text = Convert.ToString(GrossSalesAmt);

            txtCash.Text = Convert.ToString(Cash);
            txtChecks.Text = Convert.ToString(Checks);
            txtCredit.Text = Convert.ToString(Credit);
            txtPalmPay.Text = Convert.ToString(PalmPay);

            txtTransactions.Text = Convert.ToString(Transactions);
            txtAvgTransactions.Text = Convert.ToInt32(Transactions) != 0 ? (Convert.ToDecimal(GrossSalesAmt) / Convert.ToDecimal(Transactions)).ToString("0.00") : "0.00";

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current window
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
