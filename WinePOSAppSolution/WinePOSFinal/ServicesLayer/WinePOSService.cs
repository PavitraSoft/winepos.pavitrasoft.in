using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WinePOSFinal.DataAccessLayer;
using WinePOSFinal.Classes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WinePOSFinal.ServicesLayer
{
    internal class WinePOSService
    {
        WinePOSDAL objDAL = new WinePOSDAL();

        public WinePOSService()
        { }

        public DataTable GetIMDropdownData()
        {
            return objDAL.GetIMDropdownData();
        }

        public bool SaveItem(Items objItem)
        {
            return objDAL.SaveItem(objItem);
        }

        public DataTable GetInventoryData(string strDescription)
        {
            return objDAL.GetInventoryData(strDescription);
        }

        public Items FetchItemDataByID(int intItemID)
        {
            return objDAL.FetchItemDataByID(intItemID);
        }

        public bool DeleteItemDataByID(int intItemID)
        {
            return objDAL.DeleteItemDataByID(intItemID);
        }

        public int ValidateLogin(string strUserName, string strPassWord)
        {
            return objDAL.ValidateLogin(strUserName, strPassWord);
        }

        public bool SaveInlineItemData(int ItemID, string columnName, string value)
        {
            return objDAL.SaveInlineItemData(ItemID, columnName, value);
        }



        public bool SaveInvoice(BillingItem objBillingItem, bool IsVoidInvoice)
        {
            return objDAL.SaveInvoice(objBillingItem, IsVoidInvoice);
        }

        public DataTable GetTaxData()
        {
            return objDAL.GetTaxData();
        }

        public DataTable FetchAndPopulateInvoice(bool IsAdmin)
        {
            return objDAL.FetchAndPopulateInvoice(IsAdmin);
        }
    }
}
