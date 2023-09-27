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

        public static string StaffRole;

        //Take chat room id
        public List<string> Chat()
        {
            List<string> chatIds = new List<string>();
            foreach (var chat in _context.Chat)
            {
                chatIds.Add(chat.ChatRoomID.ToString());
            }
            return chatIds;
        }

        public override Task OnConnectedAsync()
        {
            StaffRole = TablesideOrdering.Areas.Staff.Controllers.ChatController.StaffRole;
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
                Groups.AddToGroupAsync(Context.ConnectionId, StaffRole);
            }
            return base.OnConnectedAsync();
        }

        //Send message to two groups
        public void SendMessageToGroup(string sender, string receiver, string message)
        {
            //Lock spam text if user doesn't have chat id 
            if (sender != "")
            {
                ChatHistory chat = new ChatHistory();
                if (sender != StaffRole)
                {
                    chat.ChatRoomId = sender;
                }
                else
                {
                    chat.ChatRoomId = receiver;
                }
                chat.Sender = sender;
                chat.Message = message;
                chat.Receiver = StaffRole;
                chat.MessageDate = DateTime.Now.ToString();

                history.Add(chat);
                TablesideOrdering.Areas.Staff.Controllers.ChatController.ChatViewModel.ChatHistory = history;

                SendMessageToCustomer(sender, receiver, message);
                SendMessageToStaff(sender, message);
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
                return Clients.Group(StaffRole).SendAsync("ReceiveMessage", sender, messageChanges);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
