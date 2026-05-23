namespace Order.Domain.Entities;

public sealed class CustomerOrder
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public int Quantity { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }
    public string? FailureReason { get; private set; }

    private CustomerOrder()
    {
    }

    public CustomerOrder(Guid bookId, int quantity)
    {
        if (bookId == Guid.Empty) throw new ArgumentException("BookId is required.", nameof(bookId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        BookId = bookId;
        Quantity = quantity;
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        FailureReason = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Fail(string reason)
    {
        Status = OrderStatus.Failed;
        FailureReason = reason;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
