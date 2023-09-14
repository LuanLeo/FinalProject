using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Areas.StoreOwner.Models;
namespace TablesideOrdering.Areas.Staff.ViewModels
{
    public class ChatViewModel
    {
        public List<Chat> ChatRoomList { get; set; }
        public Chat ChatRoom { get; set; }
        public List<ChatHistory> ChatHistory { get; set; }
    }
}
