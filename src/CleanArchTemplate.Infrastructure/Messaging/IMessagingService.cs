using RabbitMQ.Client;
using System.Text.Json;

namespace CleanArchTemplate.Infrastructure.Messaging
{
    public interface IMessagingService
    {
        Task PublishMessage<T>(
            T message,
            string exchangeName,
            Dictionary<string, string>? headers = null,
            JsonSerializerOptions? serializerOptions = null,
            string exchangeType = ExchangeType.Fanout,
            string routingKey = "");
    }
}
