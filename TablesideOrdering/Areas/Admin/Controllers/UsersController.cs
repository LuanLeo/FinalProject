using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public INotyfService notyfService { get; }
        public UsersController(ApplicationDbContext _context, UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public IActionResult Index()
        {
            DropDownList();
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
            notyfService.Success("The user is deleted", 5);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            DropDownList();
            if (id == null || context.ApplicationUsers == null)
            {
                return NotFound();
            }
            var User = await context.ApplicationUsers.FindAsync(id);
            var viewUser = (from user in context.ApplicationUsers
                            join ur in context.UserRoles on user.Id equals ur.UserId
                            join r in context.Roles on ur.RoleId equals r.Id
                            where user.Id == id
                            select new UsersViewModel
                            {
                                UserID = id,
                                RoleId = r.Id,
                                FirstName = User.Firstname,
                                LastName = User.Lastname,
                                Email = User.Email,
                            });

            UsersViewModel view = new UsersViewModel();
            foreach (var views in viewUser)
            {
                view.UserID = views.UserID;
                view.RoleId = views.RoleId;
                view.FirstName = views.FirstName;
                view.LastName = views.LastName;
                view.Email = views.Email;
            }
            return PartialView("Edit", view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsersViewModel model)
        {
            DropDownList();
            var user = await context.ApplicationUsers.FindAsync(model.UserID);
            if (user == null)
            {
                return NotFound();
            }
            var userWithSameEmail = await userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != model.UserID)
            {
                ModelState.AddModelError("Email", "This email is already used");
                return View(model);
            }

            if (model.RoleId == "--Please select role--")
            {
                return RedirectToAction("Index");
            }

            user.UserName = model.Email;
            user.Firstname = model.FirstName;
            user.Lastname = model.LastName;
            user.Email = model.Email;
            await userManager.UpdateAsync(user);


            var users = await userManager.FindByIdAsync(model.UserID);
            var role = await roleManager.FindByIdAsync(model.RoleId);
            var oldrole = await userManager.GetRolesAsync(users);
            var newrole = await roleManager.GetRoleNameAsync(role);
            await userManager.RemoveFromRolesAsync(user, oldrole);
            await userManager.AddToRoleAsync(user, newrole);
            return RedirectToAction("Index");
        }
        public void DropDownList()
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            foreach (var rol in context.Roles)
            {
                roles.Add(new SelectListItem { Text = rol.Name, Value = Convert.ToString(rol.Id) });
            }
            ViewBag.RolesList = roles;
        }
    }
}
