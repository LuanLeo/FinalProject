using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;
using TablesideOrdering.Models;
using TablesideOrdering.ViewModels;
using Product = TablesideOrdering.Models.Product;

namespace TablesideOrdering.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var prodata = new ProductViewModel();
            var product = (from products in _context.Products
                           select new Product
                           {
                               ProductId = products.ProductId,
                               Description = products.Description,
                               Name = products.Name,
                               Pic = products.Pic,
                               CategoryId = products.CategoryId,
                           });
            prodata.Product = product;
            return View(prodata);
        }
    }
}
