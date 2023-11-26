using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;

namespace TablesideOrdering.Areas.StoreOwner.Controllers
{
    [Area("StoreOwner")]
    [Authorize(Roles = "Store Owner, Admin")]

    public class ProductSizePricesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService notyfService { get; }

        public ProductSizePricesController(ApplicationDbContext context, INotyfService _notyfService)
        {
            _context = context;
            notyfService = _notyfService;
        }

        // GET: Admin/ProductSizePrices
        public async Task<IActionResult> Index()
        {
            ProductSizeList();
            return _context.ProductSizePrice != null ?
                          View(await _context.ProductSizePrice.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ProductSizePrice'  is null.");
        }

        // GET: Admin/ProductSizePrices/Create
        public IActionResult Create()
        {
            ProductSizePrice sizePrice = new ProductSizePrice();
            return PartialView("Create", sizePrice);
        }

        // POST: Admin/ProductSizePrices/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductSizePrice productSizePrice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productSizePrice);
                await _context.SaveChangesAsync();

                notyfService.Success("New product has been created!");
                return RedirectToAction(nameof(Index));
            }
            notyfService.Error("Something went wrong, try again!");

            return View(productSizePrice);
        }



        // GET: Admin/ProductSizePrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ProductSizeList();

            if (id == null || _context.ProductSizePrice == null)
            {
                return NotFound();
            }

            var productSizePrice = await _context.ProductSizePrice.FindAsync(id);
            if (productSizePrice == null)
            {
                return NotFound();
            }
            return PartialView("Edit", productSizePrice);
        }

        // POST: Admin/ProductSizePrices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductSizePrice productSizePrice)
        {
            if (id != productSizePrice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(productSizePrice);
                await _context.SaveChangesAsync();

                notyfService.Success("New product has been updated!");
                return RedirectToAction(nameof(Index));
            }
            notyfService.Error("Something went wrong, try again!");
            return View(productSizePrice);
        }

        // GET: Admin/ProductSizePrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductSizePrice == null)
            {
                return NotFound();
            }

            var productSizePrice = await _context.ProductSizePrice
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productSizePrice == null)
            {
                return NotFound();
            }

            return PartialView("Delete", productSizePrice);
        }

        // POST: Admin/ProductSizePrices/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductSizePrice productSizePrice)
        {
            if (productSizePrice != null)
            {
                _context.ProductSizePrice.Remove(productSizePrice);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public List<SelectListItem> ProductList()
        {
            var list = new List<SelectListItem>();
            foreach (var pro in _context.Products)
            {
                list.Add(new SelectListItem() { Value = pro.ProductId.ToString(), Text = pro.Name });
            }
            return list;
        }

        public List<SelectListItem> SizeList()
        {
            var list = new List<SelectListItem>();
            foreach (var size in _context.ProductSize)
            {
                list.Add(new SelectListItem() { Value = size.SizeName, Text = size.SizeName });
            }
            return list;
        }

        public void ProductSizeList()
        {
            ViewBag.ProductList = ProductList();
            ViewBag.SizeList = SizeList();
        }

        private bool ProductSizePriceExists(int id)
        {
            return (_context.ProductSizePrice?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
