namespace TablesideOrdering.ViewModels
{
    public class TopFoodSizePrice
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
    }

    public class TopFoodSizePriceDistinct
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }

        public float TotalPrice { get; set; }
    }
}
