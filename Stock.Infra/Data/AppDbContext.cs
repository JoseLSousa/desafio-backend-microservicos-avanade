using Microsoft.EntityFrameworkCore;
using Stock.Domain.Entitites;

namespace Stock.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<StockItem> StockItems { get; set; }
    }
}
