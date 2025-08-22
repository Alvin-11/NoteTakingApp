using Microsoft.EntityFrameworkCore;

namespace BlazorApp2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Product> Produc { get; set; }
    }
}