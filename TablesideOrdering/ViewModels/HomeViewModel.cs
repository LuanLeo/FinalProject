using TablesideOrdering.Models;

namespace TablesideOrdering.ViewModels
{
    public class HomeViewModel
    {
        public IQueryable<Category> Category { get; set; }
        public IQueryable<Product> Product { get; set; }
    }
}
