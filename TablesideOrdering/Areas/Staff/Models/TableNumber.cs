using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.Staff.Models
{
    public class TableNumber
    {
        [Key]
        public string IdTable { get; set; }
    }
}
