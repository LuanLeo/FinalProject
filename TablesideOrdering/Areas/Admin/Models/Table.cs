using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.Admin.Models
{
    public class Table
    {
        [Key]
        public string IdTable { get; set; }
        public string Status { get; set; }
    }
}
