using Shared.Contracts;

namespace Order.Application.Abstractions;

public interface IOrderEventPublisher
{
    Task PublishOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default);
}
