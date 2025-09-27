using CleanArchTemplate.Infrastructure.Messaging;
using Moq;
using RabbitMq.Messaging.Publisher;
using System.Text.Json;

namespace CleanArchTemplate.UnitTests.Infrastructure.Messaging
{
    public class MessagingServiceTests
    {
        [Fact]
        public async Task PublishMessage_CallsPublisherWithCorrectParameters()
        {
            // Arrange
            var publisherMock = new Mock<IRabbitMqPublisherService>();
            var service = new MessagingService(publisherMock.Object);

            var message = new { Name = "Test" };
            var exchangeName = "exchange";
            var headers = new Dictionary<string, string> { { "key", "value" } };
            var serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var exchangeType = "fanout";
            var routingKey = "route";

            publisherMock
                .Setup(p => p.PublishMessage(
                    message,
                    exchangeName,
                    headers,
                    serializerOptions,
                    exchangeType,
                    routingKey))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await service.PublishMessage(message, exchangeName, headers, serializerOptions, exchangeType, routingKey);

            // Assert
            publisherMock.Verify(p => p.PublishMessage(
                message,
                exchangeName,
                headers,
                serializerOptions,
                exchangeType,
                routingKey), Times.Once);
        }

        [Fact]
        public async Task PublishMessage_AllowsNullOptionalParameters()
        {
            // Arrange
            var publisherMock = new Mock<IRabbitMqPublisherService>();
            var service = new MessagingService(publisherMock.Object);

            var message = "simple";
            var exchangeName = "exchange";

            publisherMock
                .Setup(p => p.PublishMessage(
                    message,
                    exchangeName,
                    null,
                    null,
                    "fanout",
                    ""))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await service.PublishMessage(message, exchangeName);

            // Assert
            publisherMock.Verify(p => p.PublishMessage(
                message,
                exchangeName,
                null,
                null,
                "fanout",
                ""), Times.Once);
        }
    }
}