using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order.Application.Orders;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts;

namespace Order.Infrastructure.Messaging;

public sealed class StockResultConsumer(
    RabbitMqConnection connection,
    IServiceScopeFactory scopeFactory,
    ILogger<StockResultConsumer> logger) : BackgroundService
{
    private IModel? _channel;
    private readonly ResiliencePipeline _retryPipeline = new ResiliencePipelineBuilder()
        .AddRetry(new Polly.Retry.RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(2),
            BackoffType = DelayBackoffType.Exponential
        })
        .Build();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = connection.Connection.CreateModel();
        _channel.BasicQos(0, 1, false);
        StartConsumer(RabbitMqConstants.StockReservedQueue, reserved: true, stoppingToken);
        StartConsumer(RabbitMqConstants.StockFailedQueue, reserved: false, stoppingToken);
        return Task.CompletedTask;
    }

    private void StartConsumer(string queue, bool reserved, CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                await _retryPipeline.ExecuteAsync(async token =>
                {
                    using var scope = scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var json = Encoding.UTF8.GetString(args.Body.ToArray());

                    if (reserved)
                    {
                        var message = JsonSerializer.Deserialize<StockReservedEvent>(json)
                            ?? throw new InvalidOperationException("StockReservedEvent body is invalid.");
                        await mediator.Send(new ApplyStockResultCommand(message.OrderId, Reserved: true), token);
                    }
                    else
                    {
                        var message = JsonSerializer.Deserialize<StockFailedEvent>(json)
                            ?? throw new InvalidOperationException("StockFailedEvent body is invalid.");
                        await mediator.Send(new ApplyStockResultCommand(message.OrderId, Reserved: false, message.Reason), token);
                    }
                }, stoppingToken);

                _channel?.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Order service failed to process stock result message from {Queue}.", queue);
                _channel?.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel!.BasicConsume(queue, autoAck: false, consumer);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}
