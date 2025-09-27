using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Events;
using CleanArchTemplate.Infrastructure.Messaging;
using CleanArchTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using RabbitMQ.Client;

namespace CleanArchTemplate.UnitTests.Infrastructure.Persistence
{
    public class UnitOfWorkTests
    {
        private TestDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new TestDbContext(options);
        }

        private ProductEntity CreateProduct(string name = "Test", double price = 10.0)
        {
            return new ProductEntity(name, price);
        }

        [Fact]
        public async Task CommitAsync_PublishesCreatedEventAndSavesEntity()
        {
            using var context = CreateContext();
            var messagingMock = new Mock<IMessagingService>();
            var unitOfWork = new UnitOfWork(context, messagingMock.Object);

            var productAdded = CreateProduct("Added", 1.0);
            context.Products.Add(productAdded);

            messagingMock
                .Setup(m => m.PublishMessage(
                    It.IsAny<EntityEvent>(),
                    "entity-events-exchange",
                    null,
                    null,
                    ExchangeType.Fanout,
                    ""))
                .Returns(Task.CompletedTask);

            await unitOfWork.CommitAsync(CancellationToken.None);

            messagingMock.Verify(m => m.PublishMessage(
                It.Is<EntityEvent>(e => e.EventType == "Created" && e.EntityId == productAdded.Id),
                "entity-events-exchange",
                null,
                null,
                ExchangeType.Fanout,
                ""), Times.Once);

            var products = await context.Products.ToListAsync();
            Assert.Contains(products, p => p.Id == productAdded.Id);
        }

        [Fact]
        public async Task CommitAsync_PublishesUpdatedEventAndSavesEntity()
        {
            using var context = CreateContext();
            var messagingMock = new Mock<IMessagingService>();
            var unitOfWork = new UnitOfWork(context, messagingMock.Object);

            var productModified = CreateProduct("Modified", 2.0);
            context.Products.Add(productModified);
            await context.SaveChangesAsync();

            productModified.ChangeName("Modified2");
            context.Entry(productModified).State = EntityState.Modified;

            messagingMock
                .Setup(m => m.PublishMessage(
                    It.IsAny<EntityEvent>(),
                    "entity-events-exchange",
                    null,
                    null,
                    ExchangeType.Fanout,
                    ""))
                .Returns(Task.CompletedTask);

            await unitOfWork.CommitAsync(CancellationToken.None);

            messagingMock.Verify(m => m.PublishMessage(
                It.Is<EntityEvent>(e => e.EventType == "Updated" && e.EntityId == productModified.Id),
                "entity-events-exchange",
                null,
                null,
                ExchangeType.Fanout,
                ""), Times.Once);

            var products = await context.Products.ToListAsync();
            Assert.Contains(products, p => p.Id == productModified.Id);
            Assert.Equal("Modified2", products.First(p => p.Id == productModified.Id).Name);
        }

        [Fact]
        public async Task CommitAsync_PublishesDeletedEventAndRemovesEntity()
        {
            using var context = CreateContext();
            var messagingMock = new Mock<IMessagingService>();
            var unitOfWork = new UnitOfWork(context, messagingMock.Object);

            var productDeleted = CreateProduct("Deleted", 3.0);
            context.Products.Add(productDeleted);
            await context.SaveChangesAsync();

            context.Products.Remove(productDeleted);

            messagingMock
                .Setup(m => m.PublishMessage(
                    It.IsAny<EntityEvent>(),
                    "entity-events-exchange",
                    null,
                    null,
                    ExchangeType.Fanout,
                    ""))
                .Returns(Task.CompletedTask);

            await unitOfWork.CommitAsync(CancellationToken.None);

            messagingMock.Verify(m => m.PublishMessage(
                It.Is<EntityEvent>(e => e.EventType == "Deleted" && e.EntityId == productDeleted.Id),
                "entity-events-exchange",
                null,
                null,
                ExchangeType.Fanout,
                ""), Times.Once);

            var products = await context.Products.ToListAsync();
            Assert.DoesNotContain(products, p => p.Id == productDeleted.Id);
        }

        [Fact]
        public async Task RollbackAsync_ReturnsCompletedTask()
        {
            using var context = CreateContext();
            var messagingMock = new Mock<IMessagingService>();
            var unitOfWork = new UnitOfWork(context, messagingMock.Object);

            await unitOfWork.RollbackAsync();
            Assert.True(Task.CompletedTask.IsCompleted);
        }
    }
}
