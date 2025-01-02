using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using WinePOSFinal.ServicesLayer;
using System.Data;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using WinePOSFinal.Classes;
using DocumentFormat.OpenXml.Vml.Office;

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
        DataTable dtInventoryData = new DataTable();
        public SearchInventory()
        {
            InitializeComponent();

            ReloadSearchInventoryData();
        }

        public void ReloadSearchInventoryData()
        {
            btnSearch_Click(null, null);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strDescription = txtDescription.Text;
            string strUPC = txtUPC.Text;
            dtInventoryData = objService.GetInventoryData(strUPC, strDescription);

            InventoryDataGrid.ItemsSource = dtInventoryData.DefaultView;
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
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
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
            var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mainWindow.MainTabControl.SelectedIndex = 2; // Select Tab 2

            // Call the method to populate data in UserControl2
            InventoryMaintenance InventoryMaintenance = mainWindow.InventoryMaintenance as InventoryMaintenance;
            if (InventoryMaintenance != null)
            {
                // Fetch data based on the selected ID and populate the fields in UserControl2
                InventoryMaintenance.btnClear_Click(null, null);
            }
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

        private void InventoryDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                int i = 0;

                string headerName = e.Column.Header.ToString();
                string columnName = string.Empty;
                string newValue = string.Empty;
                string oldValue = string.Empty;

                var selectedEditRow = e.Row.Item as DataRowView;
                var editingElement = e.EditingElement as TextBox;

                if (selectedEditRow != null && editingElement != null)
                {
                    switch (headerName)
                    {
                        case "UPC":
                            columnName = "UPC";
                            break;
                        case "Description":
                            columnName = "Name";
                            break;
                        case "Item Cost":
                            columnName = "ItemCost";
                            break;
                        case "Price Cost":
                            columnName = "ChargedCost";
                            break;
                        case "Sales Tax":
                            columnName = "SalesTax";
                            break;
                        case "Stock":
                            columnName = "InStock";
                            break;
                        case "Additional Description":
                            columnName = "Additional_Description";
                            break;
                        case "Vendor Part Num.":
                            columnName = "VendorPartNum";
                            break;
                        case "Vendor Name.":
                            columnName = "VendorName";
                            break;
                    }

                    //value = editingElement.Text;

                    newValue = editingElement.Text;
                    oldValue = selectedEditRow[columnName]?.ToString();

                    // Check if the new value is the same as the old value
                    if (newValue == oldValue)
                    {
                        MessageBox.Show("No changes were made. The value remains the same.",
                                        "No Changes",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);

                        // Cancel the edit
                        e.Cancel = true;
                        return;
                    }

                    // Ask for confirmation
                    var result = MessageBox.Show($"Are you sure you want to change the value of '{columnName}' from '{oldValue}' to '{newValue}'?",
                                                 "Confirm Change",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        int itemID = Convert.ToInt32(selectedEditRow["ItemID"]);

                        // Save the changes
                        objService.SaveInlineItemData(itemID, columnName, newValue);
                    }
                    else
                    {
                        // Cancel the edit
                        e.Cancel = true;
                        editingElement.Text = oldValue;
                    }

                    //int ItemID = Convert.ToInt32(selectedEditRow["ItemID"]);

                    //objService.SaveInlineItemData(ItemID, columnName, value);
                }
            }
        }

        private void IntegerOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            int totalLength = text.Replace(".", "").Length;
            return Regex.IsMatch(text, "^[0-9]+$") && totalLength <= 8;
        }

        private void DecimalOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Get the TextBox where the input is being entered
            var textBox = sender as TextBox;

            // Allow only numbers with up to two decimal places
            e.Handled = !IsTextValidDecimal(textBox.Text, e.Text);
        }

        private static bool IsTextValidDecimal(string currentText, string newInput)
        {
            // Combine the current text and the new input
            string fullText = currentText + newInput;

            // Regular expression for a valid decimal number (e.g., 123, 123.45, or .45)
            Regex regex = new Regex(@"^\d*\.?\d{0,2}$");

            int totalLength = fullText.Replace(".", "").Length;

            return regex.IsMatch(fullText) && totalLength <= 12;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define the custom folder name
            string customFileName = "InventoryData//" + GetTimestampedFileName("xlsx");

            // Combine the app directory with the custom folder name to get the full folder path
            string fullFolderPath = System.IO.Path.Combine(appDirectory, customFileName);

            ExportDataTableToExcel(dtInventoryData, fullFolderPath);
        }

        public void ExportDataTableToExcel(DataTable dataTable, string filePath)
        {
            // Create a new workbook
            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet to the workbook
                var worksheet = workbook.AddWorksheet("Inventory");

                // Insert the DataTable into the worksheet starting at the first row and column
                worksheet.Cell(1, 1).InsertTable(dataTable);

                // Save the workbook to the specified file path
                workbook.SaveAs(filePath);
            }

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        public static string GetTimestampedFileName(string fileExtension)
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Format the timestamp (e.g., "2024-12-21_14-23-45")
            string timestamp = now.ToString("yyyy_MM_dd_HH_mm_ss");

            // Append the file extension
            return $"{timestamp}.{fileExtension}";
        }

        public static DataTable LoadExcelToDataTable(string filePath)
        {
            // Create a new DataTable to store the Excel data
            DataTable dt = new DataTable();

            // Ensure the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }

            // Load the Excel file
            using (var workbook = new XLWorkbook(filePath))
            {
                // Get the first worksheet
                var worksheet = workbook.Worksheet(1);

                // Read the header row (assumes headers are in the first row)
                bool isFirstRow = true;
                foreach (var row in worksheet.RowsUsed())
                {
                    if (isFirstRow)
                    {
                        // Add columns to the DataTable
                        foreach (var cell in row.CellsUsed())
                        {
                            dt.Columns.Add(cell.GetString());
                        }
                        isFirstRow = false;
                    }
                    else
                    {
                        // Add rows to the DataTable
                        DataRow dataRow = dt.NewRow();
                        int columnIndex = 0;

                        foreach (var cell in row.CellsUsed())
                        {
                            dataRow[columnIndex++] = cell.Value.ToString() ?? string.Empty;
                        }

                        dt.Rows.Add(dataRow);
                    }
                }
            }

            return dt;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {

            // Show loading window
            var loadingWindow = new LoadingWindow();
            loadingWindow.Owner = (MainWindow)System.Windows.Application.Current.MainWindow; // Set the owner to block interaction with the main window
            loadingWindow.Show();

            try
            {
                // Create an OpenFileDialog
                OpenFileDialog openFileDialog = new OpenFileDialog();

                // Set filter options for file types
                openFileDialog.Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*";
                openFileDialog.Title = "Select a File";

                // Show the dialog
                if (openFileDialog.ShowDialog() == true)
                {
                    // Get the selected file path
                    string filePath = openFileDialog.FileName;

                    // Validate the file extension
                    string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();
                    if (fileExtension == ".xlsx" || fileExtension == ".xls")
                    {

                        DataTable dtData = LoadExcelToDataTable(filePath);

                        foreach (DataRow dr in dtData.Rows)
                        {
                            Items objItem = new Items();

                            objItem.ItemID = 0;

                            objItem.Category = Convert.ToString(dr["Category"]);
                            objItem.UPC = Convert.ToString(dr["UPC"]);
                            objItem.Name = Convert.ToString(dr["Name"]);
                            objItem.Additional_Description = Convert.ToString(dr["Additional_Description"]);

                            objItem.ItemCost = Convert.ToDecimal(dr["ItemCost"]);
                            objItem.ChargedCost = Convert.ToDecimal(dr["ChargedCost"]);
                            objItem.InStock = Convert.ToInt32(dr["InStock"]);

                            objItem.Sales_Tax = Convert.ToString(dr["Sales_Tax"]) == "True";
                            objItem.Sales_Tax_2 = Convert.ToString(dr["Sales_Tax_2"]) == "True";
                            objItem.Sales_Tax_3 = Convert.ToString(dr["Sales_Tax_3"]) == "True";
                            objItem.Sales_Tax_4 = Convert.ToString(dr["Sales_Tax_4"]) == "True";
                            objItem.Sales_Tax_5 = Convert.ToString(dr["Sales_Tax_5"]) == "True";
                            objItem.Sales_Tax_6 = Convert.ToString(dr["Sales_Tax_6"]) == "True";
                            objItem.Bar_Tax = Convert.ToString(dr["Bar_Tax"]) == "True";

                            objService.SaveItem(objItem);
                        }
                        MessageBox.Show("Data Imported Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        btnSearch_Click(null, null);
                    }
                    else
                    {
                        // Invalid file extension
                        MessageBox.Show("Invalid file type. Please select a valid Excel file (*.xlsx or *.xls).",
                                        "Invalid File",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                // Invalid file extension
                MessageBox.Show("Error Occurred while Uploding the file - " + ex.Message,
                                "Error Occurred",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            finally
            {
                // Close the loading window
                loadingWindow.Close();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtDescription.Text = string.Empty;
            txtUPC.Text = string.Empty;
        }

        private void txtUPC_TextChanged(object sender, TextChangedEventArgs e)
        {


            string text = txtUPC.Text;

            if (text.Length == 10 || text.Length == 9 || text.Length == 6)
            {
                HandleScannedInput(text);
                return;
            }
        }


        private void HandleScannedInput(string barcode)
        {
            btnSearch_Click(null,null);
        }
    }
}
