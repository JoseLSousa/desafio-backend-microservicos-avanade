using Microsoft.EntityFrameworkCore;
using Stock.Domain.Common;
using Stock.Domain.Interfaces;
using Stock.Infra.Data;

namespace Stock.Infra.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext Context;
        public BaseRepository(AppDbContext context)
        {
            Context = context;
        }
        public void Create(T entity)
        {
            Context.Add(entity);
        }

        public void Delete(T entity)
        {
            Context.Remove(entity);
        }

        public async Task<List<T>> GetAll(CancellationToken cancellationToken)
        {
            return await Context.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public void Update(T entity)
        {
            Context.Update(entity);
        }
    }
}
