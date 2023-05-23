using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Data;
using TablesideOrdering.Models;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductSizePricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductSizePricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductSizePrices
        public async Task<IActionResult> Index()
        {
              return _context.ProductSizePrice != null ? 
                          View(await _context.ProductSizePrice.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ProductSizePrice'  is null.");
        }

        // GET: Admin/ProductSizePrices/Create
        public IActionResult Create()
        {
            ViewBag.ProductList = ProductList();
            ViewBag.SizeList = SizeList();
            return View();
        }

        // POST: Admin/ProductSizePrices/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( ProductSizePrice productSizePrice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productSizePrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productSizePrice);
        }

        // GET: Admin/ProductSizePrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.ProductList = ProductList();
            ViewBag.SizeList = SizeList();
            if (id == null || _context.ProductSizePrice == null)
            {
                return NotFound();
            }

            var productSizePrice = await _context.ProductSizePrice.FindAsync(id);
            if (productSizePrice == null)
            {
                return NotFound();
            }
            return View(productSizePrice);
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
                try
                {
                    _context.Update(productSizePrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSizePriceExists(productSizePrice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
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

            return View(productSizePrice);
        }

        // POST: Admin/ProductSizePrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductSizePrice == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductSizePrice'  is null.");
            }
            var productSizePrice = await _context.ProductSizePrice.FindAsync(id);
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

        private bool ProductSizePriceExists(int id)
        {
          return (_context.ProductSizePrice?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
