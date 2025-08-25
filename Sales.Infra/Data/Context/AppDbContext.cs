using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Infra.Data.Configurations;

namespace Sales.Infra.Data.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SaleConfiguration());
            modelBuilder.ApplyConfiguration(new SaleItemConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
