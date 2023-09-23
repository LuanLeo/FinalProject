using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId {  get; set; }
        public int CartId { get; set; }
        public int SizePriceId { get; set; }
        public int Quantity { get; set; }
    }
}
