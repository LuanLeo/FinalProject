using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.StoreOwner.Models
{
    public class Table
    {
        [Key]
        public int Id { get; set; }
        public int IdTable { get; set; }
        public string Status { get; set; }
        public string PeopleCap { get; set; }
    }
}
