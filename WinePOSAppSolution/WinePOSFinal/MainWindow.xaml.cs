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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinePOSFinal.Classes;
using WinePOSFinal.UserControls;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string currentRole = AccessRightsManager.GetUserRole();

            Inventorymaintenance.Visibility = Visibility.Collapsed;
            SalesHistory.Visibility = Visibility.Collapsed;

            if (currentRole.ToUpper() == "ADMIN")
            {
                Inventorymaintenance.Visibility = Visibility.Visible;
                SalesHistory.Visibility = Visibility.Visible;
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //// Check which tab is selected and reload data accordingly
            //if (Billing.IsSelected)
            //{
            //    // Ensure the Billing content is initialized before trying to access it
            //    if (Billing.Content is Billing billingControl)
            //    {
            //        billingControl.ReloadBillingData(); // Reload Billing data
            //    }
            //}
            //else if (SearchInventory.IsSelected)
            //{
            //    // Ensure the SearchInventory content is initialized before trying to access it
            //    if (SearchInventory.Content is SearchInventory searchInventoryControl)
            //    {
            //        searchInventoryControl.ReloadSearchInventoryData(); // Reload Search Inventory data
            //    }
            //}
            //else if (Inventorymaintenance.IsSelected)
            //{
            //    // Ensure the Inventorymaintenance content is initialized before trying to access it
            //    if (Inventorymaintenance.Content is InventoryMaintenance inventoryMaintenanceControl)
            //    {
            //        inventoryMaintenanceControl.ReloadInventoryMaintenanceData(); // Reload Inventory Maintenance data
            //    }
            //}
            //else if (SalesHistory.IsSelected)
            //{
            //    // Ensure the SalesHistory content is initialized before trying to access it
            //    if (SalesHistory.Content is SalesHistory salesHistoryControl)
            //    {
            //        salesHistoryControl.ReloadSalesHistoryData(); // Reload Sales History data
            //    }
            //}
        }
    }
}
