using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;  // ADO.NET

namespace BusinessClasses
{
    public class Order
    {
        // public properties
        public int OrderID { get; set; }            // the order Id number
        public string CustomerID { get; set; }      // the customer Id number
        public DateTime? OrderDate { get; set; }    // the date the order placed
        public DateTime? RequiredDate { get; set; } // the date the product is required
        public DateTime? ShippedDate { get; set; }  // the date the item is shipped

        public Order CopyOrder() // copy created for update to the database
        {
            Order copy = new Order();
            copy.OrderID = OrderID;
            copy.CustomerID = CustomerID;
            copy.OrderDate = OrderDate;
            copy.RequiredDate = RequiredDate;
            copy.ShippedDate = ShippedDate;
            return copy;
        }

    }
}
