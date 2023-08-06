using System.ComponentModel.DataAnnotations;
using System.Drawing;
using TablesideOrdering.Areas.StoreOwner.Models;

namespace TablesideOrdering.Models
{
    public class AddToCart
    {
        public int SizePriceId { get; set; }
        public Product Product { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float TotalProPrice { get; set; }
    }
}
