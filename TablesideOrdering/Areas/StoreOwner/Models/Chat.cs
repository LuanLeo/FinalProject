using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.StoreOwner.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public string TableId { get; set; }
        public int ChatRoomID { get; set; }
    }
}
