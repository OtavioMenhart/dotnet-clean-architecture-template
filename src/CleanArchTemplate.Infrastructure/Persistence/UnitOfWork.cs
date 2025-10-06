using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Events;
using CleanArchTemplate.Domain.Repositories;
using CleanArchTemplate.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CleanArchTemplate.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IMessagingService _messagingService;

    public UnitOfWork(AppDbContext context, IMessagingService messagingService)
    {
        _context = context;
        _messagingService = messagingService;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        // Detect changes in tracked entities
        // Here you can send events for created, updated, and deleted entities
        // For simplicity, we'll just send a generic event for each changed entity
        // A nice strategy also is to have Outbox pattern implemented

        var entries = _context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            string eventType = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted => "Deleted",
                _ => "Unknown"
            };

            var eventMessage = new EntityEvent(eventType, entry.Entity.GetType().Name, entry.Entity.Id, entry.Entity);

            await _messagingService.PublishMessage(
                eventMessage,
                exchangeName: "entity-events-exchange",
                exchangeType: ExchangeType.Fanout
            );
        }

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
