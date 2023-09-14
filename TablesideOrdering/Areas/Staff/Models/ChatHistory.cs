namespace TablesideOrdering.Areas.Staff.Models
{
    public class ChatHistory
    {
        public string ChatRoomId { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public string Receiver { get; set; }
        public string MessageDate { get; set; }

    }
}
