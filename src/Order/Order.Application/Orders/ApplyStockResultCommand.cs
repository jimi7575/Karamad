using MediatR;
using Order.Domain.Interfaces;

namespace Order.Application.Orders;

public sealed record ApplyStockResultCommand(Guid OrderId, bool Reserved, string? FailureReason = null) : IRequest;

public sealed class ApplyStockResultCommandHandler(IOrderRepository repository) : IRequestHandler<ApplyStockResultCommand>
{
    public async Task Handle(ApplyStockResultCommand request, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"Order {request.OrderId} was not found.");

        if (request.Reserved)
        {
            order.Confirm();
        }
        else
        {
            order.Fail(request.FailureReason ?? "Stock reservation failed.");
        }

        await repository.SaveChangesAsync(cancellationToken);
    }
}
