namespace TablesideOrdering.ViewModels
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public string Pic { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public float Price {  get; set; }
        public float TotalProPrice {  get; set; }
    }
}
