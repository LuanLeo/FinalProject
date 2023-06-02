using System.ComponentModel.DataAnnotations;
using TablesideOrdering.Models;

namespace TablesideOrdering.ViewModels
{
    public class HomeViewModel
    {
        public IQueryable<Category> Category { get; set; }
        public IQueryable<ProductSizePriceViewModel> Product { get; set; }
        public CartList Cart { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [MaxLength(10, ErrorMessage = "Length must be 10")]
        public string PhoneNumber { get; set; }
    }
}
