using RabbitMq.Messaging.Publisher;
using System.Text.Json;

namespace CleanArchTemplate.Infrastructure.Messaging
{
    public class MessagingService : IMessagingService
    {
        private readonly IRabbitMqPublisherService _publisher;
        public MessagingService(IRabbitMqPublisherService publisher)
        {
            _publisher = publisher;
        }
        public async Task PublishMessage<T>(
            T message,
            string exchangeName,
            Dictionary<string, string>? headers = null,
            JsonSerializerOptions? serializerOptions = null,
            string exchangeType = "fanout",
            string routingKey = "")
        {
            await _publisher.PublishMessage(
                message,
                exchangeName,
                headers,
                serializerOptions,
                exchangeType,
                routingKey);
        }
    }
}
