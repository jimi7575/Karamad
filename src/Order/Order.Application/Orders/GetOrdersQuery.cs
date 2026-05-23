using MediatR;
using Order.Domain.Interfaces;

namespace Order.Application.Orders;

public sealed record GetOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;

public sealed class GetOrdersQueryHandler(IOrderRepository repository) : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await repository.GetAllAsync(cancellationToken);
        return orders.Select(OrderDto.FromEntity).ToList();
    }
}
