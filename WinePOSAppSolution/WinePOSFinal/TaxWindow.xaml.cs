using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using WinePOSFinal.ServicesLayer;
using System.Data;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for TaxWindow.xaml
    /// </summary>
    public partial class TaxWindow : Window
    {
        public ObservableCollection<TaxData> TaxList { get; set; }
        WinePOSService objService = new WinePOSService();

        public TaxWindow()
        {
            InitializeComponent();

            DataTable dtTax = objService.GetTaxData();

            // Create a list to hold the tax data
            TaxList = new ObservableCollection<TaxData>();

            foreach (DataRow dr in dtTax.Rows)
            {
                // Add a new TaxData object to the TaxList
                TaxList.Add(new TaxData
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    Type = Convert.ToString(dr["Type"]),
                    Percentage = Convert.ToDecimal(dr["Percentage"])
                });
            }

            // Bind data to DataGrid
            TaxDataGrid.ItemsSource = TaxList;
        }

        // Click event for the Save button
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TaxData taxData in TaxList)
            {

                objService.SaveTaxData(taxData.ID, taxData.Percentage);
            }

            // Optional: Close the window after saving
            this.Close();
        }


        // Validate input when the user finishes editing a cell
        private void TaxDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Percentage of Tax")
            {
                var textBox = e.EditingElement as TextBox;
                if (textBox != null)
                {
                    decimal parsedValue;
                    bool isValidDecimal = decimal.TryParse(textBox.Text, out parsedValue);

                    if (isValidDecimal)
                    {
                        if (parsedValue >= 0 && parsedValue <= 99.999m)
                        {
                            // Valid input, update the value in the grid
                            var taxData = e.Row.Item as TaxData;
                            if (taxData != null)
                            {
                                taxData.Percentage = Math.Round(parsedValue, 3);
                            }
                        }
                        else
                        {
                            // Show validation error if out of bounds
                            MessageBox.Show("Percentage must be between 0 and 99.999.", "Invalid Value", MessageBoxButton.OK, MessageBoxImage.Error);
                            e.Cancel = true;  // Prevent the edit from being committed
                        }
                    }
                    else
                    {
                        // Show validation error if not a valid decimal
                        MessageBox.Show("Please enter a valid decimal value (e.g., 12.345).", "Invalid Value", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;  // Prevent the edit from being committed
                    }
                }
            }
        }
    }

    // Data model for the Tax information
    public class TaxData
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public decimal Percentage { get; set; }
    }
}
