using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.StoreOwner.Models
{
    public class Table
    {
        [Key]
        public string IdTable { get; set; }
        public string Status { get; set; }
        public string PeopleCap { get; set; }
    }
}
