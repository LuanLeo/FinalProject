using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TablesideOrdering.Areas.Identity.Pages.Account;

namespace TablesideOrdering.Models
{
    public class ResetPassModel
    {
        public ResetPasswordModel.InputModel Register { get; set; }

        [BindProperty]
        public ResetPasswordModel.InputModel inputModel { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
    }
}