namespace TablesideOrdering.Models.Admin
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public int ProQuantity { get; set; }
        public float Price { get; set; }
    }
}
