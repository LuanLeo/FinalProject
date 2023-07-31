using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Areas.Staff.ViewModels;
using TablesideOrdering.Data;
using Microsoft.AspNetCore.Authorization;

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
                if (orders.Status == "Done")
                    OList.Add(orders);
            }
            var order = _context.Orders.FirstOrDefault(o => o.Status == "Done");
            return View(OList);
        }


        //Functions for NOT PAID ORDERS
        public IActionResult NotPaidOrders()
        {
            List<Orders> OList = new List<Orders>();
            foreach (var orders in _context.Orders)
            {
                if (orders.Status == "Not Paid")
                    OList.Add(orders);
            }
            return View(OList);
        }
        [HttpGet]
        public IActionResult PaidOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            return PartialView("PaidOrder", order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkPaid(Orders model)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == model.OrderId);
            order.Status = "Processing";

            _context.SaveChanges();
            _notyfService.Success("The order is paid", 5);
            return RedirectToAction("NotPaidOrders");
        }

        [HttpGet]
        public IActionResult DeleteNotPaid(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            return PartialView("DeleteNotPaid", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNotPaid(Orders model)
        {
            var order = await _context.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            foreach (var od in _context.OrderDetails)
            {
                if (od.OrderId == model.OrderId)
                {
                    _context.OrderDetails.Remove(od);
                }
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            _notyfService.Success("The order is deleted", 5);
            return RedirectToAction("NotPaidOrder");
        }


        //Functions for PROCESSING ORDERS
        public IActionResult Details(int id)
        {
            OrderViewModel OrderData = new OrderViewModel();
            OrderData.Order = (from o in _context.Orders
                               where id == o.OrderId && o.Status == "Processing"
                               select new OrderViewModel
                               {
                                   OrderId = o.OrderId,
                                   OrderDate = o.OrderDate,
                                   OrderPrice = o.OrderPrice,
                                   PhoneNumber = o.PhoneNumber,
                                   ProductQuantity = o.ProductQuantity,
                                   TableNo = o.TableNo,
                                   Status = o.Status,
                                   CusName = o.CusName
                               });

            OrderData.OrderDetail = (from o in _context.OrderDetails
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
            return View(OrderData);
        }

        public IActionResult MarkDone(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            order.Status = "Done";

            _context.SaveChanges();
            _notyfService.Success("The order is marked as done", 5);
            return RedirectToAction("Index");
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
            var order = await _context.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            foreach (var od in _context.OrderDetails)
            {
                if (od.OrderId == model.OrderId)
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
