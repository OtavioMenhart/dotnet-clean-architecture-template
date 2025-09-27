using CleanArchTemplate.Domain.Events;
using CleanArchTemplate.Workers.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using System.Text.Json;

namespace CleanArchTemplate.UnitTests.Workers.Consumers
{
    // Testable subclass to expose protected HandleMessageAsync
    public class TestableEntityEventConsumer : EntityEventConsumer
    {
        public TestableEntityEventConsumer(
            IConfiguration configuration,
            IConnection connection,
            ILogger<EntityEventConsumer> logger)
            : base(configuration, connection, logger) { }

        public async Task InvokeHandleMessageAsync(byte[] messageBody, IReadOnlyBasicProperties properties, CancellationToken cancellationToken)
        {
            await base.HandleMessageAsync(messageBody, properties, cancellationToken);
        }
    }

    public class EntityEventConsumerTests
    {
        [Fact]
        public async Task HandleMessageAsync_DeserializesEventAndLogsInformation()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["RabbitMq:MaxRetryAttempts"]).Returns("3");
            configMock.Setup(c => c["RabbitMq:RetryTTlMilliseconds"]).Returns("30000");
            configMock.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            var connectionMock = new Mock<IConnection>();
            var loggerMock = new Mock<ILogger<EntityEventConsumer>>();

            var consumer = new TestableEntityEventConsumer(
                configMock.Object,
                connectionMock.Object,
                loggerMock.Object);

            var entityEvent = new EntityEvent(
                "Created",
                "Product",
                Guid.NewGuid(),
                new { Name = "Test", Price = 10.0 });

            var messageBody = JsonSerializer.SerializeToUtf8Bytes(entityEvent);
            var propertiesMock = new Mock<IReadOnlyBasicProperties>();

            // Act
            await consumer.InvokeHandleMessageAsync(messageBody, propertiesMock.Object, CancellationToken.None);

            // Assert
            var invocation = loggerMock.Invocations.FirstOrDefault();
            var state = invocation?.Arguments[2];
            Assert.Contains("Event received: Created for Product", state?.ToString());
        }
    }
}