using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using Microsoft.AspNetCore.Authorization;
using TablesideOrdering.Areas.StoreOwner.ViewModels;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles ="Staff, Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static int Num = 0;
        public INotyfService _notyfService { get; }
        public OrderController(INotyfService notyfService, ApplicationDbContext context)
        {
            _notyfService = notyfService;
            _context = context;
        }

        public IActionResult Index()
        {
            if (Num == 0)
            {
                ViewBag.Num = Num;
                ViewBag.Message = "New order has been updated";
            }
            Num = 0;
            return View();
        }

        //Functions for DONE ORDERS
        public IActionResult DoneOrders()
        {
            List<Orders> OList = new List<Orders>();
            foreach (var orders in _context.Orders)
            {
                if (orders.Status == "Done" && orders.OrderType == "Eat in")
                    OList.Add(orders);
            }
            return View(OList);
        }

        //Functions for PROCESSING ORDERS
        public IActionResult Details(int id)
        {
            OrderViewModel OrderData = new OrderViewModel();
            List<OrderDetail> OList = new List<OrderDetail>();
            OrderData.Order = _context.Orders.Find(id);
            foreach(var item in _context.OrderDetails)
            {
                if(item.OrderId == id)
                {
                    OList.Add(item);
                }
            };
            OrderData.OrderDetail = OList;
            return View(OrderData);
        }

        public void MarkDone(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            order.Status = "Done";

            _context.SaveChanges();
            _notyfService.Success("The order is marked as done", 5);
            
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            Num++;
            return PartialView("Delete", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(OrderViewModel model)
        {
            var order = await _context.Orders.FindAsync(model.Order.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            foreach (var od in _context.OrderDetails)
            {
                if (od.OrderId == model.Order.OrderId)
                {
                    _context.OrderDetails.Remove(od);
                }
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            Num++;
            _notyfService.Success("The order is deleted", 5);
            return RedirectToAction(nameof(Index));
        }
    }
}
