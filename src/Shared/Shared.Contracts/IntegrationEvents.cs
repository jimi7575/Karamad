namespace Shared.Contracts;

public sealed record OrderCreatedEvent(Guid OrderId, Guid BookId, int Quantity);

public sealed record StockReservedEvent(Guid OrderId, Guid BookId, int Quantity);

public sealed record StockFailedEvent(Guid OrderId, Guid BookId, int Quantity, string Reason);

public static class RabbitMqConstants
{
    public const string ExchangeName = "bookstore.events";
    public const string OrderCreatedQueue = "catalog.order-created";
    public const string StockReservedQueue = "order.stock-reserved";
    public const string StockFailedQueue = "order.stock-failed";
    public const string OrderCreatedRoutingKey = "order.created";
    public const string StockReservedRoutingKey = "stock.reserved";
    public const string StockFailedRoutingKey = "stock.failed";
}
