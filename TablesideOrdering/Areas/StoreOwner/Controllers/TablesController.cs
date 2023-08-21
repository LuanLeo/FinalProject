using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Data;
using Twilio.Rest.Preview.Marketplace.AvailableAddOn;

namespace TablesideOrdering.Areas.StoreOwner.Controllers
{
    [Area("StoreOwner")]
    [Authorize(Roles = "Store Owner, Admin")]

    public class TablesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }

        public TablesController(ApplicationDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: StoreOwner/Tables
        public async Task<IActionResult> Index()
        {
            ViewBag.TableStatus = TableStatus();
            var TableList = await _context.Tables.ToListAsync();
            return View(TableList);
        }

        // GET: StoreOwner/Tables/Create
        public IActionResult Create()
        {
            Table model = new Table();
            return PartialView("Create", model);
        }

        // POST: StoreOwner/Tables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Table table)
        {
            var existTable = _context.Tables.Find(table.IdTable);
            if (existTable == null)
            {
                _context.Add(table);
                await _context.SaveChangesAsync();
                _notyfService.Success("Table is created successfully", 5);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Table has already existed ", 5);
            return View(table);
        }

        // GET: StoreOwner/Tables/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FindAsync(id);
            Table model = new Table()
            {
               IdTable = table.IdTable,
               Status = table.Status,
               PeopleCap = table.PeopleCap,
            };
            return PartialView("Edit", model);
        }

        // POST: StoreOwner/Tables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Update(table);
                _context.SaveChanges();

                _notyfService.Success("The info is edited succeesfully", 5);
                return RedirectToAction("Index");
            }

            _notyfService.Error("Something went wrong, try again!", 5);
            return View(table);
        }

        // GET: StoreOwner/Tables/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.ProductSizePrice == null)
            {
                return NotFound();
            }

            var model = await _context.Tables.FirstOrDefaultAsync(m => m.IdTable == id);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("Delete", model);
        }

        // POST: StoreOwner/Tables/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Table model)
        {
            var table = _context.Tables.Find(model.IdTable);
                if (table.Status == "Available")
                {
                    _context.Tables.Remove(table);
                    _context.SaveChanges();

                    _notyfService.Success("Table is deleted successfully", 5);
                    return RedirectToAction("Index");
                }
                _notyfService.Warning("Table is being used", 5);
                return View("Index");
        }

        public List<SelectListItem> TableStatus()
        {
            var list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "Available", Text = "Available" });
                list.Add(new SelectListItem() { Value = "Busy", Text = "Busy" });
            return list;
        }
    }
}
