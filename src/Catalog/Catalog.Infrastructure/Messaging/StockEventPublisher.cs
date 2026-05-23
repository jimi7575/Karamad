using System.Text;
using System.Text.Json;
using Catalog.Application.Abstractions;
using RabbitMQ.Client;
using Shared.Contracts;

namespace Catalog.Infrastructure.Messaging;

public sealed class StockEventPublisher(RabbitMqConnection connection) : IStockEventPublisher
{
    public Task PublishStockReservedAsync(StockReservedEvent message, CancellationToken cancellationToken = default)
    {
        Publish(message, RabbitMqConstants.StockReservedRoutingKey);
        return Task.CompletedTask;
    }

    public Task PublishStockFailedAsync(StockFailedEvent message, CancellationToken cancellationToken = default)
    {
        Publish(message, RabbitMqConstants.StockFailedRoutingKey);
        return Task.CompletedTask;
    }

    private void Publish<T>(T message, string routingKey)
    {
        using var channel = connection.Connection.CreateModel();
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        channel.BasicPublish(RabbitMqConstants.ExchangeName, routingKey, properties, body);
    }
}
