using System.Data.SqlClient;
using System.Data;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Migrations;

namespace TablesideOrdering.SignalR.Repositories
{
    public class OrderRepository
    {
        string connectionString;

        public OrderRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Orders> GetOrders()
        {
            List<Orders> orders = new List<Orders>();
            var data = GetOrderDetailsFromDb();
            foreach (DataRow row in data.Rows)
            {
                if(row["Status"].ToString() == "Processing")
                {
                    Orders order = new Orders
                    {
                        OrderId = Convert.ToInt32(row["OrderId"]),
                        OrderDate = row["OrderDate"].ToString(),
                        OrderPrice = Convert.ToSingle(row["OrderPrice"]),
                        ProductQuantity = Convert.ToInt32(row["ProductQuantity"]),
                        PhoneNumber = row["PhoneNumber"].ToString(),
                        TableNo = row["TableNo"].ToString(),
                        Status = row["Status"].ToString(),
                        CusName = row["CusName"].ToString(),
                        OrderType = row["OrderType"].ToString(),
                        Address = row["Address"].ToString(),
                        PickTime = TimeSpan.Parse(row["PickTime"].ToString()),
                        CouponId = Convert.ToInt32(row["CouponId"]),

                    };
                    orders.Add(order);
                }
            }
            return orders;
        }

        public DataTable GetOrderDetailsFromDb()
        {
            var query = "SELECT OrderId, OrderDate, OrderPrice, ProductQuantity, PhoneNumber, TableNo, Status, CusName, OrderType, Address, PickTime, CouponId FROM Orders";
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }

            }
        }
    }
}
