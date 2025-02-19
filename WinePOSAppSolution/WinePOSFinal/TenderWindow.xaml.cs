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
using Microsoft.PointOfService;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for TenderWindow.xaml
    /// </summary>
    public partial class TenderWindow : Window
    {
        private decimal TotalAmount = 0m; // Total amount passed from MainWindow
        private decimal RemainingAmount = 0m; // Remaining amount after payments
        private List<Payment> Payments = new List<Payment>();
        private TextBlock _lblRemaining;
        private TextBlock _lblAmtRemaining;
        private TextBlock _lblChange;
        private TextBlock _lblAmtChange;



        private PosExplorer explorer;

        private CashDrawer cashDrawer;
        private bool firstKeyPress;

        public TenderWindow(decimal initialAmount, TextBlock lblRemaining, TextBlock lblAmtRemaining, TextBlock lblChange, TextBlock lblAmtChange)
        {
            InitializeComponent();

            //InitializeCashDrawer();

            _lblRemaining = lblRemaining;
            _lblAmtRemaining = lblAmtRemaining;
            _lblChange = lblChange;
            _lblAmtChange = lblAmtChange;

            TotalAmount = initialAmount; // Set the initial amount passed from MainWindow
            RemainingAmount = TotalAmount; // Initialize remaining amount
            AmountTextBox.Text = TotalAmount.ToString("F2"); // Populate the amount text box
            UpdateRemainingAmount();
            PaymentGrid.ItemsSource = Payments;

            btnCustomAmount.Content = Math.Ceiling(initialAmount).ToString();
            firstKeyPress = true;
        }

        public TenderWindow()
        {
            InitializeComponent();
            //InitializeCashDrawer();
            TotalAmount = 0; // Set the initial amount passed from MainWindow
            RemainingAmount = TotalAmount; // Initialize remaining amount
            AmountTextBox.Text = TotalAmount.ToString("F2"); // Populate the amount text box
            UpdateRemainingAmount();
            PaymentGrid.ItemsSource = Payments;
        }

        private void InitializeCashDrawer()

        {

            try

            {

                explorer = new PosExplorer();

                DeviceInfo deviceInfo = explorer.GetDevice(DeviceType.CashDrawer, "CashDrawer");

                cashDrawer = (CashDrawer)explorer.CreateInstance(deviceInfo);

                cashDrawer.Open();

                cashDrawer.Claim(1000);

                cashDrawer.DeviceEnabled = true;

            }

            catch (Exception ex)

            {

                MessageBox.Show("Error initializing cash drawer: " + ex.Message);

            }

        }

        // Number Button Click Handler
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                if (firstKeyPress)
                {
                    AmountTextBox.Text = button.Content.ToString();
                    firstKeyPress = false;
                }
                else
                {
                    AmountTextBox.Text += button.Content.ToString();
                }
            }
        }


        // Clear Button Click Handler
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            AmountTextBox.Text = string.Empty;
            Payments.Clear();
            RemainingAmount = 0m;
            UpdateRemainingAmount();
            PaymentGrid.Items.Refresh();
            firstKeyPress = false;
        }

        // Quick Tender Button Click Handler
        private void QuickTenderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                firstKeyPress = false;
                decimal amount = decimal.Parse(button.Content.ToString().Trim('$'));
                AddPayment("Quick Tender", amount);
            }
        }

        // Add Payment Logic
        private void AddPayment(string type, decimal amount)
        {
            //if (RemainingAmount >= amount)
            //{
                Payments.Add(new Payment { Type = type, Amount = amount });
                RemainingAmount -= amount;
                UpdateRemainingAmount();
                PaymentGrid.Items.Refresh();
            //}
            //else
            //{
            //    MessageBox.Show("Amount exceeds the remaining balance!");
            //}
        }

        // Update Remaining Amount
        private void UpdateRemainingAmount()
        {
            RemainingAmountText.Text = $"Amount Remaining: ${RemainingAmount:F2}";
        }

        // Done Button Click Handler
        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (RemainingAmount > 0)
            {
                _lblRemaining.Visibility = Visibility.Visible;
                _lblAmtRemaining.Visibility = Visibility.Visible;
                _lblAmtRemaining.Text = Convert.ToString(RemainingAmount);
                _lblChange.Visibility = Visibility.Collapsed;
                _lblAmtChange.Visibility = Visibility.Collapsed;
            }
            else if (RemainingAmount < 0)
            {
                _lblRemaining.Visibility = Visibility.Collapsed;
                _lblAmtRemaining.Visibility = Visibility.Collapsed;
                _lblChange.Visibility = Visibility.Visible;
                _lblAmtChange.Visibility = Visibility.Visible;
                _lblAmtChange.Text = Convert.ToString(RemainingAmount * -1);
            }
            else
            {
                _lblRemaining.Visibility = Visibility.Collapsed;
                _lblAmtRemaining.Visibility = Visibility.Collapsed;
                _lblChange.Visibility = Visibility.Collapsed;
                _lblAmtChange.Visibility = Visibility.Collapsed;
            }

            OpenCashDrawer();
            this.Close(); // Close the TenderWindow
        }

        // Operator Button (+/-) Click Handler
        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                firstKeyPress = false;
                string operation = button.Content.ToString();
                if (operation == "+" && decimal.TryParse(AmountTextBox.Text, out decimal currentAmount))
                {
                    AmountTextBox.Text = (currentAmount + 1).ToString("F2");
                }
                else if (operation == "-" && decimal.TryParse(AmountTextBox.Text, out currentAmount))
                {
                    AmountTextBox.Text = (currentAmount - 1).ToString("F2");
                }
            }
        }

        // Handle AmountTextBox TextChanged
        private void AmountTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal enteredAmount))
            {
                TotalAmount = enteredAmount;
                RemainingAmount = TotalAmount - GetTotalPaid();
            }
            else
            {
                TotalAmount = 0m;
                RemainingAmount = 0m;
            }
            UpdateRemainingAmount();
        }



        // Helper to calculate the total amount paid
        private decimal GetTotalPaid()
        {
            decimal totalPaid = 0m;
            foreach (var payment in Payments)
            {
                totalPaid += payment.Amount;
            }
            return totalPaid;
        }

        // Payment class for the grid
        public class Payment
        {
            public string Type { get; set; }
            public decimal Amount { get; set; }
        }

        private void RemoveLastButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the AmountTextBox is not empty
            if (!string.IsNullOrEmpty(AmountTextBox.Text))
            {
                // Remove the last character
                AmountTextBox.Text = AmountTextBox.Text.Substring(0, AmountTextBox.Text.Length - 1);
            }

            // Update the remaining amount based on the new value
            UpdateRemainingAmountFromTextbox();
        }

        private void UpdateRemainingAmountFromTextbox()
        {
            // Parse the AmountTextBox value to update RemainingAmount
            if (decimal.TryParse(AmountTextBox.Text, out decimal enteredAmount))
            {
                RemainingAmount = enteredAmount - TotalAmountInGrid();
            }
            else
            {
                RemainingAmount = 0; // Reset if invalid
            }

            // Update the RemainingAmountText
            UpdateRemainingAmount();
        }

        private decimal TotalAmountInGrid()
        {
            // Calculate the total amount in the grid
            decimal total = 0;
            foreach (var item in PaymentGrid.Items)
            {
                if (item is Payment paymentRow)
                {
                    total += paymentRow.Amount;
                }
            }
            return total;
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure AmountTextBox is not empty and does not already contain a decimal point
            if (!AmountTextBox.Text.Contains("."))
            {
                if (firstKeyPress)
                {
                    AmountTextBox.Text = string.Empty;
                    firstKeyPress = false;
                }
                if (string.IsNullOrEmpty(AmountTextBox.Text))
                {
                    // If textbox is empty, start with "0."
                    AmountTextBox.Text = "0.";
                }
                else
                {
                    // Append a decimal point
                    AmountTextBox.Text += ".";
                }
            }
        }


        private void OpenCashDrawer()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            CashDrawer cashDrawer = mainWindow.cashDrawer;
            try
            {

                cashDrawer.Open();
                cashDrawer.Claim(5000);
                cashDrawer.DeviceEnabled = true;
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

            finally
            {
                // ✅ Ensure the device is properly released before initializing the printer
                if (cashDrawer != null)
                {
                    cashDrawer.DeviceEnabled = false;
                    cashDrawer.Release();
                    cashDrawer.Close();
                }
            }
        }

    }
}

