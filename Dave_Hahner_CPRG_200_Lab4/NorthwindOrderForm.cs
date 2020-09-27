using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;  // ADO.NET
using DataClasses;
using BusinessClasses;

/* CPRG 200 LAB 4
 * Create GUI to update the Shipped Date for a customer's order
 * Author: David Hahner
 * Date: September 2020
 */

namespace Dave_Hahner_CPRG_200_Lab4
{
    public partial class frmNorthwindOrder : Form
    {
        public Order newOrder; // current order
        public Order oldOrder; // to store data before updating
        List<Order> orders;    // list of the OrderIds
        DateTime? tmpDate;     // temporary variable for setting new date value (can be null)
        

        public frmNorthwindOrder()
        {
            InitializeComponent();
        }
        // need to retrieve the list of OrderIDs upon form loading
        private void frmNorthwindOrder_Load(object sender, EventArgs e)
        {
            try
            {
                orders = OrderDB.GetOrders(); // get the list of OrderIDs
               
                foreach (var order in orders) // add items to the OrderID combobox
                {
                    orderIDComboBox.Items.Add(order.OrderID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading order data: " + ex.Message,
                    ex.GetType().ToString());
            }
        }

        // OrderID is selected and shows the details in the list box
        private void orderIDComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtpShippedDate.Enabled = false;
            btnUpdate.Enabled = true; // can update the shippedDate
           
            foreach (var order in orders)  // display the selected order 
            {
                if (orderIDComboBox.Text == order.OrderID.ToString())
                {
                    newOrder = oldOrder = order.CopyOrder(); // make separate copy before update
                    customerIDTextBox.Text = order.CustomerID;

                    txtOrderDate.Text = (order.OrderDate == null) ? null : Convert.ToDateTime(order.OrderDate).ToShortDateString();
                    txtRequiredDate.Text = (order.RequiredDate == null) ? null : Convert.ToDateTime(order.RequiredDate).ToShortDateString();

                    if (order.ShippedDate != null)  // ShippedDate is not null
                    {
                        dtpShippedDate.Text = Convert.ToDateTime(order.ShippedDate).ToShortDateString();

                    }
                    else // ShippedDate is null
                    {
                        dtpShippedDate.CustomFormat = " ";
                        dtpShippedDate.Format = DateTimePickerFormat.Custom;
                        order.ShippedDate = tmpDate;
                    }
                }
            }
            // get the OrderDetails for display in the list box
            List<OrderDetail> orderDetails = OrderDetailDB.GetOrderDetails();
            lstOrdDetails.Items.Clear(); // clear the list box
            int i = 0; 
            decimal orderTotal = 0; // the total amount of each order for the OrderID
           
            foreach (var orderDetail in orderDetails) // details to be listed for the chosen OrderID
            {
                if (orderIDComboBox.Text == orderDetail.OrderID.ToString())
                {
                    lstOrdDetails.Items.Add(orderDetail.OrderID.ToString());
                    lstOrdDetails.Items[i].SubItems.Add(orderDetail.ProductID.ToString());
                    lstOrdDetails.Items[i].SubItems.Add(orderDetail.Quantity.ToString());
                    lstOrdDetails.Items[i].SubItems.Add(orderDetail.UnitPrice.ToString("c"));
                    lstOrdDetails.Items[i].SubItems.Add(orderDetail.Discount.ToString("p0"));
                    lstOrdDetails.Items[i].SubItems.Add(orderDetail.OrderTotal.ToString("c"));

                    orderTotal += orderDetail.OrderTotal;
                    i++;
                }
            }
            txtTotalAmount.Text = orderTotal.ToString("c"); // display to total sum 
        }

        // update button selected enables controls
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            dtpShippedDate.Enabled = true;
            btnSave.Enabled = true;
            btnSave.Focus();
        }

        // resets the DateTimepicker after update selected if previously deleted to be null entry
        private void dtpShippedDate_ValueChanged(object sender, EventArgs e)
        {
              dtpShippedDate.CustomFormat = "yyyy/MM/dd";
        
            if (dtpShippedDate.Value != null)
            {
                dtpShippedDate.Format = DateTimePickerFormat.Short;
                tmpDate = dtpShippedDate.Value;
            }
        }

        // user selects null value by using backspace or delete to remove the input
        private void dtpShippedDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
           {
                dtpShippedDate.CustomFormat = " ";
                dtpShippedDate.Format = DateTimePickerFormat.Custom;
                tmpDate = null;
            }
        }

        // user selects save after updating the ShippedDate
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool update = false;
            if (dtpShippedDate != null) // has value
            {
                newOrder.ShippedDate = dtpShippedDate.Value; // get the new value
                // validate the ShippedDate
                if (oldOrder.OrderDate != null) // has value
                {
                    if (oldOrder.RequiredDate != null) // has value
                    {
                        if (oldOrder.OrderDate <= newOrder.ShippedDate &&
                            oldOrder.RequiredDate >= newOrder.ShippedDate)
                        {   // new value for ShippedDate should not be earlier than OrderDate and no later than RequiredDate
                            update = true;
                        }
                        else // outside of requested parameters show an error message
                        {
                            update = false;
                            MessageBox.Show("Shipped Date can not be earlier than the Order Date or later than the Required Date");
                        }
                    }
                    else // only requiredDate is unknown
                    {
                        if (oldOrder.OrderDate <= newOrder.ShippedDate) { update = true; }
                        else MessageBox.Show("Shipped Date can not be earlier than the Order Date");
                    }
                }
                else // only OrderDate is unknown
                {
                    if (oldOrder.RequiredDate != null)  
                    {
                        if (oldOrder.RequiredDate >= newOrder.ShippedDate) { update = true; }
                        else MessageBox.Show("Shipped Date can not be later than the Required Date");
                    }
                    else update = true; // both Order and Required dates are unknown
                }
            }
            else // ShippedDate is unknown
            {
                newOrder.ShippedDate = null;
                update = true;
            }
            if (update) 
            {
                bool success = OrderDB.UpdateOrder(oldOrder, newOrder);
                if (update)
                {
                    MessageBox.Show("Shipped Date updated successfully!");
                }
                else // error in ShippedDate entry
                {
                    MessageBox.Show("Shipped Date update failed!");
                }
            }
           
            btnSave.Enabled = false; // reset the save button
        }
        // closes the form once exit button clicked
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

