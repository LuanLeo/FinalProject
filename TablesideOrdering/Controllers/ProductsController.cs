using System;
using System.Collections.Generic;
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
using TablesideOrdering.Models;
using TablesideOrdering.ViewModels;

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
                               Price = products.Price,
                               Description = products.Description,
                               Name = products.Name,
                               Pic = products.Pic,
                               CategoryId = products.CategoryId,
                           });
            prodata.Product = product;
            return View(prodata);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.CategoryList = CategoryList();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model, Product product)
        {
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
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
                return RedirectToAction("Index");

            }

            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.CategoryList = CategoryList();
            var product = GetProductByID(id);
            ProductViewModel model = new()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Price = product.Price,
                ExistingImage = product.Pic,
            };
            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {

            //var products = model.Product.FirstOrDefault(x=>x.ProductId==id);
            Product product = _context.Products.FirstOrDefault(x => x.ProductId == id);
            product.Price = model.Price;
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


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product pro = GetProductByID(id);
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
                list.Add(new SelectListItem() { Value = cate.CategoryId.ToString(), Text = cate.CategoryName});
            }
            return list;
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
