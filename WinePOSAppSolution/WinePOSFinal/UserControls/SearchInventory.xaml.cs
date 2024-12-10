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
using WinePOSFinal.ServicesLayer;
using System.Data;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for SearchInventory.xaml
    /// </summary>
    public partial class SearchInventory : UserControl
    {

        WinePOSService objService = new WinePOSService();
        // This will hold the selected row
        private DataRowView selectedRow;

        public SearchInventory()
        {
            InitializeComponent();

            btnSearch_Click(null, null);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strDescription = txtDescription.Text;
            DataTable dtData = objService.GetInventoryData(strDescription);

            InventoryDataGrid.ItemsSource = dtData.DefaultView;
        }

        private void InventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected row
            selectedRow = (DataRowView)InventoryDataGrid.SelectedItem;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRow != null)
            {
                // Navigate to the second tab
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainTabControl.SelectedIndex = 2; // Select Tab 2

                // Call the method to populate data in UserControl2
                InventoryMaintenance InventoryMaintenance = mainWindow.InventoryMaintenance as InventoryMaintenance;
                if (InventoryMaintenance != null)
                {
                    int selectedId = (int)selectedRow["ItemID"];

                    // Fetch data based on the selected ID and populate the fields in UserControl2
                    InventoryMaintenance.PopulateData(selectedId);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to edit.");
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRow != null)
            {
                int selectedId = (int)selectedRow["ItemID"];

                if (objService.DeleteItemDataByID(selectedId))
                {
                    MessageBox.Show("Item Deleted Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnSearch_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Some error occurred while deleting the Item.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }
    }
}
