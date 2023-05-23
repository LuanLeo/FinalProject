using AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
            List<Product> products = _context.Products.ToList();

            List<ProductSizePriceViewModel> productlist = new List<ProductSizePriceViewModel>();

            ProductSizePriceViewModel product = new ProductSizePriceViewModel();

            var productList = (from ProSP in _context.ProductSizePrice
                               join Pro in _context.Products on ProSP.ProductId equals Pro.ProductId
                               select new ProductSizePriceViewModel
                               {
                                   SizePriceId = ProSP.Id,
                                   ProductId = Pro.ProductId,
                                   Name = Pro.Name,
                                   CategoryId = Pro.CategoryId,
                                   Description = Pro.Description,
                                   Pic = Pro.Pic,
                                   Size = ProSP.Size,
                                   Price = ProSP.Price
                               });

            var cat = (from categories in _context.Categories
                       select new Category
                       {
                           CategoryId = categories.CategoryId,
                           CategoryName = categories.CategoryName,
                       });
            Homedata.Category = cat;
            Homedata.Product = productList;
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