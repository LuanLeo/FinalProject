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

namespace TablesideOrdering.Areas.StoreOwner.Controllers
{
    [Area("StoreOwner")]
    public class DiscountsController : Controller
    {
        private readonly ApplicationDbContext context;
        public INotyfService notyfService { get; }

        public DiscountsController(ApplicationDbContext _context, INotyfService _notyfService)
        {
            context = _context;
            notyfService = _notyfService;
        }
        // GET: StoreOwner/Discounts
        public IActionResult Index()
        {
            var discount = context.Discounts.ToList();
            return View(discount);
        }

        // GET: StoreOwner/Discounts/Create
        public IActionResult Create()
        {
            Discount discount = new Discount();
            return PartialView("Create", discount);
        }

        // POST: StoreOwner/Discounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount discount)
        {
            if (ModelState.IsValid)
            {
                context.Add(discount);
                await context.SaveChangesAsync();

                notyfService.Success("New discount has been created");
                return RedirectToAction(nameof(Index));
            }
            notyfService.Error("Something went wrong, try again!");
            return View(discount);
        }

        // GET: StoreOwner/Discounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Discounts == null)
            {
                return NotFound();
            }
            var discount = await context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return PartialView("Edit", discount);
        }

        // POST: StoreOwner/Discounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Discount discount)
        {
            if (ModelState.IsValid)
            {
                context.Update(discount);
                await context.SaveChangesAsync();

                notyfService.Information("The info has been updated", 5);
                return RedirectToAction(nameof(Index));
            }
            notyfService.Error("Something went wrong, try again!");
            return View(discount);
        }


        // GET: StoreOwner/Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Discounts == null)
            {
                return NotFound();
            }

            var discount = await context.Discounts.FirstOrDefaultAsync(m => m.Id == id);
            if (discount == null)
            {
                return NotFound();
            }

            return PartialView("Delete", discount);
        }

        // POST: StoreOwner/Discounts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Discount discount)
        {
            if (discount != null)
            {
                context.Discounts.Remove(discount);
            }
            await context.SaveChangesAsync();
            notyfService.Success("The discount is deleted", 5);
            return RedirectToAction(nameof(Index));
        }
    }
}