using Microsoft.EntityFrameworkCore;

namespace DbLibrary.ModelsDb
{
    public class LuxDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }


        public LuxDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;" +
                                     "Port=5433;" +
                                     "Database=LuxDatabase;" +
                                     "Username=postgres;" +
                                     "Password=super200;");
        }
    }
}