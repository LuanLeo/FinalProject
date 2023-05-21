using System.Drawing;

namespace TablesideOrdering.Models
{
    public class ProductSizePrice
    {
        public Product product { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
    }
}
