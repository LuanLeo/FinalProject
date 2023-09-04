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
            //Create chat room from customer
            ChatId = TablesideOrdering.Controllers.HomeController.TableNo;
            List<SelectListItem> SelectList = TablesideOrdering.Areas.Staff.Controllers.HomeController.SelectList;
            if (SelectList != null)
            {
                foreach (var list in SelectList)
                {
                    if (ChatId == list.Value)
                    {
                        Groups.AddToGroupAsync(Context.ConnectionId, list.Value);
                    }
                }
            }

            //Create chat room from staff
            if (ChatId == null)
            {
                Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            }

            return base.OnConnectedAsync();
        }

        //Send message to two groups
        public void SendMessageToGroup(string sender, string receiver, string message)
        {
            SendMessageFromStaff(sender, message);
            SendMessageToCustomer(sender, receiver, message);
        }

        //Send message to Staff gr
        public Task SendMessageToCustomer(string sender, string receiver, string message)
        {
            return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }

        //Send message to Customer gr
        public Task SendMessageFromStaff(string sender, string message)
        {
            return Clients.Group("admin@gmail.com").SendAsync("ReceiveMessage", sender, message);
        }
    }
}
