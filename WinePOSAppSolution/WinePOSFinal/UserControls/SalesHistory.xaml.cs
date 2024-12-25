using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WinePOSFinal.ServicesLayer;

namespace WinePOSFinal.UserControls
{
    public partial class SalesHistory : UserControl
    {
        private readonly WinePOSService objService = new WinePOSService();
        private string selectedInvoiceCode;

        public SalesHistory()
        {
            InitializeComponent();
            FetchAndPopulateInvoice();
        }

        private void FetchAndPopulateInvoice()
        {
            try
            {
                // Fetch invoice data
                DataTable dtInvoice = objService.FetchAndPopulateInvoice(true);

                // Bind to DataGrid
                SalesInventoryDataGrid.ItemsSource = dtInvoice.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching invoice data: {ex.Message}");
            }
        }

        // Handle Row Selection
        private void SalesInventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            MessageBox.Show($"Invoice {invoiceCode} sent to the printer!");
        }
    }
}
