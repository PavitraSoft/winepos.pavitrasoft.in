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
using System.Xml.Serialization;
using System.IO;

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
                string query = "SELECT Code AS Code, [Description] FROM CategoryMaster ORDER BY Description ASC"; // Replace with your actual query

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
            SqlParameter param;
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
                    //cmd.Parameters.AddWithValue("ItemCost", objItem.ItemCost);


                    param = new SqlParameter("ItemCost", SqlDbType.Decimal);
                    param.Precision = 13;  // Matches SQL precision
                    param.Scale = 5;       // Matches SQL scale
                    param.Value = objItem.ItemCost;

                    cmd.Parameters.Add(param);

                    param = new SqlParameter("ChargedCost", SqlDbType.Decimal);
                    param.Precision = 13;  // Matches SQL precision
                    param.Scale = 5;       // Matches SQL scale
                    param.Value = objItem.ChargedCost;

                    cmd.Parameters.Add(param);

                    //cmd.Parameters.AddWithValue("ChargedCost", objItem.ChargedCost);
                    cmd.Parameters.AddWithValue("Sales_Tax", objItem.Sales_Tax);
                    cmd.Parameters.AddWithValue("InStock", objItem.InStock);
                    cmd.Parameters.AddWithValue("VendorName", objItem.VendorName);
                    //cmd.Parameters.AddWithValue("CaseCost", objItem.CaseCost);


                    param = new SqlParameter("CaseCost", SqlDbType.Decimal);
                    param.Precision = 13;  // Matches SQL precision
                    param.Scale = 5;       // Matches SQL scale
                    param.Value = objItem.CaseCost;

                    cmd.Parameters.Add(param);
                    cmd.Parameters.AddWithValue("InCase", objItem.InCase);
                    //cmd.Parameters.AddWithValue("SalesTaxAmt", objItem.SalesTaxAmt);


                    param = new SqlParameter("SalesTaxAmt", SqlDbType.Decimal);
                    param.Precision = 13;  // Matches SQL precision
                    param.Scale = 5;       // Matches SQL scale
                    param.Value = objItem.SalesTaxAmt;

                    cmd.Parameters.AddWithValue("QuickADD", objItem.QuickADD);

                    if (objItem.BulkPricingItems != null && objItem.BulkPricingItems.Count > 0)
                    {
                        string xmlData = SerializeToXml(objItem.BulkPricingItems);
                        cmd.Parameters.AddWithValue("BulkPricing", xmlData);
                    }

                    cmd.Parameters.AddWithValue("EnableStockAlert", objItem.EnableStockAlert);
                    cmd.Parameters.AddWithValue("StockAlertLimit", objItem.StockAlertLimit);
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

        // Helper method to serialize the list to XML
        private string SerializeToXml(List<BulkPricingItem> list)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<BulkPricingItem>));
                using (var stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, list);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during serialization: {ex.Message}", "Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // Helper method to serialize the list to XML
        private string SerializeToXml(List<Payments> list)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<Payments>));
                using (var stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, list);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during serialization: {ex.Message}", "Serialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
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
                MessageBox.Show($"Error2: {ex.Message}");
            }

            return dt;
        }
        public Items FetchItemDataByID(int intItemID)
        {
            Items items = new Items();
            BulkPricingItem bulkPricing = new BulkPricingItem();
            DataSet ds = new DataSet();

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
                    dataAdapter.Fill(ds); // Fill the DataTable with data from the database

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        items.ItemID = dt.Rows[0]["ItemID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["ItemID"]) : 0;
                        items.Name = dt.Rows[0]["Name"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["Name"]) : string.Empty;
                        items.Category = dt.Rows[0]["Category"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["Category"]) : string.Empty;
                        items.UPC = dt.Rows[0]["UPC"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["UPC"]) : string.Empty;
                        items.Additional_Description = dt.Rows[0]["Additional_Description"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["Additional_Description"]) : string.Empty;
                        items.ItemCost = dt.Rows[0]["ItemCost"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["ItemCost"]) : 0;
                        items.ChargedCost = dt.Rows[0]["ChargedCost"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["ChargedCost"]) : 0;
                        items.Sales_Tax = dt.Rows[0]["Sales_Tax"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["Sales_Tax"]) : false;
                        items.InStock = dt.Rows[0]["InStock"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["InStock"]) : 0;
                        items.VendorName = dt.Rows[0]["VendorName"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["VendorName"]) : string.Empty;
                        items.SalesTaxAmt = dt.Rows[0]["SalesTax"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["SalesTax"]) : 0;
                        items.CaseCost = dt.Rows[0]["CaseCost"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["CaseCost"]) : 0;
                        items.InCase = dt.Rows[0]["NumberInCase"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["NumberInCase"]) : 0;
                        items.QuickADD = dt.Rows[0]["QuickADD"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["QuickADD"]) : false;
                        items.DroppedItem = dt.Rows[0]["DroppedItem"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["DroppedItem"]) : string.Empty;
                        items.EnableStockAlert = dt.Rows[0]["EnableStockAlert"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[0]["EnableStockAlert"]) : false;
                        items.StockAlertLimit = dt.Rows[0]["StockAlertLimit"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["StockAlertLimit"]) : 0;

                        items.BulkPricingItems = new List<BulkPricingItem>();

                        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                        {
                            DataTable dtBulk = ds.Tables[1];

                            foreach (DataRow dr in ds.Tables[1].Rows)
                            {
                                BulkPricingItem bItem = new BulkPricingItem();
                                bItem.BuilkPricingID = Convert.ToInt32(dr["BuilkPricingID"]);
                                bItem.ItemID = Convert.ToInt32(dr["ItemID"]);
                                bItem.Quantity = Convert.ToInt32(dr["Quantity"]);
                                bItem.Price = Convert.ToDecimal(dr["Pricing"]);

                                items.BulkPricingItems.Add(bItem);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error3: {ex.Message}");
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
                MessageBox.Show($"Error4: {ex.Message}");
            }

            return bIsSuccess;
        }


        public string ValidateLogin(string strUserName, string strPassWord)
        {
            string role = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Sample query to retrieve data
                    string query = "SELECT TOP 1 UserRole FROM Users WHERE UserName = '" + strUserName.Replace(",", "''") + "' AND Password = '" + strPassWord.Replace(",", "''") + "'"; // Replace with your actual query

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dt); // Fill the DataTable with data from the database

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            role = Convert.ToString(dt.Rows[0]["UserRole"]);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the execution
                MessageBox.Show($"Error5: {ex.ToString()}");
            }

            return role;
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
                MessageBox.Show($"Error6: {ex.Message}");
            }
            return bIsSuccess;
        }

        public bool SaveInvoice(BillingItem objBillingItem, bool IsVoidInvoice, string PaymentType, ref int invoiceNumber, List<Payments> objPayments)
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
                    cmd.Parameters.AddWithValue("Discount", objBillingItem.Discount);

                    if (objPayments != null && objPayments.Count > 0)
                    {
                        string xmlData = SerializeToXml(objPayments);
                        cmd.Parameters.AddWithValue("Payments", xmlData);
                    }

                    // Execute the stored procedure (this will not return anything)
                    int rowsAffected = cmd.ExecuteNonQuery();
                    invoiceNumber = nextInvoiceCode;
                    bIsSuccess = true;
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the execution
                    MessageBox.Show($"Error8: {ex.Message}");
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
                string query = "SELECT * FROM SalesTax WITH (NOLOCK) WHERE IsActive = 1"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }

            return dt;

        }
        public DataTable FetchBulkPricingData(int ItemID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Sample query to retrieve data
                string query = "SELECT * FROM BulkPricing WITH (NOLOCK) WHERE ItemID = " + Convert.ToString(ItemID); // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }

            return dt;

        }


        public DataSet FetchAndPopulateInvoice(bool IsAdmin, DateTime? fromDate, DateTime? toDate, string InvoiceNumber)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string where = " AND 1 = 1 ";

                //if (!IsAdmin)
                //{
                //    where = " AND I.IsVoided <> 1 ";
                //}

                if (fromDate.HasValue)
                {
                    where += " AND I.CreatedDateTime >= CONVERT(DATETIME, '" + fromDate.Value.Date.ToString("yyyy-MM-dd") + "') ";
                }

                if (toDate.HasValue)
                {
                    where += " AND I.CreatedDateTime <= DATEADD(DAY,1,CONVERT(DATETIME, '" + toDate.Value.Date.ToString("yyyy-MM-dd") + "')) ";
                }

                if (!string.IsNullOrWhiteSpace(InvoiceNumber))
                {
                    where += " AND I.InvoiceCode IN ('" + InvoiceNumber + "') ";
                }


                // Sample query to retrieve data
                //string query = "SELECT InvoiceCode, UPC, Name ,Price,Quantity,Tax,TotalPrice,UserName,CreatedDateTime,PaymentType, CASE WHEN IsVoided = 1 THEN 'Yes' ELSE 'No' END AS IsVoided, Discount FROM Invoice WITH (NOLOCK) WHERE 1 = 1 " + where + " ORDER BY CreatedDateTime DESC"; // Replace with your actual query

                // Sample query to retrieve data
                string query = "usp_GetSalesHistoryData_2"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    command.Parameters.AddWithValue("Where", where);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(ds); // Fill the DataTable with data from the database
                }
            }

            return ds;

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
                MessageBox.Show($"Error9: {ex.Message}");
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


        public bool VoidInvoice(int invoiceCode)
        {
            bool bIsSuccess = false;
            StringBuilder sb = new StringBuilder();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    sb.Append("UPDATE Invoice SET ");
                    sb.Append(" IsVoided = 1 ");
                    sb.Append(" WHERE InvoiceCode = " + Convert.ToString(invoiceCode));

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
                MessageBox.Show($"Error10: {ex.Message}");
            }

            return bIsSuccess;
        }

        public bool SaveTaxData(int TaxID, Decimal Percentage)
        {
            bool bIsSuccess = false;
            StringBuilder sb = new StringBuilder();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    sb.Append("UPDATE SalesTax SET ");
                    sb.Append(" Percentage =  " + Convert.ToString(Percentage));
                    sb.Append(" WHERE ID = " + Convert.ToString(TaxID));

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
                MessageBox.Show($"Error11: {ex.Message}");
            }

            return bIsSuccess;
        }

        public bool SaveBulkPricing(int itemID, int quantity, Decimal price)
        {
            bool bIsSuccess = false;
            StringBuilder sb = new StringBuilder();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    sb.Append(" INSERT INTO BulkPricing (ItemID, Quantity, Pricing) VALUES (" + Convert.ToString(itemID) + "," + Convert.ToString(quantity) + "," + Convert.ToString(price) + ") ");

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
                MessageBox.Show($"Error12: {ex.Message}");
            }

            return bIsSuccess;
        }


        public DataTable GetBulkPricingData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Sample query to retrieve data
                string query = "SELECT I.UPC, B.BuilkPricingID,B.ItemID,B.Quantity,B.Pricing FROM BulkPricing B WITH (NOLOCK) INNER JOIN Items I WITH (NOLOCK) ON B.ItemID = I.ItemID"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
            }

            return dt;

        }

        public DataTable VoidInvoicesByCodes(string strInvoiceCodes, string strUserName)
        {
            bool bIsSuccess = false;
            DataTable dt = new DataTable();

            // Create SQL connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create the command to execute the stored procedure
                    SqlCommand cmd = new SqlCommand("usp_VoidInvoice", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("InvoiceCodes", strInvoiceCodes);
                    cmd.Parameters.AddWithValue("UserName", strUserName);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the execution
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

            return dt;
        }

        public bool DeleteInvoiceByNumber(int intInvoiceNumber)
        {
            bool bIsSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Sample query to retrieve data
                    string query = "DELETE FROM INVOICE WHERE InvoiceCode = " + Convert.ToString(intInvoiceNumber); // Replace with your actual query

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
                MessageBox.Show($"Error4: {ex.Message}");
            }

            return bIsSuccess;
        }

        public string GetValueFromConfig(string strKey)
        {
            string strValue = string.Empty;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Sample query to retrieve data
                string query = "SELECT [Value] FROM Config WITH (NOLOCK) WHERE [Key] = '" + strKey + "'"; // Replace with your actual query

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Create a DataAdapter to fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dt); // Fill the DataTable with data from the database
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        strValue = Convert.ToString(dt.Rows[0][0]);
                    }
                }
            }
            return strValue;
        }
    }
}
