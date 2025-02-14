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
        bool _Prompt = false;

        public Login()
        {
            _Prompt = false;
            InitializeComponent();
        }

        public Login(bool Prompt)
        {
            _Prompt = Prompt;
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
                if (_Prompt)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    // Open the Main Application Window
                    MainWindow mainApp = new MainWindow();
                    App.SetMainAppInstance(mainApp);  // Store reference to main window

                    mainApp.Show();
                    Application.Current.MainWindow = mainApp;
                    this.Close(); // Close the login window

                    //// If login is successful, hide the current window and show the new window
                    //MainWindow mainwindow = new MainWindow(); // Replace with your actual dashboard window
                    //mainwindow.Show();

                    //// Update the application's MainWindow reference
                    //Application.Current.MainWindow = mainwindow;
                    //this.Hide();
                }
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
                if (!_Prompt) //No need to set user role if prompted.
                {
                    AccessRightsManager.SetUserRole(role);
                    AccessRightsManager.SetUserName(username);
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(LoginButton, new RoutedEventArgs()); // Call the Login button click event
            }
        }
    }
}
