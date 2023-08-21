namespace TablesideOrdering.Areas.Staff.Models
{
    public class TableBooking
    {
        public int Id { get; set; }
        public string Table { get; set; }
        public DateTime BookDate { get; set; }
        public string CusName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
