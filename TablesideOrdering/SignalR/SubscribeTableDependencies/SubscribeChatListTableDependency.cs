using AspNetCoreHero.ToastNotification.Abstractions;
using TableDependency.SqlClient;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.SignalR.Hubs;

namespace TablesideOrdering.SignalR.SubscribeTableDependencies
{
    public class SubscribeChatListTableDependency
    {
        SqlTableDependency<Chat> tableDependency;
        ChatListHub chatlistHub;
        public INotyfService notyfService { get; }

        public SubscribeChatListTableDependency(ChatListHub chatlistHub)
        {
            this.chatlistHub = chatlistHub;
        }

        public void SubscribeTableDependency()
        {
            string connectionString = "server=(localdb)\\MSSQLLocalDB; database=TablesideOrderingSystem; Integrated Security=true;MultipleActiveResultSets=true ";
            tableDependency = new SqlTableDependency<Chat>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();

        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(Chat)} SqlTableDependency error: {e.Error.Message}");
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Chat> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
               chatlistHub.SendChats();
            }
        }
    }
}
