using CleanArchTemplate.Domain.Entities;

namespace CleanArchTemplate.UnitTests.Domain.Entities
{
    // Concrete class for testing BaseEntity
    public class TestEntity : BaseEntity
    {
        public TestEntity() : base() { }
    }

    public class BaseEntityTests
    {
        [Fact]
        public void Constructor_InitializesProperties()
        {
            var entity = new TestEntity();

            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.True((DateTime.UtcNow - entity.CreatedAt).TotalSeconds < 5);
            Assert.Null(entity.UpdatedAt);
        }

        [Fact]
        public void SetUpdated_SetsUpdatedAtToCurrentTime()
        {
            var entity = new TestEntity();
            Assert.Null(entity.UpdatedAt);

            Thread.Sleep(10); // Ensure time difference
            entity.SetUpdated();

            Assert.NotNull(entity.UpdatedAt);
            Assert.True((DateTime.UtcNow - entity.UpdatedAt.Value).TotalSeconds < 5);
        }
    }
}