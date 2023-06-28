using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Models;

namespace TablesideOrdering.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
        }

        //Databse for Admin
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductSize> ProductSize { get; set; }
        public DbSet<ProductSizePrice> ProductSizePrice { get; set; }

        //Database for Customer
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

    }
}
