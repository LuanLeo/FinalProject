using System.ComponentModel.DataAnnotations;
using TablesideOrdering.Models;

namespace TablesideOrdering.ViewModels
{
    public class ProductViewModel
    {
        [Key]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string Pic { get; set; }
        public IFormFile PicFile { get; set; } 
        public IQueryable<Product> Product { get; set; }
        public string ExistingImage { get; set; }
    }
}
