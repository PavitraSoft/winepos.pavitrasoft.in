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

        public DataTable GetInventoryData(string strUPC, string strDescription)
        {
            return objDAL.GetInventoryData(strUPC, strDescription);
        }

        public Items FetchItemDataByID(int intItemID)
        {
            return objDAL.FetchItemDataByID(intItemID);
        }

        public bool DeleteItemDataByID(int intItemID)
        {
            return objDAL.DeleteItemDataByID(intItemID);
        }

        public string ValidateLogin(string strUserName, string strPassWord)
        {
            return objDAL.ValidateLogin(strUserName, strPassWord);
        }

        public bool SaveInlineItemData(int ItemID, string columnName, string value)
        {
            return objDAL.SaveInlineItemData(ItemID, columnName, value);
        }



        public bool SaveInvoice(BillingItem objBillingItem, bool IsVoidInvoice, string PaymentType, ref int invoiceNumber)
        {
            return objDAL.SaveInvoice(objBillingItem, IsVoidInvoice, PaymentType, ref invoiceNumber);
        }

        public DataTable GetTaxData()
        {
            return objDAL.GetTaxData();
        }

        public DataTable GetBulkPricingData()
        {
            return objDAL.GetBulkPricingData();
        }

        public DataTable FetchAndPopulateInvoice(bool IsAdmin, DateTime? fromDate, DateTime? toDate, string InvoiceNumber)
        {
            return objDAL.FetchAndPopulateInvoice(IsAdmin, fromDate, toDate, InvoiceNumber);
        }

        public bool UpdateSentEmailDetail(int ID)
        {
            return objDAL.UpdateSentEmailDetail(ID);
        }

        public DataTable GetLowQuentityEmailDetails()
        {
            return objDAL.GetLowQuentityEmailDetails();
        }
        

        public bool VoidInvoice(int invoiceCode)
        {
            return objDAL.VoidInvoice(invoiceCode);
        }


        public bool SaveTaxData(int TaxID, Decimal Percentage)
        {
            return objDAL.SaveTaxData(TaxID, Percentage);
        }

        public bool SaveBulkPricing(int itemID,int quantity, Decimal price)
        {
            return objDAL.SaveBulkPricing(itemID, quantity, price);
        }

        public DataTable VoidInvoicesByCodes(string strInvoiceCode, string strUserName)
        {
            return objDAL.VoidInvoicesByCodes(strInvoiceCode, strUserName);
        }
    }
}
