using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Data;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.Identity.Pages.Account;
using TablesideOrdering.Data;
using TablesideOrdering.Models.Admin;
using TablesideOrdering.ViewModels.Admin;

namespace TablesideOrdering.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;


        public INotyfService notyfService { get; }

        public string RoleName;

        public UsersController(ApplicationDbContext _context, UserManager<IdentityUser> _userManager,
            RoleManager<IdentityRole> _roleManager, INotyfService _notyfService, IUserStore<IdentityUser> userStore,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
            notyfService = _notyfService;
            _userStore = userStore;
            _logger = logger;
        }
        public IActionResult Index()
        {
            DropDownList();
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
            IQueryable userData = users;
            return View(userData);
        }

        [HttpGet]
        public IActionResult Create()
        {
            UsersViewModel user = new UsersViewModel();
            DropDownList();
            return PartialView("Create", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsersViewModel model)
        {
            var user = CreateUser();
            user.Firstname = model.Register.Firstname;
            user.Lastname = model.Register.Lastname;
            user.UserName = model.Register.Email;
            user.Email = model.Register.Email;

            var result = await userManager.CreateAsync(user, model.Register.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, model.Register.Role);
                return RedirectToAction("Index");
            }
            notyfService.Error("Something went wrong, please try again", 5);
            return RedirectToAction("Index");
        }

        private ApplicationUser CreateUser()
        {
            return Activator.CreateInstance<ApplicationUser>();
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
            return PartialView("Edit", viewUser);
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

            if (model.Roles == "--Please select role--")
            {
                return RedirectToAction("Index");
            }

            user.Firstname = model.FirstName;
            user.Lastname = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            await userManager.UpdateAsync(user);

            var users = await userManager.FindByIdAsync(model.UserID);
            var role = await roleManager.FindByNameAsync(model.Roles);
            var oldrole = await userManager.GetRolesAsync(users);
            var newrole = await roleManager.GetRoleNameAsync(role);

            await userManager.RemoveFromRolesAsync(user, oldrole);
            await userManager.AddToRoleAsync(user, newrole);

            notyfService.Information("The user info has been updated", 5);
            return RedirectToAction("Index");
        }

        public async Task DropDownList()
        {
            var model = new UsersViewModel();
            model.inputModel = new RegisterModel.InputModel()
            {
                RoleList = roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            ViewBag.RolesList = model.inputModel.RoleList;
        }
    }
}
