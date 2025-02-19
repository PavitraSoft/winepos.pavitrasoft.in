using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace WinePOSReportService
{
    internal class DAL
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        public DAL()
        {
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
        public bool UpdateConfigValue(string strKey, string strValue)
        {
            bool bIsSuccess = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Config SET [Value] = '" + strValue + "' WHERE [Key] = '" + strKey + "'";

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
                //MessageBox.Show($"Error4: {ex.Message}");
            }

            return bIsSuccess;
        }
    }



}
