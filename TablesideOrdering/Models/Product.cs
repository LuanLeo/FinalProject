using System.ComponentModel.DataAnnotations;
namespace TablesideOrdering.Models
{
    public class Product
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Pic { get; set; }
        public IFormFile PicFile { get; set; }
    }
}
