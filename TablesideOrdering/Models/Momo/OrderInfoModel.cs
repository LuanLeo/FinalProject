namespace TablesideOrdering.Models.Momo
{
    public class OrderInfoModel
    {
        public string FullName { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; } = "Checkout at L&L coffee";
        public double Amount { get; set; }
    }
}
