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
        public IActionResult DoneOrders()
        {
            var order = _context.Orders.FirstOrDefault(o => o.Status == "Done");
            List<Orders> OList = new List<Orders>();
            OList.Add(order);
            return View(OList);
        }
        public IActionResult NotPaidOrders()
        {
            var order = _context.Orders.FirstOrDefault(o => o.Status == "Not Paid");
            List<Orders> OList = new List<Orders>();
            OList.Add(order);
            return View(OList);
        }
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
            _notyfService.Success("The user is deleted", 5);
            return RedirectToAction(nameof(Index));
        }
    }
}
