using System.ComponentModel.DataAnnotations;
using TablesideOrdering.Models.Admin;
using TablesideOrdering.Models.Customer;
using TablesideOrdering.ViewModels.Admin;

namespace TablesideOrdering.ViewModels.Customer
{
    public class HomeViewModel
    {
        public IQueryable<Category> Category { get; set; }
        public IQueryable<ProductSizePriceViewModel> Product { get; set; }
        public CartList Cart { get; set; }
        public PhoneValidation PhoneValid { get; set; }
    }
}
