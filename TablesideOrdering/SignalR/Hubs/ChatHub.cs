using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;
using Twilio.TwiML.Fax;

namespace TablesideOrdering.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private static List<string> ChatRoomList;
        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public static string StaffName;

        //Take chat room id
        public List<string> Chat()
        {
            List<string> chatIds = new List<string>();
            foreach (var chat in _context.Chats)
            {
                chatIds.Add(chat.ChatRoomID.ToString());
            }
            return chatIds;
        }

        public override Task OnConnectedAsync()
        {
            CheckRole();
            //Create chat room from customer
            ChatRoomList = Chat();

            if (ChatRoomList != null)
            {
                foreach (var list in ChatRoomList)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, list);
                }
            }
            //Create chat room from staff
            else
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
                SendMessageToStaff(sender, message);

                ChatHistory chat = new ChatHistory();
                if (sender != StaffName)
                {
                    chat.ChatRoomId = sender;
                }
                else
                {
                    chat.ChatRoomId = receiver;
                }
                chat.Sender = sender;
                chat.Message = message;
                chat.Receiver = StaffName;
                chat.MessageDate = DateTime.Now.ToString();

                history.Add(chat);
                TablesideOrdering.Areas.Staff.Controllers.ChatController.ChatViewModel.ChatHistory = history;
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

        static List<ChatHistory> history = new List<ChatHistory>();

        //Send message to Customer gr
        public Task SendMessageToStaff(string sender, string message)
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
