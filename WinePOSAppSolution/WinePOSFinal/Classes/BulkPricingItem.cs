﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinePOSFinal.Classes
{
    public class BulkPricingItem
    {
        public int BuilkPricingID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}