using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using TablesideOrdering.ViewModels;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff, Admin")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TableList()
        {
            return View(_context.Tables.ToList());
        }

        public void SwitchTableStatus(int id)
        {
            var table = _context.Tables.Find(id);
            if (table.Status == "Available")
            {
                table.Status = "Busy";
            }
            else
            {
                table.Status = "Available";
            }
            _context.Update(table);
            _context.SaveChanges();
        }

        public void AvailableAll()
        {
            foreach (var tab in _context.Tables)
            {
                if (tab.Status == "Busy")
                {
                    tab.Status = "Available";
                }
                _context.Update(tab);
            }
            _context.SaveChanges();
        }
    }
}
