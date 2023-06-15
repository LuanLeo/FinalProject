using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TablesideOrdering.Areas.Identity.Pages.Account;

namespace TablesideOrdering.Areas.Admin.Models
{
    public class UsersViewModel
    {
        public RegisterModel.InputModel Register { get; set; }

        [BindProperty]
        public RegisterModel.InputModel inputModel { get; set; }
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
    }
}