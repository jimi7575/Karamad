using System.Text;
using System.Text.Json;
using Order.Application.Abstractions;
using RabbitMQ.Client;
using Shared.Contracts;

namespace Order.Infrastructure.Messaging;

public sealed class OrderEventPublisher(RabbitMqConnection connection) : IOrderEventPublisher
{
    public Task PublishOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
    {
        using var channel = connection.Connection.CreateModel();
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        channel.BasicPublish(RabbitMqConstants.ExchangeName, RabbitMqConstants.OrderCreatedRoutingKey, properties, body);
        return Task.CompletedTask;
    }
}
