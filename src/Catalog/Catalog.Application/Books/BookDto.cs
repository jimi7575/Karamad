using Catalog.Domain.Entities;

namespace Catalog.Application.Books;

public sealed record BookDto(Guid Id, string Title, string Author, int Stock, decimal Price)
{
    public static BookDto FromEntity(Book book) => new(book.Id, book.Title, book.Author, book.Stock, book.Price);
}
