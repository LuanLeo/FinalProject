using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TablesideOrdering.Data;
using TablesideOrdering.Models;
using TablesideOrdering.ViewModels;

namespace TablesideOrdering.Controllers
{
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
            var Homedata = new HomeViewModel();
            var product = (from products in _context.Products
                           select new Product
                           {
                               ProductId = products.ProductId,
                               Description = products.Description,
                               Name = products.Name,
                               Pic = products.Pic,
                               CategoryId = products.CategoryId,
                           });
            var cat = (from categories in _context.Categories
                       select new Category
                       {
                           CategoryId = categories.CategoryId,
                           CategoryName = categories.CategoryName,
                       });
            Homedata.Category = cat;
            Homedata.Product = product;
            return View(Homedata);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}