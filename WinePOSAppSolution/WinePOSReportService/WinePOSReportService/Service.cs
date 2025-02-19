using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinePOSReportService
{
    internal class Service
    {
        DAL objDAL = new DAL();

        public Service()
        { }

        public DataSet FetchAndPopulateInvoice(bool IsAdmin, DateTime? fromDate, DateTime? toDate, string InvoiceNumber)
        {
            return objDAL.FetchAndPopulateInvoice(IsAdmin, fromDate, toDate, InvoiceNumber);
        }

        public string GetValueFromConfig(string strKey)
        {
            return objDAL.GetValueFromConfig(strKey);
        }

        public bool UpdateConfigValue(string strKey, string strValue)
        {
            return objDAL.UpdateConfigValue(strKey, strValue);
        }
    }
}
