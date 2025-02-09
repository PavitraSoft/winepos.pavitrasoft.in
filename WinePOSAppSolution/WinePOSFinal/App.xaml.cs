using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MainAppInstance { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Manually create and show the Login window
            Login loginWindow = new Login();
            this.MainWindow = loginWindow; // Set it as the main window
            loginWindow.Show();
        }

        public static void SetMainAppInstance(MainWindow mainApp)
        {
            MainAppInstance = mainApp;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Call CleanupResources in MainAppWindow before exit
            if (MainAppInstance != null)
            {
                MainAppInstance.Window_Closing();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
