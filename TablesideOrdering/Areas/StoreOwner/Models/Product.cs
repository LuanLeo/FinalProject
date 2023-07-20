using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TablesideOrdering.Areas.StoreOwner.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Pic { get; set; }
    }
}
