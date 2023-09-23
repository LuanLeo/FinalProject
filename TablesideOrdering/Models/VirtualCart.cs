using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class VirtualCart
    {
        [Key]
        public int CartId { get; set; }
        public int TableId { get; set; }
        public float CartAmount { get; set; }//cart total
    }
}
