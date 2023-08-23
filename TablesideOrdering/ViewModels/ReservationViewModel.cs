namespace TablesideOrdering.ViewModels
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public string CusName { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Email { get; set; }
        public int People { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
        public int OrderId { get; set; }

    }
}
