namespace TablesideOrdering.Areas.StoreOwner.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public string DisCode { get; set; }
        public float DisValue { get; set; }
        public string DisType { get; set; }
        public DateTime DayStart { get; set; }
        public DateTime DayEnd { get; set; }
    }
}
