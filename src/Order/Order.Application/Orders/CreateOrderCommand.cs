using MediatR;
using Order.Application.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Interfaces;
using Shared.Contracts;

namespace Order.Application.Orders;

public sealed record CreateOrderCommand(Guid BookId, int Quantity) : IRequest<OrderDto>;

public sealed class CreateOrderCommandHandler(IOrderRepository repository, IOrderEventPublisher publisher)
    : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new CustomerOrder(request.BookId, request.Quantity);
        await repository.AddAsync(order, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await publisher.PublishOrderCreatedAsync(new OrderCreatedEvent(order.Id, order.BookId, order.Quantity), cancellationToken);
        return OrderDto.FromEntity(order);
    }
}
