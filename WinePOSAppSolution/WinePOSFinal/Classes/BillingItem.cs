using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinePOSFinal.Classes
{
    public class BillingItem
    {
        public string UPC { get; set; }
        public string ItemID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Discount { get; set; }
        public string TotalPrice { get; set; }
        public string Tax { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
    }
}
