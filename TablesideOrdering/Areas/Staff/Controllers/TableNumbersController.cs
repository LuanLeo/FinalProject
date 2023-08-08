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
    [Area("Staff")]
    public class TableNumbersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TableNumbersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff/TableNumbers
        public async Task<IActionResult> Index()
        {
              return _context.TableNumbers != null ? 
                          View(await _context.TableNumbers.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TableNumbers'  is null.");
        }

        // GET: Staff/TableNumbers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TableNumbers == null)
            {
                return NotFound();
            }

            var tableNumber = await _context.TableNumbers
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTable")] TableNumber tableNumber)
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
            if (id == null || _context.TableNumbers == null)
            {
                return NotFound();
            }

            var tableNumber = await _context.TableNumbers.FindAsync(id);
            if (tableNumber == null)
            {
                return NotFound();
            }
            return View(tableNumber);
        }

        // POST: Staff/TableNumbers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdTable")] TableNumber tableNumber)
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
            if (id == null || _context.TableNumbers == null)
            {
                return NotFound();
            }

            var tableNumber = await _context.TableNumbers
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
            if (_context.TableNumbers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TableNumbers'  is null.");
            }
            var tableNumber = await _context.TableNumbers.FindAsync(id);
            if (tableNumber != null)
            {
                _context.TableNumbers.Remove(tableNumber);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableNumberExists(string id)
        {
          return (_context.TableNumbers?.Any(e => e.IdTable == id)).GetValueOrDefault();
        }
    }
}
