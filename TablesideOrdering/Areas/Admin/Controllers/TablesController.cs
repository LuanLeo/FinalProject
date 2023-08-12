using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Staff")]
    public class TablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff/TableNumbers
        public async Task<IActionResult> Index()
        {
            return _context.Tables != null ?
                        View(await _context.Tables.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.TableNumbers'  is null.");
        }

        // GET: Staff/TableNumbers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var tableNumber = await _context.Tables
                .FirstOrDefaultAsync(m => m.IdTable == id);
            if (tableNumber == null)
            {
                return NotFound();
            }

            return View(tableNumber);
        }

        // GET: Staff/TableNumbers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staff/TableNumbers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTable")] Table tableNumber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tableNumber);
        }

        // GET: Staff/TableNumbers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var tableNumber = await _context.Tables.FindAsync(id);
            if (tableNumber == null)
            {
                return NotFound();
            }
            return View(tableNumber);
        }

        // POST: Staff/TableNumbers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdTable")] Table tableNumber)
        {
            if (id != tableNumber.IdTable)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableNumber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableNumberExists(tableNumber.IdTable))
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
            return View(tableNumber);
        }

        // GET: Staff/TableNumbers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var tableNumber = await _context.Tables
                .FirstOrDefaultAsync(m => m.IdTable == id);
            if (tableNumber == null)
            {
                return NotFound();
            }

            return View(tableNumber);
        }

        // POST: Staff/TableNumbers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Tables == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TableNumbers'  is null.");
            }
            var tableNumber = await _context.Tables.FindAsync(id);
            if (tableNumber != null)
            {
                _context.Tables.Remove(tableNumber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableNumberExists(string id)
        {
            return (_context.Tables?.Any(e => e.IdTable == id)).GetValueOrDefault();
        }
    }
}
