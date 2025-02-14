using Microsoft.PointOfService;
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
using WinePOSFinal.Classes;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for SplitPayment.xaml
    /// </summary>
    public partial class SplitPayment : Window
    {
        private decimal totalAmount = 50m; // Example total amount
        private decimal remainingAmount;
        private List<Payments> _paymentList = new List<Payments>(); // Renamed to avoid conflict

        public SplitPayment(List<Payments> paymentList, decimal amt)
        {
            _paymentList = paymentList;
            InitializeComponent();
            InitializePayment(amt);
        }

        private void InitializePayment(decimal amt)
        {
            totalAmount = amt; 
            remainingAmount = totalAmount;
            txtTotalAmount.Text = $"Total Amount: ${totalAmount.ToString("G29")}";
            txtRemainingAmount.Text = $"Remaining Amount: ${remainingAmount.ToString("G29")}";
            _paymentList.Clear();
            lstPayments.Items.Clear(); // lstPayments remains for ListBox
        }

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtAmount.Text, out decimal enteredAmount) && enteredAmount > 0)
            {
                if (remainingAmount < enteredAmount)
                {
                    MessageBox.Show("Entered amount exceeds the remaining amount!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Button clickedButton = sender as Button;
                string paymentType = clickedButton.Content.ToString();

                // Check if the payment type already exists in the list
                var existingPayment = _paymentList.FirstOrDefault(p => p.Type == paymentType);

                if (existingPayment != null)
                {
                    // If payment type exists, update its amount
                    existingPayment.Amount += enteredAmount;
                }
                else
                {
                    // If it's a new payment type, add it to the list
                    _paymentList.Add(new Payments(paymentType, enteredAmount));
                }

                // Update UI ListBox
                RefreshPaymentList();

                // Update remaining amount
                remainingAmount -= enteredAmount;
                txtRemainingAmount.Text = $"Remaining Amount: ${remainingAmount.ToString("G29")}";

                // Clear input field
                txtAmount.Text = "";
            }
            else
            {
                MessageBox.Show("Please enter a valid amount!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (remainingAmount > 0)
            {
                MessageBox.Show("Payment is not complete. Please pay the remaining amount!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                //MessageBox.Show("Payment completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Close the window and return success
                OpenCashDrawer();
                this.DialogResult = true;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InitializePayment(totalAmount);
            txtAmount.Text = "";
        }

        private void FillRemainingAmount_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.Text = remainingAmount.ToString("G29");
        }

        private void RefreshPaymentList()
        {
            lstPayments.Items.Clear();
            foreach (var payment in _paymentList)
            {
                lstPayments.Items.Add($"{payment.Type}: ${payment.Amount.ToString("G29")}");
            }
        }

        private void OpenCashDrawer()
        {
            try
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                CashDrawer cashDrawer = mainWindow.cashDrawer;

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
        }
    }
}
