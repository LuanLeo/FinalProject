using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.Admin.Models
{
    public class ProductSize
    {
        [Key]
        public int SizeId { get; set; }
        public string SizeName { get; set; }
    }
}
