using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinePOSFinal.Classes
{
    internal class Items
    {
        public int itemid;
        public string name;
        public int category;
        public string upc;
        public string additional_description;
        public decimal itemcost;
        public decimal chargedcost;
        public bool sales_tax;
        public bool sales_tax_2;
        public bool sales_tax_3;
        public bool sales_tax_4;
        public bool sales_tax_5;
        public bool sales_tax_6;
        public bool bar_tax;
        public int instock;

        public int ItemID
        { set { itemid = value; } get { return itemid; } }

        public string Name
        { get { return name; } set { name = value; } }

        public int Category
        { get { return category; } set { category = value; } }

        public string UPC
        { get { return upc; } set { upc = value; } }

        public string Additional_Description
        { get { return additional_description; } set { additional_description = value; } }

        public decimal ItemCost
        { get { return itemcost; } set { itemcost = value; } }

        public decimal ChargedCost
        { get { return chargedcost; } set { chargedcost = value; } }

        public bool Sales_Tax
        { get { return sales_tax; } set { sales_tax = value; } }

        public bool Sales_Tax_2
        { get { return sales_tax_2; } set { sales_tax_2 = value; } }

        public bool Sales_Tax_3
        { get { return sales_tax_3; } set { sales_tax_3 = value; } }

        public bool Sales_Tax_4
        { get { return sales_tax_4; } set { sales_tax_4 = value; } }

        public bool Sales_Tax_5
        { get { return sales_tax_5; } set { sales_tax_5 = value; } }

        public bool Sales_Tax_6
        { get { return sales_tax_6; } set { sales_tax_6 = value; } }

        public bool Bar_Tax
        { get { return bar_tax; } set { bar_tax = value; } }

        public int InStock
        { get { return instock; } set { instock = value; } }
    }
}
