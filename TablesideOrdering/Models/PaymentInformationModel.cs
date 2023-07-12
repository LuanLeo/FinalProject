namespace TablesideOrdering.Models
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; } = "Drinks";
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }

    }
}
