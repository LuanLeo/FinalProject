using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class TableBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; set; }
        public TableBookingsController(ApplicationDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: StoreOwner/TableBookings
        public async Task<IActionResult> Index()
        {
            ViewBag.TableBookingList = TableNameCombine();
            return _context.TableBookings != null ?
                        View(await _context.TableBookings.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.TableBooking'  is null.");
        }

        // GET: StoreOwner/TableBookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TableBookings == null)
            {
                return NotFound();
            }

            var tableBooking = await _context.TableBookings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tableBooking == null)
            {
                return NotFound();
            }

            return View(tableBooking);
        }

        // GET: StoreOwner/TableBookings/Create
        public IActionResult Create()
        {
            TableBooking model = new TableBooking();
            return PartialView("Create", model);
        }

        // POST: StoreOwner/TableBookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TableBooking tableBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableBooking);
                await _context.SaveChangesAsync();

                _notyfService.Success("New size has been created");
                return RedirectToAction("Index");
            }
            _notyfService.Error("Something went wrong, please try again!");
            return View(tableBooking);
        }

        // GET: StoreOwner/TableBookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TableBookings == null)
            {
                return NotFound();
            }

            var tableBooking = await _context.TableBookings.FindAsync(id);
            if (tableBooking == null)
            {
                return NotFound();
            }
            return PartialView("Edit", tableBooking);
        }

        // POST: StoreOwner/TableBookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TableBooking tableBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Update(tableBooking);
                await _context.SaveChangesAsync();

                _notyfService.Information("The size info has been updated", 5);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Something went wrong, please try again!");
            return View(tableBooking);
        }

        // GET: StoreOwner/TableBookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TableBookings == null)
            {
                return NotFound();
            }

            var tableBooking = await _context.TableBookings.FirstOrDefaultAsync(m => m.Id == id);
            if (tableBooking == null)
            {
                return NotFound();
            }

            return PartialView("Delete", tableBooking);
        }

        // POST: StoreOwner/TableBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TableBooking tableBooking)
        {
            _context.TableBookings.Remove(tableBooking);
            await _context.SaveChangesAsync();

            _notyfService.Success("The size is deleted", 5);
            return RedirectToAction(nameof(Index));
        }

        public List<SelectListItem> TableNameCombine()
        {
            var list = new List<SelectListItem>();
            foreach (var item in _context.Tables)
            {
                string name ="Table " + item.IdTable + " - for " + item.PeopleCap + " People";
                list.Add(new SelectListItem() { Value = name, Text = name });
            }
            return list;
        }
    }
}
