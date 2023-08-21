using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using TablesideOrdering.Areas.Staff.ViewModels;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff, Admin")]
    public class DeliveryOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static int Num;
        public DeliveryOrderController(ApplicationDbContext context)
        {
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
        public void Delivery(int id)
        {
            Orders order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
            order.Status = "Delivering";
            _context.SaveChanges();
        }
        public IActionResult DoneDeliveryOrders()
        {
            var processOrder = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "Done");
            return View(processOrder);
        }
        public IActionResult Delivering()
        {
            var delivering = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "delivering");
            return View(delivering);

        }
    }
}
