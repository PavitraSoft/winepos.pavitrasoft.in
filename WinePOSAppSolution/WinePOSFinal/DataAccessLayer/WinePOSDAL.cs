using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using WinePOSFinal.Classes;
using System.Windows;
using System.ComponentModel;

namespace WinePOSFinal.DataAccessLayer
{
    internal class WinePOSDAL
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        public WinePOSDAL()
        {
        }

        public DataTable GetIMDropdownData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Sample query to retrieve data
                string query = "SELECT CatagoryID AS Code, [Description] FROM CategoryMaster ORDER BY Description ASC"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }

            return dt;

        }

        public bool SaveItem(Items objItem)
        {
            bool bIsSuccess = false;

            // Create SQL connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create the command to execute the stored procedure
                    SqlCommand cmd = new SqlCommand("usp_SaveItems", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("ItemID", objItem.ItemID);
                    cmd.Parameters.AddWithValue("Name", objItem.Name);
                    cmd.Parameters.AddWithValue("Category", objItem.Category);
                    cmd.Parameters.AddWithValue("UPC", objItem.UPC);
                    cmd.Parameters.AddWithValue("Additional_Description", objItem.Additional_Description);
                    cmd.Parameters.AddWithValue("ItemCost", objItem.ItemCost);
                    cmd.Parameters.AddWithValue("ChargedCost", objItem.ChargedCost);
                    cmd.Parameters.AddWithValue("Sales_Tax", objItem.Sales_Tax);
                    cmd.Parameters.AddWithValue("Sales_Tax_2", objItem.Sales_Tax_2);
                    cmd.Parameters.AddWithValue("Sales_Tax_3", objItem.Sales_Tax_3);
                    cmd.Parameters.AddWithValue("Sales_Tax_4", objItem.Sales_Tax_4);
                    cmd.Parameters.AddWithValue("Sales_Tax_5", objItem.Sales_Tax_5);
                    cmd.Parameters.AddWithValue("Sales_Tax_6", objItem.Sales_Tax_6);
                    cmd.Parameters.AddWithValue("Bar_Tax", objItem.Bar_Tax);
                    cmd.Parameters.AddWithValue("InStock", objItem.InStock);

                    // Execute the stored procedure (this will not return anything)
                    int rowsAffected = cmd.ExecuteNonQuery();

                    bIsSuccess = true;
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the execution
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

            return bIsSuccess;
        }

        public DataTable GetInventoryData(string strDescription)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Create the command to execute the stored procedure
                    SqlCommand cmd = new SqlCommand("usp_GetInventoryData", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("Description", strDescription);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error: {ex.Message}");
            }

            return dt;
        }
        public Items FetchItemDataByID(int intItemID)
        {
            Items items = new Items();
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Create the command to execute the stored procedure
                    SqlCommand cmd = new SqlCommand("usp_ItemDataByID", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("ItemID", intItemID);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        items.ItemID = Convert.ToInt32(dt.Rows[0]["ItemID"]);
                        items.Name = Convert.ToString(dt.Rows[0]["Name"]);
                        items.Category = Convert.ToInt32(dt.Rows[0]["Category"]);
                        items.UPC = Convert.ToString(dt.Rows[0]["UPC"]);
                        items.Additional_Description = Convert.ToString(dt.Rows[0]["Additional_Description"]);
                        items.ItemCost = Convert.ToDecimal(dt.Rows[0]["ItemCost"]);
                        items.ChargedCost = Convert.ToDecimal(dt.Rows[0]["ChargedCost"]);
                        items.Sales_Tax = Convert.ToBoolean(dt.Rows[0]["Sales_Tax"]);
                        items.Sales_Tax_2 = Convert.ToBoolean(dt.Rows[0]["Sales_Tax_2"]);
                        items.Sales_Tax_3 = Convert.ToBoolean(dt.Rows[0]["Sales_Tax_3"]);
                        items.Sales_Tax_4 = Convert.ToBoolean(dt.Rows[0]["Sales_Tax_4"]);
                        items.Sales_Tax_5 = Convert.ToBoolean(dt.Rows[0]["Sales_Tax_5"]);
                        items.Sales_Tax_6 = Convert.ToBoolean(dt.Rows[0]["Sales_Tax_6"]);
                        items.Bar_Tax = Convert.ToBoolean(dt.Rows[0]["Bar_Tax"]);
                        items.InStock = Convert.ToInt32(dt.Rows[0]["InStock"]);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error: {ex.Message}");
            }

            return items;
        }

        public bool DeleteItemDataByID(int intItemID)
        {
            bool bIsSuccess = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Sample query to retrieve data
                    string query = "DELETE FROM Items WHERE ItemID = " + Convert.ToString(intItemID); // Replace with your actual query

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
                bIsSuccess = true;

            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error: {ex.Message}");
            }

            return bIsSuccess;


        }
    }
}
