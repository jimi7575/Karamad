using System.Text;
using System.Text.Json;
using Catalog.Application.Orders;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts;

namespace Catalog.Infrastructure.Messaging;

public sealed class OrderCreatedConsumer(
    RabbitMqConnection connection,
    IServiceScopeFactory scopeFactory,
    ILogger<OrderCreatedConsumer> logger) : BackgroundService
{
    private IModel? _channel;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = connection.Connection.CreateModel();
        _channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(json)
                    ?? throw new InvalidOperationException("OrderCreatedEvent body is invalid.");

                using var scope = scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new ReserveStockCommand(message.OrderId, message.BookId, message.Quantity), stoppingToken);
                _channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Catalog failed to process OrderCreatedEvent.");
                _channel?.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(RabbitMqConstants.OrderCreatedQueue, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}
