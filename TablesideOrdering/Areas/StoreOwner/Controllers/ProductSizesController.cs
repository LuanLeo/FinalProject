using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using TablesideOrdering.Migrations;

namespace TablesideOrdering.Areas.StoreOwner.Controllers
{
    [Area("StoreOwner")]
    public class ProductSizesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService notyfService { get; }

        public ProductSizesController(ApplicationDbContext context, INotyfService _notyfService)
        {
            _context = context;
            notyfService = _notyfService;
        }

        // GET: Admin/ProductSizes
        public async Task<IActionResult> Index()
        {
            var size = await _context.ProductSize.ToListAsync();
            return View(size);
        }

        // GET: Admin/ProductSizes/Create
        public IActionResult Create()
        {
            ProductSize size = new ProductSize();
            return PartialView("Create", size);
        }

        // POST: Admin/ProductSizes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productSize);
                await _context.SaveChangesAsync();

                notyfService.Success("New size has been created");
                return RedirectToAction(nameof(Index));
            }
            notyfService.Error("Something went wrong, please try again!");
            return View(productSize);
        }

        // GET: Admin/ProductSizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductSize == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSize.FindAsync(id);
            if (productSize == null)
            {
                return NotFound();
            }
            return PartialView("Edit",productSize);
        }

        // POST: Admin/ProductSizes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productSize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSizeExists(productSize.SizeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                notyfService.Information("The size info has been updated", 5);
                return RedirectToAction(nameof(Index));
            }
            notyfService.Error("Something went wrong, please try again!");
            return View(productSize);
        }

        // GET: Admin/ProductSizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductSize == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSize.FirstOrDefaultAsync(m => m.SizeId == id);
            if (productSize == null)
            {
                return NotFound();
            }

            return PartialView("Delete",productSize);
        }

        // POST: Admin/ProductSizes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductSize productSize)
        {
            if (productSize != null)
            {
                _context.ProductSize.Remove(productSize);
            }
            await _context.SaveChangesAsync();
            notyfService.Success("The size is deleted", 5);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSizeExists(int id)
        {
          return (_context.ProductSize?.Any(e => e.SizeId == id)).GetValueOrDefault();
        }
    }
}
