using System.Data.SqlClient;
using System.Data;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Migrations;

namespace TablesideOrdering.Repositories
{
    public class OrderRepository
    {
        string connectionString;

        public OrderRepository (string connectionString)
        {
            this.connectionString = connectionString;
        }

        public  List<Orders> GetOrders()
        {
            List<Orders> orders = new List<Orders>();
            Orders order;
            var data = GetOrderDetailsFromDb();
            foreach(DataRow row in data.Rows)
            {
                order=new Orders
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    OrderPrice = Convert.ToSingle(row["OrderPrice"]),
                    ProductQuantity = Convert.ToInt32(row["ProductQuantity"]),
                    PhoneNumber = row["PhoneNumber"].ToString()
                };
                orders.Add(order);
            }
            return orders;
        }
        
        public DataTable GetOrderDetailsFromDb()
        {
            var query = "SELECT OrderId, OrderDate, OrderPrice, ProductQuantity, PhoneNumber FROM Orders";
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
