using Microsoft.AspNetCore.SignalR;
using System.Drawing.Drawing2D;
using TablesideOrdering.Repositories;

namespace TablesideOrdering.Hubs
{
    public class OrderHub : Hub
    {
        OrderRepository orderRepository;

        public OrderHub(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TablesideOrdering");
            orderRepository= new OrderRepository(connectionString);
        }

        public async Task SendOrders()
        {
            var orders = orderRepository.GetOrders();
            await Clients.All.SendAsync("ReceivedOrders", orders);
        }
    }
}
