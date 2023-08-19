using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff, Admin")]
    public class DeliveryOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DeliveryOrderController(ApplicationDbContext context) 
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var processOrder = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "Processing");
            return View(processOrder);
        }

        public IActionResult DoneDeliveryOrders()
        {
            var processOrder = _context.Orders.Where(i => i.OrderType == "Delivery" && i.Status == "Done");
            return View(processOrder);
        }
    }
}
