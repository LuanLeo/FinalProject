using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Areas.Admin.Models;
using TablesideOrdering.Areas.Staff.Models;
using TablesideOrdering.Areas.StoreOwner.Models;
using TablesideOrdering.Models;

namespace TablesideOrdering.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
        }

        //Database for Admin
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        //Database for Store Owner
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductSize> ProductSize { get; set; }
        public DbSet<ProductSizePrice> ProductSizePrice { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<EmailPR> EmailPRs { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<VirtualCart> VirtualCarts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

        //Database for Staff
        public DbSet<Reservation> Reservations { get; set; }
    }
}
