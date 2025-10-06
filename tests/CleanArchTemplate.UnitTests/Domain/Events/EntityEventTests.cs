using CleanArchTemplate.Domain.Events;

namespace CleanArchTemplate.UnitTests.Domain.Events;

public class EntityEventTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var eventType = "Created";
        var entityName = "Product";
        var entityId = Guid.NewGuid();
        var data = new { Name = "Test", Price = 10.0 };

        var evt = new EntityEvent(eventType, entityName, entityId, data);

        Assert.Equal(eventType, evt.EventType);
        Assert.Equal(entityName, evt.EntityName);
        Assert.Equal(entityId, evt.EntityId);
        Assert.Equal(data, evt.Data);
        Assert.True((DateTime.UtcNow - evt.Timestamp).TotalSeconds < 5);
    }

    [Fact]
    public void Constructor_AllowsNullData()
    {
        var evt = new EntityEvent("Updated", "Product", Guid.NewGuid(), null);

        Assert.Null(evt.Data);
    }
}
