using Microsoft.AspNetCore.Identity;

namespace TablesideOrdering.Areas.Admin.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfilePic { get; set; }
    }
}
