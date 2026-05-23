using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using Shared.Contracts;

namespace Catalog.Infrastructure.Messaging;

public sealed class RabbitMqConnection : IDisposable
{
    public IConnection Connection { get; }

    public RabbitMqConnection(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"] ?? "localhost",
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };

        Connection = CreateConnectionWithRetry(factory);
        using var channel = Connection.CreateModel();
        channel.ExchangeDeclare(RabbitMqConstants.ExchangeName, ExchangeType.Direct, durable: true);
        channel.QueueDeclare(RabbitMqConstants.OrderCreatedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueDeclare(RabbitMqConstants.StockReservedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueDeclare(RabbitMqConstants.StockFailedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(RabbitMqConstants.OrderCreatedQueue, RabbitMqConstants.ExchangeName, RabbitMqConstants.OrderCreatedRoutingKey);
        channel.QueueBind(RabbitMqConstants.StockReservedQueue, RabbitMqConstants.ExchangeName, RabbitMqConstants.StockReservedRoutingKey);
        channel.QueueBind(RabbitMqConstants.StockFailedQueue, RabbitMqConstants.ExchangeName, RabbitMqConstants.StockFailedRoutingKey);
    }

    public void Dispose() => Connection.Dispose();

    private static IConnection CreateConnectionWithRetry(ConnectionFactory factory)
    {
        for (var attempt = 1; attempt <= 30; attempt++)
        {
            try
            {
                return factory.CreateConnection();
            }
            catch (BrokerUnreachableException) when (attempt < 30)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        return factory.CreateConnection();
    }
}
