using MediatR;
using Order.Domain.Interfaces;

namespace Order.Application.Orders;

public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

public sealed class GetOrderByIdQueryHandler(IOrderRepository repository) : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        return order is null ? null : OrderDto.FromEntity(order);
    }
}
