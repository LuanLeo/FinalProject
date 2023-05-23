using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace TablesideOrdering.Models
{
    public class ProductSizePrice
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
    }
}
