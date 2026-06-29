namespace Wallet.Domain.Repositories;

public interface IRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    void Add(TEntity entity);

    void Remove(TEntity entity);
    
    Task<int> SaveChangesAsync();
    
    Task<TEntity?> GetSingleOrDefaultAsync(uint id);
}