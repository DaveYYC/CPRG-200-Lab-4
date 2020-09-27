using BusinessClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;  // ADO.NET
using System.Text;
using System.Threading.Tasks;

namespace DataClasses
{
    public static class OrderDetailDB
    {
        // connection to the database
        public static SqlConnection GetConnection()
        {
            string connectionString = (@"Data Source=DAVES-LAPTOP;Initial Catalog=Northwind;Integrated Security=True");
            return new SqlConnection(connectionString);
        }
        // get the OrderDetails data
        public static List<OrderDetail> GetOrderDetails()
        {
            // list of order details for viewing i the list box
            List<OrderDetail> orderDetails = new List<OrderDetail>(); // empty list
            OrderDetail orderDetail;
            using (SqlConnection connection = GetConnection())
            {
                string query = "SELECT OrderID, ProductID, UnitPrice, Quantity, Discount " +
                                     "FROM [Order Details] ORDER BY OrderID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        orderDetail = new OrderDetail();
                        orderDetail.OrderID = (int)reader["OrderID"];         // datatype int 
                        orderDetail.ProductID = (int)reader["ProductID"];     // datatype int
                        orderDetail.UnitPrice = (decimal)reader["UnitPrice"]; // datatype money
                        orderDetail.Quantity = (short)reader["Quantity"];     // datatype smllint
                        orderDetail.Discount = (Single)(reader["Discount"]);  // datatype real
                        orderDetail.CalculateOrderTotal();                // total amount of the order
                        orderDetails.Add(orderDetail);
                    }
                } // cmd recycled
            } // connection recycled
            return orderDetails;
         }
    }
}


