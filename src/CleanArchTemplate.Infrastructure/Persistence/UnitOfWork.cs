using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task RollbackAsync()
        {
            // Entity Framework Core doesn't support explicit rollback outside of transactions.
            // For more advanced scenarios, use transactions:
            // await _context.Database.RollbackTransactionAsync();
            return Task.CompletedTask;
        }
    }
}
