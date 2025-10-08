using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Infrastructure.Persistence;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _dbSet.FindAsync(id, cancellationToken).ConfigureAwait(false);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        => await _dbSet.ToListAsync(cancellationToken).ConfigureAwait(false);

    public virtual async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        return await _dbSet
            .OrderBy(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
        entity.SetUpdated();
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        => await _dbSet.FindAsync(id, cancellationToken).ConfigureAwait(false) != null;

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken)
        => await _dbSet.CountAsync(cancellationToken).ConfigureAwait(false);
}
