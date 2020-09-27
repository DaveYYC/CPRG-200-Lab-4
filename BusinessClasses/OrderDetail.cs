using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;  // ADO.NET

namespace BusinessClasses
{
    public class OrderDetail
    {
        // public properties
        public int OrderID { get; set; }        // order Id number
        public int ProductID { get; set; }      // product Id number
        public decimal UnitPrice { get; set; }  // prouct unit price
        public short Quantity { get; set; }     // product quantity
        public Single Discount { get; set; }    // product discount amount
        public decimal OrderTotal { get; set; } // total of the product line item amount

        // calulation to determine the product line item total for display in the list box
        public decimal CalculateOrderTotal()
        {
            return OrderTotal = UnitPrice * (decimal)(1 - Discount) * Quantity;
        }
    }
}
