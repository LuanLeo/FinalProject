﻿using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using TableDependency.SqlClient;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.SignalR.Hubs;

namespace TablesideOrdering.SignalR.SubscribeTableDependencies
{
    public class SubscribeOrderTableDependency
    {
        SqlTableDependency<Orders> tableDependency;
        OrderHub orderHub;
        public INotyfService notyfService { get; }

        public SubscribeOrderTableDependency(OrderHub orderHub)
        {
            this.orderHub = orderHub;
        }

        public void SubscribeTableDependency()
        {
            string connectionString = "server=sql.bsite.net\\MSSQL2016;  Initial Catalog=llcoffee_;Persist Security Info=True;User ID=llcoffee_;Password=Longtran1; MultipleActiveResultSets=True";
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
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                orderHub.SendOrders();
            }
        }
    }
}
