using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Models.Admin
{
    public class ProductSize
    {
        [Key]
        public int SizeId { get; set; }
        public string SizeName { get; set; }
    }
}
