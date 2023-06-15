using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Data;
using TablesideOrdering.ViewModels;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
    private readonly ApplicationDbContext context;
    public INotyfService notyfService { get; }

    public CategoriesController(ApplicationDbContext _context, INotyfService _notyfService)
    {
        context = _context;
        notyfService = _notyfService;
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        var categories = await context.Categories.ToListAsync();
        return View(categories);
    }

    // GET: Categories/Create
    public IActionResult Create()
    {
        Category Category = new Category();
        return PartialView("Create", Category);
    }

    // POST: Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            context.Add(category);
            await context.SaveChangesAsync();

            notyfService.Success("New category has been created");
            return RedirectToAction(nameof(Index));
        }
        notyfService.Error("Something went wrong, please try again!");
        return View(category);
    }

    // GET: Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || context.Categories == null)
        {
            return NotFound();
        }
        var category = await context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return PartialView("Edit", category);
    }

    // POST: Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            try
            {
                context.Update(category);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            notyfService.Information("The category info has been updated", 5);
            return RedirectToAction(nameof(Index));
        }
        notyfService.Error("Something went wrong, please try again!");
        return View(category);
    }

    // GET: Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || context.Categories == null)
        {
            return NotFound();
        }

        var category = await context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
        if (category == null)
        {
            return NotFound();
        }

        return PartialView("Delete", category);
    }

    // POST: Categories/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Category category)
    {
        if (category != null)
        {
            context.Categories.Remove(category);
        }
        await context.SaveChangesAsync();
        notyfService.Success("The category is deleted", 5);
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return (context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
    }
}
}
