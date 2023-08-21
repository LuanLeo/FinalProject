using System.Security.Cryptography.X509Certificates;
using TablesideOrdering.Areas.StoreOwner.Models;

namespace TablesideOrdering.Areas.Staff.ViewModels
{
    public class OrderViewModel
    {
        public IQueryable<OrderViewModel> Order { get; set; }
        public IQueryable<OrderViewModel> OrderDetail { get; set; }
        public IQueryable<OrderViewModel> ProductSizePrices{ get; set; }

        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public string TableNo { get; set; }
        public float OrderPrice { get; set; }
        public string PhoneNumber { get; set; }
        public int ProductQuantity { get; set; }
        public int OrderDetailId { get; set; }
        public string ProductName {get; set; }
        public string Size { get; set; }
        public int ProQuantity { get; set; }
        public float Price { get; set; }
        public string Status { get; set; }
        public string CusName { get; set; }
        public string OrderType { get; set; }
        public string Address { get; set; }
        public TimeSpan PickTime { get; set; }
    }
}
