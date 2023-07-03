using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.Admin.ViewModels;
using TablesideOrdering.Areas.Staff.ViewModels;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }
        public OrderController(INotyfService notyfService, ApplicationDbContext context)
        {
            _notyfService = notyfService;
            _context = context;

        }
        public IActionResult Index()
        {
            var order = _context.Orders;
            return View(order);
        }
        public async Task<IActionResult> Delete(int id)
        {

            var order = GetOrderByID(id);
            OrderViewModel OrderData = new OrderViewModel();
            //List<OrderViewModel> orderList = new List<OrderViewModel>();
            var orderList = (from o in _context.ProductSizePrice
                             where id == o.Id
                             select new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderPrice = order.OrderPrice,
                OrderDate = order.OrderDate,
                ProductQuantity = order.ProductQuantity,
                PhoneNumber = order.PhoneNumber,
                TableNo = order.TableNo,
            });
            var orderDetailList = (from o in _context.OrderDetails
                                   where id == o.OrderId
                                   select new OrderViewModel
                                   {
                                       OrderId = o.OrderId,
                                       OrderDetailId= o.OrderDetailId,
                                       ProductName = o.ProductName,
                                       Size = o.Size,
                                       ProQuantity = o.ProQuantity,
                                       Price = o.Price,
                                   });
            OrderData.OrderDetail = orderDetailList;
            OrderData.Order = orderList;
            return PartialView("Delete", OrderData);
        }
        public Orders GetOrderByID(int id)
        {
            return _context.Orders.FirstOrDefault(x => x.OrderId == id);
        }

    }
}
