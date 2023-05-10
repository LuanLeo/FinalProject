﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TablesideOrdering.Models;

namespace TablesideOrdering.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}