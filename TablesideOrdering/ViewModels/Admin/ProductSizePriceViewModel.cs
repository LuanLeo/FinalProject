
using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.ViewModels.Admin
{
    public class ProductSizePriceViewModel
    {
        public int SizePriceId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Pic { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
    }
}
