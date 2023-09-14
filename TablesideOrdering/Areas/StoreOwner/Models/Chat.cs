using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.StoreOwner.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public int TableId { get; set; }
        public string ChatRoomID { get; set; }
    }
}
