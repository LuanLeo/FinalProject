using Microsoft.AspNetCore.Identity;
using TablesideOrdering.Models.Admin;

namespace TablesideOrdering.Areas.Admin.Models
{
    public class UsersViewModel
    {
        public List<ApplicationUser> UserList { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string RoleId { get; set; }       
        public IdentityUser User { get; set; }
        public IQueryable<UsersViewModel> Users { get; set; }        
    }
}
