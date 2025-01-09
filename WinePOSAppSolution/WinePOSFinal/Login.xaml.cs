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
using WinePOSFinal.ServicesLayer;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        WinePOSService objService = new WinePOSService();
        public Login()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUserName.Text;
            string password = txtPassword.Password;

            // Clear previous status message
            StatusMessage.Visibility = Visibility.Collapsed;

            // Validate login credentials
            if (IsValidLogin(username, password))
            {
                // If login is successful, hide the current window and show the new window
                MainWindow mainwindow = new MainWindow(); // Replace with your actual dashboard window
                mainwindow.Show();

                // Update the application's MainWindow reference
                Application.Current.MainWindow = mainwindow;
                this.Hide();
            }
            else
            {
                // Show an error message if login is invalid
                StatusMessage.Text = "Invalid username or password.";
                StatusMessage.Visibility = Visibility.Visible;
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            string role = objService.ValidateLogin(username, password);

            if (!string.IsNullOrWhiteSpace(role))
            {
                AccessRightsManager.SetUserRole(role);
                AccessRightsManager.SetUserName(username);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
