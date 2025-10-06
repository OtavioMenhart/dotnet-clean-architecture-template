using CleanArchTemplate.Domain.Events;
using RabbitMq.Messaging.Consumer;
using RabbitMQ.Client;
using System.Text.Json;

namespace CleanArchTemplate.Workers.Consumers;

public class EntityEventConsumer : BaseConsumer<EntityEvent>
{
    public EntityEventConsumer(
        IConfiguration configuration,
        IConnection connection,
        ILogger<EntityEventConsumer> logger)
        : base(
              configuration,
              connection,
              logger,
              new[] {
                  new ExchangeBinding("entity-events-exchange", ExchangeType.Fanout, "")
              },
              "entity-events-queue")
    {
        //_parallelWorkerCount = 5; // The default is 5
        //_prefetchCount = 0; // The default is 0 (unlimited)
        // You can configure other properties here if needed via appsettings.json -> check https://github.com/OtavioMenhart/RabbitMq.Messaging.Toolkit
    }

    protected override async Task HandleMessageAsync(byte[] messageBody, IReadOnlyBasicProperties properties, CancellationToken cancellationToken)
    {
        var entityEvent = JsonSerializer.Deserialize<EntityEvent>(messageBody);
        _logger.LogInformation("Event received: {EventType} for {EntityName} with ID {EntityId} at {Timestamp}", entityEvent.EventType, entityEvent.EntityName, entityEvent.EntityId, entityEvent.Timestamp);

        // Here you can add logic to process the event, e.g., update a read model, send notifications, etc.
        // For start retry/dead-letter logic you can just throw an exception here
    }
}
