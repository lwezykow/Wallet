using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;
using Wallet.Domain.Repositories;

namespace Wallet.Data.Repositories;

public class GenericRepository<TEntity>(WalletDbContext context)
    : IRepository<TEntity> where TEntity : class, IEntity
{
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await context.Set<TEntity>().ToListAsync();
    }

    public void Add(TEntity entity)
    {
        context.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        context.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public async Task<TEntity> GetSingleAsync(uint id)
    {
        return await context.Set<TEntity>().SingleAsync(rate => rate.Id == id);
    }

    public async Task<TEntity?> GetSingleOrDefaultAsync(uint id)
    {
        return await context.Set<TEntity>().SingleOrDefaultAsync(rate => rate.Id == id);
    }
}