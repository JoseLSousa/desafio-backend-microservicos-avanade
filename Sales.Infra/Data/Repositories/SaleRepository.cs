using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using Sales.Infra.Data.Context;

namespace Sales.Infra.Data.Repositories
{
    public class SaleRepository : BaseRepository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext context) : base(context)
        {
        }
    }
}
