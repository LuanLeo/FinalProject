using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using TablesideOrdering.Data;

namespace TablesideOrdering.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public static string ChatId;
        public static string StaffName;
        public override Task OnConnectedAsync()
        {
            CheckRole();

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
                Groups.AddToGroupAsync(Context.ConnectionId, StaffName);
            }
            return base.OnConnectedAsync();
        }


        //Check Role
        public void CheckRole()
        {
            if (Context.User.IsInRole("Staff") == true || Context.User.IsInRole("Admin") == true)
            {
                StaffName = Context.User.Identity.Name;
            };
        }

        //Send message to two groups
        public void SendMessageToGroup(string sender, string receiver, string message)
        {
            //Lock spam text if user doesn't have chat id 
            if (sender != "")
            {
                SendMessageToCustomer(sender, receiver, message);
                SendMessageFromStaff(sender, message);
            }
        }

        //Send message to Staff gr
        public Task SendMessageToCustomer(string sender, string receiver, string message)
        {
            if (message != "" && sender != "")
            {
                //Delete first space(s) of the message
                var messageChanges = message.TrimStart().TrimEnd();
                return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, messageChanges);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        //Send message to Customer gr
        public Task SendMessageFromStaff(string sender, string message)
        {
            if (message != "")
            {
                //Delete first space(s) of the message
                var messageChanges = message.TrimStart().TrimEnd();
                return Clients.Group(StaffName).SendAsync("ReceiveMessage", sender, messageChanges);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
