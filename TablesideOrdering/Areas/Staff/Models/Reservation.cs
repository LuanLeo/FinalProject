using System.ComponentModel.DataAnnotations;

namespace TablesideOrdering.Areas.Staff.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public string CusName { get; set; }
        public DateTime Datetime { get; set; }
        public string Email { get; set; }
        public int People { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
        public int OrderId { get; set; }
    }
}
