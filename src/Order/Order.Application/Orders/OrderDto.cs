using Order.Domain.Entities;

namespace Order.Application.Orders;

public sealed record OrderDto(Guid Id, Guid BookId, int Quantity, OrderStatus Status, string? FailureReason, DateTime CreatedAtUtc)
{
    public static OrderDto FromEntity(CustomerOrder order)
        => new(order.Id, order.BookId, order.Quantity, order.Status, order.FailureReason, order.CreatedAtUtc);
}
