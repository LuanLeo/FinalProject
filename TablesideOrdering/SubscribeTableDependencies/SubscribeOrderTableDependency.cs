using TableDependency.SqlClient;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Hubs;

namespace TablesideOrdering.SubscribeTableDependencies
{
    public class SubscribeOrderTableDependency
    {
        SqlTableDependency<Orders> tableDependency;
        OrderHub orderHub;

        public SubscribeOrderTableDependency(OrderHub orderHub)
        {            
            this.orderHub = orderHub;
        }

        public void SubscribeTableDependency()
        {
            string connectionString = "server=(localdb)\\MSSQLLocalDB; database=TablesideOrderingSystem; Integrated Security=true;MultipleActiveResultSets=true ";
            tableDependency = new SqlTableDependency<Orders>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();

        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e) 
        {
            Console.WriteLine($"{nameof(Orders)} SqlTableDependency error: {e.Error.Message}");
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Orders> e) 
        {
            if(e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                orderHub.SendOrders();
            }
        }
    }
}
