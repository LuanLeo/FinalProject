using Microsoft.AspNetCore.SignalR;
using TablesideOrdering.SignalR.Repositories;

namespace TablesideOrdering.SignalR.Hubs
{
    public class OrderHub : Hub
    {
        OrderRepository orderRepository;
        protected IHubContext<OrderHub> _context;
        public OrderHub(IConfiguration configuration, IHubContext<OrderHub> context)
        {
            var connectionString = configuration.GetConnectionString("TablesideOrdering");
            orderRepository = new OrderRepository(connectionString);
            _context = context;
        }

        public async Task SendOrders()
        {
            var orders = orderRepository.GetOrders();
            await this._context.Clients.All.SendAsync("ReceivedOrders", orders);
        }
    }
}
