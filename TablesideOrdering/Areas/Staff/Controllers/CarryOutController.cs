using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff, Admin")]
    public class CarryOutController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static int Num;
        public CarryOutController(ApplicationDbContext context)
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
        public void Ready(int id)
        {
            Orders order = _context.Orders.Where(o => o.OrderId == id).FirstOrDefault();
            order.Status = "Ready";
            _context.SaveChanges();
        }
        public IActionResult DoneCarryOutOrders()
        {
            var processOrder = _context.Orders.Where(i => i.OrderType == "Carry out" && i.Status == "Done");
            return View(processOrder);
        }
        public IActionResult ReadyOrders()
        {
            var ready = _context.Orders.Where(i => i.OrderType == "Carry out" && i.Status == "Ready");
            return View(ready);
        }
    }
}

