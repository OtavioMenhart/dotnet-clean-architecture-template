using System.Diagnostics.CodeAnalysis;

namespace CleanArchTemplate.Domain.Events;

[ExcludeFromCodeCoverage]
public class EntityEvent
{
    public string EventType { get; }
    public string EntityName { get; }
    public Guid EntityId { get; }
    public DateTime Timestamp { get; }
    public object Data { get; }

    public EntityEvent(string eventType, string entityName, Guid entityId, object data)
    {
        EventType = eventType;
        EntityName = entityName;
        EntityId = entityId;
        Timestamp = DateTime.UtcNow;
        Data = data;
    }
}
