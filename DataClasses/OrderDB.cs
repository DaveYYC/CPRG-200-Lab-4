using BusinessClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;  // ADO.NET
using System.Text;
using System.Threading.Tasks;

namespace DataClasses
{
    public static class OrderDB
    {
        // connection to the database
        public static SqlConnection GetConnection()
        {
            string connectionString = @"Data Source=DAVES-LAPTOP;Initial Catalog=Northwind;Integrated Security=True";
            return new SqlConnection(connectionString);
        }

        // get all the orders by OrderID
        public static List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>(); // empty list 
            Order order = null;
            using (SqlConnection con = GetConnection())
            {
                string query = "SELECT OrderID, CustomerID, OrderDate, RequiredDate, ShippedDate " +
                               "FROM Orders " +
                               "ORDER BY OrderID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        order = new Order();
                        order.OrderID = (int)reader["OrderID"];
                        order.CustomerID = reader["CustomerID"].ToString();
                        order.OrderDate = reader["OrderDate"] as DateTime?;
                        order.RequiredDate = reader["RequiredDate"] as DateTime?;
                        order.ShippedDate = reader["ShippedDate"] as DateTime?;
                        orders.Add(order);

                        // for any column that can be null need to determine if it is DBNull
                        // and set accordingly
                        int col = reader.GetOrdinal("ShippedDate"); // GetOrdinal is the column number of ShippedDate
                        if (reader.IsDBNull(col)) // if reader contains DBNull in this column
                            order.ShippedDate = null; // make it null in the object
                        else // is not null
                            order.ShippedDate = Convert.ToDateTime(reader["ShippedDate"]);
                    }
                } // cmd recycled
            } // connection recycled
            return orders;
        }
       
        // update order: oldShippedDate - before update, newShippedDate - new data
        public static bool UpdateOrder(Order oldOrder, Order newOrder)
        {
            bool success = false; // no success yet

            using (SqlConnection con = GetConnection())
            {
                string updateStatement = "UPDATE Orders SET" +
                                         "  ShippedDate = @NewShippedDate " + // only ShippedDate can be updated
                                         "WHERE OrderID = @OldOrderID ";  // indentifies the order
                                        // "  AND (ShippedDate = @OldShippedDate OR " + // either equal or both are null
                                        // "       ShippedDate IS NULL AND @OldShippedDate IS NULL)";
                using (SqlCommand cmd = new SqlCommand(updateStatement, con))
                {
                    // provide values for parameters
                    // for every new column (new or old) that allows null have to check if null 
                    if (newOrder.ShippedDate.Value == null) // check if null
                        cmd.Parameters.AddWithValue("@NewShippedDate", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@NewShippedDate", (DateTime)newOrder.ShippedDate);
                    cmd.Parameters.AddWithValue("@OldOrderID", oldOrder.OrderID);
                    // for every old column that allows null also have to check if null 
                    if (oldOrder.ShippedDate == null)
                            cmd.Parameters.AddWithValue("@OldShippedDate", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("OldShippedDate", (DateTime)oldOrder.ShippedDate);

                    // open connection
                    con.Open();
                    // execute Update command
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0) 
                        success = true;
                } // command object recycled
            } // connection closed and recycled
            return success;
        }
    } // class
} // namespace