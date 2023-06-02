namespace TablesideOrdering.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public float OrderPrice { get; set; }
        public int ProductQuantity { get; set; }
    }
}
