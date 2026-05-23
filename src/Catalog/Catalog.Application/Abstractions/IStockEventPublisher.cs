using Shared.Contracts;

namespace Catalog.Application.Abstractions;

public interface IStockEventPublisher
{
    Task PublishStockReservedAsync(StockReservedEvent message, CancellationToken cancellationToken = default);
    Task PublishStockFailedAsync(StockFailedEvent message, CancellationToken cancellationToken = default);
}
