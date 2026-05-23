namespace Catalog.Domain.Entities;

public sealed class Book
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public decimal Price { get; private set; }

    private Book()
    {
    }

    public Book(string title, string author, int stock, decimal price)
    {
        Update(title, author, stock, price);
    }

    public void Update(string title, string author, int stock, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author is required.", nameof(author));
        if (stock < 0) throw new ArgumentOutOfRangeException(nameof(stock), "Stock cannot be negative.");
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        Title = title.Trim();
        Author = author.Trim();
        Stock = stock;
        Price = price;
    }

    public bool TryReserve(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        if (Stock < quantity) return false;

        Stock -= quantity;
        return true;
    }
}
