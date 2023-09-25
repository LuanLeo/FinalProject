﻿using TablesideOrdering.SignalR.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace TablesideOrdering.SignalR.Hubs
{
    public class ChatListHub : Hub
    {
        ChatRepository chatRepository;

        public ChatListHub(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TablesideOrdering");
            chatRepository = new ChatRepository(connectionString);
        }

        public async Task SendChats()
        {
            var chats = chatRepository.GetChats();
            await Clients.All.SendAsync("ReceivedChats", chats);
        }
    }
}