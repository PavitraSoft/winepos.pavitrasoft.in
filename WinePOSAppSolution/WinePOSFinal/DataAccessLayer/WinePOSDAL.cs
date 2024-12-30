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
                    cmd.Parameters.AddWithValue("VendorPartNo", objItem.VendorPartNo);
                    cmd.Parameters.AddWithValue("VendorName", objItem.VendorName);
                    cmd.Parameters.AddWithValue("CaseCost", objItem.CaseCost);
                    cmd.Parameters.AddWithValue("InCase", objItem.InCase);
                    cmd.Parameters.AddWithValue("SalesTaxAmt", objItem.SalesTaxAmt);
                    cmd.Parameters.AddWithValue("QuickADD", objItem.QuickADD);

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

        public DataTable GetInventoryData(string strUPC, string strDescription)
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
                    cmd.Parameters.AddWithValue("UPC", strUPC);
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
                        items.ItemID = dt.Rows[0]["ItemID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["ItemID"]) : 0;
                        items.Name = dt.Rows[0]["Name"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["Name"]) : string.Empty;
                        items.Category = dt.Rows[0]["Category"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["Category"]) : string.Empty;
                        items.UPC = dt.Rows[0]["UPC"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["UPC"]) : string.Empty;
                        items.Additional_Description = dt.Rows[0]["Additional_Description"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["Additional_Description"]) : string.Empty;
                        items.ItemCost = dt.Rows[0]["ItemCost"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["ItemCost"]) : 0;
                        items.ChargedCost = dt.Rows[0]["ChargedCost"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["ChargedCost"]) : 0;
                        items.Sales_Tax = dt.Rows[0]["Sales_Tax"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Sales_Tax"]) : false;
                        items.Sales_Tax_2 = dt.Rows[0]["Sales_Tax_2"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Sales_Tax_2"]) : false;
                        items.Sales_Tax_3 = dt.Rows[0]["Sales_Tax_3"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Sales_Tax_3"]) : false;
                        items.Sales_Tax_4 = dt.Rows[0]["Sales_Tax_4"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Sales_Tax_4"]) : false;
                        items.Sales_Tax_5 = dt.Rows[0]["Sales_Tax_5"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Sales_Tax_5"]) : false;
                        items.Sales_Tax_6 = dt.Rows[0]["Sales_Tax_6"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Sales_Tax_6"]) : false;
                        items.Bar_Tax = dt.Rows[0]["Bar_Tax"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Bar_Tax"]) : false;
                        items.InStock = dt.Rows[0]["InStock"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["InStock"]) : 0;
                        items.VendorName = dt.Rows[0]["VendorName"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["VendorName"]) : string.Empty;
                        items.VendorPartNo = dt.Rows[0]["VendorPartNum"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["VendorPartNum"]) : string.Empty;
                        items.SalesTaxAmt = dt.Rows[0]["SalesTax"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["SalesTax"]) : 0;
                        items.CaseCost = dt.Rows[0]["CaseCost"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["CaseCost"]) : 0;
                        items.InCase = dt.Rows[0]["NumberInCase"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["NumberInCase"]) : 0;
                        items.QuickADD = dt.Rows[0]["QuickADD"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["QuickADD"]) : false;
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
                        if (rowsAffected > 0)
                        {
                            bIsSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error: {ex.Message}");
            }

            return bIsSuccess;
        }


        public int ValidateLogin(string strUserName, string strPassWord)
        {
            int iIsAdmin = -1;
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Sample query to retrieve data
                    string query = "SELECT TOP 1 IsAdmin FROM Users WHERE UserName = '" + strUserName.Replace(",", "''") + "' AND Password = '" + strPassWord.Replace(",", "''") + "'"; // Replace with your actual query

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dt); // Fill the DataTable with data from the database

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            iIsAdmin = Convert.ToString(dt.Rows[0]["IsAdmin"]) == "True" ? 1 : 0;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error: {ex.Message}");
            }

            return iIsAdmin;
        }

        public bool SaveInlineItemData(int ItemID, string columnName, string value)
        {
            bool bIsSuccess = false;
            StringBuilder sb = new StringBuilder();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    sb.Append("UPDATE Items SET ");
                    sb.Append(" " + columnName + " = '" + (!string.IsNullOrWhiteSpace(value) ? value : string.Empty) + "'");
                    sb.Append(" WHERE ItemID = " + Convert.ToString(ItemID));

                    // Sample query to retrieve data
                    string query = sb.ToString(); // Replace with your actual query

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            bIsSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error: {ex.Message}");
            }

            return bIsSuccess;
        }

        public bool SaveInvoice(BillingItem objBillingItem, bool IsVoidInvoice, string PaymentType, ref int invoiceNumber)
        {
            bool bIsSuccess = false;

            // Create SQL connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    int nextInvoiceCode = 0;
                    // Open the connection
                    connection.Open();
                    if (invoiceNumber == 0)
                    {
                        string nextNumberQuery = "SELECT ISNULL(MAX(InvoiceCode), 0) + 1 FROM Invoice"; // Handle null case

                        using (SqlCommand command = new SqlCommand(nextNumberQuery, connection))
                        {
                            // ExecuteScalar() fetches the first column of the first row in the result
                            object result = command.ExecuteScalar();
                            nextInvoiceCode = result != DBNull.Value ? Convert.ToInt32(result) : 1; // Default to 1 if null
                            Console.WriteLine("Next Invoice Code: " + nextInvoiceCode);
                        }
                    }
                    else
                    {
                        nextInvoiceCode = invoiceNumber;
                    }
                    // Create the command to execute the stored procedure
                    SqlCommand cmd = new SqlCommand("usp_SaveInvoice", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("UPC", objBillingItem.UPC);
                    cmd.Parameters.AddWithValue("Name", objBillingItem.Name);
                    cmd.Parameters.AddWithValue("Price", objBillingItem.Price);
                    cmd.Parameters.AddWithValue("Quantity", objBillingItem.Quantity);
                    cmd.Parameters.AddWithValue("TotalPrice", objBillingItem.TotalPrice);
                    cmd.Parameters.AddWithValue("Tax", objBillingItem.Tax);
                    cmd.Parameters.AddWithValue("UserName", objBillingItem.UserName);
                    cmd.Parameters.AddWithValue("IsVoided", IsVoidInvoice);
                    cmd.Parameters.AddWithValue("InvoiceCode", nextInvoiceCode);
                    cmd.Parameters.AddWithValue("PaymentType", PaymentType);

                    // Execute the stored procedure (this will not return anything)
                    int rowsAffected = cmd.ExecuteNonQuery();
                    invoiceNumber = nextInvoiceCode;
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

        public DataTable GetTaxData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Sample query to retrieve data
                string query = "SELECT * FROM SalesTax WITH (NOLOCK)"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }

            return dt;

        }


        public DataTable FetchAndPopulateInvoice(bool IsAdmin, DateTime? fromDate, DateTime? toDate, string InvoiceNumber)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string where = " AND IsVoided = 0 ";

                if (IsAdmin)
                {
                    where = " AND IsVoided = 1 ";
                }

                if (fromDate.HasValue)
                {
                    where += " AND CreatedDateTime >= CONVERT(DATETIME, '" + fromDate.Value.Date.ToString("yyyy-MM-dd") + "') ";
                }

                if (fromDate.HasValue)
                {
                    where += " AND CreatedDateTime <= CONVERT(DATETIME, '" + toDate.Value.Date.ToString("yyyy-MM-dd") + "') ";
                }

                if (!string.IsNullOrWhiteSpace(InvoiceNumber))
                {
                    where += " AND InvoiceCode = '" + InvoiceNumber + "' ";
                }


                // Sample query to retrieve data
                string query = "SELECT InvoiceCode, UPC, Name ,Price,Quantity,Tax,TotalPrice,UserName,CreatedDateTime,PaymentType FROM Invoice WITH (NOLOCK) WHERE 1 = 1 " + where + " ORDER BY CreatedDateTime DESC"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }

            return dt;

        }


        public bool UpdateSentEmailDetail(int ID)
        {
            bool bIsSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Sample query to retrieve data
                    string query = "UPDATE Email SET IsSent = 1, SentDateTime = GETDATE() WHERE ID = " + Convert.ToString(ID); // Replace with your actual query

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            bIsSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error: {ex.Message}");
            }

            return bIsSuccess;
        }


        public DataTable GetLowQuentityEmailDetails()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Sample query to retrieve data
                string query = "SELECT (SELECT TOP 1 [Value] FROM Config WHERE [Key] = 'LowStockAlertEmailSMTPUser') AS smtpUser, (SELECT TOP 1 [Value] FROM Config WHERE [Key] = 'LowStockAlertSMTPPassword') AS smtpPassword ,ToMail ,Subject ,Body, ID FROM Email WHERE IsSent = 0"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }
            return dt;
        }
    }
}
