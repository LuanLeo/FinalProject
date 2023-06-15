using System.ComponentModel.DataAnnotations;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.Admin.ViewModels;
using TablesideOrdering.Models;

namespace TablesideOrdering.ViewModels
{
    public class HomeViewModel
    {
        public IQueryable<Category> Category { get; set; }
        public IQueryable<ProductSizePriceViewModel> Product { get; set; }
        public CartList Cart { get; set; }
        public PhoneValidation PhoneValid { get; set; }
    }
}
