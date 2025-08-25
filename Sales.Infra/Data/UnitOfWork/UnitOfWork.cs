using Sales.Domain.Interfaces;
using Sales.Infra.Data.Context;

namespace Sales.Infra.Data.UnitOfWork
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
