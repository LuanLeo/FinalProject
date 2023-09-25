using System.Data.SqlClient;
using System.Data;
using TablesideOrdering.Areas.StoreOwner.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;

namespace TablesideOrdering.SignalR.Repositories
{
    public class ChatRepository
    {
        string connectionString;

        public ChatRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Chat> GetChats()
        {
            List<Chat> chats = new List<Chat>();
            var data = GetOrderDetailsFromDb();
            foreach (DataRow row in data.Rows)
            {
                Chat chat = new Chat
                {
                    Id = Convert.ToInt32(row["Id"]),
                    TableId = row["TableId"].ToString(),
                    ChatRoomID = row["ChatRoomID"].ToString()
                };
                chats.Add(chat);
            }
            return chats;
        }

        public DataTable GetOrderDetailsFromDb()
        {
            var query = "SELECT Id, TableId, ChatRoomID FROM Chats";
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
