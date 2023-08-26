using TablesideOrdering.Areas.StoreOwner.Models;

namespace TablesideOrdering.Areas.StoreOwner.ViewModels
{
    public class OrderViewModel
    {
        public Orders Order { get; set; }
        public List<Orders> OrderList { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
    }
}
