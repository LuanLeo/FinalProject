using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using TablesideOrdering.Data;

namespace TablesideOrdering.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public static string ChatId;
        public override Task OnConnectedAsync()
        {
            ChatId = TablesideOrdering.Controllers.HomeController.TableNo;
            List<SelectListItem> SelectList = TablesideOrdering.Areas.Staff.Controllers.HomeController.SelectList;
            foreach (var list in SelectList)
            {
                if (ChatId == list.Value)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, list.Value);
                }
            }
            return base.OnConnectedAsync();
        }

        public Task SendMessageToGroup(string sender, string receiver, string message)
        {
            return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }

    }
}
