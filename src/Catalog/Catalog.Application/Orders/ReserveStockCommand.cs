using Catalog.Application.Abstractions;
using Catalog.Domain.Interfaces;
using MediatR;
using Shared.Contracts;

namespace Catalog.Application.Orders;

public sealed record ReserveStockCommand(Guid OrderId, Guid BookId, int Quantity) : IRequest;

public sealed class ReserveStockCommandHandler(IBookRepository repository, IBookCache cache, IStockEventPublisher publisher)
    : IRequestHandler<ReserveStockCommand>
{
    public async Task Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var book = await repository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            await publisher.PublishStockFailedAsync(new StockFailedEvent(request.OrderId, request.BookId, request.Quantity, "Book was not found."), cancellationToken);
            return;
        }

        if (!book.TryReserve(request.Quantity))
        {
            await publisher.PublishStockFailedAsync(new StockFailedEvent(request.OrderId, request.BookId, request.Quantity, "Insufficient stock."), cancellationToken);
            return;
        }

        await repository.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(request.BookId, cancellationToken);
        await publisher.PublishStockReservedAsync(new StockReservedEvent(request.OrderId, request.BookId, request.Quantity), cancellationToken);
    }
}
