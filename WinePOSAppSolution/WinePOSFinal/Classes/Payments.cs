using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinePOSFinal.Classes
{
    public class Payments
    {
        public decimal Amount { get; set; }
        public string Type { get; set; }


        // Parameterless constructor (needed for serialization)
        public Payments() { }

        public Payments(string type, decimal amount)
        {
            Type = type;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{Type}: ${Amount.ToString("G29")}";
        }
    }
}
