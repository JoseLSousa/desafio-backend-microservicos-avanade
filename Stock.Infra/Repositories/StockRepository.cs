using Stock.Domain.Entitites;
using Stock.Domain.Interfaces;
using Stock.Infra.Data;

namespace Stock.Infra.Repositories
{
    public class StockRepository : BaseRepository<StockItem>, IStockRepository
    {
        public StockRepository(AppDbContext context) : base(context)
        {
        }
    }
}
