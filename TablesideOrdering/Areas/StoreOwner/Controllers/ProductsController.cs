using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Areas.StoreOwner.ViewModels;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;

namespace TablesideOrdering.Areas.StoreOwner.Controllers
{
    [Area("StoreOwner")]

    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public INotyfService notyfService { get; }

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, INotyfService _notyfService)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
            notyfService = _notyfService;
        }

        // GET: Products
        public async Task<IActionResult> Index(int id)
        {
            
            ViewBag.CategoryList = CategoryList();
            var products= (from p in _context.Products
                           select new ProductViewModel
                           {
                               ProductId= p.ProductId,
                               CategoryId= p.CategoryId,
                               Description= p.Description,
                               Pic= p.Pic,
                               Name= p.Name,
                           });
      
            return View(products);
        }
        // GET: Products/Create
        public IActionResult Create()
        {
            ProductViewModel ViewModel = new ProductViewModel();
            return PartialView("Create", ViewModel);
        }

        // POST: Products/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            Product product = new Product();
            product.Name = model.Name;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;

            if (model.PicFile == null)
            {
                ViewBag.NullFile = "Please upload an image!";
                return View();
            }

            if (model.PicFile != null)
            {
                product.Pic = UploadFile(model.PicFile);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                notyfService.Success("New category has been created");
                return RedirectToAction(nameof(Index));
            }
            notyfService.Error("Something went wrong, please try again!");
            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = GetProductByID(id);
            ProductViewModel model = new()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                ExistingImage = product.Pic,
            };
            return PartialView("Edit", model);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            Product product = _context.Products.FirstOrDefault(x => x.ProductId == model.ProductId);
            product.CategoryId = model.CategoryId;
            product.Description = model.Description;
            product.Name = model.Name;

            if (model.PicFile != null)
            {
                if (product.Pic != null)
                {
                    string ExitingFile = Path.Combine(_hostEnvironment.WebRootPath, "ProductImage", product.Pic);
                    System.IO.File.Delete(ExitingFile);
                }
                product.Pic = UploadFile(model.PicFile);
            }

            _context.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {

            var product = GetProductByID(id);
            ProductViewModel model = new()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                ExistingImage = product.Pic,
            };

            return PartialView("Delete", model);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductViewModel model)
        {
            Product pro = GetProductByID(model.ProductId);
            if (pro.Pic != null)
            {
                string ExistingFile = Path.Combine(_hostEnvironment.WebRootPath, "ProductImage", pro.Pic);
                System.IO.File.Delete(ExistingFile);
            }
            if (pro != null)
            {
                _context.Products.Remove(pro);
            }

            await _context.SaveChangesAsync();
            notyfService.Success("The category is deleted", 5);
            return RedirectToAction(nameof(Index));
        }

        public Product GetProductByID(int id)
        {
            return _context.Products.FirstOrDefault(x => x.ProductId == id);
        }

        public string GetProductImage(int id)
        {
            var pro = _context.Products.FirstOrDefault(x => x.ProductId == id);
            return pro.Pic;
        }

        private string UploadFile(IFormFile formFile)
        {
            string UniqueFileName = formFile.FileName;
            string TargetPath = Path.Combine(_hostEnvironment.WebRootPath, "ProductImage", UniqueFileName);
            using (var stream = new FileStream(TargetPath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
            return UniqueFileName;
        }

        public List<SelectListItem> CategoryList()
        {
            var list = new List<SelectListItem>();
            foreach (var cate in _context.Categories)
            {
                list.Add(new SelectListItem() { Value = cate.CategoryId.ToString(), Text = cate.CategoryName });
            }
            return list;
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
