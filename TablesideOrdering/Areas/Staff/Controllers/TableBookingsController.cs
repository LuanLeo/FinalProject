using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("StoreOwner")]
    public class TableBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TableBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StoreOwner/TableBookings
        public async Task<IActionResult> Index()
        {
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
            return View();
        }

        // POST: StoreOwner/TableBookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Table,BookDate,CusName,PhoneNumber")] TableBooking tableBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            return View(tableBooking);
        }

        // POST: StoreOwner/TableBookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Table,BookDate,CusName,PhoneNumber")] TableBooking tableBooking)
        {
            if (id != tableBooking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(tableBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tableBooking);
        }

        // GET: StoreOwner/TableBookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: StoreOwner/TableBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tableBooking = await _context.TableBookings.FindAsync(id);
            if (tableBooking != null)
            {
                _context.TableBookings.Remove(tableBooking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public List<SelectListItem> TableNameCombine()
        {
            var list = new List<SelectListItem>();
            foreach (var item in _context.Tables)
            {
                string name = item.IdTable + " + " + item.PeopleCap + " People";
                list.Add(new SelectListItem() { Value = name, Text = name });
            }
            return list;
        }
    }
}
