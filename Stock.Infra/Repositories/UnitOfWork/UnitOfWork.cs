using Stock.Domain.Interfaces;
using Stock.Infra.Data;

namespace Stock.Infra.Repositories.UnitOfWork
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync();
        }
    }
}
