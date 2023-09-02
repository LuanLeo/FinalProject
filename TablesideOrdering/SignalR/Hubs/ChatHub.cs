using Microsoft.AspNetCore.SignalR;

namespace TablesideOrdering.SignalR.Hubs
{
    public class ChatHub:Hub
    {
        public async void SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);
        }
    }
}
