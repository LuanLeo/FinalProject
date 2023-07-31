using TablesideOrdering.Areas.StoreOwner.Models;

namespace TablesideOrdering.Areas.StoreOwner.ViewModels
{
    public class OrderViewModel
    {
        public List<Orders> Order { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
    }
}
