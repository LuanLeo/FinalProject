using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class VirtualCart
    {
        [Key]
        public int CartId { get; set; }
        public string TableId { get; set; }
        public float CartAmount { get; set; }//cart total
    }
}
