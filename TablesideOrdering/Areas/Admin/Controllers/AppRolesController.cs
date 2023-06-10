using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Abstractions;
using TablesideOrdering.Data;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AppRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public INotyfService _notyfService { get; }
        public AppRolesController(RoleManager<IdentityRole> roleManager, INotyfService notyfService, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _notyfService = notyfService;
            _context = context;

        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            IdentityRole role = new IdentityRole();
            return PartialView("Create", role);
        }
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            //avoid duplicate role
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
                _notyfService.Success("New role has been created");
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Something went wrong, please try again!");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            //Check id and database exist
            if (id == null || _roleManager.Roles == null)
            {
                return NotFound();
            }

            //Take component in database
            var role = await _roleManager.Roles.FirstOrDefaultAsync(m => m.Id == id);

            //Check if database has value
            if (role == null)
            {
                return NotFound();
            }

            return PartialView("Delete", role);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usedrole = await _roleManager.Roles.FirstOrDefaultAsync(m => m.Id == id);
            var users = (from user in _context.ApplicationUsers
                         join ur in _context.UserRoles on user.Id equals ur.UserId
                         where ur.RoleId == id
                         select user).Count();
            if (users == 0)
            {
                //Check if database exists
                if (_roleManager.Roles == null)
                {
                    return Problem("Entity set 'RoleManager<IdentityRole>.Roles'  is null.");
                }

                //Create variable stores details of component need delete
                IdentityRole roles = new IdentityRole();

                //Find value need to be deleted compares to id 
                foreach (var role in _roleManager.Roles)
                {
                    if (id == role.Id)
                    {
                        roles = role;
                    }
                }

                //Delete roles
                await _roleManager.DeleteAsync(roles);
                return RedirectToAction("Index");
            }
            else
            {
                _notyfService.Warning("This role is already in use");
            }
            return RedirectToAction("Index");
        }
    }
}
