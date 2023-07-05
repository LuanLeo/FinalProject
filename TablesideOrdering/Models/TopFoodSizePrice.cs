using TablesideOrdering.Areas.Admin.Models;

namespace TablesideOrdering.Models
{
    public class TopFoodSizePrice
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
    }

    public class ProductFull
    {
        public int ProductId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Pic { get; set; }
        public string Size { get; set; }
        public float Price { get;set; }
        public int ProSizePriceId { get; set; }

    }

    public class TopFood
    {
        public ProductFull ProductFull { get; set; }
        public float TotalPrice { get; set; }
    }
}
