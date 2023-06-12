using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Data;
using TablesideOrdering.Models.Admin;
using TablesideOrdering.ViewModels.Admin;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        public readonly ApplicationDbContext context;
        public readonly UserManager<IdentityUser> userManager;
        public readonly RoleManager<IdentityRole> roleManager;
        public string RoleName;
        public UsersController(ApplicationDbContext _context, UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public IActionResult Index()
        {
            var userData = new UsersViewModel();
            var users = (from user in context.ApplicationUsers                         
                         join ur in context.UserRoles on user.Id equals ur.UserId
                         join r in context.Roles on ur.RoleId equals r.Id                       
                         select new UsersViewModel
                         {
                             UserID = user.Id,
                             FirstName = user.Firstname,
                             LastName = user.Lastname,
                             Email = user.Email,
                             Roles = r.Name,
                         });
            userData.Users = users;
            return View(users);
        }
        [HttpGet]
        public IActionResult Create()
        {
            IdentityUser user = new IdentityUser();
            return PartialView("Create", user);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || context.ApplicationUsers == null)
            {
                return NotFound();
            }
            var viewUser = (from user in context.ApplicationUsers
                            join ur in context.UserRoles on user.Id equals ur.UserId
                            join r in context.Roles on ur.RoleId equals r.Id
                            where user.Id == id
                            select new UsersViewModel
                            {
                                UserID = id,
                                FirstName = user.Firstname,
                                LastName = user.Lastname,
                                Email = user.Email,
                                Roles = r.Name,
                            });
            return PartialView("Delete", viewUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(UsersViewModel model)
        {
            var user = await context.ApplicationUsers.FindAsync(model.UserID);
            if (user == null)
            {
                return NotFound();
            }
            context.ApplicationUsers.Remove(user);
            context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
