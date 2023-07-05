using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
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
        public IActionResult Detail(int id)
        {
            OrderViewModel OrderData = new OrderViewModel();
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            var orders = (from o in _context.Orders
                                   where id == o.OrderId
                                   select new OrderViewModel
                                   {
                                       OrderId = o.OrderId,
                                       OrderDate = o.OrderDate,
                                       OrderPrice = o.OrderPrice,
                                       PhoneNumber = o.PhoneNumber,
                                       ProductQuantity = o.ProductQuantity,
                                       TableNo = o.TableNo,                                       
                                   });
            var orderDetailList = (from o in _context.OrderDetails
                                   where id == o.OrderId
                                   select new OrderViewModel
                                   {
                                       OrderId = o.OrderId,
                                       OrderDetailId = o.OrderDetailId,
                                       ProductName = o.ProductName,
                                       Size = o.Size,
                                       ProQuantity = o.ProQuantity,
                                       Price = o.Price,
                                       SubTotal = o.Price * o.ProQuantity,
                                   });
            OrderData.OrderDetail = orderDetailList;
            OrderData.Order = orders;
            return View(OrderData);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {            
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            return PartialView("Delete", order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(OrderViewModel model)
        {
            var order = await _context.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return NotFound();
            }
            List<OrderDetail> orderDetailsList= new List<OrderDetail>();
            foreach(var od in _context.OrderDetails)
            {
                if(od.OrderId== model.OrderId)
                {
                    orderDetailsList.Add(od);
                }
            }
            foreach (var item in orderDetailsList)
            {
                _context.OrderDetails.Remove(item);
            }
            _context.Orders.Remove(order);
            _context.SaveChanges();
            //notyfService.Success("The user is deleted", 5);
            return RedirectToAction(nameof(Index));
        }
        public Orders GetOrderByID(int id)
        {
            return _context.Orders.FirstOrDefault(x => x.OrderId == id);
        }

    }
}
