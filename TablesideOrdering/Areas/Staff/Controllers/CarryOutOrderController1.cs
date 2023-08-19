using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff, Admin")]
    public class CarryOutOrderController1 : Controller
    {
        private readonly ApplicationDbContext _context;
        public static int Num;
        public CarryOutOrderController1(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var processOrder = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "Processing");
            if (Num == 0)
            {
                ViewBag.Num = Num;
                ViewBag.Message = "New order has been updated";
            }
            Num = 0;
            return View(processOrder);
        }
        public void Delivery(int id)
        {
            Orders order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
            order.Status = "Delivering";
        }
        public IActionResult DoneDeliveryOrders()
        {
            var processOrder = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "Done");
            return View(processOrder);
        }
    }
}
