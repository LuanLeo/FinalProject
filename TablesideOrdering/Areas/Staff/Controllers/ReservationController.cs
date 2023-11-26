using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class ReservationController : Controller
    {
         private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }

        public ReservationController(INotyfService notyfService, ApplicationDbContext context)
        {
            _context = context;
            _notyfService = notyfService; 
        }
        public IActionResult Index(string term = "")
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            if (term != ""){
                var res = _context.Reservations.Where(m => m.PhoneNumber.Contains(term)).ToList();
                return View(res);
            }
            else { 
                var res = _context.Reservations.ToList();
                return View(res);
            }
        }
        public async Task<IActionResult> Details(string id)
        {
            var res = await _context.Reservations.FirstOrDefaultAsync(m => m.Id.ToString() == id);
            return PartialView("Details", res);
        }
        public async Task<IActionResult> Delete(string id)
        {
            //Check id and database exist
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            //Take component in database
            var res = await _context.Reservations.FirstOrDefaultAsync(m => m.Id.ToString() == id);

            //Check if database has value
            if (res == null)
            {
                return NotFound();
            }

            return PartialView("Delete", res);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Reservation res)
        {
            if (res != null)
            {
                _context.Reservations.Remove(res);
            }
            await _context.SaveChangesAsync();
            _notyfService.Success("The reservation is deleted!", 5);
            return RedirectToAction(nameof(Index));
        }
    }
}


