using TablesideOrdering.ViewModels;

namespace TablesideOrdering.Models
{
    public class Cart
    {
        public List<CartViewModel> cartViewModels { get; set; }
        public float CartTotal { get; set; }
        public string PhoneNumber { get; set; }
        public float DicountAmount { get; set; }
        public float MustPaid { get; set; }
    }
}
