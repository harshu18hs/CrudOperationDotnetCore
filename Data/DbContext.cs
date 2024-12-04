using Microsoft.EntityFrameworkCore;
using ReactCRUDAPI.Model;

using System.Collections.Generic;
namespace ReactCRUDAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
